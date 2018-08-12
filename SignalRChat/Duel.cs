using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Selfs;
using Duel2;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Threading;

namespace SignalRChat
{
    public class Duel
    {
        public int ID { get; set; }        // save to ALL
        public bool Offline { get; set; }  // save
        public int Connected { get; set; }     
        public int Width { get; set; }     // save
        public int Height{ get; set; }     // save

        private int bulletNum = 0, boomNum = 0, boxNum = 0, bonusNum = 0, ghostNum = 0; 
        private Dictionary<Object, string> pageObjects = new Dictionary<object, string>();
        private GameEngine engine;
        private IHubContext context;       
        private Random rand = new Random();
        //private Thread processor;
        private Task ticker;
        private string newName;
        private int saveID = -1;

        public Duel(int _connected, GameEngine _engine, int id, bool offline)
        {
            Offline = offline;
            Connected = _connected;
            engine = _engine;
            ID = id;

            //processor = new Thread(engine.Timertick);
            ticker = new Task(engine.Timertick);
            context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
        }

        public void End()
        {
            Connected = 0;
            pageObjects = new Dictionary<object, string>();
            engine = new GameEngine();
            engine.Shoot -= page_Shoot;
            engine.Explode -= page_Explode;
            engine.NewBox -= page_NewBox;
            engine.NewBonus -= page_NewBonus;
            engine.NewGhost -= page_NewGhost;
            engine.TickEnd -= process_End;

            bulletNum = 0;
            boomNum = 0;
            boxNum = 0;
            bonusNum = 0;
            ghostNum = 0;
        }

        public void Save()
        {
            if (saveID == -1) saveID = Startup.DataBase.GetFreeID();

            Startup.DataBase.AddDuel(saveID, Offline, Width, Height);
            Startup.DataBase.AddPlayer(engine.Players[0], saveID);
            Startup.DataBase.AddPlayer(engine.Players[1], saveID);

            foreach (Box b in engine.Boxes)
            {
                Startup.DataBase.AddBox(b, saveID);
            }
            foreach (Bonus b in engine.Bonuses)
            {
                Startup.DataBase.AddBonus(b, saveID);
            }
            foreach (Wall w in engine.Walls)
            {
                Startup.DataBase.AddWall(w, saveID);
            }
          
        }

        public void LoadGame(int saveID)
        {
            engine.Shoot += page_Shoot;
            engine.Explode += page_Explode;
            engine.NewBox += page_NewBox;
            engine.NewBonus += page_NewBonus;
            engine.TickEnd += process_End;
            Width = Startup.DataBase.GetDuelParams(saveID)[0];
            Height = Startup.DataBase.GetDuelParams(saveID)[1];

            engine.Players = Startup.DataBase.GetPlayers(saveID, Convert.ToInt32(Width * 0.05), Convert.ToInt32(Height * 0.08), Consts.PlayerSpeed);
            engine.Boxes = Startup.DataBase.GetBoxes(saveID, Convert.ToInt32(Width * 0.05), Convert.ToInt32(Height * 0.08));
            engine.Bonuses = Startup.DataBase.GetBonuses(saveID, Convert.ToInt32(Width * 0.05), Convert.ToInt32(Height * 0.08));

            for (int i = 0; i < 2; i++)
            {
                context.Clients.Group(ID.ToString()).setPlayer(i);

                engine.Players[i].Mooved += page_Mooved;
                engine.Players[i].PlayerHealthChange += page_PlayerHealthChange;
                engine.Players[i].PlayerStateChange += page_PlayerStateChange;

                engine.Rigids.Add(engine.Players[i]);
                if (i == 0) pageObjects.Add(engine.Players[i], "player");
                else pageObjects.Add(engine.Players[i], "opponent");

                context.Clients.Group(ID.ToString()).poseObject(engine.Players[i].Position.X, engine.Players[i].Position.Y, pageObjects[engine.Players[i]]);
            }
            context.Clients.Group(ID.ToString()).showHealth(engine.Players[0].Health, engine.Players[1].Health);
            foreach (Box b in engine.Boxes) 
            {
                newName = "box" + boxNum.ToString();
                boxNum++;
           
                engine.Rigids.Add(b);
                b.BoxStateChange += page_BoxStateChange;
                b.Removed += page_Remove;

                pageObjects.Add(b, newName);
                context.Clients.Group(ID.ToString()).setBox(b.Position.X, b.Position.Y, newName);
                //context.Clients.Group(ID.ToString()).animBox(newName, b.State);
                b.TakeDamage(0);

            }
            if (engine.Boxes.Count < Consts.MaxBoxNumber) engine.TimeToBox = rand.Next(Consts.MinTimeToBox, Consts.MaxTimeToBox);
            else engine.TimeToBox = -1;
            foreach (Bonus b in engine.Bonuses) 
            {
                newName = "bonus" + bonusNum.ToString();
                bonusNum++;
                
                engine.Bonuses.Last().Removed += page_Remove;
                engine.TimeToBonus = rand.Next(Consts.MinTimeToBonus, Consts.MaxTimeToBonus);

                pageObjects.Add(b, newName);
                context.Clients.Group(ID.ToString()).setBonus(b.Position.X, b.Position.Y, newName);
            }
            
        }

        public void SetSize(int width, int height)
        {
            Width = width;
            Height = height;
            
            engine.Shoot += page_Shoot;
            engine.Explode += page_Explode;
            engine.NewBox += page_NewBox;
            engine.NewBonus += page_NewBonus;
            engine.NewGhost += page_NewGhost;
            engine.TickEnd += process_End;
            
        }

        public void FillPage()
        {
            context.Clients.Group(ID.ToString()).setPlayer(0);
            context.Clients.Group(ID.ToString()).setPlayer(1);
            
        }

        private void MakeBox()
        {
            newName = "box" + boxNum.ToString();
            boxNum++;

            int x = rand.Next(Convert.ToInt32(Width * 0.25), Convert.ToInt32(Width * 0.75));
            int y = rand.Next(Convert.ToInt32(Height * 0.05), Convert.ToInt32(Height * 0.85));
            Box b = new Box(Consts.BoxHealth, x, y, Convert.ToInt32(Width * 0.05), Convert.ToInt32(Height * 0.08));

            bool put = false;
            while (!put)
            {
                put = true;
                foreach (IObject i in engine.Rigids)
                {
                    if (GameEngine.Intercept(b, i))
                    {
                        put = false;
                        b.Position = new Point(rand.Next(Convert.ToInt32(Width * 0.25), Convert.ToInt32(Width * 0.75)),
                            rand.Next(Convert.ToInt32(Height * 0.05), Convert.ToInt32(Height * 0.85)));
                        break;
                    }
                }
            }
            
            engine.Boxes.Add(b);
            engine.Rigids.Add(b);
            engine.Boxes.Last().BoxStateChange += page_BoxStateChange;
            engine.Boxes.Last().Removed += page_Remove;

            if (engine.Boxes.Count < Consts.MaxBoxNumber) engine.TimeToBox = rand.Next(Consts.MinTimeToBox, Consts.MaxTimeToBox);
            else engine.TimeToBox = -1;

            pageObjects.Add(engine.Boxes.Last(), newName);

            context.Clients.Group(ID.ToString()).setBox(b.Position.X, b.Position.Y, newName);

        }

        private void MakeGhost()
        {
            newName = "ghost" + ghostNum.ToString();
            ghostNum++;

            int x = rand.Next(Convert.ToInt32(Width * 0.25), Convert.ToInt32(Width * 0.75));
            int y = rand.Next(Convert.ToInt32(Height * 0.05), Convert.ToInt32(Height * 0.85));
            Ghost g = new Ghost(x, y, Convert.ToInt32(Width * 0.05), Convert.ToInt32(Height * 0.08), 
                engine.Players[0].Position, engine.Players[1].Position, Consts.GhostSpeed, Consts.GhostDamage);

            
            engine.Ghosts.Add(g);          
            engine.Ghosts.Last().Mooved += page_Mooved;
            engine.Ghosts.Last().Removed += page_Remove;

            engine.TimeToGhost = rand.Next(Consts.MinTimeToGhost, Consts.MaxTimeToGhost); ;

            pageObjects.Add(engine.Ghosts.Last(), newName);

            context.Clients.Group(ID.ToString()).setGhost(g.Position.X, g.Position.Y, newName);

        }

        private void MakeBonus()
        {
            newName = "bonus" + bonusNum.ToString();
            bonusNum++;

            int x = rand.Next(Convert.ToInt32(Width * 0.25), Convert.ToInt32(Width * 0.75));
            int y = rand.Next(Convert.ToInt32(Height * 0.05), Convert.ToInt32(Height * 0.85));
            Healing b = new Healing(x, y, Convert.ToInt32(Width * 0.05), Convert.ToInt32(Height * 0.08), Consts.BonusLifetime);

            bool put = false;
            while (!put)
            {
                put = true;
                foreach (IObject i in engine.Rigids)
                {
                    if (GameEngine.Intercept(b, i))
                    {
                        put = false;
                        b.Position = new Point(rand.Next(Convert.ToInt32(Width * 0.25), Convert.ToInt32(Width * 0.75)),
                            rand.Next(Convert.ToInt32(Height * 0.05), Convert.ToInt32(Height * 0.85)));
                        break;
                    }
                }
            }
            
            engine.Bonuses.Add(b);          
            engine.Bonuses.Last().Removed += page_Remove;
            engine.TimeToBonus = rand.Next(Consts.MinTimeToBonus, Consts.MaxTimeToBonus);
            pageObjects.Add(engine.Bonuses.Last(), newName);

            context.Clients.Group(ID.ToString()).setBonus(b.Position.X, b.Position.Y, newName);
        }

        private void MakeBoom(Selfs.Point pos)
        {

            newName = "boom" + boomNum.ToString();
            boomNum++;
            
            engine.Booms.Add(new Boom(pos.X - Convert.ToInt32(Width * 0.03), pos.Y - Convert.ToInt32(Width * 0.02),
                Convert.ToInt32(Width * 0.07), Convert.ToInt32(Width * 0.1), Consts.BoomDamage, Consts.BoomTime));
            engine.Booms.Last().Removed += page_Remove;

            pageObjects.Add(engine.Booms.Last(), newName);

            context.Clients.Group(ID.ToString()).setBoom(pos.X - Convert.ToInt32(Width * 0.03), pos.Y - Convert.ToInt32(Height * 0.02), newName);

            foreach (Player p in engine.Players)
            {
                if (GameEngine.Intercept(p, engine.Booms.Last()))
                {

                    //((PictureBox)ObjBox[p]).Image = states[p.playernumber, 1];
                    context.Clients.Group(ID.ToString()).animPlayer(p.PlayerNumber, 1);
                    p.Burn = Consts.BurningTime;

                    engine.Booms.Last().DealDamage(p);
                    if (p.Health <= 0) context.Clients.Group(ID.ToString()).gameOver(p.PlayerNumber);

                }
            }
        }

        private void MakeBullet(int from)
        {

            newName = "bullet" + bulletNum.ToString();
            bulletNum++;
            
            engine.Bullets.Add(new Bullet(Consts.BulletSpeed, Convert.ToInt32(Width * 0.03),
                Convert.ToInt32(Height * 0.05), 1, from,
                engine.Players[from].Position.X + engine.Players[from].Width / 2,
                engine.Players[from].Position.Y + engine.Players[from].Height / 2,
                engine.Players[1 - from].Position.X + engine.Players[1 - from].Width / 2,
                engine.Players[1 - from].Position.Y + engine.Players[1 - from].Height / 2));
            engine.Bullets.Last().Mooved += page_Mooved;
            engine.Bullets.Last().Removed += page_Remove;

            engine.Players[from].Reload = Consts.RealoadTime;

            pageObjects.Add(engine.Bullets.Last(), newName);

            context.Clients.Group(ID.ToString()).setBullet(from, newName);
        }

        public void AddWall(int x, int y, int height, int width, string name)
        {
            Wall w = new Wall(width, height, x, y);

            engine.Walls.Add(w);
            engine.Rigids.Add(w);

            pageObjects.Add(w, name);
        }

        public void AddPlayer(int num, int x, int y, int height, int width, string name)
        {
            Player p = new Player(num, 10, x, y, width, height, 5);

            engine.Players[num] = p;
            engine.Players[num].Mooved += page_Mooved;
            engine.Players[num].PlayerHealthChange += page_PlayerHealthChange;
            engine.Players[num].PlayerStateChange += page_PlayerStateChange;

            engine.Rigids.Add(p);
            pageObjects.Add(p, name);

            if (num == 1)
            {
                int n = Consts.MaxBoxNumber - engine.Boxes.Count;
                for (int i = 0; i < n; i++) MakeBox();
            }
            //GroupSend(name, p.position.x.ToString() + " " + p.position.y.ToString() + " " + p.hight.ToString() + " " + p.width.ToString() + " ", gameID.ToString());
        }

        public void Process()
        {
            ticker = new Task(engine.Timertick);
            ticker.Start();
          
        }

        public void Moove(int dir, int num)
        {
            //rlySend("Control", "moove");

            if (num == 0)
            {
                if (dir == 4)
                {
                    engine.GoLeft0 = true;
                }
                if (dir == 3)
                {
                    engine.GoRight0 = true;
                }
                if (dir == 1)
                {
                    engine.GoUp0 = true;
                }
                if (dir == 2)
                {
                    engine.GoDown0 = true;
                }
                if (dir == 0)
                {
                    engine.Shoot0 = true;
                }
            }
            if (num == 1)
            {
                if (dir == 4)
                {
                    engine.GoLeft1 = true;
                }
                if (dir == 3)
                {
                    engine.GoRight1 = true;
                }
                if (dir == 1)
                {
                    engine.GoUp1 = true;
                }
                if (dir == 2)
                {
                    engine.GoDown1 = true;
                }
                if (dir == 0)
                {
                    engine.Shoot1 = true;
                }
            }
        }

        public void Stop(int dir, int num)
        {
            if (num == 0)
            {
                if (dir == 4)
                {
                    engine.GoLeft0 = false;
                }
                if (dir == 3)
                {
                    engine.GoRight0 = false;
                }
                if (dir == 1)
                {
                    engine.GoUp0 = false;
                }
                if (dir == 2)
                {
                    engine.GoDown0 = false;
                }
                if (dir == 0)
                {
                    engine.Shoot0 = false;
                }
            }
            if (num == 1)
            {
                if (dir == 4)
                {
                    engine.GoLeft1 = false;
                }
                if (dir == 3)
                {
                    engine.GoRight1 = false;
                }
                if (dir == 1)
                {
                    engine.GoUp1 = false;
                }
                if (dir == 2)
                {
                    engine.GoDown1 = false;
                }
                if (dir == 0)
                {
                    engine.Shoot1 = false;
                }
            }
        }

        //Event handling
        private void page_NewBox(object sender, EventArgs e)
        {

            MakeBox();
        }

        private void page_NewBonus(object sender, EventArgs e)
        {
            //rlySend("NewBonus", "now");
            MakeBonus();
        }

        private void page_NewGhost(object sender, EventArgs e)
        {
            //rlySend("NewBonus", "now");
            MakeGhost();
        }

        private void page_Shoot(object sender, ShootArgs e)
        {

            MakeBullet(e.From);
        }

        private void page_Explode(object sender, BoomArgs e)
        {
            MakeBoom(e.pos);
        }
        
        private void page_PlayerHealthChange(object sender, PlayerStateArgs e)
        {
            if (e.player.Health <= 0) context.Clients.Group(ID.ToString()).gameOver(e.player.PlayerNumber);
            else context.Clients.Group(ID.ToString()).showHealth(engine.Players[0].Health, engine.Players[1].Health);
        }

        private void page_PlayerStateChange(object sender, PlayerStateArgs e)
        {
            context.Clients.Group(ID.ToString()).animPlayer(e.player.PlayerNumber, e.player.State);

        }

        private void page_BoxStateChange(object sender, BoxStateArgs e)
        {
            context.Clients.Group(ID.ToString()).animBox(pageObjects[sender], e.box.State);

        }

        private void page_Mooved(object sender, MooveArgs e)
        {
            try
            {
                context.Clients.Group(ID.ToString()).poseObject(e.Pos.X, e.Pos.Y, pageObjects[sender]);
            }
            catch { }

        }

        private void page_Remove(object sender, EventArgs e)
        {
            try
            {
                context.Clients.Group(ID.ToString()).removeObj(pageObjects[sender]);
                pageObjects.Remove(sender);
            }
            catch
            {
                
            }
        }

        private void process_End(object sender, EventArgs e)
        {
            //processor.Abort();
        }

    }
}