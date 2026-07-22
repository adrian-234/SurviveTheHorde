using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectOption : MonoBehaviour
{
    private enum SelectOptions
    {
        character,
        weapon
    }

    private Button button;
    private DataManager dataManager;

    [SerializeField]
    private SelectOptions optionType; 
    public int id;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Select);
        
        dataManager = DataManager.getInstance();
    }

    void Select() {
        if (optionType == SelectOptions.character)
        {
            dataManager.selectedCharacterId = id;

            SceneManager.LoadScene(5); // WeaponSelectorScene == 5
        } else if (optionType == SelectOptions.weapon)
        {
            dataManager.selectedWeaponId = id;

            SceneManager.LoadScene(3); // GameScene == 3
        }
        
    }
}
