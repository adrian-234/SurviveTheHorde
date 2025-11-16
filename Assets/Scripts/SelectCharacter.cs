using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectCharacter : MonoBehaviour
{
    private Button button;
    private DataManager dataManager;

    public int characterId;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Select);
        
        dataManager = DataManager.getInstance();
    }

    void Select() {
        dataManager.selectedCharacterId = characterId;

        SceneManager.LoadScene(3); // GameScene == 3
    }
}
