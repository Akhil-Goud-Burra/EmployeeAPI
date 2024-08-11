using AutoMapper;
using Employee_API.Data;
using Employee_API.Logging;
using Employee_API.Models;
using Employee_API.Models.Dto;
using Employee_API.Repository;
using Employee_API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;

namespace Employee_API.Controllers
{

    [Route("api/employeeapi/")] 
    [ApiController] 

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public class EmployeeAPIController : Controller
    {
        private readonly ILogging Custom_Logger;

        private readonly IEmployeeRepository _dbEmployee;

        private readonly IMapper _mapper;

        protected APIResponse _response;

        public EmployeeAPIController(ILogging logger, IEmployeeRepository dbEmployee, IMapper _mapper)
        {
            Custom_Logger = logger;
            this._dbEmployee = dbEmployee;
            this._mapper = _mapper;
            this._response = new();
        }



        // Getting all the Employees
        [HttpGet]
        public async Task< ActionResult<APIResponse> > GetEmployees()
        {
            try
            {
                Custom_Logger.Log("Attempting to get all the employees", "");

                // Fetch the employees from the database asynchronously
                var employees = await _dbEmployee.GetAllAsync();

                // Check if any employees were found
                if (employees == null || !employees.Any())
                {
                    Custom_Logger.Log("No employees found", "");
                    return NotFound(new { Message = "No employees found" });
                }

                // Logging the success Message
                Custom_Logger.Log("Successfully retrieved employees", "");

                _response.Result = employees;
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Logging the exception
                Custom_Logger.Log("An error occurred while getting employees", ex.Message);

                // Return an internal server error response
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while retrieving employees", Details = ex.Message });
            }

        }








        // Getting the Selected Employee

        [HttpGet("{id:int}", Name = "GetEmployee")]
        public async Task< ActionResult<APIResponse> > GetEmployee(int id)
        {

            try
            {
                // Log the attempt to find an employee.
                Custom_Logger.Log("Attempting to get an employee", $"EmployeeID: {id}");

                // Attempt to fetch the employee from the database asynchronously.
                var employee = await _dbEmployee.GetAsync(u => u.Id == id);

                // Check if the employee was not found.
                if (employee == null)
                {
                    Custom_Logger.Log("No employee found with the specified ID", $"EmployeeID: {id}");

                    return NotFound(new { Message = $"No employee found with ID {id}." });
                }

                // Log the successful retrieval
                Custom_Logger.Log("Employee retrieved successfully", $"EmployeeID: {id}");

                _response.Result = employee;
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Log the exception
                Custom_Logger.Log("An error occurred while retrieving an employee", $"EmployeeID: {id}, Error: {ex.Message}");

                // Return an internal server error response
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while retrieving the employee", Details = ex.Message });
            }
        }










        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task< ActionResult<APIResponse> > CreateEmployee([FromBody] EmployeeCreateDTO employee_dto)
        {
            try
            {
                if (employee_dto == null)
                {
                    Custom_Logger.Log("Attempt to create a null employee", "");
                    return BadRequest(new { Message = "Employee data is null." });
                }

                var employeeName = employee_dto.Name.Trim().ToLower();

                var employeeExistence = await _dbEmployee.GetAsync(u => u.Name.ToLower() == employeeName);


                if (employeeExistence != null)
                {
                    Custom_Logger.Log("Attempt to create a duplicate employee", $"EmployeeName: {employeeName}");
                    return Conflict(new { Message = "An employee with the same name already exists." });
                }


                // Used Auto Mapper.
                Employee model = _mapper.Map<Employee>(employee_dto);


                await _dbEmployee.CreateAsync(model);


                Custom_Logger.Log("Employee created successfully", $"EmployeeName: {model.Name}");

                _response.Result = model;
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;


                return CreatedAtRoute("GetEmployee", new { id = model.Id }, _response);
            }
            catch (Exception ex)
            {
                // Logging the exception
                Custom_Logger.Log("An error occurred while getting employees", ex.Message);

                // Return an internal server error response
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while retrieving employees", Details = ex.Message });
            }
        }












        //Deleting the Employee
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpDelete("{id:int}", Name = "DeleteEmployee")]

        public async Task<APIResponse> DeleteEmployee([FromBody] EmployeeDeleteDTO employee_delete_dto)
        {
            try
            {
                if (employee_delete_dto.Id <= 0)
                {
                    Custom_Logger.Log("Attempt to delete an employee with invalid ID", $"EmployeeID: {employee_delete_dto.Id}");

                    _response.InputValidationError = "Invalid ID. Please provide a valid employee ID.";
                    _response.StatusCode = (HttpStatusCode)StatusCodes.Status400BadRequest;

                    return _response;
                }

                var employeeToBeDeleted = await _dbEmployee.GetAsync(u => u.Id == (employee_delete_dto.Id));

                if (employeeToBeDeleted == null)
                {
                    Custom_Logger.Log("No employee found for deletion", $"EmployeeID: {employee_delete_dto.Id}");

                    _response.InputValidationError = $"No employee found with ID {employee_delete_dto.Id}.";
                    _response.StatusCode = (HttpStatusCode)StatusCodes.Status400BadRequest;

                    return _response;
                }


                await _dbEmployee.DeleteAsync(employeeToBeDeleted);

                Custom_Logger.Log("Employee deleted successfully", $"EmployeeID: {employee_delete_dto.Id}");

                _response.Result = $"Employee deleted successfully ID {employee_delete_dto.Id}.";
                _response.StatusCode = (HttpStatusCode)StatusCodes.Status204NoContent;
                _response.IsSuccess = true;

                return _response;
            }
            catch (Exception ex)
            {
                // Logging the exception
                Custom_Logger.Log("An error occurred while getting employees", ex.Message);

                // Return an internal server error response

                _response.ErrorMessage = "An error occurred while retrieving employees";
                _response.StatusCode = (HttpStatusCode)StatusCodes.Status500InternalServerError;
                _response.ThrowedException = ex.Message;

                return _response;
            }
        }











        [HttpPut("{id:int}", Name = "UpdateEmployee")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<APIResponse> UpdateVilla(int id, [FromBody] EmployeeUpdateDTO employee_dto)
        {

            try
            {
                if (employee_dto == null)
                {
                    Custom_Logger.Log("Attempt to update an employee with null data", "");

                    _response.InputValidationError = "Attempt to update an employee with null data";
                    _response.StatusCode = (HttpStatusCode)StatusCodes.Status400BadRequest;

                    return _response;
                }

                if (id != employee_dto.Id)
                {
                    Custom_Logger.Log("Mismatched ID in the request", $"Route ID: {id}, Employee ID: {employee_dto.Id}");

                    _response.InputValidationError = $"Mismatched ID in the request Route ID: {id}, Employee ID: {employee_dto.Id}";
                    _response.StatusCode = (HttpStatusCode)StatusCodes.Status400BadRequest;

                    return _response;
                }

                var employeeToUpdate = await _dbEmployee.GetAsync(e => e.Id == id);
                if (employeeToUpdate == null)
                {
                    Custom_Logger.Log("Employee not found for update", $"EmployeeID: {id}");

                    _response.InputValidationError = $"Employee not found for update EmployeeID: {{id}}";
                    _response.StatusCode = (HttpStatusCode)StatusCodes.Status400BadRequest;

                    return _response;
                }

                // Update the properties of the existing employee
                employeeToUpdate.Name = employee_dto.Name;

                await _dbEmployee.UpdateAsync(employeeToUpdate);

                Custom_Logger.Log("Employee updated successfully", $"EmployeeID: {id}");

                _response.ErrorMessage = $"Employee updated successfully EmployeeID: {{id}}";
                _response.StatusCode = (HttpStatusCode)StatusCodes.Status204NoContent;
                _response.IsSuccess = true;

                return _response;
            }
            catch (Exception ex)
            {
                // Logging the exception
                Custom_Logger.Log("An error occurred while getting employees", ex.Message);

                // Return an internal server error response
                _response.ErrorMessage = "An error occurred while retrieving employees";
                _response.StatusCode = (HttpStatusCode)StatusCodes.Status500InternalServerError;
                _response.ThrowedException = ex.Message;

                return _response;
            }
        }

    }

}
