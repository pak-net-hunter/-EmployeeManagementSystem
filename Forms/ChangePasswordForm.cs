using System;
using System.Drawing;
using System.Windows.Forms;
using EmployeeManagementSystem.Database;

namespace EmployeeManagementSystem.Forms
{
    public class ChangePasswordForm : Form
    {
        private readonly string _username;
        private TextBox _txtOld, _txtNew, _txtConfirm;

        public ChangePasswordForm(string username)
        {
            _username = username;

            Text            = "Change Password";
            Size            = new Size(420, 340);
            MinimumSize     = Size;
            MaximumSize     = Size;
            StartPosition   = FormStartPosition.CenterParent;
            BackColor       = AppTheme.Card;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;

            BuildUi();
        }

        private void BuildUi()
        {
            // Header
            var hdr = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = AppTheme.Sidebar };
            hdr.Controls.Add(new Label
            {
                Text = "  Change Password", Dock = DockStyle.Fill, AutoSize = false,
                ForeColor = Color.White, Font = AppTheme.FontLarge,
                TextAlign = ContentAlignment.MiddleLeft
            });

            // Body
            var body = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Card, Padding = new Padding(30, 16, 30, 16) };

            int y = 12;
            y = AddField(body, "Current Password",      y, out _txtOld,     true);
            y = AddField(body, "New Password",           y, out _txtNew,     true);
            y = AddField(body, "Confirm New Password",   y, out _txtConfirm, true);

            var btnSave = UiHelper.MakeButton("Update Password", AppTheme.Accent,  150, 36);
            var btnCancel = UiHelper.MakeButton("Cancel",        AppTheme.Sidebar, 100, 36);
            btnSave.Location   = new Point(0, y);
            btnCancel.Location = new Point(160, y);
            btnSave.Click   += BtnSave_Click;
            btnCancel.Click += (s, e) => Close();
            body.Controls.Add(btnSave);
            body.Controls.Add(btnCancel);

            Controls.Add(body);
            Controls.Add(hdr);
            AcceptButton = btnSave;
        }

        private static int AddField(Panel parent, string label, int y, out TextBox tb, bool password)
        {
            parent.Controls.Add(new Label
            {
                Text = label, AutoSize = true, Location = new Point(0, y),
                Font = AppTheme.FontNormal, ForeColor = AppTheme.TextSecondary
            });
            y += 22;
            tb = new TextBox
            {
                Width = 340, Location = new Point(0, y),
                Font  = AppTheme.FontNormal, BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = password
            };
            parent.Controls.Add(tb);
            y += 44;
            return y;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtOld.Text) ||
                string.IsNullOrWhiteSpace(_txtNew.Text) ||
                string.IsNullOrWhiteSpace(_txtConfirm.Text))
            {
                Warn("Tamam fields fill karein."); return;
            }
            if (_txtNew.Text != _txtConfirm.Text)
            {
                Warn("New password aur confirm password match nahi karte."); return;
            }
            if (_txtNew.Text.Length < 6)
            {
                Warn("Password kam az kam 6 characters ka hona chahiye."); return;
            }
            if (Db.ChangePassword(_username, _txtOld.Text, _txtNew.Text))
            {
                MessageBox.Show("Password successfully change ho gaya!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            else
            {
                MessageBox.Show("Purana password ghalat hai.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _txtOld.Clear(); _txtOld.Focus();
            }
        }

        private static void Warn(string msg) =>
            MessageBox.Show(msg, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
