using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace SparkCrasher
{
    class FollowObj
    {
        public Rectangle location;
        public Rectangle prevLocation;
        public Character leadChar;
        public Animation anim;
        FollowObj follower;

        //constructor
        public FollowObj(Character leader)
        {
            location = new Rectangle();
            leadChar = leader;
        }
        public FollowObj(FollowObj _follower)
        {
            location = new Rectangle();
            prevLocation = new Rectangle();
            follower = _follower;
        }

        //will attach to the radius of the lead circle at the coordinates
        public void followChar()
        {
            location = leadChar.PrevLocation;
        }
        public void followFollower()
        {
            location = follower.prevLocation;
        }
        public void Draw(Color color, SpriteBatch sb, GameTime gt)
        {
            anim.animate(color, location, gt);
            sb.Draw(anim.Texture, location, anim.SourceRect, color);
        }
    }
}
