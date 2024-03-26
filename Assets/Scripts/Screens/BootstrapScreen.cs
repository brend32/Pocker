using System;
using AurumGames.Animation;
using AurumGames.Animation.Tracks;
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
        [SerializeField] private CanvasGroup _brendLogo;
        [SerializeField] private CanvasGroup _pokerLogo;
        [SerializeField] private Camera _camera;
        [SerializeField] private Canvas _canvas;
        
        private PageSystem _pageSystem;
        
        private void Awake()
        {
            Application.targetFrameRate = 60;
            
            RegisterDependencies();

            MenuScreen menuScreen = null;

            var animationPlayer = new AnimationPlayer(this, new ITrack[]
            {
                new CanvasGroupOpacityTrack(_brendLogo, new[]
                {
                    new KeyFrame<float>(3000, 1, Easing.QuadOut),
                    new KeyFrame<float>(3800, 0, Easing.QuadOut),
                }),
                new CanvasGroupOpacityTrack(_pokerLogo, new[]
                {
                    new KeyFrame<float>(3000, 0, Easing.QuadOut),
                    new KeyFrame<float>(3400, 1, Easing.QuadOut),
                    new KeyFrame<float>(5400, 1, Easing.QuadOut),
                    new KeyFrame<float>(6000, 0, Easing.QuadOut),
                }),
                new TriggerTrack(new[]
                {
                    new TriggerKeyFrame(3800, () =>
                    {
                        _pageSystem.Load<MenuScreen>(menu =>
                        {
                            Destroy(_camera.gameObject);
                            menuScreen = menu;
                        });
                    }),
                    new TriggerKeyFrame(5400, () =>
                    {
                        menuScreen!.PlayAnimation();
                    })
                })
            });
            animationPlayer.Ended += () =>
            {
                Destroy(_canvas.gameObject);
            };
            animationPlayer.Play();
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
