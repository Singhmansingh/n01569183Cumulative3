using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using n01569183Cumulative3.Models;
namespace n01569183Cumulative3.Controllers
{
    public class ClassController : Controller
    {
        // GET: Class/List
        public ActionResult List(string SearchParam = null)
        {
            ClassDataController classController = new ClassDataController();
            IEnumerable<Class> Classes = classController.ListClasses(SearchParam);
            return View(Classes);
        }
        // GET: Class/List/{id}

        public ActionResult Show(int id)
        {
            ClassDataController classController = new ClassDataController();
            Class SelectedClass = classController.SelectClass(id);
            return View(SelectedClass);
        }

        public ActionResult Update(int id)
        {
            ClassDataController classController = new ClassDataController();
            Class SelectedClass = classController.SelectClass(id);
            return View(SelectedClass);
        }


        public ActionResult UpdateClass(int id, DateTime StartDate, DateTime FinishDate, int TeacherId, string ClassName, string ClassCode)
        {
            Class NewClass = new Class()
            {
                ClassId = id,
                ClassName = ClassName,
                ClassCode = ClassCode,
                StartDate = StartDate,
                FinishDate = FinishDate,
                TeacherId = TeacherId
            };
            ClassDataController controller = new ClassDataController();
            controller.UpdateClass(id,NewClass);
            return RedirectToAction("Show", new { id }); ;
        }
    }
}