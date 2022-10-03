using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveData(GameManager gameManager)
    {
        string path = Application.persistentDataPath + "/data.graphicless";
        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = new GameData(gameManager);
        
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, data);

        stream.Close();
    }

    public static GameData LoadData()
    {
        string path = Application.persistentDataPath + "/data.graphicless";
        if(File.Exists(path))
        {
            FileStream  stream = new FileStream(path, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Data can't find at " + path);
            return null;
        }
        
    }
}
