using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Selfs
{
    public class Wall: IObject
    {

        public Point Position { get; set; }


        public int Width { get; set; }


        public int Height { get; set; }


        public bool Rigid { get; set; }


        public Wall( int w, int h, int x1, int y1)
        {

            Position = new Point(x1, y1);

            Width = w;
            Height = h;

            Rigid = true;

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
