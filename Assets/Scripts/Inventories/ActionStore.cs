using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RPG.Saving;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.Inventories
{
    /// <summary>
    /// Provides the storage for an action bar. The bar has a finite number of
    /// slots that can be filled and actions in the slots can be "used".
    ///
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class ActionStore : MonoBehaviour, ISaveable
    {
        private Dictionary<int, DockedItemSlot> dockedItems = new Dictionary<int, DockedItemSlot>();

        private class DockedItemSlot
        {
            public ActionItem item;
            public int number;
        }

        /// <summary>
        /// Broadcasts when the items in the slots are added/removed.
        /// </summary>
        public event Action storeUpdated;

        /// <summary>
        /// Get the number of items left at the given index.
        /// </summary>
        /// <param name="index">Number corresponding to the slot.</param>
        /// <returns>
        /// Will return 0 if no item is in the index or the item has
        /// been fully consumed.
        /// </returns>
        public ActionItem GetAction(int index)
        {
            if (dockedItems.ContainsKey(index))
            {
                return dockedItems[index].item;
            }

            return null;
        }

        /// <summary>
        /// Get the number of items left at the given index.
        /// </summary>
        /// <param name="index">The position of the action slot.</param>
        /// <returns>
        /// Will return 0 if no item is in the index or the item has
        /// been fully consumed.
        /// </returns>
        public int GetNumber(int index)
        {
            if (dockedItems.ContainsKey(index))
            {
                return dockedItems[index].number;
            }

            return 0;
        }

        /// <summary>
        /// Add an item to the given index.
        /// </summary>
        /// <param name="item">What item should be added.</param>
        /// <param name="index">Where should the item be added.</param>
        /// <param name="number">How many items to add.</param>
        public void AddAction(InventoryItem item, int index, int number)
        {
            if (dockedItems.ContainsKey(index))
            {
                if (object.ReferenceEquals(item, dockedItems[index].item))
                {
                    dockedItems[index].number += number;
                }
            }
            else
            {
                var slot = new DockedItemSlot();
                slot.item = item as ActionItem;
                slot.number = number;
                dockedItems[index] = slot;
            }

            if (storeUpdated != null)
            {
                storeUpdated();
            }

            Debug.Log("Action Added");
        }

        /// <summary>
        /// Use the item at the given slot. If the item is consumable one
        /// instance will be destroyed until the item is removed completely.
        /// </summary>
        /// <param name="index">Where the action is located.</param>
        /// <param name="user">The character that wants to use this action.</param>
        /// <returns>False if the action could not be executed.</returns>
        public bool Use(int index, GameObject user)
        {
            if (dockedItems.ContainsKey(index))
            {
                dockedItems[index].item.Use(user);
                if (dockedItems[index].item.isConsumable())
                {
                    RemoveItems(index, 1);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove given number of items from the given slot.
        /// </summary>
        /// <param name="index">Location of slot.</param>
        /// <param name="number">Amount to be deducted.</param>
        public void RemoveItems(int index, int number)
        {
            if (dockedItems.ContainsKey(index))
            {
                dockedItems[index].number -= number;
                if (dockedItems[index].number <= 0)
                {
                    dockedItems.Remove(index);
                }

                if (storeUpdated != null)
                {
                    storeUpdated();
                }
            }
        }

        /// <summary>
        /// What is the maximum number of items allowed in this slot.
        ///
        /// This takes into account whether the slot already contains an item
        /// and whether it is the same type. Will only accept multiple if the
        /// item is consumable.
        /// </summary>
        /// <returns>Will return int.MaxValue when there is not effective bound.</returns>
        public int MaxAcceptable(InventoryItem item, int index)
        {
            var actionItem = item as ActionItem;
            if (!actionItem) return 0;

            if (dockedItems.ContainsKey(index) && !object.ReferenceEquals(item, dockedItems[index].item))
            {
                return 0;
            }

            if (actionItem.isConsumable())
            {
                return int.MaxValue;
            }

            if (dockedItems.ContainsKey(index))
            {
                return 0;
            }

            return 1;
        }

        [System.Serializable]
        struct DockedItemRecord
        {
            public string itemID;
            public int number;
        }

        object ISaveable.CaptureState()
        {
            var state = new Dictionary<int, DockedItemRecord>();
            foreach (var pair in dockedItems)
            {
                var record = new DockedItemRecord();
                record.itemID = pair.Value.item.GetItemID();
                record.number = pair.Value.number;
                state[pair.Key] = record;
            }

            return state;
        }

        void ISaveable.RestoreState(object state)
        {
            var stateDict = (Dictionary<int, DockedItemRecord>)state;
            foreach (var pair in stateDict)
            {
                AddAction(InventoryItem.GetFromID(pair.Value.itemID), pair.Key, pair.Value.number);
            }
        }
    }
}