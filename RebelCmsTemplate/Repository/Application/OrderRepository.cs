using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Application;
using RebelCmsTemplate.Models.Shared;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Repository.Application;

public class OrderRepository
{
    private readonly SharedUtil _sharedUtil;

    public OrderRepository(IHttpContextAccessor httpContextAccessor)
    {
        _sharedUtil = new SharedUtil(httpContextAccessor);
    }

    public int Create(OrderModel orderModel)
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
                @"INSERT INTO `order` (orderId,tenantId,customerId,shipperId,employeeId,orderDate,orderRequiredDate,orderShippedDate,orderFreight,orderShipName,orderShipAddress,orderShipCity,orderShipRegion,orderShipPostalCode,orderShipCountry,isDelete) VALUES (null,@tenantId,@customerId,@shipperId,@employeeId,@orderDate,@orderRequiredDate,@orderShippedDate,@orderFreight,@orderShipName,@orderShipAddress,@orderShipCity,@orderShipRegion,@orderShipPostalCode,@orderShipCountry,@isDelete);";
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
                    Value = orderModel.CustomerKey
                },
                new()
                {
                    Key = "@shipperId",
                    Value = orderModel.ShipperKey
                },
                new()
                {
                    Key = "@employeeId",
                    Value = orderModel.EmployeeKey
                },
                new()
                {
                    Key = "@orderDate",
                    Value = orderModel.OrderDate?.ToString("yyyy-MM-dd")
                },
                new()
                {
                    Key = "@orderRequiredDate",
                    Value = orderModel.OrderRequiredDate?.ToString("yyyy-MM-dd")
                },
                new()
                {
                    Key = "@orderShippedDate",
                    Value = orderModel.OrderShippedDate?.ToString("yyyy-MM-dd")
                },
                new()
                {
                    Key = "@orderFreight",
                    Value = orderModel.OrderFreight
                },
                new()
                {
                    Key = "@orderShipName",
                    Value = orderModel.OrderShipName
                },
                new()
                {
                    Key = "@orderShipAddress",
                    Value = orderModel.OrderShipAddress
                },
                new()
                {
                    Key = "@orderShipCity",
                    Value = orderModel.OrderShipCity
                },
                new()
                {
                    Key = "@orderShipRegion",
                    Value = orderModel.OrderShipRegion
                },
                new()
                {
                    Key = "@orderShipPostalCode",
                    Value = orderModel.OrderShipPostalCode
                },
                new()
                {
                    Key = "@orderShipCountry",
                    Value = orderModel.OrderShipCountry
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
            lastInsertKey = (int)mySqlCommand.LastInsertedId;
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

    public List<OrderModel> Read()
    {
        List<OrderModel> orderModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        // the reason limit avoid hang . dom browser can't process a lot of record. Want more paging or ajax paging
        try
        {
            connection.Open();
            sql = @"
                SELECT      *
                FROM        `order` 
	 JOIN customer 
	 USING(customerId)
	 JOIN shipper 
	 USING(shipperId)
	 JOIN employee 
	 USING(employeeId)
	 WHERE   `order`.isDelete != 1
                ORDER BY    orderId DESC LIMIT 100 ";
            MySqlCommand mySqlCommand = new(sql, connection);
            _sharedUtil.SetSqlSession(sql, parameterModels);
            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    orderModels.Add(new OrderModel
                    {
                        OrderKey = Convert.ToUInt32(reader["orderId"]),
                        CustomerName = reader["customerName"].ToString(),
                        CustomerKey = Convert.ToUInt32(reader["customerId"]),
                        ShipperName = reader["shipperName"].ToString(),
                        ShipperKey = Convert.ToUInt32(reader["shipperId"]),
                        EmployeeLastName = reader["employeeLastName"].ToString(),
                        EmployeeKey = Convert.ToUInt32(reader["employeeId"]),
                        OrderDate = reader["orderDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["orderDate"])
                            : null,
                        OrderRequiredDate = reader["orderRequiredDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["orderRequiredDate"])
                            : null,
                        OrderShippedDate = reader["orderShippedDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["orderShippedDate"])
                            : null,
                        OrderFreight = Convert.ToDecimal(reader["orderFreight"]),
                        OrderShipName = reader["orderShipName"].ToString(),
                        OrderShipAddress = reader["orderShipAddress"].ToString(),
                        OrderShipCity = reader["orderShipCity"].ToString(),
                        OrderShipRegion = reader["orderShipRegion"].ToString(),
                        OrderShipPostalCode = reader["orderShipPostalCode"].ToString(),
                        OrderShipCountry = reader["orderShipCountry"].ToString()
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

        return orderModels;
    }

    public List<OrderModel> Search(string search)
    {
        List<OrderModel> orderModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
            SELECT  *
            FROM    `order`

            JOIN customer 
            USING(customerId)

            JOIN shipper 
            USING(shipperId)

            JOIN employee 
            USING(employeeId)

            WHERE   `order`.isDelete != 1
            AND     `order`.tenantId = @tenantId
     		AND (				
				customer.customerCode LIKE CONCAT('%',@search,'%') OR
				customer.customerName LIKE CONCAT('%',@search,'%') OR
				customer.customerContactName LIKE CONCAT('%',@search,'%') OR
				customer.customerContactTitle LIKE CONCAT('%',@search,'%') OR
				customer.customerAddress LIKE CONCAT('%',@search,'%') OR
				customer.customerCity LIKE CONCAT('%',@search,'%') 
            ) ORDER BY `order`.`orderId` DESC LIMIT 100 ";
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
                    orderModels.Add(new OrderModel
                    {
                        CustomerName = reader["customerName"].ToString(),
                        CustomerKey = Convert.ToUInt32(reader["customerId"]),
                        ShipperName = reader["shipperName"].ToString(),
                        ShipperKey = Convert.ToUInt32(reader["shipperId"]),
                        EmployeeLastName = reader["employeeLastName"].ToString(),
                        EmployeeKey = Convert.ToUInt32(reader["employeeId"]),
                        OrderDate = reader["orderDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["orderDate"])
                            : null,
                        OrderRequiredDate = reader["orderRequiredDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["orderRequiredDate"])
                            : null,
                        OrderShippedDate = reader["orderShippedDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["orderShippedDate"])
                            : null,
                        OrderFreight = Convert.ToDecimal(reader["orderFreight"]),
                        OrderShipName = reader["orderShipName"].ToString(),
                        OrderShipAddress = reader["orderShipAddress"].ToString(),
                        OrderShipCity = reader["orderShipCity"].ToString(),
                        OrderShipRegion = reader["orderShipRegion"].ToString(),
                        OrderShipPostalCode = reader["orderShipPostalCode"].ToString(),
                        OrderShipCountry = reader["orderShipCountry"].ToString()
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

        return orderModels;
    }
    public OrderModel GetSingleWithDetail(OrderModel orderModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
            SELECT  *
            FROM    `order`

            JOIN customer 
            USING(customerId)

            JOIN shipper 
            USING(shipperId)

            JOIN employee 
            USING(employeeId)

            WHERE   `order`.isDelete    !=   1
            AND     `order`.tenantId    =   @tenantId
            AND     `order`.orderId   =   @orderId
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
                    Key = "@orderId",
                    Value = orderModel.OrderKey
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
                    orderModel = new OrderModel
                    {
                        OrderKey = Convert.ToUInt32(reader["orderId"]),
                        CustomerKey = Convert.ToUInt32(reader["customerId"]),
                        ShipperKey = Convert.ToUInt32(reader["shipperId"]),
                        EmployeeKey = Convert.ToUInt32(reader["employeeId"]),
                        OrderDate = reader["orderDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["orderDate"])
                            : null,
                        OrderRequiredDate = reader["orderRequiredDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["orderRequiredDate"])
                            : null,
                        OrderShippedDate = reader["orderShippedDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["orderShippedDate"])
                            : null,
                        OrderFreight = Convert.ToDecimal(reader["orderFreight"]),
                        OrderShipName = reader["orderShipName"].ToString(),
                        OrderShipAddress = reader["orderShipAddress"].ToString(),
                        OrderShipCity = reader["orderShipCity"].ToString(),
                        OrderShipRegion = reader["orderShipRegion"].ToString(),
                        OrderShipPostalCode = reader["orderShipPostalCode"].ToString(),
                        OrderShipCountry = reader["orderShipCountry"].ToString()
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

        List<OrderDetailModel> orderDetailModels = new();
        try
        {
            sql = @"
            SELECT      *
            FROM        order_detail

            JOIN `order` 
            USING(orderId)

            JOIN product 
            USING(productId)

            WHERE   `order`.isDelete        !=  1
            AND     `order`.tenantId        =   @tenantId
            AND     order_detail.isDelete != 1
            AND   order_detail.orderId  =   @orderId";
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
                    Key = "@orderId",
                    Value = orderModel.OrderKey
                }
            };
            foreach (var parameter in parameterModels)
            {
                mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    orderDetailModels.Add(new OrderDetailModel
                    {
                        OrderDetailKey = Convert.ToUInt32(reader["orderDetailId"]),
                        OrderKey = Convert.ToUInt32(reader["orderId"]),
                        ProductKey = Convert.ToUInt32(reader["productId"]),
                        OrderDetailUnitPrice = Convert.ToDecimal(reader["orderDetailUnitPrice"]),
                        OrderDetailQuantity = Convert.ToInt32(reader["orderDetailQuantity"]),
                        OrderDetailDiscount = Convert.ToDouble(reader["orderDetailDiscount"])
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


        orderModel.Data = orderDetailModels;


        return orderModel;
    }

    public byte[] GetExcel()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Administrator > Order ");

        worksheet.Cell(1, 1).Value = "Customer";
        worksheet.Cell(1, 2).Value = "Shipper";
        worksheet.Cell(1, 3).Value = "Employee";
        worksheet.Cell(1, 4).Value = "Order Date";
        worksheet.Cell(1, 5).Value = "Required Date";
        worksheet.Cell(1, 6).Value = "Shipped Date";
        worksheet.Cell(1, 7).Value = "Freight";
        worksheet.Cell(1, 8).Value = "Ship Name";
        worksheet.Cell(1, 9).Value = "Ship Address";
        worksheet.Cell(1, 10).Value = "Ship City";
        worksheet.Cell(1, 11).Value = "Ship Region";
        worksheet.Cell(1, 12).Value = "Ship Postal Code";
        worksheet.Cell(1, 13).Value = "Ship Country";
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
                    worksheet.Cell(currentRow, 1).Value = reader["customerName"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["shipperName"].ToString();
                    worksheet.Cell(currentRow, 3).Value = reader["employeeLastName"].ToString();
                    worksheet.Cell(currentRow, 4).Value = reader["orderDate"].ToString();
                    worksheet.Cell(currentRow, 5).Value = reader["orderRequiredDate"].ToString();
                    worksheet.Cell(currentRow, 6).Value = reader["orderShippedDate"].ToString();
                    worksheet.Cell(currentRow, 7).Value = reader["orderFreight"].ToString();
                    worksheet.Cell(currentRow, 8).Value = reader["orderShipName"].ToString();
                    worksheet.Cell(currentRow, 9).Value = reader["orderShipAddress"].ToString();
                    worksheet.Cell(currentRow, 10).Value = reader["orderShipCity"].ToString();
                    worksheet.Cell(currentRow, 11).Value = reader["orderShipRegion"].ToString();
                    worksheet.Cell(currentRow, 12).Value = reader["orderShipPostalCode"].ToString();
                    worksheet.Cell(currentRow, 13).Value = reader["orderShipCountry"].ToString();
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

    public void Update(OrderModel orderModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();
            sql = @"
            UPDATE  `order` 
            SET     customerId              =   @customerId,
                    shipperId               =   @shipperId,
                    employeeId              =   @employeeId,
                    orderDate        =   @orderDate,
                    orderRequiredDate     =   @orderRequiredDate,
                    orderShippedDate      =   @orderShippedDate,
                    orderFreight          =   @orderFreight,
                    orderShipName         =   @orderShipName,
                    orderShipAddress      =   @orderShipAddress,
                    orderShipCity         =   @orderShipCity,
                    orderShipRegion       =   @orderShipRegion,
                    orderShipPostalCode   =   @orderShipPostalCode,
                    orderShipCountry      =   @orderShipCountry

            WHERE   orderId    =   @orderId";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@orderId",
                    Value = orderModel.OrderKey
                },
                new()
                {
                    Key = "@tenantId",
                    Value = _sharedUtil.GetTenantId()
                },
                new()
                {
                    Key = "@customerId",
                    Value = orderModel.CustomerKey
                },
                new()
                {
                    Key = "@shipperId",
                    Value = orderModel.ShipperKey
                },
                new()
                {
                    Key = "@employeeId",
                    Value = orderModel.EmployeeKey
                },
                new()
                {
                    Key = "@orderDate",
                    Value = orderModel.OrderDate?.ToString("yyyy-MM-dd")
                },
                new()
                {
                    Key = "@orderRequiredDate",
                    Value = orderModel.OrderRequiredDate?.ToString("yyyy-MM-dd")
                },
                new()
                {
                    Key = "@orderShippedDate",
                    Value = orderModel.OrderShippedDate?.ToString("yyyy-MM-dd")
                },
                new()
                {
                    Key = "@orderFreight",
                    Value = orderModel.OrderFreight
                },
                new()
                {
                    Key = "@orderShipName",
                    Value = orderModel.OrderShipName
                },
                new()
                {
                    Key = "@orderShipAddress",
                    Value = orderModel.OrderShipAddress
                },
                new()
                {
                    Key = "@orderShipCity",
                    Value = orderModel.OrderShipCity
                },
                new()
                {
                    Key = "@orderShipRegion",
                    Value = orderModel.OrderShipRegion
                },
                new()
                {
                    Key = "@orderShipPostalCode",
                    Value = orderModel.OrderShipPostalCode
                },
                new()
                {
                    Key = "@orderShipCountry",
                    Value = orderModel.OrderShipCountry
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

    public void Delete(OrderModel orderModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();
            sql = @"
            UPDATE  `order` 
            SET     isDelete    =   1
            WHERE   orderId    =   @orderId";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@orderId",
                    Value = orderModel.OrderKey
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