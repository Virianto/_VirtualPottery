using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class USculptedObjCanvas : MonoBehaviour
{
    #region ATTRIBUTES

    public BSculptedObject sculptedObject;

    [Header("Rotation attributes")]

    public Slider yRotationSlider;
    public Slider xRotationSlider;
    public Slider zRotationSlider;

    [Header("Extrusion attributes")]

    public Toggle meshDeformationToggle;
    public Text pullForceValueText;

    [Header("IO attributes")]

    public Dropdown gallerySelector;

    public InputField fileNameInput;

    public List<Mesh> primitivesAvailable;

    byte currentPrimitive = 0;
    
    #endregion

    #region METHODS

    void Awake()
    {
        FillGallerySelector();

        xRotationSlider.onValueChanged.AddListener((value) => { sculptedObject.ForwardSpeed = (sbyte)value; });
        yRotationSlider.onValueChanged.AddListener((value) => { sculptedObject.HorizontalSpeed = (sbyte)value; });
        zRotationSlider.onValueChanged.AddListener((value) => { sculptedObject.LateralSpeed = (sbyte)value; });

        meshDeformationToggle.onValueChanged.AddListener((value) => 
        {            
            sculptedObject.PullForce = (sbyte)(value ? 1 : -1);
            pullForceValueText.text = value ? "Pull" : "Push";
        });
    }

    void FillGallerySelector()
    {
        List<string> galleriesNames = MeshIOSystem.Instance.GetAllStoringDirectories();

        gallerySelector.AddOptions(galleriesNames);
    }

    public void BGoBackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void BToggleXRotation()
    {
        sculptedObject.RotateX = !sculptedObject.RotateX;
    }

    public void BToggleYRotation()
    {
        sculptedObject.RotateY = !sculptedObject.RotateY;
    }

    public void BToggleZRotation()
    {
        sculptedObject.RotateZ = !sculptedObject.RotateZ;
    }

    public void BSaveMeshToDisk(bool saveAsFBX)
    {
        MeshIOSystem.Instance.SetStoringDirectory(gallerySelector.options[gallerySelector.value].text);

        if(saveAsFBX)
        {
            MeshIOSystem.Instance.SaveMeshAsFBX(fileNameInput.text, sculptedObject.gameObject);
        }
        else
        {
            MeshIOSystem.Instance.SaveMeshToDisk(fileNameInput.text, sculptedObject.gameObject);
        }
    }

    public void BLoadMeshFromDisk()
    {
        sculptedObject.gameObject.GetComponent<MeshFilter>().mesh = MeshIOSystem.Instance.LoadMeshFromDisk(fileNameInput.text);
        sculptedObject.RefreshMeshCollider();
    }

    public void BLoadNextPrimitiveMesh()
    {
        currentPrimitive += 1;
        Mesh newMesh = primitivesAvailable[currentPrimitive % primitivesAvailable.Count];
        sculptedObject.gameObject.GetComponent<MeshFilter>().mesh = newMesh;
        sculptedObject.RefreshMeshCollider();
    }

    #endregion
}
