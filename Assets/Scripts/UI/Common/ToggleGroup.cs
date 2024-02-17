using System;
using AurumGames.CompositeRoot;
using UnityEngine;

namespace Poker.UI.Common
{
	public interface IToggleOption
	{
		
	}
	
	public class ToggleGroup : LazyMonoBehaviour
	{
		public event Action<IToggleOption> SelectionChanged;
		
		public IToggleOption Current
		{
			get => _selected;
			set
			{
				_selected = value;
				SelectionChanged?.Invoke(value);
			}
		}

		private IToggleOption _selected;
		[SerializeField] private MonoBehaviour _initialSelected;
		
		protected override void InitInnerState()
		{
			
		}

		protected override void Initialized()
		{
			if (_initialSelected is IToggleOption option)
			{
				Current = option;
			}
		}
	}
}