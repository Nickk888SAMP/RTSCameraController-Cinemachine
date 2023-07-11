using System;
using System.IO;
using Newtonsoft.Json;
using Unity.Services.Core.Internal;
using UnityEngine;
using NotNull = JetBrains.Annotations.NotNullAttribute;

namespace Unity.Services.Core.Telemetry.Internal
{
    abstract class FileCachePersister
    {
        internal static bool IsAvailableFor(RuntimePlatform platform)
        {
            return !string.IsNullOrEmpty(GetPersistentDataPathFor(platform));
        }

        internal static string GetPersistentDataPathFor(RuntimePlatform platform)
        {
            // Application.persistentDataPath has side effects on Switch so it shouldn't be called.
            if (platform == RuntimePlatform.Switch)
                return string.Empty;

            return Application.persistentDataPath;
        }
    }

    class FileCachePersister<TPayload> : FileCachePersister, ICachePersister<TPayload>
        where TPayload : ITelemetryPayload
    {
        public FileCachePersister(string fileName)
        {
            FilePath = Path.Combine(GetPersistentDataPathFor(Application.platform), fileName);
        }

        public string FilePath { get; }

        public bool CanPersist { get; } = IsAvailableFor(Application.platform);

        public void Persist(CachedPayload<TPayload> cache)
        {
            var serializedEvents = JsonConvert.SerializeObject(cache);
            File.WriteAllText(FilePath, serializedEvents);
        }

        public bool TryFetch(out CachedPayload<TPayload> persistedCache)
        {
            if (!File.Exists(FilePath))
            {
                persistedCache = default;
                return false;
            }

            try
            {
                var rawPersistedCache = File.ReadAllText(FilePath);
                persistedCache = JsonConvert.DeserializeObject<CachedPayload<TPayload>>(rawPersistedCache);
                return true;
            }
            catch (Exception e)
            {
                CoreLogger.LogException(e);
                persistedCache = default;
                return false;
            }
        }

        public void Delete()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }
    }
}
