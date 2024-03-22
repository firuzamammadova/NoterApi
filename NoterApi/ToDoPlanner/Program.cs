using Core.Models;
using FluentMigrator.Runner;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository.CQRS.Commands;
using Repository.CQRS.Queries;
using Repository.Identity;
using Repository.Infrastructure;
using Repository.Repositories;
using Service.Services;
using Services.Service;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("EnableCORS", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddTransient<IAppUserStore<User>, UserStore>();
builder.Services.AddTransient<Microsoft.AspNetCore.Identity.IUserStore<User>, UserStore>();

builder.Services.AddTransient<IAppRoleStore<Role>, RoleStore>();
builder.Services.AddTransient<Microsoft.AspNetCore.Identity.IRoleStore<Role>, RoleStore>();
builder.Services.AddIdentity<User, Role>(opts =>
{
    opts.Password.RequireDigit = true;
    opts.Password.RequireLowercase = false;
    opts.Password.RequireUppercase = false;
    opts.Password.RequireNonAlphanumeric = false;
    opts.Password.RequiredLength = 8;
}).AddUserManager<UserManager>()
            .AddRoleManager<RoleManager>()
                .AddDefaultTokenProviders();

builder.Services.AddAuthentication(opts =>
{
    opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
   .AddJwtBearer(cfg =>
   {
       cfg.RequireHttpsMetadata = false;
       cfg.SaveToken = true;
       cfg.TokenValidationParameters = new TokenValidationParameters()
       {
           // standard configuration
           ValidIssuer = builder.Configuration["Auth:Jwt:Issuer"],
           IssuerSigningKey = new SymmetricSecurityKey(
               Encoding.UTF8.GetBytes(builder.Configuration["Auth:Jwt:Key"])),
           ValidAudience = builder.Configuration["Auth:Jwt:Audience"],
           ClockSkew = TimeSpan.Zero,
           // security switches
           RequireExpirationTime = true,
           ValidateIssuer = true,
           ValidateIssuerSigningKey = true,
           ValidateAudience = true
       };
       cfg.IncludeErrorDetails = true;
       cfg.Events = new JwtBearerEvents
       {
           OnMessageReceived = context =>
           {
               var accessToken = context.Request.Headers["Authorization"];

               var path = context.HttpContext.Request.Path;
               if ((path.StartsWithSegments("/hubs/operation")))
               {
                   accessToken = context.Request.Query["access_token"];
                   context.Token = accessToken;
               }
               return Task.CompletedTask;
           },
           OnAuthenticationFailed = context =>
           {
               var te = context.Exception;
               return Task.CompletedTask;
           },
           OnTokenValidated = context =>
           {

               return Task.CompletedTask;
           }
       };
   }).Services.ConfigureApplicationCookie(
       options =>
       {
           options.ExpireTimeSpan = TimeSpan.FromMinutes(3600);
       });
// Add builder.Services to the container.
builder.Services.AddScoped<IFileTypeRepository, FileTypeRepository>();
builder.Services.AddScoped<IFileTypeQuery, FileTypeQuery>();
builder.Services.AddScoped<IFileTypeCommand, FileTypeCommand>();

builder.Services.AddScoped<IFileTypeService, FileTypeService>();



builder.Services.AddScoped<IRecordFileRepository, RecordFileRepository>();
builder.Services.AddScoped<IRecordFileQuery, RecordFileQuery>();
builder.Services.AddScoped<IRecordFileCommand, RecordFileCommand>();

builder.Services.AddScoped<IRecordFileService, RecordFileService>();


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ITokenQuery, TokenQuery>();
builder.Services.AddScoped<ITokenCommand, TokenCommand>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();





builder.Services.AddScoped<Database>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pharmacy_API", Version = "v1" });

    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securitySchema);

    var securityRequirement = new OpenApiSecurityRequirement();
    securityRequirement.Add(securitySchema, new[] { "Bearer" });
    c.AddSecurityRequirement(securityRequirement);
});
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
//app.MigrateDatabase();
app.Run();
