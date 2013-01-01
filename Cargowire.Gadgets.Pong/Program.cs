using System;
using Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;

using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Color = Microsoft.SPOT.Presentation.Media.Color;

namespace Cargowire.Gadgets.Pong
{
    /// <summary>The main program loop for a simple pong game</summary>
    public partial class Program
    {
        /// <summary>Canvas to draw onto</summary>
        private Canvas canvas;
        
        /// <summary>The game rendering</summary>
        private Bitmap game;

        /// <summary>The game window</summary>
        private Window window;

        
        /// Some fixed values related to the 128x128 display        
        private double x1 = 0;
        private double x2 = 127;
        private int score1 = 0;

        private double y1 = 0;
        private double y2 = 0;
        private int score2 = 0;

        private double ballx = 63;
        private double bally = 63;

        private double ballStep = 3;
        private double xBallAcceleration = 0;
        private double yBallAcceleration = 0;

        private Font scoreFont;

        private bool IsPlaying = true;

        /// <summary>This method is run when the mainboard is powered up or reset</summary>
        public void ProgramStarted()
        {
            Debug.Print("Program Started");

            this.canvas = new Canvas();
            
            this.window = this.oledDisplay.WPFWindow;
            this.window.Child = this.canvas;

            Timer timer = new Timer(34);
            timer.Tick += new Timer.TickEventHandler(TimerTick);
            timer.Start();

            game = new Bitmap(SystemMetrics.ScreenWidth, SystemMetrics.ScreenHeight);
            scoreFont = Resources.GetFont(Resources.FontResources.NinaB);

            joystick.JoystickReleased += new Joystick.JoystickEventHandler(Player1JoystickReleased);

            InitBall();
        }

        public void InitBall()
        {
            double angle = (new Random().NextDouble() * 45);
            this.xBallAcceleration = System.Math.Cos((System.Math.PI / 180) * angle) * ballStep;
            this.yBallAcceleration = System.Math.Sin((System.Math.PI / 180) * angle) * ballStep;

            if (System.DateTime.Now.Millisecond % 2 == 0)
            {
                this.xBallAcceleration = -this.xBallAcceleration;
            }
            if(System.DateTime.Now.Millisecond % 5 == 0)
            {
                this.yBallAcceleration = -this.yBallAcceleration;
            }
        }

        public void Update()
        {
            if (IsPlaying)
            {
                Joystick.Position pos1 = this.joystick.GetJoystickPosition();
                Joystick.Position pos2 = this.joystick1.GetJoystickPosition();

                int step = 10;

                // Player one
                double y1Acceleration = pos1.Y - 0.5;
                this.y1 -= step*y1Acceleration;
                if (this.y1 + 20 > 127)
                    this.y1 = 127 - 20;
                if (this.y1 < 0)
                    this.y1 = 0;

                // Player two
                double y2Acceleration = pos2.Y - 0.5;
                this.y2 -= (step*y2Acceleration);
                if (this.y2 + 20 > 127)
                    this.y2 = 127 - 20;
                if (this.y2 < 0)
                    this.y2 = 0;

                // Ball
                if (ballx <= 0 && bally > y1 && bally < y1 + 20)
                {
                    // We have a bounce
                    ballx -= y1Acceleration;

                    xBallAcceleration = -xBallAcceleration;
                }
                else if (ballx <= 0)
                {
                    this.score2++;
                    this.ballx = 63;
                    this.bally = 63;
                    InitBall();
                }

                if (this.ballx >= 127 && this.bally > this.y2 && this.bally < this.y2 + 20)
                {
                    // We have a bounce
                    this.ballx -= y1Acceleration;

                    this.xBallAcceleration = -this.xBallAcceleration;
                }
                else if (ballx >= 127)
                {
                    this.score1++;
                    this.ballx = 63;
                    this.bally = 63;
                    InitBall();
                }

                if (bally < 0)
                {
                    this.yBallAcceleration = -this.yBallAcceleration;
                }

                if (bally > 127)
                {
                    this.yBallAcceleration = -this.yBallAcceleration;
                }

                // Game Over
                if (score1 >= 10 || score2 >= 10)
                {
                    IsPlaying = false;
                }
                else
                {
                    this.ballx += this.xBallAcceleration;
                    this.bally -= this.yBallAcceleration;
                }
            }
        }

        public void Draw()
        {
            this.game.Clear();
            this.game.DrawEllipse(Color.White, (int)this.ballx, (int)this.bally, 1, 1);
            this.game.DrawLine(Colors.Green, 1, (int)this.x1, (int)this.y1, (int)this.x1, (int)this.y1 + 20);
            this.game.DrawLine(Colors.Red, 1, (int)this.x2, (int)this.y2, (int)this.x2, (int)this.y2 + 20);
            this.game.DrawText(this.score1.ToString("G"), this.scoreFont, Colors.Magenta, 50, 100);
            this.game.DrawText(this.score2.ToString("G"), this.scoreFont, Colors.Magenta, 77, 100);
            this.game.Flush();

            this.oledDisplay.SimpleGraphics.DisplayImage(this.game, 0, 0);
        }

        private void TimerTick(Timer timer)
        {
            Update();
            Draw();
        }

        private void Player1JoystickReleased(Joystick sender, Joystick.JoystickState state)
        {
            if (!IsPlaying)
            {
                score1 = 0;
                score2 = 0;
                IsPlaying = !IsPlaying;
            }
        }
    }
}