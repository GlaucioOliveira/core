using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using webhook.Models;

namespace webhook
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //get the set list of valid github Users
            Util.githubUsers = configuration.GetSection("webhook")
                                            .GetSection("github-allowed-user")
                                            .GetChildren()
                                            .Select(x => x.Value).ToList();

            Util.bashScript = configuration.GetSection("webhook").GetValue<string>("bash-script");

            Util.githubUserName = configuration.GetSection("webhook").GetValue<string>("github-username");
            Util.githubUserPassword = configuration.GetSection("webhook").GetValue<string>("github-password");
            Util.bashDir = configuration.GetSection("webhook").GetValue<string>("bash-dir");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UsePathBase("/webhook");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
