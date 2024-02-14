using System;
using AurumGames.Animation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AurumGames.SceneManagement
{
    /// <summary>
    /// Page base class
    /// </summary>
    public abstract class PageScript : SceneInitScript
    {
        /// <summary>
        /// Called when page closes
        /// </summary>
        public event Action Hide;
        
        /// <summary>
        /// Is page interactable
        /// </summary>
        public bool IsActive
        {
            get => _pageActive;
            set
            {
                if (_pageActive == value)
                    return;
                
                _pageActive = value;
                if (value)
                {
                    Activated();
                }
                else
                {
                    Deactivated();
                }

                IsBlocked = PageSystem.ActiveWindow != null;
            }
        }

        /// <summary>
        /// Is page blocked by window
        /// </summary>
        public bool IsBlocked
        {
            get => _pageBlocked;
            set
            {
                if (_pageBlocked == value)
                    return;
                
                _pageBlocked = value;
                BlockStateChanged();
            }
        }

        protected AnimationPlayer _hidePlayer;
        
        private bool _pageActive;
        private bool _pageBlocked;

        /// <summary>
        /// Unload scene
        /// </summary>
        protected void Unload()
        {
            SceneManager.UnloadSceneAsync(gameObject.scene);
        }

        /// <summary>
        /// Close page
        /// </summary>
        protected void HidePage()
        {
            Hide?.Invoke();
            BeginHide();

            if (_hidePlayer == null)
            {
                Unload();
                return;
            }

            _hidePlayer.Ended += Unload;
            _hidePlayer.Play();
        }

        /// <summary>
        /// Check back input signal
        /// </summary>
        /// <returns>True if back input pressed</returns>
        protected bool IsBackPressed()
        {
            return IsActive && IsBlocked == false && Input.GetKeyDown(KeyCode.Escape);
        }

        /// <summary>
        /// Default page animation
        /// </summary>
        /// <param name="canvasGroup">Root canvasGroup</param>
        /// <param name="transform">Root transform</param>
        protected void SetupDefaultAnimations(CanvasGroup canvasGroup, Transform transform)
        {
            (TracksEvaluator show, TracksEvaluator hide) = DefaultAnimations.ScaleFadeAnimation(canvasGroup, transform);
            new AnimationPlayer(this, show).Play();
            _hidePlayer = new AnimationPlayer(this, hide);
            _hidePlayer.Started += () =>
            {
                if (canvasGroup != null)
                    canvasGroup.blocksRaycasts = false;
            };
        }

        /// <summary>
        /// Page become active
        /// </summary>
        protected virtual void Activated() {}
        /// <summary>
        /// Page become inactive
        /// </summary>
        protected virtual void Deactivated() {}
        /// <summary>
        /// Page hides
        /// </summary>
        protected virtual void BeginHide() {}
        /// <summary>
        /// Blocked state changed
        /// </summary>
        protected virtual void BlockStateChanged() {}
    }
}