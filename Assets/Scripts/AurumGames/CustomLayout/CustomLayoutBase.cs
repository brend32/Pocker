using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AurumGames.CustomLayout
{
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public abstract class CustomLayoutBase : MonoBehaviour
    {
        public UnityEvent OnLayoutUpdate;
        
        [SerializeField] private CustomLayoutBase _parentLayout;
        
        private RectTransform _self;
        private bool _dirty = true;

        public RectTransform RectTransform
        {
            get
            {
                if (_self == null)
                    _self = GetComponent<RectTransform>();

                return _self;
            }
        }

        private void OnEnable()
        {
            if (_dirty)
                UpdateLayout();
        }

        private void OnDisable()
        {
            if (_dirty == false)
            {
                CustomLayoutBase layoutRoot = GetLayoutRoot();
                if (layoutRoot != null)
                {
                    layoutRoot.UpdateLayout(false);
                }
            }
            
            _dirty = true;
        }

#if  UNITY_EDITOR

        [EasyButtons.Button]
        public void CalculateInEditor()
        {
            UnityEditor.Undo.RegisterFullObjectHierarchyUndo(gameObject, "Calculate in editor");
            UpdateLayout();
        }   
        
        [EasyButtons.Button]
        public void FindParent()
        {
            Transform parent = transform.parent;
            while (parent != null)
            {
                if (parent.TryGetComponent(out CustomLayoutBase layoutBase))
                {
                    _parentLayout = layoutBase;
                    return;
                }
                
                parent = parent.parent;
            }
            
            Debug.LogError("No parent found");
        }  
#endif

        public void UpdateLayout(bool notifyParent = true)
        {
            if (enabled == false)
                return;
            
            if (notifyParent)
            {
                CustomLayoutBase layoutRoot = GetLayoutRoot();
                if (layoutRoot != null)
                {
                    layoutRoot.UpdateLayout(false);
                    return;
                }
            }

            _dirty = false;
            UpdateLayoutInternal();
            OnLayoutUpdate?.Invoke();
        }

        private CustomLayoutBase GetLayoutRoot()
        {
            CustomLayoutBase parent = _parentLayout;
            if (parent == null)
                return null;

            while (parent._parentLayout != null)
            {
                parent = parent._parentLayout;
            }

            return parent;
        }

        protected Vector2 FindBiggest(IReadOnlyList<Vector2> size)
        {
            var maxX = 0f;
            var maxY = 0f;

            for (var i = 0; i < size.Count; i++)
            {
                Vector2 item = size[i];

                if (item.x > maxX)
                    maxX = item.x;
                if (item.y > maxY)
                    maxY = item.y;
            }
            
            return new Vector2(maxX, maxY);
        }

        public abstract Vector2 GetSize(Vector2 preferredSize);
        protected abstract void UpdateLayoutInternal();
    }
}