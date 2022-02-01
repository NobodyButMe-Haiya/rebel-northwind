using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Enum;
using RebelCmsTemplate.Models.Administrator;
using RebelCmsTemplate.Repository.Administrator;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Controllers.Administrator;

[Route("api/administrator/[controller]")]
[ApiController]
public class UserController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RenderViewToStringUtil _renderViewToStringUtil;

    public UserController(RenderViewToStringUtil renderViewToStringUtil, IHttpContextAccessor httpContextAccessor)
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

        UserRepository userRepository = new(_httpContextAccessor);
        var content = userRepository.GetExcel();
        return File(
            content,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "roles.xlsx");
    }

    [HttpPost]
    public ActionResult Post()
    {
        var status = false;
        string? code;
        var mode = Request.Form["mode"];
        var leafCheckKey = Convert.ToInt32(Request.Form["leafCheckKey"]);
        
        UserRepository userRepository = new(_httpContextAccessor);
        RoleRepository roleRepository = new(_httpContextAccessor);
        SharedUtil sharedUtil = new(_httpContextAccessor);
        CheckAccessUtil checkAccessUtil = new(_httpContextAccessor);

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
          
                        uint roleKey;
                        if (!string.IsNullOrWhiteSpace(Request.Form["roleKey"]))
                        {
                            if (!uint.TryParse(Request.Form["roleKey"], out roleKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }
                        }
                        else
                        {
                            roleKey = roleRepository.GetDefault();
                        }
                        var userName = Request.Form["userName"].ToString();
                        var userPassword = Request.Form["userPassword"].ToString();
                        var userEmail = Request.Form["userEmail"].ToString();

                        var lastInsertKey = userRepository.Create(new UserModel
                        {
                            RoleKey = roleKey,
                            UserName = userName,
                            UserPassword = userPassword,
                            UserEmail = userEmail
                        });
                        code = ((int) ReturnCodeEnum.CREATE_SUCCESS).ToString();
                        status = true;
                        return Ok(new {code, status, lastInsertKey});
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
                        var data = userRepository.Read();
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
                            var data = userRepository.Search(search);
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
                    if (!string.IsNullOrEmpty(Request.Form["userKey"]))
                    {
                        try
                        {
                            if (!uint.TryParse(Request.Form["userKey"], out var userKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }
                            uint roleKey;
                            if (!string.IsNullOrWhiteSpace(Request.Form["roleKey"]))
                            {
                                if (!uint.TryParse(Request.Form["roleKey"], out roleKey))
                                {
                                    code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                    return Ok(new {status, code});
                                }
                            }
                            else
                            {
                                roleKey = roleRepository.GetDefault();
                            }
                            var userName = Request.Form["userName"].ToString();
                            var userPassword = Request.Form["userPassword"].ToString();
                            var userEmail = Request.Form["userEmail"].ToString();

                            userRepository.Update(new UserModel
                            {
                                RoleKey = roleKey,
                                UserName = userName,
                                UserPassword = userPassword,
                                UserEmail = userEmail,
                                UserKey = userKey
                            });
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
                            if (!uint.TryParse(Request.Form["userKey"], out var userKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }

                            userRepository.Delete(new UserModel
                            {
                                UserKey = userKey
                            });

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
                code = SharedUtil.Return500();
                break;
        }

        return Ok(new {status, code});
    }
}