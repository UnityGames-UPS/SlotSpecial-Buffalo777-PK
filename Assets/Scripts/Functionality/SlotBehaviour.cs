using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;


public class SlotBehaviour : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField]
    private Sprite[] myImages;

    [Header("Slot matrix")]
    [SerializeField] private List<SlotImage> slotmatrix;

    [SerializeField] private Color slotColor;

    [Header("Slots Transforms")]
    [SerializeField] private Image[] Slot_Transform;
    [SerializeField] private ImageAnimation[] sparkleAnim;


    [Header("Buttons")]
    [SerializeField] private CustomBtn SlotStart_Button;
    [SerializeField] private Button AutoSpinStop_Button;
    [SerializeField] private Button MaxBet_Button;
    [SerializeField] private Button TBetPlus_Button;
    [SerializeField] private Button TBetMinus_Button;
    [SerializeField] private Button Turbo_Button;
    [SerializeField] private Button StopSpin_Button;

    [Header("Animated Sprites")]
    [SerializeField] private Sprite[] anim_ID_0;
    [SerializeField] private Sprite[] anim_ID_1;
    [SerializeField] private Sprite[] anim_ID_2;
    [SerializeField] private Sprite[] anim_ID_3;
    [SerializeField] private Sprite[] anim_ID_4;
    [SerializeField] private Sprite[] anim_ID_5;
    [SerializeField] private Sprite[] anim_ID_6;
    [SerializeField] private Sprite[] anim_ID_7;
    [SerializeField] private Sprite[] anim_ID_8;
    [SerializeField] private Sprite[] anim_ID_9;
    [SerializeField] private Sprite[] anim_ID_10;
    [SerializeField] private Sprite[] anim_ID_11;
    [SerializeField] private PayoutHandler[] payoutHandlers;
    [SerializeField] Image animationPanel;


    [Header("Miscellaneous UI")]
    [SerializeField] private TMP_Text Balance_text;
    [SerializeField] private TMP_Text TotalBet_text;
    [SerializeField] private TMP_Text LineBet_text;
    [SerializeField] private TMP_Text TotalWin_text;
    [SerializeField] private GameObject TotalWinAnim;
    [SerializeField] private TMP_Text line_text;

    [Header("Audio Management")]
    [SerializeField] private AudioController audioController;

    [SerializeField] private UIManager uiManager;



    [Header("Free Spins Board")]
    [SerializeField] private GameObject FSBoard_Object;
    [SerializeField] private TMP_Text FSnum_text;

    [SerializeField] private GameObject FP_startObject;

    [SerializeField] private int tweenHeight = 0;
    [SerializeField] private float tweenStopPos;

    [SerializeField] Sprite[] TurboToggleSprites;

    private List<Tweener> alltweens = new List<Tweener>();

    private Tweener WinTween = null;

    [SerializeField]
    private List<ImageAnimation> TempList;

    [SerializeField]
    private SocketIOManager SocketManager;

    private Coroutine AutoSpinRoutine = null;
    private Coroutine FreeSpinRoutine = null;
    private Coroutine tweenroutine;
    private Tween BalanceTween;
    List<GameObject> coinsAnim = new List<GameObject>();

    internal bool IsAutoSpin = false;
    internal bool IsFreeSpin = false;
    private bool IsSpinning = false;
    private bool CheckSpinAudio = false;
    internal bool CheckPopups = false;
    internal int BetCounter = 0;
    private double currentBalance = 0;
    private double currentTotalBet = 0;
    protected int Lines = 1;
    private bool StopSpinToggle;
    private float SpinDelay = 0.2f;
    private bool IsTurboOn;
    internal bool WasAutoSpinOn;

    [SerializeField] private Image Bg;
    [SerializeField] private Sprite freeSpinBg;
    [SerializeField] private Sprite normalBg;
    [SerializeField] private GameObject TransitionImage;
    [SerializeField] private GameObject spinButtonAnim;

    private void Start()
    {
        IsAutoSpin = false;

        SlotStart_Button.SpinAction = () => {
        if (audioController) audioController.PlaySpinButtonAudio();
            StartSlots();

        };
        SlotStart_Button.AutoSpinACtion = () =>{
            AutoSpin();
            
        } ;

        if (TBetPlus_Button) TBetPlus_Button.onClick.RemoveAllListeners();
        if (TBetPlus_Button) TBetPlus_Button.onClick.AddListener(delegate { ChangeBet(true); });

        if (TBetMinus_Button) TBetMinus_Button.onClick.RemoveAllListeners();
        if (TBetMinus_Button) TBetMinus_Button.onClick.AddListener(delegate { ChangeBet(false); });

        // if (MaxBet_Button) MaxBet_Button.onClick.RemoveAllListeners();
        // if (MaxBet_Button) MaxBet_Button.onClick.AddListener(MaxBet);

        if (StopSpin_Button) StopSpin_Button.onClick.RemoveAllListeners();
        if (StopSpin_Button) StopSpin_Button.onClick.AddListener(() => { audioController.PlayButtonAudio(); StopSpinToggle = true; StopSpin_Button.gameObject.SetActive(false); });


        if (Turbo_Button) Turbo_Button.onClick.RemoveAllListeners();
        if (Turbo_Button) Turbo_Button.onClick.AddListener(TurboToggle);

        if (AutoSpinStop_Button) AutoSpinStop_Button.onClick.RemoveAllListeners();
        if (AutoSpinStop_Button) AutoSpinStop_Button.onClick.AddListener(StopAutoSpin);

        if (FSBoard_Object) FSBoard_Object.SetActive(false);


    }

    void TurboToggle()
    {
        audioController.PlayButtonAudio();
        if (IsTurboOn)
        {
            IsTurboOn = false;
            Turbo_Button.GetComponent<ImageAnimation>().StopAnimation();
            Turbo_Button.image.sprite = TurboToggleSprites[0];
            Turbo_Button.image.color = new Color(0.86f, 0.86f, 0.86f, 1);
        }
        else
        {
            IsTurboOn = true;
            Turbo_Button.GetComponent<ImageAnimation>().StartAnimation();
            Turbo_Button.image.color = new Color(1, 1, 1, 1);
        }
    }

    #region Autospin
    private void AutoSpin()
    {
        if (!IsAutoSpin)
        {

            IsAutoSpin = true;
            if (AutoSpinStop_Button) AutoSpinStop_Button.gameObject.SetActive(true);

            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                AutoSpinRoutine = null;
            }
            AutoSpinRoutine = StartCoroutine(AutoSpinCoroutine());

        }
    }

    private void StopAutoSpin()
    {
        audioController.PlayButtonAudio();
        if (IsAutoSpin)
        {
            IsAutoSpin = false;
            if (AutoSpinStop_Button) AutoSpinStop_Button.gameObject.SetActive(false);
            StartCoroutine(StopAutoSpinCoroutine());
        }
    }

    private IEnumerator AutoSpinCoroutine()
    {
        while (IsAutoSpin)
        {
            StartSlots(IsAutoSpin);
            yield return tweenroutine;
            yield return new WaitForSeconds(SpinDelay);
        }
        WasAutoSpinOn = false;
    }

    private IEnumerator StopAutoSpinCoroutine()
    {
        WasAutoSpinOn=false;
        yield return new WaitUntil(() => !IsSpinning);
        ToggleButtonGrp(true);
        if (AutoSpinRoutine != null || tweenroutine != null)
        {
            StopCoroutine(AutoSpinRoutine);
            StopCoroutine(tweenroutine);
            tweenroutine = null;
            AutoSpinRoutine = null;
            StopCoroutine(StopAutoSpinCoroutine());
        }
    }
    #endregion

    #region FreeSpin
    internal void FreeSpin(int spins)
    {
        if (!IsFreeSpin)
        {
            if (FSnum_text) FSnum_text.text = spins.ToString();
            if (FSBoard_Object) FSBoard_Object.SetActive(true);
            IsFreeSpin = true;
            ToggleButtonGrp(false);

            if (FreeSpinRoutine != null)
            {
                StopCoroutine(FreeSpinRoutine);
                FreeSpinRoutine = null;
            }
            FreeSpinRoutine = StartCoroutine(FreeSpinCoroutine(spins));
        }
    }

    private IEnumerator FreeSpinCoroutine(int spinchances)
    {
        if (Bg.sprite != freeSpinBg)
        {
            TransitionImage.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            Bg.sprite = freeSpinBg;
            TransitionImage.SetActive(false);
            audioController.SwitchBGSound(true);

        }

        int i = 0;
        while (i < spinchances)
        {
            uiManager.FreeSpins--;
            if (FSnum_text) FSnum_text.text = uiManager.FreeSpins.ToString();
            StartSlots();
            yield return tweenroutine;
            yield return new WaitForSeconds(SpinDelay);
            i++;
        }
        if (FSBoard_Object) FSBoard_Object.SetActive(false);

        if (Bg.sprite == freeSpinBg)
        {
            TransitionImage.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            Bg.sprite = normalBg;
            TransitionImage.SetActive(false);
            audioController.SwitchBGSound(false);


        }

        if (WasAutoSpinOn)
        {
            AutoSpin();
        }
        else
        {
            ToggleButtonGrp(true);
        }
        IsFreeSpin = false;
    }
    #endregion

    private void CompareBalance()
    {
        if (currentBalance < currentTotalBet)
        {
            uiManager.LowBalPopup();
        }
    }

    #region LinesCalculation
    //Fetch Lines from backend
    internal void FetchLines(string LineVal, int count)
    {
        // y_string.Add(count + 1, LineVal);
        // StaticLine_Texts[count].text = (count + 1).ToString();
        // StaticLine_Objects[count].SetActive(true);
    }

    //Generate Static Lines from button hovers
    internal void GenerateStaticLine(TMP_Text LineID_Text)
    {

    }

    //Destroy Static Lines from button hovers
    internal void DestroyStaticLine()
    {
    }
    #endregion

    private void MaxBet()
    {
        if (audioController) audioController.PlayButtonAudio();
        BetCounter = SocketManager.initialData.Bets.Count - 1;
        if (LineBet_text) LineBet_text.text = SocketManager.initialData.Bets[BetCounter].ToString();
        if (TotalBet_text) TotalBet_text.text = (SocketManager.initialData.Bets[BetCounter] * Lines).ToString();
        currentTotalBet = SocketManager.initialData.Bets[BetCounter] * Lines;
        // CompareBalance();
    }

    private void ChangeBet(bool IncDec)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (IncDec)
        {
            BetCounter++;
            if (BetCounter >= SocketManager.initialData.Bets.Count)
            {
                BetCounter = 0; // Loop back to the first bet
            }
        }
        else
        {
            BetCounter--;
            if (BetCounter < 0)
            {
                BetCounter = SocketManager.initialData.Bets.Count - 1; // Loop to the last bet
            }
        }
        if (LineBet_text) LineBet_text.text = SocketManager.initialData.Bets[BetCounter].ToString();
        if (TotalBet_text) TotalBet_text.text = (SocketManager.initialData.Bets[BetCounter] * Lines).ToString();

        currentTotalBet = SocketManager.initialData.Bets[BetCounter] * Lines;
        foreach (var symbol in SocketManager.initUIData.paylines.symbols)
        {
            int id = symbol.ID;
            if(id==9)
            continue;
            else{
            double payoutValue = symbol.payout * SocketManager.initialData.Bets[BetCounter];
            foreach (var handler in payoutHandlers.Where(handler => handler.id == id))
            {
                handler.text.text = payoutValue.ToString("f3");
            }

            }
        }
        // CompareBalance();
    }

    #region InitialFunctions
    internal void shuffleInitialMatrix()
    {
        for (int i = 0; i < slotmatrix.Count; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int randomIndex = UnityEngine.Random.Range(0,  11);
                slotmatrix[i].slotImages[j].sprite = myImages[randomIndex];
            }
        }
    }

    internal void SetInitialUI()
    {
        BetCounter = 0;
        if (LineBet_text) LineBet_text.text = SocketManager.initialData.Bets[BetCounter].ToString();
        Lines = SocketManager.initialData.linesApiData.Count;
        if (TotalBet_text) TotalBet_text.text = (SocketManager.initialData.Bets[BetCounter] * Lines).ToString();
        if (TotalWin_text) TotalWin_text.text = "0.000";
        if (Balance_text) Balance_text.text = SocketManager.playerdata.Balance.ToString("F3");
        currentBalance = SocketManager.playerdata.Balance;
        currentTotalBet = SocketManager.initialData.Bets[BetCounter] * Lines;
        line_text.text=Lines.ToString();
        foreach (var symbol in SocketManager.initUIData.paylines.symbols)
        {
            int id = symbol.ID;
            if(id==9)
            continue;

            double payoutValue = symbol.payout * SocketManager.initialData.Bets[BetCounter];
            
            foreach (var handler in payoutHandlers.Where(handler => handler.id == id))
            {
                handler.text.text = payoutValue.ToString("f3");
            }
        }
        CompareBalance();
        uiManager.InitialiseUIData(SocketManager.initUIData.AbtLogo.link, SocketManager.initUIData.AbtLogo.logoSprite, SocketManager.initUIData.ToULink, SocketManager.initUIData.PopLink, SocketManager.initUIData.paylines);
    }
    #endregion

    private void OnApplicationFocus(bool focus)
    {
        audioController.CheckFocusFunction(focus, CheckSpinAudio);
    }

    //function to populate animation sprites accordingly
    private void PopulateAnimationSprites(ImageAnimation animScript, int val)
    {
        animScript.textureArray.Clear();
        animScript.textureArray.TrimExcess();
        switch (val)
        {

            case 9:
                animScript.textureArray.AddRange(anim_ID_9);
                animScript.AnimationSpeed = anim_ID_9.Length / 1.5f;
                break;
            case 5:
                animScript.textureArray.AddRange(anim_ID_5);
                animScript.AnimationSpeed = anim_ID_5.Length / 1.5f;
                break;
            case 6:
                animScript.textureArray.AddRange(anim_ID_6);
                animScript.AnimationSpeed = anim_ID_6.Length;
                break;
            case 7:
                animScript.textureArray.AddRange(anim_ID_7);
                animScript.AnimationSpeed = anim_ID_7.Length / 1.5f;
                break;
            case 8:
                animScript.textureArray.AddRange(anim_ID_8);
                animScript.AnimationSpeed = anim_ID_8.Length / 1.5f;
                break;
            case 0:
                animScript.textureArray.AddRange(anim_ID_0);
                animScript.AnimationSpeed = anim_ID_0.Length / 1.5f;
                break;
            case 1:
                animScript.textureArray.AddRange(anim_ID_1);
                animScript.AnimationSpeed = anim_ID_1.Length / 1.5f;
                break;
            case 2:
                animScript.textureArray.AddRange(anim_ID_2);
                animScript.AnimationSpeed = anim_ID_2.Length / 1.5f;
                break;
            case 3:
                animScript.textureArray.AddRange(anim_ID_3);
                animScript.AnimationSpeed = anim_ID_3.Length / 1.5f;
                break;
            case 4:
                animScript.textureArray.AddRange(anim_ID_4);
                animScript.AnimationSpeed = anim_ID_4.Length / 1.5f;
                break;
            case 11:
                animScript.textureArray.AddRange(anim_ID_11);
                animScript.AnimationSpeed = anim_ID_11.Length / 1.5f;
                break;
            case 10:
                animScript.textureArray.AddRange(anim_ID_10);
                animScript.AnimationSpeed = anim_ID_10.Length / 1.5f;
                break;
        }
    }

    #region SlotSpin
    //starts the spin process
    private void StartSlots(bool autoSpin = false)
    {

        if (!autoSpin)
        {
            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                StopCoroutine(tweenroutine);
                tweenroutine = null;
                AutoSpinRoutine = null;
            }
        }
        WinningsAnim(false);
        audioController.StopCoinSounds();
        // if (SlotStart_Button) SlotStart_Button.btn.interactable = false;
        if (TempList.Count > 0)
        {
            StopGameAnimation();
        }
        tweenroutine = StartCoroutine(TweenRoutine());
    }

    //manage the Routine for spinning of the slots
    private IEnumerator TweenRoutine()
    {
        if (currentBalance < currentTotalBet && !IsFreeSpin)
        {
            CompareBalance();
            StopAutoSpin();
            yield return new WaitForSeconds(1);
            ToggleButtonGrp(true);
            yield break;
        }
            TotalWinAnim.SetActive(false);
        
        CheckSpinAudio = true;

        IsSpinning = true;

        ToggleButtonGrp(false);

        for (int i = 0; i < coinsAnim.Count; i++)
        {
            coinsAnim[i].SetActive(false);
        }
        if (!IsTurboOn && !IsFreeSpin && !IsAutoSpin)
        {
            StopSpin_Button.gameObject.SetActive(true);
        }
        for (int i = 0; i < Slot_Transform.Length; i++)
        {
            InitializeTweening(Slot_Transform[i].transform);
            // yield return new WaitForSeconds(0.1f);
        }

        if (!IsFreeSpin)
        {
            BalanceDeduction();
        }

        SocketManager.AccumulateResult(BetCounter);
        yield return new WaitUntil(() => SocketManager.isResultdone);

        for (int j = 0; j < slotmatrix.Count; j++)
        {
            for (int i = 0; i < slotmatrix[j].slotImages.Count; i++)
            {
                int id = SocketManager.resultData.ResultReel[j][i];
                slotmatrix[i].slotImages[j].sprite = myImages[id];
                PopulateAnimationSprites(slotmatrix[i].slotImages[j].gameObject.GetComponent<ImageAnimation>(), id);
            }
        }

        if (IsTurboOn || IsFreeSpin)
        {

            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSeconds(0.1f);

                if (StopSpinToggle)
                {
                    break;
                }
            }
            StopSpin_Button.gameObject.SetActive(false);
        }

        for (int i = 0; i < Slot_Transform.Length; i++)
        {
            yield return StopTweening(5, Slot_Transform[i].transform, i, StopSpinToggle);
        }
        StopSpinToggle = false;

        yield return alltweens[^1].WaitForCompletion();
        KillAllTweens();

        if (SocketManager.playerdata.currentWining > 0)
        {
            SpinDelay = 1.2f;
        }
        else
        {
            SpinDelay = 0.2f;
        }

        if (SocketManager.resultData.symbolsToEmit.Count > 0)
        {
            animationPanel.color = slotColor;

            for (int i = 0; i < SocketManager.resultData.linesToEmit.Count; i++)
            {
                sparkleAnim[SocketManager.resultData.linesToEmit[i]].gameObject.SetActive(true);

            }
            yield return new WaitForSeconds(0.8f);
            animationPanel.color = new Color(0, 0, 0, 0);
            for (int i = 0; i < SocketManager.resultData.linesToEmit.Count; i++)
            {
                int id = GetWinAnimId(SocketManager.resultData.linesToEmit[i]);
                foreach (var item in payoutHandlers)
                {
                    if (item.id == id)
                    {
                        item.imageAnimation.SetActive(true);
                        coinsAnim.Add(item.imageAnimation);
                    }
                }

            }


            for (int i = 0; i < 3; i++)
            {
                sparkleAnim[i].gameObject.SetActive(false);
                Slot_Transform[i].color = Color.white;
            }


            CheckPayoutLineBackend(SocketManager.resultData.symbolsToEmit, SocketManager.resultData.jackpot);

        }
        if (SocketManager.playerdata.currentWining > 0){
            TotalWinAnim.SetActive(true);
            WinningsAnim(true);
            audioController.PlayCoinSounds();

        }


        CheckPopups = true;

        if (TotalWin_text) TotalWin_text.text = SocketManager.playerdata.currentWining.ToString("F3");
        BalanceTween?.Kill();
        if (Balance_text) Balance_text.text = SocketManager.playerdata.Balance.ToString("F3");

        currentBalance = SocketManager.playerdata.Balance;

        if (SocketManager.resultData.jackpot >  0)
        {
            uiManager.PopulateWin(4, SocketManager.resultData.jackpot);
            yield return new WaitUntil(() => !CheckPopups);
            // CheckPopups = true;
        }else
        CheckWinPopups();

        yield return new WaitUntil(() => !CheckPopups);

        if (SocketManager.resultData.isfreeSpinAdded)
        {
            if (IsFreeSpin)
            {
                IsFreeSpin = false;
                if (FreeSpinRoutine != null)
                {
                    StopCoroutine(FreeSpinRoutine);
                    FreeSpinRoutine = null;
                }
            }
            else
            {

                FP_startObject.SetActive(true);
                yield return new WaitForSeconds(1.5f);
            }
            uiManager.FreeSpinPopup(SocketManager.resultData.freeSpinCount);
            yield return new WaitForSeconds(2);
            FP_startObject.SetActive(false);
            uiManager.ClosePopup();
            FreeSpin(SocketManager.resultData.freeSpinCount);
            if (AutoSpinRoutine!=null)
            {
                WasAutoSpinOn = true;
                StopAutoSpin();
                yield return new WaitForSeconds(0.1f);
            }
        }
        if (!IsAutoSpin && !IsFreeSpin)
        {
            ToggleButtonGrp(true);
            IsSpinning = false;
        }
        else
        {
            // yield return new WaitForSeconds(2f);
            IsSpinning = false;
        }

    }

    private void BalanceDeduction()
    {
        double bet = 0;
        double balance = 0;
        try
        {
            bet = double.Parse(TotalBet_text.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error while conversion " + e.Message);
        }

        try
        {
            balance = double.Parse(Balance_text.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error while conversion " + e.Message);
        }
        double initAmount = balance;

        balance = balance - bet;

        BalanceTween = DOTween.To(() => initAmount, (val) => initAmount = val, balance, 0.8f).OnUpdate(() =>
        {
            if (Balance_text) Balance_text.text = initAmount.ToString("F3");
        });
    }

    internal void CheckWinPopups()
    {
        if (SocketManager.resultData.WinAmout >= currentTotalBet * 10 && SocketManager.resultData.WinAmout < currentTotalBet * 15)
        {
            uiManager.PopulateWin(1, SocketManager.resultData.WinAmout);
        }
        else if (SocketManager.resultData.WinAmout >= currentTotalBet * 15 && SocketManager.resultData.WinAmout < currentTotalBet * 20)
        {
            uiManager.PopulateWin(2, SocketManager.resultData.WinAmout);
        }
        else if (SocketManager.resultData.WinAmout >= currentTotalBet * 20)
        {
            uiManager.PopulateWin(3, SocketManager.resultData.WinAmout);
        }
        else
        {
            CheckPopups = false;
        }
    }



    //generate the payout lines generated 
    private void CheckPayoutLineBackend(List<List<string>> points_AnimString, double jackpot = 0)
    {
        List<int> y_points = null;
        List<int> points_anim = null;
        if (points_AnimString.Count > 0)
        {

            // for (int i = 0; i < LineId.Count; i++)
            // {
            //     y_points = y_string[LineId[i] + 1]?.Split(',')?.Select(Int32.Parse)?.ToList();
            //     PayCalculator.GeneratePayoutLinesBackend(y_points, y_points.Count);
            // }

            if (jackpot > 0)
            {
                if (audioController) audioController.PlayWLAudio("megaWin");
                if(audioController) audioController.PlayCoinSounds();
                for (int i = 0; i < slotmatrix.Count; i++)
                {
                    for (int k = 0; k < slotmatrix[i].slotImages.Count; k++)
                    {
                        StartGameAnimation(slotmatrix[i].slotImages[k].gameObject);
                    }
                }
            }
            else
            {
                if (audioController) audioController.PlayWLAudio("win");
                if(audioController) audioController.PlayCoinSounds();
                for (int i = 0; i < points_AnimString.Count; i++)
                {

                    for (int k = 0; k < points_AnimString[i].Count; k++)
                    {
                        points_anim = points_AnimString[i][k]?.Split(',')?.Select(Int32.Parse)?.ToList();
                        StartGameAnimation(slotmatrix[points_anim[0]].slotImages[points_anim[1]].gameObject);

                    }
                }
            }
        }
        else
        {
            //if (audioController) audioController.PlayWLAudio("lose");
            if (audioController) audioController.StopWLAaudio();
            audioController.StopCoinSounds();
        }
        CheckSpinAudio = false;
    }

    private void WinningsAnim(bool IsStart)
    {
        if (IsStart)
        {
            WinTween = TotalWin_text.transform.DOScale(new Vector2(1.5f, 1.5f), 1f).SetLoops(-1, LoopType.Yoyo).SetDelay(0);
        }
        else
        {
            WinTween.Kill();
            TotalWin_text.transform.localScale = Vector3.one;
        }
    }

    #endregion

    internal void CallCloseSocket()
    {
        SocketManager.CloseSocket();
    }


    void ToggleButtonGrp(bool toggle)
    {
        if (SlotStart_Button) SlotStart_Button.btn.interactable = toggle;
        // if (MaxBet_Button) MaxBet_Button.interactable = toggle;
        if (TBetMinus_Button) TBetMinus_Button.interactable = toggle;
        if (TBetPlus_Button) TBetPlus_Button.interactable = toggle;
        spinButtonAnim.SetActive(!toggle);
        // if(Turbo_Button) Turbo_Button.interactable = toggle;
    }

    //start the icons animation
    private void StartGameAnimation(GameObject animObjects)
    {
        ImageAnimation temp = animObjects.GetComponent<ImageAnimation>();
        animObjects.transform.SetParent(animationPanel.transform, true);
        animObjects.transform.GetChild(0).gameObject.SetActive(true);
        temp.StartAnimation();
        TempList.Add(temp);
    }

    //stop the icons animation
    private void StopGameAnimation()
    {
        for (int i = 0; i < TempList.Count; i++)
        {
            TempList[i].StopAnimation();
            TempList[i].ResetParenrt();
            TempList[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        TempList.Clear();
        TempList.TrimExcess();
    }


    #region TweeningCode
    private void InitializeTweening(Transform slotTransform)
    {
        // slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        Tweener tweener = slotTransform.DOLocalMoveY(-tweenHeight, 0.45f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart).SetDelay(0);
        tweener.Play();
        alltweens.Add(tweener);
    }



    private IEnumerator StopTweening(int reqpos, Transform slotTransform, int index, bool isStop)
    {
        alltweens[index].Kill();
        // int tweenpos = (reqpos * IconSizeFactor) - IconSizeFactor;
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, tweenStopPos + 200);

        alltweens[index] = slotTransform.DOLocalMoveY(tweenStopPos, 0.3f).SetEase(Ease.OutQuint);
        if (audioController) audioController.PlayWLAudio("spinstop");
        if (!isStop)
        {
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            yield return null;
        }
    }



    private void KillAllTweens()
    {
        for (int i = 0; i < alltweens.Count; i++)
        {
            alltweens[i].Kill();
        }
        alltweens.Clear();

    }
    #endregion

    private int GetWinAnimId(int lineId)
    {

        int firstSymbol = SocketManager.resultData.ResultReel[lineId][0];
        int secondSymbol = SocketManager.resultData.ResultReel[lineId][1];
        int thirdSymbol=SocketManager.resultData.ResultReel[lineId][2];

        if(firstSymbol== secondSymbol ){
            if(firstSymbol==thirdSymbol) 
            return firstSymbol;
            else
            return 11;
        }
        else if (firstSymbol != secondSymbol)
        {
            if (secondSymbol is 6 or 7 or 8)
                return 11;
        }


        return -1;
    }



}


[Serializable]
public class SlotImage
{
    public List<Image> slotImages = new List<Image>(10);
}

