using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Enum;
using RebelCmsTemplate.Models.Menu;
using RebelCmsTemplate.Repository.Menu;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Controllers.Menu;

[Route("api/menu/[controller]")]
[ApiController]
public class LeafAccessController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RenderViewToStringUtil _renderViewToStringUtil;

    public LeafAccessController(RenderViewToStringUtil renderViewToStringUtil, IHttpContextAccessor httpContextAccessor)
    {
        _renderViewToStringUtil = renderViewToStringUtil;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        const string? templatePath = "~/Views/Error/403.cshtml";
        var page = await _renderViewToStringUtil.RenderViewToStringAsync(ControllerContext, templatePath);
        return Ok(page);
    }

    [HttpPost]
    public ActionResult Post()
    {
        var status = false;
        var mode = Request.Form["mode"];
        var leafCheckKey = Convert.ToInt32(Request.Form["leafCheckKey"]);

        var folderKey = Request.Form["folderKey"].ToString() != null ? Convert.ToInt32(Request.Form["folderKey"]) : 0;
        var leafKey = Request.Form["leafKey"].ToString() != null ? Convert.ToInt32(Request.Form["leafKey"]) : 0;
        var roleKey = Request.Form["roleKey"].ToString() != null ? Convert.ToInt32(Request.Form["roleKey"]) : 0;


        string[] leafAccessIdValue = Request.Form["keyArray[]"].ToArray();

        string[] leafAccessCreateValue = Request.Form["createValue[]"].ToArray();
        string[] leafAccessReadValue = Request.Form["readValue[]"].ToArray();
        string[] leafAccessUpdateValue = Request.Form["updateValue[]"].ToArray();
        string[] leafAccessDeleteValue = Request.Form["deleteValue[]"].ToArray();

        var total = leafAccessIdValue.Length;
        List<LeafAccessModel> leafAccessModels = new();
        if (total > 0)
        {
            for (var i = 0; i < total; i++)
            {
                leafAccessModels.Add(new LeafAccessModel
                {
                    LeafAccessKey = Convert.ToUInt32(leafAccessIdValue[i]),
                    LeafAccessCreateValue = Convert.ToInt32(leafAccessCreateValue[i]),
                    LeafAccessReadValue = Convert.ToInt32(leafAccessReadValue[i]),
                    LeafAccessUpdateValue = Convert.ToInt32(leafAccessUpdateValue[i]),
                    LeafAccessDeleteValue = Convert.ToInt32(leafAccessDeleteValue[i])
                });
            }
        }

        LeafAccessRepository leafAccessRepository = new(_httpContextAccessor);
        SharedUtil sharedUtil = new(_httpContextAccessor);
        CheckAccessUtil checkAccessUtil = new(_httpContextAccessor);
        
        string code;
        switch (mode)
        {
            case "read":
            case "search":

                if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.READ_ACCESS))
                {
                    code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                }
                else
                {
                    try
                    {
                        var data = leafAccessRepository.Read(roleKey, folderKey, leafKey);
                        code = ((int) ReturnCodeEnum.CREATE_SUCCESS).ToString();
                        status = true;
                        return Ok(new {code, status, data});
                    }
                    catch (Exception ex)
                    {
                        code = sharedUtil.GetRoleId() == (int) AccessEnum.ADMINISTRATOR_ACCESS
                            ? ex.Message
                            : ((int) ReturnCodeEnum.SYSTEM_ERROR).ToString();
                    }
                }

                break;

            case "update":
                if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.UPDATE_ACCESS))
                {
                    code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                }
                else
                {
                    try
                    {
                        leafAccessRepository.Update(leafAccessModels);
                        code = ((int) ReturnCodeEnum.UPDATE_SUCCESS).ToString();
                        status = true;
                    }
                    catch (Exception ex)
                    {
                        code = sharedUtil.GetRoleId() == (int) AccessEnum.ADMINISTRATOR_ACCESS
                            ? ex.Message
                            : ((int) ReturnCodeEnum.SYSTEM_ERROR).ToString();
                    }
                }

                break;
            case "filterFolder":
                try
                {
                    FolderRepository folderRepository = new(_httpContextAccessor);
                    var folderModels = folderRepository.Read().Select(x => new FolderOptionModel
                        {FolderKey = x.FolderKey, FolderName = x.FolderName}).ToList();
                    code = ((int) ReturnCodeEnum.CREATE_SUCCESS).ToString();
                    status = true;
                    return Ok(new {code, status, folderModels});
                }
                catch (Exception ex)
                {
                    code = sharedUtil.GetRoleId() == (int) AccessEnum.ADMINISTRATOR_ACCESS
                        ? ex.Message
                        : ((int) ReturnCodeEnum.SYSTEM_ERROR).ToString();
                }

                break;
            case "filterLeaf":
                try
                {
                    LeafRepository leafRepository = new(_httpContextAccessor);
                    var leafModels = leafRepository.Read().Where(x => x.FolderKey == folderKey)
                        .Select(x => new LeafOptionModel {LeafKey = x.LeafKey, LeafName = x.LeafName}).ToList();
                    code = ((int) ReturnCodeEnum.CREATE_SUCCESS).ToString();
                    status = true;
                    return Ok(new {code, status, leafModels});
                }
                catch (Exception ex)
                {
                    code = sharedUtil.GetRoleId() == (int) AccessEnum.ADMINISTRATOR_ACCESS
                        ? ex.Message
                        : ((int) ReturnCodeEnum.SYSTEM_ERROR).ToString();
                }

                break;

            default:
                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                break;
        }

        return Ok(new {status, code});
    }
}