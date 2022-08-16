using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lab5.Models.DataAccess
{
    public partial class Employee
    {
        public Employee()
        {
            Roles = new HashSet<Role>();
        }

        public int Id { get; set; }
        [DisplayName("Employee Name")]
        public string Name { get; set; } = null!;
        [DisplayName("Network ID")]
        [MinLength(3)]
        public string UserName { get; set; } = null!;
        [MinLength(5, ErrorMessage = "Password should containd atleast 5 characters")]
        public string Password { get; set; } = null!; 
        public virtual ICollection<Role> Roles { get; set; }
    }
}
