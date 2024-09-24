using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public sealed class HandsDragDropEventHandler : MonoBehaviour {

    public ClockHandsEnum hand;

    private AnalogClock analogClock;

    private void Awake() {
        analogClock = GetComponentInParent<AnalogClock>();
        if (analogClock == null) {
            Debug.LogError("HandsDragDropEventHandler Awake: No AnalogClock was found!");
            return;
        }

        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entryDrag = new EventTrigger.Entry();
        EventTrigger.Entry entryDrop = new EventTrigger.Entry();
        EventTrigger.Entry entryPointerExit = new EventTrigger.Entry();
        entryDrag.eventID = EventTriggerType.Drag;
        entryDrop.eventID = EventTriggerType.Drop;
        entryPointerExit.eventID = EventTriggerType.PointerExit;
        entryDrag.callback.AddListener((data) => { OnDragDelegate((PointerEventData)data); });
        entryDrop.callback.AddListener((data) => { OnDropDelegate((PointerEventData)data); });
       entryPointerExit.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });
        trigger.triggers.Add(entryDrag);
        trigger.triggers.Add(entryDrop);
        trigger.triggers.Add(entryPointerExit);
    }

    private void OnDragDelegate(PointerEventData eventData) {
        analogClock.OnDragHand(hand, eventData);
        SwitchHandsColliders(false);
    }

    private void OnDropDelegate(PointerEventData eventData) { 
        analogClock.OnDropDragHand(hand, eventData);
        SwitchHandsColliders(true);
    }

    private void OnPointerExit(PointerEventData eventData) {
        if (eventData.dragging) {
            OnDropDelegate(eventData);
            eventData.pointerDrag = null;
        }
    }

    public void SwitchHandsColliders(bool isActive) {
        analogClock.SwitchHandsColliders(isActive, hand);
    }
}