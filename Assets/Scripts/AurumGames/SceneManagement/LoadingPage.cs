using System;
using AurumGames.Animation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AurumGames.SceneManagement
{
    /// <summary>
    /// Loading page base class
    /// </summary>
    public abstract class LoadingPage : PageScript
    {
        /// <summary>
        /// Called when page closes
        /// </summary>
        public event Action FullyVisible;

        [SerializeField] private Camera _camera;
        
        protected void BecomeFullyVisible()
        {
            FullyVisible?.Invoke();
        }

        internal void DestroyCamera()
        {
            Destroy(_camera.gameObject);
        }
        
        /// <summary>
        /// Default page animation
        /// </summary>
        /// <param name="canvasGroup">Root canvasGroup</param>
        /// <param name="transform">Root transform</param>
        protected override void SetupDefaultAnimations(CanvasGroup canvasGroup, Transform transform)
        {
            (TracksEvaluator show, TracksEvaluator hide) = DefaultAnimations.ScaleFadeAnimation(canvasGroup, transform);
            var showPlayer = new AnimationPlayer(this, show);
            showPlayer.Ended += BecomeFullyVisible;
            showPlayer.Play();
            
            _hidePlayer = new AnimationPlayer(this, new TracksEvaluator(150, hide));
            _hidePlayer.Started += () =>
            {
                if (canvasGroup != null)
                    canvasGroup.blocksRaycasts = false;
            };
        }
    }
}