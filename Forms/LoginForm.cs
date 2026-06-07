using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using EmployeeManagementSystem.Database;

namespace EmployeeManagementSystem.Forms
{
    public class LoginForm : Form
    {
        private TextBox _txtUser, _txtPass;
        private Label   _lblError;

        public LoginForm()
        {
            Text            = "Employee Management System — Login";
            Size            = new Size(840, 520);
            MinimumSize     = Size;
            MaximumSize     = Size;
            StartPosition   = FormStartPosition.CenterScreen;
            BackColor       = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox     = false;

            BuildUi();
        }

        private void BuildUi()
        {
            // ── Right panel (Fill — add first) ───────────────────────────────
            var right = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

            var card = new Panel { Width = 360, Height = 400, BackColor = Color.White };

            int y = 0;
            var lblWelcome = new Label
            {
                Text = "Welcome Back", Size = new Size(360, 38), Location = new Point(0, y),
                Font = AppTheme.FontTitle, ForeColor = AppTheme.TextPrimary,
                TextAlign = ContentAlignment.BottomLeft
            };
            y += 44;
            var lblSub = new Label
            {
                Text = "Sign in to your account", Size = new Size(360, 22), Location = new Point(0, y),
                Font = AppTheme.FontNormal, ForeColor = AppTheme.TextSecondary
            };
            y += 38;

            y = AddField(card, y, "Username", out _txtUser, false);
            y = AddField(card, y, "Password", out _txtPass, true);

            _lblError = new Label
            {
                Size = new Size(360, 20), Location = new Point(0, y),
                Font = AppTheme.FontSmall, ForeColor = AppTheme.Inactive,
                AutoSize = false, Visible = false
            };
            y += 24;

            var btnLogin = UiHelper.MakeButton("  SIGN IN", AppTheme.Accent, 360, 44);
            btnLogin.Font     = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnLogin.Location = new Point(0, y);
            y += 52;

            var hint = new Label
            {
                Text = "Default credentials:   admin  /  admin@123",
                Size = new Size(360, 18), Location = new Point(0, y),
                Font = AppTheme.FontSmall, ForeColor = AppTheme.TextSecondary,
                TextAlign = ContentAlignment.MiddleCenter
            };

            card.Controls.AddRange(new Control[] { lblWelcome, lblSub, _lblError, btnLogin, hint });
            card.Size = new Size(360, y + 24);

            right.Resize += (s, e) => CenterCard(card, right);
            CenterCard(card, right);
            right.Controls.Add(card);

            btnLogin.Click += BtnLogin_Click;
            AcceptButton    = btnLogin;

            // ── Left branding panel (Left — add last) ────────────────────────
            var left = new Panel { Dock = DockStyle.Left, Width = 380, BackColor = AppTheme.Sidebar };
            left.Paint += PaintBrand;

            Controls.Add(right);
            Controls.Add(left);
        }

        private static int AddField(Panel parent, int y, string label, out TextBox tb, bool password)
        {
            var lbl = new Label
            {
                Text = label, Size = new Size(360, 20), Location = new Point(0, y),
                Font = AppTheme.FontNormal, ForeColor = AppTheme.TextSecondary
            };
            parent.Controls.Add(lbl);
            y += 24;
            tb = new TextBox
            {
                Width = 360, Location = new Point(0, y),
                Font = AppTheme.FontNormal, BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = password
            };
            if (label == "Username") tb.Text = "admin";
            parent.Controls.Add(tb);
            y += 42;
            return y;
        }

        private static void CenterCard(Panel card, Panel parent)
        {
            card.Location = new Point(
                Math.Max(0, (parent.Width  - card.Width)  / 2),
                Math.Max(0, (parent.Height - card.Height) / 2));
        }

        private void PaintBrand(object sender, PaintEventArgs e)
        {
            var g  = e.Graphics;
            var p  = (Panel)sender;
            int w  = p.Width, h = p.Height;
            g.SmoothingMode       = SmoothingMode.AntiAlias;
            g.TextRenderingHint   = TextRenderingHint.ClearTypeGridFit;

            // Subtle decorative circles
            using (var b = new SolidBrush(Color.FromArgb(18, 255, 255, 255)))
            {
                g.FillEllipse(b, w - 130, -70, 220, 220);
                g.FillEllipse(b, -70, h - 160, 220, 220);
                g.FillEllipse(b, w / 2 - 30, h - 100, 140, 140);
            }

            // Logo circle
            int lx = w / 2 - 50, ly = h / 2 - 105;
            using (var b = new SolidBrush(AppTheme.Accent))
                g.FillEllipse(b, lx, ly, 100, 100);
            using (var f = new Font("Segoe UI", 24F, FontStyle.Bold))
            using (var b = new SolidBrush(Color.White))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("EMS", f, b, new RectangleF(lx, ly, 100, 100), sf);
            }

            // Title
            var sfC = new StringFormat { Alignment = StringAlignment.Center };
            using (var f = new Font("Segoe UI", 20F, FontStyle.Bold))
            using (var b = new SolidBrush(Color.White))
            {
                g.DrawString("Employee", f, b, new RectangleF(20, ly + 115, w - 40, 36), sfC);
                g.DrawString("Management System", f, b, new RectangleF(20, ly + 151, w - 40, 36), sfC);
            }
            using (var f = new Font("Segoe UI", 10.5F))
            using (var b = new SolidBrush(Color.FromArgb(150, 255, 255, 255)))
                g.DrawString("Manage your team with confidence", f, b, new RectangleF(20, ly + 200, w - 40, 28), sfC);

            // Divider
            using (var pen = new Pen(Color.FromArgb(40, 255, 255, 255), 1))
                g.DrawLine(pen, 40, h - 56, w - 40, h - 56);

            // Bottom tag
            using (var f = new Font("Segoe UI", 8.5F))
            using (var b = new SolidBrush(Color.FromArgb(90, 255, 255, 255)))
                g.DrawString("C#  ·  WinForms  ·  SQLite  ·  .NET 8", f, b,
                    new RectangleF(20, h - 42, w - 40, 28), sfC);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            _lblError.Visible = false;
            string u = _txtUser.Text.Trim();
            string p = _txtPass.Text;

            if (string.IsNullOrWhiteSpace(u) || string.IsNullOrWhiteSpace(p))
            {
                ShowError("Username aur password dono zaroori hain.");
                return;
            }
            if (Db.ValidateUser(u, p))
            {
                Hide();
                using var main = new MainForm(u);
                main.ShowDialog();
                Close();
            }
            else
            {
                ShowError("Ghalat username ya password. Dobara koshish karein.");
                _txtPass.Clear();
                _txtPass.Focus();
            }
        }

        private void ShowError(string msg)
        {
            _lblError.Text    = "⚠  " + msg;
            _lblError.Visible = true;
        }
    }
}
