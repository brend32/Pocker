using System.Collections.Generic;
using UnityEngine;

namespace AurumGames.CustomLayout
{
    public struct LayoutGroupCalculation
    {
        public Dictionary<int, IReadOnlyList<Rect>> Transforms;
        public Vector2 ContainerSize;

        public LayoutGroupCalculation(Dictionary<int, IReadOnlyList<Rect>> transforms, Vector2 containerSize)
        {
            Transforms = transforms;
            ContainerSize = containerSize;
        }
    }
    
    public struct LayoutCalculation
    {
        public IReadOnlyList<Rect> Transforms;
        public Vector2 ContainerSize;

        public LayoutCalculation(IReadOnlyList<Rect> transforms, Vector2 containerSize)
        {
            Transforms = transforms;
            ContainerSize = containerSize;
        }
    }
    
    public abstract class CustomStructureLayout : CustomLayoutBase
    {
        [SerializeField] protected RectOffset _offset = new();
        [SerializeField] protected TextAnchor _alignment;
        [SerializeField] protected bool _reverse;

        public RectOffset Offset
        {
            get => _offset;
            set => _offset = value;
        }
        
        public TextAnchor Alignment
        {
            get => _alignment;
            set => _alignment = value;
        }

        protected Vector2 GetPivot(Vector2 containerSize)
        {
            var xOffset = HorizontalAlignTo(_offset.left, (_offset.left - _offset.right) * 0.5f, -_offset.right);
            var yOffset = VerticalAlignTo(_offset.top, (_offset.top - _offset.bottom) * 0.5f, -_offset.bottom);
            var pivotX = HorizontalAlignTo(0, 0.5f, 1) * containerSize.x + xOffset;
            var pivotY = VerticalAlignTo(0, 0.5f, 1) * containerSize.y * -1 - yOffset;
            
            return new Vector2(pivotX, pivotY);
        }
        
        protected Vector2 GetContainerPivot(Vector2 containerSize)
        {
            Vector2 pivot = RectTransform.pivot;
            pivot.y = 1 - pivot.y;
            
            return containerSize * pivot;
        }
        
        protected Vector2 GetConvertToObjectPivotPosition(Vector2 position, RectTransform obj)
        {
            return position + (obj.pivot - Vector2.one / 2) * obj.rect.size;
        }
        
        protected Vector2 GetSelfSizeExcludePadding()
        {
            Rect self = RectTransform.rect;
            
            return new Vector2(self.width - _offset.horizontal, self.height - _offset.vertical);
        }
        
        protected float HorizontalAlignTo(float left, float middle, float right)
        {
            switch (_alignment)
            {
                case TextAnchor.UpperLeft:
                case TextAnchor.MiddleLeft:
                case TextAnchor.LowerLeft:
                    return left;
                
                case TextAnchor.UpperCenter:
                case TextAnchor.MiddleCenter:
                case TextAnchor.LowerCenter:
                    return middle;
                
                case TextAnchor.UpperRight:
                case TextAnchor.MiddleRight:
                case TextAnchor.LowerRight:
                    return right;
            }

            return 0;
        }
        
        protected float VerticalAlignTo(float top, float middle, float bottom)
        {
            switch (_alignment)
            {
                case TextAnchor.UpperLeft:
                case TextAnchor.UpperCenter:
                case TextAnchor.UpperRight:
                    return top;
                
                case TextAnchor.MiddleLeft:
                case TextAnchor.MiddleCenter:
                case TextAnchor.MiddleRight:
                    return middle;
                
                case TextAnchor.LowerLeft:
                case TextAnchor.LowerCenter:
                case TextAnchor.LowerRight:
                    return bottom;
            }

            return 0;
        }

        protected Dictionary<int, List<RectTransform>> GetChildren()
        {
            var children = new Dictionary<int, List<RectTransform>>();
            
            var childCount = transform.childCount;
            for (var i = 0; i < childCount; i++)
            {
                var groupIndex = 0;
                
                var rectTransform = transform.GetChild(i).transform as RectTransform;
                if (rectTransform == null || rectTransform.gameObject.activeInHierarchy == false)
                    continue;

                if (rectTransform.gameObject.TryGetComponent(out CustomLayoutLayer layer))
                {
                    if (layer.Ignore)
                        continue;
                    groupIndex = layer.Layer;
                }

                //rectTransform.anchorMin = new Vector2(0, 1);
                //rectTransform.anchorMax = new Vector2(0, 1);
                //rectTransform.pivot = new Vector2(0.5f, 0.5f);

                if (children.ContainsKey(groupIndex) == false)
                {
                    children.Add(groupIndex, new List<RectTransform>());
                }
                children[groupIndex].Add(rectTransform);
            }
            return children;
        }

        public abstract LayoutGroupCalculation Calculate(Dictionary<int, List<RectTransform>> childrenGroups);
        public abstract LayoutCalculation Calculate(IReadOnlyList<RectTransform> children);
    }
}