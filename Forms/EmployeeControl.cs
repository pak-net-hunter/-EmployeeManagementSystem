using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using EmployeeManagementSystem.Database;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Forms
{
    public class EmployeeControl : UserControl
    {
        private readonly MainForm _main;
        private DataGridView _dgv;
        private TextBox  _txtSearch;
        private ComboBox _cmbDept;
        private Label    _lblCount;
        private List<Employee> _currentList = new List<Employee>();

        public EmployeeControl(MainForm main)
        {
            _main = main;
            Dock  = DockStyle.Fill;
            BackColor = AppTheme.Background;
            BuildUi();
        }

        private void BuildUi()
        {
            // ── Page title bar ────────────────────────────────────────────────
            var topBar = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = AppTheme.Background };
            var lblPage = new Label
            {
                Text = "Employees", Dock = DockStyle.Left, Width = 220, AutoSize = false,
                Font = AppTheme.FontTitle, ForeColor = AppTheme.TextPrimary,
                TextAlign = ContentAlignment.BottomLeft, Padding = new Padding(24, 0, 0, 10)
            };
            var btnAdd = UiHelper.MakeButton("+ Add Employee", AppTheme.Accent, 145, 34);
            btnAdd.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnAdd.Click += BtnAdd_Click;
            topBar.Resize += (s, e) => btnAdd.Location = new Point(topBar.Width - 165, 18);
            topBar.Controls.Add(lblPage);
            topBar.Controls.Add(btnAdd);

            // ── Toolbar ───────────────────────────────────────────────────────
            var toolbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = AppTheme.Card };
            toolbar.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(AppTheme.Border), 0, toolbar.Height - 1, toolbar.Width, toolbar.Height - 1);

            int tx = 14;
            AddLabel(toolbar, "Search:", ref tx, 14);
            _txtSearch = UiHelper.MakeTextBox(225);
            _txtSearch.Location = new Point(tx, 12);
            toolbar.Controls.Add(_txtSearch); tx += 238;

            AddLabel(toolbar, "Department:", ref tx, 14);
            _cmbDept = UiHelper.MakeCombo(180);
            _cmbDept.Location = new Point(tx, 12);
            toolbar.Controls.Add(_cmbDept); tx += 194;

            var btnGo     = UiHelper.MakeButton("Search",   AppTheme.Accent,                    80, 28); btnGo.Location     = new Point(tx, 12); tx += 90;
            var btnClear  = UiHelper.MakeButton("Clear",    AppTheme.Sidebar,                   70, 28); btnClear.Location  = new Point(tx, 12); tx += 82;
            var btnExport = UiHelper.MakeButton("⬇ Export", Color.FromArgb(67, 160, 71), 100, 28); btnExport.Location = new Point(tx, 12);

            toolbar.Controls.AddRange(new Control[] { btnGo, btnClear, btnExport });

            btnGo.Click     += (s, e) => DoSearch();
            btnClear.Click  += (s, e) => { _txtSearch.Clear(); _cmbDept.SelectedIndex = 0; RefreshGrid(); };
            btnExport.Click += (s, e) => ExportCsv();
            _txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) DoSearch(); };

            // ── Footer status ─────────────────────────────────────────────────
            var footer = new Panel { Dock = DockStyle.Bottom, Height = 28, BackColor = AppTheme.Card };
            footer.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(AppTheme.Border), 0, 0, footer.Width, 0);
            _lblCount = new Label
            {
                Dock = DockStyle.Left, Width = 280, AutoSize = false,
                Font = AppTheme.FontSmall, ForeColor = AppTheme.TextSecondary,
                TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(12, 0, 0, 0)
            };
            var lblHint = new Label
            {
                Dock = DockStyle.Right, Width = 300, AutoSize = false,
                Font = AppTheme.FontSmall, ForeColor = AppTheme.TextSecondary,
                TextAlign = ContentAlignment.MiddleRight, Padding = new Padding(0, 0, 12, 0),
                Text = "Double-click or right-click row → Edit / Delete"
            };
            footer.Controls.Add(_lblCount);
            footer.Controls.Add(lblHint);

            // ── DataGridView ──────────────────────────────────────────────────
            _dgv = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = false };
            UiHelper.StyleGrid(_dgv);
            SetupColumns();
            _dgv.CellFormatting  += Dgv_CellFormatting;
            _dgv.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) EditSelected(); };
            _dgv.MouseClick      += Dgv_RightClick;

            var ctx = new ContextMenuStrip();
            ctx.Font = AppTheme.FontNormal;
            ctx.Items.Add("  ✏  Edit Employee",   null, (s, e) => EditSelected());
            ctx.Items.Add("  🗑  Delete Employee", null, (s, e) => DeleteSelected());
            ctx.Items.Add(new ToolStripSeparator());
            ctx.Items.Add("  👁  View Details",    null, (s, e) => ViewSelected());
            _dgv.ContextMenuStrip = ctx;

            // ── Add to control (Fill first) ───────────────────────────────────
            Controls.Add(_dgv);       // Fill (first)
            Controls.Add(footer);     // Bottom
            Controls.Add(toolbar);    // Top
            Controls.Add(topBar);     // Top (last = topmost)
        }

        private static void AddLabel(Panel parent, string text, ref int x, int y)
        {
            var l = new Label
            {
                Text = text, AutoSize = true, Location = new Point(x, y),
                Font = AppTheme.FontNormal, ForeColor = AppTheme.TextSecondary
            };
            parent.Controls.Add(l);
            x += l.PreferredWidth + 6;
        }

        private void SetupColumns()
        {
            _dgv.Columns.Clear();
            _dgv.Columns.Add(Col("Id",          "Id",          "#",             52,  false));
            _dgv.Columns.Add(Col("FullName",     "FullName",    "Full Name",     165, true));
            _dgv.Columns.Add(Col("Department",   "Department",  "Department",    155, true));
            _dgv.Columns.Add(Col("Designation",  "Designation", "Designation",   145, true));
            _dgv.Columns.Add(Col("Gender",       "Gender",      "Gender",         72, true));
            _dgv.Columns.Add(Col("Phone",        "Phone",       "Phone",         120, true));

            var salCol = Col("Salary", "Salary", "Salary (PKR)", 120, true);
            salCol.DefaultCellStyle = new DataGridViewCellStyle
            {
                Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight
            };
            _dgv.Columns.Add(salCol);
            _dgv.Columns.Add(Col("JoiningDate",  "JoiningDate", "Joined",        100, true));
            _dgv.Columns.Add(Col("Status",       "Status",      "Status",         90, true));
        }

        private static DataGridViewTextBoxColumn Col(string name, string prop, string hdr, int w, bool fill) =>
            new DataGridViewTextBoxColumn
            {
                Name = name, DataPropertyName = prop, HeaderText = hdr,
                Width = w, ReadOnly = true,
                AutoSizeMode = fill
                    ? DataGridViewAutoSizeColumnMode.Fill
                    : DataGridViewAutoSizeColumnMode.None
            };

        private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || !(_dgv.Rows[e.RowIndex].DataBoundItem is Employee emp)) return;

            if (_dgv.Columns[e.ColumnIndex].Name == "Status")
            {
                e.CellStyle.ForeColor = emp.Status switch
                {
                    "Active"   => AppTheme.Active,
                    "Inactive" => AppTheme.Inactive,
                    "On Leave" => AppTheme.OnLeave,
                    _          => AppTheme.TextSecondary
                };
                e.CellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            }
        }

        private void Dgv_RightClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            var hit = _dgv.HitTest(e.X, e.Y);
            if (hit.RowIndex >= 0) _dgv.Rows[hit.RowIndex].Selected = true;
        }

        private void DoSearch()
        {
            string dept = _cmbDept.SelectedIndex <= 0 ? "" : _cmbDept.SelectedItem?.ToString() ?? "";
            RefreshGrid(_txtSearch.Text.Trim(), dept);
        }

        public void RefreshGrid(string search = "", string deptFilter = "")
        {
            // Refresh dept filter list
            var depts = Db.GetDepartments();
            string prevDept = _cmbDept.SelectedIndex > 0 ? _cmbDept.SelectedItem?.ToString() : "";
            _cmbDept.Items.Clear();
            _cmbDept.Items.Add("All Departments");
            foreach (var d in depts) _cmbDept.Items.Add(d);
            int idx = _cmbDept.Items.IndexOf(prevDept);
            _cmbDept.SelectedIndex = idx > 0 ? idx : 0;

            _currentList   = Db.GetEmployees(search, deptFilter);
            _dgv.DataSource = null;
            _dgv.DataSource = _currentList;
            _lblCount.Text  = $"  Showing {_currentList.Count} employee(s)";
            _main?.SetStatus($"{_currentList.Count} employees loaded");
        }

        private Employee SelectedEmployee()
        {
            if (_dgv.CurrentRow?.DataBoundItem is Employee e) return e;
            return null;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using var dlg = new AddEditEmployeeForm(null);
            if (dlg.ShowDialog(this) == DialogResult.OK) RefreshGrid();
        }

        private void EditSelected()
        {
            var emp = SelectedEmployee();
            if (emp == null) { ShowInfo("Pehle grid se koi employee select karein."); return; }
            using var dlg = new AddEditEmployeeForm(emp);
            if (dlg.ShowDialog(this) == DialogResult.OK) RefreshGrid();
        }

        private void DeleteSelected()
        {
            var emp = SelectedEmployee();
            if (emp == null) { ShowInfo("Pehle grid se koi employee select karein."); return; }
            var r = MessageBox.Show(
                $"Kya aap '{emp.FullName}' ko delete karna chahte hain?\nYeh action undo nahi ho sakta.",
                "Delete Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (r == DialogResult.Yes)
            {
                Db.DeleteEmployee(emp.Id);
                RefreshGrid();
                _main?.SetStatus($"'{emp.FullName}' delete ho gaya.");
            }
        }

        private void ViewSelected()
        {
            var e = SelectedEmployee();
            if (e == null) return;
            MessageBox.Show(
                $"ID           :  {e.Id}\n" +
                $"Full Name    :  {e.FullName}\n" +
                $"Email        :  {e.Email}\n" +
                $"Phone        :  {e.Phone}\n" +
                $"Department   :  {e.Department}\n" +
                $"Designation  :  {e.Designation}\n" +
                $"Gender       :  {e.Gender}\n" +
                $"Salary       :  PKR {e.Salary:N0}\n" +
                $"Joining Date :  {e.JoiningDate}\n" +
                $"Status       :  {e.Status}\n" +
                $"Address      :  {e.Address}\n" +
                $"Notes        :  {e.Notes}",
                $"Employee Details — {e.FullName}",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ExportCsv()
        {
            using var dlg = new SaveFileDialog
            {
                Filter   = "CSV Files (*.csv)|*.csv",
                FileName = $"employees_{DateTime.Now:yyyyMMdd_HHmm}"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            var sb = new StringBuilder();
            sb.AppendLine("ID,Full Name,Email,Phone,Department,Designation,Salary,Gender,Joining Date,Status,Address");
            foreach (var e in _currentList)
                sb.AppendLine($"{e.Id},{Qv(e.FullName)},{Qv(e.Email)},{Qv(e.Phone)},{Qv(e.Department)},{Qv(e.Designation)},{e.Salary},{Qv(e.Gender)},{Qv(e.JoiningDate)},{Qv(e.Status)},{Qv(e.Address)}");
            File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
            MessageBox.Show("CSV export ho gaya:\n" + dlg.FileName, "Export Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _main?.SetStatus("CSV exported → " + Path.GetFileName(dlg.FileName));
        }

        private static string Qv(string s) => $"\"{(s ?? "").Replace("\"", "\"\"")}\"";
        private static void ShowInfo(string msg) =>
            MessageBox.Show(msg, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
