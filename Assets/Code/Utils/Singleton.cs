using UnityEngine;
 
/// <summary>
/// Be aware this will not prevent a non singleton constructor
/// such as `T myT = new T();`
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	#region ATTRIBUTES
	
	static T _instance;
 
	static object _lock = new object();

    #endregion

    #region METHODS
    
    public static T Instance
	{
		get
		{ 
			lock(_lock)
			{
				if (_instance == null)
				{
					_instance = (T) FindObjectOfType(typeof(T));

					if (_instance == null)
					{
						GameObject singleton = new GameObject();
						_instance = singleton.AddComponent<T>();
						singleton.name = typeof(T).ToString();
                        DontDestroyOnLoad(singleton);
					} 
				} 
				return _instance;
			}
		}
	}
    
	#endregion
}