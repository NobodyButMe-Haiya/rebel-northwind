using MySql.Data.MySqlClient;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Repository.Dashboard;

public class DashboardRepository
{
    private readonly SharedUtil _sharedUtil;

    public DashboardRepository(IHttpContextAccessor httpContextAccessor)
    {
        _sharedUtil = new SharedUtil(httpContextAccessor);
    }

    public Dictionary<string, int> GetCrossTabOrder()
    {
        Dictionary<string, int> crossTabOrder = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            const string sql = @"
            SELECT  sum(if(orderStatusId=1,1,0)) as `new`, 
                    sum(if(orderStatusId=2,1,0)) as `invoiced`, 
                    sum(if(orderStatusId=3,1,0)) as `shipped`, 
                    sum(if(orderStatusId=4,1,0)) as `closed` 
            FROM    `order` 
            WHERE   tenantId  = @tenantId LIMIT 1";
            MySqlCommand mySqlCommand = new(sql, connection);
            mySqlCommand.Parameters.AddWithValue("@tenantId", _sharedUtil.GetTenantId());
            var mySqlDataReader = mySqlCommand.ExecuteReader();

            while (mySqlDataReader.Read())
            {
                crossTabOrder.Add("New", mySqlDataReader.GetInt32("new"));


                crossTabOrder.Add("Invoiced", mySqlDataReader.GetInt32("invoiced"));


                crossTabOrder.Add("Shipped", mySqlDataReader.GetInt32("shipped"));


                crossTabOrder.Add("Closed", mySqlDataReader.GetInt32("closed"));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetSystemException(ex);
        }

        return crossTabOrder;
    }

    public int GetTotalEmployee()
    {
        var totalEmployee = 0;
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            const string sql = @"
            SELECT  COUNT(*) AS totalEmployee 
            FROM    employee 
            WHERE   tenantId  = @tenantId LIMIT 1";
            MySqlCommand mySqlCommand = new(sql, connection);
            mySqlCommand.Parameters.AddWithValue("@tenantId", _sharedUtil.GetTenantId());
            var mySqlDataReader = mySqlCommand.ExecuteReader();
            while (mySqlDataReader.Read())
            {
                totalEmployee = mySqlDataReader.GetInt32("totalEmployee");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetSystemException(ex);
        }

        return totalEmployee;
    }

    public int GetTotalCustomer()
    {
        var totalCustomer = 0;
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            const string sql = @"
            SELECT  COUNT(*) AS totalCustomer 
            FROM    customer 
            WHERE   tenantId  = @tenantId LIMIT 1";
            MySqlCommand mySqlCommand = new(sql, connection);
            mySqlCommand.Parameters.AddWithValue("@tenantId", _sharedUtil.GetTenantId());
            var mySqlDataReader = mySqlCommand.ExecuteReader();
            while (mySqlDataReader.Read())
            {
                totalCustomer = mySqlDataReader.GetInt32("totalCustomer");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetSystemException(ex);
        }

        return totalCustomer;
    }

    public int GetTotalShipper()
    {
        var totalShipper = 0;
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            string sql = @"
            SELECT  COUNT(*) AS totalShipper
            FROM    shipper 
            WHERE   tenantId  = @tenantId LIMIT 1";
            MySqlCommand mySqlCommand = new(sql, connection);
            mySqlCommand.Parameters.AddWithValue("@tenantId", _sharedUtil.GetTenantId());
            var mySqlDataReader = mySqlCommand.ExecuteReader();
            while (mySqlDataReader.Read())
            {
                totalShipper = mySqlDataReader.GetInt32("totalShipper");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetSystemException(ex);
        }

        return totalShipper;
    }

    public int GetTotalSupplier()
    {
        var totalSupplier = 0;
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            const string sql = @"
            SELECT  COUNT(*) AS totalSupplier
            FROM    supplier
            WHERE   tenantId  = @tenantId LIMIT 1";
            MySqlCommand mySqlCommand = new(sql, connection);
            mySqlCommand.Parameters.AddWithValue("@tenantId", _sharedUtil.GetTenantId());
            var mySqlDataReader = mySqlCommand.ExecuteReader();
            while (mySqlDataReader.Read())
            {
                totalSupplier = mySqlDataReader.GetInt32("totalSupplier");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetSystemException(ex);
        }

        return totalSupplier;
    }

    public int GetTotalProduct()
    {
        var totalProduct = 0;
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            const string sql = @"
            SELECT  COUNT(*) AS totalProduct
            FROM    product
            WHERE   tenantId  = @tenantId LIMIT 1";
            MySqlCommand mySqlCommand = new(sql, connection);
            mySqlCommand.Parameters.AddWithValue("@tenantId", _sharedUtil.GetTenantId());
            var mySqlDataReader = mySqlCommand.ExecuteReader();
            while (mySqlDataReader.Read())
            {
                totalProduct = mySqlDataReader.GetInt32("totalProduct");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetSystemException(ex);
        }

        return totalProduct;
    }

    public Dictionary<string, double> GetTotalOrder()
    {
        Dictionary<string, double> crossTabOrder = new();
        using var connection = SharedUtil.GetConnection();
        connection.Open();
        const string sql = @"
            SELECT  sum(if(orderStatusId=1,((orderDetailUnitPrice * order_detail.orderDetailQuantity) - order_detail.orderDetailDiscount),0)) as `new`, 
                    sum(if(orderStatusId=2,((orderDetailUnitPrice * order_detail.orderDetailQuantity) - order_detail.orderDetailDiscount),0)) as `invoiced`, 
                    sum(if(orderStatusId=3,((orderDetailUnitPrice * order_detail.orderDetailQuantity) - order_detail.orderDetailDiscount),0)) as `shipped`, 
                    sum(if(orderStatusId=4,((orderDetailUnitPrice * order_detail.orderDetailQuantity) - order_detail.orderDetailDiscount),0))  as `closed`
            FROM    `order_detail` 
            join    `order` 
            using(orderId)  
            WHERE   `order`.tenantId  = @tenantId 
            AND     `order`.isDelete != 1
            LIMIT 1";
        MySqlCommand mySqlCommand = new(sql, connection);
        mySqlCommand.Parameters.AddWithValue("@tenantId", _sharedUtil.GetTenantId());
        var mySqlDataReader = mySqlCommand.ExecuteReader();
        while (mySqlDataReader.Read())
        {
            crossTabOrder.Add("New", mySqlDataReader.GetDouble("new"));

            crossTabOrder.Add("Invoiced", mySqlDataReader.GetDouble("invoiced"));

            crossTabOrder.Add("Shipped", mySqlDataReader.GetDouble("shipped"));

            crossTabOrder.Add("Closed", mySqlDataReader.GetDouble("closed"));
        }

        return crossTabOrder;
    }

    public List<ProductStatistic> GetProductSales()
    {
        List<ProductStatistic> productStatistic = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            const string sql = @"
            SELECT  count(*) as total, 
                    productName as name
            from    order_detail 
            join    product 
            using(productId) 
 
            WHERE   `order_detail`.tenantId  = @tenantId 
            AND     `order_detail`.isDelete != 1 
            group by productId 
            ORDER BY `total` DESC
            LIMIT 50 ";
            MySqlCommand mySqlCommand = new(sql, connection);
            mySqlCommand.Parameters.AddWithValue("@tenantId", _sharedUtil.GetTenantId());
            var mySqlDataReader = mySqlCommand.ExecuteReader();
            while (mySqlDataReader.Read())
            {
                productStatistic.Add(new ProductStatistic
                {
                    x = mySqlDataReader.GetString("name"),
                    y = mySqlDataReader.GetInt32("total")
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetSystemException(ex);
        }

        return productStatistic;
    }

    public class ProductStatistic
    {
        public string? x { get; set; }
        public int y { get; set; }
    }
}