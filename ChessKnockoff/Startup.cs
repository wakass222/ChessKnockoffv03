using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ChessKnockoff.Startup))]
namespace ChessKnockoff
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
