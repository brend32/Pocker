using System;
using UnityEngine;

namespace AurumGames.SceneManagement
{
    /// <summary>
    /// Called when page loaded through page system
    /// </summary>
    public abstract class SceneInitScript : MonoBehaviour
    {
        /// <summary>
        /// Reference to page system
        /// </summary>
        protected PageSystem PageSystem { get; private set; }
        /// <summary>
        /// Is page initialized
        /// </summary>
        protected bool IsInit { get; private set; }
        
        private void Awake()
        {
            BeforeInit();
        }

        private void Start()
        {
            SceneLoader.Loaded(this, gameObject.scene);
            Init();
            IsInit = true;
        }

        internal void PassSystem(PageSystem system)
        {
            PageSystem = system;
        }

        /// <summary>
        /// Page initialization. Called after callback fired 
        /// </summary>
        protected virtual void Init() {}
        /// <summary>
        /// Page initialization. Called before callback fired 
        /// </summary>
        protected virtual void BeforeInit() {}
    }
}