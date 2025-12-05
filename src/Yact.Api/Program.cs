using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Yact.Application.Handlers.Activities.GetActivities;
using Yact.Application.Handlers.Activities.GetActivitiesById;
using Yact.Application.Handlers.Activities.UploadActivity;
using Yact.Application.Interfaces;
using Yact.Application.Mapping;
using Yact.Domain.Repositories;
using Yact.Infrastructure.Configuration;
using Yact.Infrastructure.Data;
using Yact.Infrastructure.Repositories;
using Yact.Infrastructure.Services.ActivityReader;
using Yact.Infrastructure.Services.FileStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Db context
builder.Services.AddDbContext<AppDbContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Automapper
IMapper mapper = ActivityMapper.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(UploadActivityHandler).Assembly));

// Repositories
builder.Services.AddScoped<IActivityRepository, ActivityRepository>();

// Storage
builder.Services.Configure<FileStorageConfiguration>(
    builder.Configuration.GetSection("FileStorage"));
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

// Read service
builder.Services.AddScoped<IActivityReaderService, ActivityReaderService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
ApplyMigration();
app.Run();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (dbContext.Database.GetPendingMigrations().Count() > 0)
        {
            dbContext.Database.Migrate();
        }
    }
}