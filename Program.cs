using DotNetEnv;
using LoginApp.Data;
using LoginApp.Providers;
using LoginApp.Responses;
using LoginApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LoginApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services.AddHttpClient();
            //builder.Services.AddHttpClient<AuthService>();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            builder.Services.AddScoped(x =>
                (CustomAuthenticationStateProvider)x.GetRequiredService<AuthenticationStateProvider>());
            builder.Services.AddScoped<CustomAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<JwtService>();
            builder.Services.AddScoped<ProtectedLocalStorage>();
            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    Env.Load();
                    string jwtKey = Env.GetString("JWT_KEY");
                    string jwtIssuer = Env.GetString("JWT_ISSUER");
                    var key = Encoding.ASCII.GetBytes(jwtKey);
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });
            builder.Services.AddAuthorization();
            builder.WebHost.UseStaticWebAssets();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.

            }
            app.UseHsts();
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();            

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            //TesteLogin();

            //TesteAuthState(app.Services);

            app.Run();
        }

        static async Task TesteAuthState(IServiceProvider services)
        {
            LoginResponse loginResponse = await TesteLogin();
            var authStateProvider = services.GetRequiredService<CustomAuthenticationStateProvider>();

            authStateProvider.MarkUserAsAuthenticated(loginResponse);

            var authState = await authStateProvider.GetAuthenticationStateAsync();
            Console.WriteLine(authState.User.Identity.Name);
        }

        static async Task<LoginResponse> TesteLogin()
        {
            HttpClient httpClient = new HttpClient();
            AuthService authService = new AuthService(httpClient);
            LoginResponse loginResponse = await authService.LoginAsync("edinho_adm", "12qw!@QW");
            return loginResponse;
        }
    }
}
