using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class MeshIOSystem : Singleton<MeshIOSystem>
{
    #region ATTRIBUTES

    public struct SingleModel
    {
        public string name;
        public Mesh model;
    }

    string storingDirectoryPath = "";
    string path = "";

    #endregion

    #region METHODS

    void Awake()
    {
        SetStoringDirectory("SavedModels");
        CreateNewStoringDirectory();        
    }

    public void SetStoringDirectory(string newDirectoryName)
    {
        if(!string.IsNullOrEmpty(newDirectoryName) && newDirectoryName != "Unity")
        {
            storingDirectoryPath = Path.Combine(Application.persistentDataPath, newDirectoryName);
        }        
    }

    public void CreateNewStoringDirectory()
    {
        if (!string.IsNullOrEmpty(storingDirectoryPath) && !Directory.Exists(storingDirectoryPath))
        {
            Directory.CreateDirectory(storingDirectoryPath);
        }        
    }

    public void DestroyStoringDirectory(string directoryName)
    {
        string dirPath = Path.Combine(Application.persistentDataPath, directoryName);

        if (Directory.Exists(dirPath))
        {
            Directory.Delete(dirPath, true);
        }
    }    

    /// <summary>
    /// Creates a binary dump of a mesh
    /// </summary>
    public void SaveMeshToDisk(string fileName, GameObject sculptedObj)
    {
        path = Path.Combine(storingDirectoryPath, fileName);

        Debug.Log(path);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(path, FileMode.Create);
        SerializableMeshInfo smi = new SerializableMeshInfo(sculptedObj.GetComponent<MeshFilter>().sharedMesh);
        bf.Serialize(fs, smi);
        fs.Close();
    }

    public void SaveMeshAsFBX(string fileName, GameObject sculptedObj)
    {
        path = Path.Combine(storingDirectoryPath, fileName);

        FBXExporter.ExportGameObjToFBX(sculptedObj, path);
    }

    /// <summary>
    /// Loads a mesh from a binary dump
    /// </summary>
    public Mesh LoadMeshFromDisk(string fileName)
    {
        path = Path.Combine(storingDirectoryPath, fileName);

        if (!File.Exists(path))
        {
            Debug.LogError("meshFile.dat file does not exist.");
            return null;
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(path, FileMode.Open);
        SerializableMeshInfo smi = (SerializableMeshInfo)bf.Deserialize(fs);        
        fs.Close();

        return smi.GetMesh();
    }

    public List<SingleModel> GetAllMeshesInCurrentDirectory()
    {
        List<SingleModel> models = new List<SingleModel>();

        DirectoryInfo info = new DirectoryInfo(storingDirectoryPath);
        FileInfo[] filesInfo = info.GetFiles();

        for (byte f = 0; f < filesInfo.Length; f++)
        {
            SingleModel single = new SingleModel
            {
                name = filesInfo[f].Name,
                model = LoadMeshFromDisk(filesInfo[f].Name)
            };

            models.Add(single);
        }

        return models;
    }

    public List<string> GetAllStoringDirectories()
    {
        List<string> galleries = new List<string>();

        string path = Application.persistentDataPath;
        DirectoryInfo i = new DirectoryInfo(path);
        DirectoryInfo[] directories = i.GetDirectories();

        for (byte d = 0; d < directories.Length; d++)
        {
            if (!directories[d].Name.Equals("Unity"))
            {
                galleries.Add(directories[d].Name);
            }            
        }

        return galleries;
    }

    #endregion
}
