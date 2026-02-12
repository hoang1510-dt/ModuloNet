using FluentValidation;
using ModuloNet.Application;
using ModuloNet.Application.Features.Courses.Create;
using ModuloNet.Application.Features.Courses.Delete;
using ModuloNet.Application.Features.Courses.GetById;
using ModuloNet.Api.Middlewares;
using ModuloNet.Api.Swagger;
using ModuloNet.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "ModuloNet.Default.Key.Minimum32Characters!"))
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(SwaggerConfig.Configure);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ModuloNet API v1"));
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapCreateCourse();
app.MapGetCourseById();
app.MapDeleteCourse();

app.Run();
