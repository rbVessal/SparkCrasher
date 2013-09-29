using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

//@author Hillary Murray
//Instructor: Professor Cascioli
//Operates all of the game's animations

namespace SparkCrasher
{
    class Animation : Microsoft.Xna.Framework.Game
    {
        #region Attributes
        //attibutes - all will be constant
        //a timer variable 
        private float timer;
        //the frame rate/ interval
        private float interval;
        //current frame holder 
        private int currentFrame;
        //total amounts of frames
        private int totalFrames;
        //width of a single sprite image, not the whole sprite sheet
        private int spriteWidth;
        //height of a single sprite image
        private int spriteHeight;
        //rectangle for showing the image
        public Rectangle sourceRect;
        //spritebatch so we can draw in this class
        SpriteBatch spritebatch;
        //bool to tell when the animation is finished
        private bool finished;
        //image of the animation
        Texture2D texture;
        #endregion

        #region Constructor
        public Animation(GraphicsDevice graphicsDevice, SpriteBatch _spriteBatch, int TotalFrames, int Interval, Texture2D text)
        {
            timer = 0f;
            interval = Interval;
            currentFrame = 1; // (starting at 1)
            //total frames different for each animation
            totalFrames = TotalFrames;
            spriteWidth = 100;
            spriteHeight = 100;
            sourceRect = new Rectangle(currentFrame * spriteWidth, 0, spriteWidth, spriteHeight);
            //currentFrame * spriteWidth = the x position will move over by the value of currentFrame
            //0 = the y position (at the top)
            //spriteWidth, sprightHeight = self explanitory

            //spriteBatch needs to be the same spriteBatch as the one in Game1.cs
            spritebatch = _spriteBatch;
            finished = false;
            texture = text;
        }
        #endregion

        #region properties
        public float Interval
        {
            //curdleled milk
            get { return interval; }
            set { interval = value; }
        }

        public float Timer
        {
            get { return timer; }
        }

        public Rectangle SourceRect
        {
            get { return sourceRect; }
            set { sourceRect = value; }
        }
        public int TotalFrames
        {
            get { return totalFrames; }
            set { totalFrames = value; }
        }
        public int CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = value; }
        }
        public bool Finished
        {
            get { return finished; }
        }
        public int SpriteWidth
        {
            get { return spriteWidth; }
        }
        public int SpriteHeight
        {
            get { return spriteHeight; }
        }
        public Texture2D Texture
        {
            get { return texture; }
        }
        #endregion

        #region methods
        public int updateFrame(GameTime gameTime)
        {
            finished = false;
            //setting the timer float
            timer += (float)gameTime.ElapsedGameTime.Milliseconds;

            //if the timer is larger than the interval
            if (timer > interval)
            {
                //moving to the next frame
                currentFrame++;

                //resetting timer
                timer = 0f;
            }
            //if on the last frame, reset the count
            if (currentFrame == totalFrames)
            {
                finished = true;
                currentFrame = 0;
            }
            return currentFrame;
        }

        public void animate(Color color, Rectangle locRect, GameTime gameTime)
        {
            sourceRect.X = updateFrame(gameTime) * spriteWidth;
            spritebatch.Draw(texture, locRect, sourceRect, color);
        }
        #endregion

    }
}
