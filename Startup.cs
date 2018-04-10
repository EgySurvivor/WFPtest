using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WFPtest.Startup))]
namespace WFPtest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
