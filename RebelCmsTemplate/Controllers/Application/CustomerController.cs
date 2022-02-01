using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Enum;
using RebelCmsTemplate.Models.Application;
using RebelCmsTemplate.Repository.Application;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Controllers.Application;

[Route("api/application/[controller]")]
[ApiController]
public class CustomerController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RenderViewToStringUtil _renderViewToStringUtil;

    public CustomerController(RenderViewToStringUtil renderViewToStringUtil, IHttpContextAccessor httpContextAccessor)
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

        CustomerRepository customerRepository = new(_httpContextAccessor);
        var content = customerRepository.GetExcel();
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "customer89.xlsx");
    }

    [HttpPost]
    public ActionResult Post()
    {
        var status = false;
        var mode = Request.Form["mode"];
        var leafCheckKey = Convert.ToInt32(Request.Form["leafCheckKey"]);
        CustomerRepository customerRepository = new(_httpContextAccessor);
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
                        var customerCode = Request.Form["customerCode"];
                        var customerName = Request.Form["customerName"];
                        var customerContactName = Request.Form["customerContactName"];
                        var customerContactTitle = Request.Form["customerContactTitle"];
                        var customerAddress = Request.Form["customerAddress"];
                        var customerCity = Request.Form["customerCity"];
                        var customerRegion = Request.Form["customerRegion"];
                        var customerPostalCode = Request.Form["customerPostalCode"];
                        var customerCountry = Request.Form["customerCountry"];
                        var customerPhone = Request.Form["customerPhone"];
                        var customerFax = Request.Form["customerFax"];
                        CustomerModel customerModel = new()
                        {
                            CustomerCode = customerCode,
                            CustomerName = customerName,
                            CustomerContactName = customerContactName,
                            CustomerContactTitle = customerContactTitle,
                            CustomerAddress = customerAddress,
                            CustomerCity = customerCity,
                            CustomerRegion = customerRegion,
                            CustomerPostalCode = customerPostalCode,
                            CustomerCountry = customerCountry,
                            CustomerPhone = customerPhone,
                            CustomerFax = customerFax
                        };
                        var lastInsertKey = customerRepository.Create(customerModel);
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
                        var data = customerRepository.Read();
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
                            var data = customerRepository.Search(search);
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
                    if (!string.IsNullOrWhiteSpace(Request.Form["customerKey"]))
                    {
                        try
                        {
                            if (!uint.TryParse(Request.Form["customerKey"], out var customerKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }

                            if (customerKey == 0)
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                            }
                            else
                            {
                                CustomerModel customerModel = new()
                                {
                                    CustomerKey = customerKey
                                };
                                var dataSingle = customerRepository.GetSingle(customerModel);
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
                    if (!string.IsNullOrWhiteSpace(Request.Form["customerKey"]))
                    {
                        try
                        {
                            if (!uint.TryParse(Request.Form["customerKey"], out var customerKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }

                            if (customerKey > 0)
                            {
                                var customerCode = Request.Form["customerCode"];
                                var customerName = Request.Form["customerName"];
                                var customerContactName = Request.Form["customerContactName"];
                                var customerContactTitle = Request.Form["customerContactTitle"];
                                var customerAddress = Request.Form["customerAddress"];
                                var customerCity = Request.Form["customerCity"];
                                var customerRegion = Request.Form["customerRegion"];
                                var customerPostalCode = Request.Form["customerPostalCode"];
                                var customerCountry = Request.Form["customerCountry"];
                                var customerPhone = Request.Form["customerPhone"];
                                var customerFax = Request.Form["customerFax"];
                                CustomerModel customerModel = new()
                                {
                                    CustomerKey = customerKey,
                                    CustomerCode = customerCode,
                                    CustomerName = customerName,
                                    CustomerContactName = customerContactName,
                                    CustomerContactTitle = customerContactTitle,
                                    CustomerAddress = customerAddress,
                                    CustomerCity = customerCity,
                                    CustomerRegion = customerRegion,
                                    CustomerPostalCode = customerPostalCode,
                                    CustomerCountry = customerCountry,
                                    CustomerPhone = customerPhone,
                                    CustomerFax = customerFax
                                };
                                customerRepository.Update(customerModel);
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
                    if (!string.IsNullOrWhiteSpace(Request.Form["customerKey"]))
                    {
                        try
                        {
                            if (!uint.TryParse(Request.Form["customerKey"], out var customerKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }

                            if (customerKey > 0)
                            {
                                CustomerModel customerModel = new()
                                {
                                    CustomerKey = customerKey
                                };
                                customerRepository.Delete(customerModel);
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