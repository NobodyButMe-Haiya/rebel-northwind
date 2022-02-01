using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Enum;
using RebelCmsTemplate.Models.Application;
using RebelCmsTemplate.Repository.Application;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Controllers.Application;

[Route("api/application/[controller]")]
[ApiController]
public class EmployeeController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RenderViewToStringUtil _renderViewToStringUtil;

    public EmployeeController(RenderViewToStringUtil renderViewToStringUtil, IHttpContextAccessor httpContextAccessor)
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

        EmployeeRepository employeeRepository = new(_httpContextAccessor);
        var content = employeeRepository.GetExcel();
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "employee94.xlsx");
    }

    [HttpPost]
    public async Task<ActionResult> Post()
    {
        string code;
        var status = false;
        var mode = Request.Form["mode"];
        var leafCheckKey = Convert.ToInt32(Request.Form["leafCheckKey"]);
        EmployeeRepository employeeRepository = new(_httpContextAccessor);
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
                        var employeeLastName = Request.Form["employeeLastName"];
                        var employeeFirstName = Request.Form["employeeFirstName"];
                        var employeeTitle = Request.Form["employeeTitle"];
                        var employeeTitleOfCourtesy = Request.Form["employeeTitleOfCourtesy"];
                        var employeeBirthDate = DateOnly.FromDateTime(DateTime.Now);
                        if (!string.IsNullOrEmpty(Request.Form["employeeBirthDate"]))
                        {
                            var dateString = Request.Form["employeeBirthDate"].ToString().Split("-");
                            employeeBirthDate = new DateOnly(Convert.ToInt32(dateString[0]),
                                Convert.ToInt32(dateString[1]),
                                Convert.ToInt32(dateString[2]));
                        }

                        var employeeHireDate = DateOnly.FromDateTime(DateTime.Now);
                        if (!string.IsNullOrEmpty(Request.Form["employeeHireDate"]))
                        {
                            var dateString = Request.Form["employeeHireDate"].ToString().Split("-");
                            employeeHireDate = new DateOnly(Convert.ToInt32(dateString[0]),
                                Convert.ToInt32(dateString[1]),
                                Convert.ToInt32(dateString[2]));
                        }

                        var employeeAddress = Request.Form["employeeAddress"];
                        var employeeCity = Request.Form["employeeCity"];
                        var employeeRegion = Request.Form["employeeRegion"];
                        var employeePostalCode = Request.Form["employeePostalCode"];
                        var employeeCountry = Request.Form["employeeCountry"];
                        var employeeHomePhone = Request.Form["employeeHomePhone"];
                        var employeeExtension = Request.Form["employeeExtension"];
                        var employeePhoto = Array.Empty<byte>();
                        foreach (var formFile in Request.Form.Files)
                        {
                            if (!formFile.Name.Equals("employeePhoto")) continue;
                            if (formFile.Length <= 0) continue;
                            employeePhoto = await SharedUtil.GetByteArrayFromImageAsync(formFile);
                        }

                        var employeeNotes = Request.Form["employeeNotes"];
                        var employeePhotoPath = Request.Form["employeePhotoPath"];
                        var employeeSalary = !string.IsNullOrEmpty(Request.Form["employeeSalary"])
                            ? Convert.ToDouble(Request.Form["employeeSalary"])
                            : 0;
                        EmployeeModel employeeModel = new()
                        {
                            EmployeeLastName = employeeLastName,
                            EmployeeFirstName = employeeFirstName,
                            EmployeeTitle = employeeTitle,
                            EmployeeTitleOfCourtesy = employeeTitleOfCourtesy,
                            EmployeeBirthDate = employeeBirthDate,
                            EmployeeHireDate = employeeHireDate,
                            EmployeeAddress = employeeAddress,
                            EmployeeCity = employeeCity,
                            EmployeeRegion = employeeRegion,
                            EmployeePostalCode = employeePostalCode,
                            EmployeeCountry = employeeCountry,
                            EmployeeHomePhone = employeeHomePhone,
                            EmployeeExtension = employeeExtension,
                            EmployeePhoto = employeePhoto,
                            EmployeeNotes = employeeNotes,
                            EmployeePhotoPath = employeePhotoPath,
                            EmployeeSalary = employeeSalary
                        };
                        
                        var lastInsertKey = employeeRepository.Create(employeeModel);
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
                        var data = employeeRepository.Read();
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
                            var data = employeeRepository.Search(search);

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
                    if (!string.IsNullOrEmpty(Request.Form["employeeKey"]))
                    {
                        try
                        {
                            if (!uint.TryParse(Request.Form["employeeKey"], out var employeeKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }

                            if (employeeKey == 0)
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                            }
                            else 
                            {
                                EmployeeModel employeeModel = new()
                                {
                                    EmployeeKey = employeeKey
                                };
                                var dataSingle = employeeRepository.GetSingle(employeeModel);
                                if (dataSingle.EmployeePhoto != null)
                                    dataSingle.EmployeePhotoBase64String =
                                        SharedUtil.GetImageString(dataSingle.EmployeePhoto);
                                dataSingle.EmployeePhoto = Array.Empty<byte>();
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
                    if (!string.IsNullOrEmpty(Request.Form["employeeKey"]))
                    {
                        try
                        {
                            if (!uint.TryParse(Request.Form["employeeKey"], out var employeeKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }

                            if (employeeKey > 0)
                            {
                                var employeeLastName = Request.Form["employeeLastName"];
                                var employeeFirstName = Request.Form["employeeFirstName"];
                                var employeeTitle = Request.Form["employeeTitle"];
                                var employeeTitleOfCourtesy = Request.Form["employeeTitleOfCourtesy"];
                                var employeeBirthDate = DateOnly.FromDateTime(DateTime.Now);
                                if (!string.IsNullOrEmpty(Request.Form["employeeBirthDate"]))
                                {
                                    var dateString = Request.Form["employeeBirthDate"].ToString().Split("-");
                                    employeeBirthDate = new DateOnly(Convert.ToInt32(dateString[0]),
                                        Convert.ToInt32(dateString[1]),
                                        Convert.ToInt32(dateString[2]));
                                }

                                var employeeHireDate = DateOnly.FromDateTime(DateTime.Now);
                                if (!string.IsNullOrEmpty(Request.Form["employeeHireDate"]))
                                {
                                    var dateString = Request.Form["employeeHireDate"].ToString().Split("-");
                                    employeeHireDate = new DateOnly(Convert.ToInt32(dateString[0]),
                                        Convert.ToInt32(dateString[1]),
                                        Convert.ToInt32(dateString[2]));
                                }

                                var employeeAddress = Request.Form["employeeAddress"];
                                var employeeCity = Request.Form["employeeCity"];
                                var employeeRegion = Request.Form["employeeRegion"];
                                var employeePostalCode = Request.Form["employeePostalCode"];
                                var employeeCountry = Request.Form["employeeCountry"];
                                var employeeHomePhone = Request.Form["employeeHomePhone"];
                                var employeeExtension = Request.Form["employeeExtension"];
                                var employeePhoto = Array.Empty<byte>();
                                foreach (var formFile in Request.Form.Files)
                                {
                                    if (!formFile.Name.Equals("employeePhoto")) continue;
                                    if (formFile.Length <= 0) continue;
                                    employeePhoto = await SharedUtil.GetByteArrayFromImageAsync(formFile);
                                }

                                var employeeNotes = Request.Form["employeeNotes"];
                                var employeePhotoPath = Request.Form["employeePhotoPath"];
                                var employeeSalary = !string.IsNullOrEmpty(Request.Form["employeeSalary"])
                                    ? Convert.ToDouble(Request.Form["employeeSalary"])
                                    : 0;
                                EmployeeModel employeeModel = new()
                                {
                                    EmployeeKey = employeeKey,
                                    EmployeeLastName = employeeLastName,
                                    EmployeeFirstName = employeeFirstName,
                                    EmployeeTitle = employeeTitle,
                                    EmployeeTitleOfCourtesy = employeeTitleOfCourtesy,
                                    EmployeeBirthDate = employeeBirthDate,
                                    EmployeeHireDate = employeeHireDate,
                                    EmployeeAddress = employeeAddress,
                                    EmployeeCity = employeeCity,
                                    EmployeeRegion = employeeRegion,
                                    EmployeePostalCode = employeePostalCode,
                                    EmployeeCountry = employeeCountry,
                                    EmployeeHomePhone = employeeHomePhone,
                                    EmployeeExtension = employeeExtension,
                                    EmployeePhoto = employeePhoto,
                                    EmployeeNotes = employeeNotes,
                                    EmployeePhotoPath = employeePhotoPath,
                                    EmployeeSalary = employeeSalary
                                };
                                employeeRepository.Update(employeeModel);
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
                    if (!string.IsNullOrEmpty(Request.Form["employeeKey"]))
                    {
                        try
                        {
                            if (!uint.TryParse(Request.Form["employeeKey"], out var employeeKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }

                            if (employeeKey > 0)
                            {
                                EmployeeModel employeeModel = new()
                                {
                                    EmployeeKey = employeeKey
                                };
                                employeeRepository.Delete(employeeModel);
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