using System;
using UnityEngine;

namespace Poker.UI.Menu
{
    public class FanPlacement : MonoBehaviour
    {
        [SerializeField] private float _rotationStep;
        [SerializeField] private float _radius;
        [SerializeField] private float _radiusDecay;

        private void Awake()
        {
            Transform self = transform;
            var count = self.childCount;
            var alignment = _rotationStep * (count - 1) / 2;
            Vector3 up = Quaternion.Euler(0, 0, alignment) * new Vector3(0, _radius);
            Quaternion rotation = Quaternion.Euler(0, 0, _rotationStep * -1);

            for (int i = 0; i < count; i++)
            {
                Transform child = transform.GetChild(i);

                child.eulerAngles = new Vector3(0, 0, i * _rotationStep * -1 + alignment);
                if (i != 0)
                    up = rotation * up;

                var decay = Mathf.Floor(Math.Abs(count / 2f - i - 0.5f));
                child.localPosition = up - new Vector3(0, _radius + _radiusDecay * decay, 0);
            }
        }

        [EasyButtons.Button]
        private void OnValidate()
        {
            Awake();
        }
    }
}
