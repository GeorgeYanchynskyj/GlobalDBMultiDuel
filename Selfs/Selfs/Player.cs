using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Selfs
{
    public class Player : IBreakable, IMoovable, IChanging
    {
        public int Health 
        {
            get { return _health; }
            set
            {
                _health = value;
                OnPlayerHealthChange(new PlayerStateArgs(this));
                if (value < 0) State = 1;
            }
        }
        private int _health;

        public Point Position
        {
            get { return _position; }
            set
            {
                _position = value;

                OnMooved(new MooveArgs(_position));
            }
        }
        private Point _position;

        public int Width { get; set; }
     
        public int Height { get; set; }
      
        public bool Rigid { get; set; }
      
        public int PlayerNumber;

        public int Reload;

        public int Burn;

        public int Speed { get; set; }
    
        public int State 
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPlayerStateChange(new PlayerStateArgs(this));
                
            }
        }
        private int _state;

        public Player(int number, int startHealth, int x, int y, int w, int h, int s)
        {
            PlayerNumber = number;
            Health = startHealth;

            Position = new Point(x, y);
            Width = w;
            Height = h;
            Rigid = true;
            Speed = s;
            
            State = 0;
            Reload = 0;
            Burn = 0;
        }
        
        public void TakeDamage(int damage)
        {
            Health = Health - damage;

            if (Health <= 0) Break();
        }

        public void Break()
        {
            //Console.WriteLine("Player {0} is dead", playernumber);
        }

        public void Moove(int deltaX, int deltaY)
        {
            
            Position = new Point(Position.X + deltaX, Position.Y + deltaY);

            
        }
     
        protected virtual void OnPlayerHealthChange(PlayerStateArgs e)
        {
            EventHandler<PlayerStateArgs> handler = PlayerHealthChange;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnPlayerStateChange(PlayerStateArgs e)
        {
            EventHandler<PlayerStateArgs> handler = PlayerStateChange;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnMooved(MooveArgs e)
        {
            
            EventHandler<MooveArgs> handler = Mooved;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void Remove()
        {
            OnRemoved(EventArgs.Empty);
        }

        protected virtual void OnRemoved(EventArgs e)
        {
            
            EventHandler<EventArgs> handler = Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<EventArgs> Removed;

        public event EventHandler<MooveArgs> Mooved;

        public event EventHandler<PlayerStateArgs> PlayerStateChange;

        public event EventHandler<PlayerStateArgs> PlayerHealthChange;

        
    }
}
