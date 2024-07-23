using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Employee_API.Models
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // Unique identifier for the employee
        public string Name { get; set; }  // Employee's name
        public DateTime HireDate { get; set; }

        public Employee()
        {
            Name = string.Empty; // To make the name to null.
        }

    }
}
