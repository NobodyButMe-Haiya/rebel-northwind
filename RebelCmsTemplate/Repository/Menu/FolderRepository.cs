using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Menu;
using RebelCmsTemplate.Models.Shared;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Repository.Menu;

public class FolderRepository
{
    private readonly SharedUtil _sharedUtil;

    public FolderRepository(IHttpContextAccessor httpContextAccessor)
    {
        _sharedUtil = new SharedUtil(httpContextAccessor);
    }

    public int Create(FolderModel folderModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        // okay next we create skeleton for the code
        int lastInsertKey;
        try
        {
            connection.Open();

            var
                mySqlTransaction = connection.BeginTransaction();

            sql += @"
            INSERT INTO folder
            ( 
                folderId,           folderName, 
                folderFilename,     folderIcon, 
                folderSeq,          isDelete,
                tenantId
            )VALUES(
                NULL,               @folderName,
                @folderFilename,    @folderIcon,
                @folderSeq,         0,
                @tenantId
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
                    Key = "@folderName",
                    Value = folderModel.FolderName
                },
                new()
                {
                    Key = "@folderFilename",
                    Value = folderModel.FolderFilename
                },
                new()
                {
                    Key = "@folderIcon",
                    Value = folderModel.FolderIcon
                },
                new()
                {
                    Key = "@folderSeq",
                    Value = folderModel.FolderSeq
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

    public List<FolderModel> Read()
    {
        List<FolderModel> folderModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
            SELECT      *
            FROM        folder
            WHERE       tenantId = @tenantId
            AND         isDelete !=1
            ORDER BY    folderSeq  ";
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
                    folderModels.Add(new FolderModel
                    {
                        FolderKey = Convert.ToUInt32(reader["folderId"]),
                        FolderName = reader["folderName"].ToString(),
                        FolderFilename = reader["folderFilename"].ToString(),
                        FolderSeq = Convert.ToInt32(reader["folderSeq"]),
                        FolderIcon = reader["folderIcon"].ToString(),
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


        return folderModels;
    }

    public List<FolderModel> Search(string search)
    {
        List<FolderModel> folderModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
            SELECT  *
            FROM    folder
            WHERE   tenantId= @tenantId 
            AND     isDelete != 1
            AND     folderName  LIKE CONCAT('%',@search,'%'); ";
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
                    folderModels.Add(new FolderModel
                    {
                        FolderKey = Convert.ToUInt32(reader["FolderId"]),
                        FolderName = reader["folderName"].ToString(),
                        FolderFilename = reader["folderFilename"].ToString(),
                        FolderSeq = Convert.ToInt32(reader["folderSeq"]),
                        FolderIcon = reader["folderIcon"].ToString(),
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


        return folderModels;
    }

    public void Update(FolderModel folderModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();

            sql += @"
            UPDATE  folder
            SET     folderName      =   @folderName,
                    folderFilename  =   @folderFilename,
                    folderIcon      =   @folderIcon,
                    folderSeq       =   @folderSeq
            WHERE   folderId        =   @folderId ";

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
                    Key = "@folderName",
                    Value = folderModel.FolderName
                },
                new()
                {
                    Key = "@folderFilename",
                    Value = folderModel.FolderFilename
                },
                new()
                {
                    Key = "@folderIcon",
                    Value = folderModel.FolderIcon
                },
                new()
                {
                    Key = "@folderSeq",
                    Value = folderModel.FolderSeq
                },
                new()
                {
                    Key = "@folderId",
                    Value = folderModel.FolderKey
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

    public void Delete(FolderModel folderModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();

            sql += @"
            UPDATE  folder
            SET     isDelete = 1
            WHERE   folderId  = @folderId;";
            MySqlCommand mySqlCommand = new(sql, connection);

            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@folderId",
                    Value = folderModel.FolderKey
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