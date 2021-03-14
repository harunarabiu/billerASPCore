using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FirstApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Refit;
using FirstApp.Services;
using FirstApp.Helpers;
using System.Threading.Tasks;
using System.Linq;
using FirstApp.Models.ViewModels;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.OpenApi.Models;

namespace FirstApp
{
    public class Startup
    {
        private readonly IConfigurationRoot configuration;
        ILogger _logger;

        public Startup(IWebHostEnvironment env)
        {
            configuration = new ConfigurationBuilder()
                                    .AddEnvironmentVariables()
                                    .AddJsonFile(env.ContentRootPath + "/config.json")
                                    .AddJsonFile(env.ContentRootPath + "/config.development.json", true)
                                    .Build();

        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<TextFormatting>();
            services.AddTransient<FeatureToggles>(x => new FeatureToggles
            {
                EnableDeveloperExceptions = configuration.GetValue<bool>("FeatureToggles:EnableDeveloperExceptions")
            });
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));

            

            // Add converter to DI
            services.AddMvc().AddControllersAsServices();

            // Services dependencies
            services.AddTransient<DocumentService>();
            services.AddTransient<IRazorRenderer, RazorRendererHelper>();

            services.AddTransient<Monnify>();
            services.AddTransient<KEDCO>();
            services.AddTransient<GladePay>();
            services.AddTransient<MultiTexter>();
            services.AddTransient<SmartSMS>();
            services.AddTransient<Utils>();
            services.AddTransient<TransactionService>();


            services.AddTransient<IEmailService, EmailService>();

            var DB_PWD = Environment.GetEnvironmentVariable("DB_PWD");
            var DB_USERNAME = Environment.GetEnvironmentVariable("DB_USERNAME");
            var DB_HOST = Environment.GetEnvironmentVariable("DB_HOST");
            var DB_NAME = Environment.GetEnvironmentVariable("DB_NAME");

            var KEDCO_ENDPOINT = Environment.GetEnvironmentVariable("KEDCO_TEST_ENDPOINT");
            var MONNIFY_ENDPOINT = Environment.GetEnvironmentVariable("MONNIFY_TEST_ENDPOINT");
            var SMARTSMS_ENDPOINT = Environment.GetEnvironmentVariable("SMARTSMS_TEST_ENDPOINT");
            var GLADE_ENDPOINT = Environment.GetEnvironmentVariable("GLADE_TEST_ENDPOINT");

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
            {

                KEDCO_ENDPOINT = Environment.GetEnvironmentVariable("KEDCO_ENDPOINT");
                MONNIFY_ENDPOINT = Environment.GetEnvironmentVariable("MONNIFY_ENDPOINT");
                GLADE_ENDPOINT = Environment.GetEnvironmentVariable("GLADE_ENDPOINT");
                SMARTSMS_ENDPOINT = Environment.GetEnvironmentVariable("SMARTSMS_ENDPOINT");

                services.AddDbContextPool<DBDataContext>(options =>
                {

                    options.UseMySql(
                             // Replace with your connection string.
                             //$"server={DB_HOST};user={DB_USERNAME};password={DB_PWD};database={DB_NAME}",
                             configuration.GetConnectionString("DatabaseConnectionString"),
                             // Replace with your server version and type.
                             // For common usages, see pull request #1233.
                             new MySqlServerVersion(new Version(10, 4, 17)), // use MariaDbServerVersion for MariaDB
                             mySqlOptions => mySqlOptions
                                 .CharSetBehavior(CharSetBehavior.NeverAppend))
                         // Everything from this point on is optional but helps with debugging.
                         .EnableSensitiveDataLogging()
                         .EnableDetailedErrors();
                    options.UseLazyLoadingProxies();
                });
            }
            else

            {
                KEDCO_ENDPOINT = Environment.GetEnvironmentVariable("KEDCO_TEST_ENDPOINT");
                //services.AddDbContext<DBDataContext>(options =>
                //{

                //    var connectionString = configuration.GetConnectionString("DBContext");

                //    //if (Environment.IsDevelopment())
                //    //{
                //    //    options.UseSqlite(connectionString);
                //    //}
                //    //else
                //    //{
                //    //    options.UseSqlServer(connectionString);
                //    //}
                //    options.UseLazyLoadingProxies();
                //    options.UseSqlite(connectionString);

                //});
                services.AddDbContextPool<DBDataContext>(options =>
                {

                    options.UseMySql(
                             // Replace with your connection string.
                             //$"server={DB_HOST};user={DB_USERNAME};password={DB_PWD};database={DB_NAME}",
                             configuration.GetConnectionString("DatabaseConnectionString"),
                             // Replace with your server version and type.
                             // For common usages, see pull request #1233.
                             new MySqlServerVersion(new Version(10, 4, 17)), // use MariaDbServerVersion for MariaDB
                             mySqlOptions => mySqlOptions
                                 .CharSetBehavior(CharSetBehavior.NeverAppend))
                         // Everything from this point on is optional but helps with debugging.
                         .EnableSensitiveDataLogging()
                         .EnableDetailedErrors();
                    options.UseLazyLoadingProxies();
                });
            }



            services.AddHttpClient("kedco", c =>
            {
                c.BaseAddress = new Uri(KEDCO_ENDPOINT);
            }).AddTypedClient(c => RestService.For<IKEDCOApi>(c));

            services.AddHttpClient("monnify", c =>
            {
                c.BaseAddress = new Uri(MONNIFY_ENDPOINT);
            }).AddTypedClient(c => RestService.For<IMonnifyApi>(c));
            services.AddHttpClient("gladepay", c =>
            {
                c.BaseAddress = new Uri(GLADE_ENDPOINT);
            }).AddTypedClient(c => RestService.For<IGladePayAPI>(c));

            services.AddHttpClient("MultiTexter", c =>
            {
                c.BaseAddress = new Uri("https://app.multitexter.com/v2/app");
            }).AddTypedClient(c => RestService.For<IMultiTexterAPI>(c));

            services.AddHttpClient("SmartSMS", c =>
            {
                c.BaseAddress = new Uri(SMARTSMS_ENDPOINT);
            }).AddTypedClient(c => RestService.For<ISmartSMSAPI>(c));

            // options.UseSqlite(

            services.AddIdentityCore<ApplicationUser>(
                options => options.SignIn.RequireConfirmedAccount = false
                )
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DBDataContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();




            services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddCookie(IdentityConstants.ApplicationScheme, options =>
                {
                    options.SlidingExpiration = true;
                }).AddCookie(IdentityConstants.TwoFactorUserIdScheme, options =>
                {
                    options.Cookie.Name = IdentityConstants.TwoFactorUserIdScheme;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["JWT:Issuer"],
                        ValidAudience = configuration["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"])),
                        ClockSkew = TimeSpan.Zero

                    };
                }).AddExternalCookie();

            //.AddCookie(IdentityConstants.ApplicationScheme, options =>
            //{
            //    options.LoginPath = "/account/login/";
            //    options.AccessDeniedPath = "/account/Forbidden/";
            //});
            //.AddIdentityCookies(o => { });


            //services.AddTransient<IEmailSender, AuthMessageSender>();
            //services.AddTransient<ISmsSender, AuthMessageSender>()

            services.AddAuthorization(options =>
            {
                options.AddPolicy("claimBased", policy => policy.RequireClaim("Agent"));
                options.AddPolicy("AdministratorsOnly", policy => policy.RequireRole("Root", "Administrator"));
            });

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

                options.LoginPath = "/account/login";
                options.AccessDeniedPath = "/account/login";
                options.SlidingExpiration = true;
            });

            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            //services.AddControllers();
            services.AddControllers().AddNewtonsoftJson();
            services.AddHttpContextAccessor();

            //cors

            services.AddCors(options =>
            {
                options.AddPolicy("frontend", builder =>
                {
                    builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
                });
            });

            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Biller", Version = "v1" });
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (
            IApplicationBuilder app,
            IWebHostEnvironment env,
            FeatureToggles features,
            IServiceProvider serviceProvider,
            DBDataContext DB
        )
        {
            

            //if (configuration["EnableDeveloperOptions"] == "True")
            //if (configuration.GetValue<bool>("FeatureToggles:EnableDeveloperExceptions"))
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (env.IsProduction() || env.IsStaging())
            {
                app.UseExceptionHandler("/Error.html");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/", async context =>
                //{
                //    if (context.Request.Path.Value.Contains("invalid"))
                //    {
                //        throw new Exception("Error");
                //    }

                //    await context.Response.WriteAsync("Hello World!");
                //});
                //endpoints.MapGet("/auth", async context =>
                //{
                //    //context.Request.Path.Value.Contains("invalid");
                //    await context.Response.WriteAsync("Authentication");
                //});

                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });


            app.UseFileServer();
            //foreach (var item in Environment.GetEnvironmentVariables())
            //{
            //    var value = item;
            //    _logger.Log(LogLevel.Warning, item.ToString());
            //}

            CreateRoles(serviceProvider, DB).Wait();
        }

        private async Task CreateRoles(IServiceProvider serviceProvider, DBDataContext DB)
        {
            var _DB = DB;
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string[] roleNames = { "Root", "Administrator", "Supervisor", "ExSupervisor", "Manager", "Customer", "Merchant", "Agent" };
            IdentityResult roleResult;
            foreach(var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var users = _DB.Users.Any();
            if (!users){

                var root = new ApplicationUser
                {
                    FirstName = "root",
                    LastName = "toor",
                    Email = "root@biller.ng",
                    UserName = "root@biller.ng",
                    PhoneNumber = "08189931773",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                var pwd = configuration["JWT:Secret"];
                var createUser = await UserManager.CreateAsync(root, pwd);

                if (createUser.Succeeded)
                    await UserManager.AddToRoleAsync(root, "Root");
            }

            var _service = _DB.Services.Any();

            if (!_service)
            {


                var serviceType = new ServiceType
                {
                    Name = "Electricity",
                    Key = "Electricity",
                    Status = "Active"
                };

                _DB.ServiceTypes.Add(serviceType);
                _DB.SaveChanges();


                var service = new Service
                {
                    Name = "KEDCO",
                    Key = "kedco",
                    ServiceType = serviceType,
                    Status = "Active"
                };

                _DB.Services.Add(service);
                _DB.SaveChanges();



                var serviceplanprepaid = new ServicePlan
                {
                    Name = "Prepaid",
                    Key = "prepaid",
                    Service = service,
                    Price = 0,
                    IsDefault = true,
                    CommissionType = "Variable",
                    Commission = 3.5,
                    ConvienceFee = 50,
                    Status = "Active"

                };
                _DB.ServicePlans.Add(serviceplanprepaid);

                var serviceplanpostpaid = new ServicePlan
                {
                    Name = "Postpaid",
                    Key = "postpaid",
                    Service = service,
                    Price = 0,
                    IsDefault = true,
                    CommissionType = "Variable",
                    Commission = 3.5,
                    ConvienceFee = 50,
                    Status = "Active"

                };
                _DB.ServicePlans.Add(serviceplanpostpaid);

                _DB.PaymentPlans.Add(new PaymentPlan
                {
                    Name = "Kedco Merchant Paid",
                    Key = "kedcomerchantprepaid",
                    ServicePlan = serviceplanprepaid,
                    IsDefault = true,
                    Status = "Active",
                    CommissionType = "Variable",
                    Commission = 2,
                    ConvienceFee = 50
                });
                _DB.PaymentPlans.Add(new PaymentPlan
                {
                    Name = "Kedco Merchant PostPaid",
                    Key = "kedcomerchantpostpaid",
                    ServicePlan = serviceplanpostpaid,
                    IsDefault = true,
                    Status = "Active",
                    CommissionType = "Variable",
                    Commission = 2,
                    ConvienceFee = 50

                });

                _DB.SaveChanges();

               

            }

           

        }

            
    }
}
