using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Administrator;
using System.Security.Cryptography;
using System.Text;
using RebelCmsTemplate.Enum;

namespace RebelCmsTemplate.Util;

public class CheckAccessUtil
{
    private readonly SharedUtil _sharedUtil;

    public CheckAccessUtil(IHttpContextAccessor httpContextAccessor) =>
        _sharedUtil = new SharedUtil(httpContextAccessor);

    public bool GetCheckAccessFromWeb(string userName, string? userPassword)
    {
        var access = false;

        const string sql = @"
            SELECT  * 
            FROM    user 
            WHERE   isDelete        !=  1 
            AND     userName        =   @userName 
            AND     userPassword    =   @userPassword
            LIMIT   1";

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            MySqlCommand mySqlCommand = new(sql, connection);
            mySqlCommand.Parameters.AddWithValue("@userName", userName);
            mySqlCommand.Parameters.AddWithValue("@userPassword", CalculateSha256(userPassword));


            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    access = true;

                    UserModel userModel = new()
                    {
                        UserKey = Convert.ToUInt32(reader["userId"]),
                        TenantKey = Convert.ToUInt32(reader["tenantId"]),
                        RoleKey = Convert.ToUInt32(reader["roleId"]),
                        UserName = reader["userName"].ToString()
                    };
                    _sharedUtil.SetSession(userModel);

                    break;
                }
            }

            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetSystemException(ex);
        }

        return access;
    }

   
    public static string CalculateSha256(string? rawData)
    {
        StringBuilder builder = new();
        // Create a SHA256   
        using var sha256Hash = SHA256.Create();
        // ComputeHash - returns byte array  
        if (rawData == null) return builder.ToString();
        var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

        // Convert byte array to a string   

        foreach (var t in bytes)
        {
            builder.Append(t.ToString("x2"));
        }

        return builder.ToString();

    }

    private static string GetFieldCheck(AuthenticationEnum authenticationEnum)
    {
        var fieldAccess = authenticationEnum switch
        {
            AuthenticationEnum.CREATE_ACCESS => "Create",
            AuthenticationEnum.READ_ACCESS => "Read",
            AuthenticationEnum.UPDATE_ACCESS => "Update",
            AuthenticationEnum.DELETE_ACCESS => "Delete",
            _ => ""
        };
        return fieldAccess;
    }

    public bool GetPermission(int leafId, AuthenticationEnum authenticationEnum)
    {
        var access = false;
        var fieldName = GetFieldCheck(authenticationEnum);
        if (leafId <= 0) return access;
        var sql = @"
                SELECT  leafAccess" + fieldName + "Value as access";
        sql += " FROM    leaf_access";
        sql += " WHERE   leafId  =   @leafId";
        sql += " AND     roleId  =   @roleId";
        sql += " AND     leafAccess" + fieldName + "Value = 1";
        sql += " LIMIT 1";
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            MySqlCommand mySqlCommand = new(sql, connection);
            mySqlCommand.Parameters.AddWithValue("@leafId", leafId);
            mySqlCommand.Parameters.AddWithValue("@roleId", _sharedUtil.GetRoleId());

            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (Convert.ToInt32(reader["access"].ToString()) != 1) continue;
                    access = true;
                    break;
                }
            }

            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetSystemException(ex);
        }

        return access;
    }
}