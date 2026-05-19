using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSingleGalleryPanel : MonoBehaviour
{
    #region ATTRIBUTES

    public GameObject singleExhibitor;

    public Button bLoadGallery;
    public Button bDeleteGallery;

    public Text galleryName;

    #endregion

    #region METHODS

    public void InitializePanel(string galleryName)
    {
        this.galleryName.text = string.Format("Load {0}", galleryName);

        bLoadGallery.onClick.AddListener(()=> 
        {
            MeshIOSystem.Instance.SetStoringDirectory(galleryName);
            List<MeshIOSystem.SingleModel> allModels = MeshIOSystem.Instance.GetAllMeshesInCurrentDirectory();
            for(byte a = 0; a < allModels.Count; a++)
            {
                byte x = a;
                GameObject newExhibitor = Instantiate(singleExhibitor);
                newExhibitor.name = string.Format("{0}({1})", allModels[a].name, x);
                newExhibitor.GetComponent<CSingleExhibitor>().InitializeExhibitor(allModels[a].name, allModels[a].model, x);
            }
        });
        
        bDeleteGallery.onClick.AddListener(() =>
        {
            MeshIOSystem.Instance.DestroyStoringDirectory(galleryName);
            gameObject.SetActive(false);
        });

        gameObject.SetActive(true);
    }

    #endregion
}
