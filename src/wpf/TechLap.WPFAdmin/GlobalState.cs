﻿namespace TechLap.WPFAdmin
{
    internal class GlobalState
    {
        public static string Token { get; set; } = string.Empty;

        public static void Clear()
        {
            Token = string.Empty;
        }
    }

}
