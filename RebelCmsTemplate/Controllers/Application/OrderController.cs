using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Enum;
using RebelCmsTemplate.Models.Application;
using RebelCmsTemplate.Repository.Application;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Controllers.Application;

[Route("api/application/[controller]")]
[ApiController]
public class OrderController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RenderViewToStringUtil _renderViewToStringUtil;

    public OrderController(RenderViewToStringUtil renderViewToStringUtil, IHttpContextAccessor httpContextAccessor)
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

        OrderRepository orderRepository = new(_httpContextAccessor);
        var content = orderRepository.GetExcel();
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "order6.xlsx");
    }

    [HttpPost]
    public ActionResult Post()
    {
      
        var status = false;
        var mode = Request.Form["mode"];
        var leafCheckKey = Convert.ToInt32(Request.Form["leafCheckKey"]);

        OrderRepository orderRepository = new(_httpContextAccessor);
        CustomerRepository customerRepository = new(_httpContextAccessor);
        ShipperRepository shipperRepository = new(_httpContextAccessor);
        EmployeeRepository employeeRepository = new(_httpContextAccessor);

        SharedUtil sharedUtil = new(_httpContextAccessor);
        CheckAccessUtil checkAccessUtil = new(_httpContextAccessor);
        var code = string.Empty;
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
                        uint customerKey;
                        if (!string.IsNullOrWhiteSpace(Request.Form["customerKey"]))
                        {
                            if (!uint.TryParse(Request.Form["customerKey"], out customerKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }
                        }
                        else
                        {
                            customerKey = customerRepository.GetDefault();
                        }

                        uint shipperKey;
                        if (!string.IsNullOrWhiteSpace(Request.Form["shipperKey"]))
                        {
                            if (!uint.TryParse(Request.Form["shipperKey"], out shipperKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }
                        }
                        else
                        {
                            shipperKey = shipperRepository.GetDefault();
                        }

                        uint employeeKey ;
                        if (!string.IsNullOrWhiteSpace(Request.Form["employeeKey"]))
                        {
                            if (!uint.TryParse(Request.Form["employeeKey"], out employeeKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }
                        }
                        else
                        {
                            employeeKey = employeeRepository.GetDefault();
                        }

                        var orderOrderDate = DateOnly.FromDateTime(DateTime.Now);
                        if (!string.IsNullOrEmpty(Request.Form["orderOrderDate"]))
                        {
                            var dateString = Request.Form["orderOrderDate"].ToString().Split("-");
                            orderOrderDate = new DateOnly(Convert.ToInt32(dateString[0]),
                                Convert.ToInt32(dateString[1]),
                                Convert.ToInt32(dateString[2]));
                        }

                        var orderRequiredDate = DateOnly.FromDateTime(DateTime.Now);
                        if (!string.IsNullOrEmpty(Request.Form["orderRequiredDate"]))
                        {
                            var dateString = Request.Form["orderRequiredDate"].ToString().Split("-");
                            orderRequiredDate = new DateOnly(Convert.ToInt32(dateString[0]),
                                Convert.ToInt32(dateString[1]),
                                Convert.ToInt32(dateString[2]));
                        }

                        var orderShippedDate = DateOnly.FromDateTime(DateTime.Now);
                        if (!string.IsNullOrEmpty(Request.Form["orderShippedDate"]))
                        {
                            var dateString = Request.Form["orderShippedDate"].ToString().Split("-");
                            orderShippedDate = new DateOnly(Convert.ToInt32(dateString[0]),
                                Convert.ToInt32(dateString[1]),
                                Convert.ToInt32(dateString[2]));
                        }

                        var orderFreight = !string.IsNullOrEmpty(Request.Form["orderFreight"])
                            ? Convert.ToDecimal(Request.Form["orderFreight"])
                            : 0;
                        var orderShipName = Request.Form["orderShipName"];
                        var orderShipAddress = Request.Form["orderShipAddress"];
                        var orderShipCity = Request.Form["orderShipCity"];
                        var orderShipRegion = Request.Form["orderShipRegion"];
                        var orderShipPostalCode = Request.Form["orderShipPostalCode"];
                        var orderShipCountry = Request.Form["orderShipCountry"];

                        OrderModel orderModel = new()
                        {
                            CustomerKey = customerKey,
                            ShipperKey = shipperKey,
                            EmployeeKey = employeeKey,
                            OrderDate = orderOrderDate,
                            OrderRequiredDate = orderRequiredDate,
                            OrderShippedDate = orderShippedDate,
                            OrderFreight = orderFreight,
                            OrderShipName = orderShipName,
                            OrderShipAddress = orderShipAddress,
                            OrderShipCity = orderShipCity,
                            OrderShipRegion = orderShipRegion,
                            OrderShipPostalCode = orderShipPostalCode,
                            OrderShipCountry = orderShipCountry
                        };
                        var lastInsertKey = orderRepository.Create(orderModel);
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
                        var data = orderRepository.Read();
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
                            var data = orderRepository.Search(search);
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
            case "singleWithDetail":
                if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.READ_ACCESS))
                {
                    code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.Form["orderKey"]))
                    {
                        try
                        {
                            uint orderKey = 0;
                            if (!string.IsNullOrWhiteSpace(Request.Form["orderKey"]))
                            {
                                if (!uint.TryParse(Request.Form["orderKey"], out orderKey))
                                {
                                    code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                    return Ok(new {status, code});
                                }
                            }

                            if (orderKey == 0)
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                            }
                            else 
                            {
                                OrderModel orderModel = new()
                                {
                                    OrderKey = orderKey
                                };
                                var dataSingle = orderRepository.GetSingleWithDetail(orderModel);
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
                    if (!string.IsNullOrEmpty(Request.Form["orderKey"]))
                    {
                        try
                        {
                            uint orderKey = 0;
                            if (!string.IsNullOrWhiteSpace(Request.Form["orderKey"]))
                            {
                                if (!uint.TryParse(Request.Form["orderKey"], out orderKey))
                                {
                                    code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                    return Ok(new {status, code});
                                }
                            }

                            if (orderKey > 0)
                            {
                                uint customerKey;
                                if (!string.IsNullOrWhiteSpace(Request.Form["customerKey"]))
                                {
                                    if (!uint.TryParse(Request.Form["customerKey"], out customerKey))
                                    {
                                        code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                        return Ok(new {status, code});
                                    }
                                }
                                else
                                {
                                    customerKey = customerRepository.GetDefault();
                                }

                                uint shipperKey;
                                if (!string.IsNullOrWhiteSpace(Request.Form["shipperKey"]))
                                {
                                    if (!uint.TryParse(Request.Form["shipperKey"], out shipperKey))
                                    {
                                        code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                        return Ok(new {status, code});
                                    }
                                }
                                else
                                {
                                    shipperKey = shipperRepository.GetDefault();
                                }

                                uint employeeKey ;
                                if (!string.IsNullOrWhiteSpace(Request.Form["employeeKey"]))
                                {
                                    if (!uint.TryParse(Request.Form["employeeKey"], out employeeKey))
                                    {
                                        code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                        return Ok(new {status, code});
                                    }
                                }
                                else
                                {
                                    employeeKey = employeeRepository.GetDefault();
                                }

                                var orderOrderDate = DateOnly.FromDateTime(DateTime.Now);
                                if (!string.IsNullOrEmpty(Request.Form["orderOrderDate"]))
                                {
                                    var dateString = Request.Form["orderOrderDate"].ToString().Split("-");
                                    orderOrderDate = new DateOnly(Convert.ToInt32(dateString[0]),
                                        Convert.ToInt32(dateString[1]),
                                        Convert.ToInt32(dateString[2]));
                                }

                                var orderRequiredDate = DateOnly.FromDateTime(DateTime.Now);
                                if (!string.IsNullOrEmpty(Request.Form["orderRequiredDate"]))
                                {
                                    var dateString = Request.Form["orderRequiredDate"].ToString().Split("-");
                                    orderRequiredDate = new DateOnly(Convert.ToInt32(dateString[0]),
                                        Convert.ToInt32(dateString[1]),
                                        Convert.ToInt32(dateString[2]));
                                }

                                var orderShippedDate = DateOnly.FromDateTime(DateTime.Now);
                                if (!string.IsNullOrEmpty(Request.Form["orderShippedDate"]))
                                {
                                    var dateString = Request.Form["orderShippedDate"].ToString().Split("-");
                                    orderShippedDate = new DateOnly(Convert.ToInt32(dateString[0]),
                                        Convert.ToInt32(dateString[1]),
                                        Convert.ToInt32(dateString[2]));
                                }

                                var orderFreight = !string.IsNullOrEmpty(Request.Form["orderFreight"])
                                    ? Convert.ToDecimal(Request.Form["orderFreight"])
                                    : 0;
                                var orderShipName = Request.Form["orderShipName"];
                                var orderShipAddress = Request.Form["orderShipAddress"];
                                var orderShipCity = Request.Form["orderShipCity"];
                                var orderShipRegion = Request.Form["orderShipRegion"];
                                var orderShipPostalCode = Request.Form["orderShipPostalCode"];
                                var orderShipCountry = Request.Form["orderShipCountry"];

                                OrderModel orderModel = new()
                                {
                                    OrderKey = orderKey,
                                    CustomerKey = customerKey,
                                    ShipperKey = shipperKey,
                                    EmployeeKey = employeeKey,
                                    OrderDate = orderOrderDate,
                                    OrderRequiredDate = orderRequiredDate,
                                    OrderShippedDate = orderShippedDate,
                                    OrderFreight = orderFreight,
                                    OrderShipName = orderShipName,
                                    OrderShipAddress = orderShipAddress,
                                    OrderShipCity = orderShipCity,
                                    OrderShipRegion = orderShipRegion,
                                    OrderShipPostalCode = orderShipPostalCode,
                                    OrderShipCountry = orderShipCountry
                                };
                                orderRepository.Update(orderModel);
                                code = ((int) ReturnCodeEnum.UPDATE_SUCCESS).ToString();
                                status = true;
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
                    if (!string.IsNullOrEmpty(Request.Form["orderKey"]))
                    {
                        try
                        {
                            uint orderKey = 0;
                            if (!string.IsNullOrWhiteSpace(Request.Form["orderKey"]))
                            {
                                if (!uint.TryParse(Request.Form["orderKey"], out orderKey))
                                {
                                    code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                    return Ok(new {status, code});
                                }
                            }

                            if (orderKey > 0)
                            {
                                OrderModel orderModel = new()
                                {
                                    OrderKey = orderKey
                                };
                                orderRepository.Delete(orderModel);
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