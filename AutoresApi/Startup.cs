using AutoresApi.Servicios;
using AutoresApi.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace AutoresApi
{
    public class Startup
    {
        private object configuration;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration _configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = _configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(opciones =>
            {
                opciones.Conventions.Add(new SwaggerAgrupaVersion());
            }).AddNewtonsoftJson();
            services.AddDbContext<AplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"))
            );
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, 
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["LlaveJwt"])),
                    ClockSkew = TimeSpan.Zero
                });
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "WebApiAutores", 
                    Version = "v1",
                    Description = "WebApi para autores y lirbos",
                    Contact = new OpenApiContact
                    {
                        Email = "i.licona@gmail.com",
                        Name = "Jose Ignacio Licona",
                    }
                });
                s.SwaggerDoc("v2", new OpenApiInfo { 
                    Title = "WebApiAutores", 
                    Version = "v2",
                    Description = "WebApi para autores y lirbos",
                    Contact = new OpenApiContact
                    {
                        Email = "i.licona@gmail.com",
                        Name = "Jose Ignacio Licona",
                    }
                });
                s.OperationFilter<AgregarParametroHATEOAS>();
                s.OperationFilter<AgregarParametroXVersion>();
                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });
                s.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                         new OpenApiSecurityScheme
                         {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id =  "Bearer"
                            }
                         },
                         new string[]{}
                    }
                });
            });
            /* configuracion de automapper*/
            services.AddAutoMapper(typeof(Startup));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddAuthorization(opciones =>
            {
                opciones.AddPolicy("Es admin", politica => politica.RequireClaim("esAdmin"));
                opciones.AddPolicy("Es vendedor", politica => politica.RequireClaim("esVendedor"));
            });
            services.AddDataProtection();
            services.AddTransient<HashService>();
            services.AddCors(opciones =>
            {
                opciones.AddDefaultPolicy(builder =>
                {
                    //allow any origin
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders(new string[] { "CantidadTotalRegistros" });
                    //allow specific origin
                    //builder.WithOrigins("https://www.apirequest.io").AllowAnyMethod().AllowAnyHeader().WithExposedHeaders();
                });
            });

            services.AddTransient<GeneradorEnlaces>();
            services.AddTransient<HATEOASAuthorFilterAttribute>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                
            }

            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiAutores v1");
                s.SwaggerEndpoint("/swagger/v2/swagger.json", "WebApiAutores v2");
            });

            app.UseCors();

            app.UseHttpsRedirection();

            app.UseAuthorization();
        }
    }
}
