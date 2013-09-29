using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//@author: Jen Dziuba
//Instructor: Professor Cascioli
//Date: 3/18/11
//
//Poles.cs
//
//Poles.cs represents a pole in which the character either attracts or repel from.
namespace SparkCrasher
{
    class Poles
    {
        //Attributes of pole
        int charge;
        int xPos;
        int yPos;
        //Properties of pole
        //Property of pole charge
        public int Charge
        {
            get { return charge; }
        }
        //Property of pole x coordinate
        public int XPos
        {
            get { return xPos; }
        }
        //Property of pole y coordinate
        public int YPos
        {
            get { return yPos; }
        }
        //Parameterized constructor of pole
        public Poles(int c, int x, int y)
        {
            charge = c;
            xPos = x;
            yPos = y;
        }
    }
}
