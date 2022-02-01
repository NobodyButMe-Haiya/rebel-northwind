using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Application;
using RebelCmsTemplate.Models.Shared;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Repository.Application;

public class ShipperRepository
{
    private readonly SharedUtil _sharedUtil;

    public ShipperRepository(IHttpContextAccessor httpContextAccessor)
    {
        _sharedUtil = new SharedUtil(httpContextAccessor);
    }

    public int Create(ShipperModel shipperModel)
    {
        int lastInsertKey;
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();
            sql +=
                @"INSERT INTO shipper (shipperId,tenantId,shipperName,shipperPhone,isDelete) VALUES (null,@tenantId,@shipperName,@shipperPhone,@isDelete);";
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
                    Key = "@shipperName",
                    Value = shipperModel.ShipperName
                },
                new()
                {
                    Key = "@shipperPhone",
                    Value = shipperModel.ShipperPhone
                },
                new()
                {
                    Key = "@isDelete",
                    Value = 0
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

    public List<ShipperModel> Read()
    {
        List<ShipperModel> shipperModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql = @"
                SELECT      *
                FROM        shipper 
                WHERE       isDelete !=1
                AND         tenantId   = @tenantId
                ORDER BY    shipperId DESC ";
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
            _sharedUtil.SetSqlSession(sql, parameterModels);
            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    shipperModels.Add(new ShipperModel
                    {
                        ShipperKey = Convert.ToUInt32(reader["shipperId"]),
                        TenantKey = Convert.ToUInt32(reader["tenantId"]),
                        ShipperName = reader["shipperName"].ToString(),
                        ShipperPhone = reader["shipperPhone"].ToString(),
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

        return shipperModels;
    }

    public List<ShipperModel> Search(string search)
    {
        List<ShipperModel> shipperModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
                SELECT  *
                FROM    shipper 
	 WHERE   shipper.isDelete != 1
	 AND (
	 shipper.shipperName LIKE CONCAT('%',@search,'%') OR
	 shipper.shipperPhone LIKE CONCAT('%',@search,'%') )";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
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
                    shipperModels.Add(new ShipperModel
                    {
                        ShipperKey = Convert.ToUInt32(reader["shipperId"]),
                        TenantKey = Convert.ToUInt32(reader["tenantId"]),
                        ShipperName = reader["shipperName"].ToString(),
                        ShipperPhone = reader["shipperPhone"].ToString(),
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

        return shipperModels;
    }

    public ShipperModel GetSingle(ShipperModel shipperModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
                SELECT  *
                FROM    shipper 
                WHERE   shipper.isDelete != 1
                AND   shipper.shipperId    =   @shipperId LIMIT 1";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@shipperId",
                    Value = shipperModel.ShipperKey
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
                    shipperModel = new ShipperModel
                    {
                        ShipperKey = Convert.ToUInt32(reader["shipperId"]),
                        TenantKey = Convert.ToUInt32(reader["tenantId"]),
                        ShipperName = reader["shipperName"].ToString(),
                        ShipperPhone = reader["shipperPhone"].ToString(),
                        IsDelete = Convert.ToInt32(reader["isDelete"])
                    };
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

        return shipperModel;
    }

    public uint GetDefault()
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        uint shipperId;
        try
        {
            connection.Open();
            sql += @"
            SELECT  shipperId
            FROM    shipper 
            WHERE   isDelete != 1
            AND     isDefault = 1
            AND     tenantId  = @tenantId
            LIMIT 1";
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
            shipperId = (uint) (long) mySqlCommand.ExecuteScalar();

            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
            throw new Exception(ex.Message);
        }

        return shipperId;
    }

    public byte[] GetExcel()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Administrator > Shipper ");
        worksheet.Cell(1, 1).Value = "shipperId";
        worksheet.Cell(1, 2).Value = "tenantId";
        worksheet.Cell(1, 3).Value = "shipperName";
        worksheet.Cell(1, 4).Value = "shipperPhone";
        worksheet.Cell(1, 5).Value = "isDelete";
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
                var counter = 3;
                while (reader.Read())
                {
                    var currentRow = counter++;
                    worksheet.Cell(currentRow, 1).Value = reader["shipperId"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["tenantId"].ToString();
                    worksheet.Cell(currentRow, 3).Value = reader["shipperName"].ToString();
                    worksheet.Cell(currentRow, 4).Value = reader["shipperPhone"].ToString();
                    worksheet.Cell(currentRow, 5).Value = reader["isDelete"].ToString();
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

    public void Update(ShipperModel shipperModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();
            sql = @"
            UPDATE  shipper 
            SET     tenantId        =   @tenantId,
                    shipperName     =   @shipperName,
                    shipperPhone    =   @shipperPhone,
                    isDelete        =   @isDelete
            WHERE   shipperId       =   @shipperId";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@shipperId",
                    Value = shipperModel.ShipperKey
                },
                new()
                {
                    Key = "@tenantId",
                    Value = _sharedUtil.GetTenantId()
                },
                new()
                {
                    Key = "@shipperName",
                    Value = shipperModel.ShipperName
                },
                new()
                {
                    Key = "@shipperPhone",
                    Value = shipperModel.ShipperPhone
                },
                new()
                {
                    Key = "@isDelete",
                    Value = 0
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

    public void Delete(ShipperModel shipperModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();
            sql = @"
            UPDATE  shipper 
            SET     isDelete    =   1
            WHERE   shipperId   =   @shipperId";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@shipperId",
                    Value = shipperModel.ShipperKey
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