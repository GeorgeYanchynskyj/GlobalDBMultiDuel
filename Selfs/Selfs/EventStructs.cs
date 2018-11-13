using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selfs
{

    public class ShootArgs : EventArgs
    {
        public int From { get; set; }
        
        public ShootArgs(int _from)
        {
            From = _from;
        }
    }

    public class SizeArgs: EventArgs
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public SizeArgs(int _height, int _width)
        {
            Width = _width;
            Height = _height;
        }
    }

    public class MooveArgs : EventArgs
    {
        public Point Pos { get; set; }

        public MooveArgs(Point _pos)
        {
            Pos = _pos;
        }
    }

    

    public class PlayerStateArgs: EventArgs
    {
        public Player player { get; set; }

        public PlayerStateArgs(Player _player)
        {
            player = _player;
        }
    }

    public class BoxStateArgs : EventArgs
    {
        public Box box { get; set; }

        public BoxStateArgs(Box _box)
        {
            box = _box;
        }
    }

    public class BoomArgs : EventArgs
    {
        public Selfs.Point pos { get; set; }

        public BoomArgs(Selfs.Point _pos)
        {
            pos = _pos;
        }
    }

    public class NoBoomArgs : EventArgs
    {
        public Boom boom { get; set; }

        public NoBoomArgs(Boom _boom)
        {
            boom = _boom;
        }
    }
    
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x,int y)
        {
            X = x;
            Y = y;
        }

        public static double range(int x1, int y1, int x2, int y2)
        {
            return Convert.ToInt32(Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)));
        }
    }

}
