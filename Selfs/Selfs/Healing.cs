using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Selfs
{
    public class Healing: Bonus
    {

        public override Point Position { get; set; }


        public override int Width { get; set; }


        public override int Height { get; set; }


        public override bool Rigid { get; set; }


        public override int Picked { get; set; }


        public override int Lifetime { get; set; }


        public int HealPower { get; set; }


        public Healing(int x, int y, int width1, int hight1, int lifetime1)
        {

            Position = new Point(x, y);
            Width = width1;
            Height = hight1;
            Lifetime = lifetime1;

            Rigid = false;
            HealPower = 3;
            Picked = 0;
        }

        public override void Pickup(Player p)
        {

            p.Health = p.Health + HealPower;
            Picked = 1;

        }

        public override void Olden()
        {
            if (Lifetime > 0) Lifetime--;
            
        }

        public override void Shrink()
        {
           
            Width--;
            Height--;
            OnResized(new SizeArgs(Height, Width));
        }
        
        public override void Remove()
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

        public override event EventHandler<EventArgs> Removed;

        protected virtual void OnResized(SizeArgs e)
        {
          
            EventHandler<SizeArgs> handler = Resized;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public override event EventHandler<SizeArgs> Resized;
    }
}
