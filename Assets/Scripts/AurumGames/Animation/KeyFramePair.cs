using System;
using UnityEngine;

namespace AurumGames.Animation
{
    /// <summary>
    /// Pair of keyframes
    /// </summary>
    /// <typeparam name="T">Track data type</typeparam>
    public sealed class KeyFramePair<T>
    {
        public readonly KeyFrame<T> From;
        public readonly KeyFrame<T> To;

        private readonly float _duration;

        private KeyFramePair(KeyFrame<T> from, KeyFrame<T> to)
        {
            From = from;
            To = to;

            _duration = to.Time - from.Time;
            
            if (_duration <= 0) 
                throw new ArgumentException("Re-range frames");
        }

        /// <summary>
        /// Checks if times lays between frames in pair
        /// </summary>
        /// <param name="time">Time</param>
        /// <returns>True if time lays between frames in pair</returns>
        public bool Intersecting(float time)
        {
            return From.Time <= time && To.Time >= time;
        }

        /// <summary>
        /// Interpolating t
        /// </summary>
        /// <param name="time">t</param>
        /// <returns>Interpolated t</returns>
        public float EasedTimePercentage(float time)
        {
            return From.Ease(Mathf.Clamp01((time - From.Time) / _duration));
        }

        /// <summary>
        /// Create pairs from keyframes array
        /// </summary>
        /// <param name="frames">Keyframes</param>
        /// <returns>Pairs array</returns>
        /// <exception cref="ArgumentException">Must pass minimum 2 frames</exception>
        public static KeyFramePair<T>[] FromFrames(params KeyFrame<T>[] frames)
        {
            if (frames.Length < 2)
                throw new ArgumentException("Require minimum 2 frames");
            
            var previousFrame = frames[0];
            var pairs = new KeyFramePair<T>[frames.Length - 1];
            for (var i = 1; i < frames.Length; i++)
            {
                var currentFrame = frames[i];
                pairs[i - 1] = new KeyFramePair<T>(previousFrame, currentFrame);
                previousFrame = currentFrame;
            }

            return pairs; 
        }
    }
}