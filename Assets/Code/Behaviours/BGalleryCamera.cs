using UnityEngine;

public class BGalleryCamera : MonoBehaviour
{
    #region ATTRIBUTES

    #endregion

    #region METHODS

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * Time.deltaTime * 2;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += Vector3.back * Time.deltaTime * 2;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * Time.deltaTime * 2;
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.forward * Time.deltaTime * 2;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.left * Time.deltaTime * 3);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.right * Time.deltaTime * 3);
        }
    }

    #endregion
}
