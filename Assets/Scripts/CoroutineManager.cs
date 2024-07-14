using UnityEngine;

/**
 * Singleton class that manages coroutines.
 * This class ensures that there is only one instance of the coroutine manager throughout the game.
 */
public class CoroutineManager : MonoBehaviour
{
    private static CoroutineManager instance;

    /**
     * Gets the instance of the coroutine manager.
     * If the instance is null, it creates a new one.
     * 
     * Example:
     * ```csharp
     * CoroutineManager manager = CoroutineManager.Instance;
     * ```
     */
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

    /**
     * Called when the script is initialized.
     * Sets the instance to this script if it's the first instance, or destroys the game object if it's not.
     */
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