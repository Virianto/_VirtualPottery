using UnityEngine;

public class BCollisionObject : MonoBehaviour
{
    #region ATTRIBUTES

    public byte id;

    Vector3 lastPosition;
    Vector3 direction;

    Vector3 contactPoint = Vector3.zero;

    #endregion

    #region METHODS

    public void Start()
    {
        lastPosition = transform.position;
    }

    public void Update()
    {
        direction = transform.position - lastPosition;
    }

    public Vector3 GetDirectionVector()
    {
        return direction;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(contactPoint, 0.05f);
    }
    #endregion
}
