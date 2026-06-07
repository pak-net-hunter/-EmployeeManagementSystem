using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Microsoft.Data.Sqlite;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Database
{
    public static class Db
    {
        private static readonly string DbPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ems.db");

        private static string ConnStr => $"Data Source={DbPath}";

        public static SqliteConnection GetConnection() => new SqliteConnection(ConnStr);

        // ── Initialization ────────────────────────────────────────────────────
        public static void Initialize()
        {
            using var conn = GetConnection();
            conn.Open();

            Exec(conn, @"CREATE TABLE IF NOT EXISTS Users (
                Id       INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE,
                Password TEXT NOT NULL,
                Role     TEXT NOT NULL DEFAULT 'admin');");

            Exec(conn, @"CREATE TABLE IF NOT EXISTS Employees (
                Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                FullName    TEXT NOT NULL,
                Email       TEXT,
                Phone       TEXT,
                Department  TEXT,
                Designation TEXT,
                Salary      REAL DEFAULT 0,
                Gender      TEXT,
                JoiningDate TEXT,
                Status      TEXT DEFAULT 'Active',
                Address     TEXT,
                Notes       TEXT);");

            if (Scalar(conn, "SELECT COUNT(*) FROM Users") == 0)
                Exec(conn, "INSERT INTO Users (Username,Password,Role) VALUES ('admin','admin@123','admin')");

            if (Scalar(conn, "SELECT COUNT(*) FROM Employees") == 0)
                SeedEmployees(conn);
        }

        private static void SeedEmployees(SqliteConnection conn)
        {
            var data = new (string n, string e, string ph, string d, string des, double s, string g, string j, string st, string a)[]
            {
                ("Ali Khan",       "ali.khan@company.com",    "03001234567", "Information Technology", "Software Engineer",    125000, "Male",   "2023-01-15", "Active",   "Lahore, Pakistan"),
                ("Sara Ahmed",     "sara.ahmed@company.com",  "03007654321", "Human Resources",        "HR Manager",           155000, "Female", "2022-06-10", "Active",   "Karachi, Pakistan"),
                ("Bilal Hussain",  "bilal.h@company.com",     "03331122334", "Finance",                "Senior Accountant",    110000, "Male",   "2023-03-20", "Active",   "Islamabad, Pakistan"),
                ("Ayesha Malik",   "ayesha.m@company.com",    "03219988776", "Marketing",              "Marketing Lead",       120000, "Female", "2021-11-05", "On Leave", "Multan, Pakistan"),
                ("Usman Tariq",    "usman.t@company.com",     "03445566778", "Information Technology", "DevOps Engineer",      140000, "Male",   "2024-02-01", "Active",   "Faisalabad, Pakistan"),
                ("Hina Raza",      "hina.r@company.com",      "03126677889", "Sales",                  "Sales Executive",       85000, "Female", "2023-08-12", "Inactive", "Peshawar, Pakistan"),
                ("Kamran Shah",    "kamran.s@company.com",    "03334455667", "Operations",             "Operations Manager",   180000, "Male",   "2020-04-01", "Active",   "Quetta, Pakistan"),
                ("Zara Farooq",    "zara.f@company.com",      "03218899001", "Finance",                "Financial Analyst",    115000, "Female", "2022-09-15", "Active",   "Lahore, Pakistan"),
                ("Hassan Rauf",    "hassan.r@company.com",    "03009988776", "Information Technology", "Backend Developer",    130000, "Male",   "2023-05-10", "Active",   "Karachi, Pakistan"),
                ("Nadia Islam",    "nadia.i@company.com",     "03117766554", "Human Resources",        "Recruiter",             90000, "Female", "2024-01-08", "Active",   "Multan, Pakistan"),
                ("Farhan Qureshi", "farhan.q@company.com",    "03456789012", "Sales",                  "Sales Manager",        145000, "Male",   "2021-07-20", "Active",   "Lahore, Pakistan"),
                ("Maham Saeed",    "maham.s@company.com",     "03345678901", "Marketing",              "Digital Marketer",      95000, "Female", "2023-11-01", "Active",   "Karachi, Pakistan"),
            };

            foreach (var r in data)
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"INSERT INTO Employees
                    (FullName,Email,Phone,Department,Designation,Salary,Gender,JoiningDate,Status,Address,Notes)
                    VALUES (@n,@e,@p,@d,@des,@s,@g,@j,@st,@a,@no)";
                cmd.Parameters.AddWithValue("@n",   r.n);
                cmd.Parameters.AddWithValue("@e",   r.e);
                cmd.Parameters.AddWithValue("@p",   r.ph);
                cmd.Parameters.AddWithValue("@d",   r.d);
                cmd.Parameters.AddWithValue("@des", r.des);
                cmd.Parameters.AddWithValue("@s",   r.s);
                cmd.Parameters.AddWithValue("@g",   r.g);
                cmd.Parameters.AddWithValue("@j",   r.j);
                cmd.Parameters.AddWithValue("@st",  r.st);
                cmd.Parameters.AddWithValue("@a",   r.a);
                cmd.Parameters.AddWithValue("@no",  "");
                cmd.ExecuteNonQuery();
            }
        }

        // ── Authentication ─────────────────────────────────────────────────────
        public static bool ValidateUser(string username, string password)
        {
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE Username=@u AND Password=@p";
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", password);
            return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
        }

        public static bool ChangePassword(string username, string oldPass, string newPass)
        {
            if (!ValidateUser(username, oldPass)) return false;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Users SET Password=@np WHERE Username=@u";
            cmd.Parameters.AddWithValue("@u",  username);
            cmd.Parameters.AddWithValue("@np", newPass);
            cmd.ExecuteNonQuery();
            return true;
        }

        // ── Employee CRUD ──────────────────────────────────────────────────────
        public static List<Employee> GetEmployees(string search = "", string dept = "")
        {
            var list = new List<Employee>();
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            var where = "WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(search))
            {
                where += " AND (FullName LIKE @s OR Email LIKE @s OR Phone LIKE @s OR Designation LIKE @s)";
                cmd.Parameters.AddWithValue("@s", "%" + search.Trim() + "%");
            }
            if (!string.IsNullOrWhiteSpace(dept))
            {
                where += " AND Department=@d";
                cmd.Parameters.AddWithValue("@d", dept);
            }
            cmd.CommandText = $"SELECT * FROM Employees {where} ORDER BY FullName";
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(Map(r));
            return list;
        }

        public static void AddEmployee(Employee e)
        {
            using var conn = GetConnection(); conn.Open();
            Insert(conn, e);
        }

        public static void UpdateEmployee(Employee e)
        {
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Employees SET
                FullName=@n,Email=@e,Phone=@p,Department=@d,Designation=@des,
                Salary=@s,Gender=@g,JoiningDate=@j,Status=@st,Address=@a,Notes=@no
                WHERE Id=@id";
            Bind(cmd, e);
            cmd.Parameters.AddWithValue("@id", e.Id);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteEmployee(int id)
        {
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Employees WHERE Id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        // ── Dashboard Stats ────────────────────────────────────────────────────
        public static int     GetTotalEmployees()   => (int)Q("SELECT COUNT(*) FROM Employees");
        public static int     GetActiveEmployees()  => (int)Q("SELECT COUNT(*) FROM Employees WHERE Status='Active'");
        public static int     GetTotalDepts()       => (int)Q("SELECT COUNT(DISTINCT Department) FROM Employees WHERE Department<>''");
        public static decimal GetTotalSalary()
        {
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT IFNULL(SUM(Salary),0) FROM Employees";
            var o = cmd.ExecuteScalar();
            return (o == null || o == DBNull.Value) ? 0 : Convert.ToDecimal(o);
        }

        public static Dictionary<string, int> GetByDepartment()
        {
            var d = new Dictionary<string, int>();
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Department, COUNT(*) c FROM Employees
                WHERE Department<>'' GROUP BY Department ORDER BY c DESC";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                d[r["Department"]?.ToString() ?? "(none)"] = Convert.ToInt32(r["c"]);
            return d;
        }

        public static List<string> GetDepartments()
        {
            var list = new List<string>();
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT DISTINCT Department FROM Employees WHERE Department<>'' ORDER BY Department";
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(r.GetString(0));
            return list;
        }

        // ── Reports ────────────────────────────────────────────────────────────
        public static DataTable GetDepartmentSummary()
        {
            var dt = new DataTable();
            dt.Columns.Add("Department");
            dt.Columns.Add("Total",              typeof(int));
            dt.Columns.Add("Active",             typeof(int));
            dt.Columns.Add("Inactive",           typeof(int));
            dt.Columns.Add("On Leave",           typeof(int));
            dt.Columns.Add("Avg Salary (PKR)",   typeof(decimal));
            dt.Columns.Add("Total Salary (PKR)", typeof(decimal));

            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Department,
                COUNT(*) Total,
                SUM(CASE WHEN Status='Active'   THEN 1 ELSE 0 END) Act,
                SUM(CASE WHEN Status='Inactive' THEN 1 ELSE 0 END) Inact,
                SUM(CASE WHEN Status='On Leave' THEN 1 ELSE 0 END) Lv,
                ROUND(AVG(Salary),0) AvgS,
                ROUND(SUM(Salary),0) TotS
                FROM Employees WHERE Department<>''
                GROUP BY Department ORDER BY Total DESC";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                dt.Rows.Add(r["Department"],
                    Convert.ToInt32(r["Total"]),  Convert.ToInt32(r["Act"]),
                    Convert.ToInt32(r["Inact"]),  Convert.ToInt32(r["Lv"]),
                    Convert.ToDecimal(r["AvgS"]), Convert.ToDecimal(r["TotS"]));
            return dt;
        }

        // ── Private helpers ────────────────────────────────────────────────────
        private static Employee Map(SqliteDataReader r) => new Employee
        {
            Id          = Convert.ToInt32(r["Id"]),
            FullName    = r["FullName"]?.ToString()    ?? "",
            Email       = r["Email"]?.ToString()       ?? "",
            Phone       = r["Phone"]?.ToString()       ?? "",
            Department  = r["Department"]?.ToString()  ?? "",
            Designation = r["Designation"]?.ToString() ?? "",
            Salary      = r["Salary"] == DBNull.Value ? 0 : Convert.ToDecimal(r["Salary"]),
            Gender      = r["Gender"]?.ToString()      ?? "",
            JoiningDate = r["JoiningDate"]?.ToString() ?? "",
            Status      = r["Status"]?.ToString()      ?? "Active",
            Address     = r["Address"]?.ToString()     ?? "",
            Notes       = r["Notes"]?.ToString()       ?? "",
        };

        private static void Insert(SqliteConnection conn, Employee e)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Employees
                (FullName,Email,Phone,Department,Designation,Salary,Gender,JoiningDate,Status,Address,Notes)
                VALUES (@n,@e,@p,@d,@des,@s,@g,@j,@st,@a,@no)";
            Bind(cmd, e);
            cmd.ExecuteNonQuery();
        }

        private static void Bind(SqliteCommand cmd, Employee e)
        {
            cmd.Parameters.AddWithValue("@n",   e.FullName    ?? "");
            cmd.Parameters.AddWithValue("@e",   e.Email       ?? "");
            cmd.Parameters.AddWithValue("@p",   e.Phone       ?? "");
            cmd.Parameters.AddWithValue("@d",   e.Department  ?? "");
            cmd.Parameters.AddWithValue("@des", e.Designation ?? "");
            cmd.Parameters.AddWithValue("@s",   (double)e.Salary);
            cmd.Parameters.AddWithValue("@g",   e.Gender      ?? "");
            cmd.Parameters.AddWithValue("@j",   e.JoiningDate ?? "");
            cmd.Parameters.AddWithValue("@st",  e.Status      ?? "Active");
            cmd.Parameters.AddWithValue("@a",   e.Address     ?? "");
            cmd.Parameters.AddWithValue("@no",  e.Notes       ?? "");
        }

        private static void Exec(SqliteConnection conn, string sql)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        private static long Scalar(SqliteConnection conn, string sql)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            return Convert.ToInt64(cmd.ExecuteScalar());
        }

        private static long Q(string sql)          // query scalar with own connection
        {
            using var conn = GetConnection(); conn.Open();
            return Scalar(conn, sql);
        }
    }
}
