using System.Collections.Generic;
using AurumGames.Animation;
using AurumGames.Animation.Tracks;
using AurumGames.CompositeRoot;
using AurumGames.SceneManagement;
using Poker.Gameplay.Core;
using Poker.Menu.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Poker.Screens
{
    [SceneInitScript("Loading")]
    public partial class LoadingScreen : LoadingPage
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _root;
        
        protected override void BeforeInit()
        {
            Context.Global.Resolve(this);
            CustomInitializer.StartInitialization(gameObject.scene);
            
            SetupDefaultAnimations(_canvasGroup, _root);
        }
    }
}
