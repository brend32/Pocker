using AurumGames.Animation.Tracks;
using UnityEngine;

namespace AurumGames.Animation
{
    /// <summary>
    /// Common animations 
    /// </summary>
    public static class DefaultAnimations
    {
        /// <summary>
        /// Scaling page animation
        /// </summary>
        /// <param name="canvasGroup">Root CanvasGroup</param>
        /// <param name="transform">Root Transform</param>
        /// <returns>Tracks group</returns>
        public static TracksEvaluator ScaleFadePageAnimation(CanvasGroup canvasGroup, Transform transform)
        {
            return new TracksEvaluator(new ITrack[]
            {
                new CanvasGroupOpacityTrack(canvasGroup, new []
                {
                    new KeyFrame<float>(0, 0, Easing.QuartOut), 
                    new KeyFrame<float>(150, 1), 
                }), 
                new ScaleTrack(transform, new []
                {
                    new KeyFrame<Vector3>(0, Vector3.one * 0.5f, Easing.QuartOut), 
                    new KeyFrame<Vector3>(250, Vector3.one), 
                }), 
            });
        }
        
        /// <summary>
        /// Scaling animation
        /// </summary>
        /// <param name="canvasGroup">Target CanvasGroup</param>
        /// <param name="transform">Target Transform</param>
        /// <returns>Show tracks, hide tracks</returns>
        public static (TracksEvaluator show, TracksEvaluator hide) ScaleFadeAnimation(CanvasGroup canvasGroup, Transform transform)
        {
            var show = new TracksEvaluator(new ITrack[]
            {
                new CanvasGroupOpacityTrack(canvasGroup, new []
                {
                    new KeyFrame<float>(0, 0, Easing.QuadOut), 
                    new KeyFrame<float>(300, 1), 
                }), 
                new ScaleTrack(transform, new []
                {
                    new KeyFrame<Vector3>(0, Vector3.one * 0.67f, Easing.QuintOut), 
                    new KeyFrame<Vector3>(300, Vector3.one, Easing.QuintOut), 
                }), 
            });
            var hide = new TracksEvaluator(new ITrack[]
            {
                new CanvasGroupOpacityTrack(canvasGroup, new []
                {
                    new KeyFrame<float>(0, 1, Easing.QuadOut), 
                    new KeyFrame<float>(200, 0), 
                }), 
                new ScaleTrack(transform, new []
                {
                    new KeyFrame<Vector3>(0, Vector3.one, Easing.QuintOut), 
                    new KeyFrame<Vector3>(300, Vector3.one * 0.87f), 
                }), 
            });
            return (show, hide);
        }

        /// <summary>
        /// Popup window animation
        /// </summary>
        /// <param name="root">Window parent CanvasGroup</param>
        /// <param name="window">Window Transform</param>
        /// <returns>Show tracks, hide tracks</returns>
        public static (TracksEvaluator show, TracksEvaluator hide) PopupWindowAnimation(CanvasGroup root, Transform window)
        {
            var show = new TracksEvaluator(new ITrack[]
            {
                new CanvasGroupOpacityTrack(root, new []
                {
                    new KeyFrame<float>(0, 0, Easing.QuintOut), 
                    new KeyFrame<float>(200, 1), 
                }), 
                new ScaleTrack(window, new []
                {
                    new KeyFrame<Vector3>(0, Vector3.one * 0.01f, Easing.OutBack), 
                    new KeyFrame<Vector3>(250, Vector3.one), 
                }), 
            });
            var hide = new TracksEvaluator(new ITrack[]
            {
                new CanvasGroupOpacityTrack(root, new []
                {
                    new KeyFrame<float>(0, 1, Easing.QuintOut), 
                    new KeyFrame<float>(300, 0), 
                }),
                new ScaleTrack(window, new []
                {
                    new KeyFrame<Vector3>(0, Vector3.one, Easing.QuintOut), 
                    new KeyFrame<Vector3>(250, Vector3.one * 0.01f), 
                }), 
            });
            return (show, hide);
        }
        
        /// <summary>
        /// Fade animation
        /// </summary>
        /// <param name="target">Target</param>
        /// <param name="mono">Owner</param>
        /// <param name="transition">Transition</param>
        /// <returns>Ready to use animation player</returns>
        public static StatedFluentAnimationPlayer<Visibility> Fade(this CanvasGroup target, MonoBehaviour mono, Transition transition = null)
        {
            var track = new FluentCanvasGroupOpacityTrack(target, transition ?? new Transition(300, Easing.QuartOut));
            var player = new StatedFluentAnimationPlayer<Visibility>(mono, track);
            player.StateChanged += (previous, current, options) =>
            {
                switch (current)
                {
                    case Visibility.Visible:
                        track.Set(1, options);
                        target.blocksRaycasts = true;
                        break;
                    
                    case Visibility.Hidden:
                        track.Set(0, options);
                        target.blocksRaycasts = false;
                        break;
                }
            };
            return player;
        }
    }

    /// <summary>
    /// Object visibility
    /// </summary>
    public enum Visibility
    {
        Visible,
        Hidden
    }
}