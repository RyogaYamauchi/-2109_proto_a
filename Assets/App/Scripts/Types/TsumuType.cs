using System;
using UnityEngine;

namespace App.Types
{
    public enum TsumuType
    {
        Red,
        Green,
        Blue,
        Yellow,
    }

    public class TsumuColor
    {
        public static Color ConvertTsumuColor(TsumuType type)
        {
            switch (type)
            {
                case TsumuType.Red:
                    return Color.red;
                case TsumuType.Blue:
                    return Color.blue;
                case TsumuType.Green:
                    return Color.green;
                case TsumuType.Yellow:
                    return Color.yellow;
            }

            throw new ArgumentException($"指定されたTypeに対する色がありません{type}");
        }
    }
}