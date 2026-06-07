using System.Drawing;

namespace EmployeeManagementSystem
{
    public static class AppTheme
    {
        // Core palette
        public static readonly Color Sidebar     = Color.FromArgb(26,  47,  75);
        public static readonly Color Header      = Color.FromArgb(30,  55,  88);
        public static readonly Color Accent      = Color.FromArgb(0,  137, 123);
        public static readonly Color AccentDark  = Color.FromArgb(0,  105,  92);
        public static readonly Color AccentLight = Color.FromArgb(224, 242, 241);
        public static readonly Color Background  = Color.FromArgb(240, 242, 245);
        public static readonly Color Card        = Color.White;
        public static readonly Color TextPrimary   = Color.FromArgb(26,  47,  75);
        public static readonly Color TextSecondary = Color.FromArgb(84, 110, 122);
        public static readonly Color Border      = Color.FromArgb(207, 216, 220);

        // Status colours
        public static readonly Color Active   = Color.FromArgb(67,  160,  71);
        public static readonly Color Inactive = Color.FromArgb(229,  57,  53);
        public static readonly Color OnLeave  = Color.FromArgb(251, 140,   0);

        // Card accent bars
        public static readonly Color CardTeal   = Color.FromArgb(0,  137, 123);
        public static readonly Color CardBlue   = Color.FromArgb(30, 136, 229);
        public static readonly Color CardGreen  = Color.FromArgb(67, 160,  71);
        public static readonly Color CardOrange = Color.FromArgb(251,140,   0);

        // Chart bar cycle
        public static readonly Color[] ChartBars =
        {
            Color.FromArgb(0,  137, 123), Color.FromArgb(30, 136, 229),
            Color.FromArgb(171, 71, 188), Color.FromArgb(251,140,   0),
            Color.FromArgb(67, 160,  71), Color.FromArgb(229, 57,  53),
            Color.FromArgb(0,  188, 212), Color.FromArgb(255,179,   0),
        };

        // Fonts
        public static readonly Font FontSmall  = new Font("Segoe UI",  9.0F);
        public static readonly Font FontNormal = new Font("Segoe UI", 10.0F);
        public static readonly Font FontBold   = new Font("Segoe UI", 10.0F, FontStyle.Bold);
        public static readonly Font FontLarge  = new Font("Segoe UI", 12.0F, FontStyle.Bold);
        public static readonly Font FontHuge   = new Font("Segoe UI", 22.0F, FontStyle.Bold);
        public static readonly Font FontTitle  = new Font("Segoe UI", 18.0F, FontStyle.Bold);
    }
}
