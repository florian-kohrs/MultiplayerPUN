using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class ItemDisplayer
{

    protected List<InventorySlot> items;

    private static InventorySlot GetSlot(int index, List<InventorySlot> slots, GameObject slotPrefab, Transform slotParent)
    {

        InventorySlot slot;
        if (index >= slots.Count)
        {
            slot = GameObject.Instantiate(slotPrefab).GetComponent<InventorySlot>();
            slots.Add(slot);
            slot.transform.SetParent(slotParent, false);
        }
        else
        {
            slot = slots[index];
            slot.gameObject.SetActive(true);
        }
        return slot;
    }

    public static void Fill<T>(Rect rect, GameObject slotPrefab,
        Transform slotParent, List<T> items,
        List<InventorySlot> slots, int inventorySlotSize, int inventorySlotSpace,
        System.Action<T> clickEvent, System.Action<T> hoverEvent,
        System.Action<T> hoverExitEvent, System.Func<T, bool> filter = null)
        where T : MapOccupationObject
    {
        int counter = 0;
        InventorySlot s;
        Vector2 uiSize = rect.max - rect.min;
        int slotDistance = inventorySlotSize + inventorySlotSpace;
        int startX = inventorySlotSpace + inventorySlotSize / 2;
        int currentX = startX;
        int currentY = currentX;
        int uiXSize = (int)uiSize.x + inventorySlotSize / 2;
        foreach (T item in items)
        {
            if (filter != null && filter(item))
                continue;

            s = GetSlot(counter, slots, slotPrefab, slotParent);
            s.gameObject.SetActive(true);
            Transform prefab = Object.Instantiate(item.prefab, slotParent).transform;
            Button btn = s.GetOrAddComponent<Button>();

            ///remove previous click events & add new one
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(delegate { clickEvent?.Invoke(item); });

            RectTransform t = s.GetComponent<RectTransform>();
            t.anchorMax = new Vector2(0, 1);
            t.anchorMin = new Vector2(0, 1);
            t.anchoredPosition = new Vector2(currentX, -currentY);
            prefab.transform.position = t.position;
            prefab.transform.Translate(0, 0, -1);
            currentX += slotDistance;
            if (currentX + inventorySlotSize > uiXSize)
            {
                currentX = startX;
                currentY += slotDistance;
            }

            t.GetOrAddComponent<BoxCollider2D>();

            ///add hover event
            if (hoverEvent != null)
            {
                EventTrigger trigger = btn.GetOrAddComponent<EventTrigger>();
                trigger.triggers.Clear();
                Entry entry = new Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener(eventData => { hoverEvent(item); });
                trigger.triggers.Add(entry);

                entry = new Entry();
                entry.eventID = EventTriggerType.PointerExit;
                entry.callback.AddListener(e => hoverExitEvent(item));
                trigger.triggers.Add(entry);
            }

            counter++;
            
        }

        ///disable all unused slots
        while (counter < slots.Count)
        {
            slots[counter].gameObject.SetActive(false);
            counter++;
        }
    }


}
