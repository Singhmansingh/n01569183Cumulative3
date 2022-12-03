using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace n01569183Cumulative2.Models
{
    public class Class
    {
        public int ClassId;
        public string ClassCode;
        public int TeacherId;
        public Teacher TeacherData;
        public DateTime StartDate;
        public DateTime FinishDate;
        public string ClassName;
        public List<Student> ClassStudentList;
    }
}