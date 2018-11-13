using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selfs
{
    public class Ghost: IMoovable,IDamaging
    {
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


        public int StepX { get; set; }
        public int StepY { get; set; }
        public int Target { get; set; }
        public int Hit { get; set; }

        public Ghost(int x, int y, int width, int height, Point pos0, Point pos1, int speed, int damage)
        {
            Position = new Point(x, y);

            Width = width;
            Height = height;
            Speed = speed;
            Damage = damage;
            Rigid = false;        
            Hit = 0;

            ChangeTarget(pos0, pos1);

        }

        public void ChangeTarget(Point pos0, Point pos1)
        {
            if (Point.range(pos0.X, pos0.Y, Position.X, Position.Y) < Point.range(pos1.X, pos1.Y, Position.X, Position.Y))
            {
                StepY = Convert.ToInt32(Speed * (Math.Abs(Position.Y - pos0.Y)) / Point.range(pos0.X, pos0.Y, Position.X, Position.Y));
                StepX = Convert.ToInt32(Speed * (Math.Abs(Position.X - pos0.X)) / Point.range(pos0.X, pos0.Y, Position.X, Position.Y));
                if (pos0.Y < Position.Y) StepY = -StepY;
                if (pos0.X < Position.X) StepX = -StepX;
                Target = 0;
            }
            else
            {
                StepY = Convert.ToInt32(Speed * (Math.Abs(Position.Y - pos1.Y)) / Point.range(pos1.X, pos1.Y, Position.X, Position.Y));
                StepX = Convert.ToInt32(Speed * (Math.Abs(Position.X - pos1.X)) / Point.range(pos1.X, pos1.Y, Position.X, Position.Y));
                if (pos1.Y < Position.Y) StepY = -StepY;
                if (pos1.X < Position.X) StepX = -StepX;
                Target = 1;
            }
        }

        public void DealDamage(IBreakable obj)
        {
            obj.TakeDamage(Damage);
        }

        public void Moove(int deltaX, int deltaY)
        {

            Position = new Point(Position.X + deltaX, Position.Y + deltaY);


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
    }
}
