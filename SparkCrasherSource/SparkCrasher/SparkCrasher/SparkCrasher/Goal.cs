using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//@author: Jen Dziuba
//Instructor: Professor Cascioli
//Date: 3/18/11
//
//Goal.cs
//
//Goal.cs contains properties and attributes for the coordinates of the goal in the game.
namespace SparkCrasher
{
    class Goal
    {
        //Attributes of goal
        int x;
        int y;
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
    }
}
