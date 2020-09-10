using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace _2DExercise
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D colorStrip;
        Texture2D ball;
        Texture2D im;

        Vector2 upperLeft;
        Vector2 upperRight;
        Vector2 lowerLeft;
        Vector2 lowerRight;

        Rectangle northWall;
        Rectangle southWall;
        Rectangle eastWall;
        Rectangle westWall;
        Rectangle wallColor;

        float eastWallAngle;
        float westWallAngle;
        int lineThickness;

        Vector2 ballPos;
        Vector2 ballVel;

        Vector2 imPos;
        Vector2 imVel;

        Color[] ballData;
        Color[] imData;

        Color[,] ballColor;
        Color[,] imColor;

        SpriteFont dfont;
        Color textColor = Color.White;
        Vector2 scorePos;
        int score = 0;

        bool colliding = true;

        Random rnd = new Random();


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            dfont = Content.Load<SpriteFont>("posdisplayfont");


            lineThickness = 4;

            upperLeft = new Vector2(20, 20);
            upperRight = new Vector2(GraphicsDevice.Viewport.Width - 20, 20);
            lowerLeft = new Vector2(20, GraphicsDevice.Viewport.Height - 20);
            lowerRight = new Vector2(GraphicsDevice.Viewport.Width - 20, GraphicsDevice.Viewport.Height - 20);

            northWall = new Rectangle((int)(upperLeft.X - lineThickness), (int)(upperLeft.Y - lineThickness), (int)(upperRight.X - upperLeft.X), lineThickness);
            southWall = new Rectangle((int)lowerLeft.X, (int)lowerLeft.Y, (int)(lowerRight.X - lowerLeft.X), lineThickness);

            Vector2 eastWallVectorAngle = Vector2.Subtract(upperLeft, lowerLeft);
            eastWall = new Rectangle((int)upperLeft.X, (int)upperLeft.Y, (int)eastWallVectorAngle.Length() + lineThickness, lineThickness);
            eastWallAngle = (float)Math.Atan2(eastWallVectorAngle.Y, eastWallVectorAngle.X);

            Vector2 westWallVectorAngle = Vector2.Subtract(upperRight, lowerRight);
            westWall = new Rectangle((
                int)upperRight.X, (int)(upperRight.Y - lineThickness), (int)westWallVectorAngle.Length() + lineThickness, lineThickness);
            westWallAngle = (float)Math.Atan2(westWallVectorAngle.Y, westWallVectorAngle.X);


            colorStrip = Content.Load<Texture2D>("colorStrip");
            wallColor = new Rectangle(5, 0, 1, 1);

            ball = Content.Load<Texture2D>("ball");
            im = Content.Load<Texture2D>("b1");

            ballPos = new Vector2((float)((rnd.NextDouble() * ((upperRight.X - upperLeft.X) - ball.Width)) + upperLeft.X),
                (float)((rnd.NextDouble() * ((lowerRight.Y - upperRight.Y) - ball.Height)) + upperRight.Y));

            imPos = new Vector2(50, 100);

            ballVel = new Vector2(10);
            imVel = new Vector2(0);

            ballData = new Color[ball.Width * ball.Height];
            imData = new Color[im.Width * im.Height];

            ball.GetData<Color>(ballData);
            im.GetData<Color>(imData);

            ballColor = new Color[ball.Width, ball.Height];
            imColor = new Color[im.Width, im.Height];

            for (int x = 0; x < ball.Width; x++)
            {
                for (int y = 0; y < ball.Height; y++)
                {
                    ballColor[x, y] = ballData[x + y * ball.Width];
                }
            }

            for (int x = 0; x < im.Width; x++)
            {
                for (int y = 0; y < im.Height; y++)
                {
                    imColor[x, y] = imData[x + y * im.Width];
                }
            }
            scorePos = new Vector2((GraphicsDevice.Viewport.Width / 2) - (dfont.MeasureString("0").X / 2), (GraphicsDevice.Viewport.Height / 2) - (dfont.MeasureString("0").Y / 2));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //re-initialize y to 0, otherwise it will get faster and faster.
            //I could have just set the Y velocity if a key was pressed, but this way it takes fewer lines of code to make up and down cancel each other out
            imVel.Y = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                imVel.Y += 5;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                imVel.Y -= 5;
            }

            ballPos += ballVel;

            if (ballPos.Y <= upperLeft.Y)
            {
                ballVel = Vector2.Reflect(ballVel, Vector2.UnitY);
            }

            if (ballPos.Y + ball.Height >= lowerLeft.Y)
            {
                ballVel = Vector2.Reflect(ballVel, Vector2.UnitY);
            }

            if (ballPos.X <= upperLeft.X)
            {
                score = 0;
                textColor = Color.Crimson;
                ballVel = Vector2.Reflect(ballVel, Vector2.UnitX);
            }

            if (ballPos.X + ball.Width >= upperRight.X)
            {
                score++;
                if (score > 9)
                {
                    textColor = Color.Yellow;
                }
                else
                {
                    textColor = Color.White;
                }
                ballVel = Vector2.Reflect(ballVel, Vector2.UnitX);
            }
            scorePos = new Vector2((GraphicsDevice.Viewport.Width / 2) - (dfont.MeasureString(score.ToString()).X / 2), (GraphicsDevice.Viewport.Height / 2) - (dfont.MeasureString(score.ToString()).Y / 2));
            imPos += imVel;

            if (imPos.Y <= upperLeft.Y)
            {
                imPos.Y = upperLeft.Y;
            }

            if (imPos.Y + im.Height >= lowerLeft.Y)
            {
                imPos.Y = (lowerLeft.Y - im.Height);
            }

            Rectangle c1 = new Rectangle((int)ballPos.X, (int)ballPos.Y, (int)ball.Width, (int)ball.Height);
            Rectangle c2 = new Rectangle((int)imPos.X, (int)imPos.Y, (int)im.Width, (int)im.Height);

            bool collided = false;
            if (c1.Intersects(c2))
            {
                bool moveIm = false;
                Rectangle intersect = Rectangle.Intersect(c1, c2);


                for (int x = intersect.X; x < intersect.X + intersect.Width; x++)
                {
                    for (int y = intersect.Y; y < intersect.Y + intersect.Height; y++)
                    {
                        int a1 = ballColor[x - (int)ballPos.X, y - (int)ballPos.Y].A;
                        int a2 = imColor[x - (int)imPos.X, y - (int)imPos.Y].A;
                        if ((a1 != 0) && (a2 != 0))
                        {
                            if (!colliding)
                            {
                                //checks if the collision happened on the X axis or the Y, if it's neither, assume the ball came in
                                //at an angle and needs to change both directions.
                                bool failedToBounce = true;
                                if (((imPos.X < ballPos.X + ball.Width) && (ballPos.X + ball.Width < imPos.X + ballVel.X)) ||
                                    ((imPos.X + im.Width > ballPos.X) && (ballPos.X > imPos.X + im.Width + ballVel.X)))
                                {
                                    failedToBounce = false;
                                    ballVel = Vector2.Reflect(ballVel, Vector2.UnitX);
                                }
                                if (((imPos.Y < ballPos.Y + ball.Height) && (ballPos.Y + ball.Height < imPos.Y + ballVel.Y - imVel.Y)) ||
                                    ((imPos.Y + im.Height > ballPos.Y) && (ballPos.Y > imPos.Y + im.Height + ballVel.Y - imVel.Y)))
                                {
                                    failedToBounce = false;
                                    ballVel = Vector2.Reflect(ballVel, Vector2.UnitY);
                                    moveIm = true;
                                }
                                if (failedToBounce)
                                {
                                    ballVel *= -1;
                                    moveIm = true;
                                }
                                colliding = true;
                                collided = true;
                            }

                        }

                    }
                }
                //By moving the paddle back into it's original position after a y collision, I avoid a problem in which users can squeeze the ball between
                //the paddle and the wall
                if(moveIm)
                {
                    imPos -= imVel;
                }
            }
            if (colliding && !collided)
            {
                colliding = false;
            }



            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkOrange);

            spriteBatch.Begin();
            spriteBatch.Draw(colorStrip, northWall, wallColor, Color.White);
            spriteBatch.Draw(colorStrip, southWall, wallColor, Color.White);
            spriteBatch.Draw(colorStrip, eastWall, wallColor, Color.White, (float)(eastWallAngle + Math.PI), Vector2.Zero, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(colorStrip, westWall, wallColor, Color.White, (float)(westWallAngle + Math.PI), Vector2.Zero, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(ball, ballPos, Color.White);
            spriteBatch.Draw(im, imPos, Color.White);
            spriteBatch.DrawString(dfont, score.ToString(), scorePos, textColor);
            spriteBatch.End();



            base.Draw(gameTime);
        }
    }
}
