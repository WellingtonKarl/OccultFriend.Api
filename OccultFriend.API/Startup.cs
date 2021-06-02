using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using OccultFriend.Domain.IRepositories;
using OccultFriend.Domain.Model;
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

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddScoped<IEmailSettingService, EmailSettingService>(_ => new EmailSettingService(Configuration.GetSection("EmailSettings").Get<EmailSetting>()));
            services.AddScoped<IRepositoriesFriend, RepositoriesFriend>(_ => new RepositoriesFriend(new SqlConnection(Configuration.GetConnectionString("connection"))));
            services.AddScoped<IEmailService, EmailServices>();
            services.AddScoped<IServicesFriend, ServicesFriend>();
            services.AddScoped<IEmailTemplate, EmailTemplate>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "API Amigo Oculto",
                        Version = "v1",
                        Description = "Uma aplicação para automatizar o sorteio do amigo aculto",
                        Contact = new OpenApiContact
                        {
                            Name = "Wellington Karl",
                            Url = new Uri("https://github.com/WellingtonKarl")
                        }

                    });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Aplicação Amigo Oculto"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
