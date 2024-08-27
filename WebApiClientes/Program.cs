using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Reflection;
using BlazorClientes.Shared.Entities;
using WebApiClientes.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApiClientes.Services.Interfaces;
using WebApiClientes.Middlewares;
using WebApiClientes.Attributes;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the containers
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//Adicionando serviços que criei
builder.Services.AddScoped<IClientes, ClientesBD>();
builder.Services.AddScoped<IUsuarios, UsuariosBD>();

//Adicionando COPRS para acesso cross-platform
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

//Configurações do Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API de Clientes",
        Description = "API para praticar uso do C#",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "DevPegasus",
            Email = "devpegasus@outlook.com",
            Url = new Uri("https://github.com/rafael-figueiredo-alves")
        },
        License = new OpenApiLicense
        {
            Name = "Leia a Licença de exemplo",
            Url = new Uri("https://example.com/license")
        }
    });

    //Configurações de Autenticação
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = @"JWT Authorization header usando o schema Bearer
                       \r\n\r\n Informe 'Bearer'[space].
                       Examplo: \'Bearer 12345abcdef\'",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                         Array.Empty<string>()
                    }
    });

    //Configurações para inserir comments que são usados no Swagger
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    options.OperationFilter<ApiKeyHeaderSwaggerAttribute>();
});

//Usado para personalizar mensagem de erro do BadRequest por validation error
builder.Services.AddMvc()
                  .ConfigureApiBehaviorOptions(options =>
                  {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var problemDetails = new ErroValidacao(context);
                        return new BadRequestObjectResult(problemDetails);
                    };
                  });

//Serviços de autenticação e autorização
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))

    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}

app.UseCors(option =>
           {
               option.AllowAnyOrigin();
               option.AllowAnyMethod();
               option.AllowAnyHeader();
           });

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseApiKeyMiddleware();
app.UseRateLimitMiddleware();

app.MapControllers();

app.Run();
