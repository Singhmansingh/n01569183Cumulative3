using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace n01569183Cumulative3.Models
{
    public class Teacher
    {
        public int Id;
        public string FName;
        public string LName;
        public string EmployeeNumber;
        public DateTime HireDate;
        public decimal Salary;
        public List<Class> classList;
    }
}