using System;
using AurumGames.CompositeRoot;
using AurumGames.SceneManagement;
using Poker.Gameplay.Configuration;
using Poker.Gameplay.Core;
using UnityEngine;

namespace Poker.Screens
{
    public class BootstrapScreen : MonoBehaviour
    {
        [SerializeField] private CardsDatabase _cardsDatabase;
        
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
            
            context.Register(_cardsDatabase);

            GameManager.FastCreateAndRegister(context);
        }
    }
}
