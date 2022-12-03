using Microsoft.Ajax.Utilities;
using MySql.Data.MySqlClient;
using n01569183Cumulative2.Models;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq.Expressions;

namespace n01569183Cumulative2.Controllers
{
    public class TeacherDataController : ApiController
    {
        private SchoolDbContext School = new SchoolDbContext();


        /// <summary>
        /// Generates a list of all teachers in the Database
        /// </summary>
        /// <param name="SearchParam">(optional) String. Name of the teacher</param>
        /// <param name="SalaryParam">(optional) Decimal. Dolar value to search.</param>
        /// <param name="SalaryOperator">(optional) String. Comparator operator for Salary (Before, Equal, After). Default Equal.</param>
        /// <param name="HireParam">(optional) String. Date of hire. formatted as YYYY-MM-DD</param>
        /// <param name="HireOperator">(optional) String. Comparator operator for Hire Date (Before, Equal, After). Default Equal.</param>
        /// <example>
        /// GET: api/Teacherdata/ListTeachers -> List of all Teachers
        /// GET: api/Teacherdata/ListTeachers/Joe -> List of all Teachers with name "Joe"
        /// GET: api/Teacherdata/ListTeachers/Joe/50.22/Equal -> List of all Teachers with name "Joe", and a salary equal to 50.22
        /// GET: api/Teacherdata/ListTeachers/Joe/50.22/Equal/2015-10-05/Less -> List of all Teachers with name "Joe", a salary equal to 50.22, and hired before 2015-10-05
        /// </example>
        /// <returns>List of type Teacher</returns>
        [HttpGet]
        [Route("api/TeacherData/ListTeachers/{SearchParam?}/{SalaryParam?}/{SalaryOperator?}/{HireParam?}/{HireOperator?}")]

        public IEnumerable<Teacher> ListTeachers(string SearchParam = null, decimal SalaryParam = -1, string SalaryOperator = "Equal",  string HireParam = null, string HireOperator = "Equal" )
        {
            MySqlConnection Conn = School.AccessDatabase();

            Conn.Open();
            string query = "SELECT * FROM Teachers";

            // If a search parameter was included
            if (SearchParam != null || SalaryParam != -1 || HireParam != null)
            {
                // search for name 
                query += " WHERE lower(CONCAT(teacherfname, ' ', teacherlname)) LIKE @search";

                // search for salary
                if (SalaryParam > -1) query += $" AND salary {GetOpperator(SalaryOperator)} @salary";
                
                // search for hire date
                if (HireParam != "") query += $" AND hiredate {GetOpperator(HireOperator)} @hiredate";
            }
            MySqlCommand cmd = Conn.CreateCommand();
            Debug.WriteLine(query);
            cmd.CommandText = query;

            cmd.Parameters.AddWithValue("@search", "%" + SearchParam + "%");
            cmd.Parameters.AddWithValue("@salary", SalaryParam);
            cmd.Parameters.AddWithValue("@hiredate", HireParam);

            cmd.Prepare();

            MySqlDataReader ResultSet = cmd.ExecuteReader();

            List<Teacher> Teachers = new List<Teacher>();

            while (ResultSet.Read())
            {
                Teacher _teacher = new Teacher()
                {
                    Id = Convert.ToInt32(ResultSet["teacherid"]),
                    FName = ResultSet["teacherfname"].ToString(),
                    LName = ResultSet["teacherlname"].ToString(),
                    EmployeeNumber = ResultSet["employeenumber"].ToString(),
                    HireDate = Convert.ToDateTime(ResultSet["hiredate"]),
                    Salary = Convert.ToDecimal(ResultSet["salary"])
                };
                Teachers.Add(_teacher);
            }
            Conn.Clone();
            return Teachers;
        }

        /// <summary>
        /// Select a Teacher by their ID in the Database
        /// </summary>
        /// <param name="id">Integer ID of the Teacher</param>
        /// <example>
        /// GET: api/TeacherData/SelectTeacher/5 -> Teacher at ID of 5, and all their classes
        /// </example>
        /// <returns>Teacher object</returns>
        [HttpGet]
        [Route("api/TeacherData/SelectTeacher/{id}")]
        public Teacher SelectTeacher(int id)
        {
            // get all teacher data, and join classes, to get a list of all classes that teacher teaches
            string query = "SELECT * FROM Teachers LEFT JOIN classes ON classes.teacherid = teachers.teacherid  WHERE teachers.teacherid = @id ";

            MySqlConnection Conn = School.AccessDatabase();

            Conn.Open();
            MySqlCommand cmd = Conn.CreateCommand();

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            Teacher SelectedTeacher = new Teacher();

            // List of Classes teacher teaches
            List<Class> teacherClasses = new List<Class>();

            while (ResultSet.Read())
            {
                Teacher _teacher = new Teacher()
                {
                    Id = Convert.ToInt32(ResultSet["teacherid"]),
                    FName = ResultSet["teacherfname"].ToString(),
                    LName = ResultSet["teacherlname"].ToString(),
                    EmployeeNumber = ResultSet["employeenumber"].ToString(),
                    HireDate = Convert.ToDateTime(ResultSet["hiredate"]),
                    Salary = Convert.ToDecimal(ResultSet["salary"])
                };

                if (!Convert.IsDBNull(ResultSet["classid"]))
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

                    teacherClasses.Add(_class);

                }



                SelectedTeacher = _teacher;
            }
            SelectedTeacher.classList = teacherClasses;
            return SelectedTeacher;

        }


        [HttpPost]
        [Route("api/TeacherData/AddTeacher")]
        public int AddTeacher([FromBody] Teacher teacher)
        {
            MySqlConnection Conn = School.AccessDatabase();
            Conn.Open();

            MySqlCommand cmd = Conn.CreateCommand();

            string query = "INSERT INTO teachers(teacherfname,teacherlname,employeenumber,hiredate,salary) VALUES (@fname,@lname,@eid,@hiredate,@salary);";
            cmd.CommandText = query;

            cmd.Parameters.AddWithValue("@fname", teacher.FName);
            cmd.Parameters.AddWithValue("@lname", teacher.LName);
            cmd.Parameters.AddWithValue("@eid", teacher.EmployeeNumber);
            cmd.Parameters.AddWithValue("@hiredate", teacher.HireDate);
            cmd.Parameters.AddWithValue("@salary", teacher.Salary);

            cmd.Prepare();

            int Res = cmd.ExecuteNonQuery();
            if(Res > 0)
            {
                cmd.CommandText = "SELECT MAX(teacherid) AS teacherid FROM Teachers";
                MySqlDataReader RowData = cmd.ExecuteReader();
                int NewId = 0;
                while (RowData.Read())
                {
                    NewId = Convert.ToInt32(RowData["teacherid"]);
                }
                Conn.Close();
                return NewId;
            }
        
            Conn.Close();
            return Res;
        }

        [HttpPost]
        [Route("api/TeacherData/DeleteTeacher")]
        public void DeleteTeacher([FromBody]int TeacherId)
        {
            MySqlConnection Conn = School.AccessDatabase();
            Conn.Open();

            MySqlCommand cmd = Conn.CreateCommand();

            string query = "DELETE FROM teachers WHERE teacherid = @id";

            cmd.CommandText = query;

            cmd.Parameters.AddWithValue("@id", TeacherId);

            cmd.ExecuteNonQuery();

            ClassDataController classDataController = new ClassDataController();
            classDataController.RemoveTeacher(TeacherId);

        }


        // Converts the string into a symbol
        private string GetOpperator(string param)
        {
            string expression = "=";
            switch (param)
            {
                case "Less":
                    expression = "<=";
                    break;
                case "Greater":
                    expression = ">=";
                    break;
            }

            return expression;
        }
    }
}
