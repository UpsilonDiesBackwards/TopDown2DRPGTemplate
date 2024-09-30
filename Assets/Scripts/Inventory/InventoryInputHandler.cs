using System;
using UnityEngine;

namespace Inventory {
    public class InventoryInputHandler : MonoBehaviour {
        private Inventory _inventory;

        private void Awake() {
            _inventory = GetComponent<Inventory>();
        }

        private void Update() {
            switch (true) {
                case var _ when Input.GetKeyDown(KeyCode.Alpha1):
                    ActivateSlot(0);
                    break;
                case var _ when Input.GetKeyDown(KeyCode.Alpha2):
                    ActivateSlot(1);
                    break;
                case var _ when Input.GetKeyDown(KeyCode.Alpha3):
                    ActivateSlot(2);
                    break;
                case var _ when Input.GetKeyDown(KeyCode.Alpha4):
                    ActivateSlot(3);
                    break;
                case var _ when Input.GetKeyDown(KeyCode.Alpha5):
                    ActivateSlot(4);
                    break;
                case var _ when Input.GetKeyDown(KeyCode.Alpha6):
                    ActivateSlot(5);
                    break;
                case var _ when Input.GetKeyDown(KeyCode.Alpha7):
                    ActivateSlot(6);
                    break;
                case var _ when Input.GetKeyDown(KeyCode.Alpha8):
                    ActivateSlot(7);
                    break;
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f) {
                OnNextItem();
            } else if (scroll < 0f) {
                OnPreviousItem();
            }

            if (Input.GetKeyDown(KeyCode.Q)) {
                OnThrowItem();
            }
        }

        private void OnThrowItem() {
            if (_inventory.GetActiveSlot().HasItem) {
                _inventory.RemoveItem(_inventory.ActiveSlotIndex, true);
            }
        }

        private void ActivateSlot(int slotIndex) {
            _inventory.ActivateSlot(slotIndex);
        }
        
        private void OnNextItem() {
            int nextIndex = (_inventory.ActiveSlotIndex + 1) % _inventory.Slots.Count;
            _inventory.ActivateSlot(nextIndex);
        }

        private void OnPreviousItem() {
            int previousIndex = (_inventory.ActiveSlotIndex - 1 + _inventory.Slots.Count) % _inventory.Slots.Count;
            _inventory.ActivateSlot(previousIndex);
        }
    }
}