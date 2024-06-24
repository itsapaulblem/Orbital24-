using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    private static CoroutineManager instance;

    public static CoroutineManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CoroutineManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(CoroutineManager).Name;
                    instance = obj.AddComponent<CoroutineManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
