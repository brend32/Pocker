using System;
using System.Reflection;

namespace AurumGames.Animation
{
    /// <summary>
    /// Transition between changes
    /// </summary>
    public class Transition
    {
        public readonly float Time;
        public readonly float Delay;
        public readonly Func<float, float> Ease;
        
        public float Duration => Time + Delay;

        /// <summary>
        /// Transition between changes
        /// </summary>
        /// <param name="time">Duration (ms)</param>
        /// <param name="ease">Easing</param>
        /// <param name="delay">Delay before start (ms)</param>
        public Transition(float time, Func<float, float> ease = null, float delay = 0)
        {
            Time = time;
            Delay = delay;
            Ease = ease ?? ((t) => t);
        }
        
        public KeyFrame<T>[] GetKeyFrames<T>(T from, T to)
        {
            return new[]
            {
                new KeyFrame<T>(Delay, from, Ease),
                new KeyFrame<T>(Time + Delay, to),
            };
        }
    }
    
    /// <summary>
    /// Transition between changes
    /// </summary>
    public struct TransitionStruct
    {
        public readonly float Time;
        public readonly float Delay;
        public readonly Func<float, float> Ease;
        
        public float Duration => Time + Delay;

        /// <summary>
        /// Transition between changes
        /// </summary>
        /// <param name="time">Duration (ms)</param>
        /// <param name="ease">Easing</param>
        /// <param name="delay">Delay before start (ms)</param>
        public TransitionStruct(float time, Func<float, float> ease = null, float delay = 0)
        {
            Time = time;
            Delay = delay;
            Ease = ease ?? ((t) => t);
        }

        public KeyFrame<T>[] GetKeyFrames<T>(T from, T to)
        {
            return new[]
            {
                new KeyFrame<T>(Delay, from, Ease),
                new KeyFrame<T>(Time + Delay, to),
            };
        }
        
        public static implicit operator TransitionStruct(SerializableTransition transition) => new(transition.Time, transition.GetEase(), transition.Delay);
    }

    [Serializable]
    public class SerializableTransition
    {
        public float Time;
        public float Delay;
        public string Ease;

        public float Duration => Time + Delay;

        public SerializableTransition(float time, string ease = nameof(Easing.Linear), float delay = 0)
        {
            Time = time;
            Delay = delay;
            Ease = ease;
        }
        
        public Func<float, float> GetEase()
        {
            return Easing.Functions[Ease];
        }
        
        public KeyFrame<T>[] GetKeyFrames<T>(T from, T to)
        {
            return new[]
            {
                new KeyFrame<T>(Delay, from, GetEase()),
                new KeyFrame<T>(Time + Delay, to),
            };
        }
    }
}