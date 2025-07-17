## Install The EF Core (CLI) tools
``dotnet tool install --global dotnet-ef``

## Create your database and schema if not exist or updates your database to the latest migration
``dotnet ef database update``

## Example create your first migration
``dotnet ef migrations add InitialCreate``