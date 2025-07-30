//using AuthenticationAuthorization.Context;
//using System.Text;
//using ChatWebApp.Context;
//using Microsoft.EntityFrameworkCore;
//using ChatWebApp.ServicesInterface;
//using ChatWebApp.ServiceImplementation;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using ChatWebApp.Hubs;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//// Configure DB context
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<MyDBConext>(options =>
//    options.UseSqlServer(connectionString));

//// Add Scoped Services
//builder.Services.AddScoped<JwtTokenGenerator>();
//builder.Services.AddTransient<MessageInterface, messageImplementation>();
//builder.Services.AddTransient<RegistrationInterface, RegistrationImplementation>();

//// Configure JWT Authentication
//var jwtSettings = builder.Configuration.GetSection("Jwt");
//var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = jwtSettings["Issuer"],
//        ValidAudience = jwtSettings["Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
//    };
//});

//// ? Configure CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", builder =>
//    {
//        builder.AllowAnyOrigin()
//               .AllowAnyMethod()
//               .AllowAnyHeader();
//    });
//});



//builder.Services.AddSignalR(); // ?? Add SignalR
//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(policy =>
//    {
//        policy.AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials()
//              .WithOrigins("http://localhost:4200"); // Angular frontend
//    });
//});

//var app = builder.Build();

//// Configure middleware
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//// ? Use CORS middleware
//app.UseCors("AllowAll");

//app.UseAuthentication(); // Make sure this comes before UseAuthorization
//app.UseAuthorization();

//app.MapControllers();
//app.MapHub<ChatHub>("/chatHub");
//app.Run();



using AuthenticationAuthorization.Context;
using System.Text;
using ChatWebApp.Context;
using Microsoft.EntityFrameworkCore;
using ChatWebApp.ServicesInterface;
using ChatWebApp.ServiceImplementation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ChatWebApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Add Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MyDBConext>(options =>
    options.UseSqlServer(connectionString));

// 3. Add Scoped & Transient Services
builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddTransient<MessageInterface, messageImplementation>();
builder.Services.AddTransient<RegistrationInterface, RegistrationImplementation>();

// 4. JWT Authentication Configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
    };
});

// 5. CORS for Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// 6. Add SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// 7. Middleware Configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

// Order is important!
app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();

// 8. Map Endpoints
app.MapControllers();
app.MapHub<ChatHub>("/chatHub");

app.Run();
