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
using ModuloNet.Api.Auth;
using ModuloNet.Infrastructure.Auth.ExternalAuth;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using ModuloNet.Application.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<ExternalAuthRedirectOptions>(builder.Configuration.GetSection("ExternalAuth:Redirect"));

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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "CourseMrThanh.Default.Key.Minimum32Characters!"))
        };
    })
    .AddCookie("External", o => o.ExpireTimeSpan = TimeSpan.FromMinutes(5))
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        var cfg = builder.Configuration.GetSection(ExternalAuthOptions.SectionName + ":Google");
        options.ClientId = cfg["ClientId"] ?? "";
        options.ClientSecret = cfg["ClientSecret"] ?? "";
        options.SignInScheme = "External";
        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    })
    .AddFacebook(FacebookDefaults.AuthenticationScheme, options =>
    {
        var cfg = builder.Configuration.GetSection(ExternalAuthOptions.SectionName + ":Facebook");
        options.AppId = cfg["AppId"] ?? "";
        options.AppSecret = cfg["AppSecret"] ?? "";
        options.SignInScheme = "External";
        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy(AuthPolicies.AdminOnly, p => p.RequireRole(AuthRoles.Admin));
    o.AddPolicy(AuthPolicies.ParentOnly, p => p.RequireRole(AuthRoles.Parent));
    o.AddPolicy(AuthPolicies.StudentOnly, p => p.RequireRole(AuthRoles.Student));
    o.AddPolicy(AuthPolicies.ParentOrAdmin, p => p.RequireRole(AuthRoles.Admin, AuthRoles.Parent));
});

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
