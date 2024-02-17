using System;
using UnityEngine;

namespace Poker.UI.Common
{
    public class Card : MonoBehaviour
    {
        public const float Ratio = 0.6F;

        [SerializeField] private float _height = 500;
 
        private void Awake()
        {
            var rect = (RectTransform)transform;
            
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _height);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _height * Ratio);
        }

        private void OnValidate()
        {
            Awake();
        }
    }
}
