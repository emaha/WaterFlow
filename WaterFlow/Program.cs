using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace WaterFlow
{
    internal class Program
    {
        private static bool isExit = false;

        private static void Main(string[] args)
        {
            CellManager _cellManager = new CellManager();
            _cellManager.Init();

            Clock clock = new Clock();
            Time UPS = Time.FromSeconds(1 / 60.0f);
            Time accum = Time.Zero;

            RenderWindow window = new RenderWindow(new VideoMode(1200, 800), "Solar");
            window.Closed += OnClose;
            window.KeyPressed += OnKeyPressed;
            window.SetActive();

            int timer = 0;

            while (window.IsOpen && !isExit)
            {
                window.Clear();
                window.DispatchEvents();

                if (accum >= UPS)
                {
                    accum -= UPS;
                    _cellManager.HandleKeyboard();
                    _cellManager.Update();
                }

                _cellManager.Draw(window);

                accum += clock.Restart();
                window.Display();
            }
        }

        private static void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
            {
                isExit = true;
            }
        }

        private static void OnClose(object sender, EventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }
    }
}