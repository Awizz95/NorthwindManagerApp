using Microsoft.OpenApi.Models;
using Northwind.API.Infrastructure.Mapper;
using NorthwindBL.Interfaces;
using NorthwindBL.Services;
using NorthwindDAL.Interfaces;
using NorthwindDAL.Services.Dapper;
using System.Reflection;

namespace Northwind.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "NorthwindAPI",
                    Description = "My Northwind API application.",
                    Contact = new OpenApiContact
                    {
                        Name = "Alexander",
                        Email = "test@test.com"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Test"
                    }
                });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            builder.Services.AddScoped<IAppLogic, AppLogic>();
            builder.Services.AddScoped<IRepository, RepositoryDapperDAL>(serviceProvider =>
            {
                var connectionString = builder.Configuration.GetConnectionString("Default");
                var providerName = builder.Configuration["ProviderName"];
                return new RepositoryDapperDAL(connectionString!, providerName!);
            });
            builder.Services.AddGrpc();
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    c.RoutePrefix = string.Empty;
                });
            }
            app.MapGrpcService<Services.OrderService>();
            app.UseHttpsRedirection();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
