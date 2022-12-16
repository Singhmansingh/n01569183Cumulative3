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
    public class ClassDataController : ApiController
    {
        private SchoolDbContext School = new SchoolDbContext();

        /// <summary>
        /// Gets a list of all Classes
        /// </summary>
        /// <param name="SearchParam">(optional) String. Name of class or class code to search for</param>
        /// <example>
        /// GET: api/ClassData/ListClasses -> List of all classes
        /// GET: api/ClassData/ListClasses/HTTP -> List of all classes with a class code of HTTP
        /// GET: api/ClassData/ListClasses/Web -> List of all classes with a name that contains "Web"
        /// </example>
        /// <returns>List of type Class</returns>
        [HttpGet]
        [Route("api/ClassData/ListClasses/{SearchParam?}")]
        public IEnumerable<Class> ListClasses(string SearchParam = null)
        {
            MySqlConnection Conn = School.AccessDatabase();

            Conn.Open();
            string query = "SELECT * FROM Classes";
            if (SearchParam != null) query += " WHERE classcode LIKE @search OR classname LIKE @search";
            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@search","%" + SearchParam + "%");
            cmd.Prepare();

            MySqlDataReader ResultSet = cmd.ExecuteReader();

            List<Class> Classes = new List<Class>();

            while (ResultSet.Read())
            {
                Class _class = new Class()
                {
                    ClassId = Convert.ToInt32(ResultSet["classid"]),
                    ClassCode = ResultSet["classcode"].ToString(),
                    TeacherId = Convert.ToInt32(ResultSet["teacherid"]),
                    StartDate = Convert.ToDateTime(ResultSet["startdate"]),
                    FinishDate = Convert.ToDateTime(ResultSet["finishdate"]),
                    ClassName = ResultSet["classname"].ToString()
                };

                Classes.Add(_class);
            }
            Conn.Clone();
            return Classes;
        }

        /// <summary>
        /// Gets a specific Class by its ID
        /// </summary>
        /// <param name="id">Integer. ID of the Class</param>
        /// <returns>Class Object</returns>
        [HttpGet]
        [Route("api/ClassData/SelectClass/{id}")]
        public Class SelectClass(int id)
        {
            MySqlConnection Conn = School.AccessDatabase();

            Conn.Open();

            // Gets a list of all students enrolled in that class, and the teacher who teaches it
            string query = "SELECT classes.*, teachers.teacherid, teachers.employeenumber, teachers.teacherfname, teachers.teacherlname, students.studentid, students.studentnumber, students.studentfname, students.studentlname FROM Classes " +
                "LEFT JOIN teachers ON teachers.teacherid = classes.teacherid " +
                "LEFT JOIN studentsxclasses ON studentsxclasses.classid = classes.classid " +
                "LEFT JOIN students ON  students.studentid = studentsxclasses.studentid " +
                "WHERE classes.classid = @id";

            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            Class SelectedClass = new Class();
            List<Student> ClassStudents = new List<Student>();

            while (ResultSet.Read())
            {
                Class _class = new Class()
                {
                    ClassId = Convert.ToInt32(ResultSet["classid"]),
                    ClassCode = ResultSet["classcode"].ToString(),
                    TeacherId = Convert.ToInt32(ResultSet["teacherid"]),
                    StartDate = Convert.ToDateTime(ResultSet["startdate"]),
                    FinishDate = Convert.ToDateTime(ResultSet["finishdate"]),
                    ClassName = ResultSet["classname"].ToString(),
                    TeacherData = new Teacher()
                    {
                        Id = Convert.ToInt32(ResultSet["teacherid"]),
                        FName = ResultSet["teacherfname"].ToString(),
                        LName = ResultSet["teacherlname"].ToString(),
                        EmployeeNumber = ResultSet["employeenumber"].ToString(),
                    }
                };

                try
                {
                    Student _Student = new Student()
                    {
                        StudentId = Convert.ToInt32(ResultSet["studentid"]),
                        StudentNumber = ResultSet["studentnumber"].ToString(),
                        StudentFName = ResultSet["studentfname"].ToString(),
                        StudentLName = ResultSet["studentlname"].ToString(),
                    };
                    ClassStudents.Add(_Student);
                }
                catch
                {
                    Debug.WriteLine("No student found... Continuing");
                }

                SelectedClass = _class;
            }

            SelectedClass.ClassStudentList = ClassStudents;
            Conn.Clone();
            return SelectedClass;
        }

        /// <summary>
        /// Update a class
        /// </summary>
        /// <param name="id">Integer. Class to Update</param>
        /// <param name="UpdateClass">Class object</param>
        /// <returns>Integer. Affected Rows</returns>
        [HttpPost]
        [Route("api/ClassData/UpdateClass/{id}")]

        public int UpdateClass(int id, [FromBody] Class UpdateClass)
        {
            MySqlConnection Conn = School.AccessDatabase();

            Conn.Open();
            string query = "UPDATE classes SET classname = @classname, classcode = @classcode, teacherid = @teacherid, startdate = @startDate, finishdate = @finishDate WHERE classid = @id";
            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@classname", UpdateClass.ClassName);
            cmd.Parameters.AddWithValue("@classcode", UpdateClass.ClassCode);

            cmd.Parameters.AddWithValue("@teacherid", UpdateClass.TeacherId);
            cmd.Parameters.AddWithValue("@startDate", UpdateClass.StartDate);
            cmd.Parameters.AddWithValue("@finishDate", UpdateClass.FinishDate);
            cmd.Prepare();

            int AffectedRows = cmd.ExecuteNonQuery();

            return AffectedRows;
        }

        [HttpPost]
        [Route("api/ClassData/RemoveTeacher")]

        public int RemoveTeacher([FromBody] int TeacherId)
        {
            MySqlConnection Conn = School.AccessDatabase();

            Conn.Open();
            string query = "UPDATE classes SET teacherid = 0 WHERE teacherid = @teacherid";
            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@teacherid", TeacherId);
            cmd.Prepare();

            int AffectedRows = cmd.ExecuteNonQuery();

            return AffectedRows;
        }
    }
}
