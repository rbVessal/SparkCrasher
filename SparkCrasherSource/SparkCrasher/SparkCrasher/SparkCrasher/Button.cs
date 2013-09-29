using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@author Jen Dziuba
//Instructor: Professor Cascioli
//Reference buttons for MapMaker.cs

namespace SparkCrasher
{
    class Button
    {
        int x;
        int y;
        char thing;

        public int X
        {
            get { return x; }
            set { x = value; }
        }
        public int Y
        {
            get { return y; }
            set { y = value; }
        }
        public char Thing
        {
            get { return thing; }
            set { thing = value; }
        }
    }
}
