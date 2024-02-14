using System;

namespace AurumGames.Animation
{
    /// <summary>
    /// Keyframe for animation
    /// </summary>
    /// <typeparam name="T">Track data type</typeparam>
    public sealed class KeyFrame<T>
    {
        public readonly float Time;
        public T Value => _valueGetter is { } ? _valueGetter.Invoke() : _value;
        public readonly Func<float, float> Ease;

        private readonly Func<T> _valueGetter;
        private readonly T _value;
        
        /// <summary>
        /// Keyframe for animation
        /// </summary>
        /// <param name="time">Key frame position (ms)</param>
        /// <param name="value">Value</param>
        /// <param name="ease">Easing</param>
        public KeyFrame(float time, T value, Func<float, float> ease = null) : this(ease)
        {
            Time = time;
            _value = value;
        }
        
        /// <summary>
        /// Keyframe for animation
        /// </summary>
        /// <param name="time">Key frame position (ms)</param>
        /// <param name="value">Value</param>
        /// <param name="ease">Easing</param>
        public KeyFrame(float time, Func<T> value, Func<float, float> ease = null) : this(ease)
        {
            Time = time;
            _valueGetter = value;
        }

        private KeyFrame(Func<float, float> ease = null)
        {
            Ease = ease ?? ((t) => t);
        }
    }
}