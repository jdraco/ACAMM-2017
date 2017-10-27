using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

enum ScanState
{
    SCANSTATE_START,
    SCANSTATE_SCANNING,
    SCANSTATE_JUSTSCANNED,
    SCANSTATE_SCANNED
}

public class ScanManager : MonoBehaviour
{
    public float FillRate = 1.0f;
    public ScanHand HandScanSprite;
    public Image RadialRing;
    public List<GameObject> Flags;
    public string ThisConsoleCountry = "None";

    public Text WelcomeText;
    public float TextOffset = 0.0f;
    public float TextTime = 1.0f;
    float TextProgress;

    public float BlockOffTime = 1.0f;
    float BlockOffTimeProgress;

    Vector3 OldWhitePosition;
    Vector3 OldPosition;

    public GameObject ScreenBlock;
    bool Handgone = false;

    public float FlagTimeDelay = 0.0f;
    public float FlagTime = 1.0f;
    float FlagProgress;

    public GridManager GridMaseter;


    ScanState CurrentState = ScanState.SCANSTATE_START;
    GameObject FlagMainParent = null;

    Dictionary<string, int> Country = new Dictionary<string, int>();

    // Use this for initialization 
    void Start()
    {
        WelcomeText.rectTransform.localPosition = WelcomeText.rectTransform.localPosition + new Vector3(0, TextOffset, 0);
        OldPosition = new Vector3();
        OldPosition = WelcomeText.rectTransform.localPosition;

        OldWhitePosition = new Vector3();
        OldWhitePosition = ScreenBlock.transform.localPosition;

        WelcomeText.color = new Color(WelcomeText.color.r, WelcomeText.color.g, WelcomeText.color.b, 0.0f);
        TextProgress = 0.0f;

        FlagProgress = 0.0f;

        RadialRing.fillAmount = 0.0f;
        // Scanned();
        FlagMainParent = Flags[0].transform.parent.gameObject;
        foreach (GameObject Flag in Flags)
        {
            GameObject FlagParent = new GameObject(Flag.name + "Parent");
            FlagParent.transform.parent = Flag.gameObject.transform.parent;
            Flag.transform.parent = FlagParent.transform;

            Country.Add(Flag.name, Flags.IndexOf(Flag));

            Flag.transform.localPosition = new Vector3(4, Flag.transform.localPosition.y, Flag.transform.localPosition.z);
            FlagParent.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, Country[Flag.name] * (360 / Flags.Count)));
            Flag.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -Country[Flag.name] * (360 / Flags.Count)));

        }

        Flags[Country[ThisConsoleCountry]].transform.localPosition = new Vector3(0,0, 0.1f);
        Flags[Country[ThisConsoleCountry]].transform.localScale = new Vector3(1.3f, 1.3f,0.15f);

        //FlagMainParent.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrentState = ScanState.SCANSTATE_SCANNING;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrentState = ScanState.SCANSTATE_START;
        }

        switch (CurrentState)
        {
            case ScanState.SCANSTATE_START:
                RadialRing.fillAmount -= Time.deltaTime * FillRate * 3;
                break;

            case ScanState.SCANSTATE_SCANNING:
                RadialRing.fillAmount += Time.deltaTime * FillRate;
                if (RadialRing.fillAmount >= 1.0f)
                {
                    Scanned();
                }
                break;

            case ScanState.SCANSTATE_JUSTSCANNED:
                {
                    //Text 
                    float CurrentProgress = 0.0f;
                    TextProgress += Time.deltaTime;
                    CurrentProgress = Mathf.Lerp(0.0f, 1.0f, TextProgress / TextTime);
                    WelcomeText.color = new Color(WelcomeText.color.r, WelcomeText.color.g, WelcomeText.color.b, CurrentProgress);

                    WelcomeText.rectTransform.localPosition = Vector3.Lerp(OldPosition, OldPosition - new Vector3(0, TextOffset, 0), CurrentProgress);
                    RadialRing.transform.localScale += new Vector3(1,1,1) * Time.deltaTime * 20;

                    //White Going up
                    BlockOffTimeProgress += Time.deltaTime;
                    float CurrentWhiteProgress = Mathf.Lerp(0.0f, 1.0f, BlockOffTimeProgress / BlockOffTime);
                    ScreenBlock.transform.localPosition = Vector3.Lerp(OldWhitePosition, OldWhitePosition + new Vector3(0,20,0), CurrentWhiteProgress);

                    //Flags

                    float CurrentFlagProgress = 0.0f;

                    FlagProgress += Time.deltaTime;

                    if (FlagProgress >= FlagTimeDelay)
                    {
                        CurrentFlagProgress = Mathf.Lerp(0.0f, 1.0f, (FlagProgress - FlagTimeDelay) / FlagTime);
                        Flags[Country[ThisConsoleCountry]].transform.localPosition = Vector3.Lerp(new Vector3(0, 0, 0.1f), new Vector3(4, Flags[Country[ThisConsoleCountry]].transform.localPosition.y, Flags[Country[ThisConsoleCountry]].transform.localPosition.z), CurrentFlagProgress);
                        Flags[Country[ThisConsoleCountry]].transform.localScale = Vector3.Lerp(new Vector3(1.3f, 1.3f, 0.15f), new Vector3(0.15f, 0.15f, 0.15f), CurrentFlagProgress);

                    }

                    if (CurrentProgress >= 1.0f && Handgone && BlockOffTimeProgress >= 1.0f && CurrentFlagProgress >= 1.0f)
                        CurrentState = ScanState.SCANSTATE_SCANNED;
                    break;
                }
        }

    }

    void Scanned()
    {
       // ScreenBlock.SetActive(false);
       // RadialRing.gameObject.SetActive(false);

        CurrentState = ScanState.SCANSTATE_JUSTSCANNED;
        FlagMainParent.SetActive(true);

        iTween.ScaleBy(HandScanSprite.gameObject, iTween.Hash(
            "amount", new Vector3(0, 0, 0),
            "time", 0.5f, 
            "easetype", "EaseInBack", 
            "looptype", "none", 
            "delay", 0.0f,
            "onComplete", "HandGone"));

    }

    void HandGone()
    {
        Handgone = true;
    }
}
