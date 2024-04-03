using System;
using System.Linq;

namespace AurumGames.Animation
{
    /// <summary>
    /// Group of tracks
    /// </summary>
    public sealed class TracksEvaluator : ITrack
    {
        public float FullDuration => _duration + _startDelay;
        /// <summary>
        /// Play backwards
        /// </summary>
        public bool PlayBackwards { get; set; }
        /// <summary>
        /// Current time
        /// </summary>
        public float Position { get; private set; }
        
        private ITrack[] _tracks;
        private float _duration;
        private float _startDelay;

        /// <summary>
        /// Group of tracks
        /// </summary>
        /// <param name="tracks">Tracks</param>
        /// <param name="startDelay">Animation start delay</param>
        /// <exception cref="Exception">Minimum 1 track required</exception>
        public TracksEvaluator(float startDelay = 0, params ITrack[] tracks)
        {
            if (tracks is null || tracks.Length == 0)
                throw new Exception("No tracks");
            
            SetTracks(startDelay, tracks);
        }
        
        /// <summary>
        /// Group of tracks
        /// </summary>
        /// <param name="tracks">Tracks</param>
        /// <exception cref="Exception">Minimum 1 track required</exception>
        public TracksEvaluator(params ITrack[] tracks) : this(0, tracks)
        {
            
        }
        
        /// <summary>
        /// Set new tracks
        /// </summary>
        /// <param name="tracks">New tracks</param>
        /// <param name="startDelay">Animation start delay</param>
        public void SetTracks(float startDelay, params ITrack[] tracks)
        {
            _startDelay = startDelay;
            _tracks = tracks;
            _duration = _tracks.MaxTrackDuration();
            
            Reset();
        }

        /// <summary>
        /// Set new tracks
        /// </summary>
        /// <param name="tracks">New tracks</param>
        public void SetTracks(params ITrack[] tracks)
        {
            SetTracks(0, tracks);
        }
        
        /// <summary>
        /// Reset track state and position
        /// </summary>
        public void Reset()
        {
            ResetTracksState();
            Position = PlayBackwards ? _duration + _startDelay : 0;
        }
        
        private void ResetTracksState()
        {
            foreach (ITrack track in _tracks)
            {
                track.ResetState();
            }
        }

        /// <summary>
        /// Jump to <paramref name="time"/>
        /// </summary>
        /// <param name="time">Time</param>
        public void EvaluateAt(float time)
        {
            InternalEvaluateAt(time, PlayBackwards);
        }

        private void InternalEvaluateAt(float time, bool isBackwards)
        {
            Position = time;
            time = MathF.Max(time - _startDelay, 0);
            foreach (ITrack track in _tracks)
            {
                track.Evaluate(time, isBackwards);
            }
        }
        
        /// <summary>
        /// Jump to time in percents
        /// </summary>
        /// <param name="t">Percent</param>
        public void EvaluatePercent(float t)
        {
            t = PlayBackwards ? 1 - t : t;
            EvaluateAt(FullDuration * t);
        }

        /// <summary>
        /// Jump to end
        /// </summary>
        public void EvaluateEnd()
        {
            EvaluateAt(PlayBackwards ? 0 : FullDuration);
        }
        
        /// <summary>
        /// Jump to start
        /// </summary>
        public void EvaluateStart()
        {
            EvaluateAt(PlayBackwards ? FullDuration : 0);
        }

        /// <summary>
        /// Calculate change
        /// </summary>
        /// <param name="timeDelta">Time passed</param>
        public void EvaluateNext(float timeDelta)
        {
            EvaluateAt(Position + timeDelta * (PlayBackwards ? -1 : 1));
        }

        /// <summary>
        /// Checks if we reach the end of the track
        /// </summary>
        /// <returns>True if we reached the end of the track</returns>
        public bool IsReachEnd()
        {
            return PlayBackwards ? Position <= 0 : Position >= FullDuration;   
        }

        public void Evaluate(float time, bool isBackwards) 
        {
            InternalEvaluateAt(time, isBackwards ^ PlayBackwards);
        }

        public void ResetState()
        {
            ResetTracksState();
        }
    }
}