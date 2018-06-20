using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LoanCalculator.Startup))]
namespace LoanCalculator
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
