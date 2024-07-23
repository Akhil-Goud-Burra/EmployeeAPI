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


        // Getting All the Employees

        [HttpGet]
        public ActionResult<IEnumerable<EmployeeDTO>> GetEmployees()
        {
            Custom_Logger.Log("Getting all the Employees", "");
            return Ok( appDbContext.Employee_Table.ToList() );
        }







        // Getting the Selected Employee
        [HttpGet("{id:int}", Name ="GetEmployee")]
        public ActionResult<IEnumerable<EmployeeDTO>> GetEmployee(int id)
        {

            var employee = appDbContext.Employee_Table.FirstOrDefault(u => u.Id == id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }









        // Creating the Employee
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<EmployeeDTO> CreateEmployee([FromBody] EmployeeDTO employee_dto) 
        {

            var empolyee_existence = appDbContext.Employee_Table.FirstOrDefault( u => u.Name.ToLower() == employee_dto.Name.ToLower() );

            if(empolyee_existence != null)
            {
                ModelState.AddModelError("Custom Error: ", "Employee Already Exists!");
                return BadRequest(ModelState);
            }

            if(employee_dto == null) { ModelState.AddModelError("Custom Error: ", "Employee is Null!");  return BadRequest(ModelState); }

            if (employee_dto.Id > 0) { ModelState.AddModelError("Custom Error: ", "Given Employee Id is Not Valid!"); return StatusCode(StatusCodes.Status400BadRequest, ModelState); }

            Employee model = new()
            {
                Id = employee_dto.Id,
                Name = employee_dto.Name
            };

            appDbContext.Employee_Table.Add(model);
            appDbContext.SaveChanges();


            Custom_Logger.Log("Employee Created", "");

            return CreatedAtRoute("GetEmployee", new {id= employee_dto .Id} ,employee_dto);
        }










        // Deleting the Employee
        [HttpDelete("{id:int}", Name = "DeleteEmployee")]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteEmployee(int id) 
        {
            if(id == 0) { return BadRequest(); }

            var Emp_to_be_Delete = appDbContext.Employee_Table.FirstOrDefault( u => u.Id == id );

            if (Emp_to_be_Delete == null) return BadRequest();

            appDbContext.Employee_Table.Remove( Emp_to_be_Delete );

            appDbContext.SaveChanges();

            Custom_Logger.Log("Employee Deleted", "");

            return NoContent();
        }










        // Updating the Employee
        [HttpPut("{id:int}", Name = "UpdateEmployee")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdateVilla(int id, [FromBody] EmployeeDTO employee_dto)
        {
            if (employee_dto == null || id != employee_dto.Id) { return BadRequest(); }

            Employee model = new()
            {
                Id = employee_dto.Id,
                Name = employee_dto.Name
            };

            appDbContext.Employee_Table.Update( model );
            appDbContext.SaveChanges();


            Custom_Logger.Log("Employee Updated", "");

            return NoContent();
        }

    }

}
