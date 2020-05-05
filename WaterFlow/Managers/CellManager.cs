using SFML.Graphics;
using SFML.System;
using System;

namespace WaterFlow.Managers
{
    public class CellManager
    {
        private const int Width = 120;
        private const int Height = 80;
        private const int CellWidth = 10;
        private const int CellHeight = 10;
        private static Cell[,] map = new Cell[Width, Height];
        private static RectangleShape[,] shapes = new RectangleShape[Width, Height];

        // Гравитация
        public static float Gravity = 100f;

        // Текучесть
        public static float Fluent = 1f;

        // Выталкиваюшая сила
        public static float ArchForce = 0f;

        public void Init()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    shapes[x, y] = new RectangleShape(new Vector2f(CellWidth, CellHeight));
                    shapes[x, y].Position = new Vector2f(x * CellHeight, y * CellHeight);
                }
            }

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

            map[23, 23].Preasure = -25000;
            map[63, 23].Preasure = 300000;
        }

        public void Update()
        {
            for (int x = 1; x < Width - 1; x++)
            {
                for (int y = 1; y < Height - 1; y++)
                {
                    if (map[x, y].isStatic) continue;
                    CalculateCell(x, y);
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

        public void Draw(RenderTarget target)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var pres = map[x, y].Preasure;
                    if (pres > 0)
                    {
                        if (pres > 255) pres = 255;
                        shapes[x, y].FillColor = new Color(0, 0, (byte)pres);
                    } 
                    else
                    {
                        if (pres < -255) pres = -255;
                        shapes[x, y].FillColor = new Color((byte)Math.Abs(pres),0,0);
                    }

                    target.Draw(shapes[x, y]);
                }
            }
        }

        private static void CalculateCell(int x, int y)
        {
            int cnt = 0;
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (map[x + dx, y + dy].isStatic || dx == 0 && dy == 0) continue;
                    cnt++;
                }
            }

            if (cnt == 0) return;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (map[x + dx, y + dy].isStatic || (dx == 0 && dy == 0)) continue;

                    float diff = map[x, y].Preasure - map[x + dx, y + dy].Preasure;

                    if (diff > 0)
                        //(dy == -1 && diff > Gravity ||   // гравитация(сила не дающая распространяться вверх или плотность слоев)
                        //dy == 0 && diff > Fluent ||     // текучесть(в стороны)
                        //dy == 1 && diff > ArchForce))    // сила выталкивания (чем глубже, тем сильнее давление), способность более высокого слоя проникать в нижний   
                    {
                        //if(dy == 1) diff *= 1.3f;
                        //if (dy == -1) diff *= 0.7f;


                        map[x + dx, y + dy].PreCalcPreasure += diff / cnt;
                        map[x, y].PreCalcPreasure -= diff / cnt;
                    }
                }
            }
        }
    }
}
