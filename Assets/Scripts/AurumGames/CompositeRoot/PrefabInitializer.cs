using System;
using UnityEngine;

namespace AurumGames.CompositeRoot
{
    [RequireComponent(typeof(InitializationGroupSetter))]
    public class PrefabInitializer : MonoBehaviour
    {
        internal Context InitializationContext;
        
        private void Awake()
        {
            InitializationContext ??= Context.Global;
            GetComponent<InitializationGroupSetter>().StartInitialization(InitializationContext);
        }
    }
}