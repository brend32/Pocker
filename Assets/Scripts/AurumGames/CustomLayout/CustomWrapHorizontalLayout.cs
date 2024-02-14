using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AurumGames.CustomLayout
{
    public sealed class CustomWrapHorizontalLayout : CustomStructureLayout
    {
        [SerializeField] private float _spacingVertical;
        [SerializeField] private float _spacingHorizontal;
        [SerializeField] private float _verticalSize;
        [SerializeField] private bool _fitVertical;
        
        protected override void UpdateLayoutInternal()
        {
            var childrenGroups = GetChildren();
            if (childrenGroups.Count == 0)
                return;

            LayoutGroupCalculation layoutGroupCalculation = Calculate(childrenGroups);

            if (_fitVertical)
                RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, layoutGroupCalculation.ContainerSize.y);
            
            foreach (var pair in layoutGroupCalculation.Transforms)
            {
                Apply(childrenGroups[pair.Key], pair.Value);
            }
        }

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
                
                var childrenGroups = GetChildren();
                var childrenSizes = childrenGroups.Values
                    .Select(GetSizes).ToArray();
                var linesFitSizes = childrenSizes
                    .Select(GetLinesFitSizes).ToArray();
                var fitSizes = linesFitSizes
                    .Select(linesFitSize => GetFitSize(linesFitSize.Count)).ToArray();
                Vector2 containerSize = GetContainerSize(FindBiggest(fitSizes));
                
                if (_fitVertical)
                    preferredSize.y = containerSize.y;

                return preferredSize;
            }
            finally
            {
                RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
                RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);
            }
        }

        public override LayoutGroupCalculation Calculate(Dictionary<int, List<RectTransform>> childrenGroups)
        {
            var result = new Dictionary<int, IReadOnlyList<Rect>>();
            var childrenSizes = childrenGroups.Values
                .Select(GetSizes).ToArray();
            var linesFitSizes = childrenSizes
                .Select(GetLinesFitSizes).ToArray();
            var fitSizes = linesFitSizes
                .Select(linesFitSize => GetFitSize(linesFitSize.Count)).ToArray();

            Vector2 containerSize = GetContainerSize(FindBiggest(fitSizes));
            Vector2 containerPivot = GetContainerPivot(containerSize);
            Vector2 pivot = GetPivot(containerSize);

            var i = 0;
            foreach (var pair in childrenGroups)
            {
                result.Add(pair.Key, CalculateLayout(pair.Value, childrenSizes[i], linesFitSizes[i], pivot, fitSizes[i], containerPivot)); 
                i++;
            }

            return new LayoutGroupCalculation(result, containerSize);
        }
        
        public override LayoutCalculation Calculate(IReadOnlyList<RectTransform> children)
        {
            var childrenSizes = GetSizes(children);
            var linesFitSizes = GetLinesFitSizes(childrenSizes);
            Vector2 fit = GetFitSize(linesFitSizes.Count);

            Vector2 containerSize = GetContainerSize(fit);
            Vector2 containerPivot = GetContainerPivot(containerSize);
            Vector2 pivot = GetPivot(containerSize);

            return new LayoutCalculation(CalculateLayout(children, childrenSizes, linesFitSizes, pivot, fit, containerPivot), containerSize);
        }

        public void Apply(IReadOnlyList<RectTransform> children, IReadOnlyList<Rect> calculations, bool applyPosition = true)
        {
            for (var i = 0; i < children.Count; i++)
            {
                var index = _reverse ? children.Count - i - 1 : i;
                RectTransform rectTransform = children[index];
                Rect rect = calculations[index];
                
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _verticalSize);
                
                if (applyPosition)
                    rectTransform.localPosition = new Vector3(rect.x, rect.y, rectTransform.localPosition.z);
                
                if (rectTransform.TryGetComponent(out CustomLayoutBase layout))
                    layout.UpdateLayout(false);
            }
        }
        
        private Vector2 GetFitSize(int lines)
        {
            Rect self = RectTransform.rect;

            var fitHeight = GetHeightFit(lines);
            var fitWidth = self.width;

            return new Vector2(fitWidth, fitHeight);
        }

        private IReadOnlyList<Vector2> GetSizes(IReadOnlyList<RectTransform> children)
        {
            var result = new Vector2[children.Count];
            Vector2 preferSize = Vector2.positiveInfinity;
            preferSize.y = _verticalSize;

            for (var i = 0; i < children.Count; i++)
            {
                RectTransform child = children[i];
                if (child.TryGetComponent(out CustomLayoutBase layout))
                {
                     result[i] = layout.GetSize(preferSize);
                }
                else
                {
                    result[i] = child.rect.size;
                }
            }

            return result;
        }
        
        private float GetHeightFit(int lines)
        {
            return lines * (_verticalSize + _spacingVertical) - _spacingVertical + _offset.vertical;
        }

        private IReadOnlyList<float> GetLinesFitSizes(IReadOnlyList<Vector2> childrenSizes)
        {
            var result = new List<float>();
            
            Rect self = RectTransform.rect;
            float lineWidth = _offset.horizontal;
            
            for (var i = 0; i < childrenSizes.Count; i++)
            {
                var index = _reverse ? childrenSizes.Count - i - 1 : i;
                var width = childrenSizes[index].x;

                if (lineWidth + width <= self.width)
                {
                    lineWidth += width + _spacingHorizontal;
                    continue;
                }
                
                result.Add(lineWidth - _spacingHorizontal);
                lineWidth = _offset.horizontal + width + _spacingHorizontal;
            }

            result.Add(lineWidth - _spacingHorizontal);
            return result;
        }

        private Vector2 GetContainerSize(Vector2 fitSize)
        {
            Rect self = RectTransform.rect;
            
            var containerWidth = self.width;
            var containerHeight = self.height;
            if (_fitVertical)
                containerHeight = fitSize.y;
            
            return new Vector2(containerWidth, containerHeight);
        }

        private IReadOnlyList<Rect> CalculateLayout(IReadOnlyList<RectTransform> children, IReadOnlyList<Vector2> childrenSizes, IReadOnlyList<float> linesFitSizes, Vector2 pivot, Vector2 fitSize, Vector2 containerPivot)
        {
            var result = new Rect[children.Count];
            var currentX = pivot.x - HorizontalAlignTo(0, 0.5f, 1) * (linesFitSizes[0] - _offset.horizontal);
            float lineWidth = _offset.horizontal;
            var lines = 1;

            for (var i = 0; i < children.Count; i++)
            {
                var index = _reverse ? children.Count - i - 1 : i;
                RectTransform rectTransform = children[index];
                Vector2 size = childrenSizes[index]; 
                var height = _verticalSize;
                var width = size.x;

                lineWidth += width + _spacingHorizontal;
                if (lineWidth - _spacingHorizontal > fitSize.x)
                {
                    lineWidth = _offset.horizontal + width + _spacingHorizontal;
                    currentX = pivot.x - HorizontalAlignTo(0, 0.5f, 1) * (linesFitSizes[lines] - _offset.horizontal);
                    lines++;
                }
                
                var x = currentX + width / 2;
                currentX += _spacingHorizontal + width;
                var verticalOffset = lines * (_verticalSize + _spacingVertical) - _spacingVertical;
                var y = pivot.y - verticalOffset + _verticalSize * 0.5f + VerticalAlignTo(0, 0.5f, 1) * (fitSize.y - _offset.vertical);

                x -= containerPivot.x;
                y += containerPivot.y;
                Vector2 position = GetConvertToObjectPivotPosition(new Vector2(x, y), rectTransform);
                result[index] = new Rect(position.x, position.y, width, height);
            }

            return result;
        }
    }
}
