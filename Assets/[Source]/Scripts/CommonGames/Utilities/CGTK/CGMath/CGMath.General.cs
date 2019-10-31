namespace CommonGames.Utilities.CGTK
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Utilities.Extensions;
    using static System.Math;

    public static partial class CGMath
    {
        //TODO: Comments & Summaries
        
        /// <summary>
        /// Used mainly in Monte Carlo algorithms to give an algorithm some exploration room.
        /// </summary>
        public static double UCT(float value, int visitedAmount) =>
            value / (visitedAmount + Mathf.Epsilon) + Mathf.Sqrt(Mathf.Log(visitedAmount + 1) / (visitedAmount + Mathf.Epsilon)) + Mathf.Epsilon;

        /// <summary>
        /// Clamps a double between min and max.
        /// </summary>
        public static double Clamp(double d, double min, double max)
        {
            if (d < min)
            { d = min; }
            else if (d > max)
            { d = max; }
            
            return d;
        }
    }
}