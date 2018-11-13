using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Selfs
{
    public abstract class Bonus: IObject, IPickable, ITemporary
    {

        public abstract Point Position { get; set; }


        public abstract int Width { get; set; }


        public abstract int Height { get; set; }


        public abstract bool Rigid { get; set; }
        

        public abstract void Pickup(Player p);


        public abstract void Olden();

        public abstract void Shrink();
        
        public abstract int Picked { get; set; }


        public abstract int Lifetime { get; set; }

        public abstract void Remove();           

        public abstract event EventHandler<EventArgs> Removed;

        public abstract event EventHandler<SizeArgs> Resized;
    }
}
