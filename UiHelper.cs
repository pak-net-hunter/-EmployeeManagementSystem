using System.Drawing;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public static class UiHelper
    {
        public static void StyleGrid(DataGridView g)
        {
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor  = AppTheme.Sidebar;
            g.ColumnHeadersDefaultCellStyle.ForeColor  = Color.White;
            g.ColumnHeadersDefaultCellStyle.Font       = AppTheme.FontBold;
            g.ColumnHeadersDefaultCellStyle.Padding    = new Padding(8, 0, 0, 0);
            g.ColumnHeadersHeight = 40;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.RowTemplate.Height = 34;
            g.DefaultCellStyle.Font              = AppTheme.FontNormal;
            g.DefaultCellStyle.SelectionBackColor = AppTheme.Accent;
            g.DefaultCellStyle.SelectionForeColor = Color.White;
            g.DefaultCellStyle.Padding            = new Padding(6, 0, 0, 0);
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 250, 249);
            g.GridColor       = AppTheme.Border;
            g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            g.BackgroundColor = Color.White;
            g.BorderStyle     = BorderStyle.None;
            g.ReadOnly        = true;
            g.AllowUserToAddRows    = false;
            g.AllowUserToDeleteRows = false;
            g.AllowUserToResizeRows = false;
            g.RowHeadersVisible     = false;
            g.SelectionMode         = DataGridViewSelectionMode.FullRowSelect;
            g.MultiSelect           = false;
        }

        public static Button MakeButton(string text, Color back, int width = 110, int height = 36)
        {
            var b = new Button
            {
                Text      = text,
                Width     = width,
                Height    = height,
                BackColor = back,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = AppTheme.FontBold,
                Cursor    = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter,
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(back, 0.12f);
            return b;
        }

        public static Label MakeLabel(string text, Font font, Color color,
            ContentAlignment align = ContentAlignment.TopLeft) => new Label
        {
            Text      = text,
            Font      = font,
            ForeColor = color,
            AutoSize  = false,
            TextAlign = align,
        };

        public static TextBox MakeTextBox(int width = 260) => new TextBox
        {
            Width       = width,
            Font        = AppTheme.FontNormal,
            BorderStyle = BorderStyle.FixedSingle,
        };

        public static ComboBox MakeCombo(int width = 260,
            ComboBoxStyle style = ComboBoxStyle.DropDownList) => new ComboBox
        {
            Width         = width,
            Font          = AppTheme.FontNormal,
            DropDownStyle = style,
        };
    }
}
