
using AuthWithKeyCloak.KeyCloak;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.OpenApi.Models;

namespace AuthWithKeyCloak
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var configuration = builder.Configuration;
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(cf =>
            {
                cf.SwaggerDoc("v1", new OpenApiInfo { Title = "Keycloack Demo", Version = "v1" });

                // Configuração do esquema de segurança JWT para o Swagger
                cf.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http, // Altere para Http para indicar que é um esquema de autenticação HTTP com Bearer
                    Scheme = "bearer", // Especifique "bearer" para indicar o formato JWT
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Por favor, insira o token JWT no campo 'Authorization' usando o prefixo 'Bearer '"
                });

                cf.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            });

            var authenticationOptions = builder
                                        .Configuration
                                        .GetSection(KeycloakAuthenticationOptions.Section)
                                        .Get<KeycloakAuthenticationOptions>();

            builder.Services.AddKeycloakAuthentication(authenticationOptions!);


            var authorizationOptions = builder
                                        .Configuration
                                        .GetSection(KeycloakProtectionClientOptions.Section)
                                        .Get<KeycloakProtectionClientOptions>();

            builder.Services.AddKeycloakAuthorization(authorizationOptions!);

            builder.Services.AddOptions<LoginOption>()
                .BindConfiguration("LoginOption")
                .ValidateDataAnnotations();


            builder.Services.AddHttpClient("Keycloak", client=>
             {
                client.BaseAddress = new Uri(configuration["EndpointKeycloak:url"]!);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });


            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
