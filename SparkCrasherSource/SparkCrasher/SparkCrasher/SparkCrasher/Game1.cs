using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using System.IO;

//@author:  Jen Dzuiba, Rebecca Vessal, Hillary Murray, Anne Fonicello
//Instructor: Professor Cascioli
//Date: 3/18/11 - 4/26/11
//
//Game1.cs
//
//Game1.cs loads all of the levels and contains the game's finite state machine and game mechanics.
namespace SparkCrasher
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Attributes
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont text; // current font
        Dictionary<string, Texture2D> images; // image dictionary
        Dictionary<string, SoundEffect> sounds; // sound dictionary
        KeyboardState kbState; // current keyboard state
        KeyboardState prevKBState; // previous frame's keyboard state

        Character player; // player object
        Mechanics mechanics; // mechanics - really shouldn't need to be an object
        LevelSelect levelSelect; // level select - also should not need to be an object
        LevelLoader levelLoader; // loads the levels - also should not have to be an object
        List<Poles> poles; // collection of pole objects
        List<Wall> walls; // collection of wall objects
        List<Balloon> balloons;//collection of balloons, ripe for the popping
        Goal goal; // goal object
        int currentLevel; // filename of current level
        int maxLevel; // maximum amount of levels present in the game
        FollowObj tail1;
        FollowObj tail2;
        FollowObj tail3;

        //rectangles for the objects
        Rectangle goalRect;
        Rectangle playerRect;
        Rectangle poleRect;

        //animation objects for the animations (duh!)
        Animation poleAnimation;
        Animation charAnimation;
        Animation tailAnimation;

        //Doubles for the timer
        double totalMins;
        double totalSecs;
        double totalMils;
        
        //Attributes for songs
        double songTime;
        Random ranSong;
        int nextSong;
        bool playing;
        SoundEffect currentSong;

        //Create attributes for positive and negative pole
        Poles pos;
        Poles neg;


        //just for printing out the score
        string levelScore; //changed to double
        String totalScore;
        String scorePrint; //used for printing high score lists
        int balloonPop;
        double endScore;

        enum GameState
        {
            Level, // playing the level
            Map, // selecting a level
            Load, // loading that level
            Title, // title splash
            LevelComplete, // result screen on finishing a level
            Pause, // in-level pause menu
            Edit, // map editor
            GameFinish, //For when the final level is beat (currentlevel=maxlevel)
            HighScores, //A page that displays the top 11 high scores
            //Orbit, //player orbits clockwise around pole
            //CounterClockwiseOrbit //player orbits counter clockwise around a pole
        }
        GameState gameState; // track the game's state

        enum OrbitState
        {
            Orbit, // orbit clockwise around pole
            CounterClockwiseOrbit, // orbit counterclockwise around pole
            None, // is not orbiting around a pole
        }
        OrbitState orbitState; // track player's orbit state

        MapMaker mapMaker; // map editor
        Scoring getScore; //Score attribute
        #endregion

        #region Constructor
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            // Set the screen size
            graphics.PreferredBackBufferWidth = Constants.SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = Constants.SCREEN_HEIGHT;

            Content.RootDirectory = "Content";
        }
        #endregion

        #region Initialize

        protected override void Initialize()
        {
            gameState = new GameState();
            gameState = GameState.Title;
            orbitState = new OrbitState();
            orbitState = OrbitState.None;
            kbState = new KeyboardState();
            prevKBState = new KeyboardState();
            player = new Character();
            mechanics = new Mechanics();
            poles = new List<Poles>();
            walls = new List<Wall>();
            balloons = new List<Balloon>();
            goal = new Goal();
            images = new Dictionary<string, Texture2D>();
            sounds = new Dictionary<string, SoundEffect>();
            currentLevel = 1;
            mapMaker = null;
            levelScore = "";
            totalScore = "";
            balloonPop = 0;
            endScore = 0;
            getScore = new Scoring(); //Score object
            tail1 = new FollowObj(player);
            tail2 = new FollowObj(tail1);
            tail3 = new FollowObj(tail2);

            base.Initialize();
        }
        #endregion

        #region LoadContent

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            images.Add("poleSS", this.Content.Load<Texture2D>("spritesheetPole"));
            //sparky animation
            images.Add("sparkSS", this.Content.Load<Texture2D>("sparkSS")); // character
            images.Add("wall", this.Content.Load<Texture2D>("wall")); // wall
            images.Add("goal", this.Content.Load<Texture2D>("goal")); // goal
            images.Add("goalFinish", this.Content.Load<Texture2D>("goalFinish"));//touched goal
            images.Add("title", this.Content.Load<Texture2D>("title_screen")); // title image
            images.Add("balloon", this.Content.Load<Texture2D>("Heart Balloon")); // balloon image
            // map editor only images
            images.Add("MAPsparky", this.Content.Load<Texture2D>("MAPsparky")); // sparky
            images.Add("MAPpole", this.Content.Load<Texture2D>("MAPpole")); // pole
            images.Add("test", this.Content.Load<Texture2D>("Test")); // white tile
            images.Add("MAPballoon", this.Content.Load<Texture2D>("MAPballoon")); // balloon

            //sounds - various sound effects and music for the game
            sounds.Add("collision", this.Content.Load<SoundEffect>("Collision")); // collision sound effect
            sounds.Add("long collision", this.Content.Load<SoundEffect>("CollisionLong")); // longer version of collision sound effect
            sounds.Add("pious", this.Content.Load<SoundEffect>("Pious_on")); // Track: Get Your Pious On (non loop)
            sounds.Add("base", this.Content.Load<SoundEffect>("sparkBase")); // Track: Spark Base (loop)
            sounds.Add("alchemist", this.Content.Load<SoundEffect>("Alchemist")); // Track: Alchemist
            sounds.Add("faster", this.Content.Load<SoundEffect>("Faster")); // Track: Faster
            sounds.Add("mta", this.Content.Load<SoundEffect>("MTA")); // Track: MTA
            sounds.Add("rising", this.Content.Load<SoundEffect>("Rising")); // Track: Rising

            images.Add("tailSS", this.Content.Load<Texture2D>("tailSS"));

            //background images
            images.Add("active", this.Content.Load<Texture2D>("testroom"));
            images.Add("r1_light", this.Content.Load<Texture2D>("room1_light"));
            images.Add("r1_dark", this.Content.Load<Texture2D>("room1_dark"));
            images.Add("r2_light", this.Content.Load<Texture2D>("room2_light"));
            images.Add("r2_dark", this.Content.Load<Texture2D>("room2_dark"));
            images.Add("r3_light", this.Content.Load<Texture2D>("room3_light"));
            images.Add("r3_dark", this.Content.Load<Texture2D>("room3_dark"));
            images.Add("r4_light", this.Content.Load<Texture2D>("room4_light"));
            images.Add("r4_dark", this.Content.Load<Texture2D>("room4_dark"));
            images.Add("lvl7_light", this.Content.Load<Texture2D>("levl7_light"));
            images.Add("lvl7_dark", this.Content.Load<Texture2D>("lvl7_dark"));


            ranSong = new Random();
            playing = false;
            currentSong = sounds["base"];

            //font - that custom font Jen used for the example title splash
            //text = this.Content.Load<SpriteFont>("SpriteFont1"); // filler for font - Arial, size 14
            text = this.Content.Load<SpriteFont>("CrackedFont"); // size 20
            
            //test images
            images.Add("testBackground", this.Content.Load<Texture2D>("testroom")); // test background image

            // TODO: use this.Content to load your game content here
            poleAnimation = new Animation(GraphicsDevice, spriteBatch, 9, 500, images["poleSS"]);
            charAnimation = new Animation(GraphicsDevice, spriteBatch, 5, 100, images["sparkSS"]);
            tailAnimation = new Animation(GraphicsDevice, spriteBatch, 5, 100, images["tailSS"]);
            tail1.anim = tailAnimation;
            tail2.anim = tailAnimation;
            tail3.anim = tailAnimation;

            // load the level select stuff
            CheckLevelCount();
            levelSelect = new LevelSelect(images, text, currentLevel, maxLevel);
        }
        #endregion

        #region Update
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            kbState = Keyboard.GetState();

            #region Song stuff
            // play the current song
            if (playing == false)
            {
                currentSong.Play();
                songTime = 0;
                playing = true;
            }

            // increment song timer
            songTime += gameTime.ElapsedGameTime.TotalMilliseconds;

            // upon completing the song, pick a new one
            if (songTime >= (currentSong.Duration.TotalMilliseconds - Constants.SONG_OFFSET))
            {
                playing = false;
                if (currentSong == sounds["long collision"])
                {
                    nextSong = ranSong.Next(7);
                    switch (nextSong)
                    {
                        #region switch songs
                        case 0:
                            currentSong = sounds["base"];
                            break;
                        case 1:
                            currentSong = sounds["base"];
                            break;
                        case 2:
                            currentSong = sounds["pious"];
                            break;
                        case 3:
                            currentSong = sounds["rising"];
                            break;
                        case 4:
                            currentSong = sounds["mta"];
                            break;
                        case 5:
                            currentSong = sounds["faster"];
                            break;
                        case 6:
                            currentSong = sounds["alchemist"];
                            break;
                        #endregion
                    }
                }
                else
                {
                    currentSong = sounds["long collision"];
                }
            }
            #endregion

            switch (gameState)
            {
                #region Case: Title
                case GameState.Title: // title splash

                    if (this.SingleKeyPress(Keys.Enter))
                    {
                        // start the game
                        CheckLevelCount();
                        currentLevel = 1;
                        gameState = GameState.Map;
                    }
                    break;
                #endregion

                #region Case: Map
                case GameState.Map:
                    // update the level selector
                    levelSelect.Update(kbState, prevKBState);
                    // get changed level info
                    currentLevel = levelSelect.currentLevel;
                    maxLevel = levelSelect.maxLevel;

                    // select a level
                    if (SingleKeyPress(Keys.Enter) && levelSelect.screenState == LevelSelect.ScreenState.Select)
                    {
                        // check if previous level has been cleared, or if is LV1
                        if (currentLevel == 1 || (currentLevel > 1 && levelSelect.cleared[currentLevel - 1]))
                        {
                            gameState = GameState.Load;
                        }
                    }

                    // return to the title
                    else if (SingleKeyPress(Keys.S))
                    {
                        levelSelect.screenState = LevelSelect.ScreenState.Select;
                        gameState = GameState.Title;
                    }

                    // view the high scores
                    else if (SingleKeyPress(Keys.Q))
                    {
                        gameState = GameState.HighScores;
                    }

                    // enter the map editor
                    else if (SingleKeyPress(Keys.Space))
                    {
                        gameState = GameState.Edit;
                    }
                    break;
                #endregion

                #region Case: Load
                case GameState.Load: // load selected level
                    // make a new loader
                    levelLoader = new LevelLoader();
                    // load the file
                    levelLoader.Load(currentLevel + ".txt");
                    // set locations based on file
                    player = levelLoader.SetStart(player);
                    goal = levelLoader.SetFinish(goal);
                    walls = levelLoader.SetWalls();
                    poles = levelLoader.SetPoles();
                    balloons = levelLoader.SetBalloons();

                    //setting the animation variable in the balloons
                //they get their own animation because they need to act independantly
                    for (int i = 0; i < balloons.Count; i++)
                    {
                        balloons[i].Anim = new Animation(GraphicsDevice, spriteBatch, 5, 100,images["balloon"]);
                    }
                        
                    // reset player
                    player.XSpeed = Constants.START_SPEED;
                    player.YSpeed = Constants.START_SPEED;
                    player.Angle = Constants.START_ANGLE;
                    player.Charge = 0;

                    // start the level
                    gameState = GameState.Level;

                    //reseting the timer
                    totalSecs = 0;
                    totalMins = 0;
                    totalMils = 0;

                    // set random level background
                    int num = ranSong.Next(4);
                    switch (num)
                    {
                        #region switch bkg
                        case 0:
                            if (levelSelect.cleared[currentLevel] == false)
                            {
                                images["active"] = images["r1_dark"];
                            }
                            else
                            {
                                images["active"] = images["r1_light"];
                            }
                            break;
                        case 1:
                            if (levelSelect.cleared[currentLevel] == false)
                            {
                                images["active"] = images["r2_dark"];
                            }
                            else
                            {
                                images["active"] = images["r2_light"];
                            }
                            break;
                        case 2:
                            if (levelSelect.cleared[currentLevel] == false)
                            {
                                images["active"] = images["r3_dark"];
                            }
                            else
                            {
                                images["active"] = images["r3_light"];
                            }
                            break;
                        case 3:
                            if (levelSelect.cleared[currentLevel] == false)
                            {
                                images["active"] = images["r4_dark"];
                            }
                            else
                            {
                                images["active"] = images["r4_light"];
                            }
                            break;
                        #endregion
                    }
                    // check for 'boss level'
                    if (currentLevel == 7 && levelSelect.cleared[7] == false)
                    {
                        images["active"] = images["lvl7_dark"];
                    }
                    else if (currentLevel == 7 && levelSelect.cleared[7])
                    {
                        images["active"] = images["lvl7_light"];
                    }

                    break;
                #endregion

                #region Case: Pause
                case GameState.Pause: // pause menu
                    if (this.SingleKeyPress(Keys.Enter))
                    {
                        // unpause the game
                        gameState = GameState.Level;
                    }
                    else if (this.SingleKeyPress(Keys.A))
                    {
                        // reset the level (reload)
                        gameState = GameState.Load;
                    }
                    else if (this.SingleKeyPress(Keys.D))
                    {
                        // return to the map
                        gameState = GameState.Map;
                    }
                    break;
                #endregion

                #region Case: Level
                case GameState.Level: // play the level
                    //setting the old position
                    tail3.prevLocation = new Rectangle((tail2.prevLocation.X + 6), (tail2.prevLocation.Y + 6),
                        12, 12);
                    tail2.prevLocation = new Rectangle((tail1.prevLocation.X + 12), (tail1.prevLocation.Y + 12),
                        25, 25);
                    tail1.prevLocation = new Rectangle(player.PrevLocation.X, player.PrevLocation.Y, 50, 50);
                    player.PrevLocation = new Rectangle(player.XPos, player.YPos, 50, 50);
                        
                        // timer stuff
                        totalSecs += gameTime.ElapsedGameTime.TotalSeconds;
                        totalMins += gameTime.ElapsedGameTime.TotalMinutes;
                        totalMils += gameTime.ElapsedGameTime.TotalMilliseconds;

                        #region pause the level
                        if (this.SingleKeyPress(Keys.Enter))
                        {
                            gameState = GameState.Pause;
                        }
                        #endregion

                        #region check if the player has completed the level
                        else if (playerRect.Intersects(goalRect))
                        {
                            //adding to the score
                            levelScore = String.Format("{0:0.00}", totalSecs);

                            if (getScore.ScoreList.ContainsKey(currentLevel))
                            {
                                getScore.ScoreList[currentLevel] = totalSecs - (balloonPop*.5);
                            }
                            else
                            {
                                getScore.ScoreList.Add(currentLevel, totalSecs - (balloonPop*.5));
                            }
                           

                            //HM: making it pause for a second so people can see the lightbulb glow :)
                            Thread.Sleep(1000);
                            if (currentLevel+1  == maxLevel)
                            {
                                for (int i = 1; i <= getScore.ScoreList.Count(); i++)
                                {
                                    endScore += getScore.ScoreList[i];
                                }
                                totalScore = String.Format("{0:0.00}", endScore);
                                getScore.WriteScores("HighScores.txt");
                                gameState = GameState.GameFinish;
                            }
                            else
                            {
                                gameState = GameState.LevelComplete;
                            }
                        }
                       
                        #endregion

                        #region orbiting
                        else if (orbitState != OrbitState.None)
                        {
                            #region instances for when player is leaving orbit
                            if (SingleKeyPress(Keys.A) && player.Charge != 1)
                            {
                                player.Charge = 1;
                                orbitState = OrbitState.None;
                            }
                            else if (SingleKeyPress(Keys.S))
                            {
                                player.Charge = 0;
                                orbitState = OrbitState.None;
                            }
                            else if (SingleKeyPress(Keys.D) && player.Charge != -1)
                            {
                                player.Charge = -1;
                                orbitState = OrbitState.None;
                            }
                            #endregion

                            // otherwise, call the orbit methods & collision/speed limit
                            else
                            {
                                #region Orbit state in which player orbit around the pole both counter clockwise and clockwise
                                switch (orbitState)
                                {
                                    case OrbitState.Orbit:
                                        switch (player.Charge)
                                        {
                                            case -1:
                                            {
                                                //Call orbit method so that player can orbit around positive pole
                                                mechanics.Orbit(pos, player);
                                                //Increment player position based on the speed calculations from orbit method
                                                player.XPos += player.XSpeed;
                                                player.YPos += player.YSpeed;
                                                break;
                                            }
                                            case 1:
                                            {
                                                //Call orbit method so that player can orbit around negative pole
                                                mechanics.Orbit(neg, player);
                                                //Increment player position based on the speed calculations from orbit method
                                                player.XPos += player.XSpeed;
                                                player.YPos += player.YSpeed;
                                                break;
                                            }
                                            default:
                                            {
                                                orbitState = OrbitState.None;
                                                break;
                                            }
                                        }
                                        break;

                                    default: break;
                                }
                            #endregion

                                #region NEW METHOD FOR WALL COLLISIONS
                                int i = 0;
                                bool wallIsHit = false;
                                while (wallIsHit == false && i < (walls.Count - 1))
                                {
                                    wallIsHit = mechanics.Bounce(walls[i], player);
                                    if (wallIsHit)
                                    {
                                        sounds["collision"].Play();
                                        player = mechanics.WallCollision(player);
                                    }
                                    i++;
                                }
                                #endregion

                                #region check for max speed
                                if (player.XSpeed > Constants.MAX_SPEED)
                                {
                                    player.XSpeed = Constants.MAX_SPEED;
                                }
                                else if (player.XSpeed < -(Constants.MAX_SPEED))
                                {
                                    player.XSpeed = -(Constants.MAX_SPEED);
                                }
                                if (player.YSpeed > Constants.MAX_SPEED)
                                {
                                    player.YSpeed = Constants.MAX_SPEED;
                                }
                                else if (player.YSpeed < -(Constants.MAX_SPEED))
                                {
                                    player.YSpeed = -(Constants.MAX_SPEED);
                                }
                                #endregion
                            }
                        }
                        #endregion

                        #region Moving
                        else
                        {
                            #region find nearest pos and neg poles
                            pos = new Poles(0, 0, 0); // closest pos pole
                            neg = new Poles(0, 0, 0); // closest neg plole
                            int dis = 0; // distance between player and current pole
                            int pDis = 0; // distance between player and closest pos pole
                            int nDis = 0; // distance between player and closest neg pole
                            foreach (Poles pole in poles)
                            {
                                // calculate distance between player and current pole
                                dis = (int)(Math.Pow((pole.XPos - player.XPos), 2) + Math.Pow((pole.YPos - player.YPos), 2));
                                // check if current pos and neg poles have been set
                                if (pos.Charge == 0 && pole.Charge == 1)
                                {
                                    // set base comparision for pos pole
                                    pos = pole;
                                    pDis = dis;

                                }
                                else if (neg.Charge == 0 && pole.Charge == -1)
                                {
                                    // set base comparison for neg pole
                                    neg = pole;
                                    nDis = dis;
                                }
                                // check current pole against current pos and neg
                                switch (pole.Charge)
                                {
                                    case 1: if (dis < pDis)
                                        {
                                            // set new nearest pos pole
                                            pos = pole;
                                            pDis = dis;

                                        }
                                        break;
                                    case -1: if (dis < nDis)
                                        {
                                            // set new nearest ned pole
                                            neg = pole;
                                            nDis = dis;

                                        }
                                        break;
                                    default: break;
                                }
                            }
                            #endregion

                            #region switch the player's charge
                            if (this.SingleKeyPress(Keys.S))
                            {
                                // change charge to neutral
                                player.Charge = 0;
                            }
                            else if (this.SingleKeyPress(Keys.A))
                            {
                                // change charge to positive
                                player.Charge = 1;
                            }
                            else if (this.SingleKeyPress(Keys.D))
                            {
                                // change charge to negative
                                player.Charge = -1;
                            }
                            #endregion

                            #region player acceleration
                            if (player.Charge == 0)
                            {
                                // neutral charge
                                player.YSpeed = mechanics.Gravity(player.YSpeed);
                            }
                            else if (player.Charge == 1)
                            {
                                // positive charge
                                player = mechanics.Attraction(neg, player);

                                // stick to opposite-charge pole
                                if (mechanics.PoleCollision(neg, player))
                                {
                                    //player.XSpeed = 0;
                                    //player.YSpeed = 0;
                                    //Set orbitState to orbit
                                    orbitState = OrbitState.Orbit;
                                }

                                // repulse from same-charge pole
                                else if (mechanics.PoleCollision(pos, player))
                                {
                                    mechanics.Repulsion(pos, player);
                                }
                            }
                            else if (player.Charge == -1)
                            {
                                // negative charge
                                player = mechanics.Attraction(pos, player);

                                // stick to opposite-charge pole
                                if (mechanics.PoleCollision(pos, player))
                                {
                                    //player.XSpeed = 0;
                                    //player.YSpeed = 0;
                                    //Set orbitState to orbit
                                    orbitState = OrbitState.Orbit;
                                }

                                // repulse from same-charge pole
                                else if (mechanics.PoleCollision(neg, player))
                                {
                                    mechanics.Repulsion(neg, player);
                                }
                            }
                            #endregion

                            #region NEW METHOD FOR WALL COLLISIONS
                            int i = 0;
                            bool wallIsHit = false;
                            while (wallIsHit == false && i < (walls.Count - 1))
                            {
                                wallIsHit = mechanics.Bounce(walls[i], player);
                                if (wallIsHit)
                                {
                                    sounds["collision"].Play();
                                    player = mechanics.WallCollision(player);
                                }
                                i++;
                            }
                            #endregion

                            #region check for max speed
                            if (player.XSpeed > Constants.MAX_SPEED)
                            {
                                player.XSpeed = Constants.MAX_SPEED;
                            }
                            else if (player.XSpeed < -(Constants.MAX_SPEED))
                            {
                                player.XSpeed = -(Constants.MAX_SPEED);
                            }
                            if (player.YSpeed > Constants.MAX_SPEED)
                            {
                                player.YSpeed = Constants.MAX_SPEED;
                            }
                            else if (player.YSpeed < -(Constants.MAX_SPEED))
                            {
                                player.YSpeed = -(Constants.MAX_SPEED);
                            }
                            #endregion

                            // calculate new position
                            player.XPos += player.XSpeed;
                            player.YPos += player.YSpeed;
                        }
                        #endregion

                        // keep the player on the screen
                        BorderCollision();

                        break;
                #endregion

                #region Case: Level Complete
                case GameState.LevelComplete:
                        // leave this screen
                        if (SingleKeyPress(Keys.Enter))
                        {
                            levelSelect.cleared[(currentLevel)] = true;
                            // increment currently selected level, if possible
                            if (currentLevel < (maxLevel - 1))
                            {
                                currentLevel++;
                                levelSelect.currentLevel = currentLevel;
                            }
                            // return to the map
                            gameState = GameState.Map;
                        }
                        
                        break;
                #endregion

                #region Case: Edit
                case GameState.Edit:
                    // check if the editor is in use
                    if (mapMaker != null)
                    {
                        // return to the map and close the editor
                        if (mapMaker.mapState == MapMaker.MapState.Editing && SingleKeyPress(Keys.Escape))
                        {
                            mapMaker = null;
                            CheckLevelCount();
                            levelSelect.CheckCompletedLevels();
                            gameState = GameState.Map;
                        }

                        // otherwise, update the editor
                        else
                        {
                            mapMaker.Update(kbState, prevKBState);
                        }
                    }

                    // if there is no editor
                    else
                    {
                        mapMaker = new MapMaker(images, text);
                    }
                    break;
                #endregion

                #region Case: GameFinish
                case GameState.GameFinish:
                    if (this.SingleKeyPress(Keys.Enter))
                    {
                        gameState = GameState.HighScores;
                    }
                    break;

                #endregion

                #region Case: HighScore
                case GameState.HighScores:
                    // Open the high score list
                    getScore.LoadScores("HighScores.txt");
                    if (SingleKeyPress(Keys.W))
                    {
                        gameState = GameState.Map;
                        levelSelect.screenState = LevelSelect.ScreenState.Select;
                    }
                    else if (SingleKeyPress(Keys.E))
                    {
                        gameState = GameState.Map;
                        levelSelect.screenState = LevelSelect.ScreenState.Instructions;
                    }
                    else if (SingleKeyPress(Keys.S))
                    {
                        gameState = GameState.Title;
                    }
                    break;
                #endregion
            }

            // save the current keyboard state for reference next frame
            prevKBState = kbState;

            base.Update(gameTime);
        }
        #endregion

        #region Draw
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            Window.Title = "Spark Crasher";

            switch (gameState)
            {
                #region Case: Title
                case GameState.Title: // title splash
                    spriteBatch.Draw(images["title"], new Rectangle(0, 0, Constants.SCREEN_WIDTH, 571), Color.White);
                    break;
                #endregion

                #region Case: Map
                case GameState.Map: // level select
                    spriteBatch.Draw(images["title"], new Rectangle(0, 0, Constants.SCREEN_WIDTH, 571), Color.White);
                    // cover up the PRESS ENTER from the title image
                    spriteBatch.Draw(images["test"], new Rectangle(200, 350, 400, 100), Color.Black);

                    if (levelSelect != null)
                    {
                        levelSelect.Draw(spriteBatch);
                    }

                    break;
                #endregion

                #region Case: Loading
                case GameState.Load: // loading screen, not generally seen
                    spriteBatch.DrawString(text, "LOADING", Vector2.Zero, Color.White);
                    break;
                #endregion

                #region Case: Level
                case GameState.Level: // inside the level
                    // background
                    spriteBatch.Draw(images["active"], new Rectangle(0, 0, Constants.SCREEN_WIDTH, Constants.SCREEN_HEIGHT), Color.White);

                    #region Walls
                    foreach (Wall wall in walls)
                    {
                        spriteBatch.Draw(images["wall"], new Rectangle(wall.XPos, wall.YPos, Constants.TILE_WIDTH, Constants.TILE_HEIGHT), Color.White);
                    }
                    #endregion

                    #region Poles
                    Color color = new Color();
                    // color poles based on charge, then draw each
                    foreach (Poles pole in poles)
                    {
                        switch (pole.Charge)
                        {
                            case 0: color = Color.White;
                                break;
                            case 1: color = Color.Firebrick;
                                break;
                            case -1: color = Color.CornflowerBlue;
                                break;
                        }

                        poleRect = new Rectangle(pole.XPos, pole.YPos, Constants.TILE_WIDTH, Constants.TILE_HEIGHT);

                        poleAnimation.animate(color, poleRect, gameTime);
                    }
                    #endregion

                    #region Timer
                    //drawing out the timer
                    spriteBatch.DrawString(text, (int)totalMins % 60 + " : " + (int)totalSecs % 60 + " : " + (int)totalMils % 1000, new Vector2(10, 10), Color.White);
                    #endregion

                    #region Player
                    // color player based on charge, then draw
                    switch (player.Charge)
                    {
                        case 0: color = Color.White;
                            break;
                        case 1: color = Color.Firebrick;
                            break;
                        case -1: color = Color.CornflowerBlue;
                            break;
                    }

                    //HM: creating the player's rectangle so I can check for collision
                    playerRect = new Rectangle(player.XPos, player.YPos, Constants.TILE_WIDTH, Constants.TILE_HEIGHT);
                    //playerVect = new Vector2(player.XPos, player.YPos);

                    charAnimation.animate(color, playerRect, gameTime);
                    tail1.followChar();
                    tail2.followFollower();
                    tail3.followFollower();

                    tail3.Draw(color, spriteBatch, gameTime);
                    tail2.Draw(color, spriteBatch, gameTime);
                    tail1.Draw(color, spriteBatch, gameTime);
                    charAnimation.animate(color, playerRect, gameTime);
                    #endregion

                    #region Goal
                    //HM: creating the goal's rectangle so I can check for collision
                    goalRect = new Rectangle(goal.X, goal.Y, Constants.TILE_WIDTH, Constants.TILE_HEIGHT);
                    // draw the goal
                    spriteBatch.Draw(images["goal"], goalRect, Color.White);

                    //HM:checking for the collision between player and goal
                    if (goalRect.Intersects(playerRect))
                    {
                        spriteBatch.Draw(images["goalFinish"], goalRect, Color.White);
                    }
                    #endregion

                    #region Balloons

                    for (int i = 0; i < balloons.Count; i++)
                    {
                        balloons[i].Anim.animate(Color.White, new Rectangle(balloons[i].X, balloons[i].Y, Constants.TILE_WIDTH, Constants.TILE_HEIGHT), gameTime);

                        if (playerRect.Intersects(new Rectangle(balloons[i].X, balloons[i].Y, Constants.TILE_WIDTH, Constants.TILE_HEIGHT)) && balloons[i].Popped == false)
                        {
                            balloons[i].Popped = true;
                            balloons[i].Death();
                            balloonPop++;
                        }
                        if (balloons[i].Anim.CurrentFrame == 3 && balloons[i].Popped == true)
                        {
                            balloons.Remove(balloons[i]);
                        }
                    }

                    #endregion

                    break;
                #endregion

                #region Case: Pause
                case GameState.Pause: // pause menu
                    Color color2 = new Color();
                    // Draw all Level stuff in background
                    // background
                    spriteBatch.Draw(images["active"], new Rectangle(0, 0, Constants.SCREEN_WIDTH, Constants.SCREEN_HEIGHT), Color.White);
                    spriteBatch.Draw(images["test"], new Rectangle(0, 0, Constants.SCREEN_WIDTH, Constants.SCREEN_HEIGHT), new Color(0, 0, 0, 200));
                    //spriteBatch.DrawString(text, "( TEST IMAGE BKG )", new Vector2((Constants.SCREEN_WIDTH - 200), (Constants.SCREEN_HEIGHT - 50)), Color.White);

                    #region Walls
                    foreach (Wall wall in walls)
                    {
                        spriteBatch.Draw(images["wall"], new Rectangle(wall.XPos, wall.YPos, Constants.TILE_WIDTH, Constants.TILE_HEIGHT), Color.White);
                    }
                    #endregion

                    #region Poles
                    // color poles based on charge, then draw each
                    foreach (Poles pole in poles)
                    {
                        switch (pole.Charge)
                        {
                            case 0: color2 = Color.White;
                                break;
                            case 1: color2 = Color.Firebrick;
                                break;
                            case -1: color2 = Color.CornflowerBlue;
                                break;
                        }

                        poleRect = new Rectangle(pole.XPos, pole.YPos, Constants.TILE_WIDTH, Constants.TILE_HEIGHT);
                        // there are 9 frames in the pole sprite Sheet
                        poleAnimation.animate(color2, poleRect, gameTime);
                    }
                    #endregion

                    //HM: creating the goal's rectangle so I can check for collision
                    goalRect = new Rectangle(goal.X, goal.Y, Constants.TILE_WIDTH, Constants.TILE_HEIGHT);
                    // draw the goal
                    spriteBatch.Draw(images["goal"], goalRect, Color.White);

                    // color player based on charge, then draw
                    switch (player.Charge)
                    {
                        case 0: color2 = Color.White;
                            break;
                        case 1: color2 = Color.Firebrick;
                            break;
                        case -1: color2 = Color.CornflowerBlue;
                            break;
                    }

                    //HM: creating the player's rectangle so I can check for collision
                    playerRect = new Rectangle(player.XPos, player.YPos, Constants.TILE_WIDTH, Constants.TILE_HEIGHT);
                    // there are 5 frames in the pole sprite Sheet
                    charAnimation.animate(color2, playerRect, gameTime);

                    //HM:checking for the collision between player and goal
                    if (goalRect.Intersects(playerRect))
                    {
                        spriteBatch.Draw(images["goalFinish"], goalRect, Color.White);
                    }
                    
                    tail1.followChar();
                    tail2.followFollower();
                    tail3.followFollower();

                    tail3.Draw(color2, spriteBatch, gameTime);
                    tail2.Draw(color2, spriteBatch, gameTime);
                    tail1.Draw(color2, spriteBatch, gameTime);
                    charAnimation.animate(color2, playerRect, gameTime);

                    spriteBatch.Draw(images["test"], new Rectangle(0, 0, Constants.SCREEN_WIDTH, Constants.SCREEN_HEIGHT), new Color(0, 0, 0, 150));

                    //drawing out the timer
                    spriteBatch.DrawString(text, (int)totalMins % 60 + " : " + (int)totalSecs % 60 + " : " + (int)totalMils % 1000, new Vector2(10, 10), Color.White);

                    spriteBatch.DrawString(text, "PAUSED\n\n[A] - reset\n[D] - exit",
                        new Vector2(300,350), Color.White);
                    break;
                #endregion

                #region Case: Level Complete
                case GameState.LevelComplete:
                    spriteBatch.Draw(images["title"], new Rectangle(0, 0, Constants.SCREEN_WIDTH, 571), Color.White);
                    // cover up the PRESS ENTER from the title image
                    spriteBatch.Draw(images["test"], new Rectangle(200, 350, 400, 100), Color.Black);

                    spriteBatch.DrawString(text, "Level complete!\nPress [ENTER] to continue", new Vector2(300, 400), Color.White);
                    
                    //printing out the score
                    spriteBatch.DrawString(text, "Your score: "+ levelScore, new Vector2(300, 450), Color.White);
                    break;
                #endregion

                #region Case: Edit
                case GameState.Edit:
                    // draw the map maker
                    if (mapMaker != null)
                    {
                        mapMaker.Draw(spriteBatch);
                    }
                    break;
                #endregion

                #region Case: HighScore
                case GameState.HighScores:
                    spriteBatch.Draw(images["title"], new Rectangle(0, 0, Constants.SCREEN_WIDTH, 571), Color.White);
                    // cover up the PRESS ENTER from the title image
                    spriteBatch.Draw(images["test"], new Rectangle(200, 350, 400, 100), Color.Black);

                    // Open the high score list
                    spriteBatch.DrawString(text, "Lowest Game Times", new Vector2(300, 400), Color.Tomato);
                    scorePrint = String.Format("{0:0.00}", getScore.HighScoreList[0]);
                    spriteBatch.DrawString(text, "\n" + 1 + ": " + scorePrint, new Vector2(350, 410), Color.White);

                    for (int i = 1; i < getScore.HighScoreList.Length - 5; i++)
                    {     
                        scorePrint = String.Format("{0:0.00}", getScore.HighScoreList[i]);
                        spriteBatch.DrawString(text, "\n" + (i + 1) + ": " + scorePrint, new Vector2(250, 410 + (i*50)), Color.White);
                    }
                    for (int i = 6; i < getScore.HighScoreList.Length; i++)
                    {
                        scorePrint = String.Format("{0:0.00}", getScore.HighScoreList[i]);
                        spriteBatch.DrawString(text, "\n" + (i + 1) + ": " + scorePrint, new Vector2(450, 410 + ((i-5) * 50)), Color.White);
                    }
                    break;
                #endregion

                #region Case: GameFinish
                case GameState.GameFinish:
                    spriteBatch.Draw(images["title"], new Rectangle(0, 0, Constants.SCREEN_WIDTH, 571), Color.White);
                    spriteBatch.DrawString(text, "Congradulations! You Finished the game!!\n" + "Level Time:" + levelScore +
                "\nYour Final Score:" + totalScore, new Vector2(0, 500), Color.White);
                    break;
                #endregion
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion

        #region SingleKeyPress
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
        #endregion

        #region BorderCollision
        // helper method: keep player on the screen by bouncing off the walls
        public void BorderCollision()
        {
            // player moving off left of screen
            if (player.XPos <= 0)
            {
                player.XPos = 0;
                player.XSpeed = -(player.XSpeed /2);
            }

            // player moving off right of screen
            else if ((player.XPos + Constants.TILE_WIDTH) >= Constants.SCREEN_WIDTH)
            {
                player.XPos = (Constants.SCREEN_WIDTH - Constants.TILE_WIDTH);
                player.XSpeed = -(player.XSpeed /2);
            }

            // player moving off top of screen
            if (player.YPos <= 0)
            {
                player.YPos = 0;
                player.YSpeed = -(player.YSpeed /2);
            }

            // player moving off bottom of screen
            else if ((player.YPos + Constants.TILE_HEIGHT) >= Constants.SCREEN_HEIGHT)
            {
                player.YPos = (Constants.SCREEN_HEIGHT - Constants.TILE_HEIGHT);
                player.YSpeed = -(player.YSpeed /2);
            }
        }
        #endregion

        #region CheckLevelCount
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
        #endregion
    }
}
