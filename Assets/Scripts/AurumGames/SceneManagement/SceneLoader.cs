using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AurumGames.SceneManagement
{
    internal static class SceneLoader
    {
        private static readonly Dictionary<string, Action<SceneInitScript>> Waiting = new();

        public static void Load<T>(MonoBehaviour mono, Action<T> loaded, Action<AsyncOperation> operationCallback = null) where T : SceneInitScript
        {
            Load(typeof(T), mono, loaded, operationCallback);
        }
        
        public static void Load<T>(Type type, MonoBehaviour mono, Action<T> loaded, Action<AsyncOperation> operationCallback = null) where T : SceneInitScript
        {
            SceneInitScriptAttribute attribute = GetCustomAttribute(type);
            if (Waiting.ContainsKey(attribute.SceneName))
            {
                Waiting[attribute.SceneName] += (script) =>
                {
                    loaded.Invoke((T)script);
                };
                return;
            }

            Waiting.Add(attribute.SceneName, (script) =>
            {
                loaded.Invoke((T)script);
            });
            mono.StartCoroutine(LoadCoroutine(attribute.SceneName, operationCallback));
        }

        public static void MakeActive(SceneInitScript initScript)
        {
            SceneInitScriptAttribute attribute = GetCustomAttribute(initScript.GetType());
            Scene scene = SceneManager.GetSceneByName(attribute.SceneName);
            if (scene.isLoaded)
                SceneManager.SetActiveScene(scene);
        }
        
        private static SceneInitScriptAttribute GetCustomAttribute(Type type)
        {
            return type.GetCustomAttribute<SceneInitScriptAttribute>();
        }

        public static void Loaded<T>(T script, Scene scene) where T : SceneInitScript
        {
            Waiting[scene.name].Invoke(script);
            Waiting.Remove(scene.name);
        }

        private static IEnumerator LoadCoroutine(string sceneName, Action<AsyncOperation> callback)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            callback?.Invoke(operation);
            yield return operation;
        }
    } 
}