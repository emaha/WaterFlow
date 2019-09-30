using System;
using SFML.Graphics;
using SFML.Window;

namespace WaterFlow
{
    internal class Program
    {
        private const int Width = 50;
        private const int Height = 25;
        private static bool isExit = false;

        private static void Main(string[] args)
        {
            /*
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
                    //Offset = Earth.Position / Scale - new Vector2f(600, 400);
                    manager.Update();
                    //Console.WriteLine(Earth.Position + " " + Offset);
                }

                manager.Draw(window);
                accum += clock.Restart();
                window.Display();
            }
            */

            Cell[,] map = new Cell[Width, Height];
            Init(map);
            map[23, 23].Preasure = 50000;
            map[24, 23].Preasure = 5;
            map[25, 23].Preasure = 10;
            int timer = 0;

            while (true)
            {
                Draw(map);
                Console.ReadKey();
                for (int i = 0; i < 1; i++)
                {
                    CalculateMap(map);
                    timer++;
                }
                if (timer % 10 == 0) map[23, 1].Preasure = timer;
            }
        }

        private static void Init(Cell[,] map)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    map[i, j] = new Cell()
                    {
                        Preasure = 0.0f,
                        isStatic = false,
                    };

                    if ((j == Height - 1 || j == 0) || (i == 0 || i == Width - 1))
                    {
                        map[i, j].Preasure = 8;
                        map[i, j].isStatic = true;
                    }
                }
            }
        }

        private static void CalculateMap(Cell[,] map)
        {
            for (int x = 1; x < Width - 1; x++)
            {
                for (int y = 1; y < Height - 1; y++)
                {
                    if (map[x, y].isStatic) continue;
                    CalculateCell(map, x, y);
                }
            }

            //Apply precalc
            for (int x = 1; x < Width - 1; x++)
            {
                for (int y = 1; y < Height - 1; y++)
                {
                    map[x, y].Preasure += map[x, y].PreCalcPreasure;
                    map[x, y].PreCalcPreasure = 0;
                }
            }
        }

        private static void CalculateCell(Cell[,] map, int x, int y)
        {
            if (map[x, y].Preasure < 0.85f && !map[x, y + 1].isStatic)
            {
                map[x, y + 1].PreCalcPreasure += map[x, y].Preasure;
                map[x, y].Preasure = 0;
                return;
            }

            int cnt = 0;
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (map[x + dx, y + dy].isStatic ||
                        dx == 0 && dy == 0) continue;
                    cnt++;
                }
            }

            if (cnt == 0) return;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (map[x + dx, y + dy].isStatic ||
                        dx == 0 && dy == 0 ||
                        map[x, y].Preasure <= 0) continue;

                    float diff = map[x, y].Preasure - map[x + dx, y + dy].Preasure;
                    if (dy == -1 && diff > 5f ||    // гравитация(сила не дающая распространяться вверх или плотность слоев)
                        dy == 0 && diff > 0.01f ||  // текучесть(в стороны)
                        dy == 1 && diff > -10f)    // сила выталкивания (чем глубже, тем сильнее давление), способность более высокого слоя проникать в нижний
                    {
                        map[x + dx, y + dy].PreCalcPreasure += Math.Abs(diff / cnt);
                        map[x, y].PreCalcPreasure -= Math.Abs(diff / cnt);
                    }
                }
            }
        }

        private static void Draw(Cell[,] map)
        {
            Console.Clear();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Console.Write(Math.Ceiling(map[x, y].Preasure) + (map[x, y].Preasure > 9.0f ? "" : " "));
                    //Console.Write(map[x, y].Preasure);
                }
                Console.WriteLine();
            }
        }

        public static void HandleKeyboard()
        {
            float scrollSpeed = 20f;

            if (Keyboard.IsKeyPressed(Keyboard.Key.A)) { }
            if (Keyboard.IsKeyPressed(Keyboard.Key.D)) { }
            if (Keyboard.IsKeyPressed(Keyboard.Key.W)) { }
            if (Keyboard.IsKeyPressed(Keyboard.Key.S)) { }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Q)) { }
            if (Keyboard.IsKeyPressed(Keyboard.Key.E)) { }
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