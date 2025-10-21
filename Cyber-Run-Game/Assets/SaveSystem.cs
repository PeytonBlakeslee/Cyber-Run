using UnityEngine;
using System.IO;

public static class SaveSystem
{
    // Where files are stored and the extension we use
    public static readonly string SAVE_FOLDER = Application.persistentDataPath + "/saves/";
    public static readonly string FILE_EXT = ".json";

    
    public static void Save(string fileName, string dataToSave)
    {
        // make sure the folder exists
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
        }

        // write text to: .../saves/<fileName>.json
        File.WriteAllText(SAVE_FOLDER + fileName + FILE_EXT, dataToSave);
    }

    public static string Load(string fileName)
    {
        string fileLoc = SAVE_FOLDER + fileName + FILE_EXT;

        // if file exists, read and return; otherwise return null
        if (File.Exists(fileLoc))
        {
            string loadedData = File.ReadAllText(fileLoc);
            return loadedData;
        }
        else
        {
            return null;
        }
    }
}