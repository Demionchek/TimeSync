using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DigitalClock : BaseClock {

    [SerializeField] private InputField hoursInputField;
    [SerializeField] private InputField minutesInputField;
    [SerializeField] private InputField secondsInputField;

    private const int MAX_HOURS = 23;
    private const int MAX_MINS = 59;

    protected override void SetClockByDateTime() {
        hoursInputField.text = dateTime.ToString("HH");
        minutesInputField.text = dateTime.ToString("mm");
        secondsInputField.text = dateTime.ToString("ss");
    }

    public override void Tick() {
        base.Tick();
        secondsInputField.text = dateTime.ToString("ss");
        if (dateTime.Second == 0) {
            minutesInputField.text = dateTime.ToString("mm");
            if (dateTime.Minute == 0) {
                hoursInputField.text = dateTime.ToString("HH");
            }
        }
    }

    public override void SyncClock(BaseClock baseClock) {
        base.SyncClock(baseClock);
        SetClockByDateTime();
    }

    public override void SwitchManualMode(bool isActive) {
        secondsInputField.interactable = isActive;
        minutesInputField.interactable = isActive;
        hoursInputField.interactable = isActive;
    }

    [UsedImplicitly]
    public void OnSecondsValueChanged() {
        int newValue = 0;
        try {
            if (secondsInputField.text != "") {
                newValue = int.Parse(secondsInputField.text);
            }
        } catch (Exception e) {
            Debug.LogError("OnSecondsValueChanged : " + e);
        }
        newValue = Mathf.Clamp(newValue, 0, MAX_MINS);
        secondsInputField.text = newValue.ToString();
        int differenceValue = newValue - dateTime.Second;
        dateTime = dateTime.AddSeconds(differenceValue);
        SyncAllClocksWithThis();
    }

    [UsedImplicitly]
    public void OnMinutesValueChanged() {
        int newValue = 0;
        try {
            if (minutesInputField.text != "") { 
                newValue = int.Parse(minutesInputField.text);
            }
        } catch (Exception e) {
            Debug.LogError("OnMinutesValueChanged : " + e);
        }
        newValue = Mathf.Clamp(newValue, 0, MAX_MINS);
        minutesInputField.text = newValue.ToString();
        int differenceValue = newValue - dateTime.Minute;
        dateTime = dateTime.AddMinutes(differenceValue);
        SyncAllClocksWithThis();
    }

    [UsedImplicitly]
    public void OnHoursValueChanged() {
        int newValue = 0;
        try {
            if (hoursInputField.text != "") {
                newValue = int.Parse(hoursInputField.text);
            }
        } catch (Exception e ) {
            Debug.LogError("OnHoursValueChanged : " + e );
        }
        newValue = Mathf.Clamp(newValue, 0, MAX_HOURS);
        hoursInputField.text = newValue.ToString();
        int differenceValue = newValue - dateTime.Hour;
        dateTime = dateTime.AddHours(differenceValue);
        SyncAllClocksWithThis();
    }
}