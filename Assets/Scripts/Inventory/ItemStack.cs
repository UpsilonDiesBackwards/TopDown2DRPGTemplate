using System;
using UnityEngine;

namespace Inventory {
    [Serializable]
    public class ItemStack {
        [SerializeField] private Item _item;
        [SerializeField] private int _count;

        public bool IsStackable => _item != null && _item.IsStackable;
        public Item item => _item;
        
        public int Count {
            get => _count;
            set {
                value  = value < 0 ? 0 : value;
                _count = IsStackable ? value : 1;
            }
        }

        public ItemStack(Item item, int count) {
            _item = item;
            Count = count;
        }

        public ItemStack() {
        }
    }
}