using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Administrator;
using RebelCmsTemplate.Models.Shared;
using System.Text.Json;

namespace RebelCmsTemplate.Util;

public class SharedUtil
{
    public const string NoRecord = "no record";
    public const string RecordCreated = "Record Created";
    public const string RecordUpdated = "Record Updated";
    public const string RecordDeleted = "Record Deleted";
    public const string UserErrorNotification = "SYSTEM ERROR";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SharedUtil(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public static MySqlConnection GetConnection()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables()
            .Build();
        return new MySqlConnection(configuration.GetSection("ConnectionStrings:DefaultConnection").Value);
    }

    public void SetQueryException(string? stringSql, MySqlException exceptionText)
    {
        using var connection = GetConnection();
        try
        {
            connection.Open();


            const string sql =
                " INSERT INTO log  (logId,tenantId,userId,logUserName,logQuery,logError,LogDateTime) VALUES (null,@userId,@tenantId,@userName,@logQuery,@logError,NOW());";

            MySqlCommand mySqlCommand = new(sql, connection);

            mySqlCommand.Parameters.AddWithValue("@userId", GetUserId());
            mySqlCommand.Parameters.AddWithValue("@tenantId", GetTenantId());
            mySqlCommand.Parameters.AddWithValue("@userName", GetUserName());
            mySqlCommand.Parameters.AddWithValue("@logQuery", stringSql);
            mySqlCommand.Parameters.AddWithValue("@logError", exceptionText.ToString());

            mySqlCommand.ExecuteNonQuery();

            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }

    public void SetSystemException(Exception exceptionText)
    {
        using var connection = GetConnection();
        try
        {
            connection.Open();


            const string sql =
                " INSERT INTO log (logId,tenantId,userId,logUserName,logQuery,logError,LogDateTime) VALUES (null,@userId,@tenantId,@userName,@logQuery,@logError,NOW());";
            MySqlCommand mySqlCommand = new(sql, connection);
            mySqlCommand.Parameters.AddWithValue("@userId", GetUserId());
            mySqlCommand.Parameters.AddWithValue("@tenantId", GetTenantId());
            mySqlCommand.Parameters.AddWithValue("@userName", GetUserName());
            mySqlCommand.Parameters.AddWithValue("@logQuery", "");
            mySqlCommand.Parameters.AddWithValue("@logError", exceptionText.ToString());
            mySqlCommand.ExecuteNonQuery();

            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }

    public void SetSystemLog(string message)
    {
        using var connection = GetConnection();
        try
        {
            connection.Open();


            const string sql = "INSERT INTO log_system VALUES (null,@tenantId,@tenantName,@logSystemQuery,NOW());";
            MySqlCommand mySqlCommand = new(sql, connection);

            mySqlCommand.Parameters.AddWithValue("@tenantId", GetTenantId());
            mySqlCommand.Parameters.AddWithValue("@tenantName", GetTenantName());
            mySqlCommand.Parameters.AddWithValue("@logSystemQuery", message);
            mySqlCommand.ExecuteNonQuery();

            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            SetSystemException(ex);
        }
    }

    public NavigationModel GetNavigation(string leafName)
    {
        NavigationModel navigationModel = new();
        const string sql = @"
            SELECT * 
            FROM leaf 
            JOIN folder 
            USING (folderId) 
            WHERE leafFilename=@leafName 
            LIMIT 1";
        using var connection = GetConnection();
        try
        {
            connection.Open();
            MySqlCommand mySqlCommand = new(sql, connection);
            mySqlCommand.Parameters.AddWithValue("@leafName", leafName);

            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    navigationModel.LeafCheckKey = Convert.ToInt32(reader["leafId"]);
                    navigationModel.FolderName = reader["folderName"].ToString();
                    navigationModel.FolderIcon = reader["folderIcon"].ToString();
                    navigationModel.LeafName = reader["leafName"].ToString();
                    navigationModel.LeafIcon = reader["leafIcon"].ToString();
                }
            }

            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            SetSystemException(ex);
        }

        return navigationModel;
    }

    public int GetRoleId()
    {
        return _httpContextAccessor.HttpContext?.Session.GetInt32("roleId") ?? 0;
    }

    public int GetTenantId()
    {
        return _httpContextAccessor.HttpContext?.Session.GetInt32("tenantId") ?? 0;
    }

    private int GetUserId()
    {
        return _httpContextAccessor.HttpContext?.Session.GetInt32("userId") ?? 0;
    }

    public void SetSqlSession(string sql)
    {
        _httpContextAccessor.HttpContext?.Session.SetString("sql", sql);
    }

    public void SetSqlSession(string sql, List<ParameterModel> parameterModels)
    {
        _httpContextAccessor.HttpContext?.Session.Remove("sql");
        _httpContextAccessor.HttpContext?.Session.Remove("parameter");

        _httpContextAccessor.HttpContext?.Session.SetString("sql", sql);
        _httpContextAccessor.HttpContext?.Session.SetString("parameter", JsonSerializer.Serialize(parameterModels));
    }

    /// <summary>
    /// Replace parameter value like php str_replace  for bind parameter in SQL
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="parameterModels"></param>
    /// <returns></returns>
    public static string GetSqlSessionValue(string sql, List<ParameterModel> parameterModels)
    {
        List<string?> originalChar = new();
        List<string?> replaceWith = new();
        foreach (var parameterModel in parameterModels)
        {
            originalChar.Add(parameterModel.Key ?? "unknown key");

            if (parameterModel.Value != null)
            {
                // here we need to identify either it was string or not 
                var stringReplace = Type.GetTypeCode(parameterModel.Value.GetType()).Equals(TypeCode.String)
                    ? "'" + parameterModel.Value.ToString() + "'"
                    : parameterModel.Value.ToString();
                replaceWith.Add(stringReplace);
            }
            else
            {
                // to prevent ide invert if 
                replaceWith.Add("unknown value");
            }
        }

        originalChar.ForEach(x =>
        {
            if (x == null)
                return;
            sql = sql.Replace(x, replaceWith[originalChar.IndexOf(x)]);
        });
        return sql;
    }

    public string? GetSqlSession()
    {
        return _httpContextAccessor.HttpContext?.Session.GetString("sql");
    }

    public List<ParameterModel>? GetListSqlParameter()
    {
        List<ParameterModel> parameterModels = new();
        var json = _httpContextAccessor.HttpContext?.Session.GetString("parameter");
        return json != null ? JsonSerializer.Deserialize<List<ParameterModel>>(json) : parameterModels;
    }

    /// <summary>
    /// The point here because we may re-use in user form  not just for check access
    /// </summary>
    /// <param name="userModel"></param>
    public void SetSession(UserModel userModel)
    {
        _httpContextAccessor.HttpContext?.Session.SetInt32("userId", Convert.ToInt32(userModel.UserKey));
        _httpContextAccessor.HttpContext?.Session.SetInt32("roleId", Convert.ToInt32(userModel.RoleKey));
        _httpContextAccessor.HttpContext?.Session.SetInt32("tenantId", Convert.ToInt32(userModel.TenantKey));
        if (userModel.UserName == null)
            return;
        _httpContextAccessor.HttpContext?.Session.SetString("userName", userModel.UserName);
    }

    public void GetRemoveSession()
    {
        _httpContextAccessor.HttpContext?.Session.Clear();
        _httpContextAccessor.HttpContext?.Session.Remove("userId");
        _httpContextAccessor.HttpContext?.Session.Remove("roleId");
        _httpContextAccessor.HttpContext?.Session.Remove("tenantId");
        _httpContextAccessor.HttpContext?.Session.Remove("userName");
    }

    public string? GetUserName()
    {
        var userName = "";

        if (_httpContextAccessor.HttpContext?.Session.GetString("userName") != null)
        {
            userName = _httpContextAccessor.HttpContext?.Session.GetString("userName");
        }

        return userName;
    }

    private string? GetTenantName()
    {
        var tenantName = "";

        if (_httpContextAccessor.HttpContext?.Session.GetString("tenantName") != null)
        {
            tenantName = _httpContextAccessor.HttpContext?.Session.GetString("tenantName");
        }

        return tenantName;
    }

    public static async Task<byte[]> GetByteArrayFromImageAsync(IFormFile file)
    {
        await using var target = new MemoryStream();
        await file.CopyToAsync(target);
        return target.ToArray();
    }

    public static string GetImageString(byte[] data, string type = "png")
    {
        var base64String = Convert.ToBase64String(data);
        return "data:image/" + type + ";base64," + base64String;
    }

    public static string Return500()
    {
        const string x = "<div id=\"error\">" +
                         "<div class=\"error-page container\">" +
                         "<div class=\"col-md-8 col-12 offset-md-2\">" +
                         "<img class=\"img-error\" src=\"assets/images/samples/error-500.png\" alt=\"Not Found\">" +
                         "<div class=\"text-center\">" +
                         "<h1 class=\"error-title\">System Error</h1>" +
                         "<p class=\"fs-5 text-gray-600\">The website is currently un-available. Try again later or contact the developer.</p>" +
                         "<a href =\"index.html\" class=\"btn btn-lg btn-outline-primary mt-3\">Go Home</a>" +
                         "</div>" +
                         "</div>" +
                         "</div>";
        return x;
    }
}