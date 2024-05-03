using BetaCycle_Padova.BLogic.Authentication.Basic;
using BetaCycle_Padova.Controllers.Context;
using BetaCycle_Padova.Controllers.LTWorks;
using BetaCycle_Padova.Controllers.Users;
using BetaCycle_Padova.Log;
using BetaCycle_Padova.Models.Mongo;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAca5CodeFirst.Models;

namespace BetaCycle_Padova
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region BUILDER
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();



            builder.Services.AddScoped<OldCustomersController>();
            builder.Services.AddScoped<UsersController>();
            builder.Services.AddScoped<CredentialsController>();
            #endregion


            ///////////////////BASIC AUTHENTICATION/////////////////////////
            #region BASIC AUTHENTICATION
            //mediante dependency injection diciamo al sistema che c'è l'autenticazione di tipo Basic e lo dovrai gestire con BasicAutheticationHandler
            builder.Services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", opt => { }); //qui non facciamo nulla, ma la vuole come firma

            builder.Services.AddAuthorization(opts =>
            {
                opts.AddPolicy("BasicAuthorization", new AuthorizationPolicyBuilder("BasicAuthentication")
                    .RequireAuthenticatedUser().Build());
            });
            #endregion
            ////////////////////////////////////////////////////////
           

            ///////////////////JWT AUTHENTICATION/////////////////////////
            #region JWT AUTHENTICATION
            //devo ignettare come singleton la mia classe JwtSettings
            JwtSettings jwtSettings = new JwtSettings();
            jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>(); //prendimi quello che c'è scritto in appsettings.json, convertimelo in jwtsettins

            builder.Services.AddSingleton(jwtSettings); //così facendo io ignetto una classe jwtsettings disponibile a chiunque con la dependency injection

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(opts =>
                 {
                     opts.TokenValidationParameters = new TokenValidationParameters
                     {
                         //questi booleani posso anche dire di non usarli perchè non mi servono - dipende dalla soluzione richiesta
                         ValidateIssuer = true, //chi ha generato il token
                         ValidateAudience = true, //chi lo può andare ad utilizzare
                         ValidateLifetime = true, //durata del token
                         ValidateIssuerSigningKey = true, //la firma di chi ha generato il token
                         ValidIssuer = jwtSettings.Issuer, //qui da usare appsetting.json
                         ValidAudience = jwtSettings.Audience,
                         RequireExpirationTime = true, //molto importante: il token deve avere il tempo di scadenza, dopo quanto scade. - se manca dice che il token non è valido
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)) //
                     };
                 });
            #endregion
            //////////////////////////////////////////////////////////////

            #region CONNESSIONE MONGO DB
            builder.Services.Configure<PostMDBConfig>(builder.Configuration.GetSection("BetacycleMongoDB"));
            #endregion


            #region CONNESSIONE AI DATABASE LT2019 e USERS
            //String per la connessione al database AdventureWorksLT2019
            builder.Services.AddDbContext<AdventureWorksLt2019Context>(opts => 
                opts.UseSqlServer(
                     builder.Configuration.GetConnectionString("LTWorks")));

            //String per la connessione al database Users
            builder.Services.AddDbContext<BetacycleUsersContext>(opts =>
                opts.UseSqlServer(
                     builder.Configuration.GetConnectionString("DbUsers")));
            #endregion

            #region CORS
            //CORS - OK - per avere accesso - da rivedere per i permessi
            builder.Services.AddCors(opts =>
            {
                opts.AddPolicy("CORS_BetaCycle",
                builder => builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .SetIsOriginAllowed((hosts) => true));
            });
            #endregion

            ////////////Prova gestione errori///////////////////////////
            #region  NLog per gestire gli eventi / errori
            //LoggerNLog loggerNLog = new();
            #endregion
            //////////////////////////////////////////////////////////////

            #region app

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("CORS_BetaCycle");

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
            #endregion
        }
    }
}
