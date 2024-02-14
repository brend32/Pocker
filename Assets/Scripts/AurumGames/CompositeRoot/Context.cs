using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Tests")]

namespace AurumGames.CompositeRoot
{
    /// <summary>
    /// Dependencies container and resolver
    /// </summary>
    public class Context
    {
        /// <summary>
        /// Main context
        /// </summary>
        public static Context Global { get; private set; } = Create<Context>(null);

        public delegate void OnRegisterArgs(Type type, object instance);

        /// <summary>
        /// Fires when dependency is registered
        /// </summary>
        public event OnRegisterArgs OnRegister;
        
        private readonly Dictionary<Type, object> _dependencies = new();
        private readonly Dictionary<Type, FieldInfo[]> _fieldsCache = new();
        private readonly Dictionary<Type, MethodInfo[]> _methodsCache = new();
        private readonly Dictionary<Type, List<ILazyDependency>> _lazyWaiters = new();

        private readonly List<Context> _childScopes = new();

        private Context _parent;
        
        protected internal virtual void Init()
        {
            Register(this);
        }

        public T NewScope<T>() where T : Context, new()
        {
            var context = Create<T>(this);
            _childScopes.Add(context);
            return context;
        }
        
        /// <summary>
        /// Only create dependency
        /// </summary>
        /// <typeparam name="T">Class</typeparam>
        /// <returns>Class with resolved dependencies</returns>
        /// <exception cref="Exception">Constructor must have dependency attribute</exception>
        [Obsolete("Use fast create", true)]
        public T Create<T>()
        {
            foreach (ConstructorInfo constructor in typeof(T).GetConstructors())
            {
                if (constructor.GetCustomAttribute<DependencyAttribute>() is {})
                {
                    return (T)Activator.CreateInstance(typeof(T), CollectDependencies(constructor));
                }
            }

            throw new Exception($"Type should have {nameof(DependencyAttribute)} to be created");
        }

        
        /// <summary>
        /// Register dependency both by interface and class type
        /// </summary>
        /// <param name="value">Dependency</param>
        /// <typeparam name="T">Class</typeparam>
        /// <typeparam name="TInterface">Interface</typeparam>
        /// <exception cref="Exception">Can be registered once</exception>
        public virtual void RegisterWithInterface<T, TInterface>(T value) 
            where T : class, TInterface
            where TInterface : class
        {
            Register(value);
            Register<TInterface, T>(value);
        }

        /// <summary>
        /// Register dependency
        /// </summary>
        /// <param name="value">Dependency</param>
        /// <typeparam name="T">Class</typeparam>
        /// <exception cref="Exception">Can be registered once</exception>
        public virtual void Register<T>(T value) where T : class
        {
            Register<T, T>(value);
        }

        public void Free()
        {
            _parent?._childScopes.Remove(this);
            _dependencies?.Clear();
            _fieldsCache?.Clear();
            _methodsCache?.Clear();
            _lazyWaiters?.Clear();
        }

        /// <summary>
        /// Register multiple dependency by interface
        /// </summary>
        /// <param name="value">Dependency</param>
        /// <typeparam name="T">Interface</typeparam>
        public virtual void RegisterMultiple<T>(T value)
            where T : class, IMultipleRegistrable
        {
            Type key = typeof(IMultipleDependency<T>);
            _dependencies.TryAdd(key, new MultipleRegistrableList<T>());
            var list = (MultipleRegistrableList<T>)_dependencies[key];
            list.Add(value);

            OnRegister?.Invoke(typeof(T), value);
        }

        /// <summary>
        /// Register dependency by interface only
        /// </summary>
        /// <param name="value">Dependency</param>
        /// <typeparam name="TRegister">Interface</typeparam>
        /// <typeparam name="T">Class</typeparam>
        /// <exception cref="Exception">Can be registered once</exception>
        public virtual void Register<TRegister, T>(T value) 
            where T : class, TRegister
            where TRegister : class
        {
            Type type = typeof(TRegister);
            if (_dependencies.ContainsKey(type))
                throw new Exception($"Contains type {type.FullName}");
            
            _dependencies.Add(type, value);
            NotifyLazyWaiters(type, value);
            
            OnRegister?.Invoke(typeof(TRegister), value);
        }

        private void NotifyLazyWaiters(Type type, object value)
        {
            foreach (Context childScope in _childScopes)
            {
                if (childScope == null)
                {
                    throw new Exception("Child scope must be manually freed");
                }
                
                childScope.NotifyLazyWaiters(type, value);
            }
            
            if (_lazyWaiters.TryGetValue(type, out var waiters))
            {
                foreach (ILazyDependency waiter in waiters)
                {
                    waiter?.Resolve(value);
                }

                _lazyWaiters.Remove(type);
            }
        }

        /// <summary>
        /// Get dependency
        /// </summary>
        /// <typeparam name="T">Class</typeparam>
        /// <returns>Dependency</returns>
        /// <exception cref="KeyNotFoundException">No dependency found</exception>
        public virtual T Get<T>() where T : class
        {
            return Get<T, T>();
        }
        
        /// <summary>
        /// Get class if registered
        /// </summary>
        /// <typeparam name="T">Class</typeparam>
        /// <returns>Class or null</returns>
        public virtual T GetIfExists<T>() where T : class
        {
            return GetIfExists<T, T>();
        }
        
        /// <summary>
        /// Get dependency by interface
        /// </summary>
        /// <typeparam name="T">Class</typeparam>
        /// <typeparam name="TRegister">Interface</typeparam>
        /// <returns>Dependency</returns>
        /// <exception cref="KeyNotFoundException">No dependency found</exception>
        public virtual T Get<TRegister, T>() 
            where T : class, TRegister
            where TRegister : class
        {
            T value = GetIfExists<TRegister, T>();
            if (value == null)
                throw new Exception($"Can't resolve dependency {typeof(T).FullName} by key {typeof(TRegister).FullName}");
            
            return value;
        }
        
        /// <summary>
        /// Get class by interface if registered
        /// </summary>
        /// <typeparam name="T">Class</typeparam>
        /// <typeparam name="TRegister">Interface</typeparam>
        /// <returns>Class or null</returns>
        public virtual T GetIfExists<TRegister, T>() 
            where T : class, TRegister
            where TRegister : class
        {
            if (TryGetDependency(typeof(TRegister), out var value))
                return (T)value;

            return null;
        }

        /// <summary>
        /// Resolve object dependencies
        /// </summary>
        /// <param name="target">Target</param>
        public virtual void Resolve(object target)
        {
            if (target is IFastResolvable fastResolvable)
            {
                fastResolvable.FastResolve(this);
                return;
            }
            
            Debug.LogWarning($"Non fast path disabled: {target.GetType()}");
            return;
            ResolveFields(target);
            ResolveMethods(target);
        }
        
        public void LazyResolveDependency(ILazyDependency lazyDependency, Type dependencyType)
        {
            if (lazyDependency == null)
                return;
            
            if (TryGetDependency(dependencyType, out var value))
            {
                lazyDependency.Resolve(value);
                return;
            }

            _lazyWaiters.TryAdd(dependencyType, new List<ILazyDependency>());
            var list = _lazyWaiters[dependencyType];
            list.Add(lazyDependency);
        }

        protected virtual bool TryGetDependency(Type target, out object value)
        {
            return _dependencies.TryGetValue(target, out value) || 
                   _parent?.TryGetDependency(target, out value) == true;
        }

        private void ResolveFields(object target)
        {
            Type type = target.GetType();
            var fields = GetResolveFields(type);

            foreach (FieldInfo field in fields)
            {
                if (IsLazyDependency(field.FieldType, out Type dependencyType))
                {
                    ResolveLazyDependency(field, target, dependencyType);
                    continue;
                }

                if (TryGetDependency(field.FieldType, out var value))
                {
                    field.SetValue(target, value);
                }
                else if (IsOptionalDependency(field.GetCustomAttribute<DependencyAttribute>()) == false)
                {
                    throw new Exception($"Can't resolve non optional lazy dependency {field.FieldType.FullName}");
                }
            }
        }

        private void ResolveLazyDependency(FieldInfo field, object target, Type dependencyType)
        {
            var lazyDependency = (ILazyDependency) field.GetValue(target);
            if (lazyDependency == null)
                return;
            
            if (TryGetDependency(dependencyType, out var value))
            {
                lazyDependency.Resolve(value);
                return;
            }

            _lazyWaiters.TryAdd(dependencyType, new List<ILazyDependency>());
            var list = _lazyWaiters[dependencyType];
            list.Add(lazyDependency);
        }

        private void ResolveMethods(object target)
        {
            Type type = target.GetType();
            var methods = GetResolveMethods(type);
            
            foreach (MethodInfo method in methods)
            {
                method.Invoke(target, CollectDependencies(method));
            }
        }

        private object[] CollectDependencies(MethodBase method)
        {
            var args = method.GetParameters();
            var dependencies = new object[args.Length];
            for (var i = 0; i < args.Length; i++)
            {
                ParameterInfo parameterInfo = args[i];
                if (TryGetDependency(parameterInfo.ParameterType, out var value))
                {
                    dependencies[i] = value;
                }
                else if (parameterInfo.HasDefaultValue)
                {
                    dependencies[i] = parameterInfo.DefaultValue;
                }
                else
                {
                    throw new Exception($"Can't resolve dependency {parameterInfo.ParameterType.FullName}");
                }
            }

            return dependencies;
        }
        
        private IEnumerable<FieldInfo> GetResolveFields(Type target)
        {
            return GetFields(target);
        }

        private bool TryGetFieldsCache(Type target, out FieldInfo[] fields)
        {
            return _fieldsCache.TryGetValue(target, out fields) || 
                   _parent?.TryGetFieldsCache(target, out fields) == true;
        }
        
        private IEnumerable<FieldInfo> GetFields(Type target)
        {
            if (TryGetFieldsCache(target, out var fields))
            {
                fields = target.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Where(info => info.GetCustomAttribute<DependencyAttribute>() is {}).ToArray();
                _fieldsCache.Add(target, fields);
            }
            else
            {
                fields = Array.Empty<FieldInfo>();
            }
            
            return target.BaseType is {} ? GetFields(target.BaseType).Concat(fields) : fields;
        }
        
        private bool TryGetMethodsCache(Type target, out MethodInfo[] methods)
        {
            return _methodsCache.TryGetValue(target, out methods) || 
                   _parent?.TryGetMethodsCache(target, out methods) == true;
        }

        private IEnumerable<MethodInfo> GetResolveMethods(Type target)
        {
            if (TryGetMethodsCache(target, out var methods))
            {
                methods = GetMethods(target).OrderBy(info => info.GetCustomAttribute<DependencyAttribute>().Priority).ToArray();
                _methodsCache[target] = methods;
            }
            else
            {
                methods = Array.Empty<MethodInfo>();
            }

            return methods;
        }

        private IEnumerable<MethodInfo> GetMethods(Type target)
        {
            if (TryGetMethodsCache(target, out var methods))
            {
                methods = target.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(info => info.GetCustomAttribute<DependencyAttribute>() is {}).ToArray();
                _methodsCache.Add(target, methods);
            }
            else
            {
                methods = Array.Empty<MethodInfo>();
            }

            return target.BaseType is {} ? GetMethods(target.BaseType).Concat(methods) : methods;
        }

        private bool IsLazyDependency(Type type, out Type dependencyType)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(LazyDependency<>))
            {
                dependencyType = type.GenericTypeArguments[0];
                return true;
            }

            dependencyType = default;
            return false;
        }
        
        private bool IsOptionalDependency(DependencyAttribute attribute)
        {
            return attribute.Required == false;
        }

        private static T Create<T>(Context parent) where T : Context, new()
        {
            var context = new T
            {
                _parent = parent
            };
            context.Init();
            return context;
        } 
    }
}