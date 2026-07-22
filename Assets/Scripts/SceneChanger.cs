using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private Button button;
    public int targetSceneId;  
    public bool setTimeScaleTo1 = false;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ChangeScene);
    }

    void ChangeScene() {
        if (setTimeScaleTo1)
        {
            Time.timeScale = 1;
        }

        SceneManager.LoadScene(targetSceneId);
    }
}
