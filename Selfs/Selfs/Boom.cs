using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Selfs
{
    public class Boom: IObject, IDamaging, ITemporary
    {
        public Boom(int x, int y, int width1, int hight1, int damage1, int lifetime1)
        {
            Position = new Point(x, y);
            Width = width1;
            Height = hight1;
            Damage = damage1;
            Lifetime = lifetime1;

            Rigid = false;
            
        }

        public int Lifetime { get; set; }


        public Point Position { get; set; }


        public int Width { get; set; }


        public int Height { get; set; }


        public bool Rigid { get; set; }


        public int Damage { get; set; }


        public void DealDamage(IBreakable obj)
        {
            obj.TakeDamage(Damage);
        }

        public void Olden()
        {
            if (Lifetime > 0) Lifetime--;
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
    }
}
