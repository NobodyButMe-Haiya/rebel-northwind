using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Application;
using RebelCmsTemplate.Models.Shared;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Repository.Application;

public class EmployeeRepository
{
    private readonly SharedUtil _sharedUtil;

    public EmployeeRepository(IHttpContextAccessor httpContextAccessor)
    {
        _sharedUtil = new SharedUtil(httpContextAccessor);
    }

    public int Create(EmployeeModel employeeModel)
    {
        int lastInsertKey;
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();


            // this is special for photo /blob
            if (employeeModel.EmployeePhoto?.Length > 0)
            {
                sql +=
                    @"INSERT INTO employee (employeeId,tenantId,employeeFirstName,employeeLastName,employeeTitle,employeeTitleOfCourtesy,employeeBirthDate,employeeHireDate,employeeAddress,employeeCity,employeeRegion,employeePostalCode,employeeCountry,employeeHomePhone,employeeExtension,employeeNotes,employeePhotoPath,employeeSalary,isDelete) VALUES (null,@tenantId,@employeeFirstName,@employeeLastName,@employeeTitle,@employeeTitleOfCourtesy,@employeeBirthDate,@employeeHireDate,@employeeAddress,@employeeCity,@employeeRegion,@employeePostalCode,@employeeCountry,@employeeHomePhone,@employeeExtension,@employeeNotes,@employeePhotoPath,@employeeSalary,@isDelete);";

                parameterModels = new List<ParameterModel>
                {
                    new()
                    {
                        Key = "@tenantId",
                        Value = _sharedUtil.GetTenantId()
                    },
                    new()
                    {
                        Key = "@employeeFirstName",
                        Value = employeeModel.EmployeeFirstName
                    },
                    new()
                    {
                        Key = "@employeeLastName",
                        Value = employeeModel.EmployeeLastName
                    },
                    new()
                    {
                        Key = "@employeeTitle",
                        Value = employeeModel.EmployeeTitle
                    },
                    new()
                    {
                        Key = "@employeeTitleOfCourtesy",
                        Value = employeeModel.EmployeeTitleOfCourtesy
                    },
                    new()
                    {
                        Key = "@employeeBirthDate",
                        Value = employeeModel.EmployeeBirthDate?.ToString("yyyy-MM-dd")
                    },
                    new()
                    {
                        Key = "@employeeHireDate",
                        Value = employeeModel.EmployeeHireDate?.ToString("yyyy-MM-dd")
                    },
                    new()
                    {
                        Key = "@employeeAddress",
                        Value = employeeModel.EmployeeAddress
                    },
                    new()
                    {
                        Key = "@employeeCity",
                        Value = employeeModel.EmployeeCity
                    },
                    new()
                    {
                        Key = "@employeeRegion",
                        Value = employeeModel.EmployeeRegion
                    },
                    new()
                    {
                        Key = "@employeePostalCode",
                        Value = employeeModel.EmployeePostalCode
                    },
                    new()
                    {
                        Key = "@employeeCountry",
                        Value = employeeModel.EmployeeCountry
                    },
                    new()
                    {
                        Key = "@employeeHomePhone",
                        Value = employeeModel.EmployeeHomePhone
                    },
                    new()
                    {
                        Key = "@employeeExtension",
                        Value = employeeModel.EmployeeExtension
                    },

                    new()
                    {
                        Key = "@employeeNotes",
                        Value = employeeModel.EmployeeNotes
                    },
                    new()
                    {
                        Key = "@employeePhoto",
                        Value = employeeModel.EmployeePhoto
                    },
                    new()
                    {
                        Key = "@employeePhotoPath",
                        Value = employeeModel.EmployeePhotoPath
                    },
                    new()
                    {
                        Key = "@employeeSalary",
                        Value = employeeModel.EmployeeSalary
                    },
                    new()
                    {
                        Key = "@isDelete",
                        Value = 0
                    }
                };
            }
            else
            {
                sql +=
                    @"INSERT INTO employee (employeeId,tenantId,employeeFirstName,employeeLastName,employeeTitle,employeeTitleOfCourtesy,employeeBirthDate,employeeHireDate,employeeAddress,employeeCity,employeeRegion,employeePostalCode,employeeCountry,employeeHomePhone,employeeExtension,employeeNotes,employeePhotoPath,employeeSalary,isDelete) VALUES (null,@tenantId,@employeeFirstName,@employeeLastName,@employeeTitle,@employeeTitleOfCourtesy,@employeeBirthDate,@employeeHireDate,@employeeAddress,@employeeCity,@employeeRegion,@employeePostalCode,@employeeCountry,@employeeHomePhone,@employeeExtension,@employeeNotes,@employeePhotoPath,@employeeSalary,@isDelete);";


                parameterModels = new List<ParameterModel>
                {
                    new()
                    {
                        Key = "@tenantId",
                        Value = _sharedUtil.GetTenantId()
                    },
                    new()
                    {
                        Key = "@employeeFirstName",
                        Value = employeeModel.EmployeeFirstName
                    },
                    new()
                    {
                        Key = "@employeeLastName",
                        Value = employeeModel.EmployeeLastName
                    },
                    new()
                    {
                        Key = "@employeeTitle",
                        Value = employeeModel.EmployeeTitle
                    },
                    new()
                    {
                        Key = "@employeeTitleOfCourtesy",
                        Value = employeeModel.EmployeeTitleOfCourtesy
                    },
                    new()
                    {
                        Key = "@employeeBirthDate",
                        Value = employeeModel.EmployeeBirthDate?.ToString("yyyy-MM-dd")
                    },
                    new()
                    {
                        Key = "@employeeHireDate",
                        Value = employeeModel.EmployeeHireDate?.ToString("yyyy-MM-dd")
                    },
                    new()
                    {
                        Key = "@employeeAddress",
                        Value = employeeModel.EmployeeAddress
                    },
                    new()
                    {
                        Key = "@employeeCity",
                        Value = employeeModel.EmployeeCity
                    },
                    new()
                    {
                        Key = "@employeeRegion",
                        Value = employeeModel.EmployeeRegion
                    },
                    new()
                    {
                        Key = "@employeePostalCode",
                        Value = employeeModel.EmployeePostalCode
                    },
                    new()
                    {
                        Key = "@employeeCountry",
                        Value = employeeModel.EmployeeCountry
                    },
                    new()
                    {
                        Key = "@employeeHomePhone",
                        Value = employeeModel.EmployeeHomePhone
                    },
                    new()
                    {
                        Key = "@employeeExtension",
                        Value = employeeModel.EmployeeExtension
                    },

                    new()
                    {
                        Key = "@employeeNotes",
                        Value = employeeModel.EmployeeNotes
                    },
                    new()
                    {
                        Key = "@employeePhotoPath",
                        Value = employeeModel.EmployeePhotoPath
                    },
                    new()
                    {
                        Key = "@employeeSalary",
                        Value = employeeModel.EmployeeSalary
                    },
                    new()
                    {
                        Key = "@isDelete",
                        Value = 0
                    }
                };
            }

            MySqlCommand mySqlCommand = new(sql, connection);

            foreach (var parameter in parameterModels)
            {
                mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            mySqlCommand.ExecuteNonQuery();
            mySqlTransaction.Commit();
            lastInsertKey = (int)mySqlCommand.LastInsertedId;
            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
            throw new Exception(ex.Message);
        }

        return lastInsertKey;
    }

    public List<EmployeeModel> Read()
    {
        List<EmployeeModel> employeeModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql = @"
            SELECT      *
            FROM        employee 
            WHERE       isDelete != 1
            AND         tenantId = @tenantId
            ORDER BY    employeeId DESC ";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@tenantId",
                    Value = _sharedUtil.GetTenantId()
                }
            };
            foreach (var parameter in parameterModels)
            {
                mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            _sharedUtil.SetSqlSession(sql, parameterModels);
            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    employeeModels.Add(new EmployeeModel
                    {
                        EmployeeKey = Convert.ToUInt32(reader["employeeId"]),
                        EmployeeFirstName = reader["employeeFirstName"].ToString(),
                        EmployeeLastName = reader["employeeLastName"].ToString(),
                        EmployeeTitle = reader["employeeTitle"].ToString(),
                        EmployeeTitleOfCourtesy = reader["employeeTitleOfCourtesy"].ToString(),
                        EmployeeBirthDate = reader["employeeBirthDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["employeeBirthDate"])
                            : null,
                        EmployeeHireDate = reader["employeeHireDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["employeeHireDate"])
                            : null,
                        EmployeeAddress = reader["employeeAddress"].ToString(),
                        EmployeeCity = reader["employeeCity"].ToString(),
                        EmployeeRegion = reader["employeeRegion"].ToString(),
                        EmployeePostalCode = reader["employeePostalCode"].ToString(),
                        EmployeeCountry = reader["employeeCountry"].ToString(),
                        EmployeeHomePhone = reader["employeeHomePhone"].ToString(),
                        EmployeeExtension = reader["employeeExtension"].ToString(),
                        EmployeePhoto = reader["employeePhoto"] != DBNull.Value ? (byte[])reader["employeePhoto"] : null,
                        EmployeeNotes = reader["employeeNotes"].ToString(),
                        EmployeePhotoPath = reader["employeePhotoPath"].ToString(),
                        EmployeeSalary = Convert.ToDouble(reader["employeeSalary"])
                    });
                }
            }

            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
            throw new Exception(ex.Message);
        }

        return employeeModels;
    }

    public List<EmployeeModel> Search(string search)
    {
        List<EmployeeModel> employeeModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
            SELECT  *
            FROM    employee 
            WHERE   employee.isDelete != 1
            AND     tenantId = @tenantId
            AND (
                employee.employeeFirstName LIKE CONCAT('%',@search,'%') OR
                employee.employeeLastName LIKE CONCAT('%',@search,'%') OR
                employee.employeeTitle LIKE CONCAT('%',@search,'%') OR
                employee.employeeTitleOfCourtesy LIKE CONCAT('%',@search,'%') OR
                employee.employeeBirthDate LIKE CONCAT('%',@search,'%') OR
                employee.employeeHireDate LIKE CONCAT('%',@search,'%') OR
                employee.employeeAddress LIKE CONCAT('%',@search,'%') OR
                employee.employeeCity LIKE CONCAT('%',@search,'%') OR
                employee.employeeRegion LIKE CONCAT('%',@search,'%') OR
                employee.employeePostalCode LIKE CONCAT('%',@search,'%') OR
                employee.employeeCountry LIKE CONCAT('%',@search,'%') OR
                employee.employeeHomePhone LIKE CONCAT('%',@search,'%') OR
                employee.employeeExtension LIKE CONCAT('%',@search,'%') OR
                employee.employeePhoto LIKE CONCAT('%',@search,'%') OR
                employee.employeeNotes LIKE CONCAT('%',@search,'%') OR
                employee.employeePhotoPath LIKE CONCAT('%',@search,'%') OR
                employee.employeeSalary LIKE CONCAT('%',@search,'%')
            )";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@tenantId",
                    Value = _sharedUtil.GetTenantId()
                },
                new()
                {
                    Key = "@search",
                    Value = search
                }
            };
            foreach (var parameter in parameterModels)
            {
                mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            _sharedUtil.SetSqlSession(sql, parameterModels);
            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    employeeModels.Add(new EmployeeModel
                    {
                        EmployeeFirstName = reader["employeeFirstName"].ToString(),
                        EmployeeLastName = reader["employeeLastName"].ToString(),
                        EmployeeTitle = reader["employeeTitle"].ToString(),
                        EmployeeTitleOfCourtesy = reader["employeeTitleOfCourtesy"].ToString(),
                        EmployeeBirthDate = reader["employeeBirthDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["employeeBirthDate"])
                            : null,
                        EmployeeHireDate = reader["employeeHireDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["employeeHireDate"])
                            : null,
                        EmployeeAddress = reader["employeeAddress"].ToString(),
                        EmployeeCity = reader["employeeCity"].ToString(),
                        EmployeeRegion = reader["employeeRegion"].ToString(),
                        EmployeePostalCode = reader["employeePostalCode"].ToString(),
                        EmployeeCountry = reader["employeeCountry"].ToString(),
                        EmployeeHomePhone = reader["employeeHomePhone"].ToString(),
                        EmployeeExtension = reader["employeeExtension"].ToString(),
                        EmployeePhoto = reader["employeePhoto"] != DBNull.Value ? (byte[])reader["employeePhoto"] : null,
                        EmployeeNotes = reader["employeeNotes"].ToString(),
                        EmployeePhotoPath = reader["employeePhotoPath"].ToString(),
                        EmployeeSalary = Convert.ToDouble(reader["employeeSalary"])
                    });
                }
            }

            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
            throw new Exception(ex.Message);
        }

        return employeeModels;
    }

    public EmployeeModel GetSingle(EmployeeModel employeeModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
            SELECT  *
            FROM    employee 
            WHERE   isDelete    != 1
            AND     tenantId    = @tenantId
            AND     employeeId  =   @employeeId
            LIMIT 1";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@tenantId",
                    Value = _sharedUtil.GetTenantId()
                },
                new()
                {
                    Key = "@employeeId",
                    Value = employeeModel.EmployeeKey
                }
            };
            foreach (var parameter in parameterModels)
            {
                mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            _sharedUtil.SetSqlSession(sql, parameterModels);
            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    employeeModel = new EmployeeModel
                    {
                        EmployeeKey = Convert.ToUInt32(reader["employeeId"]),
                        EmployeeFirstName = reader["employeeFirstName"].ToString(),
                        EmployeeLastName = reader["employeeLastName"].ToString(),
                        EmployeeTitle = reader["employeeTitle"].ToString(),
                        EmployeeTitleOfCourtesy = reader["employeeTitleOfCourtesy"].ToString(),
                        EmployeeBirthDate = reader["employeeBirthDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["employeeBirthDate"])
                            : null,
                        EmployeeHireDate = reader["employeeHireDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["employeeHireDate"])
                            : null,
                        EmployeeAddress = reader["employeeAddress"].ToString(),
                        EmployeeCity = reader["employeeCity"].ToString(),
                        EmployeeRegion = reader["employeeRegion"].ToString(),
                        EmployeePostalCode = reader["employeePostalCode"].ToString(),
                        EmployeeCountry = reader["employeeCountry"].ToString(),
                        EmployeeHomePhone = reader["employeeHomePhone"].ToString(),
                        EmployeeExtension = reader["employeeExtension"].ToString(),
                        EmployeePhoto = reader["employeePhoto"] != DBNull.Value ? (byte[])reader["employeePhoto"] : null,
                        EmployeeNotes = reader["employeeNotes"].ToString(),
                        EmployeePhotoPath = reader["employeePhotoPath"].ToString(),
                        EmployeeSalary = Convert.ToDouble(reader["employeeSalary"])
                    };
                }
            }

            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
            throw new Exception(ex.Message);
        }

        return employeeModel;
    }

    public uint GetDefault()
    {
        uint employeeId;
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
            SELECT  employeeId
            FROM    employee 
            WHERE   isDelete    != 1
            AND     tenantId    = @tenantId
            AND     isDefault   =   1
            LIMIT 1";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@tenantId",
                    Value = _sharedUtil.GetTenantId()
                }
            };
            foreach (var parameter in parameterModels)
            {
                mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            _sharedUtil.SetSqlSession(sql, parameterModels);
            employeeId = (uint)mySqlCommand.ExecuteScalar();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
            throw new Exception(ex.Message);
        }

        return employeeId;
    }

    public byte[] GetExcel()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Administrator > Employee ");
        worksheet.Cell(1, 1).Value = "First Name";
        worksheet.Cell(1, 2).Value = "Last Name";
        worksheet.Cell(1, 3).Value = "Title";
        worksheet.Cell(1, 4).Value = "Title Of Courtesy";
        worksheet.Cell(1, 5).Value = "Birth Date";
        worksheet.Cell(1, 6).Value = "Hire Date";
        worksheet.Cell(1, 7).Value = "Address";
        worksheet.Cell(1, 8).Value = "City";
        worksheet.Cell(1, 9).Value = "Region";
        worksheet.Cell(1, 10).Value = "Postal Code";
        worksheet.Cell(1, 11).Value = "Country";
        worksheet.Cell(1, 12).Value = "Home Phone";
        worksheet.Cell(1, 13).Value = "Extension";
        worksheet.Cell(1, 14).Value = "Photo";
        worksheet.Cell(1, 15).Value = "Notes";
        worksheet.Cell(1, 16).Value = "Photo Path";
        worksheet.Cell(1, 17).Value = "Salary";
        var sql = _sharedUtil.GetSqlSession();
        var parameterModels = _sharedUtil.GetListSqlParameter();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            MySqlCommand mySqlCommand = new(sql, connection);
            if (parameterModels != null)
            {
                foreach (var parameter in parameterModels)
                {
                    mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
            }

            using (var reader = mySqlCommand.ExecuteReader())
            {
                var counter = 3;
                while (reader.Read())
                {
                    var currentRow = counter++;
                    worksheet.Cell(currentRow, 1).Value = reader["employeeFirstName"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["employeeLastName"].ToString();
                    worksheet.Cell(currentRow, 3).Value = reader["employeeTitle"].ToString();
                    worksheet.Cell(currentRow, 4).Value = reader["employeeTitleOfCourtesy"].ToString();
                    worksheet.Cell(currentRow, 5).Value = reader["employeeBirthDate"].ToString();
                    worksheet.Cell(currentRow, 6).Value = reader["employeeHireDate"].ToString();
                    worksheet.Cell(currentRow, 7).Value = reader["employeeAddress"].ToString();
                    worksheet.Cell(currentRow, 8).Value = reader["employeeCity"].ToString();
                    worksheet.Cell(currentRow, 9).Value = reader["employeeRegion"].ToString();
                    worksheet.Cell(currentRow, 10).Value = reader["employeePostalCode"].ToString();
                    worksheet.Cell(currentRow, 11).Value = reader["employeeCountry"].ToString();
                    worksheet.Cell(currentRow, 12).Value = reader["employeeHomePhone"].ToString();
                    worksheet.Cell(currentRow, 13).Value = reader["employeeExtension"].ToString();
                    worksheet.Cell(currentRow, 14).Value = reader["employeePhoto"].ToString();
                    worksheet.Cell(currentRow, 15).Value = reader["employeeNotes"].ToString();
                    worksheet.Cell(currentRow, 16).Value = reader["employeePhotoPath"].ToString();
                    worksheet.Cell(currentRow, 17).Value = reader["employeeSalary"].ToString();
                }
            }

            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            throw new Exception(ex.Message);
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public void Update(EmployeeModel employeeModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();

            sql = @"
            UPDATE  employee 
            SET     tenantId                =   @tenantId,
                    employeeFirstName       =   @employeeFirstName,
                    employeeLastName        =   @employeeLastName,
                    employeeTitle           =   @employeeTitle,
                    employeeTitleOfCourtesy =   @employeeTitleOfCourtesy,
                    employeeBirthDate       =   @employeeBirthDate,
                    employeeHireDate        =   @employeeHireDate,
                    employeeAddress         =   @employeeAddress,
                    employeeCity            =   @employeeCity,
                    employeeRegion          =   @employeeRegion,
                    employeePostalCode      =   @employeePostalCode,
                    employeeCountry         =   @employeeCountry,
                    employeeHomePhone       =   @employeeHomePhone,
                    employeeExtension       =   @employeeExtension,";
            if (employeeModel.EmployeePhoto?.Length > 0)
            {
                sql += @"employeePhoto           =   @employeePhoto,";
            }
            sql += @"
                    employeeNotes           =   @employeeNotes,
                    employeePhotoPath       =   @employeePhotoPath,
                    employeeSalary          =   @employeeSalary,
                    isDelete                =   @isDelete
            WHERE   employeeId              =   @employeeId";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@employeeId",
                    Value = employeeModel.EmployeeKey
                },
                new()
                {
                    Key = "@tenantId",
                    Value = _sharedUtil.GetTenantId()
                },
                new()
                {
                    Key = "@employeeFirstName",
                    Value = employeeModel.EmployeeFirstName
                },
                new()
                {
                    Key = "@employeeLastName",
                    Value = employeeModel.EmployeeLastName
                },
                new()
                {
                    Key = "@employeeTitle",
                    Value = employeeModel.EmployeeTitle
                },
                new()
                {
                    Key = "@employeeTitleOfCourtesy",
                    Value = employeeModel.EmployeeTitleOfCourtesy
                },
                new()
                {
                    Key = "@employeeBirthDate",
                    Value = employeeModel.EmployeeBirthDate?.ToString("yyyy-MM-dd")
                },
                new()
                {
                    Key = "@employeeHireDate",
                    Value = employeeModel.EmployeeHireDate?.ToString("yyyy-MM-dd")
                },
                new()
                {
                    Key = "@employeeAddress",
                    Value = employeeModel.EmployeeAddress
                },
                new()
                {
                    Key = "@employeeCity",
                    Value = employeeModel.EmployeeCity
                },
                new()
                {
                    Key = "@employeeRegion",
                    Value = employeeModel.EmployeeRegion
                },
                new()
                {
                    Key = "@employeePostalCode",
                    Value = employeeModel.EmployeePostalCode
                },
                new()
                {
                    Key = "@employeeCountry",
                    Value = employeeModel.EmployeeCountry
                },
                new()
                {
                    Key = "@employeeHomePhone",
                    Value = employeeModel.EmployeeHomePhone
                },
                new()
                {
                    Key = "@employeeExtension",
                    Value = employeeModel.EmployeeExtension
                },

                new()
                {
                    Key = "@employeeNotes",
                    Value = employeeModel.EmployeeNotes
                },
                new()
                {
                    Key = "@employeePhotoPath",
                    Value = employeeModel.EmployeePhotoPath
                },
                new()
                {
                    Key = "@employeeSalary",
                    Value = employeeModel.EmployeeSalary
                },
                new()
                {
                    Key = "@isDelete",
                    Value = 0
                }
            };
            if (employeeModel.EmployeePhoto?.Length > 0)
            {
                mySqlCommand.Parameters.AddWithValue("@employeePhoto", employeeModel.EmployeePhoto);
            }

            foreach (var parameter in parameterModels)
            {
                mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            mySqlCommand.ExecuteNonQuery();
            mySqlTransaction.Commit();
            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
            throw new Exception(ex.Message);
        }
    }

    public void Delete(EmployeeModel employeeModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();
            sql = @"
            UPDATE  employee 
            SET     isDelete    =   1
            WHERE   employeeId    =   @employeeId";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@employeeId",
                    Value = employeeModel.EmployeeKey
                }
            };
            foreach (var parameter in parameterModels)
            {
                mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            mySqlCommand.ExecuteNonQuery();
            mySqlTransaction.Commit();
            mySqlCommand.Dispose();
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
            throw new Exception(ex.Message);
        }
    }
}