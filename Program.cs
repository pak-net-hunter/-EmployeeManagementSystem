using System;
using System.Windows.Forms;
using EmployeeManagementSystem.Database;
using EmployeeManagementSystem.Forms;

namespace EmployeeManagementSystem
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Db.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Database initialize karne mein masla:\n\n" + ex.Message +
                    "\n\nApplication folder mein write permission check karein.",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.Run(new LoginForm());
        }
    }
}
