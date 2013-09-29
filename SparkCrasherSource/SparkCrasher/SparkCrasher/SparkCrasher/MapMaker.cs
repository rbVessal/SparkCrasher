using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

//@author Jen Dziuba
//Instructor: Professor Cascioli
//Operates the in-game map editor

namespace SparkCrasher
{
    class MapMaker
    {
        #region Attributes
        Rectangle[,] rectangles; // reference for drawing the buttons
        Button[,] buttons; // buttons that represent the tiles of the game field
        Dictionary<string, Texture2D> mapImages; // dictionary for map images
        SpriteFont mapText; // text for map
        KeyboardState kbState; // current keyboard state
        KeyboardState prevKBState; // previous keyboard state
        int x; // x position of the cursor in the grid
        int y; // y position of the cursor in the grid
        int levelSelected; // currently selected level
        int maxLevel; // max levels in the game

        Rectangle nameBox; // rectangle to write the name of the current file in
        string name; // name of the selected file
        bool error; // track if errors have occurred
        string errMsg; // an error message

        public enum MapState
        {
            Saving, // map is being saved
            Loading, // map is being loaded
            Editing, // map is being edited
        }
        public MapState mapState; // tracks what the map editor is doing
        #endregion

        public MapMaker(Dictionary<string, Texture2D> imgs, SpriteFont txt)
        {
            mapImages = imgs;
            mapText = txt;
            rectangles = new Rectangle[Constants.ARRAYX, Constants.ARRAYY];
            buttons = new Button[Constants.ARRAYX, Constants.ARRAYY];
            for (int i = 0; i < Constants.ARRAYY; i++)
            {
                for (int j = 0; j < Constants.ARRAYX; j++)
                {
                    // fill the rectangle and button arrays
                    rectangles[j, i] = new Rectangle((j * (Constants.TILE_WIDTH / 2)), (i * (Constants.TILE_HEIGHT / 2)),
                        (Constants.TILE_WIDTH / 2), (Constants.TILE_HEIGHT / 2));
                    buttons[j, i] = new Button();
                    // make each button default to a blank tile
                    buttons[j, i].Thing = '.';
                }
            }

            x = 0;
            y = 0;
            levelSelected = 1;
            nameBox = new Rectangle(430, 410, 150, 50); // set the location/dimensions of the name box
            name = "";
            error = false;
            errMsg = "ERROR"; // set basic error message
            mapState = MapState.Editing;
        }

        public void Update(KeyboardState kbs, KeyboardState pkbs)
        {
            kbState = kbs;
            prevKBState = pkbs;

            switch (mapState)
            {
                #region Editing
                case MapState.Editing:
                    if (SingleKeyPress(Keys.Enter))
                    {
                        switch (buttons[x, y].Thing)
                        {
                            // switch the state of the currently selected tile
                            case '.': buttons[x, y].Thing = '=';
                                break;
                            case '=': buttons[x, y].Thing = '+';
                                break;
                            case '+': buttons[x, y].Thing = '-';
                                break;
                            case '-': buttons[x, y].Thing = 'S';
                                break;
                            case 'S': buttons[x, y].Thing = 'G';
                                break;
                            case 'G': buttons[x, y].Thing = 'b';
                                break;
                            case 'b': buttons[x, y].Thing = '.';
                                break;
                            default: break;
                        }
                    }

                    // move the cursor
                    else if (SingleKeyPress(Keys.W) && y > 0)
                    {
                        y--;
                    }
                    else if (SingleKeyPress(Keys.A) && x > 0)
                    {
                        x--;
                    }
                    else if (SingleKeyPress(Keys.S) && y < (Constants.ARRAYY - 1))
                    {
                        y++;
                    }
                    else if (SingleKeyPress(Keys.D) && x < (Constants.ARRAYX - 1))
                    {
                        x++;
                    }

                    // reset the level to blank tiles
                    else if (SingleKeyPress(Keys.Space))
                    {
                        this.ResetLevel();
                    }

                    // save the current level
                    else if (SingleKeyPress(Keys.Q))
                    {
                        CheckLevelCount();
                        mapState = MapState.Saving;
                    }
                    // load a previously made level
                    else if (SingleKeyPress(Keys.E))
                    {
                        CheckLevelCount();
                        mapState = MapState.Loading;
                    }
                    break;
                #endregion

                #region Loading
                case MapState.Loading:
                    // if an error has occurred
                    if (error)
                    {
                        if (SingleKeyPress(Keys.Enter))
                        {
                            error = false;
                        }
                    }

                    // otherwise
                    else
                    {
                        // set the level name
                        name = levelSelected + ".txt";

                        // scroll up through the available levels
                        if (SingleKeyPress(Keys.D) && levelSelected < (maxLevel - 1))
                        {
                            levelSelected++;
                        }
                        // scroll down through the available levels
                        else if (SingleKeyPress(Keys.A) && levelSelected > 1)
                        {
                            levelSelected--;
                        }

                        // initiate level load
                        else if (SingleKeyPress(Keys.Enter))
                        {
                            LoadLevel();
                            // if an error has not occurred
                            if (error == false)
                            {
                                mapState = MapState.Editing;
                            }
                        }

                        // cancel level load
                        else if (SingleKeyPress(Keys.Space))
                        {
                            mapState = MapState.Editing;
                        }
                    }
                    break;
                #endregion

                #region Saving
                case MapState.Saving:
                    // if an error has occured
                    if (error)
                    {
                        if (SingleKeyPress(Keys.Enter))
                        {
                            error = false;
                        }
                    }

                    // otherwise
                    else
                    {
                        // set the level name
                        name = levelSelected + ".txt";

                        // scroll up through the available level slots
                        if (SingleKeyPress(Keys.D) && levelSelected < maxLevel)
                        {
                            levelSelected++;
                        }
                        // scroll down through the available level slots
                        else if (SingleKeyPress(Keys.A) && levelSelected > 1)
                        {
                            levelSelected--;
                        }

                        // initiate level save
                        else if (SingleKeyPress(Keys.Enter))
                        {
                            SaveLevel();
                            // if an error has not occurred
                            if (error == false)
                            {
                                mapState = MapState.Editing;
                            }
                        }

                        // cancel level save
                        else if (SingleKeyPress(Keys.Space))
                        {
                            mapState = MapState.Editing;
                        }
                    }
                    break;
                #endregion

                default: break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            #region Drawing the grid
            // draw blank white rectangles
            for (int i = 0; i < Constants.ARRAYY; i++)
            {
                for (int j = 0; j < Constants.ARRAYX; j++)
                {
                    spriteBatch.Draw(mapImages["test"], rectangles[j, i], Color.Gray);
                }
            }

            // draw the cursor
            spriteBatch.Draw(mapImages["test"], new Rectangle((x * (Constants.TILE_WIDTH / 2)), (y * (Constants.TILE_HEIGHT / 2)),
                (Constants.TILE_WIDTH / 2), (Constants.TILE_HEIGHT / 2)), Color.Yellow);

            // draw the buttons' Thing text
            for (int i = 0; i < Constants.ARRAYY; i++)
            {
                for (int j = 0; j < Constants.ARRAYX; j++)
                {
                    int xPos = (j * (Constants.TILE_WIDTH / 2));
                    int yPos = (i * (Constants.TILE_HEIGHT / 2));
                    switch (buttons[j, i].Thing)
                    {
                        case '.': spriteBatch.DrawString(mapText, buttons[j, i].Thing.ToString(), new Vector2((xPos + 5), (yPos)), Color.Black);
                            break;
                        case '=': spriteBatch.Draw(mapImages["wall"], new Rectangle(xPos, yPos, (Constants.TILE_WIDTH / 2), (Constants.TILE_HEIGHT / 2)), Color.White);
                            break;
                        case '+': spriteBatch.Draw(mapImages["MAPpole"], new Rectangle(xPos, yPos, (Constants.TILE_WIDTH / 2), (Constants.TILE_HEIGHT / 2)), Color.Red);
                            break;
                        case '-': spriteBatch.Draw(mapImages["MAPpole"], new Rectangle(xPos, yPos, (Constants.TILE_WIDTH / 2), (Constants.TILE_HEIGHT / 2)), Color.Blue);
                            break;
                        case 'S': spriteBatch.Draw(mapImages["MAPsparky"], new Rectangle(xPos, yPos, (Constants.TILE_WIDTH / 2), (Constants.TILE_HEIGHT / 2)), Color.White);
                            break;
                        case 'G': spriteBatch.Draw(mapImages["goal"], new Rectangle(xPos, yPos, (Constants.TILE_WIDTH / 2), (Constants.TILE_HEIGHT / 2)), Color.White);
                            break;
                        case 'b': spriteBatch.Draw(mapImages["MAPballoon"], new Rectangle(xPos, yPos, (Constants.TILE_WIDTH / 2), (Constants.TILE_HEIGHT / 2)), Color.White);
                            break;
                        default: break;
                    }
                }
            }
            #endregion

            #region Map info
            // map key
            string tileType = "blank space";
            switch (buttons[x, y].Thing)
            {
                case '.': tileType = "blank space";
                    break;
                case '=': tileType = "wall";
                    break;
                case '+': tileType = "positive pole";
                    break;
                case '-': tileType = "negative pole";
                    break;
                case 'S': tileType = "starting position";
                    break;
                case 'G': tileType = "goal";
                    break;
                case 'b': tileType = "balloon";
                    break;
                default: break;
            }
            spriteBatch.DrawString(mapText, "\nLevel will not save with more than\n one [start] or [goal].\n\n\nPosition:\n " + (x + 1) + " , " + (y + 1) +
                "\n\n Current tile: " + tileType + ".",
                new Vector2(((Constants.SCREEN_WIDTH / 2) + 10), 0), Color.White);
            #endregion

            #region check to see if an error has occured in the previous loop
            if (error)
            {
                spriteBatch.DrawString(mapText, "An error has occured.\nPress [ENTER] to proceed.",
                    new Vector2(0, ((Constants.SCREEN_HEIGHT / 2) + 10)), Color.White);
                spriteBatch.Draw(mapImages["test"], nameBox, Color.White);
                spriteBatch.DrawString(mapText, name,
                    new Vector2((nameBox.X + 5), (nameBox.Y + 10)), Color.Black);
            }
            #endregion

            // otherwise, write associated instructions
            else
            {
                switch (mapState)
                {
                    #region Editing
                    case MapState.Editing:
                        spriteBatch.DrawString(mapText, " [W][A][S][D] - move cursor\n\n [Q] - save\n [E] - load\n\n [SPACE] - reset\n" +
                            " [ENTER] - switch tile\n [ESC] - exit",
                            new Vector2(0, ((Constants.SCREEN_HEIGHT / 2) + 10)), Color.White);
                        break;
                    #endregion

                    #region Loading
                    case MapState.Loading:
                        spriteBatch.DrawString(mapText, " LOADING\n [A] - next\n [D] - previous\n [ENTER] - select\n [SPACE] - cancel",
                            new Vector2(0, ((Constants.SCREEN_HEIGHT / 2) + 10)), Color.White);
                        spriteBatch.Draw(mapImages["test"], nameBox, Color.White);
                        spriteBatch.DrawString(mapText, name,
                            new Vector2((nameBox.X + 5), (nameBox.Y + 10)), Color.Black);
                        break;
                    #endregion

                    #region Saving
                    case MapState.Saving:
                        spriteBatch.DrawString(mapText, " SAVING\n [A] - next\n [D] - previous\n [ENTER] - select\n [SPACE] - cancel",
                            new Vector2(0, ((Constants.SCREEN_HEIGHT / 2) + 10)), Color.White);
                        spriteBatch.Draw(mapImages["test"], nameBox, Color.White);
                        spriteBatch.DrawString(mapText, name,
                            new Vector2((nameBox.X + 5), (nameBox.Y + 10)), Color.Black);
                        break;
                    #endregion

                    default: break;
                }
            }
        }

        // save the current level
        public void SaveLevel()
        {
            try
            {
                // check to make sure that there is only one S and one G before saving
                bool foundS = false;
                bool foundG = false;
                for (int i = 0; i < Constants.ARRAYY; i++)
                {
                    for (int j = 0; j < Constants.ARRAYX; j++)
                    {
                        if (foundS == false && buttons[j, i].Thing == 'S')
                        {
                            foundS = true;
                        }
                        else if (foundG == false && buttons[j, i].Thing == 'G')
                        {
                            foundG = true;
                        }
                        else if (foundS && buttons[j, i].Thing == 'S')
                        {
                            errMsg = "multiple S";
                            throw new Exception();
                        }
                        else if (foundG && buttons[j, i].Thing == 'G')
                        {
                            errMsg = "multiple G";
                            throw new Exception();
                        }
                    }
                }

                // write each button's Thing character to the file
                if (foundS && foundG)
                {
                    StreamWriter write = new StreamWriter(name);
                    for (int i = 0; i < Constants.ARRAYY; i++)
                    {
                        for (int j = 0; j < Constants.ARRAYX; j++)
                        {
                            write.Write(buttons[j, i].Thing.ToString());
                        }
                        write.WriteLine();
                    }
                    write.Close();
                }
                else
                {
                    errMsg = "no S and/or G";
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                error = true;
                name = errMsg;
            }
        }

        // load an existing level
        public void LoadLevel()
        {
            try
            {
                StreamReader read = new StreamReader(name);
                // fill a list with the lines read from level file
                List<string> levelLines = new List<string>();
                string line = read.ReadLine();
                while (line != null)
                {
                    levelLines.Add(line);
                    line = read.ReadLine();
                }
                read.Close();

                // set each button according to the read lines
                for (int i = 0; i < Constants.ARRAYY; i++)
                {
                    char[] lineCharacters = levelLines[i].ToCharArray();
                    for (int j = 0; j < Constants.ARRAYX; j++)
                    {
                        buttons[j, i].Thing = lineCharacters[j];
                    }
                }
            }
            catch (Exception)
            {
                error = true;
                errMsg = "load error";
                name = errMsg;
            }
        }

        // reset the level to all blank tiles
        public void ResetLevel()
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    buttons[j, i].Thing = '.';
                }
            }
        }

        // helper method: check for a single key press
        public bool SingleKeyPress(Keys key)
        {
            if (prevKBState.IsKeyUp(key) && kbState.IsKeyDown(key))
            {
                prevKBState = kbState;
                return true;
            }
            else { return false; }
        }

        // check the number of levels
        public void CheckLevelCount()
        {
            maxLevel = 1;
            bool checkLevels = true;
            while (checkLevels == true)
            {
                try
                {
                    StreamReader testRead = new StreamReader(maxLevel + ".txt");
                    testRead.Close();
                    maxLevel++;
                }
                catch (Exception)
                {
                    checkLevels = false;
                }
            }
        }
    }
}
