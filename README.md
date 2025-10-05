# kitchen

# Миграции

``
dotnet ef migrations add Init --startup-project ./Product.Api --project ./Product.Infrastructure --output-dir ./Adapters/Postgres/Migrations

dotnet ef migrations remove --startup-project ./Product.Api --project ./Product.Infrastructure --output-dir ./Adapters/Postgres/Migrations

dotnet ef database update --startup-project ./Product.Api --connection "Server=localhost;Port=5432;User Id=username;Password=secret;Database=products;"
```
