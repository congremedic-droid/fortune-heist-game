using System;
using System.Collections.Generic;
using UnityEngine;

namespace FortuneHeist
{
    public class AnalyticsSystem : MonoBehaviour
    {
        [SerializeField] private bool logToConsole = true;

        private readonly List<string> recentEvents = new List<string>();
        public IReadOnlyList<string> RecentEvents => recentEvents;

        public void Track(string eventName, Dictionary<string, object> parameters = null)
        {
            string payload = parameters == null ? "{}" : MiniJson(parameters);
            string line = $"[{DateTime.UtcNow:O}] {eventName} {payload}";

            recentEvents.Add(line);
            if (recentEvents.Count > 50)
            {
                recentEvents.RemoveAt(0);
            }

            if (logToConsole)
            {
                Debug.Log($"[Analytics] {line}");
            }
        }

        private string MiniJson(Dictionary<string, object> map)
        {
            List<string> parts = new List<string>();
            foreach (KeyValuePair<string, object> item in map)
            {
                parts.Add($"\"{item.Key}\":\"{item.Value}\"");
            }

            return "{" + string.Join(",", parts) + "}";
        }
    }
}
