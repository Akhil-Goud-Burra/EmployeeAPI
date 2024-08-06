using System.ComponentModel.DataAnnotations;

namespace Employee_API.Models.Dto
{
    public class EmployeeUpdateDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        public EmployeeUpdateDTO()
        {
            Name = string.Empty;
        }
    }
}
