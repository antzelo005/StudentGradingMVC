# Student Grading MVC

Student Grading MVC is an ASP.NET Core MVC web application for managing academic grading workflows across three roles: students, professors, and secretariat staff. The project uses Entity Framework Core with SQL Server LocalDB and includes sample seeded data for local development.

## Overview

The application is organized around role-based access:

- Students can review grades by course, by semester, and as an overall average.
- Professors can view assigned courses and submit or update student grades.
- Secretariat staff can manage courses, professor assignments, and student enrollments.

The default route opens the login page, and access to each area is enforced through a role guard based on authentication cookies.

## Tech Stack

- ASP.NET Core MVC
- .NET 8
- Entity Framework Core 8
- SQL Server LocalDB
- Bootstrap

## Key Features

- Role-based login flow for `Student`, `Professor`, and `Secretary`
- Grade visibility for students across multiple views
- Grade entry workflow for professors by assigned course
- Course creation and professor assignment for secretariat staff
- Enrollment management for student-course declarations
- Seeded development data for fast local setup

## Project Structure

```text
StudentGradingMVC/
|-- Controllers/
|-- Data/
|-- Filters/
|-- Migrations/
|-- Models/
|-- Properties/
|-- Views/
|-- wwwroot/
|-- Program.cs
|-- appsettings.json
`-- StudentGradingMVC.csproj
```

## Prerequisites

Before running the project, make sure you have:

- .NET 8 SDK
- SQL Server LocalDB installed on Windows
- `dotnet-ef` tool installed globally

Install Entity Framework CLI if needed:

```powershell
dotnet tool install --global dotnet-ef
```

## Getting Started

From the project directory:

```powershell
cd c:\AntzeloProjects\StudentGradingMVC\StudentGradingMVC
dotnet restore
dotnet ef database update
dotnet run
```

When the application starts, it should listen on:

- `https://localhost:7068`
- `http://localhost:5077`

Use the HTTPS URL first. If the browser asks you to trust the local development certificate, accept it.

## Configuration

The application uses the following connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "StudentGradingDB": "Server=(localdb)\\MSSQLLocalDB;Database=StudentGradingDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

If LocalDB is not available on your machine, update the connection string to point to a SQL Server instance you control.

## Seeded Accounts

The application seeds a small dataset on startup for development use.

| Username | Password | Role |
|---|---|---|
| `stud1` | `1234` | `Student` |
| `stud2` | `1234` | `Student` |
| `prof1` | `1234` | `Professor` |
| `sec1` | `1234` | `Secretary` |

## Screenshots


- Login page with role selector and branded header
  <img width="1609" height="814" alt="image" src="https://github.com/user-attachments/assets/e02c0a58-a389-4d6d-844d-633b8b61df8b" />

- Professor grade entry screen showing a course roster and editable grade fields)
  <img width="1840" height="720" alt="image" src="https://github.com/user-attachments/assets/015cd048-19fc-4e0b-9800-d7554dd8ac2f" />

- Secretary dashboard or courses screen showing course management actions
  <img width="1854" height="912" alt="image" src="https://github.com/user-attachments/assets/35aabf07-3654-40bd-aed0-b55bd91f978e" />

## Notes

- The database is seeded on application startup if core tables are empty.
- Authentication is cookie-based and lightweight, intended for educational or local project use.
- The default landing page is the login screen.

## Future Improvements

- Replace plain-text passwords with a secure authentication flow
- Add authorization policies and server-side identity management
- Add automated tests for controller actions and grade workflows
- Add audit history for grade changes and administrative actions

## License

This project is currently provided without a defined license. Add one if the repository is intended for public distribution.
