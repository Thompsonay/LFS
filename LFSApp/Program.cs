using LFSApp.Dbcontext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Dbcontext;

namespace LFSApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            // configure Paystack settings + HttpClient
            builder.Services.AddHttpClient();
            builder.Services.Configure<LFSApp.Model.PaystackSettings>(builder.Configuration.GetSection("Paystack"));
            builder.Services.AddScoped<LFSApp.Services.IPaystackService, LFSApp.Services.PaystackService>();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDbContext<MainDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<MainDbContext>()
                .AddDefaultTokenProviders();

      
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            }); 

            var app = builder.Build();

            // Safety check: ensure Paystack secret key exists in non-development environments
            var paystackConfig = builder.Configuration.GetSection("Paystack").Get<LFSApp.Model.PaystackSettings>();
            if (!app.Environment.IsDevelopment())
            {
                if (paystackConfig == null || string.IsNullOrWhiteSpace(paystackConfig.SecretKey) || paystackConfig.SecretKey.StartsWith("sk_test_") )
                {
                    // Fail fast in production if secret is missing or still the test placeholder to avoid accidental use
                    throw new InvalidOperationException("Missing or invalid Paystack secret key in production environment. Configure Paystack:SecretKey via user-secrets, env vars or a secrets store.");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllers();


            app.Run();
        }
    }
}
