using Microsoft.Ajax.Utilities;
using MySql.Data.MySqlClient;
using n01569183Cumulative3.Models;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace n01569183Cumulative3.Controllers
{
    public class StudentDataController : ApiController
    {
        private SchoolDbContext School = new SchoolDbContext();

        /// <summary>
        /// Gets a list of all Students
        /// </summary>
        /// <param name="SearchParam">(optional) String. Name of student to search</param>
        /// <example>
        /// GET: api/StudentData/ListStudents -> List of all students
        /// GET: api/StudentData/ListStudents/Joe -> List of all students with the name "Joe"
        /// </example>
        /// <returns>List of type Student</returns>
        [HttpGet]
        [Route("api/StudentData/ListStudents/{SearchParam?}")]
        public IEnumerable<Student> ListStudents(string SearchParam = null)
        {
            MySqlConnection Conn = School.AccessDatabase();

            Conn.Open();
            string query = "SELECT * FROM Students";
            if (SearchParam != null) query = "SELECT * FROM Students WHERE lower(Concat(studentfname,' ', studentlname)) LIKE @search";
            
            
            MySqlCommand cmd = Conn.CreateCommand();

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@search", "%" + SearchParam + "%");
            cmd.Prepare();

            MySqlDataReader ResultSet = cmd.ExecuteReader();

            List<Student> Students = new List<Student>();

            while (ResultSet.Read())
            {
                Student _Student = new Student()
                {
                    StudentId = Convert.ToInt32(ResultSet["studentid"]),
                    StudentFName = ResultSet["studentfname"].ToString(),
                    StudentLName = ResultSet["studentlname"].ToString(),
                    StudentNumber = ResultSet["studentnumber"].ToString(),
                    EnrolDate = Convert.ToDateTime(ResultSet["enroldate"]),
                };

                Students.Add(_Student);
            }
            Conn.Clone();
            return Students;
        }

        /// <summary>
        /// Gets a specific Student by its ID
        /// </summary>
        /// <param name="id">Integer. ID of the Student</param>
        /// <example>
        /// GET: api/StudentData/SelectStudent/5 -> Gets student with ID of 5
        /// </example>
        /// <returns>Student Object</returns>
        [HttpGet]
        [Route("api/StudentData/SelectStudent/{id}")]
        public Student SelectStudent(int id)
        {
            MySqlConnection Conn = School.AccessDatabase();

            Conn.Open();

            // gets all student data, and the classes that student is enrolled in
            string query = "SELECT students.*, classes.classid, classes.classcode, classes.classname FROM students " +
                "LEFT JOIN studentsxclasses ON studentsxclasses.studentid = students.studentid " +
                "LEFT JOIN classes ON  classes.classid = studentsxclasses.classid " +
                "WHERE students.studentid = @id";
            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            Student SelectedStudent = new Student();
            List<Class> StudentClasses = new List<Class>();

            while (ResultSet.Read())
            {
                Student _Student = new Student()
                {
                    StudentId = Convert.ToInt32(ResultSet["studentid"]),
                    StudentFName = ResultSet["studentfname"].ToString(),
                    StudentLName = ResultSet["studentlname"].ToString(),
                    StudentNumber = ResultSet["studentnumber"].ToString(),
                    EnrolDate = Convert.ToDateTime(ResultSet["enroldate"]),
                };

                try
                {
                    Class _class = new Class()
                    {
                        ClassId = Convert.ToInt32(ResultSet["classid"]),
                        ClassCode = ResultSet["classcode"].ToString(),
                        ClassName = ResultSet["classname"].ToString()
                    };
                    StudentClasses.Add(_class);
                }
                catch
                {
                    Debug.WriteLine("Student not enrolled in class.");
                }

                SelectedStudent = _Student;
            }
            SelectedStudent.StudentClassList = StudentClasses;
            Conn.Clone();
            return SelectedStudent;
        }
    }
}
