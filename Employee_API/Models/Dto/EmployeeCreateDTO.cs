using System.ComponentModel.DataAnnotations;

namespace Employee_API.Models.Dto
{
    public class EmployeeCreateDTO
    {
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        public EmployeeCreateDTO()
        {
            Name = string.Empty;
        }
    }
}
