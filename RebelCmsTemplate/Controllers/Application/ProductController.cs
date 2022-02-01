using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Enum;
using RebelCmsTemplate.Models.Application;
using RebelCmsTemplate.Repository.Application;
using RebelCmsTemplate.Repository.Setting;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Controllers.Application;

[Route("api/application/[controller]")]
[ApiController]
public class ProductController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RenderViewToStringUtil _renderViewToStringUtil;

    public ProductController(RenderViewToStringUtil renderViewToStringUtil, IHttpContextAccessor httpContextAccessor)
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

        ProductRepository productRepository = new(_httpContextAccessor);
        var content = productRepository.GetExcel();
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "product26.xlsx");
    }

    [HttpPost]
    public ActionResult Post()
    {
        var status = false;
        var mode = Request.Form["mode"];
        var leafCheckKey = Convert.ToInt32(Request.Form["leafCheckKey"]);

        ProductRepository productRepository = new(_httpContextAccessor);
        SupplierRepository supplierRepository = new(_httpContextAccessor);
        ProductCategoryRepository productCategoryRepository = new(_httpContextAccessor);
        ProductTypeRepository productTypeRepository = new(_httpContextAccessor);

        SharedUtil sharedUtil = new(_httpContextAccessor);
        CheckAccessUtil checkAccessUtil = new(_httpContextAccessor);

        string? code = null;

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
                        uint supplierKey;
                        if (!string.IsNullOrWhiteSpace(Request.Form["supplierKey"]))
                        {
                            if (!uint.TryParse(Request.Form["supplierKey"], out supplierKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }
                        }
                        else
                        {
                            supplierKey = supplierRepository.GetDefault();
                        }

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

                        uint productTypeKey;
                        if (!string.IsNullOrWhiteSpace(Request.Form["productTypeKey"]))
                        {
                            if (!uint.TryParse(Request.Form["productTypeKey"], out productTypeKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }
                        }
                        else
                        {
                            productTypeKey = productTypeRepository.GetDefault();
                        }

                        var productName = Request.Form["productName"];
                        var productDescription = Request.Form["productDescription"];
                        var productQuantityPerUnit = Request.Form["productQuantityPerUnit"];
                        var productCostPrice = !string.IsNullOrEmpty(Request.Form["productCostPrice"])
                            ? Convert.ToDouble(Request.Form["productCostPrice"])
                            : 0;
                        var productSellingPrice = !string.IsNullOrEmpty(Request.Form["productSellingPrice"])
                            ? Convert.ToDouble(Request.Form["productSellingPrice"])
                            : 0;
                        var productUnitsInStock = !string.IsNullOrEmpty(Request.Form["productUnitsInStock"])
                            ? Convert.ToDouble(Request.Form["productUnitsInStock"])
                            : 0;
                        var productUnitsOnOrder = !string.IsNullOrEmpty(Request.Form["productUnitsOnOrder"])
                            ? Convert.ToDouble(Request.Form["productUnitsOnOrder"])
                            : 0;
                        var productReOrderLevel = !string.IsNullOrEmpty(Request.Form["productReOrderLevel"])
                            ? Convert.ToDouble(Request.Form["productReOrderLevel"])
                            : 0;

                        ProductModel productModel = new()
                        {
                            SupplierKey = supplierKey,
                            ProductCategoryKey = productCategoryKey,
                            ProductTypeKey = productTypeKey,
                            ProductName = productName,
                            ProductDescription = productDescription,
                            ProductQuantityPerUnit = productQuantityPerUnit,
                            ProductCostPrice = productCostPrice,
                            ProductSellingPrice = productSellingPrice,
                            ProductUnitsInStock = productUnitsInStock,
                            ProductUnitsOnOrder = productUnitsOnOrder,
                            ProductReOrderLevel = productReOrderLevel
                        };
                        var lastInsertKey = productRepository.Create(productModel);
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
                        var data = productRepository.Read();
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
                    try
                    {
                        var search = Request.Form["search"];
                        var data = productRepository.Search(search);
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

                break;
            case "single":
                if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.READ_ACCESS))
                {
                    code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.Form["productKey"]))
                    {
                        try
                        {
                            if (!uint.TryParse(Request.Form["productKey"], out var productKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }

                            if (productKey == 0)
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                            }
                            else
                            {
                                ProductModel productModel = new()
                                {
                                    ProductKey = productKey
                                };
                                var dataSingle = productRepository.GetSingle(productModel);
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
                    if (!string.IsNullOrEmpty(Request.Form["productKey"]))
                    {
                        try
                        {
                            if (!uint.TryParse(Request.Form["productKey"], out var productKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }

                            if (productKey > 0)
                            {
                                uint supplierKey;
                                if (!string.IsNullOrWhiteSpace(Request.Form["supplierKey"]))
                                {
                                    if (!uint.TryParse(Request.Form["supplierKey"], out supplierKey))
                                    {
                                        code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                        return Ok(new {status, code});
                                    }
                                }
                                else
                                {
                                    supplierKey = supplierRepository.GetDefault();
                                }

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

                                uint productTypeKey;
                                if (!string.IsNullOrWhiteSpace(Request.Form["productTypeKey"]))
                                {
                                    if (!uint.TryParse(Request.Form["productTypeKey"], out productTypeKey))
                                    {
                                        code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                        return Ok(new {status, code});
                                    }
                                }
                                else
                                {
                                    productTypeKey = productTypeRepository.GetDefault();
                                }


                                var productName = Request.Form["productName"];
                                var productDescription = Request.Form["productDescription"];
                                var productQuantityPerUnit = Request.Form["productQuantityPerUnit"];

                                var productCostPrice = !string.IsNullOrEmpty(Request.Form["productCostPrice"])
                                    ? Convert.ToDouble(Request.Form["productCostPrice"])
                                    : 0;
                                var productSellingPrice = !string.IsNullOrEmpty(Request.Form["productSellingPrice"])
                                    ? Convert.ToDouble(Request.Form["productSellingPrice"])
                                    : 0;
                                var productUnitsInStock = !string.IsNullOrEmpty(Request.Form["productUnitsInStock"])
                                    ? Convert.ToDouble(Request.Form["productUnitsInStock"])
                                    : 0;
                                var productUnitsOnOrder = !string.IsNullOrEmpty(Request.Form["productUnitsOnOrder"])
                                    ? Convert.ToDouble(Request.Form["productUnitsOnOrder"])
                                    : 0;
                                var productReOrderLevel = !string.IsNullOrEmpty(Request.Form["productReOrderLevel"])
                                    ? Convert.ToDouble(Request.Form["productReOrderLevel"])
                                    : 0;

                                ProductModel productModel = new()
                                {
                                    ProductKey = productKey,
                                    SupplierKey = supplierKey,
                                    ProductCategoryKey = productCategoryKey,
                                    ProductTypeKey = productTypeKey,
                                    ProductName = productName,
                                    ProductDescription = productDescription,
                                    ProductQuantityPerUnit = productQuantityPerUnit,
                                    ProductCostPrice = productCostPrice,
                                    ProductSellingPrice = productSellingPrice,
                                    ProductUnitsInStock = productUnitsInStock,
                                    ProductUnitsOnOrder = productUnitsOnOrder,
                                    ProductReOrderLevel = productReOrderLevel
                                };
                                productRepository.Update(productModel);
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
                    if (!string.IsNullOrEmpty(Request.Form["productKey"]))
                    {
                        try
                        {
                            if (!uint.TryParse(Request.Form["productKey"], out var productKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }

                            if (productKey > 0)
                            {
                                ProductModel productModel = new()
                                {
                                    ProductKey = productKey
                                };
                                productRepository.Delete(productModel);
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