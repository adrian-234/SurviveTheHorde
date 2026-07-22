using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenCharacterPanel : MonoBehaviour
{
    [Serializable]
    private class CardPanelPair{
        public GameObject card;
        public GameObject panel;
        [NonSerialized]
        public RectTransform rectTransform;
        [NonSerialized]
        public Image border;
        [NonSerialized]
        public float slideSpeed;

        public void InitializeFields(float slideInTime)
        {
            rectTransform = panel.GetComponent<RectTransform>();
            border = card.transform.Find("Shadow/Background/Border").GetComponent<Image>();
            slideSpeed = rectTransform.sizeDelta.x / slideInTime;
        }
    }

    [SerializeField]
    private List<CardPanelPair> cardPanelPairs;
    [SerializeField]
    private Color borderDefaultColor;
    [SerializeField]
    private Color borderHighlightColor;
    [SerializeField]
    private float slideInTime;
    private bool animate = false;
    private int lastOpened = -1;

    void Start()
    {
        foreach(CardPanelPair pair in cardPanelPairs)
        {
            pair.InitializeFields(slideInTime);
        }
    }

    void Update()
    {
        if (animate)
        {
            CardPanelPair lastOpenedPair = cardPanelPairs[lastOpened];
            if (lastOpenedPair.rectTransform.anchoredPosition.x * -1 <= lastOpenedPair.rectTransform.sizeDelta.x)
            {
                lastOpenedPair.panel.transform.Translate(lastOpenedPair.slideSpeed * Time.deltaTime * Vector3.left);
            } else
            {
                animate = false;
                lastOpenedPair.rectTransform.anchoredPosition = new Vector2(lastOpenedPair.rectTransform.sizeDelta.x * -1, lastOpenedPair.rectTransform.anchoredPosition.y);
            }
        }
    }

    public void OpenPanel(int cardId)
    {
        if (lastOpened == -1 || lastOpened != cardId)
        {
            cardPanelPairs[cardId].panel.SetActive(true);
            cardPanelPairs[cardId].border.color = borderHighlightColor;

            if (lastOpened != -1)
            {
                CardPanelPair lastOpenedPair = cardPanelPairs[lastOpened];
                lastOpenedPair.panel.SetActive(false);
                lastOpenedPair.border.color = borderDefaultColor;
                lastOpenedPair.rectTransform.anchoredPosition = new Vector2(0, lastOpenedPair.rectTransform.anchoredPosition.y);
            }

            lastOpened = cardId;
            animate = true;
        }
    }
}
