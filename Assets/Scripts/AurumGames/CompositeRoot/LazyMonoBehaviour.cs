using System;
using UnityEngine;

namespace AurumGames.CompositeRoot
{
    /// <summary>
    /// Custom initialization queue mono behaviour
    /// </summary>
    [RequireComponent(typeof(LazyMonoEarlyExecution))]
    public abstract class LazyMonoBehaviour : MonoBehaviour
    {
        protected bool IsInit { get; private set; }
        protected internal Context CurrentContext { get; internal set; }
        
        private event Action InitFinish;
        private bool _initStarted;
        private bool _shouldCallOnEnable;
        private bool _calledDestroy;
        internal CustomInitializationGroup _initializationGroup;

        internal void InternalAwake()
        {
            if (_initStarted)
                return;

            CurrentContext ??= Context.Global;
            _initStarted = true;
            InitInnerState();
            if (_initializationGroup != null)
            {
                _initializationGroup.Wait(this);
            }
            else
            {
                CustomInitializer.Wait(this);
            }
        }

        /// <summary>
        /// Instantly finish initialization <br/>
        /// Note: Call it if CustomInitializer.StartInitialization was already called 
        /// </summary>
        public void InstantInit(Context context = null)
        {
            if (_initStarted)
                return;
            
            _initStarted = true;
            InitInnerState();
            if (this is IFastResolvable)
            {
                context ??= CurrentContext;
                context ??= Context.Global;
                context.Resolve(this);
            }
            AfterResolve();
            ContinueInit();
        }

        internal void AddInitCallback(Action callback)
        {
            if (IsInit)
                callback?.Invoke();
            else
                InitFinish += callback;
        }
        
        internal void ContinueInit()
        {
            if (IsInit)
                return;
            
            IsInit = true;
            
            if (enabled && _shouldCallOnEnable)
                OnEnable();
            Initialized();
            
            InitFinish?.Invoke();
            InitFinish = null;
        }

        protected void OnEnable()
        {
            if (IsInit)
                Enabled();
            else
                _shouldCallOnEnable = true;
        }

        protected void OnDisable()
        {
            if (IsInit)
                Disabled();
        }

        protected internal void OnDestroy()
        {
            if (_calledDestroy)
                return;

            _calledDestroy = true;
            Destroyed();
        }

        /// <summary>
        /// Mono enabled
        /// </summary>
        protected virtual void Enabled(){}
        /// <summary>
        /// Mono disabled
        /// </summary>
        protected virtual void Disabled(){}
        /// <summary>
        /// Mono destroyed
        /// </summary>
        protected virtual void Destroyed(){}
        /// <summary>
        /// Initialize init state <br/>
        /// Note: You can register dependencies during this stage <br/>
        /// CAUTION: Don't access other LazyMonoBehaviour-s or dependencies
        /// </summary>
        protected abstract void InitInnerState();
        /// <summary>
        /// After resolve <br/>
        /// Note: You can access dependencies during this stage <br/>
        /// CAUTION: Don't access other LazyMonoBehaviour-s or register dependencies
        /// </summary>
        protected internal virtual void AfterResolve(){}
        /// <summary>
        /// Finish initialization
        /// Note: You can access other LazyMonoBehaviour-s or dependencies <br/>
        /// CAUTION: Don't register dependencies on this stage
        /// </summary>
        protected abstract void Initialized();

        public static T InstantiateAndResolve<T>(T prefab, Context context) where T : LazyMonoBehaviour
        {
            var prefabActive = prefab.gameObject.activeSelf;
            prefab.gameObject.SetActive(false);
            T obj = Instantiate(prefab);
            prefab.gameObject.SetActive(prefabActive);
            return SetupCreatedInstance(obj, context);
        }
        
        public static T InstantiateAndResolve<T>(T prefab, Context context, Transform parent) where T : LazyMonoBehaviour
        {
            var prefabActive = prefab.gameObject.activeSelf;
            prefab.gameObject.SetActive(false);
            T obj = Instantiate(prefab, parent);
            prefab.gameObject.SetActive(prefabActive);
            return SetupCreatedInstance(obj, context);
        }

        public static void DestroySafe(GameObject gameObject)
        {
            var components = gameObject.GetComponentsInChildren<LazyMonoBehaviour>(true);
            foreach (LazyMonoBehaviour lazyMonoBehaviour in components)
            {
                lazyMonoBehaviour.OnDestroy();
            }
            
            Destroy(gameObject);
        }

        private static T SetupCreatedInstance<T>(T obj, Context context) where T : LazyMonoBehaviour
        {
            if (obj.TryGetComponent(out PrefabInitializer prefabInitializer))
            {
                prefabInitializer.InitializationContext = context;
            }

            obj.gameObject.SetActive(true);
            return obj;
        }
    }
}