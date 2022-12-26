using System.Collections;
using System.Collections.Generic;
using RPG.Core.UI.Dragging;
using UnityEngine;

namespace RPG.Inventories
{
    /// <summary>
    /// Handles spawning pickups when item dropped into the world.
    ///
    /// Must be placed on the root canvas where items can be dragged. Will be
    /// called if dropped over empty space.
    /// </summary>
    public class InventoryDropTarget : MonoBehaviour, IDragDestination<InventoryItem>
    {
        public void AddItems(InventoryItem item, int number)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<ItemDropper>().DropItem(item);
        }

        public int MaxAcceptable(InventoryItem item)
        {
            return int.MaxValue;
        }
    }
}

