using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenCharacterPanel : MonoBehaviour
{
    struct PanelInfo{
        public GameObject card;
        public GameObject panel;
        public Animator animator;
        public Image border;

        public PanelInfo(GameObject card, GameObject gameObject, Animator animator, Image border)
        {
            this.card = card;
            panel = gameObject;
            this.animator = animator;
            this.border = border;
        }
    }

    public GameObject panel;

    private static readonly int SlideInHash = Animator.StringToHash("SlideIn");
    private static readonly Color borderDefaultColor = new Color(0, 0, 0, 1);
    private static readonly Color borderHighlightColor = new Color(0.83137256f, 0.627451f, 0.09019608f, 1);
    private static List<PanelInfo> panels = new();
    private static GameObject lastOpened = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(OpenPanel);

        panel = gameObject.GetComponent<OpenCharacterPanel>().panel;
        panels.Add(new PanelInfo(gameObject, panel, panel.GetComponent<Animator>(), gameObject.transform.Find("Shadow/Background/Border").GetComponent<Image>()));
    }

    void OpenPanel()
    {
        if (lastOpened != gameObject)
        {
            PanelInfo panel = panels.Find(info => info.card == gameObject);
            if (panel.panel == null)
            {
                Debug.LogError("Nem sikerult olyan panelt talalni amihez tartozik ez a card elem. Keresett card: " + gameObject);
                return;
            }

            panel.panel.SetActive(true);
            panel.border.color = borderHighlightColor;
            panel.animator.SetTrigger(SlideInHash);

            if (lastOpened)
            {
                PanelInfo lastPanel = panels.Find(info => info.card == lastOpened);
                if (lastPanel.panel == null)
                {
                    Debug.LogError("Nem sikerult olyan panelt talalni amihez tartozik ez a card elem(Korabbi elem kereses soran tortent ez a hiba). Keresett card: " + gameObject);
                    return;
                }
                lastPanel.panel.SetActive(false);
                lastPanel.border.color = borderDefaultColor;
            }

            lastOpened = gameObject;
        }
    }
}
