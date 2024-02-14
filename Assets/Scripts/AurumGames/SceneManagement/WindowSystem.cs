using System;
using System.Collections.Generic;
using System.Linq;
using AurumGames.CompositeRoot;
using UnityEngine;

namespace AurumGames.SceneManagement
{
    /// <summary>
    /// System to display fullscreen windows
    /// </summary>
    public sealed partial class WindowSystem
    {
        /// <summary>
        /// Currently active window
        /// </summary>
        public WindowView Current { get; private set; }

        public event Action<WindowView> NewWindowShown; 
        
        private readonly List<WindowView> _windows = new();
        private readonly PageSystem _pageSystem;

        [Dependency]
        public WindowSystem(PageSystem pageSystem)
        {
            _pageSystem = pageSystem;
        }

        internal void Show(WindowView window)
        {
            _windows.Add(window);
            MakeActive(window);
            _pageSystem.SetActiveWindow(window);
        }

        internal void Hide(WindowView window)
        {
            if (_windows.Remove(window) == false) 
                return;
            
            window.HideInternal();
            if (Current == window)
                Current = null;
            
            _windows.RemoveAll(w => w == null);
            if (_windows.Count > 0)
            {
                MakeActive(_windows.Last());
            }
            else
            {
                _pageSystem.RemoveActiveWindow();
                Current = null;
                NewWindowShown?.Invoke(null);
            }
        }

        private void MakeActive(WindowView window)
        {
            if (Current != null)
            {
                Current.BlockInternal();
            }

            Current = window;
            window.ShowInternal();
            NewWindowShown?.Invoke(window);
        }
    }
}