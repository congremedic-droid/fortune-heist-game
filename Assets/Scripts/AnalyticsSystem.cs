using System;
using System.Collections.Generic;
using UnityEngine;

namespace FortuneHeist
{
    public enum AnalyticsProviderType
    {
        DebugLog,
        Buffered,
        UnityBackendPlaceholder
    }

    public interface IAnalyticsProvider
    {
        void Track(string eventName, Dictionary<string, object> parameters);
    }

    public class DebugLogAnalyticsProvider : IAnalyticsProvider
    {
        public void Track(string eventName, Dictionary<string, object> parameters)
        {
            string payload = parameters == null ? "{}" : MiniJson(parameters);
            Debug.Log($"[Analytics] {eventName} {payload}");
        }

        private static string MiniJson(Dictionary<string, object> map)
        {
            List<string> parts = new List<string>();
            foreach (KeyValuePair<string, object> item in map)
            {
                parts.Add($"\"{item.Key}\":\"{item.Value}\"");
            }

            return "{" + string.Join(",", parts) + "}";
        }
    }

    public class BufferedAnalyticsProvider : IAnalyticsProvider
    {
        private readonly List<string> buffer;

        public BufferedAnalyticsProvider(List<string> buffer)
        {
            this.buffer = buffer;
        }

        public void Track(string eventName, Dictionary<string, object> parameters)
        {
            buffer.Add($"{DateTime.UtcNow:O} | {eventName} | params={parameters?.Count ?? 0}");
            if (buffer.Count > 200)
            {
                buffer.RemoveAt(0);
            }
        }
    }

    public class UnityBackendAnalyticsPlaceholderProvider : IAnalyticsProvider
    {
        public void Track(string eventName, Dictionary<string, object> parameters)
        {
            // Placeholder hook to migrate to Firebase/Unity Analytics SDK.
            Debug.Log($"[AnalyticsPlaceholder] queued {eventName}");
        }
    }

    public class AnalyticsSystem : MonoBehaviour
    {
        [SerializeField] private AnalyticsProviderType providerType = AnalyticsProviderType.DebugLog;

        private readonly List<string> recentEvents = new List<string>();
        private IAnalyticsProvider provider;

        public IReadOnlyList<string> RecentEvents => recentEvents;

        private void Awake()
        {
            provider = BuildProvider();
        }

        public void Track(string eventName, Dictionary<string, object> parameters = null)
        {
            string line = $"[{DateTime.UtcNow:O}] {eventName}";
            recentEvents.Add(line);
            if (recentEvents.Count > 50)
            {
                recentEvents.RemoveAt(0);
            }

            provider?.Track(eventName, parameters ?? new Dictionary<string, object>());
        }

        private IAnalyticsProvider BuildProvider()
        {
            if (providerType == AnalyticsProviderType.Buffered)
            {
                return new BufferedAnalyticsProvider(recentEvents);
            }

            if (providerType == AnalyticsProviderType.UnityBackendPlaceholder)
            {
                return new UnityBackendAnalyticsPlaceholderProvider();
            }

            return new DebugLogAnalyticsProvider();
        }
    }
}
