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

public class NewScanManager : MonoBehaviour
{

    ScanningState CurrentState;

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
    // Use this for initialization
    void Start()
    {
        CurrentState = ScanningState.START_STATE;

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
        StateUpdate();
    }

    void StateUpdate()
    {
        switch (CurrentState)
        {
            case ScanningState.NONE_STATE:
                Debug.Log("Error, State at none");
                break;

            case ScanningState.START_STATE:
                StateStartUpdate();
                break;

            case ScanningState.JUSTHANDPLACE_STATE:
                HandplaceUpdate();
                break;

            case ScanningState.COMPLETED_STATE:
                CompletedStateUpdate();
                break;
            case ScanningState.WELCOME_STATE:
                WelcomeUpdate();
                break;
		case ScanningState.WELCOMEDONE_STATE:
				StartCoroutine ("LoadAppAfterDelay");
                CurrentState = ScanningState.BLACKOUT_STATE;

                break;
            case ScanningState.BLACKOUT_STATE:
				//SceneManager.LoadSceneAsync ("Start");
                break;
        }
    }
	
	IEnumerator LoadAppAfterDelay()		
	{
		yield return new WaitForSeconds(2);
		SceneManager.LoadSceneAsync ("Start");
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
				if (FingerEnd.GetComponent<Collider>().Raycast(ray,out hit, Mathf.Infinity))
				{
					ThisFingerTouch = true;
					NumberOfFingerTouched++;
					break;
				}
			}

			if (!ThisFingerTouch)
			{
				FingerEnd.transform.Rotate(new Vector3(0,0,10*Time.deltaTime));
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
		#if UNITY_EDITOR
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		Debug.DrawLine(ray.origin, ray.direction);

		if (Physics.Raycast(ray, out hit))
		{
			if (hit.collider.tag == "DottedCircle")
			{
				Cursor.SetCursor(CursorTexture, new Vector2(24, 24), CursorMode.Auto);

				if (Input.GetMouseButtonDown(0))
				{
					ClickTimes++;
					NumberCounter.text = ClickTimes.ToString();

					if (ClickTimes == 3)
					{
						Destroy(NumberCounter.gameObject);
						CurrentState = ScanningState.JUSTHANDPLACE_STATE;
						Debug.Log("Enter Placed");
						Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
						PopUpTime = Random.Range(PopUpMin, PopUpMax);

						GridMaster.ScanningStart();
					}
				}

			}
		}
		else
		{
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
		#else
		if (FiveFingersTouch()) {
			Destroy(NumberCounter.gameObject);
			CurrentState = ScanningState.JUSTHANDPLACE_STATE;
			Debug.Log("Enter Placed");
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
			PopUpTime = Random.Range(PopUpMin, PopUpMax);

			GridMaster.ScanningStart();
		}
       
		#endif
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
			if (FingerPrintCounter >= 0) {
				FingerInterval -= Time.deltaTime;
				if (FingerInterval <= 0.0f) {
					FingerInterval = 0.35f;
					FingerPrints [FingerPrintCounter].StartTheEffect ();
					FingerPrints [FingerPrintCounter].OppositeDirection (true);
					FingerPrintCounter--;
				}
			} 
            Vector3 CurrentPosition = Beam.transform.localPosition;

            CurrentPosition += new Vector3(-400,0, 0) * Time.deltaTime;

            Beam.transform.localPosition = CurrentPosition;

			if (CurrentPosition.x <= -BeamYPosition) {
				BeamGoingDown = !BeamGoingDown;
				FingerPrintCounter++;
				FingerInterval = 0f;
			}
        }
        else if (Beam.transform.localPosition.x < BeamYPosition)
        {
            Vector3 CurrentPosition = Beam.transform.localPosition;

            CurrentPosition += new Vector3(400,0, 0) * Time.deltaTime;

            Beam.transform.localPosition = CurrentPosition;
        }
        //else if (FingerPrintCounter < 5)
        if(!BeamGoingDown && FingerPrintCounter < 5)
        {
            FingerInterval -= Time.deltaTime;
            if (FingerInterval <= 0.0f)
            {
                FingerInterval = 0.35f;
                FingerPrints[FingerPrintCounter].StartTheEffect();
				FingerPrints [FingerPrintCounter].OppositeDirection (false);
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
            CurrentState = ScanningState.COMPLETED_STATE;
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
            CurrentState = ScanningState.WELCOME_STATE;
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

        float Alpha = Mathf.Lerp(0.0f, 1.0f, TextInterval/1);
        WelcomeText.color = new Color(WelcomeText.color.r, WelcomeText.color.g, WelcomeText.color.b ,Alpha);
        SingaporeFlag.color = new Color(SingaporeFlag.color.r, SingaporeFlag.color.g, SingaporeFlag.color.b, Alpha);
        COAPhoto.color = new Color(COAPhoto.color.r, COAPhoto.color.g, COAPhoto.color.b, Alpha);
        Logo.color = new Color(Logo.color.r, Logo.color.g, Logo.color.b, Alpha);

        if (Alpha >= 1.0f)
        {
            CurrentState = ScanningState.WELCOMEDONE_STATE;
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

