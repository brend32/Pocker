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
        private bool _hidden;

        /// <summary>
        /// Unload scene
        /// </summary>
        protected void Unload()
        {
            if (_hidden == false)
                throw new Exception("Can`t unload when page is not released. Call HidePage first");
            
            BeforeUnload();
            SceneManager.UnloadSceneAsync(gameObject.scene);
        }
        
        internal void FastHidePage()
        {
            _hidden = true;
            Hide?.Invoke();
            BeginHide(true);
            Unload();
        }

        /// <summary>
        /// Close page
        /// </summary>
        protected internal void HidePage()
        {
            _hidden = true;
            Hide?.Invoke();
            BeginHide(false);

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
        protected virtual void SetupDefaultAnimations(CanvasGroup canvasGroup, Transform transform)
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
        /// <param name="fastHide">Is fast hidding</param>
        protected virtual void BeginHide(bool fastHide) {}
        /// <summary>
        /// Before unload
        /// </summary>
        protected virtual void BeforeUnload() {}
        /// <summary>
        /// Blocked state changed
        /// </summary>
        protected virtual void BlockStateChanged() {}
    }
}