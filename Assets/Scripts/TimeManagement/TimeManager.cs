using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Handles the backend of time itself!
 *
 * Has the ability to call events based on a certain time, this is done by doing something like this:
 * if (TimeManager.Instance.GetTimeEvent("NIGHT")) { WakeUpNPC(); }
 */

namespace TimeManagement {
    public class TimeManager : MonoBehaviour {
        private static TimeManager instance;
        [Header("Time Configuration")] public float timeScale = 48;
        public float secondsElapsed, minutesElapsed, hoursElapsed;

        private float _clock;

        public int dayNumber;
        public bool isDay = true;
        public bool enableTime = true;

        private Dictionary<string, (int hour, int minute)> timeEventMappings;

        public static TimeManager Instance {
            get {
                if (instance == null) instance = FindObjectOfType<TimeManager>();
                return instance;
            }
        }

        private void Start() {
            InitTimeMappings();
        }

        private void Update() {
            if (enableTime) {
                ElapseTime();
            }
        }

        public event Action<int> OnDayIncrement;

        private void ElapseTime() {
            secondsElapsed += timeScale * Time.deltaTime;

            while (secondsElapsed >= 60) {
                secondsElapsed -= 60;
                minutesElapsed++;
            }

            while (minutesElapsed >= 60) {
                minutesElapsed -= 60;
                hoursElapsed++;
            }

            while (hoursElapsed >= 24) {
                hoursElapsed -= 24;
                dayNumber++;

                OnDayIncrement?.Invoke(dayNumber);
            }

            if (hoursElapsed is >= 19 or < 6)
                isDay = false;
            else
                isDay = true;

            _clock = hoursElapsed;
        }

        private void InitTimeMappings() {
            timeEventMappings = new Dictionary<string, (int hour, int minute)> {
                // TIMEKEY    (HOUR, MINUTES)
                { "MIDNIGHT", (0, 0) },
                { "EARLY_MORNING", (6, 0) },
                { "MORNING", (9, 0) },
                { "NOON", (12, 0) },
                { "AFTERNOON", (15, 0) },
                { "EVENING", (18, 30) },
                { "NIGHT", (21, 0) },
            };
        }

        public bool GetTimeEvent(string timeName) {
            if (timeEventMappings.TryGetValue(timeName.ToUpper(), out var time))
                return hoursElapsed == time.hour && minutesElapsed == time.minute;

            return false;
        }

        public float[] GetCurrentTime() {
            var sec  = secondsElapsed;
            var min  = minutesElapsed;
            var hour = hoursElapsed;

            return new[] { sec, min, hour };
        }

        public float GetCurrentMinute() {
            return minutesElapsed;
        }

        public float GetCurrentHour() {
            return hoursElapsed;
        }

        public int GetCurrentDay() {
            return dayNumber;
        }

        public float GetCurrentTimeInHours() {
            return hoursElapsed + minutesElapsed / 60f + secondsElapsed / 3600f;
        }
    }
}