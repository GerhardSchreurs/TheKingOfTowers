using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MouseKeyboardLibrary;

namespace TowerTool
{
    public partial class Form1 : Form
    {
        [DllImport("USER32.DLL")]
        public static extern void mouse_event(long dwFlags);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private Bitmap TakeScreenShot(int x, int y, int width, int height)
        {
            Rectangle rect = new Rectangle(x, y, width, height);
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rect.Left, rect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
            }

            return bmp;
        }

        void RestartGame()
        {
            Thread.Sleep(1000);

            //Open dialog
            MouseSimulator.X = 1300;
            MouseSimulator.Y = 178;
            MouseSimulator.Click(MouseButton.Left);

            Thread.Sleep(700);

            //Click restart;
            MouseSimulator.X = 950;
            MouseSimulator.Y = 600;
            MouseSimulator.Click(MouseButton.Left);

            Thread.Sleep(1500);

            //Move mouse to info position
            MouseSimulator.X = 600;
            MouseSimulator.Y = 655;
            MouseSimulator.Show();

            Thread.Sleep(300);
            //remove mouse in case of reset
            MouseSimulator.X = 600;
            MouseSimulator.Y = 655;
            Thread.Sleep(1);
        }

        Bitmap Duplicate(Bitmap bitmap)
        {
            return bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat);
        }

        bool PixelIsColor(int x, int y, int r, int g, int b)
        {
            var screenshot = TakeScreenShot(x, y, 1, 1);
            BackgroundImage = Duplicate(screenshot);
            var color = screenshot.GetPixel(0, 0);
            screenshot.Dispose();

            if (color.R == r && color.G == g && color.B == b)
            {
                return true;
            }

            return false;
        }

        async Task MainAsync()
        {
            var result = await MainLoop();

        }

        bool FindThief()
        {
            RestartGame();
            Thread.Sleep(1000);
            return PixelIsColor(700, 740, 248, 201, 165);
        }

        Point FindColorPositionInBitmap(Bitmap bitmap, int r, int g, int b)
        {
            var point = new Point(0,0);

            for (var i = 0; i < bitmap.Width; i++)
            {
                for (var j = 0; j < bitmap.Height; j++)
                {
                    var color = bitmap.GetPixel(i, j);
                    if (color.R == r && color.G == g && color.B == b)
                    {
                        point.X = i;
                        point.Y = j;
                        return point;
                    }
                }
            }

            return point;
        }

        private async Task<bool> MainLoop()
        {
            var loopCount = 0;

            await Task.Run(() =>
            {
                try
                {
                    while (loopCount < 10000)
                    {
                        var foundThief = FindThief();

                        if (foundThief == true)
                        {
                            Log("Found thief (" + loopCount + ")");

                            //Click start
                            MouseSimulator.X = 600;
                            MouseSimulator.Y = 655;
                            MouseSimulator.Click(MouseButton.Left);

                            //wait
                            Thread.Sleep(5000);

                            for (var i = 0; i < 20; i++)
                            {
                                Thread.Sleep(1000);
                                //Click pause

                                MouseSimulator.X = 1248;
                                MouseSimulator.Y = 188;
                                MouseSimulator.Click(MouseButton.Left);

                                Thread.Sleep(200);

                                //Take screenshot
                                var screenshot = TakeScreenShot(578, 551, 600, 190);
                                //BackgroundImage = screenshot.Clone();

                                var thiefPos = FindColorPositionInBitmap(screenshot, 147, 134, 114);

                                if (thiefPos.X != 0 && thiefPos.Y != 0)
                                {
                                    //we found the motherfucker!
                                    thiefPos.X += 578;
                                    thiefPos.Y += 551;

                                    //unpause the game
                                    MouseSimulator.X = 955;
                                    MouseSimulator.Y = 532;
                                    MouseSimulator.Click(MouseButton.Left);
                                    KeyboardSimulator.KeyPress(Keys.S);

                                    Thread.Sleep(100);

                                    //click the mother!
                                    MouseSimulator.X = thiefPos.X;
                                    MouseSimulator.Y = thiefPos.Y;
                                    MouseSimulator.Click(MouseButton.Left);

                                    Thread.Sleep(2000);

                                    screenshot = TakeScreenShot(578, 551, 600, 190);
                                    var treasurePos = FindColorPositionInBitmap(screenshot, 94, 79, 46);
                                    treasurePos.X += 578;
                                    treasurePos.Y += 551;

                                    MouseSimulator.X = treasurePos.X;
                                    MouseSimulator.Y = treasurePos.Y;
                                    MouseSimulator.Click(MouseButton.Left);

                                    Thread.Sleep(300);

                                    MouseSimulator.X = treasurePos.X;
                                    MouseSimulator.Y = treasurePos.Y;
                                    MouseSimulator.Click(MouseButton.Left);

                                    Thread.Sleep(300);

                                    MouseSimulator.X = treasurePos.X;
                                    MouseSimulator.Y = treasurePos.Y;
                                    MouseSimulator.Click(MouseButton.Left);

                                    Thread.Sleep(1000);
                                    i = 20;
                                    Log("Clicked, exit");

                                    screenshot.Dispose();
                                }

                                //1248 188
                            }
                        }

                        loopCount++;
                    }
                }
                catch (Exception ex)
                {
                    Log("exception in mainloop: " + ex);
                }
            });

            return true;
        }

        private void Log(string text)
        {
            File.AppendAllText(@"D:\tower.txt", DateTime.Now + " " + text + Environment.NewLine);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            Log("started");
            try
            {
                Win32.FocusGame();
                Thread.Sleep(100);
                await MainLoop();
            }
            catch (Exception ex)
            {
               Log("exception in outer: " + ex); 
            }
            Log("stopped");
        }
    }
}
