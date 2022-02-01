using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Menu;
using RebelCmsTemplate.Models.Shared;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Repository.Menu;

public class LeafRepository
{
    private readonly SharedUtil _sharedUtil;

    public LeafRepository(IHttpContextAccessor httpContextAccessor)
    {
        _sharedUtil = new SharedUtil(httpContextAccessor);
    }

    public int Create(LeafModel leafModel)
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

            sql += @"INSERT INTO leaf
                ( 
                    leafId,         leafName,   folderId, 
                    leafFilename,   leafIcon,   leafSeq,
                    isDelete,       tenantId
                )VALUES(
                    NULL,           @leafName,  @folderId,
                    @leafFilename,  @leafIcon,  @leafSeq,
                    0,              @tenantId
                )";

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
                    Key = "@folderId",
                    Value = leafModel.FolderKey
                },
                new()
                {
                    Key = "@leafName",
                    Value = leafModel.LeafName
                },
                new()
                {
                    Key = "@leafFilename",
                    Value = leafModel.LeafFilename
                },
                new()
                {
                    Key = "@leafIcon",
                    Value = leafModel.LeafIcon
                },
                new()
                {
                    Key = "@leafSeq",
                    Value = leafModel.LeafSeq
                }
            };
            foreach (var parameter in parameterModels)
            {
                mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            mySqlCommand.ExecuteNonQuery();

            mySqlTransaction.Commit();

            lastInsertKey = (int) mySqlCommand.LastInsertedId;

            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
            throw new Exception(ex.Message);
        }

        return lastInsertKey;
    }

    public List<LeafModel> Read()
    {
        List<LeafModel> leafModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
            SELECT      *
            FROM        leaf
            WHERE       tenantId = @tenantId
            AND         isDelete  !=1
            ORDER BY    leafSeq ";
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
                    leafModels.Add(new LeafModel
                    {
                        LeafKey = Convert.ToUInt32(reader["LeafId"]),
                        FolderKey = Convert.ToUInt32(reader["folderId"]),
                        LeafName = reader["leafName"].ToString(),
                        LeafFilename = reader["leafFilename"].ToString(),
                        LeafSeq = Convert.ToInt32(reader["leafSeq"]),
                        LeafIcon = reader["leafIcon"].ToString(),
                        IsDelete = Convert.ToInt32(reader["isDelete"])
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


        return leafModels;
    }

    public List<LeafModel> Search(string search)
    {
        List<LeafModel> leafModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
            SELECT  *
            FROM    leaf
            JOIN    folder 
            USING   (folderId)
            WHERE   leaf.tenantId = @tenantId
            AND     leaf.isDelete != 1
            AND     folder.isDelete != 1  
            AND     leafName LIKE CONCAT('%',@search,'%') 
            OR      folderName LIKE CONCAT('%',@search,'%') ; ";
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
                    leafModels.Add(new LeafModel
                    {
                        LeafKey = Convert.ToUInt32(reader["LeafId"]),
                        FolderKey = Convert.ToUInt32(reader["folderId"]),
                        LeafName = reader["leafName"].ToString(),
                        LeafFilename = reader["leafFilename"].ToString(),
                        LeafSeq = Convert.ToInt32(reader["leafSeq"]),
                        LeafIcon = reader["leafIcon"].ToString(),
                        IsDelete = Convert.ToInt32(reader["isDelete"])
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


        return leafModels;
    }

    public void Update(LeafModel leafModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();

            sql += @"
            UPDATE  leaf
            SET     folderId        =   @folderId,
                    leafName        =   @leafName,
                    leafFilename    =   @leafFilename,
                    leafIcon        =   @leafIcon,
                    leafSeq         =   @leafSeq
            WHERE   leafId          =   @leafId ";

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
                    Key = "@folderId",
                    Value = leafModel.FolderKey
                },
                new()
                {
                    Key = "@leafName",
                    Value = leafModel.LeafName
                },
                new()
                {
                    Key = "@leafFilename",
                    Value = leafModel.LeafFilename
                },
                new()
                {
                    Key = "@leafIcon",
                    Value = leafModel.LeafIcon
                },
                new()
                {
                    Key = "@leafSeq",
                    Value = leafModel.LeafSeq
                },
                new()
                {
                    Key = "@leafId",
                    Value = leafModel.LeafKey
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

    public void Delete(LeafModel leafModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();

            sql += @"
            UPDATE  leaf
            SET     isDelete    =   1
            WHERE   leafId      =   @leafId;";
            MySqlCommand mySqlCommand = new(sql, connection);

            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@leafId",
                    Value = leafModel.LeafKey
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
}