using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace WaterFlow
{
    internal class Program
    {
        private const int Width = 120;
        private const int Height = 80;
        private const int CellWidth = 10;
        private const int CellHeight = 10;
        private static bool isExit = false;
        private static Cell[,] map = new Cell[Width, Height];
        private static RectangleShape[,] shapes = new RectangleShape[Width, Height];
        
        // Гравитация
        private static float Gravity = 1f;

        // Текучесть
        private static float Fluent = 1f;

        // Выталкиваюшая сила
        private static float ArchForce = -1f;

        private static void Main(string[] args)
        {
            for(int y = 0; y < Height; y++)
            {
                for(int x = 0; x < Width; x++)
                {
                    shapes[x, y] = new RectangleShape(new Vector2f(CellWidth, CellHeight));
                    shapes[x,y].Position = new Vector2f(x * CellHeight, y * CellHeight);
                }
            }


            Clock clock = new Clock();
            Time UPS = Time.FromSeconds(1 / 60.0f);
            Time accum = Time.Zero;

            RenderWindow window = new RenderWindow(new VideoMode(1200, 800), "Solar");
            window.Closed += OnClose;
            window.KeyPressed += OnKeyPressed;
            window.SetActive();

            
            Init(map);
            map[23, 23].Preasure = 50000;
            map[24, 23].Preasure = 5;
            map[25, 23].Preasure = 10;
            int timer = 0;

            while (window.IsOpen && !isExit)
            {
                window.Clear();
                window.DispatchEvents();

                if (accum >= UPS)
                {
                    accum -= UPS;
                    HandleKeyboard();
                   
                    
                    
                    CalculateMap(map);
                }

                Draw(window, map);

                accum += clock.Restart();
                window.Display();
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
            if (map[x, y].Preasure < 50f && !map[x, y + 1].isStatic)
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
                    if (dy == -1 && diff > Gravity ||    // гравитация(сила не дающая распространяться вверх или плотность слоев)
                        dy == 0 && diff > Fluent ||  // текучесть(в стороны)
                        dy == 1 && diff > ArchForce)    // сила выталкивания (чем глубже, тем сильнее давление), способность более высокого слоя проникать в нижний
                    {
                        map[x + dx, y + dy].PreCalcPreasure += Math.Abs(diff / cnt);
                        map[x, y].PreCalcPreasure -= Math.Abs(diff / cnt);
                    }
                }
            }
        }

        private static void Draw(RenderTarget target, Cell[,] map)
        {
            //target.Clear();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    
                    int pres = (byte)(map[x, y].Preasure*5 + 20);
                    if (pres > 255) pres = 255;

                    shapes[x,y].FillColor = new Color(0, 0, (byte) pres);

                    target.Draw(shapes[x,y]);
                }
            }
        }

        public static void HandleKeyboard()
        {
            float scrollSpeed = 20f;

            if (Keyboard.IsKeyPressed(Keyboard.Key.Q)) { Gravity += 50f; Console.WriteLine($"Gravity: {Gravity}"); }
            if (Keyboard.IsKeyPressed(Keyboard.Key.A)) { Gravity -= 50f; Console.WriteLine($"Gravity: {Gravity}"); }
            if (Keyboard.IsKeyPressed(Keyboard.Key.W)) { Fluent += 50f; Console.WriteLine($"Fluent: {Fluent}"); }
            if (Keyboard.IsKeyPressed(Keyboard.Key.S)) { Fluent -= 50f; Console.WriteLine($"Fluent: {Fluent}"); }
            if (Keyboard.IsKeyPressed(Keyboard.Key.E)) { ArchForce += 50f; Console.WriteLine($"ArchForce: {ArchForce}"); }
            if (Keyboard.IsKeyPressed(Keyboard.Key.D)) { ArchForce -= 50f; Console.WriteLine($"ArchForce: {ArchForce}"); }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Space)) { map[60, 5].Preasure = 50000f; }
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