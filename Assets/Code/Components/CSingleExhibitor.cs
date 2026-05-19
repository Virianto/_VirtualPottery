using UnityEngine;

public class CSingleExhibitor : MonoBehaviour
{
    #region ATTRIBUTES

    public GameObject model;

    public TextMesh authorName;

    #endregion

    #region METHODS

    public void InitializeExhibitor(string author, Mesh mesh, byte posIndex)
    {
        sbyte posX = posIndex % 2 == 0 ? (sbyte)- (posIndex % 7) : (sbyte)(posIndex % 7 + 1);

        byte posZ = (byte)(posIndex / 7 * 2);
        
        transform.position = new Vector3(posX, 0, posZ);

        authorName.text = author;
        model.GetComponent<MeshFilter>().sharedMesh = mesh;

        gameObject.SetActive(true);
    }

    #endregion
}
