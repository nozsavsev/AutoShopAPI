using AutoShopAPI.DbContexts;
using AutoShopAPI.Mappings;
using AutoShopAPI.Repositories;
using AutoShopAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace AutoShopAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            //setup database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine($"Using connection string: {connectionString}");
            builder.Services.AddDbContext<AutoShopDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<ICarRepository, CarRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddScoped<ICarService, CarService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                                          policy.WithOrigins("http://localhost:3000",
                                                          "http://localhost:5005",
                                                          "https://shop.nozsa.com",
                                                          "https://autoshopapi.nozsa.com").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                                      else
                                          policy.WithOrigins("https://autoshopapi.nozsa.com",
                                                          "https://shop.nozsa.com").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                                  });
            });

            var app = builder.Build();

            // Always use CORS in production
            app.UseCors(MyAllowSpecificOrigins);

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AutoShopDbContext>();
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }

            app.Run();
        }
    }
}
