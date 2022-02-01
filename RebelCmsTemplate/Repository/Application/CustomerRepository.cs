using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Application;
using RebelCmsTemplate.Models.Shared;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Repository.Application;

public class CustomerRepository
{
    private readonly SharedUtil _sharedUtil;

    public CustomerRepository(IHttpContextAccessor httpContextAccessor)
    {
        _sharedUtil = new SharedUtil(httpContextAccessor);
    }

    public int Create(CustomerModel customerModel)
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
                @"INSERT INTO customer (customerId,tenantId,customerCode,customerName,customerContactName,customerContactTitle,customerAddress,customerCity,customerRegion,customerPostalCode,customerCountry,customerPhone,customerFax,isDelete) VALUES (null,@tenantId,@customerCode,@customerName,@customerContactName,@customerContactTitle,@customerAddress,@customerCity,@customerRegion,@customerPostalCode,@customerCountry,@customerPhone,@customerFax,@isDelete);";
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
                    Key = "@customerCode",
                    Value = customerModel.CustomerCode
                },
                new()
                {
                    Key = "@customerName",
                    Value = customerModel.CustomerName
                },
                new()
                {
                    Key = "@customerContactName",
                    Value = customerModel.CustomerContactName
                },
                new()
                {
                    Key = "@customerContactTitle",
                    Value = customerModel.CustomerContactTitle
                },
                new()
                {
                    Key = "@customerAddress",
                    Value = customerModel.CustomerAddress
                },
                new()
                {
                    Key = "@customerCity",
                    Value = customerModel.CustomerCity
                },
                new()
                {
                    Key = "@customerRegion",
                    Value = customerModel.CustomerRegion
                },
                new()
                {
                    Key = "@customerPostalCode",
                    Value = customerModel.CustomerPostalCode
                },
                new()
                {
                    Key = "@customerCountry",
                    Value = customerModel.CustomerCountry
                },
                new()
                {
                    Key = "@customerPhone",
                    Value = customerModel.CustomerPhone
                },
                new()
                {
                    Key = "@customerFax",
                    Value = customerModel.CustomerFax
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

    public List<CustomerModel> Read()
    {
        List<CustomerModel> customerModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql = @"
                SELECT      *
                FROM        customer 
                WHERE       isDelete !=1
                ORDER BY    customerId DESC ";
            MySqlCommand mySqlCommand = new(sql, connection);
            _sharedUtil.SetSqlSession(sql, parameterModels);
            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    customerModels.Add(new CustomerModel
                    {
                        CustomerKey = Convert.ToUInt32(reader["customerId"]),
                        TenantKey = Convert.ToUInt32(reader["tenantId"]),
                        CustomerCode = reader["customerCode"].ToString(),
                        CustomerName = reader["customerName"].ToString(),
                        CustomerContactName = reader["customerContactName"].ToString(),
                        CustomerContactTitle = reader["customerContactTitle"].ToString(),
                        CustomerAddress = reader["customerAddress"].ToString(),
                        CustomerCity = reader["customerCity"].ToString(),
                        CustomerRegion = reader["customerRegion"].ToString(),
                        CustomerPostalCode = reader["customerPostalCode"].ToString(),
                        CustomerCountry = reader["customerCountry"].ToString(),
                        CustomerPhone = reader["customerPhone"].ToString(),
                        CustomerFax = reader["customerFax"].ToString(),
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

        return customerModels;
    }

    public List<CustomerModel> Search(string search)
    {
        List<CustomerModel> customerModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
                SELECT  *
                FROM    customer 
	 WHERE   customer.isDelete != 1
	 AND (
	 customer.customerCode LIKE CONCAT('%',@search,'%') OR
	 customer.customerName LIKE CONCAT('%',@search,'%') OR
	 customer.customerContactName LIKE CONCAT('%',@search,'%') OR
	 customer.customerContactTitle LIKE CONCAT('%',@search,'%') OR
	 customer.customerAddress LIKE CONCAT('%',@search,'%') OR
	 customer.customerCity LIKE CONCAT('%',@search,'%') OR
	 customer.customerRegion LIKE CONCAT('%',@search,'%') OR
	 customer.customerPostalCode LIKE CONCAT('%',@search,'%') OR
	 customer.customerCountry LIKE CONCAT('%',@search,'%') OR
	 customer.customerPhone LIKE CONCAT('%',@search,'%') OR
	 customer.customerFax LIKE CONCAT('%',@search,'%') )";
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
                    customerModels.Add(new CustomerModel
                    {
                        CustomerKey = Convert.ToUInt32(reader["customerId"]),
                        TenantKey = Convert.ToUInt32(reader["tenantId"]),
                        CustomerCode = reader["customerCode"].ToString(),
                        CustomerName = reader["customerName"].ToString(),
                        CustomerContactName = reader["customerContactName"].ToString(),
                        CustomerContactTitle = reader["customerContactTitle"].ToString(),
                        CustomerAddress = reader["customerAddress"].ToString(),
                        CustomerCity = reader["customerCity"].ToString(),
                        CustomerRegion = reader["customerRegion"].ToString(),
                        CustomerPostalCode = reader["customerPostalCode"].ToString(),
                        CustomerCountry = reader["customerCountry"].ToString(),
                        CustomerPhone = reader["customerPhone"].ToString(),
                        CustomerFax = reader["customerFax"].ToString(),
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

        return customerModels;
    }

    public CustomerModel GetSingle(CustomerModel customerModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
            SELECT  *
            FROM    customer 
            WHERE   isDelete    != 1
            AND     tenantId    = @tenantId
            AND     customerId  =   @customerId
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
                    Key = "@customerId",
                    Value = customerModel.CustomerKey
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
                    customerModel = new CustomerModel
                    {
                        CustomerKey = Convert.ToUInt32(reader["customerId"]),
                        TenantKey = Convert.ToUInt32(reader["tenantId"]),
                        CustomerCode = reader["customerCode"].ToString(),
                        CustomerName = reader["customerName"].ToString(),
                        CustomerContactName = reader["customerContactName"].ToString(),
                        CustomerContactTitle = reader["customerContactTitle"].ToString(),
                        CustomerAddress = reader["customerAddress"].ToString(),
                        CustomerCity = reader["customerCity"].ToString(),
                        CustomerRegion = reader["customerRegion"].ToString(),
                        CustomerPostalCode = reader["customerPostalCode"].ToString(),
                        CustomerCountry = reader["customerCountry"].ToString(),
                        CustomerPhone = reader["customerPhone"].ToString(),
                        CustomerFax = reader["customerFax"].ToString(),
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

        return customerModel;
    }

    public uint GetDefault()
    {
        uint customerId ;
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
            SELECT  customerId
            FROM    customer 
            WHERE   isDelete    != 1
            AND     tenantId    = @tenantId
            AND     customerId  =   @customerId
            AND     isDefault   =   1
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

            customerId = (uint) mySqlCommand.ExecuteScalar();

            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
            throw new Exception(ex.Message);
        }

        return customerId;
    }

    public byte[] GetExcel()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Administrator > Customer ");
        worksheet.Cell(1, 1).Value = "customerId";
        worksheet.Cell(1, 2).Value = "tenantId";
        worksheet.Cell(1, 3).Value = "customerCode";
        worksheet.Cell(1, 4).Value = "customerName";
        worksheet.Cell(1, 5).Value = "customerContactName";
        worksheet.Cell(1, 6).Value = "customerContactTitle";
        worksheet.Cell(1, 7).Value = "customerAddress";
        worksheet.Cell(1, 8).Value = "customerCity";
        worksheet.Cell(1, 9).Value = "customerRegion";
        worksheet.Cell(1, 10).Value = "customerPostalCode";
        worksheet.Cell(1, 11).Value = "customerCountry";
        worksheet.Cell(1, 12).Value = "customerPhone";
        worksheet.Cell(1, 13).Value = "customerFax";
        worksheet.Cell(1, 14).Value = "isDelete";
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
                    worksheet.Cell(currentRow, 1).Value = reader["customerId"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["tenantId"].ToString();
                    worksheet.Cell(currentRow, 3).Value = reader["customerCode"].ToString();
                    worksheet.Cell(currentRow, 4).Value = reader["customerName"].ToString();
                    worksheet.Cell(currentRow, 5).Value = reader["customerContactName"].ToString();
                    worksheet.Cell(currentRow, 6).Value = reader["customerContactTitle"].ToString();
                    worksheet.Cell(currentRow, 7).Value = reader["customerAddress"].ToString();
                    worksheet.Cell(currentRow, 8).Value = reader["customerCity"].ToString();
                    worksheet.Cell(currentRow, 9).Value = reader["customerRegion"].ToString();
                    worksheet.Cell(currentRow, 10).Value = reader["customerPostalCode"].ToString();
                    worksheet.Cell(currentRow, 11).Value = reader["customerCountry"].ToString();
                    worksheet.Cell(currentRow, 12).Value = reader["customerPhone"].ToString();
                    worksheet.Cell(currentRow, 13).Value = reader["customerFax"].ToString();
                    worksheet.Cell(currentRow, 14).Value = reader["isDelete"].ToString();
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

    public void Update(CustomerModel customerModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();
            sql = @"
                UPDATE  customer 
                SET     
tenantId=@tenantId,
customerCode=@customerCode,
customerName=@customerName,
customerContactName=@customerContactName,
customerContactTitle=@customerContactTitle,
customerAddress=@customerAddress,
customerCity=@customerCity,
customerRegion=@customerRegion,
customerPostalCode=@customerPostalCode,
customerCountry=@customerCountry,
customerPhone=@customerPhone,
customerFax=@customerFax,
isDelete=@isDelete

                WHERE   customerId    =   @customerId";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@customerId",
                    Value = customerModel.CustomerKey
                },
                new()
                {
                    Key = "@tenantId",
                    Value = _sharedUtil.GetTenantId()
                },
                new()
                {
                    Key = "@customerCode",
                    Value = customerModel.CustomerCode
                },
                new()
                {
                    Key = "@customerName",
                    Value = customerModel.CustomerName
                },
                new()
                {
                    Key = "@customerContactName",
                    Value = customerModel.CustomerContactName
                },
                new()
                {
                    Key = "@customerContactTitle",
                    Value = customerModel.CustomerContactTitle
                },
                new()
                {
                    Key = "@customerAddress",
                    Value = customerModel.CustomerAddress
                },
                new()
                {
                    Key = "@customerCity",
                    Value = customerModel.CustomerCity
                },
                new()
                {
                    Key = "@customerRegion",
                    Value = customerModel.CustomerRegion
                },
                new()
                {
                    Key = "@customerPostalCode",
                    Value = customerModel.CustomerPostalCode
                },
                new()
                {
                    Key = "@customerCountry",
                    Value = customerModel.CustomerCountry
                },
                new()
                {
                    Key = "@customerPhone",
                    Value = customerModel.CustomerPhone
                },
                new()
                {
                    Key = "@customerFax",
                    Value = customerModel.CustomerFax
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

    public void Delete(CustomerModel customerModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();
            sql = @"
                UPDATE  customer 
                SET     isDelete    =   1
                WHERE   customerId    =   @customerId";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@customerId",
                    Value = customerModel.CustomerKey
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