using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
//@author:  Anne Fonicello
//Instructor: Professor Cascioli
//Date: 3/18/11
//
//LevelLoader.cs
//
//Loads the selected level
namespace SparkCrasher
{
    class LevelLoader
    {
        //Attributes
        char[,] position = new char[Constants.ARRAYY, Constants.ARRAYX];
        List<String> tempLine = new List<String>();
        Character player = new Character();
        Goal goal = new Goal();
        List<Wall> walls = new List<Wall>();
        List<Poles> poles = new List<Poles>();
        List<Balloon> balloons = new List<Balloon>();

        //Methods

        //AF-Load Methods, populates an array with characters in what will be their correct position on the screen
        public void Load(String fn)//Takes a file name
        {
            //AF-Set up stream reader...Havent done this in a while
            StreamReader input = new StreamReader(fn);

            String line = "";
            //AF-char symbol = '';

            while ((line = input.ReadLine()) != null)
            {
                //AF-Adding individual lines to be broken into chars
                tempLine.Add(line);
            }

            for (int i = 0; i < tempLine.Count; i++)
            {
                for (int j = 0; j < tempLine[i].Length; j++)
                {
                    position[i, j] = (tempLine[i].Substring(j, 1))[0];
                }
            }
        }

        /// <summary>
        /// AF
        /// Sets the start position for Crashy, Maybe this should be put in the character Class instead
        /// I'm not sure. Also, I don't know if the way I'm setting it is right
        /// This is subject to major changes.
        /// </summary>
        public Character SetStart(Character player)
        {
            for (int i = 0; i < Constants.ARRAYY; i++)
            {
                for (int j = 0; j < Constants.ARRAYX; j++)
                {
                    if (position[i, j] == 'S')
                    {
                        player.XPos = j * Constants.GRIDPOSITION;
                        player.YPos = i * Constants.GRIDPOSITION;
                    }
                }
            }
            return player;
        }

        // set the location of the goal within the level
        public Goal SetFinish(Goal goal)
        {
            for (int i = 0; i < Constants.ARRAYY; i++)
            {
                for (int j = 0; j < Constants.ARRAYX; j++)
                {
                    if (position[i, j] == 'G')
                    {
                        goal.X = j * Constants.GRIDPOSITION;
                        goal.Y = i * Constants.GRIDPOSITION;
                    }
                }
            }
            return goal;
        }

        /// <summary>
        /// AF
        /// Method for adding in all walls and their positions. 
        /// Returns a list of Wall objects
        /// </summary>
        /// <returns></returns>
        public List<Wall> SetWalls()
        {
            //AF -Adding walls to list
            for (int i = 0; i < Constants.ARRAYY; i++)
            {
                for (int j = 0; j < Constants.ARRAYX; j++)
                {
                    if (position[i, j] == '=')
                    {
                        Wall w = new Wall(j * Constants.GRIDPOSITION, i * Constants.GRIDPOSITION);
                        walls.Add(w);
                    }
                }
            }

            return walls;
        }
        public List<Poles> SetPoles()
        {
            //AF- Adding positive poles to list
            for (int i = 0; i < Constants.ARRAYY; i++)
            {
                for (int j = 0; j < Constants.ARRAYX; j++)
                {
                    if (position[i, j] == '+')
                    {
                        Poles p = new Poles(1, j * Constants.GRIDPOSITION, i * Constants.GRIDPOSITION);
                        poles.Add(p);
                    }
                }
            }

            //AF - Add negative Poles
            for (int i = 0; i < Constants.ARRAYY; i++)
            {
                for (int j = 0; j < Constants.ARRAYX; j++)
                {
                    if (position[i, j] == '-')
                    {
                        Poles p = new Poles(-1, j * Constants.GRIDPOSITION, i * Constants.GRIDPOSITION);
                        poles.Add(p);
                    }
                }
            }

            return poles;
        }

        public List<Balloon> SetBalloons()
        {
            //AF -Adding balloons to list
            for (int i = 0; i < Constants.ARRAYY; i++)
            {
                for (int j = 0; j < Constants.ARRAYX; j++)
                {
                    if (position[i, j] == 'b')
                    {
                        Balloon b = new Balloon(j * Constants.GRIDPOSITION, i * Constants.GRIDPOSITION);
                        balloons.Add(b);
                    }
                }
            }

            return balloons;
        }
    }
}