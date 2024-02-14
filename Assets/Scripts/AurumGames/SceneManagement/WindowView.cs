using System;
using System.Collections.Generic;
using AurumGames.Animation;
using AurumGames.CompositeRoot;
using UnityEngine;

namespace AurumGames.SceneManagement
{
    /// <summary>
    /// Window base class
    /// </summary>
    public abstract partial class WindowView : LazyMonoBehaviour
    {
        /// <summary>
        /// Is visible
        /// </summary>
        public bool Visible { get; private set; }
        /// <summary>
        /// Is interactable
        /// </summary>
        public bool Interactable { get; private set; }
        /// <summary>
        /// Is suspended
        /// </summary>
        public bool Suspended { get; private set; }
        
        [Dependency] private WindowSystem _windowSystem;
        private bool _alreadyAnswered;

        /// <summary>
        /// Prevent window showing unexpectedly
        /// </summary>
        public void Suspend()
        {
            if (Suspended)
                return;
            
            Suspended = true;
            
            if (Visible == false)
                return;
            
            Hide();
            HideAnimationEnded();
        }
        
        /// <summary>
        /// Bring window to normal state
        /// </summary>
        public void Unsuspend()
        {
            Suspended = false;
        }
        
        /// <summary>
        /// Show window
        /// </summary>
        protected void Show()
        {
            if (Suspended)
                return;
            
            _windowSystem.Show(this);
        }

        /// <summary>
        /// Hide window
        /// </summary>
        protected void Hide()
        {
            _windowSystem.Hide(this);
        }

        internal void ShowInternal()
        {
            var fromBlockedState = Visible == true;
            
            Visible = true;
            Interactable = true;
            ResetMultipleAnswerProtection();
            gameObject.SetActive(true);
            if (this != null)
                BeginShow(fromBlockedState);
        }
        
        internal void HideInternal()
        {
            Visible = false;
            Interactable = false;
            if (this != null)
                BeginHide();
        }
        
        internal void BlockInternal()
        {
            Interactable = false;
            if (this != null)
                BeginBlocked();
        }

        protected override void Destroyed()
        {
            if (Visible)
                Hide();
        }

        /// <summary>
        /// Reset answer protection
        /// </summary>
        protected void ResetMultipleAnswerProtection()
        {
            _alreadyAnswered = false;
        }

        /// <summary>
        /// Notify window that hide animation ended
        /// </summary>
        protected void HideAnimationEnded()
        {
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Check back input signal
        /// </summary>
        /// <returns>True if back pressed</returns>
        protected bool IsBackPressed()
        {
            return enabled && Suspended == false && Interactable && Visible 
                   && _alreadyAnswered == false && Input.GetKeyDown(KeyCode.Escape);
        }

        /// <summary>
        /// Multiple button triggering protection
        /// </summary>
        /// <param name="action">Action to execute</param>
        protected void SafeClick(Action action)
        {
            if (_alreadyAnswered)
                return;

            _alreadyAnswered = true;
            action?.Invoke();
        }

        /// <summary>
        /// Become visible
        /// </summary>
        protected abstract void BeginShow(bool fromBlockedState);
        /// <summary>
        /// Become hidden
        /// </summary>
        protected abstract void BeginHide();
        /// <summary>
        /// Become blocked
        /// </summary>
        protected abstract void BeginBlocked();

        /// <summary>
        /// Default windows animations
        /// </summary>
        /// <param name="back">Window root canvasGroup</param>
        /// <param name="window">Window transform</param>
        /// <returns>Animation player</returns>
        protected StatedAnimationPlayer<Visibility> DefaultAnimationPlayer(CanvasGroup back, Transform window)
        {
            (TracksEvaluator show, TracksEvaluator hide) = DefaultAnimations.PopupWindowAnimation(back, window);
            
            var player = new StatedAnimationPlayer<Visibility>(this, new Dictionary<Visibility, TracksEvaluator>()
            {
                { Visibility.Visible, show },
                { Visibility.Hidden, hide }
            });
            player.StateChanged += (previous, current) =>
            {
                back.blocksRaycasts = current switch
                {
                    Visibility.Visible => true,
                    Visibility.Hidden => false,
                    _ => back.blocksRaycasts
                };
            };
            player.AnimationEnded += (previous, current) =>
            {
                switch (current)
                {
                    case Visibility.Hidden:
                        HideAnimationEnded();
                        break;
                }
            };
            
            return player;
        }
    }
}