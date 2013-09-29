﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//@author: Jen Dziuba
//Instructor: Professor Cascioli
//Date: 3/18/11
//
//Wall.cs
//
//Wall.cs represents a wall in the game.
namespace SparkCrasher
{
    class Wall
    {
        //Attributes of wall
        int xPos;
        int yPos;
        //Property of x coordinate of wall
        public int XPos
        {
            get { return xPos; }
        }
        //Property of y coordinate of wall
        public int YPos
        {
            get { return yPos; }
        }
        //Parameterized constructor of wall
        public Wall(int x, int y)
        {
            xPos = x;
            yPos = y;
        }
    }
}