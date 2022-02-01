using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Setting;
using RebelCmsTemplate.Models.Shared;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Repository.Setting;

public class ProductCategoryRepository
{
    private readonly SharedUtil _sharedUtil;

    public ProductCategoryRepository(IHttpContextAccessor httpContextAccessor)
    {
        _sharedUtil = new SharedUtil(httpContextAccessor);
    }

    public int Create(ProductCategoryModel productCategoryModel)
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
            INSERT INTO product_category  (
                productCategoryId,      tenantId,
                productCategoryName,   isDelete
            ) VALUES (
                null,                   @tenantId,
                @productCategoryName,   0
            );";
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
                    Key = "@productCategoryName",
                    Value = productCategoryModel.ProductCategoryName
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

    public List<ProductCategoryModel> Read()
    {
        List<ProductCategoryModel> productCategoryModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
            SELECT      *
            FROM        product_category
            WHERE       isDelete != 1
            AND         tenantId = @tenantId
            ORDER BY    productCategoryId DESC ";
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
                    productCategoryModels.Add(new ProductCategoryModel
                    {
                        ProductCategoryName = reader["productCategoryName"].ToString(),
                        ProductCategoryKey = Convert.ToUInt32(reader["productCategoryId"])
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


        return productCategoryModels;
    }

    public List<ProductCategoryModel> Search(string search)
    {
        List<ProductCategoryModel> productCategoryModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
            SELECT  *
            FROM    product_category
            WHERE   tenantId = @tenantId
            AND     isDelete != 1
            AND     productCategoryName LIKE CONCAT('%',@search,'%') ";
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
                    productCategoryModels.Add(new ProductCategoryModel
                    {
                        ProductCategoryName = reader["productCategoryName"].ToString(),
                        ProductCategoryKey = Convert.ToUInt32(reader["productCategoryId"])
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


        return productCategoryModels;
    }

    public uint GetDefault()
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        uint productCategoryId;
        try
        {
            connection.Open();
            sql += @"
            SELECT  productCategoryId
            FROM    product_category
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

            productCategoryId = (uint)mySqlCommand.ExecuteScalar();

            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
            throw new Exception(ex.Message);
        }


        return productCategoryId;
    }

    public byte[] GetExcel()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Setting > Product Category");

        worksheet.Cell(1, 1).Value = "No";
        worksheet.Cell(1, 2).Value = "Category";
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
                    worksheet.Cell(currentRow, 2).Value = reader["productCategoryName"].ToString();
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

    public void Update(ProductCategoryModel productCategoryModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();

            sql += @"
            UPDATE  product_category
            SET     productCategoryName =   @productCategoryName
            WHERE   productCategoryId   =   @productCategoryId ";
            MySqlCommand mySqlCommand = new(sql, connection);

            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@productCategoryName",
                    Value = productCategoryModel.ProductCategoryName
                },
                new()
                {
                    Key = "@productCategoryId",
                    Value = productCategoryModel.ProductCategoryKey
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

    public void Delete(ProductCategoryModel productCategoryModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();

            sql += @"
            UPDATE  product_category
            SET     isDelete = 1
            WHERE   productCategoryId  = @productCategoryId;";
            MySqlCommand mySqlCommand = new(sql, connection);

            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@productCategoryId",
                    Value = productCategoryModel.ProductCategoryKey
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