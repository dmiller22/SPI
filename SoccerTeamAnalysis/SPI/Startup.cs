using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SPI.Startup))]
namespace SPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
