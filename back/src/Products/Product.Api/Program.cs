using CSharpFunctionalExtensions;
using ItemProductss.Infrastructure.Adapters.Postgres;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Primitives;
using Products.Core.Application.UseCases.Commands.AddProduct;
using Products.Core.Application.UseCases.Commands.ArchiveProduct;
using Products.Core.Application.UseCases.Commands.UnArchiveProduct;
using Products.Core.Application.UseCases.Query.GetAllProducts;
using Products.Core.Application.UseCases.Query.GetProduct;
using Products.Core.Ports;
using Products.Infrastructure.Adapters.Postgres;
using Products.Infrastructure.Adapters.Postgres.Repositories;
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

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString,
        sqlOptions => { sqlOptions.MigrationsAssembly("Products.Infrastructure"); });
    options.EnableSensitiveDataLogging();
});

// UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Mediator
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

#region Commands
builder.Services.AddScoped<IRequestHandler<AddProductCommand, UnitResult<Error>>, AddProductCommandHandler>();
builder.Services.AddScoped<IRequestHandler<ArchiveProductCommand, UnitResult<Error>>, ArchiveProductCommandHandler>();
builder.Services.AddScoped<IRequestHandler<UnArchiveProductCommand, UnitResult<Error>>, UnArchiveProductCommandHandler>();
#endregion Commands

#region Queries
builder.Services.AddScoped<IRequestHandler<GetAllProductsQuery, Maybe<GetAllProductsResponse>>, GetAllProductsQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetProductQuery, Maybe<GetProductResponse>>, GetProductQueryHandler>();
#endregion Queries

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(opts =>
{
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
