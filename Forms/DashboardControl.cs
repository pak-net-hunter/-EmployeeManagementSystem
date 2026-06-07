using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using EmployeeManagementSystem.Database;

namespace EmployeeManagementSystem.Forms
{
    public class DashboardControl : UserControl
    {
        private readonly MainForm _main;
        private Label _lblTotal, _lblActive, _lblDepts, _lblSalary;
        private Panel _chartPanel;
        private Dictionary<string, int> _deptData = new Dictionary<string, int>();

        public DashboardControl(MainForm main)
        {
            _main = main;
            Dock = DockStyle.Fill;
            BackColor = AppTheme.Background;
            AutoScroll = true;
            BuildUi();
        }

        private void BuildUi()
        {
            // ── Page title bar ────────────────────────────────────────────────
            var topBar = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = AppTheme.Background };
            var lblPage = new Label
            {
                Text = "Dashboard Overview", Dock = DockStyle.Left, Width = 400, AutoSize = false,
                Font = AppTheme.FontTitle, ForeColor = AppTheme.TextPrimary,
                TextAlign = ContentAlignment.BottomLeft, Padding = new Padding(24, 0, 0, 10)
            };
            var btnRefresh = UiHelper.MakeButton("↻  Refresh", AppTheme.Accent, 115, 32);
            btnRefresh.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnRefresh.Click += (s, e) => RefreshData();
            topBar.Resize += (s, e) => btnRefresh.Location = new Point(topBar.Width - 135, 18);
            topBar.Controls.Add(lblPage);
            topBar.Controls.Add(btnRefresh);

            // ── Stat cards ────────────────────────────────────────────────────
            var cardsRow = new Panel { Dock = DockStyle.Top, Height = 128, BackColor = AppTheme.Background };
            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false, BackColor = Color.Transparent,
                Padding = new Padding(20, 10, 20, 10)
            };
            flow.Controls.Add(BuildCard("Total Employees",   AppTheme.CardTeal,   out _lblTotal));
            flow.Controls.Add(BuildCard("Active Employees",  AppTheme.CardGreen,  out _lblActive));
            flow.Controls.Add(BuildCard("Departments",       AppTheme.CardBlue,   out _lblDepts));
            flow.Controls.Add(BuildCard("Monthly Payroll",   AppTheme.CardOrange, out _lblSalary));
            cardsRow.Controls.Add(flow);

            // ── Quick action bar ──────────────────────────────────────────────
            var actBar = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = AppTheme.Background };
            var btnAdd = UiHelper.MakeButton("+ Add Employee", AppTheme.Accent, 145, 34);
            btnAdd.Location = new Point(24, 7);
            btnAdd.Click += (s, e) =>
            {
                using var dlg = new AddEditEmployeeForm(null);
                if (dlg.ShowDialog(this) == DialogResult.OK)
                { RefreshData(); _main.NavigateTo("Employees"); }
            };
            var btnView = UiHelper.MakeButton("View All Employees", AppTheme.Sidebar, 158, 34);
            btnView.Location = new Point(178, 7);
            btnView.Click += (s, e) => _main.NavigateTo("Employees");
            actBar.Controls.Add(btnAdd);
            actBar.Controls.Add(btnView);

            // ── Chart title ───────────────────────────────────────────────────
            var chartHdr = new Panel { Dock = DockStyle.Top, Height = 38, BackColor = AppTheme.Background };
            var lblChart = new Label
            {
                Text = "  Employees by Department", Dock = DockStyle.Fill, AutoSize = false,
                Font = AppTheme.FontLarge, ForeColor = AppTheme.TextPrimary,
                TextAlign = ContentAlignment.MiddleLeft
            };
            chartHdr.Controls.Add(lblChart);

            // ── Bar chart panel ───────────────────────────────────────────────
            _chartPanel = new Panel { BackColor = AppTheme.Card };
            _chartPanel.Paint   += ChartPanel_Paint;
            _chartPanel.Resize  += (s, e) => _chartPanel.Invalidate();

            var chartWrap = new Panel { Dock = DockStyle.Top, Height = 300, BackColor = AppTheme.Background, Padding = new Padding(20, 4, 20, 12) };
            _chartPanel.Dock = DockStyle.Fill;
            chartWrap.Controls.Add(_chartPanel);

            // ── Add all (first = bottommost, last = topmost) ──────────────────
            Controls.Add(chartWrap);   // ← add first (appears at bottom)
            Controls.Add(chartHdr);
            Controls.Add(actBar);
            Controls.Add(cardsRow);
            Controls.Add(topBar);      // ← add last (appears at top)
        }

        private Panel BuildCard(string caption, Color accent, out Label valueLabel)
        {
            var card = new Panel
            {
                Width     = 218, Height = 106,
                BackColor = AppTheme.Card,
                Margin    = new Padding(0, 0, 18, 0),
            };
            card.Paint += (s, e) =>
            {
                using var pen = new Pen(AppTheme.Border, 1);
                e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };

            var strip = new Panel { Dock = DockStyle.Left, Width = 5, BackColor = accent };

            var lblCap = new Label
            {
                Text = caption, AutoSize = false, Size = new Size(200, 20), Location = new Point(16, 14),
                Font = AppTheme.FontSmall, ForeColor = AppTheme.TextSecondary
            };
            valueLabel = new Label
            {
                Text = "—", AutoSize = false, Size = new Size(200, 44), Location = new Point(14, 36),
                Font = AppTheme.FontHuge, ForeColor = AppTheme.TextPrimary
            };
            var lblSub = new Label
            {
                Text = "Updated now", AutoSize = false, Size = new Size(200, 16), Location = new Point(16, 84),
                Font = AppTheme.FontSmall, ForeColor = AppTheme.TextSecondary
            };

            card.Controls.Add(lblCap);
            card.Controls.Add(valueLabel);
            card.Controls.Add(lblSub);
            card.Controls.Add(strip);
            return card;
        }

        private void ChartPanel_Paint(object sender, PaintEventArgs e)
        {
            var g  = e.Graphics;
            var p  = (Panel)sender;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            int pw = p.ClientSize.Width, ph = p.ClientSize.Height;
            int padL = 28, padR = 70, padT = 20, padB = 20;
            int chartW = pw - padL - padR;
            int chartH = ph - padT - padB;

            if (_deptData == null || _deptData.Count == 0)
            {
                using var f = AppTheme.FontNormal;
                using var b = new SolidBrush(AppTheme.TextSecondary);
                g.DrawString("Koi data nahi — employees add karein.", f, b, padL + 10, padT + chartH / 2 - 10);
                return;
            }

            int maxVal = 1;
            foreach (var kv in _deptData)
                if (kv.Value > maxVal) maxVal = kv.Value;

            int count      = _deptData.Count;
            int barH       = Math.Min(30, (chartH - 8) / count - 10);
            int labelW     = 190;
            int barStartX  = padL + labelW;
            int maxBarW    = chartW - labelW - 10;
            int colorIdx   = 0;
            int y          = padT + 4;

            using var lblFont = new Font("Segoe UI", 9.5F);
            using var numFont = new Font("Segoe UI", 9F, FontStyle.Bold);

            foreach (var kv in _deptData)
            {
                if (y + barH > padT + chartH) break;

                int barW  = (int)((double)kv.Value / maxVal * maxBarW);
                barW = Math.Max(barW, 6);
                var col   = AppTheme.ChartBars[colorIdx % AppTheme.ChartBars.Length];

                // Dept label
                using (var b = new SolidBrush(AppTheme.TextPrimary))
                    g.DrawString(kv.Key, lblFont, b, padL, y + (barH - 14) / 2);

                // Background track
                using (var b = new SolidBrush(Color.FromArgb(25, col.R, col.G, col.B)))
                    g.FillRectangle(b, barStartX, y, maxBarW, barH);

                // Filled bar
                using (var b = new SolidBrush(col))
                    g.FillRectangle(b, barStartX, y, barW, barH);

                // Count label
                using (var b = new SolidBrush(AppTheme.TextPrimary))
                    g.DrawString(kv.Value.ToString(), numFont, b,
                        barStartX + barW + 8, y + (barH - 14) / 2);

                y += barH + 14;
                colorIdx++;
            }
        }

        public void RefreshData()
        {
            _lblTotal.Text  = Db.GetTotalEmployees().ToString("N0");
            _lblActive.Text = Db.GetActiveEmployees().ToString("N0");
            _lblDepts.Text  = Db.GetTotalDepts().ToString("N0");
            _lblSalary.Text = "PKR " + Db.GetTotalSalary().ToString("N0");
            _deptData       = Db.GetByDepartment();
            _chartPanel.Invalidate();
            _main?.SetStatus("Dashboard refreshed — " + DateTime.Now.ToString("HH:mm:ss"));
        }
    }
}
