using API.Middleware;
using API.Services.CableCategories;
using API.Services.Cables;
using API.Services.Issues;
using API.Services.NodeMaterialCategories;
using API.Services.Nodes;
using API.Services.OtherMaterials;
using API.Services.OtherMaterialsCategories;
using API.Services.Requests;
using API.Services.Routes;
using API.Services.Statistic;
using API.Services.Suppliers;
using API.Services.Transaction;
using API.Services.Users;
using API.Services.Warehouses;
using AutoMapper;
using DataAccess.Configuration;
using DataAccess.DAO;
using DataAccess.DBContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace API
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
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Here Enter JWT Token with Bearer format: Bearer[space][token]"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[]{ }
                    }
                });
            }
           );
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = ConfigData.JwtAudience,
                    ValidIssuer = ConfigData.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigData.JwtKey))
                };
            });
            builder.Services.AddCors(o => o.AddPolicy("AllowOrigin", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            // ----------------------- register db context -----------------------
            builder.Services.AddDbContext<CableManagementContext>(/*options => options.UseSqlServer(ConfigData.SqlConnection), */ServiceLifetime.Scoped);
            // ----------------------- register DAO -----------------------
            builder.Services.AddTransient<DAOCableCategory>();
            builder.Services.AddTransient<DAOUser>();
            builder.Services.AddTransient<DAOCable>();
            builder.Services.AddTransient<DAOIssue>();
            builder.Services.AddTransient<DAONodeMaterialCategory>();
            builder.Services.AddTransient<DAONode>();
            builder.Services.AddTransient<DAOOtherMaterialsCategory>();
            builder.Services.AddTransient<DAOOtherMaterial>();
            builder.Services.AddTransient<DAOTransactionOtherMaterial>();
            builder.Services.AddTransient<DAOTransactionCable>();
            builder.Services.AddTransient<DAORoute>();
            builder.Services.AddTransient<DAOTransactionHistory>();
            builder.Services.AddTransient<DAORequest>();
            builder.Services.AddTransient<DAORequestCable>();
            builder.Services.AddTransient<DAORequestOtherMaterial>();
            builder.Services.AddTransient<DAOSupplier>();
            builder.Services.AddTransient<DAOWarehouse>();
            // ----------------------- register service -----------------------
            builder.Services.AddScoped<ICableCategoryService, CableCategoryService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICableService, CableService>();
            builder.Services.AddScoped<IIssueService, IssueService>();
            builder.Services.AddScoped<INodeMaterialCategoryService, NodeMaterialCategoryService>();
            builder.Services.AddScoped<INodeService, NodeService>();
            builder.Services.AddScoped<IOtherMaterialsCategoryService, OtherMaterialsCategoryService>();
            builder.Services.AddScoped<IOtherMaterialsService, OtherMaterialsService>();
            builder.Services.AddScoped<IRouteService, RouteService>();
            builder.Services.AddScoped<IStatisticService, StatisticService>();
            builder.Services.AddScoped<ISupplierService, SupplierService>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();
            builder.Services.AddScoped<IWarehouseService, WarehouseService>();
            builder.Services.AddScoped<IRequestService, RequestService>();
            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {

            }
            app.UseMiddleware<UnauthorizedMiddleware>();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();

        }
    }
}
