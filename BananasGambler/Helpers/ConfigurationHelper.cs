using BananasGambler.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BananasGambler.Helpers
{
    internal class ConfigurationHelper
    {
        private IConfigurationRoot configurationRoot;

        public ConfigurationHelper()
        {
            //this.configurationRoot = new ConfigurationBuilder()
            //    .SetBasePath(Path.GetFullPath(Directory.GetCurrentDirectory()))
            //    .AddJsonFile("appsettings.json", optional: true)
            //    .Build();
        }

        public string BaseUrl 
        {
            get 
            {
                //IConfigurationSection section = this.configurationRoot.GetSection(nameof(HttpClientConfiguration));
                //return section.GetValue<string>("BaseUrl");
                //return "http://192.168.88.117:5086";
                return "http://192.168.88.252:5086";
            }
        }

        public int Port 
        {
            get 
            {
                IConfigurationSection section = this.configurationRoot.GetSection(nameof(HttpClientConfiguration));
                return section.GetValue<int>("Port");
            }
        }
    }
}
