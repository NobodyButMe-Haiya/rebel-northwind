using System.Text;
using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Administrator;
using RebelCmsTemplate.Models.Shared;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Repository.Administrator;

public class RoleRepository
{
    private readonly SharedUtil _sharedUtil;

    public RoleRepository(IHttpContextAccessor httpContextAccessor)
    {
        _sharedUtil = new SharedUtil(httpContextAccessor);
    }

    public int Create(RoleModel roleModel)
    {
        // okay next we create skeleton for the code
        int lastInsertKey;
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();

            var mySqlTransaction = connection.BeginTransaction();

            sql += @"
                INSERT INTO role (roleId,tenantId, roleName, isDelete) VALUES (null,@tenantId,@roleName,0);";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@tenantId",
                    Value = _sharedUtil.GetTenantId()
                },
                new()
                {
                    Key = "@roleName",
                    Value = roleModel.RoleName
                }
            };
            foreach (var parameter in parameterModels)
            {
                mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            mySqlCommand.ExecuteNonQuery();


            lastInsertKey = (int) mySqlCommand.LastInsertedId;

            mySqlCommand.Dispose();
            SetLeafAccess(connection, lastInsertKey);
            mySqlTransaction.Commit();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
            throw new Exception(ex.Message);
        }

        return lastInsertKey;
    }

    public List<RoleModel> Read()
    {
        List<RoleModel> roleModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
                SELECT      *
                FROM        role
                WHERE       tenantId = @tenantId
                AND         isDelete !=1
                ORDER BY    roleId DESC ";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@tenantId",
                    Value = _sharedUtil.GetTenantId()
                }
            };
            foreach (var parameter in parameterModels)
            {
                mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            _sharedUtil.SetSqlSession(sql, parameterModels);

            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    roleModels.Add(new RoleModel
                    {
                        RoleName = reader["roleName"].ToString(),
                        RoleKey = Convert.ToUInt32(reader["roleId"])
                    });
                }
            }

            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
            throw new Exception(ex.Message);
        }


        return roleModels;
    }

    public List<RoleModel> Search(string search)
    {
        List<RoleModel> roleModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
                SELECT  *
                FROM    role
                WHERE   tenantId = @tenantId
                AND     isDelete != 1
                AND     roleName LIKE CONCAT('%',@search,'%'); ";
            MySqlCommand mySqlCommand = new(sql, connection);

            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@tenantId",
                    Value = _sharedUtil.GetTenantId()
                },
                new()
                {
                    Key = "@search",
                    Value = search
                }
            };
            foreach (var parameter in parameterModels)
            {
                mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            _sharedUtil.SetSqlSession(sql, parameterModels);

            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    roleModels.Add(new RoleModel
                    {
                        RoleName = reader["roleName"].ToString(),
                        RoleKey = Convert.ToUInt32(reader["roleId"])
                    });
                }
            }

            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
            throw new Exception(ex.Message);
        }


        return roleModels;
    }
    public uint GetDefault()
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        uint roleId;
        try
        {
            connection.Open();
            sql += @"
            SELECT  roleId
            FROM    role
            WHERE   tenantId = @tenantId
            AND     isDelete != 1
            AND     isDefault = 1
            LIMIT   1";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@tenantId",
                    Value = _sharedUtil.GetTenantId()
                }
            };
            foreach (var parameter in parameterModels)
            {
                mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            _sharedUtil.SetSqlSession(sql, parameterModels);

            roleId = (uint)mySqlCommand.ExecuteScalar();

            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
            throw new Exception(ex.Message);
        }


        return roleId;
    }
    public byte[] GetExcel()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Administrator > Role");
        worksheet.Cell(1, 1).Value = "No";
        worksheet.Cell(1, 2).Value = "Role";
        var sql = _sharedUtil.GetSqlSession();
        var parameterModels = _sharedUtil.GetListSqlParameter();
        using var connection = SharedUtil.GetConnection();

        try
        {
            connection.Open();
            MySqlCommand mySqlCommand = new(sql, connection);
            if (parameterModels != null)
            {
                foreach (var parameter in parameterModels)
                {
                    mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
            }

            using (var reader = mySqlCommand.ExecuteReader())
            {
                var counter = 1;
                while (reader.Read())
                {
                    var currentRow = counter++;
                    worksheet.Cell(currentRow, 1).Value = counter - 1;
                    worksheet.Cell(currentRow, 2).Value = reader["roleName"].ToString();
                }
            }

            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            throw new Exception(ex.Message);
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public void Update(RoleModel roleModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();

            sql += @"
            UPDATE  role
            SET     roleName    =   @roleName
            WHERE   roleId      =   @roleId ";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@roleName",
                    Value = roleModel.RoleName
                },
                new()
                {
                    Key = "@executeBy",
                    Value = _sharedUtil.GetUserName()
                },
                new()
                {
                    Key = "@roleId",
                    Value = roleModel.RoleKey
                }
            };
            foreach (var parameter in parameterModels)
            {
                mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            mySqlCommand.ExecuteNonQuery();
            mySqlTransaction.Commit();
            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
            throw new Exception(ex.Message);
        }
    }

    public void Delete(RoleModel roleModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();

            sql += @"
            UPDATE  role
            SET     isDelete = 1
            WHERE   roleId  = @roleId;";
            MySqlCommand mySqlCommand = new(sql, connection);

            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@roleId",
                    Value = roleModel.RoleKey
                }
            };
            foreach (var parameter in parameterModels)
            {
                mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            mySqlCommand.ExecuteNonQuery();

            mySqlTransaction.Commit();

            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
            throw new Exception(ex.Message);
        }
    }

    private static void SetLeafAccess(MySqlConnection connection, int roleId)
    {
        StringBuilder stringBuilder = new();
        try
        {
            const string sql = @"SELECT * FROM leaf WHERE isDelete != 1 ";
            MySqlCommand mySqlCommand = new(sql, connection);

            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    stringBuilder.Append("(null, " + reader["leafId"] + ", " + roleId + ", 0, 0, 0, 0, 0, 0),");
                }
            }

            mySqlCommand.Dispose();
            var sqlValues = stringBuilder.ToString().TrimEnd(',');
            // re loop  the access  
            var sqlConcat =
                $@" INSERT INTO leaf_access (
                        leafAccessId,           leafId,                     roleId, 
                        leafAccessCreateValue,  leafAccessReadValue,        leafAccessUpdateValue,
                        leafAccessDeleteValue,  leafAccessExtraOneValue,    leafAccessExtraTwoValue
                    ) VALUES {sqlValues}";

            mySqlCommand = new MySqlCommand(sqlConcat, connection);
            mySqlCommand.ExecuteNonQuery();
            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            throw new Exception(ex.Message);
        }
    }
}