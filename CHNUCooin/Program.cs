using CHNUCooin.Database;
using CHNUCooin.Services;
using CHNUCooin.Services.Background;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddControllers();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TransactionService>();
builder.Services.AddDbContext<ApplicationContext>(options =>
            options.UseNpgsql("User ID=postgres;Password=admin;Host=localhost;Port=5432;Database=database;Pooling=true;"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddHostedService(x =>
{
    var options = new DbContextOptionsBuilder<ApplicationContext>();
    options.UseNpgsql("User ID=postgres;Password=admin;Host=localhost;Port=5432;Database=database;Pooling=true;");
    var context = new ApplicationContext(options.Options);

    return new ProcessTransactionBackgroundService(context);

});
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

app.Run();
