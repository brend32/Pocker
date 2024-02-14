using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace AurumGames.CompositeRoot
{
    /// <summary>
    /// Custom initialization queue
    /// </summary>
    public static class CustomInitializer
    {
        private static readonly Dictionary<Scene, CustomInitializationGroup> InitWaiters = new();

        internal static void Wait(LazyMonoBehaviour mono)
        {
            Scene scene = mono.gameObject.scene;
            InitWaiters.TryAdd(scene, new CustomInitializationGroup());
            
            InitWaiters[scene].Wait(mono);
        }

        /// <summary>
        /// Start initialization
        /// Note: Must be call after all InitInnerState methods
        /// </summary>
        /// <param name="scene">Scene</param>
        /// <param name="context">Context</param>
        public static void StartInitialization(Scene scene, Context context = null)
        {
            if (InitWaiters.ContainsKey(scene) == false)
                return;

            CustomInitializationGroup list = InitWaiters[scene];
            InitWaiters.Remove(scene);
            list.StartInitialization(context);
        }

        /// <summary>
        /// Waits for init to finish
        /// </summary>
        /// <param name="mono">Target</param>
        /// <param name="callback">Finish callback</param>
        public static void WaitInitFinish(this LazyMonoBehaviour mono, Action callback)
        {
            mono.AddInitCallback(callback);
        } 
    }
}