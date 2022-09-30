using AutoresApi.Servicios;
using AutoresApi.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

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
            services.AddControllers().AddNewtonsoftJson();
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
                s.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApiAutores", Version = "v1" });
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
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders();
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
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors();

            app.UseHttpsRedirection();

            app.UseAuthorization();
        }
    }
}
