using CSharpFunctionalExtensions;
using Items.Core.Application.UseCases.Commands.AddItem;
using Items.Core.Ports;
using Items.Infrastructure.Adapters.Postgres;
using Items.Infrastructure.Adapters.Postgres.Repositories;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Primitives;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Health Checks
builder.Services.AddHealthChecks();

// Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin(); // Не делайте так в проде!
        });
});

// БД, ORM 
var connectionString = builder.Configuration["CONNECTION_STRING"];

builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseNpgsql(connectionString,
        sqlOptions => { sqlOptions.MigrationsAssembly("Items.Infrastructure"); });
    options.EnableSensitiveDataLogging();
});

// UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Repositories
builder.Services.AddScoped<IItemRepository, ItemRepository>();

// Mediator
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

#region Commands
builder.Services.AddScoped<IRequestHandler<AddItemCommand, UnitResult<Error>>, AddItemCommandHandler>();
#endregion Commands

#region Queries

#endregion Queries

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(opts => {
    opts.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standart Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    opts.OperationFilter<SecurityRequirementsOperationFilter>();
});


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseRouting();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseCors();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

// Apply Migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();
