using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class MenuController : MonoBehaviour
{
    public GameObject player_1;
    public GameObject player_2;
    public GameObject playerButton_1;
    public GameObject playerButton_2;
    public GameObject howToPlayButton;
    public GameObject pointer_p1;
    public GameObject pointer_p2;
    public GameObject pointer_htp;
    public GameObject backToMenuButton;
    public GameObject maze;
    public GameObject bgMusic;
    public AudioClip slideClip;
    public AudioClip selectButtonClip;
    GameObject currentSelected;
    GameObject lastSelected;
    Transform titlePanel;
    Transform menuPanel;
    Transform htpPanel;

    private int currentSelectButtonOrder = 1;
    private float slidePanelDuration = 0.4f;
    private float shiftPanelPosY = 360; // target screen height 1080p * 1/3
    private float shiftPanelOffsetY = 100;
    private bool showHTP;
    private bool panelIsSliding;
    private TextMeshProUGUI player1Text;
    private TextMeshProUGUI player2Text;
    private TextMeshProUGUI htpText;
    private Color selectedButtonColor = new Color(1f, 0.7333f, 0.3019f, 1f);
    private AudioSource menuAudioSource;

    void Start()
    {
        menuAudioSource = GetComponent<AudioSource>();
        titlePanel = this.gameObject.transform.Find("Title Panel");
        menuPanel = this.gameObject.transform.Find("Menu Panel");
        htpPanel = this.gameObject.transform.Find("HTP Panel");
        titlePanel.localPosition = new Vector3(0, shiftPanelPosY * 2, 0);
        playerButton_1.transform.localPosition = new Vector3(0, shiftPanelPosY + shiftPanelOffsetY, 0);
        playerButton_2.transform.localPosition = new Vector3(0, shiftPanelPosY, 0);
        howToPlayButton.transform.localPosition = new Vector3(0, shiftPanelPosY - shiftPanelOffsetY, 0);
        player1Text = GameObject.Find("P1 Button Text").GetComponent<TextMeshProUGUI>();
        player2Text = GameObject.Find("P2 Button Text").GetComponent<TextMeshProUGUI>();
        htpText = GameObject.Find("HTP Button Text").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        currentSelected = EventSystem.current.currentSelectedGameObject;        

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (!showHTP)
            {
                if (currentSelected.name == "1 Player Button")
                {
                    currentSelectButtonOrder = 3;
                }
                else if (currentSelected.name == "2 Players Button")
                {
                    currentSelectButtonOrder = 1;
                }
                else if (currentSelected.name == "How to play button")
                {
                    currentSelectButtonOrder = 2;
                }
                SelectButtonSound();
            }
            
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (!showHTP)
            {
                if (currentSelected.name == "1 Player Button")
                {
                    currentSelectButtonOrder = 2;
                }
                else if (currentSelected.name == "2 Players Button")
                {
                    currentSelectButtonOrder = 3;
                }
                else if (currentSelected.name == "How to play button")
                {
                    currentSelectButtonOrder = 1;
                }
                SelectButtonSound();
            }
        }

        switch (currentSelectButtonOrder)
        {
            case 1:
                Select1PlayerButton();
                break;
            case 2:
                Select2PlayersButton();
                break;
            case 3:
                SelectHowToPlayButton();
                break;
            case 4:
                SelectBackToMenuButton();
                break;
        }
    }

    void Hover1PlayerButton()
    {
        currentSelectButtonOrder = 1;
    }

    void Hover2PlayersButton()
    {
        currentSelectButtonOrder = 2;
    }

    public void HoverHowToPlayButton()
    {
        currentSelectButtonOrder = 3;      
    }

    void Select1PlayerButton()
    {
        EventSystem.current.SetSelectedGameObject(playerButton_1);
        player_1.SetActive(true);
        player_2.SetActive(false);
        menuPanel.localPosition = new Vector3(0, -shiftPanelPosY*2, 0);
        pointer_p1.SetActive(true);
        pointer_p2.SetActive(false);
        pointer_htp.SetActive(false);
        player1Text.color = selectedButtonColor;
        player2Text.color = Color.grey;
        htpText.color = Color.grey;
    }

    void Select2PlayersButton()
    {
        EventSystem.current.SetSelectedGameObject(playerButton_2);
        player_1.SetActive(true);
        player_2.SetActive(true);
        menuPanel.localPosition = new Vector3(0, -shiftPanelPosY*2, 0);
        pointer_p1.SetActive(false);
        pointer_p2.SetActive(true);
        pointer_htp.SetActive(false);
        player1Text.color = Color.grey;
        player2Text.color = selectedButtonColor;
        htpText.color = Color.grey;
    }

    void SelectHowToPlayButton()
    {
        EventSystem.current.SetSelectedGameObject(howToPlayButton);
        player_1.SetActive(false);
        player_2.SetActive(false);
        pointer_p1.SetActive(false);
        pointer_p2.SetActive(false);
        pointer_htp.SetActive(true);
        player1Text.color = Color.grey;
        player2Text.color = Color.grey;
        htpText.color = selectedButtonColor;
    }

    void SelectBackToMenuButton()
    {
        EventSystem.current.SetSelectedGameObject(backToMenuButton);        
    }

    void StartGame()
    {
        bgMusic.GetComponent<BackgroundMusicController>().StopMusic();
        bgMusic.GetComponent<BackgroundMusicController>().PlayBgMusic();
        GameObject.Find("Maze").GetComponent<FindPathAStar>().StartPlaying();
        if (player_1.activeInHierarchy)
        {
            player_1.GetComponent<PlayerController>().PlayerStartPlaying();
        }

        if (player_2.activeInHierarchy)
        {
            player_2.GetComponent<PlayerController>().PlayerStartPlaying();
        }        
        GameObject.Find("Maze").GetComponent<FindPathAStar>().StartBlinking();
        Destroy(gameObject);
    }


    public void ShowHtpPanel()
    {
        if (!panelIsSliding)
        {
            menuPanel.gameObject.SetActive(false);
            htpPanel.gameObject.SetActive(true);
            showHTP = true;
            currentSelectButtonOrder = 4;
            RectTransform htp_RectTransform;
            htp_RectTransform = htpPanel.gameObject.GetComponent<RectTransform>();
            StartCoroutine(SlidePanelPos(htp_RectTransform, new Vector2(0f, -360f), new Vector2(0f, 360f), slidePanelDuration));
            SlidePanelSound();
        }

    }

    public void BackToMenuPanel()
    {
        if (!panelIsSliding)
        {
            menuPanel.gameObject.SetActive(true);
            htpPanel.gameObject.SetActive(false);
            RectTransform menu_RectTransform;
            menu_RectTransform = menuPanel.gameObject.GetComponent<RectTransform>();
            StartCoroutine(SlidePanelPos(menu_RectTransform, new Vector2(0f, -540f), new Vector2(0f, -180f), slidePanelDuration));
            SlidePanelSound();
            currentSelectButtonOrder = 1;
            showHTP = false;
        }
    }


    void SlidePanelSound()
    {
        if (!menuAudioSource.isPlaying)
        {
            menuAudioSource.clip = slideClip;
            menuAudioSource.Play();
        }
    }

    void SelectButtonSound()
    {
        menuAudioSource.clip = selectButtonClip;
        menuAudioSource.Play();        
    }

    IEnumerator SlidePanelPos(RectTransform m_RectTransform, Vector2 fromPos, Vector2 toPos, float duration)
    {
        float elapsedTime = 0;
        panelIsSliding = true;
        
        while (elapsedTime < duration)
        {
            m_RectTransform.anchoredPosition = Vector2.Lerp(fromPos, toPos, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        m_RectTransform.anchoredPosition = toPos;
        panelIsSliding = false;
    }

}
