using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Menu;
using RebelCmsTemplate.Models.Shared;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Repository.Menu;

public class LeafAccessRepository
{
    private readonly SharedUtil _sharedUtil;

    public LeafAccessRepository(IHttpContextAccessor httpContextAccessor)
    {
        _sharedUtil = new SharedUtil(httpContextAccessor);
    }

    public List<LeafAccessModel> Read(int roleId = 0, int folderId = 0, int leafId = 0)
    {
        List<LeafAccessModel> leafAccessModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
            SELECT  * 
            FROM    leaf_access 
            JOIN    leaf
            USING(leafId) 
            JOIN    role
            USING(tenantId,roleId) 
            JOIN    folder 
            USING   (tenantId,folderId) 
            WHERE   leaf.tenantId = @tenantId 
            AND     role.isDelete != 1
            AND     folder.isDelete != 1 
            AND     leaf.isDelete != 1 ";
            if (roleId > 0)
            {
                sql += " AND roleId  = @roleId ";
            }

            if (folderId > 0)
            {
                sql += " AND folderId  = @folderId ";
            }

            if (leafId > 0)
            {
                sql += " AND leafId  = @leafId ";
            }

            MySqlCommand mySqlCommand = new(sql, connection);
            if (roleId > 0)
            {
                parameterModels.Add(new ParameterModel
                {
                    Key = "@roleId",
                    Value = roleId
                });
            }

            if (folderId > 0)
            {
                parameterModels.Add(new ParameterModel
                {
                    Key = "@folderId",
                    Value = folderId
                });
            }

            if (leafId > 0)
            {
                parameterModels.Add(new ParameterModel
                {
                    Key = "@leafId",
                    Value = leafId
                });
            }

            parameterModels.Add(new ParameterModel
            {
                Key = "@tenantId",
                Value = _sharedUtil.GetTenantId()
            });
            foreach (var parameter in parameterModels)
            {
                mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    leafAccessModels.Add(new LeafAccessModel
                    {
                        LeafAccessKey = Convert.ToUInt32(reader["leafAccessId"]),
                        LeafAccessCreateValue = Convert.ToInt32(reader["leafAccessCreateValue"]),
                        LeafAccessReadValue = Convert.ToInt32(reader["leafAccessReadValue"]),
                        LeafAccessUpdateValue = Convert.ToInt32(reader["leafAccessUpdateValue"]),
                        LeafAccessDeleteValue = Convert.ToInt32(reader["leafAccessDeleteValue"]),
                        LeafAccessExtraOneValue = Convert.ToInt32(reader["LeafAccessExtraOneValue"]),
                        LeafAccessExtraTwoValue = Convert.ToInt32(reader["LeafAccessExtraTwoValue"]),
                        RoleName = reader["roleName"].ToString(),
                        LeafName = reader["leafName"].ToString(),
                        FolderName = reader["folderName"].ToString(),
                        RoleKey = Convert.ToUInt32(reader["roleId"]),
                        LeafKey = Convert.ToUInt32(reader["leafId"])
                    });
                }
            }

            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetSystemException(ex);
            throw new Exception(ex.Message);
        }


        return leafAccessModels;
    }

    public void Update(List<LeafAccessModel> leafAccessModels)
    {
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();

            List<uint> primaryKeyAll = new();
            List<string> access = new()
            {
                "leafAccessCreateValue",
                "leafAccessReadValue",
                "leafAccessUpdateValue",
                "leafAccessDeleteValue"
            };
            var sql = @"UPDATE  `leaf_access`    ";
            sql += " SET";
            foreach (var fieldNameAccess in access)
            {
                switch (fieldNameAccess)
                {
                    case "leafAccessCreateValue":
                        sql += "`leafAccessCreateValue`			= CASE `leafAccessId` ";
                        foreach (var leafAccessModel in leafAccessModels)
                        {
                            sql += " WHEN  " + leafAccessModel.LeafAccessKey;
                            sql += " THEN  " + leafAccessModel.LeafAccessCreateValue;
                            primaryKeyAll.Add(leafAccessModel.LeafAccessKey);
                        }

                        sql += " ELSE " + fieldNameAccess.ToUpper() + " END,";
                        break;
                    case "leafAccessReadValue":
                        sql += "`leafAccessReadValue`			= CASE `leafAccessId` ";
                        foreach (var leafAccessModel in leafAccessModels)
                        {
                            sql += " WHEN  " + leafAccessModel.LeafAccessKey;
                            sql += " THEN  " + leafAccessModel.LeafAccessReadValue;
                            primaryKeyAll.Add(leafAccessModel.LeafAccessKey);
                        }

                        sql += " ELSE " + fieldNameAccess.ToUpper() + " END,";
                        break;
                    case "leafAccessUpdateValue":
                        sql += "`leafAccessUpdateValue`			= CASE `leafAccessId` ";
                        foreach (var leafAccessModel in leafAccessModels)
                        {
                            sql += " WHEN  " + leafAccessModel.LeafAccessKey;
                            sql += " THEN  " + leafAccessModel.LeafAccessUpdateValue;
                            primaryKeyAll.Add(leafAccessModel.LeafAccessKey);
                        }

                        sql += " ELSE " + fieldNameAccess.ToUpper() + " END,";
                        break;
                    case "leafAccessDeleteValue":
                        sql += "`leafAccessDeleteValue`			= CASE `leafAccessId` ";
                        foreach (var leafAccessModel in leafAccessModels)
                        {
                            sql += " WHEN  " + leafAccessModel.LeafAccessKey;
                            sql += " THEN  " + leafAccessModel.LeafAccessCreateValue;
                            primaryKeyAll.Add(leafAccessModel.LeafAccessKey);
                        }

                        sql += " ELSE  " + fieldNameAccess.ToUpper() + " END";
                        break;
                }
            }

            sql += " WHERE 	`leafAccessId`		IN	( " + string.Join(",", primaryKeyAll) + ")";
            MySqlCommand mySqlCommand = new(sql, connection);

            mySqlCommand.ExecuteNonQuery();
            mySqlTransaction.Commit();
            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetSystemException(ex);
            throw new Exception(ex.Message);
        }
    }
}