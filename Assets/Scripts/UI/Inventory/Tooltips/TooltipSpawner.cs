using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPG.Core.UI.Tooltips
{
    /// <summary>
    /// Abstract base class that handles the spawning of a tooltip prefab at the
    /// correct position on the screen relative to a cursor.
    ///
    /// Override the abstract functions to create a tooltip spawner for your own
    /// data.
    /// </summary>
    public abstract class TooltipSpawner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Tooltip("The prefab of the tooltip to spawn.")] [SerializeField]
        GameObject tooltipPrefab = null;

        GameObject tooltip = null;

        /// <summary>
        /// Called when it is time to update the information on the tooltip
        /// prefab.
        /// </summary>
        /// <param name="tooltip">
        /// The spawned tooltip prefab for updating.
        /// </param>
        public abstract void UpdateTooltip(GameObject tooltip);

        /// <summary>
        /// Return when the tooltip spawner should be allowed to create a tooltip. 
        /// </summary>
        public abstract bool CanCreateTooltip();

        void OnDestroy()
        {
            ClearTooltip();
        }

        void OnDisable()
        {
            ClearTooltip();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            var parentCanvas = GetComponent<Canvas>();

            if (tooltip && !CanCreateTooltip())
            {
                ClearTooltip();
            }

            if (!tooltip && CanCreateTooltip())
            {
                tooltip = Instantiate(tooltipPrefab, parentCanvas.transform);
            }

            if (tooltip)
            {
                UpdateTooltip(tooltip);
                PositionTooltip();
            }
        }

        void PositionTooltip()
        {
            // Required to ensure corners are updated by positions elements
            Canvas.ForceUpdateCanvases();

            var tooltipCorners = new Vector3[4];
            tooltip.GetComponent<RectTransform>().GetWorldCorners(tooltipCorners);
            var slotCorners = new Vector3[4];
            GetComponent<RectTransform>().GetWorldCorners(slotCorners);

            bool below = transform.position.y > Screen.height / 2;
            bool right = transform.position.x < Screen.width / 2;

            int slotCorner = GetCornerIndex(below, right);
            int tooltipCorner = GetCornerIndex(!below, !right);

            tooltip.transform.position =
                slotCorners[slotCorner] - tooltipCorners[tooltipCorner] + tooltip.transform.position;
        }

        int GetCornerIndex(bool below, bool right)
        {
            if (below && !right) return 0;
            else if (!below && !right) return 1;
            else if (!below && right) return 2;
            else return 3;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            var slotCorners = new Vector3[4];
            GetComponent<RectTransform>().GetWorldCorners(slotCorners);
            Rect rect = new Rect(slotCorners[0], (slotCorners[2] - slotCorners[0]));
            if (rect.Contains(eventData.position)) return;
            ClearTooltip();
        }

        void ClearTooltip()
        {
            if (tooltip)
            {
                Destroy(tooltip.gameObject);
            }
        }
    }
}