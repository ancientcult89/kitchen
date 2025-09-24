# kitchen

# Миграции

```
dotnet ef migrations add MeasureType --startup-project ./Items.Api --project ./Items.Infrastructure --output-dir ./Adapters/Postgres/Migrations
dotnet ef database update --startup-project ./DeliveryApp.Api --connection "Server=localhost;Port=5432;User Id=username;Password=secret;Database=delivery;"
```
