using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//@author:  Jen Dzuiba, Rebecca Vessal
//Instructor: Professor Cascioli
//Date: 3/18/11 - 4/26/11
//
//Mechanics.cs
//
//Mechanics.cs represents the game mechanics
namespace SparkCrasher
{
    class Mechanics
    {
        double radiusOfRotation;
        int counter;
        
        #region Attraction
        //accelerations ------------------------------------------------
        public Character Attraction(Poles pole, Character player)
        {
            //Constants.ATTRACTION
            //pulled towards a pole
            if (player.Charge != 0 && pole.Charge == -(player.Charge))
            {
                // calculate x,y distances between player and pole
                int xDis = pole.XPos - player.XPos + 70;
                int yDis = pole.YPos - player.YPos + Constants.TILE_HEIGHT;
                // find third side of triangle based on calculated distances
                double tDis = Math.Sqrt(Math.Pow(xDis, 2) + Math.Pow(yDis, 2));
                // scale the distances based on the attraction factor
                double scale = Constants.ATTRACTION / tDis;
                // use scale and calculated distances to get how much the player accelerates
                double sX = Math.Abs((scale * xDis));
                double sY = Math.Abs((scale * yDis));

                // use calulated acceleration to change player's speed
                if (player.XPos > (pole.XPos + Constants.TILE_WIDTH))
                {
                    player.XSpeed -= (int)(sX);
                }
                else if ((player.XPos + Constants.TILE_WIDTH) < pole.XPos)
                {
                    player.XSpeed += (int)(sX);
                }

                if (player.YPos > (pole.YPos + Constants.TILE_HEIGHT))
                {
                    player.YSpeed -= (int)(sY);
                }
                else if ((player.YPos + Constants.TILE_HEIGHT) < pole.YPos)
                {
                    player.YSpeed += (int)(sY);
                }
            }

            return player;
        }
        #endregion

        #region Repulsion

        //Create a method that will repel the character from a pole if they have the same charge
        public void Repulsion(Poles pole, Character player)
        {
            
            //If the player charge is not neutral and the pole and player charge are the same
            if (player.Charge != 0 && pole.Charge == player.Charge)
            {
                
                //Change the player's position based on what quadrant they are in
                //0 or 360 degree
                if (player.XPos > pole.XPos)
                {
                    //Increase the player's x speed
                    player.XSpeed += Constants.REPULSION;
                    
                }
                //If 180
                else if (player.XPos < pole.XPos)
                {
                    //Decrease the player's x speed
                    player.XSpeed -= Constants.REPULSION;

                }

                //If 90 degree
                if (player.YPos < pole.YPos)
                {
                    //Decrease the player's y speed
                    player.YSpeed -= Constants.REPULSION;
                    
                }
                
                //If 270
                else if (player.YPos > pole.YPos)
                {
                    //Increase the player's y speed
                    player.YSpeed += Constants.REPULSION;
                    
                }
                
            }
        }
        #endregion

        #region Gravity and Bounce
        public int Gravity(int y)
        {
            //Constants.GRAVITY
            //free falling downward
            y += Constants.GRAVITY;
            return y;
        }

        public bool Bounce(Wall wall, Character player)
        {
            // check if player is bouncing off a wall
            if (new Rectangle(wall.XPos, wall.YPos, Constants.TILE_WIDTH, Constants.TILE_HEIGHT).Intersects(
                new Rectangle(player.XPos, player.YPos, Constants.TILE_WIDTH, Constants.TILE_HEIGHT)))
            {
                return true;
            }
            else { return false; }
        }
        #endregion

        #region Solid Wall
        public Character SolidWall(Wall wall, Character player) // works with Bounce, currently buggly & making workaround
        {
            // prevent player from moving through a collided wall
            int dX = (wall.XPos - player.XPos);
            int dY = (wall.YPos - player.YPos);
            if (dX > dY) {
                if (player.XPos < wall.XPos)
                {
                    player.XPos = (wall.XPos - Constants.TILE_WIDTH);
                }
                if (player.XPos > wall.XPos)
                {
                    player.XPos = (wall.XPos + Constants.TILE_WIDTH);
                }
                player.XSpeed = 0;
                player.YSpeed = -(player.YSpeed);
            }
            if (dY > dX) {
                if (player.YPos < wall.YPos)
                {
                    player.YPos = (wall.YPos - Constants.TILE_HEIGHT);
                }
                if (player.YPos > wall.YPos)
                {
                    player.YPos = (wall.YPos + Constants.TILE_HEIGHT);
                }
                player.YSpeed = 0;
                player.XSpeed = -(player.XSpeed);
            }

            return player;
        }
        #endregion

        #region Orbit
        //Create a method that will make the player orbit counter clockwise around the pole
        public double Rotate(Poles pole, Character player, double angleChange)
        {
            //Pole rectangle
            Rectangle poleRect = new Rectangle(pole.XPos, pole.YPos, Constants.TILE_WIDTH, Constants.TILE_HEIGHT);
            //Player rectangle
            Rectangle playerRect = new Rectangle(player.XPos, player.YPos, Constants.TILE_WIDTH, Constants.TILE_HEIGHT);

            //Calculate the center points of pole and player
            double centerOfPlayerX = (playerRect.Width / 2) + playerRect.X;
            double centerOfPlayerY = (playerRect.Height / 2) + playerRect.Y;
            double centerOfPoleX = (poleRect.Width / 2) + poleRect.X;
            double centerOfPoleY = (poleRect.Height / 2) + poleRect.Y;

            //Calculate the angle for each quadrant only once
            if (counter == 0)
            {
                //When x and y are positive
                if (centerOfPlayerY > centerOfPoleY && centerOfPlayerX > centerOfPoleX)
                {
                    //Calculate angle
                    player.Angle = Math.Atan((centerOfPlayerY - centerOfPoleY) / (centerOfPlayerX - centerOfPoleX));
                }
                //When x is positve, but y is negative
                else if (centerOfPoleY > centerOfPlayerY && centerOfPlayerX > centerOfPoleX)
                {
                    player.Angle = Math.Atan((centerOfPoleY - centerOfPlayerY) / (centerOfPlayerX - centerOfPoleX));
                }
                //When x and y are negative
                else if (centerOfPoleY > centerOfPlayerY && centerOfPoleX > centerOfPlayerX)
                {
                    player.Angle = Math.Atan((centerOfPoleY - centerOfPlayerY) / (centerOfPoleX - centerOfPlayerX));
                }
                //When x is negative but y is positive
                else if (centerOfPlayerY > centerOfPoleY && centerOfPoleX > centerOfPlayerX)
                {
                    player.Angle = Math.Atan((centerOfPlayerY - centerOfPoleY) / (centerOfPoleX - centerOfPlayerX));
                }
                //Account for 90, 180, 270, 360 angle degrees
                //If 90 degrees
                else if (centerOfPlayerX == centerOfPoleX && centerOfPlayerY < centerOfPoleY)
                {
                    player.Angle = 90;
                }
                //If 180 degrees
                else if (centerOfPlayerY == centerOfPoleY && centerOfPlayerX < centerOfPoleX)
                {
                    player.Angle = 180;
                }
                //If 270 degrees
                else if (centerOfPlayerX == centerOfPoleX && centerOfPlayerY > centerOfPoleY)
                {
                    player.Angle = 270;
                }
                //If 360 degrees
                else if (centerOfPlayerY == centerOfPoleY && centerOfPlayerX > centerOfPoleX)
                {
                    player.Angle = 360;
                }
                //Incerement counter
                counter++;
            }
            if (counter > 0)
            {
                //Increment the angle over time
                player.Angle += angleChange;
                //If the angle goes over 360 then mod it to make it zero again
                if (player.Angle > double.MaxValue - 360000)
                {
                    player.Angle = player.Angle % double.MaxValue - 360000;
                }
            }
            return player.Angle;
        }

        //Create a method that will make the player orbit clockwise around the pole
        public void Orbit(Poles pole, Character player)
        {
            //Pole rectangle
            Rectangle poleRect = new Rectangle(pole.XPos, pole.YPos, Constants.TILE_WIDTH, Constants.TILE_HEIGHT);
            //Player rectangle
            Rectangle playerRect = new Rectangle(player.XPos, player.YPos, Constants.TILE_WIDTH, Constants.TILE_HEIGHT);

            //Calculate the center points of pole and player
            double centerOfPlayerX = (playerRect.Width / 2) + playerRect.X;
            double centerOfPlayerY = (playerRect.Height / 2) + playerRect.Y;
            double centerOfPoleX = (poleRect.Width / 2) + poleRect.X;
            double centerOfPoleY = (poleRect.Height / 2) + poleRect.Y;

            //Calculate the radius of the rotation
            if (counter == 0)
            {
                //When x and y are positive
                if (centerOfPlayerY > centerOfPoleY && centerOfPlayerX > centerOfPoleX)
                {
                    //Use pyrathogram theorem
                    radiusOfRotation = Math.Sqrt((Math.Pow((centerOfPlayerX - centerOfPoleX), 2) +
                        Math.Pow((centerOfPlayerY - centerOfPoleY), 2)));
                }
                //When x is positve, but y is negative
                else if (centerOfPoleY > centerOfPlayerY && centerOfPlayerX > centerOfPoleX)
                {
                    //Use pyrathogram theorem
                    radiusOfRotation = Math.Sqrt((Math.Pow((centerOfPlayerX - centerOfPoleX), 2) +
                        Math.Pow((centerOfPoleY - centerOfPlayerY), 2)));
                }
                //When x and y are negative
                else if (centerOfPoleY > centerOfPlayerY && centerOfPoleX > centerOfPlayerX)
                {
                    //Use pyrathogram theorem
                    radiusOfRotation = Math.Sqrt((Math.Pow((centerOfPoleX - centerOfPlayerX), 2) +
                        Math.Pow((centerOfPoleY - centerOfPlayerY), 2)));
                }
                //When x is negative but y is positive
                else if (centerOfPlayerY > centerOfPoleY && centerOfPoleX > centerOfPlayerX)
                {
                    //Use pyrathogram theorem
                    radiusOfRotation = Math.Sqrt((Math.Pow((centerOfPoleX - centerOfPlayerX), 2) +
                        Math.Pow((centerOfPlayerY - centerOfPoleY), 2)));
                }
                //Account for 90, 180, 270, 360 angle degrees
                //If 90 degrees
                else if (centerOfPlayerX == centerOfPoleX && centerOfPlayerY < centerOfPoleY)
                {
                    //Use pyrathogram theorem
                    radiusOfRotation = Math.Sqrt((Math.Pow((centerOfPoleX), 2) +
                        Math.Pow((centerOfPoleY - centerOfPlayerY), 2)));
                }
                //If 180 degrees
                else if (centerOfPlayerY == centerOfPoleY && centerOfPlayerX < centerOfPoleX)
                {
                    //Use pyrathogram theorem
                    radiusOfRotation = Math.Sqrt((Math.Pow((centerOfPoleX - centerOfPlayerX), 2) +
                        Math.Pow((centerOfPoleY), 2)));
                }
                //If 270 degrees
                else if (centerOfPlayerX == centerOfPoleX && centerOfPlayerY > centerOfPoleY)
                {
                    //Use pyrathogram theorem
                    radiusOfRotation = Math.Sqrt((Math.Pow((centerOfPoleX), 2) +
                        Math.Pow((centerOfPlayerY - centerOfPoleY), 2)));
                }
                //If 360 degrees
                else if (centerOfPlayerY == centerOfPoleY && centerOfPlayerX > centerOfPoleX)
                {
                    //Use pyrathogram theorem
                    radiusOfRotation = Math.Sqrt((Math.Pow((centerOfPlayerX - centerOfPoleX), 2) +
                        Math.Pow((centerOfPoleY), 2)));
                }

            }

            //Calculate the player's angle and set it
            player.Angle = Rotate(pole, player, Constants.ROTATION_SPEED);

            //Calculate the attractive force between player and pole to find the new x and y coordinates for the player
            double attractiveForceX = Math.Sin(player.Angle) * radiusOfRotation + centerOfPoleX;
            double attractiveForceY = Math.Cos(player.Angle) * radiusOfRotation + centerOfPoleY;
            //Set the new x and y coordinates of the player to the attractive force
            player.XPos = (int)attractiveForceX - playerRect.Width / 2;
            player.YPos = (int)attractiveForceY - playerRect.Height / 2;
        }

        
        #endregion

        #region Collisions
        //collision ----------------------------------------------------
        public Character WallCollision(Character player)
        {
            // reverse player's direction due to hitting a wall
            player.XSpeed = (-player.XSpeed);
            player.YSpeed = (-player.YSpeed);
            return player;
        }

        public bool PoleCollision(Poles pole, Character player)
        {
            // check if player is touching a pole
            Rectangle po = new Rectangle(pole.XPos, pole.YPos, Constants.TILE_WIDTH, Constants.TILE_HEIGHT);
            Rectangle pl = new Rectangle(player.XPos, player.YPos, Constants.TILE_WIDTH, Constants.TILE_HEIGHT);
            if (po.Intersects(pl))
            {
                return true;
            }
            else { return false; }
        }
        #endregion
    }
}
