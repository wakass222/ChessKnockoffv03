using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
[assembly: OwinStartupAttribute(typeof(ChessKnockoff.Startup))]

namespace ChessKnockoff
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //Create the signalir middleware
            app.MapSignalR();
            //Require that all hubs require authenticaion
            GlobalHost.HubPipeline.RequireAuthentication();
        }
    }
}
