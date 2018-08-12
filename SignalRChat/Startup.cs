using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using GamesBase;

[assembly: OwinStartup(typeof(SignalRChat.Startup))]

namespace SignalRChat
{
    public class Startup
    {
        public static DataBaseService DataBase = new DataBaseService();
        public static List<Duel> Duels = new List<Duel>();

        public void Configuration(IAppBuilder app)
        {
          
            app.MapSignalR();
            
        }
    }
}
