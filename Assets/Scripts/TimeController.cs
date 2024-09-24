using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class TimeController : MonoBehaviour {

    private List<BaseClock> baseClocks = new List<BaseClock>();
    private bool isTimeFlowOn = false;

    public void Awake() {
        baseClocks = FindObjectsOfType<BaseClock>().ToList();

        GlobalDateTimeRequester.DateTimeReceived += SetDateTime;
        InitTimeRequest();
    }

    private void Start() {
        isTimeFlowOn = true;
        StartCoroutine(TimeFlowCoroutine());
        StartCoroutine(TimeSyncHourly());
    }

    private void OnDestroy() {
        GlobalDateTimeRequester.DateTimeReceived -= SetDateTime;
    }

    public void InitTimeRequest() {
        StartCoroutine(GlobalDateTimeRequester.TimeRequestCoroutine());
    }

    public void SetDateTime(DateTime dateTime) {
        foreach (var clock in baseClocks) {
            clock.SetDateTime(dateTime);
        }
    }

    public void SetSaveSwitcher() {
        foreach (var clock in baseClocks) {
            clock.SwitchManualMode(isTimeFlowOn);
        }
        isTimeFlowOn = !isTimeFlowOn;
    }

    private IEnumerator TimeFlowCoroutine() {
        while (true) {
            yield return new WaitForSeconds(1);

            if (isTimeFlowOn) {
                foreach (var clock in baseClocks) {
                    clock.Tick();
                }
            }
        }
    }

    private IEnumerator TimeSyncHourly() {
        while (true) {
            yield return new WaitForSeconds(3600);
            InitTimeRequest();
        }
    }
}