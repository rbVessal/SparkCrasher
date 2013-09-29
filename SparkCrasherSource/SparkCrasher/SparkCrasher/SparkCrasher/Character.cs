﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
//@author: Jen Dziuba
//Instructor: Professor Cascioli
//Date: 3/18/11
//
//Character.cs
//
//Character.cs represents a character with different charges: positive, neutral, and negative.
//Some physics attributes associated with character.
namespace SparkCrasher
{
    class Character
    {
        int charge; //0 = neutral  1 = pos  -1 = neg
        bool isOrbiting; // track if player is orbiting a pole
        int xPos; // current x position
        int yPos; // current y position

        int xSpeed; // x-axis speed
        int ySpeed; // y-axis speed
        double angle; // reference for drawing the sprite

        Rectangle prevLocation;//for the follow effect 
        //Note: These three need to be reset for each level!

        //constructor----------------------------------------------------------------------------------------------
        public Character()
        {
            charge = 0;
            isOrbiting = false;
            xPos = 0;
            yPos = 0;
            xSpeed = Constants.START_SPEED;
            ySpeed = Constants.START_SPEED;
            angle = Constants.START_ANGLE;
        }


        //Properties-------------------------------------------------------------------------------------------------
        public int Charge
        {
            get{ return charge; }
            set { charge = value; }
        }

        public bool IsOrbiting
        {
            get { return isOrbiting; }
            set { isOrbiting = value; }
        }

        public int XPos
        {
            get { return xPos; }
            set { xPos = value; }
        }

        public int YPos
        {
            get { return yPos; }
            set { yPos = value; }
        }

        public int XSpeed
        {
            get { return xSpeed; }
            set { xSpeed = value; }
        }

        public int YSpeed
        {
            get { return ySpeed; }
            set { ySpeed = value; }
        }

        public double Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        public Rectangle PrevLocation
        {
            get { return prevLocation; }
            set { prevLocation = value; }
        }
    }
}
