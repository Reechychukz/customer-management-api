# 🥇Customer Management Web API

This project is built with ASP.NET Core(.Net7) DDD/Clean Architecture


## Testing it out
1. Clone or download this repository
2. Update appsettings.Development.json or appsettings.json with your MSSQL connectionn string, and also the JWTSettings to suit your preference.
3. Cd into src/
4. Run migration using dotnet ef migrations add [MigrationName] --project Infrastructure.Data --startup-project API
5. Update database using dotnet ef database update --project Infrastructure.Data --startup-project API
6. Build the solution using command line with `dotnet build`
7. cd to **Api** directory and run project using command line with `dotnet run`
8. Browse to this url http://localhost:5017/swagger/ to see SwaggerUI page

![alt text](<Swagger UI.png>)
