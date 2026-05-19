using UnityEngine;
using UnityEngine.SceneManagement;

public class UStartCanvas : MonoBehaviour
{
    MeshIOSystem meshIOSystem;

    void Awake()
    {
        meshIOSystem = MeshIOSystem.Instance;
    }

    public void BNavigateToScene(int destiny)
    {
        SceneManager.LoadScene(destiny);
    }
}
