using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Enum;
using RebelCmsTemplate.Models.Application;
using RebelCmsTemplate.Repository.Application;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Controllers.Application;

[Route("api/application/[controller]")]
[ApiController]
public class ShipperController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RenderViewToStringUtil _renderViewToStringUtil;

    public ShipperController(RenderViewToStringUtil renderViewToStringUtil, IHttpContextAccessor httpContextAccessor)
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

        ShipperRepository shipperRepository = new(_httpContextAccessor);
        var content = shipperRepository.GetExcel();
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "shipper41.xlsx");
    }

    [HttpPost]
    public ActionResult Post()
    {
        var status = false;
        var mode = Request.Form["mode"];
        var leafCheckKey = Convert.ToInt32(Request.Form["leafCheckKey"]);
        ShipperRepository shipperRepository = new(_httpContextAccessor);
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
                        var shipperName = Request.Form["shipperName"];
                        var shipperPhone = Request.Form["shipperPhone"];
                        ShipperModel shipperModel = new()
                        {
                            ShipperName = shipperName,
                            ShipperPhone = shipperPhone
                        };
                        var lastInsertKey = shipperRepository.Create(shipperModel);
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
                        var data = shipperRepository.Read();
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
                            var data = shipperRepository.Search(search);
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
            case "single":
                if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.READ_ACCESS))
                {
                    code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.Form["shipperKey"]))
                    {
                        try
                        {
                            if (!uint.TryParse(Request.Form["shipperKey"], out var shipperKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }

                            if (shipperKey == 0)
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                            }
                            else
                            {
                                ShipperModel shipperModel = new()
                                {
                                    ShipperKey = shipperKey
                                };
                                var dataSingle = shipperRepository.GetSingle(shipperModel);
                                code = ((int) ReturnCodeEnum.READ_SUCCESS).ToString();
                                status = true;
                                return Ok(new {status, code, dataSingle});
                            }
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
                    if (!string.IsNullOrEmpty(Request.Form["shipperKey"]))
                    {
                        try
                        {
                            if (!uint.TryParse(Request.Form["shipperKey"], out var shipperKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }

                            if (shipperKey > 0)
                            {
                                var shipperName = Request.Form["shipperName"];
                                var shipperPhone = Request.Form["shipperPhone"];
                                ShipperModel shipperModel = new()
                                {
                                    ShipperKey = shipperKey,
                                    ShipperName = shipperName,
                                    ShipperPhone = shipperPhone
                                };
                                shipperRepository.Update(shipperModel);
                                code = ((int) ReturnCodeEnum.UPDATE_SUCCESS).ToString();
                                status = true;
                            }
                            else
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                            }
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
                    if (!string.IsNullOrEmpty(Request.Form["shipperKey"]))
                    {
                        try
                        {
                            if (!uint.TryParse(Request.Form["shipperKey"], out var shipperKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }

                            if (shipperKey > 0)
                            {
                                ShipperModel shipperModel = new()
                                {
                                    ShipperKey = shipperKey
                                };
                                shipperRepository.Delete(shipperModel);
                                code = ((int) ReturnCodeEnum.DELETE_SUCCESS).ToString();
                                status = true;
                            }
                            else
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                            }
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