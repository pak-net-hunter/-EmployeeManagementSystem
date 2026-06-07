using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using EmployeeManagementSystem.Database;

namespace EmployeeManagementSystem.Forms
{
    public class ReportsControl : UserControl
    {
        private DataGridView _dgv;
        private Label        _lblSummary;

        public ReportsControl()
        {
            Dock      = DockStyle.Fill;
            BackColor = AppTheme.Background;
            BuildUi();
        }

        private void BuildUi()
        {
            // ── Page title ────────────────────────────────────────────────────
            var topBar = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = AppTheme.Background };
            var lblPage = new Label
            {
                Text = "Reports", Dock = DockStyle.Left, Width = 220, AutoSize = false,
                Font = AppTheme.FontTitle, ForeColor = AppTheme.TextPrimary,
                TextAlign = ContentAlignment.BottomLeft, Padding = new Padding(24, 0, 0, 10)
            };
            var btnRefresh = UiHelper.MakeButton("↻  Refresh", AppTheme.Accent, 115, 32);
            btnRefresh.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnRefresh.Click += (s, e) => RefreshData();
            topBar.Resize += (s, e) => btnRefresh.Location = new Point(topBar.Width - 135, 18);
            topBar.Controls.Add(lblPage);
            topBar.Controls.Add(btnRefresh);

            // ── Section header ────────────────────────────────────────────────
            var secBar = new Panel { Dock = DockStyle.Top, Height = 38, BackColor = AppTheme.Background };
            secBar.Controls.Add(new Label
            {
                Text = "  Department-wise Summary", Dock = DockStyle.Fill, AutoSize = false,
                Font = AppTheme.FontLarge, ForeColor = AppTheme.TextPrimary,
                TextAlign = ContentAlignment.MiddleLeft
            });

            // ── Footer totals bar ─────────────────────────────────────────────
            _lblSummary = new Label
            {
                Dock = DockStyle.Bottom, Height = 30, AutoSize = false,
                Font = AppTheme.FontSmall, ForeColor = AppTheme.TextSecondary,
                TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(24, 0, 0, 0),
                BackColor = AppTheme.Card
            };
            var footerLine = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = AppTheme.Border };

            // ── Grid card ─────────────────────────────────────────────────────
            var wrapper = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Background, Padding = new Padding(20, 6, 20, 10) };

            _dgv = new DataGridView { Dock = DockStyle.Fill };
            UiHelper.StyleGrid(_dgv);
            _dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _dgv.CellFormatting     += Dgv_CellFormatting;

            wrapper.Controls.Add(_dgv);

            // ── Add to control ────────────────────────────────────────────────
            Controls.Add(wrapper);      // Fill (first)
            Controls.Add(_lblSummary);  // Bottom
            Controls.Add(footerLine);   // Bottom (above summary)
            Controls.Add(secBar);       // Top
            Controls.Add(topBar);       // Top (last = topmost)
        }

        private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var colName = _dgv.Columns[e.ColumnIndex].Name;
            switch (colName)
            {
                case "Active":
                    e.CellStyle.ForeColor = AppTheme.Active;
                    e.CellStyle.Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                    break;
                case "Inactive":
                    e.CellStyle.ForeColor = AppTheme.Inactive;
                    e.CellStyle.Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                    break;
                case "On Leave":
                    e.CellStyle.ForeColor = AppTheme.OnLeave;
                    e.CellStyle.Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                    break;
                case "Avg Salary (PKR)":
                case "Total Salary (PKR)":
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    break;
            }
        }

        public void RefreshData()
        {
            var dt = Db.GetDepartmentSummary();
            _dgv.DataSource = dt;

            if (_dgv.Columns.Contains("Avg Salary (PKR)"))
                _dgv.Columns["Avg Salary (PKR)"].DefaultCellStyle.Format = "N0";
            if (_dgv.Columns.Contains("Total Salary (PKR)"))
                _dgv.Columns["Total Salary (PKR)"].DefaultCellStyle.Format = "N0";

            int    total  = Db.GetTotalEmployees();
            int    active = Db.GetActiveEmployees();
            decimal sal   = Db.GetTotalSalary();

            _lblSummary.Text =
                $"   Total Employees: {total}     |     " +
                $"Active: {active}     |     " +
                $"On Leave / Inactive: {total - active}     |     " +
                $"Total Monthly Payroll: PKR {sal:N0}";
        }
    }
}
