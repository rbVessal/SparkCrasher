using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SparkCrasher
{
    class Balloon
    {
        //Attributes of goal
        int x;
        int y;
        Animation anim;
        bool popped;

        //Property for x coordinate of goal
        public int X
        {
            get { return x; }
            set { x = value; }
        }
        //Property for y coordinate of goal
        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public Balloon(int _x, int _y)
        {
            X = _x;
            Y = _y;
        }
        public Animation Anim
        {
            get { return anim; }
            set { anim = value; }
        }
        public Boolean Popped
        {
            get { return popped; }
            set { popped = value; }
        }

        //takes in an animation
        //returns true when the animation is over
        public void Death()
        {
            anim.CurrentFrame = 0;
            anim.sourceRect.Y = 100;
            anim.TotalFrames = 4;
        }
    }
}
