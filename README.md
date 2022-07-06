# Project "Mateusz" application for SQL database management
Project "Mateusz" was created to provide user with a web application that allows him to modify his SQL database tables. Application provides user authentication and admin panel for Root user (user with "Administrator" role). User can perform CRUD action on database tables, and Root user can also access admin panel that lets him manage application users.

## General information
The main task of this project is to provide user with easy to understand interface for performing CRUD operations on "czytania" SQL database. Web application provides user authentication(Identity ASP.NET Core) for security and allows logged in user to perform CRUD operations on the db tables content. The Root user with "Administrator" role has access to extra admin panel that lets him manage application users. 
Application was created to provide user with a new web interface for his SQL db tables operations.

## Technologies used
- C# 
- ASP.NET CORE MVC
- Identity ASP.NET Core
- Entity Framework Core
- SQL
- Ajax

## Features
- User authentication
- SQL db CRUD operations
- Administator user management

## Usage
- Aplication was created using MVC. Each db table has 4 corresponding views Index, Add, Edit, Delete and one partial view _ViewNameDisplay (partial view is used in Index for refreshing data without reloading whole page).
- Each table has a assigned controller with corresponding name that handles all the operation.
- Areas folder contains a scaffolding of Identity ASP.NET CORE with slight modifications do to application needs.
- Models are defined in Models folder and their names correspond to their purpose.
- Connection strings are defined in appsettings.json and then assigned in Program.cs

## Project status
Current on hold. Main features are complete.

