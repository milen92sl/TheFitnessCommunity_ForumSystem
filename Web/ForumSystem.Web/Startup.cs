namespace ForumSystem.Web
{
    using System.Reflection;

    using CloudinaryDotNet;
    using ForumSystem.Data;
    using ForumSystem.Data.Common;
    using ForumSystem.Data.Common.Repositories;
    using ForumSystem.Data.Models;
    using ForumSystem.Data.Repositories;
    using ForumSystem.Data.Seeding;
    using ForumSystem.Services.Data;
    using ForumSystem.Services.Mapping;
    using ForumSystem.Services.Messaging;
    using ForumSystem.Web.Services.Statistics;
    using ForumSystem.Web.ViewModels;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
            {
                services.AddDbContext<ApplicationDbContext>(
                    options => options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection")));

                services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                    .AddRoles<ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>();

                services.Configure<CookiePolicyOptions>(
                    options =>
                        {
                            options.CheckConsentNeeded = context => true;
                            options.MinimumSameSitePolicy = SameSiteMode.None;
                        });

                services.AddControllersWithViews(options =>
                {
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()); // CSRF
            });
                services.AddAntiforgery(options =>
                {
                    options.HeaderName = "X-CSRF-TOKEN";
                });

                services.AddRazorPages();

                services.AddSingleton(this.configuration);

                // Cloudinary
                Account account = new (
                        this.configuration["Cloudinary:CloudName"],
                        this.configuration["Cloudinary:APIKey"],
                        this.configuration["Cloudinary:APISecret"]);

                Cloudinary cloudinary = new (account);
                services.AddSingleton(cloudinary);

                // Data repositories
                services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
                services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
                services.AddScoped<IDbQueryRunner, DbQueryRunner>();

                // Application services
                services.AddTransient<IEmailSender>(x => new SendGridEmailSender("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"));
                services.AddTransient<ICategoriesService, CategoriesService>();
                services.AddTransient<IPostsService, PostsService>();
                services.AddTransient<IVotesService, VotesService>();
                services.AddTransient<ICommentsService, CommentsService>();

                services.AddTransient<IStatisticsService, StatisticsService>();
            }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

                // Seed data on application startup
                using (var serviceScope = app.ApplicationServices.CreateScope())
                {
                    var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    if (env.IsDevelopment())
                    {
                        dbContext.Database.Migrate();
                    }

                    new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
                }

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    app.UseDatabaseErrorPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseCookiePolicy();

                app.UseRouting();

                app.UseAuthentication();
                app.UseAuthorization();

                app.UseEndpoints(
                    endpoints =>
                        {
                            endpoints.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                            endpoints.MapControllerRoute("forumCategory", "category/{name:minlength(3)}", new { controller = "Categories", action = "ByName" });
                            endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                            endpoints.MapRazorPages();
                        });
            }
        }
    }
