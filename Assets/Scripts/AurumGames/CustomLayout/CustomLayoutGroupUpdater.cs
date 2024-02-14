using System.Collections.Generic;
using UnityEngine;

namespace AurumGames.CustomLayout
{
    public class CustomLayoutGroupUpdater : CustomLayoutBase
    {
        [SerializeField] private CustomLayoutBase[] _childs;
        [SerializeField] private Vector2 _reportSize;


        public override Vector2 GetSize(Vector2 preferredSize)
        {
            return _reportSize;
        }

        protected override void UpdateLayoutInternal()
        {
            foreach (CustomLayoutBase layoutBase in _childs)
            {
                layoutBase.UpdateLayout(false);
            }
        }
    }
}