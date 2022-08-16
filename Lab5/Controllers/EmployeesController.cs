using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab5.Models.DataAccess;
using Lab5.Models;

namespace Lab5.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly StudentRecordContext _context; 
        public EmployeesController(StudentRecordContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var empList = await _context.Employees.Include(x => x.Roles).ToListAsync();
              return _context.Employees != null ? 
                          View(empList) :
                          Problem("Entity set 'StudentRecordContext.Employees'  is null.");
        }
         

        // GET: Employees/Create
        public IActionResult Create()
        {  
            return View(new Employee());
        }

        public IActionResult Edit(int id)
        {
            var emp = _context.Employees.Include(x => x.Roles).Where(x => x.Id == id).FirstOrDefault(); 
            return View(emp);
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee, string[] chkRole)
        {
            ModelState.Remove("Id"); 
            ModelState.Remove("Roles"); 
            if (!ModelState.IsValid)
            {
                if (chkRole.Length == 0)
                {
                    ViewBag.ChkRoleError = "You must select atleast one role!";
                    return View(employee);
                }
                return View(employee);
            }
            if(chkRole.Length == 0)
            {
                ViewBag.ChkRoleError = "You must select atleast one role!";
                return View(employee);
            }
            if (ModelState.IsValid)
            {
                employee.Roles = new List<Role>();
                foreach (var item in chkRole)
                {
                    employee.Roles.Add(new Role() { Id = Convert.ToInt16(item), Role1 = _context.Roles.Where(x=>x.Id == Convert.ToInt32(item)).AsNoTracking().FirstOrDefault().Role1 });  
                }
                _context.Employees.Attach(employee).State = EntityState.Added;
                _context.SaveChanges(); 
                
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Employee employee, string[] chkRole)
        {
            ModelState.Remove("Roles");
            if (!ModelState.IsValid)
            {
                if (chkRole.Length == 0)
                {
                    ViewBag.ChkRoleError = "You must select atleast one role!";
                    return View(employee);
                }
                return View(employee);
            }
            if (chkRole.Length == 0)
            {
                ViewBag.ChkRoleError = "You must select atleast one role!";
                return View(employee);
            }
            if (ModelState.IsValid)
            { 
                var OldEmployee = _context.Employees.Include(x => x.Roles).AsNoTracking().Where(x => x.Id == employee.Id).FirstOrDefault();
                OldEmployee.UserName = employee.UserName;
                OldEmployee.Name = employee.Name;
                OldEmployee.Password = employee.Password;
                // Delete existing roles for employee before adding new roles
                string sqlQuery = "DELETE FROM Employee_Role WHERE Employee_Id = " + OldEmployee.Id;
                _context.Database.ExecuteSqlRaw(sqlQuery);
                foreach (var query in chkRole) // Loop through all the selected roles
                {
                    // Add role for employee 
                    string sqlQuery1 = "INSERT INTO Employee_Role VALUES(" + OldEmployee.Id + ", " + query + ")";
                        _context.Database.ExecuteSqlRaw(sqlQuery1);
                  
                }  
                _context.Entry(OldEmployee).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        } 
          
    }
}
