using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Application;
using RebelCmsTemplate.Models.Shared;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Repository.Application;

public class OrderDetailRepository
{
    private readonly SharedUtil _sharedUtil;

    public OrderDetailRepository(IHttpContextAccessor httpContextAccessor)
    {
        _sharedUtil = new SharedUtil(httpContextAccessor);
    }

    public int Create(OrderDetailModel orderDetailModel)
    {
        int lastInsertKey ;
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();
            sql +=
                @"INSERT INTO order_detail (orderDetailId,tenantId,orderId,productId,orderDetailUnitPrice,orderDetailQuantity,orderDetailDiscount,isDelete) VALUES (null,@tenantId,@orderId,@productId,@orderDetailUnitPrice,@orderDetailQuantity,@orderDetailDiscount,@isDelete);";
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
                    Value = orderDetailModel.OrderKey
                },
                new()
                {
                    Key = "@productId",
                    Value = orderDetailModel.ProductKey
                },
                new()
                {
                    Key = "@orderDetailUnitPrice",
                    Value = orderDetailModel.OrderDetailUnitPrice
                },
                new()
                {
                    Key = "@orderDetailQuantity",
                    Value = orderDetailModel.OrderDetailQuantity
                },
                new()
                {
                    Key = "@orderDetailDiscount",
                    Value = orderDetailModel.OrderDetailDiscount
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

    public List<OrderDetailModel> Read()
    {
        List<OrderDetailModel> orderDetailModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql = @"
            SELECT      *
            FROM        order_detail 
            WHERE       isDelete !=1
            AND         tenantId = @tenantId
            ORDER BY    orderDetailId DESC ";
            MySqlCommand mySqlCommand = new(sql, connection);
            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    orderDetailModels.Add(new OrderDetailModel
                    {
                        OrderDetailKey = Convert.ToUInt32(reader["orderDetailId"]),
                        OrderKey = Convert.ToUInt32(reader["orderId"]),
                        ProductKey = Convert.ToUInt32(reader["productId"]),
                        OrderDetailUnitPrice = reader["orderDetailUnitPrice"] != DBNull.Value
                            ? Convert.ToDecimal(reader["orderDetailUnitPrice"])
                            : 0,
                        OrderDetailQuantity = reader["orderDetailQuantity"] != DBNull.Value
                            ? Convert.ToInt32(reader["orderDetailQuantity"])
                            : 0,
                        OrderDetailDiscount = reader["orderDetailDiscount"] != DBNull.Value
                            ? Convert.ToDouble(reader["orderDetailDiscount"])
                            : 0,
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

        return orderDetailModels;
    }

    public List<OrderDetailModel> Search(string search)
    {
        List<OrderDetailModel> orderDetailModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
            SELECT  *
            FROM    order_detail

            JOIN order 
            USING(orderId)

            JOIN product 
            USING(productId)

            WHERE   order_detail.isDelete != 1
            AND     order_detail.tenantId = @tenantId 
            AND     (
                        order.orderId LIKE CONCAT('%',@search,'%') OR
                        order.orderId LIKE CONCAT('%',@search,'%') OR
                        order.orderId LIKE CONCAT('%',@search,'%') OR
                        order.orderId LIKE CONCAT('%',@search,'%') OR
                        order.orderId LIKE CONCAT('%',@search,'%') OR
                        order.orderId LIKE CONCAT('%',@search,'%') OR
                        order.orderId LIKE CONCAT('%',@search,'%') OR
                        order.orderId LIKE CONCAT('%',@search,'%') OR	 order.orderId LIKE CONCAT('%',@search,'%') OR	 order.orderId LIKE CONCAT('%',@search,'%') OR	 order.orderId LIKE CONCAT('%',@search,'%') OR	 order.orderId LIKE CONCAT('%',@search,'%') OR	 order.orderId LIKE CONCAT('%',@search,'%') OR	 order.orderId LIKE CONCAT('%',@search,'%') OR	 order.orderId LIKE CONCAT('%',@search,'%') OR	 order.orderId LIKE CONCAT('%',@search,'%') OR	 product.productId LIKE CONCAT('%',@search,'%') OR	 product.productId LIKE CONCAT('%',@search,'%') OR	 product.productId LIKE CONCAT('%',@search,'%') OR	 product.productId LIKE CONCAT('%',@search,'%') OR	 product.productId LIKE CONCAT('%',@search,'%') OR	 product.productId LIKE CONCAT('%',@search,'%') OR	 product.productId LIKE CONCAT('%',@search,'%') OR	 product.productId LIKE CONCAT('%',@search,'%') OR	 product.productId LIKE CONCAT('%',@search,'%') OR	 product.productId LIKE CONCAT('%',@search,'%') OR	 product.productId LIKE CONCAT('%',@search,'%') OR	 product.productId LIKE CONCAT('%',@search,'%') OR	 product.productId LIKE CONCAT('%',@search,'%') OR	 product.productId LIKE CONCAT('%',@search,'%') OR	 product.productId LIKE CONCAT('%',@search,'%') OR
            order_detail.orderDetailUnitPrice LIKE CONCAT('%',@search,'%') OR
            order_detail.orderDetailQuantity LIKE CONCAT('%',@search,'%') OR
            order_detail.orderDetailDiscount LIKE CONCAT('%',@search,'%') )";
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
                    orderDetailModels.Add(new OrderDetailModel
                    {
                        OrderDetailKey = Convert.ToUInt32(reader["orderDetailId"]),
                        OrderKey = Convert.ToUInt32(reader["orderId"]),
                        ProductKey = Convert.ToUInt32(reader["productId"]),
                        OrderDetailUnitPrice = Convert.ToDecimal(reader["orderDetailUnitPrice"]),
                        OrderDetailQuantity = Convert.ToInt32(reader["orderDetailQuantity"]),
                        OrderDetailDiscount = Convert.ToDouble(reader["orderDetailDiscount"]),
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

        return orderDetailModels;
    }
    public byte[] GetExcel()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Administrator > OrderDetail ");
        worksheet.Cell(1, 1).Value = "orderDetailId";
        worksheet.Cell(1, 2).Value = "orderId";
        worksheet.Cell(1, 3).Value = "productId";
        worksheet.Cell(1, 4).Value = "orderDetailUnitPrice";
        worksheet.Cell(1, 5).Value = "orderDetailQuantity";
        worksheet.Cell(1, 6).Value = "orderDetailDiscount";
        worksheet.Cell(1, 7).Value = "isDelete";
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
                    worksheet.Cell(currentRow, 1).Value = reader["orderDetailId"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["orderId"].ToString();
                    worksheet.Cell(currentRow, 3).Value = reader["productId"].ToString();
                    worksheet.Cell(currentRow, 4).Value = reader["orderDetailUnitPrice"].ToString();
                    worksheet.Cell(currentRow, 5).Value = reader["orderDetailQuantity"].ToString();
                    worksheet.Cell(currentRow, 6).Value = reader["orderDetailDiscount"].ToString();
                    worksheet.Cell(currentRow, 7).Value = reader["isDelete"].ToString();
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

    public void Update(OrderDetailModel orderDetailModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();
            sql = @"
                UPDATE  order_detail 
                SET     
orderId=@orderId,
productId=@productId,
orderDetailUnitPrice=@orderDetailUnitPrice,
orderDetailQuantity=@orderDetailQuantity,
orderDetailDiscount=@orderDetailDiscount,
isDelete=@isDelete

                WHERE   orderDetailId    =   @orderDetailId";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@orderDetailId",
                    Value = orderDetailModel.OrderDetailKey
                },
                new()
                {
                    Key = "@orderId",
                    Value = orderDetailModel.OrderKey
                },
                new()
                {
                    Key = "@productId",
                    Value = orderDetailModel.ProductKey
                },
                new()
                {
                    Key = "@orderDetailUnitPrice",
                    Value = orderDetailModel.OrderDetailUnitPrice
                },
                new()
                {
                    Key = "@orderDetailQuantity",
                    Value = orderDetailModel.OrderDetailQuantity
                },
                new()
                {
                    Key = "@orderDetailDiscount",
                    Value = orderDetailModel.OrderDetailDiscount
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

    public void Delete(OrderDetailModel orderDetailModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();
            sql = @"
                UPDATE  order_detail 
                SET     isDelete    =   1
                WHERE   orderDetailId    =   @orderDetailId";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@orderDetailId",
                    Value = orderDetailModel.OrderDetailKey
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