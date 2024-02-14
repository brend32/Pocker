using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Tests")]

namespace AurumGames.CompositeRoot
{
    [DefaultExecutionOrder(-54)]
    public sealed class AlwaysAliveMono : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Context.Global.Register(this);
            Context.Global.Register<MonoBehaviour>(this);
        }
    }
}