using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using WaterFlow.Managers;

namespace WaterFlow
{
    internal class Program
    {
        private static bool isExit = false;

        private static void Main(string[] args)
        {
            CellManager cellManager = new CellManager();
            cellManager.Init();

            Clock clock = new Clock();
            Time UPS = Time.FromSeconds(1 / 60.0f);
            Time accum = Time.Zero;

            RenderWindow window = new RenderWindow(new VideoMode(1200, 800), "Solar");
            window.Closed += OnClose;
            window.KeyPressed += OnKeyPressed;
            window.SetActive();

            while (window.IsOpen && !isExit)
            {
                window.Clear();
                window.DispatchEvents();

                if (accum >= UPS)
                {
                    accum -= UPS;
                    HandleKeyboard();

                    cellManager.Update();
                }

                cellManager.Draw(window);

                accum += clock.Restart();
                window.Display();
            }

        }

        public static void HandleKeyboard()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Q)) { CellManager.Gravity += 50f; Console.WriteLine($"Gravity: {CellManager.Gravity}"); }
            if (Keyboard.IsKeyPressed(Keyboard.Key.A)) { CellManager.Gravity -= 50f; Console.WriteLine($"Gravity: {CellManager.Gravity}"); }
            if (Keyboard.IsKeyPressed(Keyboard.Key.W)) { CellManager.Fluent += 50f; Console.WriteLine($"Fluent: {CellManager.Fluent}"); }
            if (Keyboard.IsKeyPressed(Keyboard.Key.S)) { CellManager.Fluent -= 50f; Console.WriteLine($"Fluent: {CellManager.Fluent}"); }
            if (Keyboard.IsKeyPressed(Keyboard.Key.E)) { CellManager.ArchForce += 50f; Console.WriteLine($"ArchForce: {CellManager.ArchForce}"); }
            if (Keyboard.IsKeyPressed(Keyboard.Key.D)) { CellManager.ArchForce -= 50f; Console.WriteLine($"ArchForce: {CellManager.ArchForce}"); }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Space)) {  }
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