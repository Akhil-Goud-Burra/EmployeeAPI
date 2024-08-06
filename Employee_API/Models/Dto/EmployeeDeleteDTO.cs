using System.ComponentModel.DataAnnotations;

namespace Employee_API.Models.Dto
{
    public class EmployeeDeleteDTO
    {
        [Required]
        public int Id { get; set; }
    }
}
