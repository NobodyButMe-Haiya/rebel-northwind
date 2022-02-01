using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Administrator;
using RebelCmsTemplate.Models.Shared;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Repository.Administrator;

public class TenantRepository
{
    private readonly SharedUtil _sharedUtil;

    public TenantRepository(IHttpContextAccessor httpContextAccessor)
    {
        _sharedUtil = new SharedUtil(httpContextAccessor);
    }

    public int Create(TenantModel tenantModel)
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
                INSERT INTO tenant VALUES (null,@tenantName,0);";
            MySqlCommand mySqlCommand = new(sql, connection);

            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@tenantName",
                    Value = tenantModel.TenantName
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

    public List<TenantModel> Read()
    {
        List<TenantModel> tenantModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql = @"
                SELECT      *
                FROM        tenant
                WHERE       isDelete !=1
                ORDER BY    tenantId DESC ";
            MySqlCommand mySqlCommand = new(sql, connection);
            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    tenantModels.Add(new TenantModel
                    {
                        TenantName = reader["tenantName"].ToString(),
                        TenantKey = Convert.ToUInt32(reader["tenantId"])
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


        return tenantModels;
    }

    public List<TenantModel> Search(string search)
    {
        List<TenantModel> tenantModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
                SELECT  *
                FROM    tenant
                WHERE   isDelete != 1
                AND     tenantName LIKE CONCAT('%',@search,'%'); ";
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
                    tenantModels.Add(new TenantModel
                    {
                        TenantName = reader["tenantName"].ToString(),
                        TenantKey = Convert.ToUInt32(reader["tenantId"])
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


        return tenantModels;
    }

    public byte[] GetExcel()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Administrator > Tenant ");
        worksheet.Cell(1, 1).Value = "No";
        worksheet.Cell(1, 2).Value = "Tenant";
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
                    worksheet.Cell(currentRow, 2).Value = reader["tenantName"].ToString();
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

    public void Update(TenantModel tenantModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();

            sql = @"
                UPDATE  tenant
                SET     tenantName  =   @tenantName
                WHERE   tenantId    =   @tenantId ";
            MySqlCommand mySqlCommand = new(sql, connection);
            mySqlCommand.Parameters.AddWithValue("@tenantName", tenantModel.TenantName);
            mySqlCommand.Parameters.AddWithValue("@tenantId", tenantModel.TenantKey);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@tenantId",
                    Value = _sharedUtil.GetTenantId()
                },
                new()
                {
                    Key = "@tenantName",
                    Value = tenantModel.TenantName
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

    public void Delete(TenantModel tenantModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();

            sql = @"
                UPDATE  tenant
                SET     isDelete    =   1
                WHERE   tenantId    =   @tenantId;";
            MySqlCommand mySqlCommand = new(sql, connection);

            mySqlCommand.Parameters.AddWithValue("@tenantId", tenantModel.TenantKey);
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