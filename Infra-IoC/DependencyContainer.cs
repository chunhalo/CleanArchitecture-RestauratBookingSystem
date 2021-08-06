using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Infra_Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infra_IoC
{
    public class DependencyContainer
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IBookRepo, BookRepo>();

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IAuthenticationRepo, AuthenticationRepo>();

            services.AddScoped<IResService, ResService>();
            services.AddScoped<IResRepo, RestaurantRepo>();

            services.AddScoped<IAnnounceService, AnnounceService>();
            services.AddScoped<IAnnounceRepo, AnnounceRepo>();

            services.AddScoped<ITableService, TableService>();
            services.AddScoped<ITableRepo, TableRepo>();
        }
    }
}
