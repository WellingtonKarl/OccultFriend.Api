﻿using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OccultFriend.Domain.IRepositories;
using OccultFriend.Domain.Model;
using OccultFriend.Repository.MongoRepository;
using OccultFriend.Repository.Repositories;
using OccultFriend.Service.EmailService;
using OccultFriend.Service.FriendServices;
using OccultFriend.Service.Interfaces;

namespace OccultFriend.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #region Properties

        private static string MyReposytories => "https://github.com/WellingtonKarl";
        private static string Name => "Wellington Karl";
        private static string Description => "Uma aplicação para automatizar o sorteio do amigo aculto";
        private static string Version => "v1";
        private static string TitleProject => "API Amigo Oculto";
        private static string NameProject => "Aplicação Amigo Oculto";


        public IConfiguration Configuration { get; }

        #endregion

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionsStringsSqlServer = Configuration.GetConnectionString("connection");

            services.AddCors();
            services.AddControllers();

            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("SettingsJWT").GetValue<string>("Secret"));

            services.AddAuthentication(a => 
            {
                a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(j => 
            {
                j.RequireHttpsMetadata = false;
                j.SaveToken = true;
                j.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddScoped<ITokenService, TokenService>(_ => new TokenService(Configuration.GetSection("SettingsJWT").GetValue<string>("Secret")));

            services.AddScoped<IEmailSettingService, EmailSettingService>(_ => new EmailSettingService(Configuration.GetSection("EmailSettings").Get<EmailSetting>()));

            if (!string.IsNullOrWhiteSpace(connectionsStringsSqlServer))
                services.AddScoped<IRepositoriesFriend, RepositoriesFriend>(_ => new RepositoriesFriend(new SqlConnection(connectionsStringsSqlServer)));
            else
                services.AddScoped<IRepositoriesFriend, FriendRepository>(_ => new FriendRepository(Configuration.GetSection("HostMongoConnection").Get<HostMongoConnection>()));

            services.AddScoped<IEmailService, EmailServices>();
            services.AddScoped<IServicesFriend, ServicesFriend>();
            services.AddScoped<IEmailTemplate, EmailTemplate>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(Version,
                    new OpenApiInfo
                    {
                        Title = TitleProject,
                        Version = Version,
                        Description = Description,
                        Contact = new OpenApiContact
                        {
                            Name = Name,
                            Url = new Uri(MyReposytories)
                        }

                    });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", NameProject));
            }

            app.UseCors(c => 
            {
                c.AllowAnyOrigin();
                c.AllowAnyMethod();
                c.AllowAnyHeader();
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
