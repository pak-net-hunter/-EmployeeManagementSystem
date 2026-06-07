# Employee Management System (EMS Pro)
### C# · WinForms · SQLite · .NET 8 · Desktop Application

---

## ⚡ Quick Start (Roman Urdu)

### Zaroori cheezein
- **Visual Studio 2022** (Community ya upar)
- **.NET 8 SDK** — https://dotnet.microsoft.com/download
- VS mein **.NET Desktop Development** workload install hona chahiye

### Steps
1. `.zip` extract karein kisi folder mein
2. `EmployeeManagementSystem.sln` ko **Visual Studio** mein open karein
3. Pehli baar build karne par VS automatically **NuGet package** restore karta hai
   (Microsoft.Data.Sqlite — koi server install nahi karna)
4. **F5** dabayein ya **Run** karein
5. Login: `admin` / `admin@123`

> **Database** app ki same folder mein `ems.db` file automatically ban jati hai.
> Pehli baar chalane par 12 sample employees bhi automatically add ho jaate hain.

---

## 🖥️ Features

| Module | Kya milega |
|---|---|
| **Login** | Branded two-panel login, Enter key support |
| **Dashboard** | 4 stat cards + live GDI+ horizontal bar chart |
| **Employees** | Add, Edit, Delete, Search, Filter by dept, CSV Export |
| **Reports** | Department-wise summary with salary totals |
| **Change Password** | Sidebar se accessible |
| **Status Bar** | Live clock + status messages |

---

## 🎨 Tech Stack

```
Language  : C# (.NET 8)
Framework : Windows Forms (WinForms)
Database  : SQLite (file-based, zero server setup)
DB Access : ADO.NET via Microsoft.Data.Sqlite NuGet
Pattern   : No web API, No Entity Framework, No MVC
```

---

## 📁 Project Structure

```
EmployeeManagementSystem/
├── Program.cs                  ← Entry point
├── AppTheme.cs                 ← All colors & fonts
├── UiHelper.cs                 ← Shared UI helpers
├── Models/
│   └── Employee.cs             ← Employee data model
├── Database/
│   ├── Db.cs                   ← All DB access (CRUD + stats)
│   └── schema_sqlserver.sql    ← SQL Server script (if needed)
└── Forms/
    ├── LoginForm.cs            ← Login screen
    ├── MainForm.cs             ← Main shell (sidebar + header)
    ├── DashboardControl.cs     ← Dashboard with chart
    ├── EmployeeControl.cs      ← Employee grid + CRUD
    ├── AddEditEmployeeForm.cs  ← Add/Edit dialog
    ├── ReportsControl.cs       ← Department reports
    └── ChangePasswordForm.cs   ← Password change dialog
```

---

## 🗄️ Database Tables

### Users
| Column | Type | Notes |
|---|---|---|
| Id | INTEGER | Auto PK |
| Username | TEXT | Unique |
| Password | TEXT | Plain text (production mein hash karein) |
| Role | TEXT | Default: admin |

### Employees
| Column | Type |
|---|---|
| Id, FullName, Email, Phone | TEXT / INTEGER |
| Department, Designation | TEXT |
| Salary | REAL |
| Gender, JoiningDate, Status | TEXT |
| Address, Notes | TEXT |

---

## 🔄 SQL Server par switch karna

Agar aapke course mein SQL Server zaroori hai:

1. `Database/schema_sqlserver.sql` apne SQL Server mein run karein
2. `Microsoft.Data.Sqlite` NuGet ki jagah `Microsoft.Data.SqlClient` install karein
3. `Db.cs` mein:
   - `SqliteConnection` → `SqlConnection`
   - `SqliteCommand` → `SqlCommand`
   - `ConnStr` → `"Server=(localdb)\\MSSQLLocalDB;Database=EMS_DB;Integrated Security=true"`

---

## 🐛 Common Issues

**"Unable to load DLL" error**
→ Build → Clean Solution → Rebuild Solution karein
→ Ya Project properties mein Platform target x64 set karein

**Login kaam nahi karta**
→ `ems.db` delete karein, app dobara chalayein (fresh seed)

**NuGet restore nahi hota**
→ Tools → NuGet Package Manager → Restore NuGet Packages

---

## 👤 Default Login
```
Username : admin
Password : admin@123
```

---

*Built with ❤️ using C# WinForms — No external web API or framework used*
