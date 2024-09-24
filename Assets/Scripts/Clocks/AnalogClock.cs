using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

[Flags]
public enum ClockHandsEnum {
    seconds, 
    minutes, 
    hours
}

public class AnalogClock : BaseClock {

    [SerializeField] private RectTransform hoursHand;
    [SerializeField] private RectTransform minutesHand;
    [SerializeField] private RectTransform secondsHand;
    private Collider2D hoursHandCollider;
    private Collider2D minutesHandCollider;
    private Collider2D secondsHandCollider;

    private bool isInManualMode = false;

    private const float handStepSmall = 360 / 60;
    private const float handStepBig = 360 / 12;
    private readonly Vector3 stepVector = new Vector3(0, 0, handStepSmall);

    private void OnDestroy() {
        secondsHand?.DOKill();
        minutesHand?.DOKill();
        hoursHand?.DOKill();
    }

    protected override void SetClockByDateTime() {
        TimeSpan timeSpan = dateTime.TimeOfDay;

        if (secondsHand != null) 
            secondsHand.rotation = Quaternion.Euler(0, 0, -dateTime.Second * handStepSmall);
        else
            Debug.LogError("secondsHand is null!");

        if (minutesHand != null)
            minutesHand.rotation = Quaternion.Euler(0, 0, -(float)timeSpan.TotalMinutes * handStepSmall);
        else
            Debug.LogError("minutesHand is null!");

        if (hoursHand != null)
            hoursHand.rotation = Quaternion.Euler(0, 0, -(float)timeSpan.TotalHours * handStepBig);
        else
            Debug.LogError("hoursHand is null!");
    }

    public override void SwitchManualMode(bool isActive) {
        isInManualMode = isActive;
    }

    public override void Tick() {
        base.Tick();
        secondsHand?.DORotate(secondsHand.rotation.eulerAngles - stepVector, 0.3f, RotateMode.Fast);
        minutesHand?.DORotate(minutesHand.rotation.eulerAngles - stepVector / 60, 0.1f, RotateMode.Fast);
        hoursHand?.DORotate(hoursHand.rotation.eulerAngles - stepVector / 60 / 12, 0.1f, RotateMode.Fast);
    }

    public override void SyncClock(BaseClock baseClock) {
        base.SyncClock(baseClock);
        SetClockByDateTime();
    }

    public void OnDragHand(ClockHandsEnum handsEnum, PointerEventData data) {
        if (!isInManualMode || data.pointerDrag == null)
            return;

        RotateArrow(data);
    }

    public void OnDropDragHand(ClockHandsEnum handsEnum, PointerEventData data) {
        if (!isInManualMode)
            return;

        CalculateTimeFromRotation(handsEnum);
        SetClockByDateTime();
        SyncAllClocksWithThis();

        Debug.Log("DropDrag");
    }

    public void SwitchHandsColliders(bool isActive, ClockHandsEnum currentHand) {
        switch (currentHand) {
            case ClockHandsEnum.seconds:
                if (minutesHandCollider == null)
                    minutesHandCollider = minutesHand.GetComponent<Collider2D>();

                minutesHandCollider.enabled = isActive;

                if (hoursHandCollider == null)
                    hoursHandCollider = hoursHand.GetComponent<Collider2D>();

                hoursHandCollider.enabled = isActive;
                break;
            case ClockHandsEnum.minutes:
                if (secondsHandCollider == null)
                    secondsHandCollider = secondsHand.GetComponent<Collider2D>();

                secondsHandCollider.enabled = isActive;

                if (hoursHandCollider == null)
                    hoursHandCollider = hoursHand.GetComponent<Collider2D>();

                hoursHandCollider.enabled = isActive;
                break;
            case ClockHandsEnum.hours:
                if (secondsHandCollider == null)
                    secondsHandCollider = secondsHand.GetComponent<Collider2D>();

                secondsHandCollider.enabled = isActive;

                if (minutesHandCollider == null)
                    minutesHandCollider = minutesHand.GetComponent<Collider2D>();

                minutesHandCollider.enabled = isActive;
                break;
        }
    }

    private void CalculateTimeFromRotation(ClockHandsEnum handsEnum) {
        float degrees;
        int value;
        int difference;
        switch (handsEnum) {
            case ClockHandsEnum.seconds:
                degrees = 360 - secondsHand.rotation.eulerAngles.z;
                value = Mathf.RoundToInt(degrees / handStepSmall);
                difference = value - dateTime.Second;
                dateTime = dateTime.AddSeconds(difference);
                break;
            case ClockHandsEnum.minutes:
                degrees = 360 - minutesHand.rotation.eulerAngles.z;
                value = Mathf.RoundToInt( degrees / handStepSmall);
                difference = value - dateTime.Minute;
                dateTime = dateTime.AddMinutes(difference);
                break;
            case ClockHandsEnum.hours:
                degrees = 360 - hoursHand.rotation.eulerAngles.z;
                value = Mathf.RoundToInt(degrees / handStepBig);
                difference = value - dateTime.Hour;
                dateTime = dateTime.AddHours(difference);
                break;
        }
    }

    private void RotateArrow(PointerEventData data) {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 direction = new Vector2(mousePosition.x - data.pointerDrag.transform.position.x, mousePosition.y - data.pointerDrag.transform.position.y);

        data.pointerDrag.transform.up = direction;
    }
}