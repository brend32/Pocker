using System;
using AurumGames.Animation;
using AurumGames.Animation.Tracks;
using AurumGames.CompositeRoot;
using Cysharp.Threading.Tasks;
using Poker.Gameplay.Configuration;
using Poker.Gameplay.Core.Models;
using TMPro;
using UnityEngine;

namespace Poker.Gameplay.Views
{
	public partial class CardView : LazyMonoBehaviour
	{
		public bool Revealed
		{
			set
			{
				_reveal = value;
				UpdateCardTexture();
			}
			get => _reveal;
		}
		
		[SerializeField] private Material _faceReference;
		[SerializeField] private Material _coverReference;
		[SerializeField] private MeshRenderer _meshRenderer;
		[SerializeField] private Texture2D _card;
		[SerializeField] private bool _reveal;

		public TextMeshPro DebugText;

		[Dependency] private CardsDatabase _cardsDatabase;

		private Material _face;

		private AnimationPlayer _revealCardPlayer;
		
		protected override void InitInnerState()
		{
			_face = Instantiate(_faceReference);

			Transform self = transform;

			Vector3 position = self.localPosition;

			_revealCardPlayer = new AnimationPlayer(this, new ITrack[]
			{
				new LocalEulerAnglesTrack(self, new []
				{
					new KeyFrame<Vector3>(0, new Vector3(0, 0, 180), Easing.QuadOut),
					new KeyFrame<Vector3>(600, new Vector3(0, 0, 360), Easing.QuadOut)
				}),
				new LocalPositionTrack(self, new []
				{
					new KeyFrame<Vector3>(0, new Vector3(position.x, 0, position.z), Easing.QuintOut),
					new KeyFrame<Vector3>(350, new Vector3(position.x, 2, position.z), Easing.QuintIn),
					new KeyFrame<Vector3>(700, new Vector3(position.x, 0, position.z), Easing.QuintIn),
				})
			});
			
			UpdateCardTexture();
		}

		protected override void Initialized()
		{
			
		}

		public async UniTask RevealCardAnimation()
		{
			_revealCardPlayer.PlayFromStart();
			Revealed = true;

			await UniTask.WaitWhile(() => _revealCardPlayer.IsPlaying);
		}

		public void Bind(CardModel model)
		{
			DebugText.text = $"{model.Type}\n<size=150%>{model.Value}";
			_card = _cardsDatabase.GetTexture(model);
			
			UpdateCardTexture();
		}

		private void UpdateCardTexture()
		{
			Material material = _reveal ? _face : _coverReference;
			
			if (_reveal)
			{
				_face.mainTexture = _card;
			}
			
			var materials = _meshRenderer.sharedMaterials;
			materials[0] = material;
			_meshRenderer.sharedMaterials = materials;
		}
		
#if UNITY_EDITOR
		[EasyButtons.Button]
		private void OnValidate()
		{
			if (_face == _faceReference || _face == null)
			{
				_face = Instantiate(_faceReference);
			}
			
			UpdateCardTexture();
		}
#endif
	}
}