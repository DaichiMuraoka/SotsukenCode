using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataManager
{
    public static SaveData saveData = new SaveData();

    private const string saveKey = "savedata";

    public static void Save()
    {
        saveData.devices = new List<string>();
        foreach(var dev in Network.Cache.devices)
        {
            saveData.devices.Add(dev.Serialize());
        }
        PlayerPrefs.SetString(saveKey, JsonUtility.ToJson(saveData));
        Debug.Log("saved.");
    }

    public static void Load()
    {
        Network.Cache.devices = new List<Device>();
        string sd = PlayerPrefs.GetString(saveKey);
        saveData = JsonUtility.FromJson<SaveData>(sd);
        foreach(var jd in saveData.devices)
        {
            Device d = JsonUtility.FromJson<Device>(jd);
            d.Deserialize();
            Network.Cache.devices.Add(d);
        }
        Network.Cache.isHavingData = true;
        Debug.Log("loaded.");
    }

    public static bool IsSaveDataExist { get { return PlayerPrefs.HasKey(saveKey); } }

    [System.Serializable]
    public class SaveData
    {
        public List<string> devices = new List<string>();
    }
}
