using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaContentService.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MediaContentService.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MediaContentService.Services;
using Newtonsoft.Json.Serialization;

namespace MediaContentService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IFileCache cache = new FileCache(Configuration);
            IFileStore fileStore = new FileStore(cache);

            services.AddSingleton(typeof(IFileStore), fileStore);
            services.AddMvc().AddJsonOptions(opt =>
            {
                var resolver = opt.SerializerSettings.ContractResolver;
                if (resolver != null)
                {
                    var res = resolver as DefaultContractResolver;
                    res.NamingStrategy = null;  // this removes the camelcasing
                }
            });

            MediaStoreContext.ConfigureModel();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMediaStoreAuth();
            app.UseMvc();
        }
  
    }
}
