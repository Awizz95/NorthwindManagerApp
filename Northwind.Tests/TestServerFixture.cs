using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Northwind.API;
using Northwind.API.Infrastructure.Mapper;
using Northwind.API.Services;
using NorthwindBL.Interfaces;
using NorthwindBL.Services;
using NorthwindDAL.Interfaces;
using NorthwindDAL.Services.Dapper;

namespace Northwind.Tests
{
    public class TestServerFixture
    {
        private readonly TestServer _server;
        public NorthwindService.NorthwindServiceClient Client { get; }

        public TestServerFixture()
        {
            var builder = new HostBuilder().ConfigureWebHost(builder =>
                builder.UseTestServer().ConfigureServices(services =>
                {
                    services.AddGrpc();
                    services.AddScoped<IAppLogic, AppLogic>();
                    services.AddScoped<IRepository, RepositoryDapperDAL>(sp =>
                    {
                        var connectionString = "Data Source=localhost;Initial Catalog=Northwind;Integrated Security=True;TrustServerCertificate=True";
                        var providerName = "Microsoft.Data.SqlClient";
                        return new RepositoryDapperDAL(connectionString, providerName);
                    });
                    services.AddAutoMapper(typeof(MappingProfile));
                })
                .Configure(app =>
                 {
                     app.UseRouting();
                     app.UseEndpoints(endpoints =>
                     {
                         endpoints.MapGrpcService<OrderService>();
                     });
                 }));

            var host = builder.Start();
            _server = host.GetTestServer();

            var channel = GrpcChannel.ForAddress(_server.BaseAddress, new GrpcChannelOptions { HttpClient = _server.CreateClient() });

            Client = new NorthwindService.NorthwindServiceClient(channel);
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}
