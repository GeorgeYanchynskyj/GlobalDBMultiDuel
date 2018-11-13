using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;


namespace Selfs
{

    public interface IBreakable
    {
        int Health { get; set; }
        
        void TakeDamage(int damage);

        void Break();
    }

    public interface IDamaging
    {
        int Damage { get; set; }
       
        void DealDamage(IBreakable obj);
    }

    public interface IPickable
    {

        void Pickup(Player p);

    }

    public interface IMoovable:IObject
    {
        int Speed { get; set; }
        
        void Moove(int deltaX, int deltaY);

        event EventHandler<MooveArgs> Mooved;
    }

    public interface IObject
    {
        Point Position { get; set; }

        int Width { get; set; }
        
        int Height { get; set; }      

        bool Rigid { get; set; }
        
        void Remove();

        event EventHandler<EventArgs> Removed;
    }

    public interface ITemporary
    {
        int Lifetime { get; set; }

        void Olden();
    }

    public interface IChanging
    {

        int State { get; set; }

    }
    
}
