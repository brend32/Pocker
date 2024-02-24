using System;
using AurumGames.CompositeRoot;
using Poker.Gameplay.Configuration;
using Poker.Gameplay.Core.Models;
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

		[Dependency] private CardsDatabase _cardsDatabase;

		private Material _face;
		
		protected override void InitInnerState()
		{
			_face = Instantiate(_faceReference);
			
			UpdateCardTexture();
		}

		protected override void Initialized()
		{
			
		}

		public void Bind(CardModel model)
		{
			Debug.Log(model.ToString());	
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