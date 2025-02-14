using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{

    [Header("Menu UI")]
    [SerializeField]
    private Button Menu_Button;
    [SerializeField]
    private GameObject Menu_Object;

    [Header("Settings UI")]
    [SerializeField]
    private Button Settings_Button;




    [Header("Popus UI")]
    [SerializeField]
    private GameObject MainPopup_Object;

    [Header("About Popup")]
    [SerializeField]
    private Button AboutExit_Button;
    [SerializeField]
    private Image AboutLogo_Image;


    [Header("Paytable Popup")]
    [SerializeField] private Button Paytable_Button;
    [SerializeField] private GameObject PaytablePopup_Object;
    [SerializeField] private Button PaytableExit_Button;
    [SerializeField] private TMP_Text[] SymbolsText;
    [SerializeField] private TMP_Text FreeSpin_Text;
    [SerializeField] private TMP_Text Scatter_Text;
    [SerializeField] private TMP_Text Jackpot_Text;
    [SerializeField] private TMP_Text Bonus_Text;
    [SerializeField] private TMP_Text Wild_Text;
    [SerializeField] private GameObject[] Pages;
    [SerializeField] private Button leftToggle;
    [SerializeField] private Button rightToggle;
    private int currentPageIndex = 0;

    [Header("Settings Popup")]
    [SerializeField]
    private GameObject SettingsPopup_Object;
    [SerializeField]
    private Button SettingsExit_Button;
    [SerializeField] private Button Sound_Button;
    [SerializeField] private Button Music_Button;

    [SerializeField] private Sprite[] sound_onOff_sprite;
    [SerializeField] private Sprite[] MusicOnOff_Sprite;


    [Header("Win Popup")]
    [SerializeField]
    private Sprite BigWin_Sprite;
    [SerializeField]
    private Sprite HugeWin_Sprite;
    [SerializeField]
    private Sprite MegaWin_Sprite;
    [SerializeField]
    private Sprite Jackpot_Sprite;
    [SerializeField]
    private Image Win_Image;
    [SerializeField]
    private GameObject WinPopup_Object;
    [SerializeField]
    private TMP_Text Win_Text;
    [SerializeField] private Button SkipWinAnimation;

    [Header("FreeSpins Popup")]
    [SerializeField]
    private GameObject FreeSpinPopup_Object;
    [SerializeField]
    private TMP_Text Free_Text;

    [Header("Splash Screen")]
    [SerializeField]
    private GameObject Loading_Object;
    [SerializeField]
    private Image Loading_Image;
    [SerializeField]
    private TMP_Text Loading_Text;
    [SerializeField]
    private TMP_Text LoadPercent_Text;
    [SerializeField]
    private Button QuitSplash_button;

    [Header("Disconnection Popup")]
    [SerializeField]
    private Button CloseDisconnect_Button;
    [SerializeField]
    private GameObject DisconnectPopup_Object;

    [Header("AnotherDevice Popup")]
    [SerializeField]
    private Button CloseAD_Button;
    [SerializeField]
    private GameObject ADPopup_Object;

    [Header("Reconnection Popup")]
    [SerializeField]
    private TMP_Text reconnect_Text;
    [SerializeField]
    private GameObject ReconnectPopup_Object;

    [Header("LowBalance Popup")]
    [SerializeField]
    private Button LBExit_Button;
    [SerializeField]
    private GameObject LBPopup_Object;

    [Header("Quit Popup")]
    [SerializeField]
    private GameObject QuitPopup_Object;
    [SerializeField]
    private Button YesQuit_Button;
    [SerializeField]
    private Button NoQuit_Button;
    [SerializeField]
    private Button CrossQuit_Button;

    [SerializeField] private GameObject currentPopup;
    [SerializeField] private AudioController audioController;

    [SerializeField] private Button GameExit_Button;

    [SerializeField] private SlotBehaviour slotManager;


    private bool isMusic = true;
    private bool isSound = true;
    private bool isExit = false;
    private Tween WinPopupTextTween;
    private Tween ClosePopupTween;
    internal int FreeSpins;
    private void Start()
    {


        if (AboutExit_Button) AboutExit_Button.onClick.RemoveAllListeners();
        if (AboutExit_Button) AboutExit_Button.onClick.AddListener(ClosePopup);

        if (Paytable_Button) Paytable_Button.onClick.RemoveAllListeners();
        if (Paytable_Button) Paytable_Button.onClick.AddListener(delegate { OpenPopup(PaytablePopup_Object); });

        if (PaytableExit_Button) PaytableExit_Button.onClick.RemoveAllListeners();
        if (PaytableExit_Button) PaytableExit_Button.onClick.AddListener(ClosePopup);

        if (Settings_Button) Settings_Button.onClick.RemoveAllListeners();
        if (Settings_Button) Settings_Button.onClick.AddListener(delegate { OpenPopup(SettingsPopup_Object); });

        if (SettingsExit_Button) SettingsExit_Button.onClick.RemoveAllListeners();
        if (SettingsExit_Button) SettingsExit_Button.onClick.AddListener(ClosePopup);


        if (GameExit_Button) GameExit_Button.onClick.RemoveAllListeners();
        if (GameExit_Button) GameExit_Button.onClick.AddListener(delegate
        {
            OpenPopup(QuitPopup_Object);
            Debug.Log("Quit event: pressed Big_X button");

        });

        if (leftToggle) leftToggle.onClick.RemoveAllListeners();
        if (leftToggle) leftToggle.onClick.AddListener(() => TogglePage(-1));

        if (rightToggle) rightToggle.onClick.RemoveAllListeners();
        if (rightToggle) rightToggle.onClick.AddListener(() => TogglePage(1));

        if (NoQuit_Button) NoQuit_Button.onClick.RemoveAllListeners();
        if (NoQuit_Button) NoQuit_Button.onClick.AddListener(delegate
        {
            if (!isExit)
            {
                ClosePopup();
                Debug.Log("quit event: pressed NO Button ");
            }
        });

        if (CrossQuit_Button) CrossQuit_Button.onClick.RemoveAllListeners();
        if (CrossQuit_Button) CrossQuit_Button.onClick.AddListener(delegate
        {
            if (!isExit)
            {
                ClosePopup();
                Debug.Log("quit event: pressed Small_X Button ");

            }
        });

        if (LBExit_Button) LBExit_Button.onClick.RemoveAllListeners();
        if (LBExit_Button) LBExit_Button.onClick.AddListener(ClosePopup);

        if (YesQuit_Button) YesQuit_Button.onClick.RemoveAllListeners();
        if (YesQuit_Button) YesQuit_Button.onClick.AddListener(delegate
        {
            CallOnExitFunction();
            Debug.Log("quit event: pressed YES Button ");

        });

        if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.RemoveAllListeners();
        if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.AddListener(CallOnExitFunction);

        if (CloseAD_Button) CloseAD_Button.onClick.RemoveAllListeners();
        if (CloseAD_Button) CloseAD_Button.onClick.AddListener(CallOnExitFunction);

        if (QuitSplash_button) QuitSplash_button.onClick.RemoveAllListeners();
        if (QuitSplash_button) QuitSplash_button.onClick.AddListener(delegate { OpenPopup(QuitPopup_Object); });

        if (audioController) audioController.ToggleMute(false);

        isMusic = true;
        isSound = true;

        if (Sound_Button) Sound_Button.onClick.RemoveAllListeners();
        if (Sound_Button) Sound_Button.onClick.AddListener(ToggleSound);

        if (Music_Button) Music_Button.onClick.RemoveAllListeners();
        if (Music_Button) Music_Button.onClick.AddListener(ToggleMusic);

        if (SkipWinAnimation) SkipWinAnimation.onClick.RemoveAllListeners();
        if (SkipWinAnimation) SkipWinAnimation.onClick.AddListener(SkipWin);

        TogglePage(0);
    }

    internal void LowBalPopup()
    {
        OpenPopup(LBPopup_Object);
    }

    internal void DisconnectionPopup(bool isReconnection)
    {
        if (!isExit)
        {
            OpenPopup(DisconnectPopup_Object);
        }
    }

    internal void PopulateWin(int value, double amount)
    {
        switch (value)
        {
            case 1:
                if (Win_Image) Win_Image.sprite = BigWin_Sprite;
                break;
            case 2:
                if (Win_Image) Win_Image.sprite = HugeWin_Sprite;
                break;
            case 3:
                if (Win_Image) Win_Image.sprite = MegaWin_Sprite;
                break;
            case 4:
                if (Win_Image) Win_Image.sprite = Jackpot_Sprite;
                break;
        }

        StartPopupAnim(amount);
    }

    internal void CloseFreeSpinPopup()
    {
        if (MainPopup_Object) MainPopup_Object.SetActive(false);
        if (FreeSpinPopup_Object) FreeSpinPopup_Object.SetActive(false);
        // slotManager.FreeSpin(spins);
    }

    internal void FreeSpinPopup(int spins)
    {
        int ExtraSpins = spins - FreeSpins;
        FreeSpins = spins;
        Debug.Log("ExtraSpins: " + ExtraSpins);
        Debug.Log("Total Spins: " + spins);
        if (Free_Text) Free_Text.text = ExtraSpins.ToString() + " Free spins awarded.";
        OpenPopup(FreeSpinPopup_Object);
        // if (FreeSpinPopup_Object) FreeSpinPopup_Object.SetActive(true);           
        // if (MainPopup_Object) MainPopup_Object.SetActive(true);
        // DOVirtual.DelayedCall(2f, ()=>{
        //     StartFreeSpins(spins);
        // });
    }

    void SkipWin()
    {
        Debug.Log("Skip win called");
        if (ClosePopupTween != null)
        {
            ClosePopupTween.Kill();
            ClosePopupTween = null;
        }
        if (WinPopupTextTween != null)
        {
            WinPopupTextTween.Kill();
            WinPopupTextTween = null;
        }
        ClosePopup();
        slotManager.CheckPopups = false;
    }

    private void StartPopupAnim(double amount)
    {
        double initAmount = 0;
        OpenPopup(WinPopup_Object);
        // if (WinPopup_Object) WinPopup_Object.SetActive(true);
        // if (MainPopup_Object) MainPopup_Object.SetActive(true);
        WinPopupTextTween = DOTween.To(() => initAmount, (val) => initAmount = val, amount, 5f).OnUpdate(() =>
        {
            if (Win_Text) Win_Text.text = initAmount.ToString("F3");
        });

        ClosePopupTween = DOVirtual.DelayedCall(6f, () =>
        {
            ClosePopup();
            slotManager.CheckPopups = false;
        });
    }

    internal void ADfunction()
    {
        OpenPopup(ADPopup_Object);
    }

    internal void InitialiseUIData(string SupportUrl, string AbtImgUrl, string TermsUrl, string PrivacyUrl, Paylines symbolsText)
    {
        // if (Support_Button) Support_Button.onClick.RemoveAllListeners();
        // if (Support_Button) Support_Button.onClick.AddListener(delegate { UrlButtons(SupportUrl); });

        // if (Terms_Button) Terms_Button.onClick.RemoveAllListeners();
        // if (Terms_Button) Terms_Button.onClick.AddListener(delegate { UrlButtons(TermsUrl); });

        // if (Privacy_Button) Privacy_Button.onClick.RemoveAllListeners();
        // if (Privacy_Button) Privacy_Button.onClick.AddListener(delegate { UrlButtons(PrivacyUrl); });

        // StartCoroutine(DownloadImage(AbtImgUrl));
        PopulateSymbolsPayout(symbolsText);
    }

    private void PopulateSymbolsPayout(Paylines paylines)
    {
        for (int i = 0; i < SymbolsText.Length; i++)
        {

            if (SymbolsText[i]) SymbolsText[i].text = paylines.symbols[i].payout + "x";
        }

        if (Jackpot_Text) Jackpot_Text.text = paylines.symbols[11].description.ToString();
        if (Bonus_Text) Bonus_Text.text = paylines.symbols[10].description.ToString();
        if (Wild_Text) Wild_Text.text = paylines.symbols[9].description.ToString();

    }

    private void CallOnExitFunction()
    {
        isExit = true;
        audioController.PlayButtonAudio();
        slotManager.CallCloseSocket();
    }


    private void OpenPopup(GameObject Popup)
    {
        if (currentPopup != null)
        {
            ClosePopup();
        }
        if (audioController) audioController.PlayButtonAudio();
        if (Popup) Popup.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
    }

    internal void ClosePopup()
    {
        if (audioController) audioController.PlayButtonAudio();


        if (!DisconnectPopup_Object.activeSelf)
        {
            if (currentPopup) currentPopup.SetActive(false);
            if (MainPopup_Object) MainPopup_Object.SetActive(false);
        }
    }

    private void ToggleMusic()
    {
        isMusic = !isMusic;
        if (isMusic)
        {
            Music_Button.image.sprite=MusicOnOff_Sprite[0];

            audioController.ToggleMute(false, "bg");
        }
        else
        {
            Music_Button.image.sprite=MusicOnOff_Sprite[1];

            audioController.ToggleMute(true, "bg");
        }
    }

    void TogglePage(int index)
    {
        audioController.PlayButtonAudio();
        if (index > 0)
        {
            currentPageIndex++;
            if (currentPageIndex > Pages.Length - 1)
            {
                currentPageIndex = 0;
            }

        }
        else if (index < 0)
        {
            currentPageIndex--;
            if (currentPageIndex < 0)
            {
                currentPageIndex = Pages.Length - 1;

            }

        }
        else
            currentPageIndex = 0;
        foreach (var item in Pages)
        {
            item.SetActive(false);


        }
        Pages[currentPageIndex].SetActive(true);




    }

    private void UrlButtons(string url)
    {
        Application.OpenURL(url);
    }

    private void ToggleSound()
    {
        isSound = !isSound;
        if (isSound)
        {
            Sound_Button.image.sprite=sound_onOff_sprite[0];
            if (audioController) audioController.ToggleMute(false, "button");
            if (audioController) audioController.ToggleMute(false, "wl");
        }
        else
        {
            Sound_Button.image.sprite=sound_onOff_sprite[1];
            if (audioController) audioController.ToggleMute(true, "button");
            if (audioController) audioController.ToggleMute(true, "wl");
        }
    }

    private IEnumerator DownloadImage(string url)
    {
        // Create a UnityWebRequest object to download the image
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

        // Wait for the download to complete
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

            // Apply the sprite to the target image
            AboutLogo_Image.sprite = sprite;
        }
        else
        {
            Debug.LogError("Error downloading image: " + request.error);
        }
    }
}
