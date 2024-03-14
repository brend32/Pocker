using System;
using UnityEngine;

namespace AurumGames.CustomLayout
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class CustomFitter : CustomLayoutBase
    {
        [SerializeField] private RectTransform _target;
        [SerializeField] private Vector2 _min;
        [SerializeField] private Vector2 _max;
        [SerializeField] private Vector2 _extra;
        [SerializeField] private bool _fitHorizontal = true;
        [SerializeField] private bool _fitVertical = true;
        [SerializeField] private bool _minMaxWithoutExtra;

        private void OnEnable()
        {
            UpdateLayout();
        }

#if  UNITY_EDITOR
        private void OnValidate()
        {
            _max.x = _max.x == 0 ? float.MaxValue : _max.x;
            _max.y = _max.y == 0 ? float.MaxValue : _max.y;

            if (_min.x > _max.x)
                _max.x = _min.x;
            
            if (_min.y > _max.y)
                _max.y = _min.y;
        }
#endif

        public override Vector2 GetSize(Vector2 preferredSize)
        {
            var hasPreferredWidth = float.IsInfinity(preferredSize.x) == false;
            var hasPreferredHeight = float.IsInfinity(preferredSize.y) == false;

            Rect rect = RectTransform.rect;

            try
            {
                if (hasPreferredWidth)
                    RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, preferredSize.x);
                else
                    preferredSize.x = rect.width;
                
                if (hasPreferredHeight)
                    RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredSize.y);
                else
                    preferredSize.y = rect.height;
                
                return GetSize();
            }
            finally
            {
                RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
                RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);
            }
        }

        protected override void UpdateLayoutInternal()
        {
            Vector2 size = GetSize();

            if (_fitHorizontal)
                RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            if (_fitVertical)
                RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            
            if (_target.TryGetComponent(out CustomLayoutBase layout))
            {
                layout.UpdateLayout(false);
            }
        }

        private Vector2 GetSize()
        {
            Rect self = RectTransform.rect;
            Vector2 size = _target.rect.size;
            if (_target.TryGetComponent(out CustomLayoutBase layout))
            {
                size = layout.GetSize(Vector2.positiveInfinity);
            }

            var width = size.x;
            var height = size.y;

            if (_minMaxWithoutExtra == false)
            {
                width += _extra.x;
                height += _extra.y;
            }

            width = Mathf.Clamp(width, _min.x, _max.x);
            height = Mathf.Clamp(height, _min.y, _max.y);

            if (_minMaxWithoutExtra)
            {
                width += _extra.x;
                height += _extra.y;
            }

            return new Vector2(_fitHorizontal ? width : self.width, _fitVertical ? height : self.height);
        }
    }
}