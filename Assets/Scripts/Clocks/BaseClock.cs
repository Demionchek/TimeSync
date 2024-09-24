using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseClock : MonoBehaviour {

    protected DateTime dateTime;
    protected List<BaseClock> clockList = new List<BaseClock>();

    protected void Awake() {
        clockList = FindObjectsOfType<BaseClock>()
            .Where(bc => bc != this)
            .ToList();
    }

    public virtual void SwitchManualMode(bool isActive) { }

    public virtual void SetDateTime(DateTime dateTime) {
        this.dateTime = dateTime;
        SetClockByDateTime();
    }

    public virtual void Tick() {
        dateTime = dateTime.AddSeconds(1);
    }

    public virtual void SyncClock(BaseClock baseClock) { dateTime = baseClock.dateTime; }

    protected void SyncAllClocksWithThis() {
        foreach (BaseClock clock in clockList) {
            clock.SyncClock(this);
        }
    }

    protected virtual void SetClockByDateTime() { }
}