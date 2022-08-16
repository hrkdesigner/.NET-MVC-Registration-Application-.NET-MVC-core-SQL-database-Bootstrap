using Lab5.Models.DataAccess;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab5.Models.DataAccess
{
    public class EmployeeRole
    {
        public int EmployeeId { get; set; }
        public int RoleId { get; set; } 
        public Employee Employee { get; set; } 
        public Role EmpRole { get; set; }
    }
}
