using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Enum;
using RebelCmsTemplate.Models.Menu;
using RebelCmsTemplate.Repository.Menu;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Controllers.Menu;

[Route("api/menu/[controller]")]
[ApiController]
public class FolderAccessController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RenderViewToStringUtil _renderViewToStringUtil;

    public FolderAccessController(RenderViewToStringUtil renderViewToStringUtil,
        IHttpContextAccessor httpContextAccessor)
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
        SharedUtil sharedUtil = new(_httpContextAccessor);
        CheckAccessUtil checkAccessUtil = new(_httpContextAccessor);

        var status = false;
        var mode = Request.Form["mode"];
        var leafCheckKey = Convert.ToInt32(Request.Form["leafCheckKey"]);

        var folderKey = Request.Form["folderKey"].ToString() != null ? Convert.ToInt32(Request.Form["folderKey"]) : 0;
        var roleKey = Request.Form["roleKey"].ToString() != null ? Convert.ToInt32(Request.Form["roleKey"]) : 0;

        string[] folderAccessKeyValue = Request.Form["keyArray[]"].ToArray();
        string[] folderAccessValue = Request.Form["valueArray[]"].ToArray();

        var total = folderAccessKeyValue.Length;
        List<FolderAccessModel> folderAccessModels = new();
        if (total > 0)
        {
            for (var i = 0; i < total; i++)
            {
                folderAccessModels.Add(new FolderAccessModel
                {
                    FolderAccessKey = Convert.ToUInt32(folderAccessKeyValue[i]),
                    FolderAccessValue = Convert.ToInt32(folderAccessValue[i])
                });
            }
        }

        FolderAccessRepository folderAccessRepository = new(_httpContextAccessor);

        string code ;
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
                       var data = folderAccessRepository.Read(roleKey, folderKey);
                        code = ((int) ReturnCodeEnum.CREATE_SUCCESS).ToString();
                        status = true;
                        return Ok(new {status, code, data});
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
                        folderAccessRepository.Update(folderAccessModels);
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

            default:
                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                break;
        }

        return Ok(new {status, code});
    }
}