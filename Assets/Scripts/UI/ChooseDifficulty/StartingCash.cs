using AurumGames.CompositeRoot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poker.UI.ChooseDifficulty
{
	public class StartingCash : LazyMonoBehaviour
	{
		public int Value { get; private set; }
		
		[SerializeField] private int[] _values;
		[SerializeField] private TextMeshProUGUI _text;
		[SerializeField] private Slider _slider;
		
		protected override void InitInnerState()
		{
			_slider.maxValue = _values.Length - 1;
			_slider.wholeNumbers = true;
			_slider.onValueChanged.AddListener(UpdateView);
			UpdateView(_slider.value);
		}

		protected override void Initialized()
		{
			
		}

		private void UpdateView(float index)
		{
			Value = _values[(int)index];
			_text.text = $"{Value}$";
		}
	}
}