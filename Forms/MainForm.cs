using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace EmployeeManagementSystem.Forms
{
    public class MainForm : Form
    {
        private readonly string _username;
        private Panel  _contentPanel;
        private Label  _statusLabel, _clockLabel;
        private System.Windows.Forms.Timer _clockTimer;

        private DashboardControl _dashboard;
        private EmployeeControl  _employees;
        private ReportsControl   _reports;

        public MainForm(string username)
        {
            _username = username;
            BuildUi();
            NavigateTo("Dashboard");
        }

        private void BuildUi()
        {
            Text        = "Employee Management System";
            Size        = new Size(1220, 740);
            MinimumSize = new Size(1000, 640);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor   = AppTheme.Background;

            // ── Content area (Fill — add first) ──────────────────────────────
            _contentPanel = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Background };

            // ── Status bar (Bottom) ───────────────────────────────────────────
            var statusBar = new Panel { Dock = DockStyle.Bottom, Height = 28, BackColor = AppTheme.Sidebar };
            _statusLabel  = new Label
            {
                Dock = DockStyle.Fill, AutoSize = false,
                ForeColor = Color.FromArgb(160, 220, 210), Font = AppTheme.FontSmall,
                TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(12, 0, 0, 0),
                Text = "  Ready"
            };
            _clockLabel = new Label
            {
                Dock = DockStyle.Right, Width = 240, AutoSize = false,
                ForeColor = Color.FromArgb(160, 220, 210), Font = AppTheme.FontSmall,
                TextAlign = ContentAlignment.MiddleRight, Padding = new Padding(0, 0, 12, 0),
            };
            statusBar.Controls.Add(_clockLabel);
            statusBar.Controls.Add(_statusLabel);

            // ── Sidebar (Left) ────────────────────────────────────────────────
            var sidebar = new Panel { Dock = DockStyle.Left, Width = 215, BackColor = AppTheme.Sidebar };
            sidebar.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var b = new SolidBrush(Color.FromArgb(12, 255, 255, 255));
                e.Graphics.FillEllipse(b, sidebar.Width - 90, -55, 170, 170);
            };

            // Logo
            var logo = new Panel { Dock = DockStyle.Top, Height = 72, BackColor = Color.FromArgb(20, 0, 0, 0) };
            logo.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var b = new SolidBrush(AppTheme.Accent))
                    g.FillEllipse(b, 14, 14, 44, 44);
                using (var f = new Font("Segoe UI", 13F, FontStyle.Bold))
                using (var b = new SolidBrush(Color.White))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString("EMS", f, b, new RectangleF(14, 14, 44, 44), sf);
                }
                using (var f = new Font("Segoe UI", 11F, FontStyle.Bold))
                using (var b = new SolidBrush(Color.White))
                    g.DrawString("EMS Pro", f, b, 68, 16);
                using (var f = new Font("Segoe UI", 8F))
                using (var b = new SolidBrush(Color.FromArgb(120, 255, 255, 255)))
                    g.DrawString("Management System", f, b, 68, 38);
            };

            var sep1 = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Color.FromArgb(35, 255, 255, 255) };

            var navLabel = new Label
            {
                Dock = DockStyle.Top, Height = 32,
                Text = "   NAVIGATION", Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 255, 255, 255), BackColor = Color.Transparent,
                TextAlign = ContentAlignment.BottomLeft
            };

            var btnDash    = MakeNavBtn("  ▦   Dashboard",  "Dashboard");
            var btnEmp     = MakeNavBtn("  ☰   Employees",  "Employees");
            var btnReports = MakeNavBtn("  ≡   Reports",    "Reports");

            // Bottom area of sidebar
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 130, BackColor = Color.FromArgb(22, 0, 0, 0) };
            var sep2 = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Color.FromArgb(35, 255, 255, 255) };
            var userLabel = new Label
            {
                Dock = DockStyle.Top, Height = 52, AutoSize = false,
                ForeColor = Color.White, Font = AppTheme.FontNormal,
                Padding = new Padding(15, 10, 0, 0),
                Text = $"  \U0001f464  {_username}\n         Administrator"
            };
            var btnChangePwd = new Label
            {
                Dock = DockStyle.Top, Height = 26,
                Text = "   Change Password", Font = AppTheme.FontSmall,
                ForeColor = Color.FromArgb(120, 255, 255, 255),
                TextAlign = ContentAlignment.MiddleLeft, Cursor = Cursors.Hand
            };
            btnChangePwd.Click += (s, e) =>
            {
                using var dlg = new ChangePasswordForm(_username);
                dlg.ShowDialog(this);
            };
            var btnLogout = MakeNavBtn("  ✕   Logout", "Logout");
            btnLogout.BackColor = Color.FromArgb(28, 229, 57, 53);
            btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(55, 229, 57, 53);

            // Add to bottomPanel (top→bottom: sep2, userLabel, btnChangePwd, btnLogout)
            bottomPanel.Controls.Add(btnLogout);     // add first → docks last → bottom
            bottomPanel.Controls.Add(btnChangePwd);
            bottomPanel.Controls.Add(userLabel);
            bottomPanel.Controls.Add(sep2);           // add last → docks first → top

            // Add to sidebar (top→bottom: logo, sep1, navLabel, btnDash, btnEmp, btnReports; bottomPanel at bottom)
            sidebar.Controls.Add(bottomPanel);        // Bottom — add first
            sidebar.Controls.Add(btnReports);         // Top — add in reverse
            sidebar.Controls.Add(btnEmp);
            sidebar.Controls.Add(btnDash);
            sidebar.Controls.Add(navLabel);
            sidebar.Controls.Add(sep1);
            sidebar.Controls.Add(logo);               // Top — add last → topmost

            // ── Header (Top — add last) ───────────────────────────────────────
            var header = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = AppTheme.Header };
            header.Paint += (s, e) =>
                e.Graphics.FillRectangle(new SolidBrush(AppTheme.Accent), 0, header.Height - 3, header.Width, 3);

            var lblAppTitle = new Label
            {
                Dock = DockStyle.Fill, AutoSize = false,
                ForeColor = Color.White, Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(18, 0, 0, 3),
                Text = "Employee Management System"
            };
            var lblUserH = new Label
            {
                Dock = DockStyle.Right, Width = 220, AutoSize = false,
                ForeColor = Color.FromArgb(170, 220, 210), Font = AppTheme.FontNormal,
                TextAlign = ContentAlignment.MiddleRight, Padding = new Padding(0, 0, 16, 3),
                Text = $"Welcome,  {_username}"
            };
            header.Controls.Add(lblAppTitle);
            header.Controls.Add(lblUserH);

            // ── Add to form ───────────────────────────────────────────────────
            Controls.Add(_contentPanel);   // Fill (first)
            Controls.Add(sidebar);          // Left
            Controls.Add(statusBar);        // Bottom
            Controls.Add(header);           // Top (last)

            // Clock
            _clockTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _clockTimer.Tick += (s, e) => UpdateClock();
            _clockTimer.Start();
            UpdateClock();
        }

        private Button MakeNavBtn(string text, string tag)
        {
            var b = new Button
            {
                Text      = text, Tag = tag,
                Dock      = DockStyle.Top, Height = 46,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(195, 240, 235),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                Font      = new Font("Segoe UI", 10.5F),
                Cursor    = Cursors.Hand,
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = Color.FromArgb(28, 255, 255, 255);
            b.Click += NavBtn_Click;
            return b;
        }

        private void NavBtn_Click(object sender, EventArgs e)
        {
            if (sender is Button b && b.Tag is string tag)
            {
                if (tag == "Logout") { Logout(); return; }
                NavigateTo(tag);
            }
        }

        public void NavigateTo(string page)
        {
            _contentPanel.Controls.Clear();
            Control ctrl = null;
            switch (page)
            {
                case "Dashboard":
                    _dashboard ??= new DashboardControl(this);
                    _dashboard.RefreshData();
                    ctrl = _dashboard;
                    break;
                case "Employees":
                    _employees ??= new EmployeeControl(this);
                    _employees.RefreshGrid();
                    ctrl = _employees;
                    break;
                case "Reports":
                    _reports ??= new ReportsControl();
                    _reports.RefreshData();
                    ctrl = _reports;
                    break;
            }
            if (ctrl != null)
            {
                ctrl.Dock = DockStyle.Fill;
                _contentPanel.Controls.Add(ctrl);
            }
        }

        public void SetStatus(string msg) =>
            _statusLabel.Text = "  " + msg;

        private void UpdateClock() =>
            _clockLabel.Text = DateTime.Now.ToString("ddd, dd MMM yyyy    HH:mm:ss   ");

        private void Logout()
        {
            if (MessageBox.Show("Kya aap logout karna chahte hain?", "Logout",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _clockTimer?.Stop();
            base.OnFormClosed(e);
        }
    }
}
