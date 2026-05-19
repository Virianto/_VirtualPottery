using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UGalleryCanvas : MonoBehaviour
{
    #region ATTRIBUTES

    public InputField newGalleryName;

    public GameObject singleGalleryPanel;
    
    #endregion

    #region METHODS

    void Awake()
    {
        FillGalleriesList();
    }

    public void BCreateNewGallery()
    {
        MeshIOSystem.Instance.SetStoringDirectory(newGalleryName.text);
        MeshIOSystem.Instance.CreateNewStoringDirectory();
        CreateNewSingleGalleryPanel(newGalleryName.text);
    }

    public void BGoBackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
       
    void FillGalleriesList()
    {
        List<string> allGalleries = MeshIOSystem.Instance.GetAllStoringDirectories();

        for(byte a = 0; a < allGalleries.Count; a++)
        {
            CreateNewSingleGalleryPanel(allGalleries[a]);
        }
    }

    void CreateNewSingleGalleryPanel(string galleryName)
    {
        GameObject newSingleGalleryPanel = Instantiate(singleGalleryPanel, singleGalleryPanel.transform.parent);

        newSingleGalleryPanel.GetComponent<CSingleGalleryPanel>().InitializePanel(galleryName);
    }

    #endregion
}
