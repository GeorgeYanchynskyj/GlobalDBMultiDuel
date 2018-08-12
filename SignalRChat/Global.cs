using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace SignalRChat
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            //RouteTable.Routes.MapHubs();
        }
    }
}