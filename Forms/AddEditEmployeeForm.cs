using System;
using System.Drawing;
using System.Windows.Forms;
using EmployeeManagementSystem.Database;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Forms
{
    public class AddEditEmployeeForm : Form
    {
        private readonly Employee _original;
        private readonly bool     _isEdit;

        private TextBox      _txtName, _txtEmail, _txtPhone, _txtDesig, _txtSalary, _txtAddress, _txtNotes;
        private ComboBox     _cmbDept, _cmbGender, _cmbStatus;
        private DateTimePicker _dtp;

        public AddEditEmployeeForm(Employee emp)
        {
            _original = emp;
            _isEdit   = emp != null;

            Text            = _isEdit ? "Edit Employee" : "Add New Employee";
            Size            = new Size(720, 590);
            MinimumSize     = Size;
            MaximumSize     = Size;
            StartPosition   = FormStartPosition.CenterParent;
            BackColor       = AppTheme.Background;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;

            BuildUi();
            if (_isEdit) Populate(emp);
        }

        private void BuildUi()
        {
            // Header
            var hdr = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = _isEdit ? AppTheme.Sidebar : AppTheme.Accent };
            var lblH = new Label
            {
                Text = _isEdit ? "  ✏   Edit Employee Record" : "  +   Add New Employee",
                Dock = DockStyle.Fill, AutoSize = false,
                ForeColor = Color.White, Font = AppTheme.FontLarge,
                TextAlign = ContentAlignment.MiddleLeft
            };
            hdr.Controls.Add(lblH);

            // Bottom buttons
            var btnBar = new Panel { Dock = DockStyle.Bottom, Height = 54, BackColor = AppTheme.Card };
            btnBar.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(AppTheme.Border), 0, 0, btnBar.Width, 0);

            var btnSave = UiHelper.MakeButton(_isEdit ? "  Update Record" : "  Save Employee", AppTheme.Accent, 155, 36);
            var btnCancel = UiHelper.MakeButton("Cancel", AppTheme.Sidebar, 100, 36);
            btnSave.Location   = new Point(26, 9);
            btnCancel.Location = new Point(191, 9);
            btnSave.Click   += BtnSave_Click;
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            btnBar.Controls.Add(btnSave);
            btnBar.Controls.Add(btnCancel);

            // Body card
            var body = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Card, Padding = new Padding(26, 16, 26, 8) };

            var tbl = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                ColumnCount = 4,
                RowCount    = 7,
                BackColor   = Color.Transparent,
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 108));   // label col 1
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,  50));    // field col 1
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 108));   // label col 2
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,  50));    // field col 2
            for (int i = 0; i < 7; i++) tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));

            // Row 0: Full Name | Department
            AddRow(tbl, 0, "Full Name *", out _txtName,  "Department",    out _cmbDept);
            // Row 1: Email | Designation
            AddRowTT(tbl, 1, "Email",     out _txtEmail, "Designation",   out _txtDesig);
            // Row 2: Phone | Salary
            AddRowTT(tbl, 2, "Phone",     out _txtPhone, "Salary (PKR)",  out _txtSalary);
            // Row 3: Gender | Status
            AddRowCC(tbl, 3, "Gender",    out _cmbGender, "Status",       out _cmbStatus);
            // Row 4: Joining Date | Address
            tbl.Controls.Add(Lbl("Joining Date"), 0, 4);
            _dtp = new DateTimePicker { Dock = DockStyle.Fill, Margin = new Padding(0, 8, 8, 8), Format = DateTimePickerFormat.Short, Font = AppTheme.FontNormal };
            tbl.Controls.Add(_dtp, 1, 4);
            tbl.Controls.Add(Lbl("Address"), 2, 4);
            _txtAddress = Tb();
            tbl.Controls.Add(_txtAddress, 3, 4);

            // Row 5: Notes (span full row)
            tbl.Controls.Add(Lbl("Notes"), 0, 5);
            _txtNotes = new TextBox { Dock = DockStyle.Fill, Font = AppTheme.FontNormal, BorderStyle = BorderStyle.FixedSingle, Multiline = true, Margin = new Padding(0, 6, 0, 6) };
            tbl.SetColumnSpan(_txtNotes, 3);
            tbl.Controls.Add(_txtNotes, 1, 5);

            body.Controls.Add(tbl);

            // Init combo items
            _cmbDept.Items.AddRange(new object[] { "Information Technology", "Human Resources", "Finance", "Marketing", "Sales", "Operations", "Admin", "Legal", "Research & Development" });
            _cmbDept.DropDownStyle = ComboBoxStyle.DropDown;

            _cmbGender.Items.AddRange(new object[] { "Male", "Female", "Other" });
            _cmbGender.SelectedIndex = 0;

            _cmbStatus.Items.AddRange(new object[] { "Active", "Inactive", "On Leave" });
            _cmbStatus.SelectedIndex = 0;

            AcceptButton = btnSave;

            // Add in correct dock order
            Controls.Add(body);    // Fill (first)
            Controls.Add(btnBar);  // Bottom
            Controls.Add(hdr);     // Top (last)
        }

        // TextBox + ComboBox
        private void AddRow(TableLayoutPanel t, int row, string l1, out TextBox tb, string l2, out ComboBox cb)
        {
            t.Controls.Add(Lbl(l1), 0, row);
            tb = Tb(); t.Controls.Add(tb, 1, row);
            t.Controls.Add(Lbl(l2), 2, row);
            cb = new ComboBox { Dock = DockStyle.Fill, Font = AppTheme.FontNormal, Margin = new Padding(0, 8, 8, 8) };
            t.Controls.Add(cb, 3, row);
        }
        // TextBox + TextBox
        private void AddRowTT(TableLayoutPanel t, int row, string l1, out TextBox tb1, string l2, out TextBox tb2)
        {
            t.Controls.Add(Lbl(l1), 0, row);
            tb1 = Tb(); t.Controls.Add(tb1, 1, row);
            t.Controls.Add(Lbl(l2), 2, row);
            tb2 = Tb(); t.Controls.Add(tb2, 3, row);
        }
        // ComboBox + ComboBox
        private void AddRowCC(TableLayoutPanel t, int row, string l1, out ComboBox cb1, string l2, out ComboBox cb2)
        {
            t.Controls.Add(Lbl(l1), 0, row);
            cb1 = new ComboBox { Dock = DockStyle.Fill, Font = AppTheme.FontNormal, Margin = new Padding(0, 8, 8, 8) };
            t.Controls.Add(cb1, 1, row);
            t.Controls.Add(Lbl(l2), 2, row);
            cb2 = new ComboBox { Dock = DockStyle.Fill, Font = AppTheme.FontNormal, Margin = new Padding(0, 8, 0, 8) };
            t.Controls.Add(cb2, 3, row);
        }

        private static TextBox Tb() => new TextBox { Dock = DockStyle.Fill, Font = AppTheme.FontNormal, BorderStyle = BorderStyle.FixedSingle, Margin = new Padding(0, 8, 8, 8) };

        private static Label Lbl(string text) => new Label
        {
            Text = text, Dock = DockStyle.Fill, AutoSize = false,
            Font = AppTheme.FontNormal, ForeColor = AppTheme.TextSecondary,
            TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(2, 0, 6, 0)
        };

        private void Populate(Employee e)
        {
            _txtName.Text    = e.FullName;
            _txtEmail.Text   = e.Email;
            _txtPhone.Text   = e.Phone;
            _cmbDept.Text    = e.Department;
            _txtDesig.Text   = e.Designation;
            _txtSalary.Text  = e.Salary.ToString("0.##");
            _cmbGender.Text  = e.Gender;
            _cmbStatus.Text  = e.Status;
            _txtAddress.Text = e.Address;
            _txtNotes.Text   = e.Notes;
            if (DateTime.TryParse(e.JoiningDate, out var d)) _dtp.Value = d;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!Validate()) return;
            var emp = ReadForm();
            if (_isEdit)
            {
                emp.Id = _original.Id;
                Db.UpdateEmployee(emp);
                MessageBox.Show("Record successfully update ho gaya!", "Updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Db.AddEmployee(emp);
                MessageBox.Show("Naya employee successfully add ho gaya!", "Saved",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private new bool Validate()
        {
            if (string.IsNullOrWhiteSpace(_txtName.Text))
            {
                Warn("Full Name zaroori hai."); _txtName.Focus(); return false;
            }
            if (!string.IsNullOrWhiteSpace(_txtSalary.Text) &&
                !decimal.TryParse(_txtSalary.Text.Trim(), out _))
            {
                Warn("Salary sirf number honi chahiye."); _txtSalary.Focus(); return false;
            }
            if (!string.IsNullOrWhiteSpace(_txtEmail.Text) &&
                !_txtEmail.Text.Contains("@"))
            {
                Warn("Sahi email address likhein."); _txtEmail.Focus(); return false;
            }
            return true;
        }

        private Employee ReadForm()
        {
            decimal.TryParse(_txtSalary.Text.Trim(), out decimal sal);
            return new Employee
            {
                FullName    = _txtName.Text.Trim(),
                Email       = _txtEmail.Text.Trim(),
                Phone       = _txtPhone.Text.Trim(),
                Department  = _cmbDept.Text.Trim(),
                Designation = _txtDesig.Text.Trim(),
                Salary      = sal,
                Gender      = _cmbGender.Text,
                JoiningDate = _dtp.Value.ToString("yyyy-MM-dd"),
                Status      = _cmbStatus.Text,
                Address     = _txtAddress.Text.Trim(),
                Notes       = _txtNotes.Text.Trim(),
            };
        }

        private static void Warn(string msg) =>
            MessageBox.Show(msg, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
