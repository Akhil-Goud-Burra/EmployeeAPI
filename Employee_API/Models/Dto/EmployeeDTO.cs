using System.ComponentModel.DataAnnotations;

namespace Employee_API.Models.Dto
{
    public class EmployeeDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        public EmployeeDTO()
        {
            Name = string.Empty;
        }
    }
}
