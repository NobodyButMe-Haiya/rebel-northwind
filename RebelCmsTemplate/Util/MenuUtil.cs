using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Menu;

namespace RebelCmsTemplate.Util;

public class MenuUtil
{
    private readonly SharedUtil _sharedUtil;
    private readonly List<FolderAccessModel> _folderAccessModels;

    public MenuUtil(IHttpContextAccessor httpContextAccessor)
    {
        _sharedUtil = new SharedUtil(httpContextAccessor);
        _folderAccessModels = new List<FolderAccessModel>();
    }

    /// <summary>
    /// It's better to see output live in menu 
    /// </summary>
    /// <returns></returns>
    public List<MenuModel> GetMenu()
    {
        List<MenuModel> menuModels = new();

        const string sqlFolder =
            "SELECT * FROM folder_access JOIN folder USING (folderId) WHERE folder.isDelete != 1 AND roleId = @roleId ";
        const string sqlLeaf =
            "SELECT * FROM leaf_access JOIN leaf USING (leafId) WHERE leaf.isDelete != 1 AND roleId = @roleId AND folderId=@folderId";
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            MySqlCommand mySqlCommand = new(sqlFolder, connection);
            mySqlCommand.Parameters.AddWithValue("@roleId", _sharedUtil.GetRoleId());

            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    FolderAccessModel folderAccessModel = new()
                    {
                        FolderKey = Convert.ToUInt32(reader["folderId"]),
                        FolderName = reader["folderName"].ToString()
                    };
                    _folderAccessModels.Add(folderAccessModel);
                }
            }

            mySqlCommand.Dispose();


            foreach (var folderAccessModel1 in _folderAccessModels)
            {
                MenuModel menuModel1 = new()
                {
                    FolderName = folderAccessModel1.FolderName
                };

                mySqlCommand = new MySqlCommand(sqlLeaf, connection);
                mySqlCommand.Parameters.AddWithValue("@roleId", _sharedUtil.GetRoleId());
                mySqlCommand.Parameters.AddWithValue("@folderId", folderAccessModel1.FolderKey);
                List<MenuDetailModel> menuDetailModels = new();
                using (var reader = mySqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MenuDetailModel menuDetailModel = new()
                        {
                            LeafKey = Convert.ToInt32(reader["leafId"]),
                            LeafName = reader["leafName"].ToString()
                        };
                        menuDetailModels.Add(menuDetailModel);
                    }
                }

                mySqlCommand.Dispose();
                menuModel1.Details = menuDetailModels;
                menuModels.Add(menuModel1);
            }
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetSystemException(ex);
        }

        return menuModels;
    }
}