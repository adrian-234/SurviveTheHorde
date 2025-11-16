using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static DataManager instance;
    public int selectedCharacterId;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static DataManager getInstance() {
        return instance;
    }
}
