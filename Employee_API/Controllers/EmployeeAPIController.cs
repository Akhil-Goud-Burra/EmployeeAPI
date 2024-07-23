using Employee_API.Data;
using Employee_API.Logging;
using Employee_API.Models;
using Employee_API.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System;

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

        private readonly ApplicationDbContext appDbContext;

        public EmployeeAPIController(ILogging logger, ApplicationDbContext appDbContext)
        {
            Custom_Logger = logger;
            this.appDbContext = appDbContext;
        }



        // Getting all the Employees
        [HttpGet]
        public ActionResult<IEnumerable<EmployeeDTO>> GetEmployees()
        {
            try
            {
                Custom_Logger.Log("Attempting to get all the employees", "");

                // Fetch the employees from the database
                var employees = appDbContext.Employee_Table.ToList();

                // Check if any employees were found
                if (employees == null || !employees.Any())
                {
                    Custom_Logger.Log("No employees found", "");
                    return NotFound(new { Message = "No employees found" });
                }

                // Logging the success Message
                Custom_Logger.Log("Successfully retrieved employees", "");

                return Ok(employees);
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
        public ActionResult<IEnumerable<EmployeeDTO>> GetEmployee(int id)
        {

            try
            {
                // Log the attempt to find an employee
                Custom_Logger.Log("Attempting to get an employee", $"EmployeeID: {id}");

                // Attempt to fetch the employee from the database
                var employee = appDbContext.Employee_Table.FirstOrDefault(u => u.Id == id);

                // Check if the employee was not found
                if (employee == null)
                {
                    Custom_Logger.Log("No employee found with the specified ID", $"EmployeeID: {id}");

                    return NotFound(new { Message = $"No employee found with ID {id}." });
                }

                // Log the successful retrieval
                Custom_Logger.Log("Employee retrieved successfully", $"EmployeeID: {id}");

                return Ok(employee);
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
        public ActionResult<EmployeeDTO> CreateEmployee([FromBody] EmployeeDTO employee_dto)
        {
            try
            {
                if (employee_dto == null)
                {
                    Custom_Logger.Log("Attempt to create a null employee", "");
                    return BadRequest(new { Message = "Employee data is null." });
                }

                var employeeName = employee_dto.Name.Trim().ToLower();

                var employeeExistence = appDbContext.Employee_Table.FirstOrDefault(u => u.Name.ToLower() == employeeName);

                if (employeeExistence != null)
                {
                    Custom_Logger.Log("Attempt to create a duplicate employee", $"EmployeeName: {employeeName}");
                    return Conflict(new { Message = "An employee with the same name already exists." });
                }

                if (employee_dto.Id > 0)
                {
                    Custom_Logger.Log("Invalid ID provided for new employee", $"EmployeeID: {employee_dto.Id}");
                    return BadRequest(new { Message = "Invalid ID. ID should not be set for new employees." });
                }

                Employee model = new()
                {
                    Name = employee_dto.Name
                };

                appDbContext.Employee_Table.Add(model);
                appDbContext.SaveChanges();

                Custom_Logger.Log("Employee created successfully", $"EmployeeName: {model.Name}");


                return CreatedAtRoute("GetEmployee", new { id = model.Id }, model);
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
        [HttpDelete("{id:int}", Name = "DeleteEmployee")]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteEmployee(int id)
        {
            try
            {
                if (id <= 0)
                {
                    Custom_Logger.Log("Attempt to delete an employee with invalid ID", $"EmployeeID: {id}");
                    return BadRequest(new { Message = "Invalid ID. Please provide a valid employee ID." });
                }

                var employeeToBeDeleted = appDbContext.Employee_Table.FirstOrDefault(u => u.Id == id);

                if (employeeToBeDeleted == null)
                {
                    Custom_Logger.Log("No employee found for deletion", $"EmployeeID: {id}");
                    return NotFound(new { Message = $"No employee found with ID {id}." });
                }

                appDbContext.Employee_Table.Remove(employeeToBeDeleted);
                appDbContext.SaveChanges();

                Custom_Logger.Log("Employee deleted successfully", $"EmployeeID: {id}");

                return NoContent();
            }
            catch (Exception ex)
            {
                // Logging the exception
                Custom_Logger.Log("An error occurred while getting employees", ex.Message);

                // Return an internal server error response
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while retrieving employees", Details = ex.Message });
            }
        }











        [HttpPut("{id:int}", Name = "UpdateEmployee")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdateVilla(int id, [FromBody] EmployeeDTO employee_dto)
        {

            try
            {
                if (employee_dto == null)
                {
                    Custom_Logger.Log("Attempt to update an employee with null data", "");
                    return BadRequest(new { Message = "Employee data cannot be null." });
                }

                if (id != employee_dto.Id)
                {
                    Custom_Logger.Log("Mismatched ID in the request", $"Route ID: {id}, Employee ID: {employee_dto.Id}");
                    return BadRequest(new { Message = "Mismatched ID in the request." });
                }

                var employeeToUpdate = appDbContext.Employee_Table.FirstOrDefault(e => e.Id == id);
                if (employeeToUpdate == null)
                {
                    Custom_Logger.Log("Employee not found for update", $"EmployeeID: {id}");
                    return NotFound(new { Message = $"No employee found with ID {id}." });
                }

                // Update the properties of the existing employee
                employeeToUpdate.Name = employee_dto.Name;

                appDbContext.Employee_Table.Update(employeeToUpdate);
                appDbContext.SaveChanges();

                Custom_Logger.Log("Employee updated successfully", $"EmployeeID: {id}");

                return NoContent();
            }
            catch (Exception ex)
            {
                // Logging the exception
                Custom_Logger.Log("An error occurred while getting employees", ex.Message);

                // Return an internal server error response
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while retrieving employees", Details = ex.Message });
            }
        }

    }

}
