using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace SignalRChat
{
    public class ChatHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        public void Send(string name, string message)
        {
            Console.WriteLine("mess: " + message);

            Class1 c = new Class1();
            string newmes = c.op(message, name);

            // Call the broadcastMessage method to update clients.
            //Clients.All.broadcastMessage(name, newmes+", bastards");
            rlySend(name, newmes);

            Console.WriteLine("mess: "+message);
            Console.WriteLine("mess: " + message);
            Console.WriteLine("mess: " + message);
            Console.WriteLine("mess: " + message);
            Console.WriteLine("mess: " + message);

        }

        public void rlySend(string name, string message)
        {
            Clients.All.broadcastMessage(name, message + ", bastards");
        }
    }
}