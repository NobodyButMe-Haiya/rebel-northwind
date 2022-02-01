using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Enum;
using RebelCmsTemplate.Models.Setting;
using RebelCmsTemplate.Repository.Setting;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Controllers.Setting;

[Route("api/setting/[controller]")]
[ApiController]
public class ProductTypeController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RenderViewToStringUtil _renderViewToStringUtil;

    public ProductTypeController(RenderViewToStringUtil renderViewToStringUtil,
        IHttpContextAccessor httpContextAccessor)
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

        ProductTypeRepository productTypeRepository = new(_httpContextAccessor);
        var content = productTypeRepository.GetExcel();
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

        ProductTypeRepository productTypeRepository = new(_httpContextAccessor);
        ProductCategoryRepository productCategoryRepository = new(_httpContextAccessor);
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
                        uint productCategoryKey;
                        if (!string.IsNullOrWhiteSpace(Request.Form["productCategoryKey"]))
                        {
                            if (!uint.TryParse(Request.Form["productCategoryKey"], out productCategoryKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }
                        }
                        else
                        {
                            productCategoryKey = productCategoryRepository.GetDefault();
                        }

                        var productTypeName = Request.Form["productTypeName"];

                        var lastInsertKey = productTypeRepository.Create(new ProductTypeModel
                        {
                            ProductTypeName = productTypeName,
                            ProductCategoryKey = productCategoryKey
                        });
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
                        var data = productTypeRepository.Read();
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
                            var data = productTypeRepository.Search(search);
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
                    if (!string.IsNullOrEmpty(Request.Form["productTypeKey"]))
                    {
                        try
                        {
                            if (!uint.TryParse(Request.Form["productTypeKey"], out var productTypeKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }

                            if (productTypeKey > 0)
                            {
                                uint productCategoryKey;
                                if (!string.IsNullOrWhiteSpace(Request.Form["productCategoryKey"]))
                                {
                                    if (!uint.TryParse(Request.Form["productCategoryKey"], out productCategoryKey))
                                    {
                                        code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                        return Ok(new {status, code});
                                    }
                                }
                                else
                                {
                                    productCategoryKey = productCategoryRepository.GetDefault();
                                }

                                var productTypeName = Request.Form["productTypeName"];

                                productTypeRepository.Update(new ProductTypeModel
                                {
                                    ProductTypeName = productTypeName,
                                    ProductTypeKey = productTypeKey,
                                    ProductCategoryKey = productCategoryKey
                                });
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
                    if (!string.IsNullOrEmpty(Request.Form["productTypeKey"]))
                    {
                        try
                        {
                            if (!uint.TryParse(Request.Form["productTypeKey"], out var productTypeKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }

                            if (productTypeKey > 0)
                            {
                                productTypeRepository.Delete(new ProductTypeModel
                                {
                                    ProductTypeKey = productTypeKey
                                });

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