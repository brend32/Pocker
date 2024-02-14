using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace AurumGames.CustomLayout
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class CustomTextFitter : CustomLayoutBase
    {
        [SerializeField] private bool _autoUpdate = false;
        [SerializeField] private bool _fitHorizontal;
        [SerializeField] private bool _fitVertical;
        
        private TextMeshProUGUI _text;
        private float _preferredWidth;
        private float _preferredHeight;
        private string _savedText;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _text.OnPreRenderText += PreRender;
        }

        public void ForceUpdate()
        {
            _preferredWidth = 0;
            _preferredHeight = 0;
            _savedText = null;
            
            UpdateLayout();
        }

        private void PreRender(TMP_TextInfo _)
        {
            if (_autoUpdate == false)
                return;
            
            if (_text.text != _savedText)
            {
                Invoke(nameof(ForceUpdate), 0.01f);
                return;
            }
            
            const float tolerance = 0.01f;
            if (_fitHorizontal && Math.Abs(_preferredWidth - _text.preferredWidth) > tolerance)
            {
                Invoke(nameof(ForceUpdate), 0.01f);
                return;
            }
            
            if (_fitVertical && Math.Abs(_preferredHeight - _text.preferredHeight) > tolerance)
            {
                Invoke(nameof(ForceUpdate), 0.01f);
            }
        }

        private void OnEnable()
        {
            ForceUpdate();
        }

        public override Vector2 GetSize(Vector2 preferredSize)
        {
            if (_text == null)
                _text = GetComponent<TextMeshProUGUI>();
            
            var hasPreferredWidth = float.IsInfinity(preferredSize.x) == false;
            var hasPreferredHeight = float.IsInfinity(preferredSize.y) == false;
            
            Vector2 size = _text.rectTransform.rect.size;
            var width = _fitHorizontal ? Mathf.Infinity : size.x;
            var height = _fitVertical ? Mathf.Infinity : size.y;

            if (hasPreferredWidth)
                width = preferredSize.x;
            if (hasPreferredHeight)
                height = preferredSize.y;
            
            preferredSize = _text.GetPreferredValues(width, height);
            if (_fitHorizontal == false)
                preferredSize.x = size.x;
            if (_fitVertical == false)
                preferredSize.y = size.y;
            
            return preferredSize;
        }

        protected override void UpdateLayoutInternal()
        {
            if (_text == null)
                _text = GetComponent<TextMeshProUGUI>();

            _savedText = _text.text;
            Vector2 size = _text.rectTransform.rect.size;
            var width = _fitHorizontal ? Mathf.Infinity : size.x;
            var height = _fitVertical ? Mathf.Infinity : size.y;
            Vector2 preferredSize = _text.GetPreferredValues(width, height);
            
            if (_fitHorizontal)
            {
                _preferredWidth = preferredSize.x;
                _text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _preferredWidth);
            }

            if (_fitVertical)
            {
                _preferredHeight = preferredSize.y;
                _text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _preferredHeight);
            }
        }
    }
}