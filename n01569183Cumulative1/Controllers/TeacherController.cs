using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using n01569183Cumulative2.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace n01569183Cumulative2.Controllers
{
    public class TeacherController : Controller
    {

        // GET: Teacher/List?{SearchParam?}&{SalaryParam?}&{SalaryOperator?}&{HireParam?}&{HireOperator?}
        public ActionResult List(string SearchParam = null, decimal SalaryParam = -1, string SalaryOperator = "Equal", string HireParam = null, string HireOperator = "Equal")
        {
            TeacherDataController controller = new TeacherDataController();
            IEnumerable<Teacher> teacherList = controller.ListTeachers(SearchParam, SalaryParam, SalaryOperator, HireParam, HireOperator);
            return View(teacherList);
        }

        // GET: Teacher/Show/{id}
        public ActionResult Show(int id)
        {
            TeacherDataController teacherDatacontroller = new TeacherDataController();
            Teacher teacher = teacherDatacontroller.SelectTeacher(id);

            return View(teacher);
        }

        public ActionResult New()
        {
            return View();
        }

        // POST: Teacher/Create
        [HttpPost]
        public ActionResult Create(string TeacherFName, string TeacherLName, string EmployeeNumber, DateTime HireDate, string SalaryStr = "0")
        {
            decimal Salary = Convert.ToDecimal(SalaryStr);

            Teacher NewTeacher = new Teacher()
            {
                FName = TeacherFName,
                LName = TeacherLName,
                EmployeeNumber = EmployeeNumber,
                HireDate = HireDate,
                Salary = Salary
            };

            TeacherDataController controller = new TeacherDataController();
            int TeacherId = controller.AddTeacher(NewTeacher);
            Debug.WriteLine(TeacherId);
            if (TeacherId > 0) return RedirectToAction("List");
            return View();
        }

        public ActionResult DeleteConfirm(int id)
        {
            TeacherDataController controller = new TeacherDataController();

            Teacher teacher = controller.SelectTeacher(id);
            
            return View(teacher);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            TeacherDataController controller = new TeacherDataController();

            controller.DeleteTeacher(id);

            return RedirectToAction("List");
        }

    }
}