using DevArena.Data;
using DevArena.Repos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Cookie Authentication specifically tailored for an API
builder.Services.AddAuthentication("DAApiAuth")
    .AddCookie("DAApiAuth", options =>
    {
        options.Cookie.Name = "DevArena.ApiCookie";
        // Prevent redirecting to a login page (which APIs shouldn't do)
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    });

builder.Services.AddControllers();
builder.Services.AddDbContext<DevArenaDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DevArenaContext")));

// Register Repos
builder.Services.AddScoped<ContestsRepo>();
builder.Services.AddScoped<HostRepo>();
builder.Services.AddScoped<ParticipantsRepo>();
builder.Services.AddScoped<ContestRegistrationRepo>();
builder.Services.AddScoped<ProblemsRepo>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<DevArena.Shared.CurrentUserHelper>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 2. Add Authentication middleware BEFORE Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();