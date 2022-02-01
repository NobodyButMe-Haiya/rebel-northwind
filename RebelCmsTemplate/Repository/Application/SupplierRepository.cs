using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Application;
using RebelCmsTemplate.Models.Shared;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Repository.Application;

public class SupplierRepository
{
    private readonly SharedUtil _sharedUtil;

    public SupplierRepository(IHttpContextAccessor httpContextAccessor)
    {
        _sharedUtil = new SharedUtil(httpContextAccessor);
    }

    public int Create(SupplierModel supplierModel)
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
                @"INSERT INTO supplier (supplierId,tenantId,supplierName,supplierContactName,supplierContactTitle,supplierAddress,supplierCity,supplierRegion,supplierPostalCode,supplierCountry,supplierPhone,supplierFax,supplierHomePage,isDelete,isDefault) VALUES (null,@tenantId,@supplierName,@supplierContactName,@supplierContactTitle,@supplierAddress,@supplierCity,@supplierRegion,@supplierPostalCode,@supplierCountry,@supplierPhone,@supplierFax,@supplierHomePage,@isDelete,@isDefault);";
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
                    Key = "@supplierName",
                    Value = supplierModel.SupplierName
                },
                new()
                {
                    Key = "@supplierContactName",
                    Value = supplierModel.SupplierContactName
                },
                new()
                {
                    Key = "@supplierContactTitle",
                    Value = supplierModel.SupplierContactTitle
                },
                new()
                {
                    Key = "@supplierAddress",
                    Value = supplierModel.SupplierAddress
                },
                new()
                {
                    Key = "@supplierCity",
                    Value = supplierModel.SupplierCity
                },
                new()
                {
                    Key = "@supplierRegion",
                    Value = supplierModel.SupplierRegion
                },
                new()
                {
                    Key = "@supplierPostalCode",
                    Value = supplierModel.SupplierPostalCode
                },
                new()
                {
                    Key = "@supplierCountry",
                    Value = supplierModel.SupplierCountry
                },
                new()
                {
                    Key = "@supplierPhone",
                    Value = supplierModel.SupplierPhone
                },
                new()
                {
                    Key = "@supplierFax",
                    Value = supplierModel.SupplierFax
                },
                new()
                {
                    Key = "@supplierHomePage",
                    Value = supplierModel.SupplierHomePage
                },
                new()
                {
                    Key = "@isDelete",
                    Value = 0
                },
                new()
                {
                    Key = "@isDefault",
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

    public List<SupplierModel> Read()
    {
        List<SupplierModel> supplierModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql = @"
            SELECT      *
            FROM        supplier 
            WHERE       supplier.isDelete != 1
            AND         tenantId = @tenantId
            ORDER BY    supplierId DESC ";
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
                    supplierModels.Add(new SupplierModel
                    {
                        SupplierKey = Convert.ToUInt32(reader["supplierId"]),
                        SupplierName = reader["supplierName"].ToString(),
                        SupplierContactName = reader["supplierContactName"].ToString(),
                        SupplierContactTitle = reader["supplierContactTitle"].ToString(),
                        SupplierAddress = reader["supplierAddress"].ToString(),
                        SupplierCity = reader["supplierCity"].ToString(),
                        SupplierRegion = reader["supplierRegion"].ToString(),
                        SupplierPostalCode = reader["supplierPostalCode"].ToString(),
                        SupplierCountry = reader["supplierCountry"].ToString(),
                        SupplierPhone = reader["supplierPhone"].ToString(),
                        SupplierFax = reader["supplierFax"].ToString(),
                        SupplierHomePage = reader["supplierHomePage"].ToString()
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

        return supplierModels;
    }

    public List<SupplierModel> Search(string search)
    {
        List<SupplierModel> supplierModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
            SELECT  *
            FROM    supplier 
            WHERE   supplier.isDelete != 1
            AND     supplier.tenantId = @tenantId
            AND     (
                        supplier.supplierName LIKE CONCAT('%',@search,'%') OR
                        supplier.supplierContactName LIKE CONCAT('%',@search,'%') OR
                        supplier.supplierContactTitle LIKE CONCAT('%',@search,'%') OR
                        supplier.supplierAddress LIKE CONCAT('%',@search,'%') OR
                        supplier.supplierCity LIKE CONCAT('%',@search,'%') OR
                        supplier.supplierRegion LIKE CONCAT('%',@search,'%') OR
                        supplier.supplierPostalCode LIKE CONCAT('%',@search,'%') OR
                        supplier.supplierCountry LIKE CONCAT('%',@search,'%') OR
                        supplier.supplierPhone LIKE CONCAT('%',@search,'%') OR
                        supplier.supplierFax LIKE CONCAT('%',@search,'%') OR
                        supplier.supplierHomePage LIKE CONCAT('%',@search,'%')
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
                    supplierModels.Add(new SupplierModel
                    {
                        SupplierName = reader["supplierName"].ToString(),
                        SupplierContactName = reader["supplierContactName"].ToString(),
                        SupplierContactTitle = reader["supplierContactTitle"].ToString(),
                        SupplierAddress = reader["supplierAddress"].ToString(),
                        SupplierCity = reader["supplierCity"].ToString(),
                        SupplierRegion = reader["supplierRegion"].ToString(),
                        SupplierPostalCode = reader["supplierPostalCode"].ToString(),
                        SupplierCountry = reader["supplierCountry"].ToString(),
                        SupplierPhone = reader["supplierPhone"].ToString(),
                        SupplierFax = reader["supplierFax"].ToString(),
                        SupplierHomePage = reader["supplierHomePage"].ToString()
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

        return supplierModels;
    }

    public SupplierModel GetSingle(SupplierModel supplierModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
            SELECT  *
            FROM    supplier 
            WHERE   isDelete    != 1
            AND     supplierId  =   @supplierId
            AND     tenantId    =   @tenantId
            LIMIT 1";
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
                    Key = "@supplierId",
                    Value = supplierModel.SupplierKey
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
                    supplierModel = new SupplierModel
                    {
                        SupplierKey = Convert.ToUInt32(reader["supplierId"]),
                        SupplierName = reader["supplierName"].ToString(),
                        SupplierContactName = reader["supplierContactName"].ToString(),
                        SupplierContactTitle = reader["supplierContactTitle"].ToString(),
                        SupplierAddress = reader["supplierAddress"].ToString(),
                        SupplierCity = reader["supplierCity"].ToString(),
                        SupplierRegion = reader["supplierRegion"].ToString(),
                        SupplierPostalCode = reader["supplierPostalCode"].ToString(),
                        SupplierCountry = reader["supplierCountry"].ToString(),
                        SupplierPhone = reader["supplierPhone"].ToString(),
                        SupplierFax = reader["supplierFax"].ToString(),
                        SupplierHomePage = reader["supplierHomePage"].ToString()
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

        return supplierModel;
    }

    public uint GetDefault()
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        uint supplierId;
        try
        {
            connection.Open();
            sql += @"
            SELECT  supplierId
            FROM    supplier 
            WHERE   isDelete    != 1
            AND     tenantId    =   @tenantId
            LIMIT   1 ";
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
            supplierId = (uint)mySqlCommand.ExecuteScalar();
            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
            throw new Exception(ex.Message);
        }

        return supplierId;
    }

    public byte[] GetExcel()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Administrator > Supplier ");
        worksheet.Cell(1, 1).Value = "Name";
        worksheet.Cell(1, 2).Value = "Contact Name";
        worksheet.Cell(1, 3).Value = "Contact Title";
        worksheet.Cell(1, 4).Value = "Address";
        worksheet.Cell(1, 5).Value = "City";
        worksheet.Cell(1, 6).Value = "Region";
        worksheet.Cell(1, 7).Value = "Postal Code";
        worksheet.Cell(1, 8).Value = "Country";
        worksheet.Cell(1, 9).Value = "Phone";
        worksheet.Cell(1, 10).Value = "Fax";
        worksheet.Cell(1, 11).Value = "Home Page";
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
                    worksheet.Cell(currentRow, 1).Value = reader["supplierName"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["supplierContactName"].ToString();
                    worksheet.Cell(currentRow, 3).Value = reader["supplierContactTitle"].ToString();
                    worksheet.Cell(currentRow, 4).Value = reader["supplierAddress"].ToString();
                    worksheet.Cell(currentRow, 5).Value = reader["supplierCity"].ToString();
                    worksheet.Cell(currentRow, 6).Value = reader["supplierRegion"].ToString();
                    worksheet.Cell(currentRow, 7).Value = reader["supplierPostalCode"].ToString();
                    worksheet.Cell(currentRow, 8).Value = reader["supplierCountry"].ToString();
                    worksheet.Cell(currentRow, 9).Value = reader["supplierPhone"].ToString();
                    worksheet.Cell(currentRow, 10).Value = reader["supplierFax"].ToString();
                    worksheet.Cell(currentRow, 11).Value = reader["supplierHomePage"].ToString();
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

    public void Update(SupplierModel supplierModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();
            sql = @"
            UPDATE  supplier 
            SET     tenantId                =   @tenantId,
                    supplierName            =   @supplierName,
                    supplierContactName     =   @supplierContactName,
                    supplierContactTitle    =   @supplierContactTitle,
                    supplierAddress         =   @supplierAddress,
                    supplierCity            =   @supplierCity,
                    supplierRegion          =   @supplierRegion,
                    supplierPostalCode      =   @supplierPostalCode,
                    supplierCountry         =   @supplierCountry,
                    supplierPhone           =   @supplierPhone,
                    supplierFax             =   @supplierFax,
                    supplierHomePage        =   @supplierHomePage,
                    isDelete                =   @isDelete
            WHERE   supplierId              =   @supplierId";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@supplierId",
                    Value = supplierModel.SupplierKey
                },
                new()
                {
                    Key = "@tenantId",
                    Value = _sharedUtil.GetTenantId()
                },
                new()
                {
                    Key = "@supplierName",
                    Value = supplierModel.SupplierName
                },
                new()
                {
                    Key = "@supplierContactName",
                    Value = supplierModel.SupplierContactName
                },
                new()
                {
                    Key = "@supplierContactTitle",
                    Value = supplierModel.SupplierContactTitle
                },
                new()
                {
                    Key = "@supplierAddress",
                    Value = supplierModel.SupplierAddress
                },
                new()
                {
                    Key = "@supplierCity",
                    Value = supplierModel.SupplierCity
                },
                new()
                {
                    Key = "@supplierRegion",
                    Value = supplierModel.SupplierRegion
                },
                new()
                {
                    Key = "@supplierPostalCode",
                    Value = supplierModel.SupplierPostalCode
                },
                new()
                {
                    Key = "@supplierCountry",
                    Value = supplierModel.SupplierCountry
                },
                new()
                {
                    Key = "@supplierPhone",
                    Value = supplierModel.SupplierPhone
                },
                new()
                {
                    Key = "@supplierFax",
                    Value = supplierModel.SupplierFax
                },
                new()
                {
                    Key = "@supplierHomePage",
                    Value = supplierModel.SupplierHomePage
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

    public void Delete(SupplierModel supplierModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();
            sql = @"
                UPDATE  supplier 
                SET     isDelete    =   1
                WHERE   supplierId    =   @supplierId";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@supplierId",
                    Value = supplierModel.SupplierKey
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