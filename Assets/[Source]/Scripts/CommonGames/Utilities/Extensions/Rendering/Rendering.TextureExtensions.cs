using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonGames.Utilities.Extensions
{
    public static partial class Rendering
    {
        public static T[,] GetDataFromPixels<T>(this Texture2D texture, Vector2Int size, Func<Color, int, int, T> func)
        {
            T[,] values = new T[size.x, size.y];
            Color[] colors = texture.GetPixels();

            Vector2Int blockSize = new Vector2Int(texture.width / size.x, texture.height / size.y);

            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                {
                    int _x = blockSize.x * x, _y = blockSize.y * y;
                    Color color = colors[_x + _y * size.x * blockSize.x];
                    values[x, y] = func(color, x, y);
                }

            return values;
        }
    }
}