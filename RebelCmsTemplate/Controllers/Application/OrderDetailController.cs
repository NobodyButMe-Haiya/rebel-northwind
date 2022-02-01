using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Enum;
using RebelCmsTemplate.Models.Application;
using RebelCmsTemplate.Repository.Application;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Controllers.Application;

[Route("api/application/[controller]")]
[ApiController]
public class OrderDetailController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RenderViewToStringUtil _renderViewToStringUtil;

    public OrderDetailController(RenderViewToStringUtil renderViewToStringUtil,
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

        OrderDetailRepository orderDetailRepository = new(_httpContextAccessor);
        var content = orderDetailRepository.GetExcel();
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "orderDetail39.xlsx");
    }

    [HttpPost]
    public ActionResult Post()
    {
        var status = false;
        var mode = Request.Form["mode"];
        var leafCheckKey = Convert.ToInt32(Request.Form["leafCheckKey"]);
        OrderDetailRepository orderDetailRepository = new(_httpContextAccessor);
        ProductRepository productRepository = new(_httpContextAccessor);
        SharedUtil sharedUtil = new(_httpContextAccessor);
        CheckAccessUtil checkAccessUtil = new(_httpContextAccessor);
        
        string? code;
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
                        if (!uint.TryParse(Request.Form["orderKey"], out var orderKey))
                        {
                            code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                            return Ok(new {status, code});
                        }

                        if (!uint.TryParse(Request.Form["productKey"], out var productKey))
                        {
                            code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                            return Ok(new {status, code});
                        }
                        if(productKey == 0 )
                        {
                            productKey = productRepository.GetDefault();
                        }

                        var orderDetailUnitPrice = !string.IsNullOrEmpty(Request.Form["orderDetailUnitPrice"])
                            ? Convert.ToDecimal(Request.Form["orderDetailUnitPrice"])
                            : 0;
                        var orderDetailQuantity = !string.IsNullOrEmpty(Request.Form["orderDetailQuantity"])
                            ? Convert.ToInt32(Request.Form["orderDetailQuantity"])
                            : 0;
                        var orderDetailDiscount = !string.IsNullOrEmpty(Request.Form["orderDetailDiscount"])
                            ? Convert.ToDouble(Request.Form["orderDetailDiscount"])
                            : 0;

                        OrderDetailModel orderDetailModel = new()
                        {
                            OrderKey = orderKey,
                            ProductKey = productKey,
                            OrderDetailUnitPrice = orderDetailUnitPrice,
                            OrderDetailQuantity = orderDetailQuantity,
                            OrderDetailDiscount = orderDetailDiscount
                        };
                        var lastInsertKey = orderDetailRepository.Create(orderDetailModel);
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
                        var data = orderDetailRepository.Read();
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

                            var data = orderDetailRepository.Search(search);
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
                    if (!string.IsNullOrEmpty(Request.Form["orderDetailKey"]))
                    {
                        try
                        {
                            if (!uint.TryParse(Request.Form["orderDetailKey"], out var orderDetailKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }

                            if (orderDetailKey > 0)
                            {
                                if (!uint.TryParse(Request.Form["orderKey"], out var orderKey))
                                {
                                    code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                    return Ok(new {status, code});
                                }

                                if (!uint.TryParse(Request.Form["productKey"], out var productKey))
                                {
                                    code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                    return Ok(new {status, code});
                                }
                                if (productKey == 0 )
                                {
                                    productKey = productRepository.GetDefault();
                                }

                                var orderDetailUnitPrice =
                                    !string.IsNullOrEmpty(Request.Form["orderDetailUnitPrice"])
                                        ? Convert.ToDecimal(Request.Form["orderDetailUnitPrice"])
                                        : 0;
                                var orderDetailQuantity = !string.IsNullOrEmpty(Request.Form["orderDetailQuantity"])
                                    ? Convert.ToInt32(Request.Form["orderDetailQuantity"])
                                    : 0;
                                var orderDetailDiscount = !string.IsNullOrEmpty(Request.Form["orderDetailDiscount"])
                                    ? Convert.ToDouble(Request.Form["orderDetailDiscount"])
                                    : 0;
                                OrderDetailModel orderDetailModel = new()
                                {
                                    OrderDetailKey = orderDetailKey,
                                    OrderKey = orderKey,
                                    ProductKey = productKey,
                                    OrderDetailUnitPrice = orderDetailUnitPrice,
                                    OrderDetailQuantity = orderDetailQuantity,
                                    OrderDetailDiscount = orderDetailDiscount
                                };
                                orderDetailRepository.Update(orderDetailModel);
                                code = ((int) ReturnCodeEnum.UPDATE_SUCCESS).ToString();
                                status = true;
                            }
                            else
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
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
                    if (!string.IsNullOrEmpty(Request.Form["orderDetailKey"]))
                    {
                        try
                        {
                            if (!uint.TryParse(Request.Form["orderDetailKey"], out var orderDetailKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }

                            if (orderDetailKey > 0)
                            {
                                OrderDetailModel orderDetailModel = new()
                                {
                                    OrderDetailKey = orderDetailKey
                                };
                                orderDetailRepository.Delete(orderDetailModel);
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