using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public sealed class GlobalDateTimeRequester {

    private const string WORLD_TIME_API = "https://worldtimeapi.org/api/ip";

    public static Action<DateTime> DateTimeReceived;

    private struct TimeData {
        public string datetime;
    }

    public static IEnumerator TimeRequestCoroutine() {

        UnityWebRequest webRequest = UnityWebRequest.Get(WORLD_TIME_API);
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError) {
            Debug.LogError("TimeRequestCoroutine RequestError: " + webRequest.error);
        } else {

            try {
                TimeData timeData = JsonUtility.FromJson<TimeData>(webRequest.downloadHandler.text);

                DateTime dateTime = ParseDateTime(timeData.datetime);

                DateTimeReceived?.Invoke(dateTime);

                Debug.Log(dateTime.ToString("HH:mm:ss"));
            } catch (Exception e) {
                Debug.LogError("TimeRequestCoroutine ParseError: " + e);
            }
        }
    }

    private static DateTime ParseDateTime(string dateTime) {
        string date = Regex.Match(dateTime, @"^\d{4}-\d{2}-\d{2}").Value;
        string time = Regex.Match(dateTime, @"\d{2}:\d{2}:\d{2}").Value;

        return DateTime.Parse(string.Format("{0} {1}", date, time));
    }
}
