# Employee Management System (EMS Pro)
### C# · WinForms · SQLite · .NET 8 · Desktop Application
---
## ⚡ Quick Start

### Requirements
- **Visual Studio 2022** (Community or higher)
- **.NET 8 SDK** — https://dotnet.microsoft.com/download
- **.NET Desktop Development** workload must be installed in VS

### Steps
1. Extract the `.zip` file to any folder
2. Open `EmployeeManagementSystem.sln` in **Visual Studio**
3. On first build, VS automatically restores the **NuGet package**
   (Microsoft.Data.Sqlite — no server installation required)
4. Press **F5** or click **Run**
5. Login: `admin` / `admin@123`

> **Database** — an `ems.db` file is automatically created in the same folder as the app.
> On first run, 12 sample employees are also added automatically.

---
## 🖥️ Features

| Module | Description |
|---|---|
| **Login** | Branded two-panel login, Enter key support |
| **Dashboard** | 4 stat cards + live GDI+ horizontal bar chart |
| **Employees** | Add, Edit, Delete, Search, Filter by dept, CSV Export |
| **Reports** | Department-wise summary with salary totals |
| **Change Password** | Accessible from sidebar |
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
| Password | TEXT | Plain text (hash in production) |
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
## 🔄 Switching to SQL Server
If your course requires SQL Server:
1. Run `Database/schema_sqlserver.sql` in your SQL Server
2. Replace `Microsoft.Data.Sqlite` NuGet with `Microsoft.Data.SqlClient`
3. In `Db.cs`:
   - `SqliteConnection` → `SqlConnection`
   - `SqliteCommand` → `SqlCommand`
   - `ConnStr` → `"Server=(localdb)\\MSSQLLocalDB;Database=EMS_DB;Integrated Security=true"`

---
## 🐛 Common Issues

**"Unable to load DLL" error**
→ Build → Clean Solution → Rebuild Solution
→ Or set Platform target to x64 in Project properties

**Login not working**
→ Delete `ems.db` and rerun the app (fresh seed)

**NuGet restore failing**
→ Tools → NuGet Package Manager → Restore NuGet Packages

---
## 👤 Default Login
```
Username : admin
Password : admin@123
```
---
*Built with ❤️ using C# WinForms — No external web API or framework used*
