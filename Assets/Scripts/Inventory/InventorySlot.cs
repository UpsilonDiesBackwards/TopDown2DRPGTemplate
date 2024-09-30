using System;
using UnityEngine;

namespace Inventory {
    [Serializable]
    public class InventorySlot {
        public event EventHandler<InventorySlotStateChangedArgs> StateChanged; 
        
        [SerializeField] private ItemStack _state;
        private bool _active;

        public ItemStack State {
            get => _state;
            set {
                _state = value;
                NotifyStateChange();
            }
        }
        
        public bool Active {
            get => _active;
            set {
                _active = value;
                NotifyStateChange();
            }
        }

        public int Count {
            get => _state.Count;
            set {
                _state.Count = value;
                NotifyStateChange();
            }
        }

        public void Clear() {
            State = null;
        }

        public bool HasItem => _state?.item != null;
        public Item item => _state.item;
        
        private void NotifyStateChange() {
            StateChanged?.Invoke(this, new InventorySlotStateChangedArgs(_state, _active));
        }
    }
}