using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace n01569183Cumulative3.Models
{
    public class Student
    {
        public int StudentId;
        public string StudentFName;
        public string StudentLName;
        public string StudentNumber;
        public DateTime EnrolDate;
        public List<Class> StudentClassList;

    }
}