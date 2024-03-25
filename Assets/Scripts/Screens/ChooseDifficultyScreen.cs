using System.Collections.Generic;
using AurumGames.Animation;
using AurumGames.Animation.Tracks;
using AurumGames.CompositeRoot;
using AurumGames.SceneManagement;
using Poker.Gameplay.Core;
using Poker.Menu.UI;
using Poker.UI.ChooseDifficulty;
using UnityEngine;
using UnityEngine.UI;
using ToggleGroup = Poker.UI.Common.ToggleGroup;

namespace Poker.Screens
{
    [SceneInitScript("ChooseDifficulty")]
    public partial class ChooseDifficultyScreen : PageScript
    {
        [SerializeField] private MenuCardButton[] _buttons;
        [SerializeField] private DifficultyCardButton[] _difficultyButtons;
        [SerializeField] private CanvasGroup _title;
        [SerializeField] private CanvasGroup _additional;
        [SerializeField] private GraphicRaycaster _graphicRaycaster;
        [SerializeField] private ToggleGroup _difficulty;
        [SerializeField] private ToggleGroup _playersCount;
        [SerializeField] private StartingCash _startingCash;
        
        [Dependency] private GameManager _gameManager;

        private StatedAnimationPlayer<Visibility> _animation;
        
        protected override void BeforeInit()
        {
            SetupAnimation();
            _animation.SetStateInstant(Visibility.Hidden);
            
            Context.Global.Resolve(this);
            CustomInitializer.StartInitialization(gameObject.scene);
        }
        
        private void SetupAnimation()
        {
            var from = new Vector2(0, -500);
            
            var cardsShow = new ITrack[_buttons.Length];
            for (int i = 0; i < _buttons.Length; i++)
            {
                cardsShow[i] = new AnchoredPositionTrack(_buttons[i].Transform, new []
                {
                    new KeyFrame<Vector2>(150 + 50 * i, from, Easing.OutBack),
                    new KeyFrame<Vector2>(450 + 50 * i, Vector2.zero, Easing.OutBack),
                });
            }
            var difficultyCardsShow = new ITrack[_difficultyButtons.Length];
            for (int i = 0; i < _difficultyButtons.Length; i++)
            {
                DifficultyCardButton target = _difficultyButtons[i];
                difficultyCardsShow[i] = new ScaleTrack(target.Transform, new []
                {
                    new KeyFrame<Vector3>(50 * i, Vector3.zero, Easing.OutBack),
                    new KeyFrame<Vector3>(300 + 50 * i, () => target.Scale, Easing.OutBack),
                });
            }
            
            var show = new TracksEvaluator(new ITrack[]
            {
                new TracksEvaluator(cardsShow),
                new TracksEvaluator(difficultyCardsShow),
                new CanvasGroupOpacityTrack(_title, FloatTrack.KeyFrames01(new TransitionStruct(300, Easing.QuadOut))),
                new CanvasGroupOpacityTrack(_additional, FloatTrack.KeyFrames01(new TransitionStruct(300, Easing.QuadOut)))
            });
            
            
            var cardsHide = new ITrack[_buttons.Length];
            for (int i = 0; i < _buttons.Length; i++)
            {
                RectTransform target = _buttons[i].Transform;
                cardsHide[i] = new AnchoredPositionTrack(target, new []
                {
                    new KeyFrame<Vector2>(30 * i, () => target.anchoredPosition, Easing.QuadOut),
                    new KeyFrame<Vector2>(240 + 30 * i, from, Easing.OutBack),
                });
            }
            var difficultyCardsHide = new ITrack[_difficultyButtons.Length];
            for (int i = 0; i < _difficultyButtons.Length; i++)
            {
                DifficultyCardButton target = _difficultyButtons[i];
                difficultyCardsHide[i] = new ScaleTrack(target.Transform, new []
                {
                    new KeyFrame<Vector3>(30 * i, () => target.Scale, Easing.QuintOut),
                    new KeyFrame<Vector3>(240 + 30 * i, Vector3.zero, Easing.OutBack),
                });
            }
            
            var hide = new TracksEvaluator(new ITrack[]
            {
                new TracksEvaluator(cardsHide),
                new TracksEvaluator(difficultyCardsHide),
                new CanvasGroupOpacityTrack(_title, FloatTrack.KeyFrames10(new TransitionStruct(240, Easing.QuadOut))),
                new CanvasGroupOpacityTrack(_additional, FloatTrack.KeyFrames10(new TransitionStruct(240, Easing.QuadOut)))
            });

            _hidePlayer = new AnimationPlayer(this, new VoidTrack((int)hide.FullDuration));
            _animation = new StatedAnimationPlayer<Visibility>(this, new Dictionary<Visibility, TracksEvaluator>()
            {
                { Visibility.Visible, show },
                { Visibility.Hidden, hide },
            });
            _animation.StateChanged += (previous, current) =>
            {
                if (current == Visibility.Hidden)
                {
                    _graphicRaycaster.enabled = false;
                }
                
                foreach (MenuCardButton button in _buttons)
                {
                    button.CancelAnimation();
                }
            };
            _animation.AnimationEnded += (previous, current) =>
            {
                if (current == Visibility.Visible)
                {
                    _graphicRaycaster.enabled = true;
                }
            };
        }
        
        protected override void Activated()
        {
            _animation.SetState(Visibility.Visible);
        }

        protected override void Deactivated()
        {
            _animation.SetState(Visibility.Hidden);
        }

        public void Close()
        {
            HidePage();
        }

        public void StartGame()
        {
            _gameManager.StartGame(new GameSettings()
            {
                PlayersCount = ((PlayersCountButton)_playersCount.Current).Players,
                StartingCash = _startingCash.Value,
                Difficulty = GetDifficulty()
            });
            HidePage();
        }

        private Difficulty GetDifficulty()
        {
            if (_difficulty.Current is DifficultyCardButton button)
            {
                return button.Difficulty;
            }

            return Difficulty.Normal;
        }
    }
}
