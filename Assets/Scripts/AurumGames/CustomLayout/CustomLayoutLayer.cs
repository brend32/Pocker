using UnityEngine;

namespace AurumGames.CustomLayout
{
    public sealed class CustomLayoutLayer : MonoBehaviour
    {
        public int Layer => _layer;

        [SerializeField] private int _layer;
        [field: SerializeField] public bool Ignore { get; set; }
        [field: SerializeField] public float Grow { get; set; }
    }
}