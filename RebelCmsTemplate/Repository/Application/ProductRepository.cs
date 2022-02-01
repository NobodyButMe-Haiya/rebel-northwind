using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Application;
using RebelCmsTemplate.Models.Shared;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Repository.Application;

public class ProductRepository
{
    private readonly SharedUtil _sharedUtil;

    public ProductRepository(IHttpContextAccessor httpContextAccessor)
    {
        _sharedUtil = new SharedUtil(httpContextAccessor);
    }

    public int Create(ProductModel productModel)
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
                @"INSERT INTO product (productId,tenantId,supplierId,productCategoryId,productTypeId,productName,productDescription,productQuantityPerUnit,productCostPrice,productSellingPrice,productUnitsInStock,productUnitsOnOrder,productReOrderLevel,isDelete) VALUES (null,@tenantId,@supplierId,@productCategoryId,@productTypeId,@productName,@productDescription,@productQuantityPerUnit,@productCostPrice,@productSellingPrice,@productUnitsInStock,@productUnitsOnOrder,@productReOrderLevel,@isDelete);";
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
                    Value = productModel.SupplierKey
                },
                new()
                {
                    Key = "@productCategoryId",
                    Value = productModel.ProductCategoryKey
                },
                new()
                {
                    Key = "@productTypeId",
                    Value = productModel.ProductTypeKey
                },
                new()
                {
                    Key = "@productName",
                    Value = productModel.ProductName
                },
                new()
                {
                    Key = "@productDescription",
                    Value = productModel.ProductDescription
                },
                new()
                {
                    Key = "@productQuantityPerUnit",
                    Value = productModel.ProductQuantityPerUnit
                },
                new()
                {
                    Key = "@productCostPrice",
                    Value = productModel.ProductCostPrice
                },
                new()
                {
                    Key = "@productSellingPrice",
                    Value = productModel.ProductSellingPrice
                },
                new()
                {
                    Key = "@productUnitsInStock",
                    Value = productModel.ProductUnitsInStock
                },
                new()
                {
                    Key = "@productUnitsOnOrder",
                    Value = productModel.ProductUnitsOnOrder
                },
                new()
                {
                    Key = "@productReOrderLevel",
                    Value = productModel.ProductReOrderLevel
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

    public List<ProductModel> Read()
    {
        List<ProductModel> productModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql = @"
            SELECT      *
            FROM        product

            JOIN        supplier 
            USING(supplierId)

            JOIN        product_category 
            USING(productCategoryId)

            JOIN        product_type 
            USING(productTypeId)

            WHERE       product.isDelete != 1
            AND         product.tenantId = @tenantId
            ORDER BY    productId DESC ";
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
                    productModels.Add(new ProductModel
                    {
                        ProductKey = Convert.ToUInt32(reader["productId"]),
                        SupplierName = reader["supplierName"].ToString(),
                        SupplierKey = Convert.ToUInt32(reader["supplierId"]),
                        ProductCategoryName = reader["productCategoryName"].ToString(),
                        ProductCategoryKey = Convert.ToUInt32(reader["productCategoryId"]),
                        ProductTypeName = reader["productTypeName"].ToString(),
                        ProductTypeKey = Convert.ToUInt32(reader["productTypeId"]),
                        ProductName = reader["productName"].ToString(),
                        ProductDescription = reader["productDescription"].ToString(),
                        ProductQuantityPerUnit = reader["productQuantityPerUnit"].ToString(),
                        ProductCostPrice = reader["productCostPrice"] != DBNull.Value
                            ? Convert.ToDouble(reader["productCostPrice"])
                            : 0,
                        ProductSellingPrice = reader["productCostPrice"] != DBNull.Value
                            ? Convert.ToDouble(reader["productSellingPrice"])
                            : 0,
                        ProductUnitsInStock = reader["productUnitsInStock"] != DBNull.Value
                            ? Convert.ToDouble(reader["productUnitsInStock"])
                            : 0,
                        ProductUnitsOnOrder = reader["productUnitsOnOrder"] != DBNull.Value
                            ? Convert.ToDouble(reader["productUnitsOnOrder"])
                            : 0,
                        ProductReOrderLevel = reader["productReOrderLevel"] != DBNull.Value
                            ? Convert.ToDouble(reader["productReOrderLevel"])
                            : 0
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

        return productModels;
    }

    public List<ProductModel> Search(string search)
    {
        List<ProductModel> productModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
            SELECT  *
            FROM    product 

            JOIN supplier 
            USING(supplierId)

            JOIN product_category 
            USING(productCategoryId)

            JOIN product_type 
            USING(productTypeId)

            WHERE   product.isDelete != 1
            AND     product.tenantId = @tenantId
            AND (
                    supplier.supplierName LIKE CONCAT('%',@search,'%') OR
                    supplier.supplierName LIKE CONCAT('%',@search,'%') OR
                    supplier.supplierName LIKE CONCAT('%',@search,'%') OR
                    supplier.supplierName LIKE CONCAT('%',@search,'%') OR
                    supplier.supplierName LIKE CONCAT('%',@search,'%') OR
                    supplier.supplierName LIKE CONCAT('%',@search,'%') OR
                    supplier.supplierName LIKE CONCAT('%',@search,'%') OR
                    supplier.supplierName LIKE CONCAT('%',@search,'%') OR
                    supplier.supplierName LIKE CONCAT('%',@search,'%') OR
                    supplier.supplierName LIKE CONCAT('%',@search,'%') OR
                    supplier.supplierName LIKE CONCAT('%',@search,'%') OR
                    supplier.supplierName LIKE CONCAT('%',@search,'%') OR
                    supplier.supplierName LIKE CONCAT('%',@search,'%') OR
                    supplier.supplierName LIKE CONCAT('%',@search,'%') OR
                    product_category.productCategoryName LIKE CONCAT('%',@search,'%') OR
                    product_category.productCategoryName LIKE CONCAT('%',@search,'%') OR
                    product_category.productCategoryName LIKE CONCAT('%',@search,'%') OR
                    product_category.productCategoryName LIKE CONCAT('%',@search,'%') OR
                    product_category.productCategoryName LIKE CONCAT('%',@search,'%') OR
                    product_type.productTypeName LIKE CONCAT('%',@search,'%') OR
                    product_type.productTypeName LIKE CONCAT('%',@search,'%') OR
                    product_type.productTypeName LIKE CONCAT('%',@search,'%') OR
                    product_type.productTypeName LIKE CONCAT('%',@search,'%') OR
                    product_type.productTypeName LIKE CONCAT('%',@search,'%') OR
                    product_type.productTypeName LIKE CONCAT('%',@search,'%') OR
                    product.productName LIKE CONCAT('%',@search,'%') OR
                    product.productDescription LIKE CONCAT('%',@search,'%') OR
                    product.productQuantityPerUnit LIKE CONCAT('%',@search,'%') OR
                    product.productCostPrice LIKE CONCAT('%',@search,'%') OR
                    product.productSellingPrice LIKE CONCAT('%',@search,'%') OR
                    product.productUnitsInStock LIKE CONCAT('%',@search,'%') OR
                    product.productUnitsOnOrder LIKE CONCAT('%',@search,'%') OR
                    product.productReOrderLevel LIKE CONCAT('%',@search,'%')
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
                    productModels.Add(new ProductModel
                    {
                        SupplierName = reader["supplierName"].ToString(),
                        SupplierKey = Convert.ToUInt32(reader["supplierId"]),
                        ProductCategoryName = reader["productCategoryName"].ToString(),
                        ProductCategoryKey = Convert.ToUInt32(reader["productCategoryId"]),
                        ProductTypeName = reader["productTypeName"].ToString(),
                        ProductTypeKey = Convert.ToUInt32(reader["productTypeId"]),
                        ProductName = reader["productName"].ToString(),
                        ProductDescription = reader["productDescription"].ToString(),
                        ProductQuantityPerUnit = reader["productQuantityPerUnit"].ToString(),
                        ProductCostPrice = Convert.ToDouble(reader["productCostPrice"]),
                        ProductSellingPrice = Convert.ToDouble(reader["productSellingPrice"]),
                        ProductUnitsInStock = Convert.ToDouble(reader["productUnitsInStock"]),
                        ProductUnitsOnOrder = Convert.ToDouble(reader["productUnitsOnOrder"]),
                        ProductReOrderLevel = Convert.ToDouble(reader["productReOrderLevel"])
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

        return productModels;
    }

    public ProductModel GetSingle(ProductModel productModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
            SELECT  *
            FROM    product

            JOIN supplier 
            USING(supplierId)

            JOIN product_category 
            USING(productCategoryId)

            JOIN product_type 
            USING(productTypeId)

            WHERE   product.isDelete    !=  1
            AND     product.tenantId    =   @tenantId
            AND     product.productId   =   @productId
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
                    Key = "@productId",
                    Value = productModel.ProductKey
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
                    productModel = new ProductModel
                    {
                        ProductKey = Convert.ToUInt32(reader["productId"]),
                        SupplierKey = Convert.ToUInt32(reader["supplierId"]),
                        ProductCategoryKey = Convert.ToUInt32(reader["productCategoryId"]),
                        ProductTypeKey = Convert.ToUInt32(reader["productTypeId"]),
                        ProductName = reader["productName"].ToString(),
                        ProductDescription = reader["productDescription"].ToString(),
                        ProductQuantityPerUnit = reader["productQuantityPerUnit"].ToString(),
                        ProductCostPrice = Convert.ToDouble(reader["productCostPrice"]),
                        ProductSellingPrice = Convert.ToDouble(reader["productSellingPrice"]),
                        ProductUnitsInStock = Convert.ToDouble(reader["productUnitsInStock"]),
                        ProductUnitsOnOrder = Convert.ToDouble(reader["productUnitsOnOrder"]),
                        ProductReOrderLevel = Convert.ToDouble(reader["productReOrderLevel"])
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

        return productModel;
    }

    public uint GetDefault()
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        uint productId;
        try
        {
            connection.Open();
            sql += @"
            SELECT  productId
            FROM    product

            WHERE   isDelete    !=  1
            AND     tenantId    =   @tenantId
            AND     isDefault  = 1
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
            productId = (uint) mySqlCommand.ExecuteScalar();

            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
            throw new Exception(ex.Message);
        }

        return productId;
    }

    public byte[] GetExcel()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Administrator > Product ");
        worksheet.Cell(1, 1).Value = "Supplier Name";
        worksheet.Cell(1, 2).Value = "Product Category Name";
        worksheet.Cell(1, 3).Value = "Product Type Name";
        worksheet.Cell(1, 4).Value = "Name";
        worksheet.Cell(1, 5).Value = "Description";
        worksheet.Cell(1, 6).Value = "Quantity Per Unit";
        worksheet.Cell(1, 7).Value = "Cost Price";
        worksheet.Cell(1, 8).Value = "Selling Price";
        worksheet.Cell(1, 9).Value = "Units In Stock";
        worksheet.Cell(1, 10).Value = "Units On Order";
        worksheet.Cell(1, 11).Value = "Re Order Level";
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
                    worksheet.Cell(currentRow, 2).Value = reader["productCategoryName"].ToString();
                    worksheet.Cell(currentRow, 3).Value = reader["productTypeName"].ToString();
                    worksheet.Cell(currentRow, 4).Value = reader["productName"].ToString();
                    worksheet.Cell(currentRow, 5).Value = reader["productDescription"].ToString();
                    worksheet.Cell(currentRow, 6).Value = reader["productQuantityPerUnit"].ToString();
                    worksheet.Cell(currentRow, 7).Value = reader["productCostPrice"].ToString();
                    worksheet.Cell(currentRow, 8).Value = reader["productSellingPrice"].ToString();
                    worksheet.Cell(currentRow, 9).Value = reader["productUnitsInStock"].ToString();
                    worksheet.Cell(currentRow, 10).Value = reader["productUnitsOnOrder"].ToString();
                    worksheet.Cell(currentRow, 11).Value = reader["productReOrderLevel"].ToString();
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

    public void Update(ProductModel productModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();
            sql = @"
            UPDATE  product 
            SET     tenantId                =   @tenantId,
                    supplierId              =   @supplierId,
                    productCategoryId       =   @productCategoryId,
                    productTypeId           =   @productTypeId,
                    productName             =   @productName,
                    productDescription      =   @productDescription,
                    productQuantityPerUnit  =   @productQuantityPerUnit,
                    productCostPrice        =   @productCostPrice,
                    productSellingPrice     =   @productSellingPrice,
                    productUnitsInStock     =   @productUnitsInStock,
                    productUnitsOnOrder     =   @productUnitsOnOrder,
                    productReOrderLevel     =   @productReOrderLevel
            WHERE   productId               =   @productId";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@productId",
                    Value = productModel.ProductKey
                },
                new()
                {
                    Key = "@tenantId",
                    Value = _sharedUtil.GetTenantId()
                },
                new()
                {
                    Key = "@supplierId",
                    Value = productModel.SupplierKey
                },
                new()
                {
                    Key = "@productCategoryId",
                    Value = productModel.ProductCategoryKey
                },
                new()
                {
                    Key = "@productTypeId",
                    Value = productModel.ProductTypeKey
                },
                new()
                {
                    Key = "@productName",
                    Value = productModel.ProductName
                },
                new()
                {
                    Key = "@productDescription",
                    Value = productModel.ProductDescription
                },
                new()
                {
                    Key = "@productQuantityPerUnit",
                    Value = productModel.ProductQuantityPerUnit
                },
                new()
                {
                    Key = "@productCostPrice",
                    Value = productModel.ProductCostPrice
                },
                new()
                {
                    Key = "@productSellingPrice",
                    Value = productModel.ProductSellingPrice
                },
                new()
                {
                    Key = "@productUnitsInStock",
                    Value = productModel.ProductUnitsInStock
                },
                new()
                {
                    Key = "@productUnitsOnOrder",
                    Value = productModel.ProductUnitsOnOrder
                },
                new()
                {
                    Key = "@productReOrderLevel",
                    Value = productModel.ProductReOrderLevel
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

    public void Delete(ProductModel productModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();
            sql = @"
                UPDATE  product 
                SET     isDelete    =   1
                WHERE   productId    =   @productId";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@productId",
                    Value = productModel.ProductKey
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