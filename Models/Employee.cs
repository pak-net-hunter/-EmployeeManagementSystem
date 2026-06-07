namespace EmployeeManagementSystem.Models
{
    public class Employee
    {
        public int     Id          { get; set; }
        public string  FullName    { get; set; } = "";
        public string  Email       { get; set; } = "";
        public string  Phone       { get; set; } = "";
        public string  Department  { get; set; } = "";
        public string  Designation { get; set; } = "";
        public decimal Salary      { get; set; }
        public string  Gender      { get; set; } = "";
        public string  JoiningDate { get; set; } = "";
        public string  Status      { get; set; } = "Active";
        public string  Address     { get; set; } = "";
        public string  Notes       { get; set; } = "";
    }
}
