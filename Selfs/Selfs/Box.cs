using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Selfs
{

    public class Box : IObject, IBreakable, IChanging
    {

        public int Health //{ get; set; }
        {
            get { return _health; }
            set
            {
                _health = value;
                
            }
        }
        private int _health;

        public Point Position { get; set; }
       
        public int Width { get; set; }       

        public int Height { get; set; }      

        public bool Rigid { get; set; }
        
        public int broken { get; set; }

        public int State 
        {
            get { return _state; }
            set
            {
                _state = value;
                OnBoxStateChange(new BoxStateArgs(this));

            }
        }
        private int _state;


        public Box(int health1 ,int x, int y, int width1, int hight1)
        {
            Health = health1;
            Position = new Point(x, y);
            Width = width1;
            Height = hight1;

            Rigid = true;
            broken = 0;
            State = 5 - Health;
        }

        public void TakeDamage(int damage)
        {
            Health = Health - damage;

            if (Health <= 0) Break();
            else State += damage;
        }

        public void Break()
        {
            broken = 1;
            
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

        protected virtual void OnBoxStateChange(BoxStateArgs e)
        {
            EventHandler<BoxStateArgs> handler = BoxStateChange;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<BoxStateArgs> BoxStateChange;

    }

    
}
