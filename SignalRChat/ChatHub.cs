using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Duel2;


namespace SignalRChat
{
    public class ChatHub : Hub
    {
        
        // Starting

        public void Offline()
        {
            int num = Startup.Duels.Count;
            foreach (Duel d in Startup.Duels)
            {
                if (d.Connected == 0)
                {
                    num = d.ID;
                }
            }
            
            Groups.Add(Context.ConnectionId, num.ToString());

            if (num == Startup.Duels.Count) Startup.Duels.Add(new Duel(2, new GameEngine(), num, true));
            else Startup.Duels[num] = new Duel(2, new GameEngine(), num, true);

            GroupSend("Builder", "done", num.ToString());
            
            Clients.Caller.setID(num);

            Clients.Caller.getSize();
            
            Clients.Caller.getWalls();
        }

        public void Create()
        {
            int num = Startup.Duels.Count;
            foreach (Duel d in Startup.Duels)
            {
                if (d.Connected == 0)
                {
                    num = d.ID;
                }
            }
            
            Groups.Add(Context.ConnectionId, num.ToString());

            if (num == Startup.Duels.Count) Startup.Duels.Add(new Duel(1, new GameEngine(), num, false));
            else Startup.Duels[num] = new Duel(1, new GameEngine(), num, false);
           
            GroupSend("Builder", "done", num.ToString());

            Clients.Caller.setID(num);
            
        }

        public void Join()
        {
            string available = "";
            
            foreach (Duel duel in Startup.Duels)
            {
                if(duel.Connected == 1) available += duel.ID.ToString() + ", ";
            }

            if (available != "") Clients.Caller.broadcastMessage("Joiner", "Open games: " + available);
            else Clients.Caller.broadcastMessage("Joiner", "No open games");

            Clients.Caller.showGames(available);

        }

        public void JoinGame(int n)
        {
            //int n = Convert.ToInt32(sn);
            if (Startup.Duels[n].Connected < 2)
            {
                Startup.Duels[n].Connected = 2;
                Groups.Add(Context.ConnectionId, n.ToString());
                GroupSend("Players", "ready", n.ToString());
                Clients.Caller.setID(n);

                Startup.Duels[n].FillPage();
                Clients.Group(n.ToString()).setInit();
            }
            else Clients.Caller.broadcastMessage(n.ToString(), "Unavailable");
        }

        public void Save(int gameID)
        {
            Startup.Duels[gameID].Save();

            GroupSend("Saver", "done", gameID.ToString());
        }

        public void Load()
        {
            string available = "";

            foreach (int id in Startup.DataBase.GetAllID())
            {
                available += id.ToString() + ", ";
            }

            if (available != "") Clients.Caller.broadcastMessage("Loader", "Saved games: " + available);
            else Clients.Caller.broadcastMessage("Loader", "No saved games");

            Clients.Caller.showGames(available);
        }

        public void LoadGame(int saveID)
        {
            int num = Startup.Duels.Count;
            foreach (Duel d in Startup.Duels)
            {
                if (d.Connected == 0)
                {
                    num = d.ID;
                }
            }
            Groups.Add(Context.ConnectionId, num.ToString());
            
            if (num == Startup.Duels.Count) Startup.Duels.Add(new Duel(2, new GameEngine(), num, true));
            else Startup.Duels[num] = new Duel(2, new GameEngine(), num, true);

            Clients.Caller.setID(num);

            Startup.Duels[num].LoadGame(saveID);
            Clients.Caller.getWalls();
            
            GroupSend("Loader", "done", num.ToString());

            Clients.Caller.setInit();

        }

        public void GameOver(int gameID)
        {
            //Startup.Duels.Remove(Startup.Duels[gameID]);
            Startup.Duels[gameID].End();
            GroupSend("Game", "Over", gameID.ToString());
            Groups.Remove(Context.ConnectionId, gameID.ToString());
        }

        // Preparations
        public void SetSize(int width, int height, int gameID)
        {
            GroupSend("Size", Startup.Duels[gameID].Width.ToString() + " " + Startup.Duels[gameID].Height.ToString(), gameID.ToString());
            Startup.Duels[gameID].SetSize(width, height);
            
            if(Startup.Duels[gameID].Offline)
            {
                Startup.Duels[gameID].FillPage();
                Clients.Group(gameID.ToString()).setInit();
            }
        }
        
        public void AddPlayer(int num, int x, int y, int height, int width, string name, int gameID)
        {
           
            Startup.Duels[gameID].AddPlayer(num, x, y, height, width, name);
            GroupSend(name, x.ToString() + " " + y.ToString() + " " + height.ToString() + " " + width.ToString() + " ", gameID.ToString());
        }

        public void AddWall(int x, int y, int height, int width, string name, int gameID)
        {
            
            Startup.Duels[gameID].AddWall(x, y, height, width, name);

        }
        
        // Events
        public void Tick(int gameID)
        {
            
            Startup.Duels[gameID].Process();
        }

        public void Moove(int dir, int num, int gameID)
        {
            
            Startup.Duels[gameID].Moove(dir, num);
        }

        public void Stop(int dir, int num, int gameID)
        {
            Startup.Duels[gameID].Stop(dir, num);
        }

        // Chating
        public void rlySend(string name, string message)
        {
            Clients.All.broadcastMessage(name, message + ", bastards");
            
        }

        public void GroupSend(string name, string message, string group)
        {
            Clients.Group(group).broadcastMessage(name, message + ", groupMates");
        }

        public void Send(string name, string message)
        {
            rlySend(name, message);
        }

        public void Hello()
        {
            Clients.All.hello();
        }
        
    }
}