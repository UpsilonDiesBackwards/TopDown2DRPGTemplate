using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inventory {
    public class Inventory : MonoBehaviour {
        [SerializeField] private int _size = 8;
        [SerializeField] private List<InventorySlot> _slots;

        private int _activeSlotIndex;

        public int Size => _size;
        public List<InventorySlot> Slots => _slots;

        public int ActiveSlotIndex {
            get => _activeSlotIndex;
            private set {
                _slots[_activeSlotIndex].Active = false;
                _activeSlotIndex                = value < 0 ? _size - 1 : value % Size;
                _slots[_activeSlotIndex].Active = true;
            }
        }

        private void Awake() {
            if (_size > 0) _slots[0].Active = true;
        }

        private void OnValidate() {
            AdjustSize();
        }

        public event Action OnInventoryUpdated;

        public void AdjustSize() {
            _slots ??= new List<InventorySlot>();

            if (_slots.Count > _size) _slots.RemoveRange(_size, _slots.Count - _size);
            if (_slots.Count < _size) _slots.AddRange(new InventorySlot[_size - _slots.Count]);
        }

        public bool IsPopulated() {
            return _slots.Count(slot => slot.HasItem) >= _size;
        }

        public bool CanAcceptItem(ItemStack itemStack) {
            var slotWithStackableItem = FindSlot(itemStack.item, true);
            return !IsPopulated() || slotWithStackableItem != null;
        }

        private InventorySlot FindSlot(Item item, bool onlyStackable = false) {
            if (onlyStackable)
                return _slots.FirstOrDefault(slot => slot.HasItem && slot.item == item && item.IsStackable);
            return _slots.FirstOrDefault(slot => slot.HasItem && slot.item == item);
        }

        public bool HasItem(ItemStack itemStack, bool checkCount = false) {
            var itemSlot = FindSlot(itemStack.item);
            if (itemSlot == null) return false;
            if (!checkCount) return true;

            if (itemStack.item.IsStackable) return itemSlot.Count >= itemStack.Count;

            return _slots.Count(slot => slot.item == itemStack.item) >= itemStack.Count;
        }

        public ItemStack AddItem(ItemStack itemStack) {
            var relevantSlot = FindSlot(itemStack.item, true);
            if (IsPopulated() && relevantSlot == null)
                throw new InventoryException(InventoryOperation.Add, "Inventory is full");

            if (relevantSlot != null) {
                relevantSlot.Count += itemStack.Count;
            }
            else {
                relevantSlot       = _slots.First(slot => !slot.HasItem);
                relevantSlot.State = itemStack;
            }

            OnInventoryUpdated?.Invoke();
            return relevantSlot.State;
        }

        public ItemStack RemoveItem(int atIndex, bool spawn = false) {
            if (!_slots[atIndex].HasItem)
                throw new InventoryException(InventoryOperation.Remove, "Given slot is empty");

            if (spawn && TryGetComponent<GameItemSpawner>(out var itemSpawner))
                itemSpawner.SpawnItem(_slots[atIndex].State);

            ClearSlot(atIndex);

            return new ItemStack();
        }

        public ItemStack RemoveItem(ItemStack itemStack) {
            var itemSlot = FindSlot(itemStack.item);

            if (itemSlot == null) throw new InventoryException(InventoryOperation.Remove, "No item in the inventory");

            if (itemSlot.item.IsStackable && itemSlot.Count < itemStack.Count)
                throw new InventoryException(InventoryOperation.Remove, "Not enough items");

            itemSlot.Count -= itemStack.Count;

            if (itemSlot.item.IsStackable && itemSlot.Count > 0) return itemSlot.State;

            itemSlot.Clear();

            OnInventoryUpdated?.Invoke();
            return new ItemStack();
        }

        public void ClearSlot(int atIndex) {
            _slots[atIndex].Clear();
        }

        public void ActivateSlot(int atIndex) {
            ActiveSlotIndex = atIndex;
        }

        public InventorySlot GetActiveSlot() {
            return _slots[ActiveSlotIndex];
        }

        public bool IsFull() {
            return _slots.All(slot => slot.HasItem);
        }
    }
}