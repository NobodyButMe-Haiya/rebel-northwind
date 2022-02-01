using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using RebelCmsTemplate.Enum;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RouteController : Controller
{
    private readonly RenderViewToStringUtil _renderViewAsStringUtil;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RouteController(RenderViewToStringUtil renderViewToString1, IHttpContextAccessor httpContextAccessor1)
    {
        _renderViewAsStringUtil = renderViewToString1;
        _httpContextAccessor = httpContextAccessor1;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        const string? templatePath = "~/Views/Error/403.cshtml";
        var page = await _renderViewAsStringUtil.RenderViewToStringAsync(ControllerContext, templatePath,
            ("Foo", "Bar"));
        return Ok(page);
    }

    [HttpPost]
    public async Task<IActionResult> Post()
    {
        var status = false;
        var leafId = Convert.ToInt32(Request.Form["leafCheckId"]);
        SharedUtil sharedUtil = new(_httpContextAccessor);
        CheckAccessUtil checkAccessUtil = new(_httpContextAccessor);
        var page = SharedUtil.Return500();

        var access = checkAccessUtil.GetPermission(leafId, AuthenticationEnum.READ_ACCESS);
        if (access)
        {
            await using (var connection = SharedUtil.GetConnection())
            {
                try
                {
                    connection.Open();
                    const string sql = @"
                    SELECT  leafName,
                            folderName,
                            leafFilename
                    FROM    leaf
                    JOIN    folder
                    USING   (folderId)
                    WHERE   leaf.leafId =@leafId
                    LIMIT 1";
                    MySqlCommand mySqlCommand = new(sql, connection);
                    mySqlCommand.Parameters.AddWithValue("@leafId", leafId);
                    await using (var reader = mySqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var templatePath = "~/Views/Pages/" + reader["folderName"]?.ToString()?.Trim() + "/" +
                                               reader["leafFilename"].ToString()?.Trim() + ".cshtml";
                            page = await _renderViewAsStringUtil.RenderViewToStringAsync(ControllerContext,
                                templatePath);
                            status = true;
                        }
                    }

                    mySqlCommand.Dispose();
                }
                catch (MySqlException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    sharedUtil.SetSystemException(ex);
                }
            }

            return Ok(new {status, page});
        }

        var code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
        return Ok(new {status, code});
    }
}