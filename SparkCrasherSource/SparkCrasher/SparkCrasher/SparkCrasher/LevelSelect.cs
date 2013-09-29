using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//@author Jen Dziuba
//Instructor: Professor Cascioli
//Operates the level select screen

namespace SparkCrasher
{
    class LevelSelect
    {
        #region Attributes
        Dictionary<string, Texture2D> images; // image dictionary for level select
        SpriteFont text; // font for level select
        KeyboardState kbState; // current keyboard state
        KeyboardState prevKBState; // previous keyboard state
        public int currentLevel; // currently selected level
        public int maxLevel; // max levels available

        public Dictionary<int, bool> cleared; // tells if the selected level is cleared or not

        public enum ScreenState
        {
            Select, // picking a level
            Instructions, // viewing instructions
        }
        public ScreenState screenState;
        #endregion

        public LevelSelect(Dictionary<string, Texture2D> imgs, SpriteFont txt, int cLV, int mLV)
        {
            images = imgs;
            text = txt;
            currentLevel = cLV;
            maxLevel= mLV;
            screenState = new ScreenState();
            screenState = ScreenState.Select;
            cleared = new Dictionary<int, bool>();
            for (int i = 0; i < maxLevel; i++)
            {
                cleared.Add((i + 1), false);
            }
        }

        public void Update(KeyboardState kbs, KeyboardState pkbs)
        {
            kbState = kbs;
            prevKBState = pkbs;

            // switch to the Select page
            if (SingleKeyPress(Keys.W))
            {
                screenState = ScreenState.Select;
            }
            // switch to the Instructions page
            else if (SingleKeyPress(Keys.E))
            {
                screenState = ScreenState.Instructions;
            }

            switch (screenState)
            {
                case ScreenState.Select:
                    // move cursor to previous level
                    if (SingleKeyPress(Keys.A) && currentLevel > 1)
                    {
                        currentLevel--;
                    }
                    //  move cursor to next level
                    else if (SingleKeyPress(Keys.D) && currentLevel < (maxLevel - 1))
                    {
                        currentLevel++;
                    }
                    break;
                case ScreenState.Instructions:
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (screenState)
            {
                case ScreenState.Select:
                    // brief instructions & level info
                    spriteBatch.DrawString(text, "[E] INSTRUCTIONS\n[A] PREVIOUS\n[D] NEXT\n\n[ENTER] START\n\nlevel " + currentLevel,
                        new Vector2(((Constants.SCREEN_WIDTH / 2) - 100), (Constants.SCREEN_HEIGHT / 2)), Color.White);

                    // check if selected level is locked
                    if (currentLevel > 1 && cleared[(currentLevel - 1)] == false)
                    {
                        spriteBatch.DrawString(text, "\n\n\n\n\n\n\nlocked",
                            new Vector2(((Constants.SCREEN_WIDTH / 2) - 100), (Constants.SCREEN_HEIGHT / 2)), Color.White);
                    }
                    break;
                case ScreenState.Instructions:
                    // instructions for operating the game
                    spriteBatch.DrawString(text, "SELECTION screen\n [Q] - HIGH SCORES\n [W] - LEVEL SELECT\n" +
                        " [E] - INSTRUCTIONS\n [S] - TITLE\n [SPACE] - MAP EDITOR",
                        new Vector2(150, (Constants.SCREEN_HEIGHT / 2)), Color.White);
                    spriteBatch.DrawString(text, "Playing levels\n\n [W] - neutral\n\n [ENTER] - PAUSE",
                        new Vector2(((Constants.SCREEN_WIDTH / 2) + 50), (Constants.SCREEN_HEIGHT / 2)), Color.White);
                    spriteBatch.DrawString(text, "\n [A] - positive",
                        new Vector2(((Constants.SCREEN_WIDTH / 2) + 50), (Constants.SCREEN_HEIGHT / 2)), Color.Firebrick);
                    spriteBatch.DrawString(text, "\n\n\n [D] - negative",
                        new Vector2(((Constants.SCREEN_WIDTH / 2) + 50), (Constants.SCREEN_HEIGHT / 2)), Color.CornflowerBlue);
                    break;
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

        // check levels to see if a new one was added
        public void CheckCompletedLevels()
        {
            for (int i = 0; i < maxLevel; i++)
            {
                if (cleared.ContainsKey((i + 1))) { }
                else
                {
                    // add new level if one is detected
                    cleared.Add((i + 1), false);
                }
            }
        }
    }
}
