using System.ComponentModel.DataAnnotations;

namespace Employee_API.Models.Dto
{
    public class EmployeeGetDTO
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; }

        public EmployeeGetDTO()
        {
            Name = string.Empty;
        }
    }
}
