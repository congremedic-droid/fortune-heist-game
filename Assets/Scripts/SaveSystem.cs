using System;
using System.IO;
using UnityEngine;

namespace FortuneHeist
{
    public class SaveSystem : MonoBehaviour
    {
        [SerializeField] private string saveFileName = "fortune_heist_save.json";

        public string SavePath => Path.Combine(Application.persistentDataPath, saveFileName);

        public void Save(GameStateData state)
        {
            if (state == null)
            {
                return;
            }

            state.LastSaveUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string json = JsonUtility.ToJson(state, true);
            File.WriteAllText(SavePath, json);
        }

        public bool HasSave()
        {
            return File.Exists(SavePath);
        }

        public GameStateData Load()
        {
            if (!HasSave())
            {
                return null;
            }

            string json = File.ReadAllText(SavePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return JsonUtility.FromJson<GameStateData>(json);
        }

        public void ClearSave()
        {
            if (HasSave())
            {
                File.Delete(SavePath);
            }
        }
    }
}
