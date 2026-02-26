using ControleFinanceiro_REST.BLL.Entities;
using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.BLL.Utils;
using ControleFinanceiro_REST.BLL.Utils.Interfaces;
using ControleFinanceiro_REST.DAL.Entities;
using ControleFinanceiro_REST.DAL.Entities.Interfaces;
using ControleFinanceiro_REST.DAL.Mapper;
using ControleFinanceiro_REST.DAO;
using ControleFinanceiro_REST.DAO.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FinanceDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.AllowedUserNameCharacters = null;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

//BLL
builder.Services.AddScoped<IAutenticacaoBLL, AutenticacaoBLL>();
builder.Services.AddScoped<ICategoriaBLL, CategoriaBLL>();
builder.Services.AddScoped<IPessoaBLL, PessoaBLL>();
builder.Services.AddScoped<IRoleBLL, RoleBLL>();
builder.Services.AddScoped<ITransacaoBLL, TransacaoBLL>();
builder.Services.AddScoped<IUsuarioBLL, UsuarioBLL>();

//DAL
builder.Services.AddScoped<ICategoriaDAL, CategoriaDAL>();
builder.Services.AddScoped<IPessoaDAL, PessoaDAL>();
builder.Services.AddScoped<IRoleDAL, RoleDAL>();
builder.Services.AddScoped<ITransacaoDAL, TransacaoDAL>();
builder.Services.AddScoped<ITipoUsuarioDAL, TipoUsuarioDAL>();
builder.Services.AddScoped<IUsuarioDAL, UsuarioDAL>();

//Utils
builder.Services.AddScoped<IEncriptador, Encriptador>();
builder.Services.AddScoped<IGeradorToken, GeradorToken>();
builder.Services.AddScoped<IUsuarioContexto, UsuarioContexto>();

//Mapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

//Token
builder.Services.AddScoped<ITokenContexto, TokenContexto>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR().AddJsonProtocol(opt => opt.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["AuthToken:Issuer"],
        ValidAudience = builder.Configuration["AuthToken:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["AuthToken:ChaveSecreta"])),
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            var accessToken = ctx.Request.Query["access_token"];
            var path = ctx.HttpContext.Request.Path;
            ctx.Token = accessToken;

            return Task.CompletedTask;
        }
    };
    options.MapInboundClaims = false;
});
builder.Services.AddAuthorization();

//Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Controle Financeiro API", Version = "v1" });

    // Define o esquema de segurança JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Cole **apenas** o token JWT. O prefixo 'Bearer' será adicionado automaticamente."
    });

    // Aplica o esquema de segurança a todas as operações
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Id = "Bearer",
                Type = ReferenceType.SecurityScheme
            }
        },
        Array.Empty<string>()
    }
});
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowLocalFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
