using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

enum ScanningState
{
    NONE_STATE,
    START_STATE,
    JUSTHANDPLACE_STATE,
    COMPLETED_STATE,
    WELCOME_STATE,
    WELCOMEDONE_STATE,
    BLACKOUT_STATE
}

enum ThumbState
{
    NONE_STATE,
    START_STATE,
    THUMBSCAN_STATE,
    LASER_STATE,
    SCANCOMPLETE_STATE,
    WELCOME_STATE,
    BLACKOUTSTATE
}

enum ThumbSubState
{
    THUMB_BACKGROUNDBLINK,
    LASER_MOVEDOWN
}


public class NewScanManager : MonoBehaviour
{

    //ScanningState CurrentState;

    public SpriteRenderer Handprint = null;
    public Text CircleText = null;

    public Text ScanningText = null;
    public Text PercentageText = null;

    public GameObject Beam;
    float BeamYPosition;
    bool BeamGoingDown;

    float PercentageAmount;

    float TextInterval = 0.0f;

    string PreScanningText = "SCANNING...";
    int TextCounter = -2;

    public FingerPrint[] FingerPrints;
    float FingerInterval = 0.0f;
    int FingerPrintCounter = 4;

    public List<TextScript> TextScripts;

    float PopUpMin = 0.1f;
    float PopUpMax = 0.5f;
    //float PopUpTimePassed = 0.0f;
    float PopUpTime = 0.0f;
    int PopUpCounter = 0;

    int ClickTimes = 0;
    public Texture2D CursorTexture;
    public Text NumberCounter;

    public GameObject SpinningDottedCircle;
    bool HandGone = false;

    bool DelayEnd = false;
    bool CircleGone = false;

    public GameObject TempBackgroundGrid;
    public GridManager GridMaster;

    public Text WelcomeText;
    public SpriteRenderer SingaporeFlag, Logo, COAPhoto;

    public SpriteRenderer Screenshot;

    public float textSpeed = 0.05f;

    public List<GameObject> ListOfFingerPoints;

    public GameObject ThumnprintStart;
    public GameObject PanelMask, ThumbPrint, LASERPEWPEW;
    ThumbState CurrentState;

    public Image ThumbBackground;
    Vector3 PanelStartScale = new Vector3(), ThumbStartScale = new Vector3();
    Vector3 ThumbStartPos = new Vector3();
    Vector3 LaserStartPos = new Vector3();
    Vector3 BottomOfFingerPrint = new Vector3();
    public GameObject PanelOfText;

    public Text NewScanningText;
    string Scanning = "S C A N N I N G";

    public GameObject Point1, Point2;
    Dictionary<string, Vector3> DictionaryOfVectors = new Dictionary<string, Vector3>()
    {
        {"Point1StartPoint", new Vector3()},
        {"Point2StartPoint", new Vector3()},
        {"Point1StartScale", new Vector3()},
        {"Point2StartScale", new Vector3()}
    };


    Dictionary<ThumbState, Dictionary<ThumbSubState, int>> DictionaryOfTriggers = new Dictionary<ThumbState, Dictionary<ThumbSubState, int>>
    {
        {
            ThumbState.THUMBSCAN_STATE, new Dictionary<ThumbSubState, int>
            {
                {ThumbSubState.THUMB_BACKGROUNDBLINK, 0}
            }
        },
        {
            ThumbState.LASER_STATE, new Dictionary<ThumbSubState, int>
            {
                {ThumbSubState.LASER_MOVEDOWN,0 }
            }

        }
    };
    // Use this for initialization
    void Start()
    {
        CurrentState = ThumbState.START_STATE;

        ScanningText.gameObject.SetActive(false);
        PercentageText.gameObject.SetActive(false);

        ScanningText.text = "";
        PercentageAmount = 0.0f;

        BeamYPosition = Beam.transform.localPosition.x;
        BeamGoingDown = true;

    }

    // Update is called once per frame
    void Update()
    {
        //StateUpdate();
        ThumbStateUpdate();
    }

    //  void StateUpdate()
    //  {
    //      switch (CurrentState)
    //      {
    //          case ScanningState.NONE_STATE:
    //              Debug.Log("Error, State at none");
    //              break;

    //          case ScanningState.START_STATE:
    //              StateStartUpdate();
    //              break;

    //          case ScanningState.JUSTHANDPLACE_STATE:
    //              HandplaceUpdate();
    //              break;

    //          case ScanningState.COMPLETED_STATE:
    //              CompletedStateUpdate();
    //              break;
    //          case ScanningState.WELCOME_STATE:
    //              WelcomeUpdate();
    //              break;
    //case ScanningState.WELCOMEDONE_STATE:
    //		StartCoroutine ("LoadAppAfterDelay");
    //              CurrentState = ScanningState.BLACKOUT_STATE;

    //              break;
    //          case ScanningState.BLACKOUT_STATE:
    //		//SceneManager.LoadSceneAsync ("Start");
    //              break;
    //      }
    //  }

    void ThumbStateUpdate()
    {
        switch (CurrentState)
        {
            case ThumbState.NONE_STATE:
                Debug.Log("Error, State at none");
                break;
            case ThumbState.START_STATE:
                StateStartUpdate();
                break;

            case ThumbState.THUMBSCAN_STATE:
                ThumbPlaceUpdate();
                break;
            case ThumbState.LASER_STATE:
                LaserUpdate();
                break;

            case ThumbState.SCANCOMPLETE_STATE:
                WelcomeUpdate();
                break;

            case ThumbState.WELCOME_STATE:
                StartCoroutine ("LoadAppAfterDelay");
                CurrentState = ThumbState.BLACKOUTSTATE;
                break;

            case ThumbState.BLACKOUTSTATE:
                break;
        }

    }

    IEnumerator LoadAppAfterDelay()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadSceneAsync("Start");
    }

    bool FiveFingersTouch()
    {
        int NumberOfFingerTouched = 0;
        foreach (GameObject FingerEnd in ListOfFingerPoints)
        {
            bool ThisFingerTouch = false;
            foreach (Touch EachTouch in Input.touches)
            {
                Ray ray = Camera.main.ScreenPointToRay(EachTouch.position);
                RaycastHit hit;
                if (FingerEnd.GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity))
                {
                    ThisFingerTouch = true;
                    NumberOfFingerTouched++;
                    break;
                }
            }

            if (!ThisFingerTouch)
            {
                FingerEnd.transform.Rotate(new Vector3(0, 0, 10 * Time.deltaTime));
            }
        }

        if (NumberOfFingerTouched == 5)
            return true;

        return false;
    }

    bool ThreePointTouch()
    {
        if (Input.touchCount >= 3)
        {
            int NumberOfTouch = 0;
            foreach (Touch EachTouch in Input.touches)
            {
                Ray ray = Camera.main.ScreenPointToRay(EachTouch.position);
                RaycastHit hit;

                Debug.DrawLine(ray.origin, ray.direction);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "DottedCircle")
                    {
                        NumberOfTouch++;
                        if (NumberOfTouch == 3)
                            return true;
                    }
                }
            }
        }

        return false;
    }

    void StateStartUpdate()
    {
        //#if UNITY_EDITOR
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit;

        //Debug.DrawLine(ray.origin, ray.direction);

        //if (Physics.Raycast(ray, out hit))
        //{
        //	if (hit.collider.tag == "DottedCircle")
        //	{
        //		Cursor.SetCursor(CursorTexture, new Vector2(24, 24), CursorMode.Auto);

        //		if (Input.GetMouseButtonDown(0))
        //		{
        //			ClickTimes++;
        //			NumberCounter.text = ClickTimes.ToString();

        //			if (ClickTimes == 3)
        //			{
        //				Destroy(NumberCounter.gameObject);
        //				CurrentState = ScanningState.JUSTHANDPLACE_STATE;
        //				Debug.Log("Enter Placed");
        //				Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        //				PopUpTime = Random.Range(PopUpMin, PopUpMax);

        //				GridMaster.ScanningStart();
        //			}
        //		}

        //	}
        //}
        //else
        //{
        //	Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        //}
        //#else
        //if (FiveFingersTouch()) {
        //	Destroy(NumberCounter.gameObject);
        //	CurrentState = ScanningState.JUSTHANDPLACE_STATE;
        //	Debug.Log("Enter Placed");
        //	Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        //	PopUpTime = Random.Range(PopUpMin, PopUpMax);

        //	GridMaster.ScanningStart();
        //}

        //#endif
#if UNITY_EDITOR
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0))
        {
            if (ThumnprintStart.GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity))
            {
                CurrentState = ThumbState.THUMBSCAN_STATE;
            }
        }
#else
        foreach (Touch EachTouch in Input.touches)
         {
            Ray ray = Camera.main.ScreenPointToRay(EachTouch.position);
            RaycastHit hit;
            Debug.Log("Touch Detected");
            if (ThumnprintStart.GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity))
            {
                    CurrentState = ThumbState.THUMBSCAN_STATE;
                    break;
            }
        }
#endif

    }

    void ThumbPlaceUpdate()
    {
        if (DictionaryOfTriggers[ThumbState.THUMBSCAN_STATE][ThumbSubState.THUMB_BACKGROUNDBLINK] == 0)
        {
            Color CurrentColor = ThumbBackground.color;
            CurrentColor.a = Mathf.MoveTowards(CurrentColor.a, 0.7f, Time.deltaTime * 3.5f);
            ThumbBackground.color = CurrentColor;

            if (CurrentColor.a == 0.7f)
                DictionaryOfTriggers[ThumbState.THUMBSCAN_STATE][ThumbSubState.THUMB_BACKGROUNDBLINK]++;
        }
        else if (DictionaryOfTriggers[ThumbState.THUMBSCAN_STATE][ThumbSubState.THUMB_BACKGROUNDBLINK] == 1)
        {
            Color CurrentColor = ThumbBackground.color;
            CurrentColor.a = Mathf.MoveTowards(CurrentColor.a, 0, Time.deltaTime * 4);
            ThumbBackground.color = CurrentColor;

            if (CurrentColor.a == 0)
                DictionaryOfTriggers[ThumbState.THUMBSCAN_STATE][ThumbSubState.THUMB_BACKGROUNDBLINK]++;
        }
        else if (DictionaryOfTriggers[ThumbState.THUMBSCAN_STATE][ThumbSubState.THUMB_BACKGROUNDBLINK] == 2)
        {
            Color CurrentColor = ThumbBackground.color;
            CurrentColor.a = Mathf.MoveTowards(CurrentColor.a, 0.7f, Time.deltaTime * 4);
            ThumbBackground.color = CurrentColor;

            if (CurrentColor.a == 0.7f)
                DictionaryOfTriggers[ThumbState.THUMBSCAN_STATE][ThumbSubState.THUMB_BACKGROUNDBLINK]++;
        }
        else if (DictionaryOfTriggers[ThumbState.THUMBSCAN_STATE][ThumbSubState.THUMB_BACKGROUNDBLINK] == 3)
        {
            Color CurrentColor = ThumbBackground.color;
            CurrentColor.a = Mathf.MoveTowards(CurrentColor.a, 0, Time.deltaTime * 4);
            ThumbBackground.color = CurrentColor;

            if (CurrentColor.a == 0)
                DictionaryOfTriggers[ThumbState.THUMBSCAN_STATE][ThumbSubState.THUMB_BACKGROUNDBLINK]++;
        }
        else if (DictionaryOfTriggers[ThumbState.THUMBSCAN_STATE][ThumbSubState.THUMB_BACKGROUNDBLINK] == 4)
        {
            Color CurrentColor = ThumbBackground.color;
            CurrentColor.a = Mathf.MoveTowards(CurrentColor.a, 0.7f, Time.deltaTime * 4);
            ThumbBackground.color = CurrentColor;

            if (CurrentColor.a == 0.7f)
                DictionaryOfTriggers[ThumbState.THUMBSCAN_STATE][ThumbSubState.THUMB_BACKGROUNDBLINK]++;
        }
        else if (DictionaryOfTriggers[ThumbState.THUMBSCAN_STATE][ThumbSubState.THUMB_BACKGROUNDBLINK] == 5)
        {
            Color CurrentColor = ThumbBackground.color;
            CurrentColor.a = Mathf.MoveTowards(CurrentColor.a, 0, Time.deltaTime * 10);
            ThumbBackground.color = CurrentColor;

            if (CurrentColor.a == 0)
                DictionaryOfTriggers[ThumbState.THUMBSCAN_STATE][ThumbSubState.THUMB_BACKGROUNDBLINK]++;
        }
        else if (DictionaryOfTriggers[ThumbState.THUMBSCAN_STATE][ThumbSubState.THUMB_BACKGROUNDBLINK] == 6)
        {
            Color CurrentColor = ThumbBackground.color;
            CurrentColor.a = Mathf.MoveTowards(CurrentColor.a, 0.7f, Time.deltaTime * 10);
            ThumbBackground.color = CurrentColor;

            if (CurrentColor.a == 0.7f)
                DictionaryOfTriggers[ThumbState.THUMBSCAN_STATE][ThumbSubState.THUMB_BACKGROUNDBLINK]++;
        }

        if (DictionaryOfTriggers[ThumbState.THUMBSCAN_STATE][ThumbSubState.THUMB_BACKGROUNDBLINK] == 7)
        {
            LASERPEWPEW.transform.position = ThumbPrint.transform.position + new Vector3(0, (ThumbPrint.transform.lossyScale.y * ThumbPrint.GetComponent<RectTransform>().sizeDelta.y) * 0.5f, 0);
            LASERPEWPEW.SetActive(true);
            PanelMask.SetActive(true);
            ThumbPrint.SetActive(true);

            PanelStartScale = PanelMask.transform.localScale;
            ThumbStartScale = ThumbPrint.transform.localScale;
            ThumbStartPos = ThumbPrint.transform.position;
            LaserStartPos = LASERPEWPEW.transform.position;

            DictionaryOfVectors["Point1StartPoint"] = Point1.transform.position;
            DictionaryOfVectors["Point2StartPoint"] = Point2.transform.position;

            DictionaryOfVectors["Point1StartScale"] = Point1.transform.localScale;
            DictionaryOfVectors["Point2StartScale"] = Point2.transform.localScale;

            BottomOfFingerPrint = ThumbPrint.transform.position - new Vector3(0, (ThumbPrint.transform.lossyScale.y * ThumbPrint.GetComponent<RectTransform>().sizeDelta.y) * 0.5f, 0);
            PanelMask.transform.localScale = new Vector3(PanelMask.transform.localScale.x, 0, PanelMask.transform.localScale.z);
            PanelMask.transform.position = LASERPEWPEW.transform.position - ((LASERPEWPEW.transform.position - LaserStartPos) * 0.5f);
            ThumbPrint.transform.position = ThumbStartPos;

           CurrentState = ThumbState.LASER_STATE;
        }
    }
        
    void LaserUpdate()
    {
        if (DictionaryOfTriggers[CurrentState][ThumbSubState.LASER_MOVEDOWN] == 0)
        {
            LASERPEWPEW.transform.position = Vector3.MoveTowards(LASERPEWPEW.transform.position, BottomOfFingerPrint, Time.deltaTime * 0.3f);

            Vector3 SizeBetweenTopAndLaser = LASERPEWPEW.transform.position - LaserStartPos;
            float ChangeInPercent = (SizeBetweenTopAndLaser.y / (BottomOfFingerPrint - LaserStartPos).y);

            PanelMask.transform.position = LASERPEWPEW.transform.position - (SizeBetweenTopAndLaser * 0.5f);
            PanelMask.transform.localScale = new Vector3(PanelMask.transform.localScale.x, ChangeInPercent * PanelStartScale.y, PanelMask.transform.localScale.z);

            if (ChangeInPercent != 0.0f)
                ThumbPrint.transform.localScale = new Vector3(ThumbPrint.transform.localScale.x, ThumbStartScale.y / ChangeInPercent, ThumbPrint.transform.localScale.z);

            ThumbPrint.transform.position = ThumbStartPos;

            TextInterval += Time.deltaTime;
            if (TextInterval >= 0.2f)
            {
                TextInterval = 0.0f;

                if (NewScanningText.text == Scanning)
                {
                    NewScanningText.text = "";
                }
                else if (NewScanningText.text.Length + 1 < Scanning.Length)
                {
                    NewScanningText.text += Scanning[NewScanningText.text.Length];
                    NewScanningText.text += Scanning[NewScanningText.text.Length];
                }
                else
                {
                    NewScanningText.text = Scanning;
                }
            }


            if (LASERPEWPEW.transform.position == BottomOfFingerPrint)
            {
                DictionaryOfTriggers[CurrentState][ThumbSubState.LASER_MOVEDOWN]++;
            }
        }
        else if (DictionaryOfTriggers[CurrentState][ThumbSubState.LASER_MOVEDOWN] == 1)
        {
            LASERPEWPEW.transform.position = Vector3.MoveTowards(LASERPEWPEW.transform.position, LaserStartPos, Time.deltaTime * 0.3f);

            Vector3 SizeBetweenTopAndLaser = LASERPEWPEW.transform.position - LaserStartPos;
            float ChangeInPercent = (SizeBetweenTopAndLaser.y / (BottomOfFingerPrint - LaserStartPos).y);

            PanelMask.transform.position = LASERPEWPEW.transform.position - (SizeBetweenTopAndLaser * 0.5f);
            PanelMask.transform.localScale = new Vector3(PanelMask.transform.localScale.x, ChangeInPercent * PanelStartScale.y, PanelMask.transform.localScale.z);

            if (ChangeInPercent != 0.0f)
                ThumbPrint.transform.localScale = new Vector3(ThumbPrint.transform.localScale.x, ThumbStartScale.y / ChangeInPercent, ThumbPrint.transform.localScale.z);

            ThumbPrint.transform.position = ThumbStartPos;

            TextInterval += Time.deltaTime;
            if (TextInterval >= 0.2f)
            {
                TextInterval = 0.0f;

                if (NewScanningText.text == Scanning)
                {
                    NewScanningText.text = "";
                }
                else if (NewScanningText.text.Length + 1 < Scanning.Length)
                {
                    NewScanningText.text += Scanning[NewScanningText.text.Length];
                    NewScanningText.text += Scanning[NewScanningText.text.Length];
                }
                else
                {
                    NewScanningText.text = Scanning;
                }
            }

            if (LASERPEWPEW.transform.position == LaserStartPos)
            {
                DictionaryOfTriggers[CurrentState][ThumbSubState.LASER_MOVEDOWN]++;

                Point1.SetActive(true);
            }
        }
        else if (DictionaryOfTriggers[CurrentState][ThumbSubState.LASER_MOVEDOWN] == 2)
        {
            LASERPEWPEW.transform.position = Vector3.MoveTowards(LASERPEWPEW.transform.position, BottomOfFingerPrint, Time.deltaTime * 0.3f);

            Vector3 SizeBetweenTopAndLaser = LASERPEWPEW.transform.position - LaserStartPos;
            float ChangeInPercent = (SizeBetweenTopAndLaser.y / (BottomOfFingerPrint - LaserStartPos).y);

            PanelMask.transform.position = LASERPEWPEW.transform.position - (SizeBetweenTopAndLaser * 0.5f);
            PanelMask.transform.localScale = new Vector3(PanelMask.transform.localScale.x, ChangeInPercent * PanelStartScale.y, PanelMask.transform.localScale.z);

            if (ChangeInPercent != 0.0f)
            {
                ThumbPrint.transform.localScale = new Vector3(ThumbPrint.transform.localScale.x, ThumbStartScale.y / ChangeInPercent, ThumbPrint.transform.localScale.z);
                Point1.transform.localScale = new Vector3(Point1.transform.localScale.x, DictionaryOfVectors["Point1StartScale"].y / ChangeInPercent, Point1.transform.localScale.z);
            }

            ThumbPrint.transform.position = ThumbStartPos;
            Point1.transform.position = DictionaryOfVectors["Point1StartPoint"];

            TextInterval += Time.deltaTime;
            if (TextInterval >= 0.2f)
            {
                TextInterval = 0.0f;

                if (NewScanningText.text == Scanning)
                {
                    NewScanningText.text = "";
                }
                else if (NewScanningText.text.Length + 1 < Scanning.Length)
                {
                    NewScanningText.text += Scanning[NewScanningText.text.Length];
                    NewScanningText.text += Scanning[NewScanningText.text.Length];
                }
                else
                {
                    NewScanningText.text = Scanning;
                }
            }

            if (LASERPEWPEW.transform.position == BottomOfFingerPrint)
            {
                ThumbPrint.transform.SetParent(PanelMask.transform.parent);
                Point1.transform.SetParent(PanelMask.transform.parent);

                ThumbPrint.transform.SetSiblingIndex(PanelMask.transform.GetSiblingIndex());
                Point1.transform.SetSiblingIndex(PanelMask.transform.GetSiblingIndex());

                Point2.SetActive(true);

                PanelMask.transform.localScale = new Vector3(PanelMask.transform.localScale.x, 0, PanelMask.transform.localScale.z);
                PanelMask.transform.position = LASERPEWPEW.transform.position - ((LASERPEWPEW.transform.position - BottomOfFingerPrint) * 0.5f);

                DictionaryOfTriggers[CurrentState][ThumbSubState.LASER_MOVEDOWN]++;
            }
        }
        else if (DictionaryOfTriggers[CurrentState][ThumbSubState.LASER_MOVEDOWN] == 3)
        {
            LASERPEWPEW.transform.position = Vector3.MoveTowards(LASERPEWPEW.transform.position, LaserStartPos, Time.deltaTime * 0.3f);

            Vector3 SizeBetweenBottomAndLaser = LASERPEWPEW.transform.position - BottomOfFingerPrint;
            float ChangeInPercent = (SizeBetweenBottomAndLaser.y / (BottomOfFingerPrint - LaserStartPos).y);

            PanelMask.transform.position = LASERPEWPEW.transform.position - (SizeBetweenBottomAndLaser * 0.5f);
            PanelMask.transform.localScale = new Vector3(PanelMask.transform.localScale.x, ChangeInPercent * PanelStartScale.y, PanelMask.transform.localScale.z);

            if (ChangeInPercent != 0.0f)
            {
                Point2.transform.localScale = new Vector3(Point2.transform.localScale.x, DictionaryOfVectors["Point2StartScale"].y / ChangeInPercent, Point2.transform.localScale.z);
            }

            Point2.transform.position = DictionaryOfVectors["Point2StartPoint"];

            TextInterval += Time.deltaTime;
            if (TextInterval >= 0.2f)
            {
                TextInterval = 0.0f;

                if (NewScanningText.text == Scanning)
                {
                    NewScanningText.text = "";
                }
                else if (NewScanningText.text.Length + 1 < Scanning.Length)
                {
                    NewScanningText.text += Scanning[NewScanningText.text.Length];
                    NewScanningText.text += Scanning[NewScanningText.text.Length];
                }
                else
                {
                    NewScanningText.text = Scanning;
                }
            }

            if (LASERPEWPEW.transform.position == LaserStartPos)
            {
                ThumbPrint.transform.SetParent(PanelMask.transform);
                Point1.transform.SetParent(PanelMask.transform);
                Point2.transform.SetAsLastSibling();

                DictionaryOfTriggers[CurrentState][ThumbSubState.LASER_MOVEDOWN]++;
            }
        }
        else if (DictionaryOfTriggers[CurrentState][ThumbSubState.LASER_MOVEDOWN] == 4)
        {
            LASERPEWPEW.transform.position = Vector3.MoveTowards(LASERPEWPEW.transform.position, BottomOfFingerPrint, Time.deltaTime * 0.3f);

            Vector3 SizeBetweenTopAndLaser = LASERPEWPEW.transform.position - BottomOfFingerPrint;
            float ChangeInPercent = (SizeBetweenTopAndLaser.y / (BottomOfFingerPrint - LaserStartPos).y);

            PanelMask.transform.position = LASERPEWPEW.transform.position - (SizeBetweenTopAndLaser * 0.5f);
            PanelMask.transform.localScale = new Vector3(PanelMask.transform.localScale.x, ChangeInPercent * PanelStartScale.y, PanelMask.transform.localScale.z);

            if (ChangeInPercent != 0.0f)
            {
                ThumbPrint.transform.localScale = new Vector3(ThumbPrint.transform.localScale.x, ThumbStartScale.y / ChangeInPercent, ThumbPrint.transform.localScale.z);
                Point1.transform.localScale = new Vector3(Point1.transform.localScale.x, DictionaryOfVectors["Point1StartScale"].y / ChangeInPercent, Point1.transform.localScale.z);
                Point2.transform.localScale = new Vector3(Point2.transform.localScale.x, DictionaryOfVectors["Point2StartScale"].y / ChangeInPercent, Point2.transform.localScale.z);
            }

            ThumbPrint.transform.position = ThumbStartPos;
            Point1.transform.position = DictionaryOfVectors["Point1StartPoint"];
            Point2.transform.position = DictionaryOfVectors["Point2StartPoint"];

            TextInterval += Time.deltaTime;
            if (TextInterval >= 0.2f)
            {
                TextInterval = 0.0f;

                if (NewScanningText.text == Scanning)
                {
                    NewScanningText.text = "";
                }
                else if (NewScanningText.text.Length + 1 < Scanning.Length)
                {
                    NewScanningText.text += Scanning[NewScanningText.text.Length];
                    NewScanningText.text += Scanning[NewScanningText.text.Length];
                }
                else
                {
                    NewScanningText.text = Scanning;
                }

                if (LASERPEWPEW.transform.position == BottomOfFingerPrint)
                {
                    DictionaryOfTriggers[CurrentState][ThumbSubState.LASER_MOVEDOWN]++;
                }
            }
        }
        else if (DictionaryOfTriggers[CurrentState][ThumbSubState.LASER_MOVEDOWN] == 5)
        {
            TextInterval += Time.deltaTime;
            if (TextInterval >= 0.2f)
            {
                TextInterval = 0.0f;

                if (NewScanningText.text == Scanning || NewScanningText.text == "")
                {
                    NewScanningText.text = "";
                    LASERPEWPEW.SetActive(false);
                    ThumbBackground.gameObject.SetActive(false);
                    PanelOfText.SetActive(false);

                    DictionaryOfTriggers[CurrentState][ThumbSubState.LASER_MOVEDOWN]++;

                    CurrentState = ThumbState.SCANCOMPLETE_STATE;
                }
                else if (NewScanningText.text.Length + 1 < Scanning.Length)
                {
                    NewScanningText.text += Scanning[NewScanningText.text.Length];
                    NewScanningText.text += Scanning[NewScanningText.text.Length];
                }
                else
                {
                    NewScanningText.text = Scanning;
                }



            }
        }

    }
    void HandplaceUpdate()
    {
        Color TextCurrentColor = CircleText.color;
        ScanningText.gameObject.SetActive(true);
        if (TextCurrentColor.a > 0)
        {
            TextCurrentColor.a -= Time.deltaTime * 2f;
            CircleText.color = TextCurrentColor;
        }
        else
        {
            Color CurrentHandprintColor = Handprint.color;

            if (CurrentHandprintColor.a < 1.0f)
            {
                CurrentHandprintColor.a += Time.deltaTime * 2f;
                Handprint.color = CurrentHandprintColor;

                ScanningText.gameObject.SetActive(true);
                PercentageText.gameObject.SetActive(true);
            }
            else
            {
                SubScanningUpdate();

            }
        }
    }

    void SubScanningUpdate()
    {
        Color PercentgeColor = PercentageText.color;

        PercentgeColor.a += Time.deltaTime * 3f;
        PercentageText.color = PercentgeColor;

        if (PercentgeColor.a >= 1.0f)
        {
            PercentageAmount += Time.deltaTime * 25.0f;
            PercentageAmount = Mathf.Clamp(PercentageAmount, 0.0f, 100.0f);
            PercentageText.text = "Percentage: " + PercentageAmount.ToString("N0") + "%";

        }

        TextInterval += Time.deltaTime;
        if (TextInterval > 0.12f)
        {
            TextCounter++;

            if (PreScanningText.Length + 1 == TextCounter)
            {
                TextCounter = PreScanningText.Length - 3;
            }

            if (TextCounter >= 0)
            {
                TextInterval = 0.0f;
                ScanningText.text = "";

                for (int i = 0; i < TextCounter; i++)
                {
                    ScanningText.text += PreScanningText[i];
                }
            }
        }

        if (BeamGoingDown)
        {
            if (FingerPrintCounter >= 0)
            {
                FingerInterval -= Time.deltaTime;
                if (FingerInterval <= 0.0f)
                {
                    FingerInterval = 0.35f;
                    FingerPrints[FingerPrintCounter].StartTheEffect();
                    FingerPrints[FingerPrintCounter].OppositeDirection(true);
                    FingerPrintCounter--;
                }
            }
            Vector3 CurrentPosition = Beam.transform.localPosition;

            CurrentPosition += new Vector3(-400, 0, 0) * Time.deltaTime;

            Beam.transform.localPosition = CurrentPosition;

            if (CurrentPosition.x <= -BeamYPosition)
            {
                BeamGoingDown = !BeamGoingDown;
                FingerPrintCounter++;
                FingerInterval = 0f;
            }
        }
        else if (Beam.transform.localPosition.x < BeamYPosition)
        {
            Vector3 CurrentPosition = Beam.transform.localPosition;

            CurrentPosition += new Vector3(400, 0, 0) * Time.deltaTime;

            Beam.transform.localPosition = CurrentPosition;
        }
        //else if (FingerPrintCounter < 5)
        if (!BeamGoingDown && FingerPrintCounter < 5)
        {
            FingerInterval -= Time.deltaTime;
            if (FingerInterval <= 0.0f)
            {
                FingerInterval = 0.35f;
                FingerPrints[FingerPrintCounter].StartTheEffect();
                FingerPrints[FingerPrintCounter].OppositeDirection(false);
                FingerPrintCounter++;
            }
        }

        int TextBoxDis = 0;

        if (PopUpCounter < TextScripts.Count)
        {
            PopUpTime -= Time.deltaTime;
            if (PopUpTime <= 0.0f)
            {
                PopUpTime = Random.Range(PopUpMin, PopUpMax);
                TextScripts[PopUpCounter].Activate();
                PopUpCounter++;
            }
        }
        else
        {
            foreach (TextScript TextBox in TextScripts)
            {
                if (TextBox.Ended)
                    TextBoxDis++;
            }
        }


        //if (PercentageAmount >= 100.0f && TextBoxDis == 5 && (Beam.transform.localPosition.x >= BeamYPosition && !BeamGoingDown))
        if (PercentageAmount >= 100.0f && (Beam.transform.localPosition.x >= BeamYPosition && !BeamGoingDown))
        {
            //CurrentState = ScanningState.COMPLETED_STATE;
            ScanningText.text = "SCAN COMPLETED";

            TextInterval = 0;

            GridMaster.EndOfScanning();
        }
    }

    void CompletedStateUpdate()
    {
        if (!HandGone)
        {
            Color CurrentHandprintColor = Handprint.color;
            CurrentHandprintColor.a -= Time.deltaTime * 4f;
            Handprint.color = CurrentHandprintColor;

            foreach (FingerPrint Finger in FingerPrints)
            {
                Color CurrentFingerprintColor = Finger.GetComponent<Image>().color;
                CurrentFingerprintColor.a = CurrentHandprintColor.a;

                Finger.GetComponent<Image>().color = CurrentFingerprintColor;
            }

            if (CurrentHandprintColor.a <= 0.0f)
            {
                HandGone = !HandGone;
                iTween.ScaleTo(SpinningDottedCircle, iTween.Hash("scale", new Vector3(0, 0, 0), "time", 0.75f, "easetype", "easeInOutQuint", "oncomplete", "CircleIsGone", "oncompletetarget", gameObject));


            }

        }

        if (!DelayEnd)
        {
            TextInterval += Time.deltaTime;
            if (TextInterval >= 0.5f)
            {
                TextInterval = 0.0f;
                DelayEnd = !DelayEnd;
            }
        }
        else
        {
            TextInterval += Time.deltaTime;

            if (TextInterval > textSpeed)
            {
                TextInterval = 0.0f;
                if (ScanningText.text.Length > 0)
                    ScanningText.text = ScanningText.text.Remove(ScanningText.text.Length - 1);

                if (PercentageText.text.Length > 0)
                    PercentageText.text = PercentageText.text.Remove(PercentageText.text.Length - 1);
            }
        }

        if (ScanningText.text.Length == 0 && PercentageText.text.Length == 0 /*&& GridMaster.FinishEnding*/ && CircleGone)
        {
            //CurrentState = ScanningState.WELCOME_STATE;
            TextInterval = 0;
        }
    }

    void CircleIsGone()
    {
        CircleGone = true;
    }

    void WelcomeUpdate()
    {
        TextInterval += Time.deltaTime;

        float Alpha = Mathf.Lerp(0.0f, 1.0f, TextInterval / 1);
        WelcomeText.color = new Color(WelcomeText.color.r, WelcomeText.color.g, WelcomeText.color.b, Alpha);
        SingaporeFlag.color = new Color(SingaporeFlag.color.r, SingaporeFlag.color.g, SingaporeFlag.color.b, Alpha);
        COAPhoto.color = new Color(COAPhoto.color.r, COAPhoto.color.g, COAPhoto.color.b, Alpha);
        Logo.color = new Color(Logo.color.r, Logo.color.g, Logo.color.b, Alpha);

        if (Alpha >= 1.0f)
        {
            CurrentState = ThumbState.WELCOME_STATE;
            TextInterval = 0;
        }
    }

    void BlackoutUpdate()
    {
        TextInterval += Time.deltaTime;
        float Alpha = Mathf.Lerp(0.0f, 1.0f, TextInterval / 1);
        Screenshot.color = new Color(Screenshot.color.r, Screenshot.color.g, Screenshot.color.b, Alpha);
    }
}

