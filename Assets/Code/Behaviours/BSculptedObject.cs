using System.Collections.Generic;
using UnityEngine;

public class BSculptedObject : MonoBehaviour
{
    #region ATTRIBUTES

    bool rotateY = true;

    public bool RotateY
    {
        get { return rotateY; }
        set { rotateY = value; }
    }

    sbyte horizontalSpeed = 0;

    public sbyte HorizontalSpeed
    {
        get { return horizontalSpeed; }
        set { horizontalSpeed = value; }
    }

    bool rotateX = true;

    public bool RotateX
    {
        get { return rotateX; }
        set { rotateX = value; }
    }

    sbyte forwardSpeed = 0;

    public sbyte ForwardSpeed
    {
        get { return forwardSpeed; }
        set { forwardSpeed = value; }
    }

    bool rotateZ = true;

    public bool RotateZ
    {
        get { return rotateZ; }
        set { rotateZ = value; }
    }

    sbyte lateralSpeed = 0;

    public sbyte LateralSpeed
    {
        get { return lateralSpeed; }
        set { lateralSpeed = value; }
    }

    float radius = 0;

    public float Radius
    {
        get { return radius; }
        set { radius = value; }
    }

    sbyte pullForce = -1;

    public sbyte PullForce
    {
        get { return pullForce; }
        set { pullForce = value; }
    }

    public enum FallOff : byte
    {
        Gauss,
        Linear,
        Needle
    }
    FallOff fallOffType = FallOff.Gauss;

    public FallOff FallOffType
    {
        get { return fallOffType; }
        set { fallOffType = value; }
    }

    [Range(0.15f, 0.9f)]
    public float refreshTime = 0.15f;
    float refreshCount;

    MeshFilter unappliedMesh;

    Dictionary<byte, GameObject> dicSculptors = new Dictionary<byte, GameObject>();

    Vector3[] sculptorsPositions;
    Vector3[] triggerOverlaps;

    Vector3[] contactPoints = new Vector3[0];
    float[] allSculptingRadius = new float[0];

    bool sculptorsColliding = false;

    #endregion

    #region METHODS

    void Awake()
    {
        refreshCount = refreshTime;
    }

    void OnTriggerEnter(Collider other)
    {
        byte id = other.GetComponent<BCollisionObject>().id;
        dicSculptors.Add(id, other.gameObject);
        sculptorsColliding = dicSculptors.Count > 0;
    }

    void OnTriggerExit(Collider other)
    {
        byte id = other.GetComponent<BCollisionObject>().id;
        dicSculptors.Remove(id);

        if (dicSculptors.Count == 0)
        {
            sculptorsColliding = false;
            RefreshMeshCollider();
        }
    }

    void TransformObject()
    {
        float eulerX = rotateX ? forwardSpeed * Time.deltaTime : 0;
        float eulerY = rotateY ? horizontalSpeed * Time.deltaTime : 0;
        float eulerZ = rotateZ ? lateralSpeed * Time.deltaTime : 0;

        Vector3 newEuler = new Vector3(eulerX, eulerY, eulerZ);

        transform.Rotate(newEuler);
    }

    void Update()
    {
        TransformObject();

        if (sculptorsColliding)
        {
            GetAllSculptorsPosData(out sculptorsPositions, out triggerOverlaps);

            PlaceContactPoints();

            MeshFilter meshFilter = GetComponent<MeshFilter>();

            refreshCount -= Time.deltaTime;

            if (refreshCount <= 0)
            {                  
                RefreshMeshCollider();
                unappliedMesh = meshFilter;
                refreshCount = refreshTime;
            }           

            float fullRadius = 0.5f;
            float[] contactRadius = new float[dicSculptors.Count];

            CalculateAllContactRadius(fullRadius, contactRadius);

            for (byte c = 0; c < contactPoints.Length; c++)
            {
                // Obtenemos el punto local de la malla donde se ve afectada
                Vector3 relativePoint = meshFilter.transform.InverseTransformPoint(contactPoints[c]);

                ModifyMesh(meshFilter.mesh, relativePoint, pullForce * 2 * Time.deltaTime, allSculptingRadius[c]);
            }
        }
    }

    void PlaceContactPoints()
    {
        byte count = (byte)dicSculptors.Count;

        contactPoints = new Vector3[count];

        for (byte c = 0; c < count; c++)
        {
            contactPoints[c] = GetComponent<Collider>().ClosestPointOnBounds(sculptorsPositions[c]);

            RaycastHit hitInfo;
            Ray ray = new Ray(sculptorsPositions[c], triggerOverlaps[c] - sculptorsPositions[c]);

            Physics.Raycast(ray, out hitInfo, 3);

            contactPoints[c] = hitInfo.point;
        }        
    }

    void GetAllSculptorsPosData(out Vector3[] sculptorPositions, out Vector3[] triggerOverlaps)
    {
        GameObject[] sculptors = new GameObject[dicSculptors.Count];
        dicSculptors.Values.CopyTo(sculptors, 0);

        sculptorPositions = new Vector3[sculptors.Length];
        triggerOverlaps = new Vector3[sculptors.Length];

        for (byte s = 0; s < sculptors.Length; s++)
        {
            sculptorPositions[s] = sculptors[s].transform.position;
            triggerOverlaps[s] = GetComponent<Collider>().ClosestPointOnBounds(sculptorPositions[s]);
        }
    }

    void CalculateAllContactRadius(float fullRadius, float[] contactRadius)
    {
        byte count = (byte)contactRadius.Length;

        allSculptingRadius = new float[count];
        
        for (byte a = 0; a < count; a++)
        {
            //fullRadius = dicSculptors[a].GetComponent<SphereCollider>().radius;
            float cRadius = Vector3.Distance(sculptorsPositions[a], contactPoints[a]);

            if (fullRadius > cRadius)
            {
                allSculptingRadius[a] = 1 - cRadius / fullRadius;
            }
            else
            {
                allSculptingRadius[a] = 1 - fullRadius / cRadius;
            }
        }        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (contactPoints.Length > 0)
        {
            for (byte c = 0; c < contactPoints.Length; c++)
            {
                Gizmos.DrawSphere(contactPoints[c], 0.05f);
            }
        }
    }

    void ModifyMesh(Mesh mesh, Vector3 position, float power, float inRadius)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        float sqrRadius = inRadius * inRadius;

        // Calculate averaged normal of all surrounding vertices
        Vector3 averageNormal = Vector3.zero;
                
        //Calculamos desde el punto de impacto la media normal de los vértices que se encuentran dentro del radio de efecto
        for (short i = 0; i < vertices.Length; i++)
        {
            float sqrMagnitude = (vertices[i] - position).sqrMagnitude;

            // Early out if too far away
            if (sqrMagnitude > sqrRadius)
                continue;

            float distance = Mathf.Sqrt(sqrMagnitude);
            float falloff = LinearFalloff(distance, inRadius);    //Siempre hace la función lineal
            averageNormal += falloff * normals[i];
        }        

        //Normalizamos nuestra normal
        averageNormal = averageNormal.normalized;

        // Modicamos los vertices según la normal calculada
        // Deform vertices along averaged normal
        for (short i = 0; i < vertices.Length; i++)
        {
            float sqrMagnitude = (vertices[i] - position).sqrMagnitude;

            // Early out if too far away
            if (sqrMagnitude > sqrRadius)
                continue;

            float distance = Mathf.Sqrt(sqrMagnitude);
            float falloff;

            switch (fallOffType)
            {
                case FallOff.Gauss:
                falloff = GaussFalloff(distance, inRadius);
                break;
                case FallOff.Needle:
                falloff = NeedleFalloff(distance, inRadius);
                break;
                default:
                falloff = LinearFalloff(distance, inRadius);
                break;
            }

            vertices[i] += averageNormal * falloff * power;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    public void RefreshMeshCollider()
    {
        if (unappliedMesh)
        {
            //Actualizamos la malla del mesh collider
            unappliedMesh.GetComponent<MeshCollider>().sharedMesh = unappliedMesh.mesh;
        }
        unappliedMesh = null;
    }

    #region FALLOFF_STATIC_METHODS

    static float LinearFalloff(float distance, float inRadius)
    {
        return Mathf.Clamp01(1.0f - distance / inRadius);
    }

    static float GaussFalloff(float distance, float inRadius)
    {
        return Mathf.Clamp01(Mathf.Pow(360, -Mathf.Pow(distance / inRadius, 2.5f) - 0.01f));
    }

    static float NeedleFalloff(float dist, float inRadius)
    {
        return -(dist * dist) / (inRadius * inRadius) + 1.0f;
    }

    #endregion

    #endregion
}