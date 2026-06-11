using Authorization.CustomClaim;
using Authorization.Policys;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;

namespace Api.Extensions
{
    public static class AuthorizationSetup
    {
        public static void AddAuthorizationSetup(this WebApplicationBuilder builder)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(System.AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oidc";
            })
                            .AddJwtBearer("Bearer", options =>
                            {
                                options.Authority = configuration["AuthorityAddress"];
                                options.Audience = "Resource";
                                options.TokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidateAudience = false,
                                    ValidateLifetime = true,
                                    RequireExpirationTime = true,              
                                    ClockSkew = TimeSpan.Zero,
                                };
                                options.RequireHttpsMetadata = false;
                            });
            builder.Services.AddSingleton<IClaimsTransformation, ClaimTransformation>();
            var permission = new List<PermissionItem>();
            var permissionRequirement = new PermissionRequirement(permission);
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("PermissionPolicy", policy =>
                {
                    policy.AuthenticationSchemes.Add("Bearer");
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "Ids");
                    policy.Requirements.Add(permissionRequirement);
                });
            });
            builder.Services.AddSingleton<IAuthorizationHandler>(sp =>
            {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var configuration = sp.GetRequiredService<IConfiguration>();
                var claimsTransformation = sp.GetRequiredService<IClaimsTransformation>();
                var baseUrl = configuration["PlatFormUrl"];
                var sysCode = configuration["SysCode"];
                var isSuperAdmin = configuration.GetValue<bool>("IsSuperAdmin");
                return new PermissionHandler(httpContextAccessor, claimsTransformation, baseUrl, sysCode, isSuperAdmin);
            });
        }
    }
}
