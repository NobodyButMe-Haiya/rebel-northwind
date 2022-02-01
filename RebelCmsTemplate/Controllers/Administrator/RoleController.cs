using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Enum;
using RebelCmsTemplate.Models.Administrator;
using RebelCmsTemplate.Repository.Administrator;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Controllers.Administrator;

[Route("api/administrator/[controller]")]
[ApiController]
public class RoleController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RenderViewToStringUtil _renderViewToStringUtil;

    public RoleController(RenderViewToStringUtil renderViewToStringUtil, IHttpContextAccessor httpContextAccessor)
    {
        _renderViewToStringUtil = renderViewToStringUtil;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        SharedUtil sharedUtils = new(_httpContextAccessor);
        if (sharedUtils.GetTenantId() == 0 || sharedUtils.GetTenantId().Equals(null))
        {
            const string? templatePath = "~/Views/Error/403.cshtml";
            var page = await _renderViewToStringUtil.RenderViewToStringAsync(ControllerContext, templatePath);
            return Ok(page);
        }

        RoleRepository roleRepository = new(_httpContextAccessor);
        var content = roleRepository.GetExcel();
        return File(
            content,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "roles.xlsx");
    }

    [HttpPost]
    public ActionResult Post()
    {
        var status = false;
        var mode = Request.Form["mode"];
        var leafCheckKey = Convert.ToInt32(Request.Form["leafCheckKey"]);

        RoleRepository roleRepository = new(_httpContextAccessor);
        SharedUtil sharedUtil = new(_httpContextAccessor);
        CheckAccessUtil checkAccessUtil = new(_httpContextAccessor);

        string code;

        switch (mode)
        {
            case "create":
                if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.CREATE_ACCESS))
                {
                    code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                }
                else
                {
                    try
                    {
                        var roleName = Request.Form["roleName"];
                        RoleModel roleModel = new()
                        {
                            RoleName = roleName
                        };
                        var lastInsertKey = roleRepository.Create(roleModel);
                        code = ((int) ReturnCodeEnum.CREATE_SUCCESS).ToString();
                        status = true;
                        return Ok(new {status, code, lastInsertKey});
                    }
                    catch (Exception ex)
                    {
                        code = sharedUtil.GetRoleId() == (int) AccessEnum.ADMINISTRATOR_ACCESS
                            ? ex.Message
                            : ((int) ReturnCodeEnum.SYSTEM_ERROR).ToString();
                    }
                }

                break;
            case "read":
                if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.READ_ACCESS))
                {
                    code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                }
                else
                {
                    try
                    {
                        var data = roleRepository.Read();
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
            case "search":
                if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.READ_ACCESS))
                {
                    code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.Form["search"]))
                    {
                        try
                        {
                            var search = Request.Form["search"];
                            var data = roleRepository.Search(search);
                            code = ((int) ReturnCodeEnum.READ_SUCCESS).ToString();
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
                    else
                    {
                        code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
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
                    if (!string.IsNullOrEmpty(Request.Form["roleKey"]))
                    {
                        try
                        {
                            if (!uint.TryParse(Request.Form["roleKey"], out var roleKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }

                            var roleName = Request.Form["roleName"];

                            RoleModel roleModel = new()
                            {
                                RoleName = roleName,
                                RoleKey = roleKey
                            };
                            roleRepository.Update(roleModel);
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
                    else
                    {
                        code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                    }
                }

                break;
            case "delete":
                if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.DELETE_ACCESS))
                {
                    code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.Form["roleKey"]))
                    {
                        try
                        {
                            if (!uint.TryParse(Request.Form["roleKey"], out var roleKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }

                            RoleModel roleModel = new()
                            {
                                RoleKey = roleKey
                            };
                            roleRepository.Delete(roleModel);

                            code = ((int) ReturnCodeEnum.DELETE_SUCCESS).ToString();
                            status = true;
                        }
                        catch (Exception ex)
                        {
                            code = sharedUtil.GetRoleId() == (int) AccessEnum.ADMINISTRATOR_ACCESS
                                ? ex.Message
                                : ((int) ReturnCodeEnum.SYSTEM_ERROR).ToString();
                        }
                    }
                    else
                    {
                        code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
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