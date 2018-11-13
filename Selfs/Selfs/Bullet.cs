using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Selfs
{
    public class Bullet : IDamaging, IMoovable
    {
     
        public int stepX, stepY;
        public int owner;
        public int hit;

        public Bullet( int speed1, int width1, int hight1, int damage1, int owner1, int x1, int y1, int x2, int y2)
        {
          
            Position = new Point(x1, y1);

            Width = width1;
            Height = hight1;
            Speed = speed1;
            Damage = damage1;
            Rigid = true;
            hit = 0;

            owner = owner1;

            stepY = Convert.ToInt32(Speed * (Math.Abs(y2 - y1)) / Point.range(x1, y1, x2, y2));
            stepX = Convert.ToInt32(Speed * (Math.Abs(x2 - x1)) / Point.range(x1, y1, x2, y2));
            if (y1 > y2) stepY = -stepY;
            if (x1 > x2) stepX = -stepX;

        }

        
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
      

        public int Speed { get; set; }
      

        public int Damage { get; set; }
      

        public void DealDamage(IBreakable obj)
        {
            obj.TakeDamage(Damage);
        }

        public void Moove(int deltaX, int deltaY)
        {
            
            Position = new Point(Position.X + deltaX, Position.Y + deltaY);
            
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
        
        protected virtual void OnMooved(MooveArgs e)
        {
            
            EventHandler<MooveArgs> handler = Mooved;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<MooveArgs> Mooved;

    }
}
