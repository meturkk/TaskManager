# Task Manager Web API

This is a simple Task Management Web API built with **ASP.NET Core**. It allows users to register, log in (with JWT), and manage their tasks — including grouping tasks as **daily**, **weekly**, and **monthly**.

## 🚀 Features

- User registration and login with JWT authentication
- Create, read, update, and delete tasks
- Filter tasks as daily, weekly, or monthly
- Mark tasks as completed
- Swagger UI for easy API testing

## 🛠️ Technologies

- ASP.NET Core 7
- Entity Framework Core
- SQL Server
- JWT Authentication
- Swagger / Swashbuckle

## 📁 Project Structure

📦GorevYoneticiAPI
 ┣ 📂Controllers
 ┣ 📂Data
 ┣ 📂DTOs
 ┣ 📂Migrations
 ┣ 📂Models
 ┣ 📂Services
 ┣ 📄Program.cs
 ┣ 📄appsettings.json
 ┣ 📄README.md


## ⚙️ Getting Started

### 1. Clone the repository

2. Update your connection string
Open appsettings.json and modify the DefaultConnection to point to your SQL Server instance.

3. Run database migrations
```bash: dotnet ef database update
Make sure dotnet-ef is installed globally:
dotnet tool install --global dotnet-ef

4. Run the application

```bash: dotnet run
Visit Swagger UI: https://localhost:7274/swagger

🔐 How to Authenticate
Register a user via POST /api/Auth/register

Login via POST /api/Auth/login and copy the token

Click Authorize in Swagger and paste:
Bearer YOUR_TOKEN_HERE

📌 Notes
This project is backend-only. A separate frontend can be developed using HTML + JavaScript, React, or any other framework.

You can group tasks based on their due dates using GET /api/tasks?filter=daily|weekly|monthly.

📃 License
This project is licensed under the MIT License.