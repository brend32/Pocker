using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AurumGames.CustomLayout
{
    public sealed class CustomFixedSizeGridLayout : CustomStructureLayout
    {
        public int ObjectsPerLine
        {
            get => _objectsPerLine;
            set
            {
                _objectsPerLine = Mathf.Clamp(value, 1, int.MaxValue);
                UpdateLayout();
            }
        }
        
        public float GrowPadding => GetValueByDirection(_offset.vertical, _offset.horizontal);
        public float FixedPadding => GetValueByDirection(_offset.horizontal, _offset.vertical);
        public float GrowSpacing => GetValueByDirection(_verticalSpacing, _horizontalSpacing);
        public float FixedSpacing => GetValueByDirection(_horizontalSpacing, _verticalSpacing);

        [SerializeField] private float _verticalSpacing;
        [SerializeField] private float _horizontalSpacing;
        [SerializeField] private RectTransform.Axis _growDirection;
        [SerializeField] private bool _fitGrow;
        [SerializeField] private bool _useAspectRatio = true;
        [SerializeField] private float _growCellSize;
        [SerializeField] private float _aspect = 1;
        [SerializeField, Min(1)] private int _objectsPerLine = 1;

        private struct FixedGrowVector
        {
            public float Fixed;
            public float Grow;

            public FixedGrowVector(float @fixed, float grow)
            {
                Fixed = @fixed;
                Grow = grow;
            }
        }

        protected override void UpdateLayoutInternal()
        {
            var childrenGroups = GetChildren();
            if (childrenGroups.Count == 0)
                return;
            
            LayoutGroupCalculation layoutGroupCalculation = Calculate(childrenGroups);

            if (_fitGrow)
                RectTransform.SetSizeWithCurrentAnchors(_growDirection, layoutGroupCalculation.ContainerSize[(int)_growDirection]);
            
            foreach (var pair in layoutGroupCalculation.Transforms)
            {
                Apply(childrenGroups[pair.Key], pair.Value);
            }
        }

        public override LayoutGroupCalculation Calculate(Dictionary<int, List<RectTransform>> childrenGroups)
        {
            var result = new Dictionary<int, IReadOnlyList<Rect>>();
            _objectsPerLine = Mathf.Clamp(_objectsPerLine, 1, int.MaxValue);
            
            Rect rect = RectTransform.rect;
            var containerFixedSideSize = GetValueByDirection(rect.width, rect.height);
            var fixedCellSize = GetFixedCellSize(containerFixedSideSize);
            var growCellSize = GetGrowCellSize(fixedCellSize);

            var fixedFitSize = GetFitFixedSize(_objectsPerLine, fixedCellSize);
            var growFitSizes = childrenGroups.Values.Select(children => GetGrowFitSize(children.Count, growCellSize)).ToList();
            
            Vector2 containerSize = GetContainerSize(containerFixedSideSize, growFitSizes.Max());
            Vector2 containerPivot = GetContainerPivot(containerSize);
            Vector2 pivot = GetPivot(containerSize);
            
            var i = 0;
            foreach (var pair in childrenGroups)
            {
                result.Add(pair.Key, CalculateLayout(growFitSizes[i], pair.Value, fixedCellSize, growCellSize, fixedFitSize, pivot, containerPivot));
                i++;
            }

            return new LayoutGroupCalculation(result, containerSize);
        }

        public override LayoutCalculation Calculate(IReadOnlyList<RectTransform> children)
        {
            _objectsPerLine = Mathf.Clamp(_objectsPerLine, 1, int.MaxValue);
            
            Rect rect = RectTransform.rect;
            var containerFixedSideSize = GetValueByDirection(rect.width, rect.height);
            var fixedCellSize = GetFixedCellSize(containerFixedSideSize);
            var growCellSize = GetGrowCellSize(fixedCellSize);

            var fixedFitSize = GetFitFixedSize(_objectsPerLine, fixedCellSize);
            var growFitSize = GetGrowFitSize(children.Count, growCellSize);
            
            Vector2 containerSize = GetContainerSize(containerFixedSideSize, growFitSize);
            Vector2 containerPivot = GetContainerPivot(containerSize);
            Vector2 pivot = GetPivot(containerSize);

            var result = CalculateLayout(growFitSize, children, fixedCellSize, growCellSize, fixedFitSize, pivot, containerPivot);

            return new LayoutCalculation(result, containerSize);
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
                if (childrenGroups.Count == 0)
                    return preferredSize;
                
                var containerFixedSideSize = GetValueByDirection(rect.width, rect.height);
                var fixedCellSize = GetFixedCellSize(containerFixedSideSize);
                var growCellSize = fixedCellSize * _aspect;

                var growFitSizes = childrenGroups.Values.Select(children => GetGrowFitSize(children.Count, growCellSize)).ToList();
            
                Vector2 containerSize = GetContainerSize(containerFixedSideSize, growFitSizes.Max());

                if (_fitGrow)
                    preferredSize[(int)_growDirection] = containerSize[(int)_growDirection];

                return preferredSize;
            }
            finally
            {
                RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
                RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);
            }
        }

        public void Apply(IReadOnlyList<RectTransform> children, IReadOnlyList<Rect> calculations, bool applyPosition = true)
        {
            for (var i = 0; i < children.Count; i++)
            {
                var index = _reverse ? children.Count - i - 1 : i;
                RectTransform rectTransform = children[index];
                Rect rect = calculations[index];
                
                if (applyPosition)
                    rectTransform.localPosition = new Vector3(rect.x, rect.y, rectTransform.localPosition.z);
                
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);
                
                if (rectTransform.TryGetComponent(out CustomLayoutBase layout))
                    layout.UpdateLayout(false);
            }
        }

        public float GetGrowCellSize()
        {
            if (_useAspectRatio)
            {
                Rect rect = RectTransform.rect;
            
                var containerFixedSideSize = GetValueByDirection(rect.width, rect.height);
                var fixedCellSize = GetFixedCellSize(containerFixedSideSize);
                return fixedCellSize * _aspect;
            }
            else
            {
                return _growCellSize;
            }
        }

        private float GetGrowCellSize(float fixedCellSize)
        {
            if (_useAspectRatio)
            {
                return fixedCellSize * _aspect;
            }
            else
            {
                return _growCellSize;
            }
        }

        private IReadOnlyList<Rect> CalculateLayout(float growFitSize,
            IReadOnlyList<RectTransform> children,
            float fixedCellSize,
            float growCellSize,
            float fixedFitSize,
            Vector2 pivot,
            Vector2 containerPivot)
        {
            var result = new Rect[children.Count];
            
            for (int i = 0; i < children.Count; i++)
            {
                var index = _reverse ? children.Count - i - 1 : i;
                RectTransform child = children[index];
                var growIndex = i / _objectsPerLine;
                var fixedIndex = i % _objectsPerLine;

                var fixedOffset = fixedIndex * (fixedCellSize + FixedSpacing);
                var growOffset = growIndex * (growCellSize + GrowSpacing);

                var fitSizeVector = new FixedGrowVector(fixedFitSize, growFitSize);
                var objectsRemain = children.Count % _objectsPerLine;
                if (objectsRemain != 0 && i >= children.Count - objectsRemain)
                    fitSizeVector.Fixed = GetFitFixedSize(objectsRemain, fixedCellSize);

                result[index] = CalculateRect(child,
                    pivot,
                    new FixedGrowVector(fixedOffset, growOffset),
                    new FixedGrowVector(fixedCellSize, growCellSize),
                    fitSizeVector, 
                    containerPivot);
            }
            
            return result;
        }
        
        private Rect CalculateRect(RectTransform child,
            Vector2 pivot,
            FixedGrowVector offset,
            FixedGrowVector cellSize,
            FixedGrowVector fitSize,
            Vector2 containerPivot)
        {
            fitSize.Fixed -= FixedPadding;
            fitSize.Grow -= GrowPadding;
            
            var width = GetValueByDirection(cellSize.Fixed, cellSize.Grow);
            var height = GetValueByDirection(cellSize.Grow, cellSize.Fixed);

            var horizontalModifier = HorizontalAlignTo(0, 0.5f, 1);
            var verticalModifier = VerticalAlignTo(0, 0.5f, 1);

            var fixedPosition = CalculatePosition(offset.Fixed, cellSize.Fixed, fitSize.Fixed,
                GetValueByDirection(horizontalModifier, verticalModifier));
            var growPosition = CalculatePosition(offset.Grow, cellSize.Grow, fitSize.Grow,
                GetValueByDirection(verticalModifier, horizontalModifier));
                
            var x = pivot.x + GetValueByDirection(fixedPosition, growPosition);
            var y = pivot.y - GetValueByDirection(growPosition, fixedPosition);

            x -= containerPivot.x;
            y += containerPivot.y;
            Vector2 position = GetConvertToObjectPivotPosition(new Vector2(x, y), child);
            return new Rect(position.x, position.y, width, height);
        }

        private float GetFitFixedSize(int count, float cellSize)
        {
            return count * (cellSize + FixedSpacing) - FixedSpacing + FixedPadding;
        }

        private static float CalculatePosition(float offset, float cellSize, float fitSize, float alignmentModifier)
        {
            return offset + cellSize * 0.5f - fitSize * alignmentModifier;
        }

        private float GetFixedCellSize(float containerFixedSideSize)
        {
            var spacing = FixedSpacing * (_objectsPerLine - 1);
            return (containerFixedSideSize - FixedPadding - spacing) / _objectsPerLine;
        }

        private float GetGrowFitSize(int childCount, float growCellSize)
        {
            var lines = (int)Math.Ceiling(childCount / (float)_objectsPerLine);
            return lines * (GrowSpacing + growCellSize) + GrowPadding - GrowSpacing;
        }
        
        private Vector2 GetContainerSize(float containerFixedSideSize, float growFitSize)
        {
            Rect rect = RectTransform.rect;

            var growSize = _fitGrow ? growFitSize : GetValueByDirection(rect.height, rect.width);
            return _growDirection == RectTransform.Axis.Vertical 
                ? new Vector2(containerFixedSideSize, growSize) 
                : new Vector2(growSize, containerFixedSideSize);
        }

        private T GetValueByDirection<T>(T vertical, T horizontal)
        {
            return _growDirection == RectTransform.Axis.Vertical ? vertical : horizontal;
        }
    }
}