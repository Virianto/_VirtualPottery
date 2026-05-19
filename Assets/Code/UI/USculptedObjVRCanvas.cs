using Leap.Unity.Interaction;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class USculptedObjVRCanvas : MonoBehaviour
{
    #region ATTRIBUTES

    public BSculptedObject sculptedObject;

    [Header("Rotation attributes")]

    InteractionSlider xSlider;
    public GameObject xRotationSlider;

    InteractionSlider ySlider;
    public GameObject yRotationSlider;

    InteractionSlider zSlider;
    public GameObject zRotationSlider;

    [Header("Extrusion attributes")]

    public TextMesh forceValueText;

    [Header("IO attributes")]
    
    public TextMesh currentGalleryText;

    List<string> galleriesNames = new List<string>();
    byte currentGalleryIndex = 0;

    public TextMesh fileNameText;

    public List<Mesh> primitivesAvailable;

    byte currentPrimitive = 0;

    #endregion

    #region METHODS

    void Awake()
    {
        FillGallerySelector();

        xSlider = xRotationSlider.GetComponentInChildren<InteractionSlider>();
        ySlider = yRotationSlider.GetComponentInChildren<InteractionSlider>();
        zSlider = zRotationSlider.GetComponentInChildren<InteractionSlider>();
    }

    void FillGallerySelector()
    {
        galleriesNames = MeshIOSystem.Instance.GetAllStoringDirectories();
        currentGalleryText.text = galleriesNames[currentGalleryIndex];
    }

    public void BSelectNextGallery()
    {
        currentGalleryIndex = (byte)((currentGalleryIndex + 1) % galleriesNames.Count);
        currentGalleryText.text = galleriesNames[currentGalleryIndex];
    }

    public void BToggleDeformationForce()
    {
        sculptedObject.PullForce = (sbyte)(sculptedObject.PullForce == 1 ? -1 : 1);
        forceValueText.text = sculptedObject.PullForce == 1 ? "Push" : "Pull";
    }

    public void BOnXAxisSliderValueChanged()
    {
        sculptedObject.ForwardSpeed = (sbyte)xSlider.HorizontalSliderValue;       
    }

    public void BOnYAxisSliderValueChanged()
    {
        sculptedObject.HorizontalSpeed = (sbyte)ySlider.HorizontalSliderValue;
    }

    public void BOnZAxisSliderValueChanged()
    {
        sculptedObject.LateralSpeed = (sbyte)zSlider.HorizontalSliderValue;
    }

    public void BGoBackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void BToggleXRotation()
    {
        sculptedObject.RotateX = !sculptedObject.RotateX;
        xRotationSlider.SetActive(!xRotationSlider.activeInHierarchy);
    }

    public void BToggleYRotation()
    {
        sculptedObject.RotateY = !sculptedObject.RotateY;
        yRotationSlider.SetActive(!yRotationSlider.activeInHierarchy);
    }

    public void BToggleZRotation()
    {
        sculptedObject.RotateZ = !sculptedObject.RotateZ;
        zRotationSlider.SetActive(!zRotationSlider.activeInHierarchy);
    }

    public void BSaveMeshToDisk(bool saveAsFBX)
    {
        MeshIOSystem.Instance.SetStoringDirectory(galleriesNames[currentGalleryIndex]);

        if (saveAsFBX)
        {
            MeshIOSystem.Instance.SaveMeshAsFBX(fileNameText.text, sculptedObject.gameObject);
        }
        else
        {
            MeshIOSystem.Instance.SaveMeshToDisk(fileNameText.text, sculptedObject.gameObject);
        }
    }

    public void BLoadMeshFromDisk()
    {
        sculptedObject.gameObject.GetComponent<MeshFilter>().mesh = MeshIOSystem.Instance.LoadMeshFromDisk(fileNameText.text);
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
