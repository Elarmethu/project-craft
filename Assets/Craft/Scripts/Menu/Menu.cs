using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("Windows Switches")]
    [SerializeField] private Button s_settingsButton;
    [SerializeField] private Button s_villageButton;
    [SerializeField] private Button s_playButton;


    [Header("Windows References")]
    [SerializeField] private GameObject[] windows;


    private void Start()
    {
        ComunicationButtons();
    }

    #region Communication

    private void ComunicationButtons()
    {
        s_settingsButton.onClick.AddListener(() => SwitchWindow(GetPositionWindow("settings", true)));
        s_villageButton.onClick.AddListener(() => SwitchWindow(GetPositionWindow("village", true)));
    }

    private void SwitchWindow(int index)
    {
        windows[GetPositionWindow("", false)].SetActive(false);
        windows[index].SetActive(true);
    }

    private int GetPositionWindow(string windowName, bool isPosition)
    {
        for(int i = 0; i < windows.Length; i++)
        {
            if (windows[i].name == windowName && isPosition)
                return i;
            else if (windows[i].activeSelf && !isPosition)
                return i;
        }

        return -1;
    }

    #endregion
}
