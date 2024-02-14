using System;
using System.Collections.Generic;
using UnityEngine;

namespace AurumGames.Animation
{
    /// <summary>
    /// Interpolation functions
    /// </summary>
    public static class Easing
    {
        public static readonly Func<float, float> Linear = LinearFunction;
        
        public static readonly Func<float, float> CubicIn = CubicInFunction;
        
        public static readonly Func<float, float> QuadOut = QuadOutFunction;
        public static readonly Func<float, float> QuartOut = QuartOutFunction;
        
        public static readonly Func<float, float> QuintIn = QuintInFunction;
        public static readonly Func<float, float> QuintOut = QuintOutFunction;
        public static readonly Func<float, float> QuintInOut = QuintInOutFunction;
        
        public static readonly Func<float, float> BounceOut25 = BounceOut25Function;
        public static readonly Func<float, float> BounceOut20 = BounceOut20Function;
        public static readonly Func<float, float> BounceOut15 = BounceOut15Function;
        
        public static readonly Func<float, float> OutBack = OutBackFunction;
        
        public static readonly Dictionary<string, Func<float, float>> Functions = new()
        {
            {nameof(Linear), Linear},
            
            {nameof(CubicIn), CubicIn},
            
            {nameof(QuadOut), QuadOut},
            {nameof(QuartOut), QuartOut},
            
            {nameof(QuintIn), QuintIn},
            {nameof(QuintOut), QuintOut},
            {nameof(QuintInOut), QuintInOut},
            
            {nameof(BounceOut25), BounceOut25},
            {nameof(BounceOut20), BounceOut20},
            {nameof(BounceOut15), BounceOut15},
            
            {nameof(OutBack), OutBack},
        };
        
        public static float LinearFunction(float t) => t;
        
        public static float CubicInFunction(float t) => t * t * t;

        public static float QuadOutFunction(float t) => 1 - Mathf.Pow(1 - t, 2f);
        public static float QuartOutFunction(float t) => 1 - Mathf.Pow(1 - t, 4f);

        public static float QuintInFunction(float t) => t * t * t * t * t;
        public static float QuintOutFunction(float t) => 1 + (--t) * t * t * t * t;
        public static float QuintInOutFunction(float t) => t < 0.5 ? 16 * t * t * t * t * t : 1 + 16 * (--t) * t * t * t * t;

        public static float BounceOut25Function(float t) => 49f / 40f - Mathf.Pow(t - 7f / 10f, 2) * 2.5f;
        public static float BounceOut20Function(float t) => 9f / 8f - Mathf.Pow(t - 3f / 4f, 2) * 2;
        public static float BounceOut15Function(float t) => 25f / 24f - Mathf.Pow(t - 5f / 6f, 2) * 1.5f;

        public static float OutBackFunction(float t) {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;

            return 1 + c3 * Mathf.Pow(t - 1, 3f) + c1 * Mathf.Pow(t - 1, 2f);
        }

        /// <summary>
        /// Animation curve from 2 points
        /// </summary>
        /// <param name="p0">x1</param>
        /// <param name="p1">y1</param>
        /// <param name="p2">x2</param>
        /// <param name="p3">y2</param>
        /// <returns>Easing f(t)</returns>
        public static Func<float, float> CubicBezier(float p0, float p1, float p2, float p3)
        {
            // Source - https://stackoverflow.com/a/34457425
            p0 = Mathf.Clamp01(p0);
            p2 = Mathf.Clamp01(p2);

            var cx = 3 * p0;
            var bx = 3 * (p2 - p0) - cx;
            var ax = 1 - cx - bx;
            
            var cy = 3 * p1;
            var by = 3 * (p3 - p1) - cy;
            var ay = 1 - cy - by;

            static float Curve(float c, float b, float a, float t)
            {
                return ((a * t + b) * t + c) * t;
            }

            static float Derivative(float c, float b, float a, float t)
            {
                return (3 * a * t + 2 * b) * t + c;
            }

            float SolveX(float t)
            {
                float t2, x2;
                int i;
                const float epsilon = 0.02f;
                
                for (t2 = t, i = 0; i < 8; i++) {
                    x2 = Curve(cx, bx, ax, t2) - t;
                    if (Mathf.Abs(x2) < epsilon)
                        return t2;
                    
                    var d2 = Derivative(cx, bx, ax, t2);
                    if (Mathf.Abs(d2) < 1e-6)
                        break;
                    
                    t2 -= x2 / d2;
                }
                
                var t0 = 0f;
                var t1 = 1f;
                t2 = t;

                if (t2 < t0)
                    return t0;
                if (t2 > t1)
                    return t1;

                while (t0 < t1) {
                    x2 = Curve(cx, bx, ax, t2);
                    if (Mathf.Abs(x2 - t) < epsilon)
                        return t2;
                    
                    if (t > x2)
                    {
                        t0 = t2;
                    }
                    else
                    {
                        t1 = t2;
                    }

                    t2 = (t1 - t0) * 0.5f + t0;
                }

                return t2;
            }
            
            return (t) => Curve(cy, by, ay, SolveX(t));
        }
    }
}