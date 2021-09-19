using Arad.Portal.DataLayer.Entities.General.User;
using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Globalization;
using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Arad.Portal.UI.Shop.Dashboard.Authorization;
using Arad.Portal.DataLayer.Contracts.General.User;
using Arad.Portal.DataLayer.Repositories.General.User.Mongo;
using Arad.Portal.DataLayer.Contracts.General.Currency;
using Arad.Portal.DataLayer.Repositories.General.Currency.Mongo;
using Arad.Portal.DataLayer.Contracts.General.Domain;
using Arad.Portal.DataLayer.Repositories.General.Domain.Mongo;
using Arad.Portal.DataLayer.Contracts.General.Language;
using Arad.Portal.DataLayer.Repositories.General.Language.Mongo;
using Arad.Portal.DataLayer.Contracts.General.Permission;
using Arad.Portal.DataLayer.Repositories.General.Permission.Mongo;
using Arad.Portal.DataLayer.Contracts.General.Role;
using Arad.Portal.DataLayer.Repositories.General.Role.Mongo;
using Arad.Portal.DataLayer.Contracts.Shop.Order;
using Arad.Portal.DataLayer.Repositories.Shop.Order.Mongo;
using Arad.Portal.DataLayer.Contracts.Shop.Product;
using Arad.Portal.DataLayer.Repositories.Shop.Product.Mongo;
using Arad.Portal.DataLayer.Contracts.Shop.ProductGroup;
using Arad.Portal.DataLayer.Repositories.Shop.ProductGroup.Mongo;
using Arad.Portal.DataLayer.Repositories.Shop.ProductSpecification.Mongo;
using Arad.Portal.DataLayer.Contracts.Shop.ProductSpecification;
using Arad.Portal.DataLayer.Contracts.Shop.ProductSpecificationGroup;
using Arad.Portal.DataLayer.Contracts.Shop.ProductUnit;
using Arad.Portal.DataLayer.Repositories.Shop.ProductUnit.Mongo;
using Arad.Portal.DataLayer.Contracts.Shop.Promotion;
using Arad.Portal.DataLayer.Repositories.Shop.Promotion.Mongo;
using Arad.Portal.DataLayer.Contracts.Shop.ShoppingCart;
using Arad.Portal.DataLayer.Repositories.Shop.ShoppingCart.Mongo;
using Arad.Portal.DataLayer.Contracts.Shop.Transaction;
using Arad.Portal.DataLayer.Repositories.Shop.Transaction.Mongo;
using Arad.Portal.DataLayer.Repositories.Shop.ProductSpecificationGroup.Mongo;
using Microsoft.AspNetCore.Http;
using Arad.Portal.UI.Shop.Dashboard.Helpers;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Arad.Portal.DataLayer.Contracts.General.Notification;
using Arad.Portal.DataLayer.Repositories.General.Notification.Mongo;
using Arad.Portal.DataLayer.Contracts.General.MessageTemplate;
using Arad.Portal.DataLayer.Repositories.General.MessageTemplate.Mongo;
using Arad.Portal.DataLayer.Repositories.General.ContentCategory.Mongo;
using Arad.Portal.DataLayer.Contracts.General.ContentCategory;
using Arad.Portal.DataLayer.Contracts.General.Content;
using Arad.Portal.DataLayer.Repositories.General.Content.Mongo;
using Arad.Portal.DataLayer.Contracts.General.Comment;
using Arad.Portal.DataLayer.Repositories.General.Comment.Mongo;

namespace Arad.Portal.UI.Shop.Dashboard
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            GeneralLibrary.Utilities.Language._hostingEnvironment = env.WebRootPath;
            ApplicationPath = env.ContentRootPath;
        }

        public IConfiguration Configuration { get; }
        public static string ApplicationPath { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession();
            services.AddHttpClient();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<HtmlEncoder>(
                HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.BasicLatin,
                    UnicodeRanges.Arabic }));

            services.AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole, string>(identityOptions =>
            {
                identityOptions.Password.RequiredLength = 6;
                identityOptions.Password.RequireLowercase = true;
                identityOptions.Password.RequireUppercase = false;
                identityOptions.Password.RequireNonAlphanumeric = false;
                identityOptions.Password.RequireDigit = true;

            }, mongoIdentityOptions =>
            {
                mongoIdentityOptions.ConnectionString = Configuration["Database:ConnectionString"];
            });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
              .AddCookie(opt =>
              {
                  opt.Cookie.HttpOnly = true;
              });
            //services.AddRazorPages();
            services.AddTransient<IAuthorizationHandler, RoleHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Role", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new RoleRequirement());
                });
            });
            services.AddAutoMapper(typeof(Startup));

            services.AddTransient<IPermissionView, PermissionView>();
            services.AddTransient<RemoteServerConnection>();

            AddRepositoryServices(services);
            services.AddProgressiveWebApp();
            
        }

       
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

           
            app.UseHttpsRedirection();
            app.UseStaticFiles();
           
            app.UseRequestLocalization(AddMultilingualSettings());
           
            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                //if (env.IsDevelopment())
                //    endpoints.MapControllers().WithMetadata(new AllowAnonymousAttribute());
                //else
                //    endpoints.MapControllers();

                //endpoints.MapControllerRoute(
                //     name: "ProductComments",
                //     pattern: "ProductComments/{action}/{id?}",
                //     defaults: new { controller = "Comments" , t = "product"});

                //  endpoints.MapControllerRoute(
                //    name: "ContentComments",
                //    pattern: "ContentComments/{action}/{id?}",
                //    defaults: new { controller = "Comments", t = "content"});

                //endpoints.MapRazorPages();

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");


            });
            app.UseSeedDatabase(ApplicationPath);
        }
        private RequestLocalizationOptions AddMultilingualSettings()
        {
            string[] supportedCulturesStrings = Configuration.GetSection("SupportedCultures")
               .Get<string[]>();

            List<CultureInfo> supportedCultures = new();

            foreach (string item in supportedCulturesStrings)
            {
                supportedCultures.Add(new CultureInfo(item));
            }

            RequestLocalizationOptions options = new RequestLocalizationOptions()
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
                RequestCultureProviders = new List<IRequestCultureProvider>()
                {
                    new QueryStringRequestCultureProvider(),
                    new CookieRequestCultureProvider()
                }
            };
            return options;
        }

        private void AddRepositoryServices(IServiceCollection services)
        {
            services.AddTransient<ICurrencyRepository, CurrencyRepository>();
            services.AddTransient<IDomainRepository, DomainRepository>();
            services.AddTransient<ILanguageRepository, LanguageRepository>();
            services.AddTransient<IPermissionRepository, PermissionRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<UserExtensions, UserExtensions>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IProductGroupRepository, ProductGroupRepository>();
            services.AddTransient<IProductSpecificationRepository, ProductSpecificationRepository>();
            services.AddTransient<IProductSpecGroupRepository, ProductSpecGroupRepository>();
            services.AddTransient<IProductUnitRepository, ProductUnitRepository>();
            services.AddTransient<IPromotionRepository, PromotionRepository>();
            services.AddTransient<IShoppingCartRepository, ShoppingCartRepository>();
            services.AddTransient<ITransationRepository, TransactionRepository>();
            services.AddTransient<INotificationRepository, NotificationRepository>();
            services.AddTransient<IMessageTemplateRepository, MessageTemplateRepository>();
            services.AddTransient<IContentCategoryRepository, ContentCategoryRepository>();
            services.AddTransient<IContentRepository, ContentRepository>();
            services.AddTransient<ICommentRepository, CommentRepository>();

            #region contexes
            services.AddTransient<CurrencyContext>();
            services.AddTransient<DomainContext>();
            services.AddTransient<LanguageContext>();
            services.AddTransient<PermissionContext>();
            services.AddTransient<RoleContext>();
            services.AddTransient<UserContext>();
            services.AddTransient<OrderContext>();
            services.AddTransient<ProductContext>();
            services.AddTransient<PromotionContext>();
            services.AddTransient<ShoppingCartContext>();
            services.AddTransient<TransactionContext>();
            services.AddTransient<MessageTemplateContext>();
            services.AddTransient<NotificationContext>();
            services.AddTransient<ContentCategoryContext>();
            services.AddTransient<ContentContext>();
            services.AddTransient<CommentContext>();
                
            #endregion

        }
    }
}
