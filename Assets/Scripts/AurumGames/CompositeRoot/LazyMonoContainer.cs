using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace AurumGames.CompositeRoot
{
    /// <summary>
    /// Initialize inactive objects 
    /// </summary>
    internal sealed class LazyMonoContainer : MonoBehaviour
    {
        [SerializeField] private LazyMonoEarlyExecution[] _objects;

        private void Awake()
        {
            foreach (LazyMonoEarlyExecution mono in _objects)
            {
                if (mono != null)
                    mono.Awake();
            }
        }

        private void OnDestroy()
        {
            foreach (LazyMonoEarlyExecution earlyExecution in _objects)
            {
                if (earlyExecution._shouldManuallyCallDestroy)
                    earlyExecution.Destroy();
            }
        }

#if UNITY_EDITOR
        [EasyButtons.Button]
        private void UpdateList()
        {
            var foundObjects = StageUtility.GetStageHandle(gameObject)
                .FindComponentsOfType<LazyMonoEarlyExecution>()?
                .Where(o =>
            {
                GameObject obj = o.gameObject;
                return obj != null && obj.activeInHierarchy == false;
            }).ToArray();

            _objects = foundObjects;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}