
using Auth.Context;
using Auth.Services;
using Microsoft.EntityFrameworkCore;

namespace Auth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var connectionString = builder.Configuration.GetConnectionString("DB-ChallengePPI-Auth");
            builder.Services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlServer(connectionString));
            var secretKey = builder.Configuration["Auth:SecretKey"];
            var issuer = builder.Configuration["Auth:Issuer"];
            var audience = builder.Configuration["Auth:Audience"];

            // Add authentication service with configured parameters
            builder.Services.AddScoped<IAuthService>(provider =>
                new AuthService(provider.GetRequiredService<AuthDbContext>(), secretKey, issuer, audience));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
