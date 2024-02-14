using System;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace AurumGames.CompositeRoot
{
	[DefaultExecutionOrder(-99)]
	public abstract class ContextProvider : MonoBehaviour
	{
		public Context CurrentContext { get; private set; }
		
		[SerializeField] private LazyMonoBehaviour[] _objects;

		private void Awake()
		{
			CurrentContext = CreateContext();
			foreach (LazyMonoBehaviour mono in _objects)
			{
				mono.CurrentContext = CurrentContext;
			}
		}

		private void OnDestroy()
		{
			CurrentContext.Free();
		}

		protected abstract Context CreateContext();

#if UNITY_EDITOR
		[EasyButtons.Button]
		protected void UpdateList()
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