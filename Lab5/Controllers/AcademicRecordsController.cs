using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab5.Models.DataAccess;
using Microsoft.AspNetCore.Http;

namespace Lab5.Controllers
{
    public class AcademicRecordsController : Controller
    {
        private readonly StudentRecordContext _context;
        private List<AcademicRecord> _AcList;
        private List<AcademicRecord> AcList
        {
            get
            {
                if (_AcList == null)
                    _AcList = new List<AcademicRecord>();
                return _AcList;
            }
            set
            {
                _AcList = value;
            }
        }
        public AcademicRecordsController(StudentRecordContext context)
        {
            _context = context;
        }

      
        // sort in Index
        public async Task<IActionResult> Index(string sortOrder = "")
        {                        
            var courseSort = String.IsNullOrEmpty(sortOrder) ? "course_desc" : "";
            var studentSort = sortOrder == "Student" ? "student_desc" : "Student";
            HttpContext.Session.SetString("CourseSortParm", courseSort);
            HttpContext.Session.SetString("StudentSortParm", studentSort);
            HttpContext.Session.SetString("SortOrder", sortOrder);
            var Ac = _context.AcademicRecords.Include(a => a.CourseCodeNavigation).Include(a => a.Student).OrderBy(x=>x.CourseCodeNavigation.Title);
              
            switch (sortOrder)
            {
                case "course_desc":
                    Ac = Ac.OrderByDescending(s => s.CourseCodeNavigation.Title);
                    break;
                case "Student":
                    Ac = Ac.ThenBy(s => s.Student.Name);
                    break;

                case "student_desc":
                    Ac = Ac.ThenByDescending(s => s.Student.Name);
                    break;
                default:
                    Ac = Ac.ThenBy(s => s.CourseCodeNavigation.Title);
                    break;
            }
            return View(Ac.ToList());
        }

        public async Task<IActionResult> EditAll(string sortOrder= "")
        {
            var courseSort = String.IsNullOrEmpty(sortOrder) ? "course_desc" : "";
            var studentSort = sortOrder == "Student" ? "student_desc" : "Student";
            HttpContext.Session.SetString("CourseSortParm", courseSort);
            HttpContext.Session.SetString("StudentSortParm", studentSort);
            HttpContext.Session.SetString("SortOrder", sortOrder);
            var Ac = _context.AcademicRecords.Include(a => a.CourseCodeNavigation).Include(a => a.Student).OrderBy(x => x.CourseCodeNavigation.Title);

            switch (sortOrder)
            {
                case "course_desc":
                    Ac = Ac.OrderByDescending(s => s.CourseCodeNavigation.Title);
                    break;
                case "Student":
                    Ac = Ac.ThenBy(s => s.Student.Name);
                    break;

                case "student_desc":
                    Ac = Ac.ThenByDescending(s => s.Student.Name);
                    break;
                default:
                    Ac = Ac.ThenBy(s => s.CourseCodeNavigation.Title);
                    break;
            }
            return View(Ac.ToList());
        }
        [HttpPost]
        public async Task<IActionResult> EditAll(List<AcademicRecord> acList)
        {
            try
            {
                for (int i = 0; i < acList.Count(); i++)
                {
                    // Remove validation errors for CourseCodeNavigation / Student
                    // Keys in model state for list are placed in format like this:
                    // [0].CourseCodeNavigation / [0].Student
                    // [1].CourseCodeNavigation / [1].Student
                    ModelState.Remove("[" + i + "]" + ".CourseCodeNavigation");
                    ModelState.Remove("[" + i + "]" + ".Student");

                    if(acList[i].Grade == null)
                        ModelState.AddModelError("[" + i + "]" + ".Grade", "The value " + acList[i].Grade + "is not valid for Grade."); ;
                }
                if (!ModelState.IsValid)
                {
                    foreach (var item in acList)
                    {
                        item.CourseCodeNavigation = _context.Courses.Where(x => x.Code == item.CourseCode).FirstOrDefault();
                        item.Student = _context.Students.Where(x => x.Id == item.StudentId).FirstOrDefault();
                    }
                    return View(acList);
                }
                // If system comes here, that means the modelstate is valid. lets perform updation
                foreach (var item in acList)
                { 
                    _context.Update(item);
                    await _context.SaveChangesAsync(); 
                }
                
            }
            catch(Exception ex)
            {

            }
            foreach(var item in acList)
            {
                item.CourseCodeNavigation = _context.Courses.Where(x => x.Code == item.CourseCode).FirstOrDefault();
                item.Student = _context.Students.Where(x => x.Id == item.StudentId).FirstOrDefault();
            }
            return View(acList);
        }



        // GET: AcademicRecords/Details/5


        // GET: AcademicRecords/Create
        public IActionResult Create()
        {
            ViewData["CourseCode"] = new SelectList(_context.Courses, "Code", "Code");
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id");
            return View();
        }

        // POST: AcademicRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseCode,StudentId,Grade")] AcademicRecord academicRecord)
        {
            // Remove validation error for below properties as we don't need errors for them
            ModelState.Remove("CourseCodeNavigation");
            ModelState.Remove("Student");
            if (ModelState.IsValid)
            {
                _context.Add(academicRecord); // Save record in the database
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseCode"] = new SelectList(_context.Courses, "Code", "Code", academicRecord.CourseCode);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id", academicRecord.StudentId);
            return View(academicRecord);
        }

        // GET: AcademicRecords/Edit/5
        public async Task<IActionResult> Edit(string StudentId, string CourseCode)
        {
            if (string.IsNullOrEmpty(StudentId) || string.IsNullOrEmpty(CourseCode) || _context.AcademicRecords == null)
            {
                return NotFound();
            }

            var academicRecord = _context.AcademicRecords
                .Include(x=>x.CourseCodeNavigation).Include(x=>x.Student)
                .Where(x => x.StudentId == StudentId && x.CourseCode == CourseCode).FirstOrDefault();
            if (academicRecord == null)
            {
                return NotFound();
            }
            ViewData["CourseCodeTitle"] = academicRecord.CourseCode + "-" + academicRecord.CourseCodeNavigation.Title;
            ViewData["StudentIdName"] = academicRecord.StudentId + "-" + academicRecord.Student.Name;
            return View(academicRecord);
        }

        // POST: AcademicRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("CourseCode,StudentId,Grade")] AcademicRecord academicRecord)
        {
            ModelState.Remove("CourseCodeNavigation");
            ModelState.Remove("Student");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(academicRecord);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    
                }
            }
            ViewData["CourseCodeTitle"] = academicRecord.CourseCode + "-" + academicRecord.CourseCodeNavigation.Title;
            ViewData["StudentIdName"] = academicRecord.StudentId + "-" + academicRecord.Student.Name;
            return View(academicRecord);
        }
         



    }
}
