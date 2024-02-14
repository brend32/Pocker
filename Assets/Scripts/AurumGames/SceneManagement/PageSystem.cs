using System;
using System.Collections.Generic;
using System.Linq;
using AurumGames.CompositeRoot;
using UnityEngine;

namespace AurumGames.SceneManagement
{
    /// <summary>
    /// System to load and manage pages
    /// </summary>
    public sealed partial class PageSystem 
        //TODO: Merge with window system to address locality problem and support BackToPreviousScreen feature
    {
        /// <summary>
        /// Changed active page
        /// </summary>
        public event Action<PageScript> NewActivatedPage; 
        /// <summary>
        /// Currently active page
        /// </summary>
        public PageScript Active { get; private set; }
        /// <summary>
        /// Currently active page
        /// </summary>
        public WindowView ActiveWindow { get; private set; }
        /// <summary>
        /// Always alive mono
        /// </summary>
        public MonoBehaviour Mono => _defaultMono;

        private readonly MonoBehaviour _defaultMono;
        private readonly List<PageScript> _loaded = new();

        [Dependency]
        public PageSystem(MonoBehaviour defaultMono)
        {
            _defaultMono = defaultMono;
        }

        /// <summary>
        /// Try get loaded page
        /// </summary>
        /// <param name="page">Page</param>
        /// <typeparam name="T">Page type</typeparam>
        /// <returns>True if found page</returns>
        public bool TryGet<T>(out T page) where  T : PageScript
        {
            foreach (PageScript pageScript in _loaded)
            {
                if (pageScript is T script)
                {
                    page = script;
                    return true;
                }
            }

            page = default;
            return false;
        }

        /// <summary>
        /// Load page
        /// </summary>
        /// <param name="pageType">Page type</param>
        /// <param name="loaded">Loaded callback</param>
        /// <param name="operationCallback">Callback to get access to loading process</param>
        /// <typeparam name="T">Page type</typeparam>
        public void Load<T>(Type pageType, Action<T> loaded = null, Action<AsyncOperation> operationCallback = null) where T : PageScript
        {
            if (_loaded.LastOrDefault()?.GetType() == pageType)
                return;
            
            if (Active != null)
            {
                Active.IsActive = false;
                Active = null;
            }
            SceneLoader.Load<T>(pageType, _defaultMono, (page) =>
            {
                void Hide()
                {
                    Remove(page);
                    page.Hide -= Hide;
                }

                page.Hide += Hide;
                page.PassSystem(this);
                Push(page);
                loaded?.Invoke(page);
            }, operationCallback);
        }
        
        /// <summary>
        /// Load page
        /// </summary>
        /// <param name="loaded">Loaded callback</param>
        /// <param name="operationCallback">Callback to get access to loading process</param>
        /// <typeparam name="T">Page type</typeparam>
        public void Load<T>(Action<T> loaded = null, Action<AsyncOperation> operationCallback = null) where T : PageScript
        {
            Load(typeof(T), loaded, operationCallback);
        }

        internal void SetActiveWindow(WindowView windowView)
        {
            ActiveWindow = windowView;
            if (Active != null)
                Active.IsBlocked = true;
        }

        internal void RemoveActiveWindow()
        {
            ActiveWindow = null;
            if (Active != null)
                Active.IsBlocked = false;
        }
        
        private void Push(PageScript script)
        {
            _loaded.Add(script);
            MakeActive(script);
        }

        private void Remove(PageScript script)
        {
            if (_loaded.Remove(script) == false) 
                return;
            
            if (_loaded.Count > 0)
                MakeActive(_loaded.Last());
        }

        private void MakeActive(PageScript script)
        {
            if (Active != null && Active != script)
            {
                Active.IsActive = false;
            }
            
            SceneLoader.MakeActive(script);
            Active = script;
            script.IsActive = true;
            
            NewActivatedPage?.Invoke(script);
        }
    }
}