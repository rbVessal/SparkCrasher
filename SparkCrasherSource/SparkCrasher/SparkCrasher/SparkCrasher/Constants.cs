﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//@author: Jen Dziuba
//Instructor: Professor Cascioli
//Date: 3/18/11
//
//Constants.cs
//
//Constants.cs contains the game constants
namespace SparkCrasher
{
    class Constants
    {
        public const int ATTRACTION = 2; // factor for acceleration based on attraction - mininum: 2
        public const int REPULSION = 10; // factor for acceleration based on repulsion (not implemented)
        public const int GRAVITY = 1; // foactor for acceleration based on gravity
        public const int START_SPEED = 0; // starting speed for each level
        public const int MAX_SPEED = 15; // maximum speed
        public const double START_ANGLE = 0.0; // starting angle for drawing reference
        public const int SCREEN_WIDTH = 800; // set screen width
        public const int SCREEN_HEIGHT = 800; // set screen height
        public const double SONG_OFFSET = 500; // offset for switching song loops

        public const int ROTATION_SPEED = 270; // speed of angle change while rotating - leave at 270!
        public const int DECREASE_ROTATION_SPEED = -270;
        public const int ORBIT_SPEED = 1;

        // for the level loader
        public const int ARRAYX = 16; //AF-For size of array relative to screen size
        public const int ARRAYY = 16; //AF-For size of array relative to screen size
        public const int GRIDPOSITION = 50; //AF-Not really a grid, just a name for the variable that sets where different objects go.

        // Note: because of their use in calculations, these should reflect the base tile size
        public const int TILE_WIDTH = 50; // change to reflect player sprite width
        public const int TILE_HEIGHT = 50; // change to reflect player sprite height
    }
}
