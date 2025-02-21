using System;
using System.IO;
using UnityEngine;

public class SaveFileHandler<T>
{
    public const string ENCRYPTED_FILE_EXTENSION = ".bin";
    public const string PLAIN_FILE_EXTENSION = ".json";

    public bool FileExists => File.Exists(FullPath);

    private readonly string _dirPath = "";
    private readonly string _fileName = "";
    private readonly bool _useEncryption = false;

    private string FullPath => Path.Combine(_dirPath, _fileName + (_useEncryption ? ENCRYPTED_FILE_EXTENSION : PLAIN_FILE_EXTENSION));

    public SaveFileHandler(string dirPath, string fileName, bool useEncryption)
    {
        _dirPath = dirPath;
        _fileName = fileName;
        _useEncryption = useEncryption;
    }

    /// <summary>
    /// Load the game's data from disk.
    /// </summary>
    public T Load()
    {
        T loadedData = default;

        if (FileExists)
        {
            try
            {
                // Load (encrypted) JSON from the file.
                string serializedData = "";

                using FileStream stream = new(FullPath, FileMode.Open);
                using StreamReader reader = new(stream);
                serializedData = reader.ReadToEnd();

                // if (_useEncryption)
                //     serializedData = Decrypt(serializedData);

                // Deserialize JSON back to game data.
                loadedData = JsonUtility.FromJson<T>(serializedData);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error occured when trying to load file at path: {FullPath}\n{e}");
            }
        }

        return loadedData;
    }

    /// <summary>
    /// Save the game's data to disk.
    /// </summary>
    public void Save(T data)
    {
        try
        {
            // Create directory if it doesn't exist.
            Directory.CreateDirectory(Path.GetDirectoryName(FullPath));

            // Serialize GameData to JSON.
            string serializedData = JsonUtility.ToJson(data, !_useEncryption);

            // if (_useEncryption)
            //     serializedData = Encrypt(serializedData);

            // Write (encrypted) JSON data to the file.
            using FileStream stream = new(FullPath, FileMode.Create);
            using StreamWriter writer = new(stream);
            writer.Write(serializedData);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error occured when trying to save file at path: {FullPath}\n{e}");
        }
    }

    /// <summary>
    /// Delete the game data.
    /// </summary>
    public void Delete()
    {
        if (FileExists)
            File.Delete(FullPath);
    }
}
