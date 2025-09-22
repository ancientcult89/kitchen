# kitchen

# Миграции
```
dotnet ef migrations add Init --startup-project ./DeliveryApp.Api --project ./DeliveryApp.Infrastructure --output-dir ./Adapters/Postgres/Migrations
dotnet ef database update --startup-project ./DeliveryApp.Api --connection "Server=localhost;Port=5432;User Id=username;Password=secret;Database=delivery;"
```