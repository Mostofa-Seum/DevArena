using DevArena.Data;
using DevArena.Repos;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DevArenaDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DevArenaContext")));
builder.Services.AddScoped<ContestsRepo>();
builder.Services.AddScoped<HostRepo>();
builder.Services.AddScoped<ParticipantsRepo>();
builder.Services.AddScoped<ContestRegistrationRepo>();
builder.Services.AddScoped<ProblemsRepo>();
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

app.Run();
