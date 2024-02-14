using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

namespace AurumGames.CompositeRoot
{
    internal class InitializationGroupSetter : MonoBehaviour
    {
        [SerializeField] private LazyMonoBehaviour[] _objects;

        private CustomInitializationGroup _initializationGroup;

        private void Awake()
        {
            _initializationGroup = new CustomInitializationGroup();
            foreach (LazyMonoBehaviour lazyMonoBehaviour in _objects)
            {
                lazyMonoBehaviour._initializationGroup = _initializationGroup;
            }
        }

        public void StartInitialization(Context context)
        {
            _initializationGroup.StartInitialization(context);
        }
        
#if UNITY_EDITOR
        [EasyButtons.Button]
        private void UpdateList()
        {
            var foundObjects = StageUtility.GetStageHandle(gameObject)
                .FindComponentsOfType<LazyMonoBehaviour>()?
                .Where(o =>
            {
                GameObject obj = o.gameObject;
                return obj != null;
            }).ToArray();

            _objects = foundObjects;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}