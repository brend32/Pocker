using System;
using AurumGames.CompositeRoot;
using AurumGames.SceneManagement;
using Poker.Gameplay.Core;
using UnityEngine;

namespace Poker.Screens
{
    public class BootstrapScreen : MonoBehaviour
    {
        private PageSystem _pageSystem;
        
        private void Awake()
        {
            Application.targetFrameRate = 60;
            
            RegisterDependencies();
            
            _pageSystem.Load<MenuScreen>();
        }

        private void RegisterDependencies()
        {
            Context context = Context.Global;

            _pageSystem = PageSystem.FastCreateAndRegister(context);
            WindowSystem.FastCreateAndRegister(context);

            GameManager.FastCreateAndRegister(context);
        }
    }
}
