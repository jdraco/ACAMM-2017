using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Signing : MonoBehaviour
{

    public Collider PlaneCollider;
    public GameObject SignaturePrefab;
    LineRenderer CurrentLine;
	//LineRenderer NetworkLine;
	LineRenderer[] NetworkLine = new LineRenderer[10];
    bool SigningBool = true;
    int PenID = -1;

	public float scrollValue = 101.9f;
	public Transform JDFolder;
	public float jdFolderDfX = 0;

    public GameObject CountryParent;
	public Dictionary<string, GameObject> DictionaryOfEmptyCountries = new Dictionary<string, GameObject>();
	public Dictionary<string, int> DictionaryOfCountryInt = new Dictionary<string, int>();
	public Dictionary<string, float> DictionaryOfCountryLastKnownOffset = new Dictionary<string, float>();
	public authentication_Manager authManager;
	public string country = "Singapore";
    // Use this for initialization
	public Vector3 mousepos = new Vector3();
	public Scrollbar JDScrollBar;
	public float mPosMaxOffset = 423;
	public float defHeight = 2736;

    public GameObject MC, SSCamOne, SSCamTwo;

    public List<GameObject> ThingsToSet = new List<GameObject>();

    public GameObject SSButton;

    void Start()
	{
		mPosMaxOffset = mPosMaxOffset * (Screen.height / defHeight);
		jdFolderDfX = JDFolder.position.x;
		authManager = GlobalValues.authManager;
		if (authManager != null) {
			authManager.signLocal = this;
			country = authManager.userName;
		}
		if (country == "Brunei" || country == "Cambodia" || country == "Indonesia" || country == "Laos") {
			JDScrollBar.value = 0.25f;
			ScrollPage (JDScrollBar);
		} else {
			JDScrollBar.value = 0.50f;
			ScrollPage (JDScrollBar);
		}
		int i = 0;
        foreach (Transform Countries in CountryParent.transform)
        {
            DictionaryOfEmptyCountries.Add(Countries.name, Countries.gameObject);
			DictionaryOfCountryInt.Add(Countries.name, i);
			DictionaryOfCountryLastKnownOffset.Add(Countries.name, 0);
			i++;
        }

        if (country == "Screenshot")
        {

        }
        else
        {
           //SSButton.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (country != "Screenshot")
        {
            //mousepos = Input.mousePosition;
            //Debug.Log(Input.mousePosition);
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (PlaneCollider.Raycast(ray, out hit, Mathf.Infinity))
                    SigningBool = true;
                else
                    SigningBool = false;
            }
            if (SigningBool && Input.GetMouseButton(0))
            {
                Vector3 VecOffset;
                SigningUpdate(Input.mousePosition, out VecOffset);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (authManager != null)
                    authManager.SendPenLiftup();
            }
            else
            {
                PenID = -1;
            }
        }
    }

	public void ScrollPage(Scrollbar sbar){
		//Debug.Log(Input.mousePosition);
		JDFolder.position = new Vector3 (jdFolderDfX + (scrollValue * sbar.value), JDFolder.position.y, JDFolder.position.z);
	}

	public void DeleteWritten(){
		for(int i = 0; i < DictionaryOfEmptyCountries [country].gameObject.transform.childCount; i++)
		{
			Destroy (DictionaryOfEmptyCountries [country].gameObject.transform.GetChild (i).gameObject);
		}
		authManager.SendPenDelete ();
	}

	public void DeleteWritten(string ncountry){
		for(int i = 0; i < DictionaryOfEmptyCountries [ncountry].gameObject.transform.childCount; i++)
		{
			Destroy (DictionaryOfEmptyCountries [ncountry].gameObject.transform.GetChild (i).gameObject);
		}
	}

	void SigningUpdate(Vector3 position, out Vector3 Offset)
    {
//#if UNITY_EDITOR

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Ray ray = Camera.main.ScreenPointToRay(position);
		RaycastHit hit = new RaycastHit();

        if (Input.GetMouseButtonDown(0))
        {
            if (PlaneCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                CurrentLine = Instantiate(SignaturePrefab).GetComponent<LineRenderer>();
                CurrentLine.positionCount = 2;
				CurrentLine.gameObject.transform.SetParent (DictionaryOfEmptyCountries [country].gameObject.transform);
                CurrentLine.SetPosition(0, hit.point + new Vector3(0, 0, -0.01f));
                CurrentLine.SetPosition(1, hit.point + new Vector3(0, 0, -0.01f));
				if (authManager != null) {
					Vector3 networkPosition = Input.mousePosition;
					networkPosition.x = networkPosition.x;// + (mPosMaxOffset * JDScrollBar.value);
					//authManager.SendSigningCoordinates (networkPosition);
					authManager.SendSigningCoordinates(networkPosition ,JDScrollBar.value);

				}
				//DictionaryOfCountryLastKnownOffset [country] = JDScrollBar.value;
			}
        }
        else if (Input.GetMouseButton(0))
        {
	        if (PlaneCollider.Raycast(ray, out hit, Mathf.Infinity))
	        {
				//Debug.Log ("jdscrollbar val " + JDScrollBar.value);
				if (authManager != null) {
					Vector3 networkPosition = Input.mousePosition;
					networkPosition.x = networkPosition.x;// + (mPosMaxOffset * JDScrollBar.value);
					//authManager.SendSigningCoordinates (networkPosition);
					authManager.SendSigningCoordinates(networkPosition ,JDScrollBar.value);
				}
				if (CurrentLine != null)// && JDScrollBar.value == DictionaryOfCountryLastKnownOffset [country])
	            {
					CurrentLine.positionCount++;
					CurrentLine.SetPosition (CurrentLine.positionCount - 1, hit.point + new Vector3 (0, 0, -0.01f));
	            }
	            else
	            {
	                CurrentLine = Instantiate(SignaturePrefab).GetComponent<LineRenderer>();
	                CurrentLine.positionCount = 2;
					CurrentLine.gameObject.transform.SetParent (DictionaryOfEmptyCountries [country].gameObject.transform);
	                CurrentLine.SetPosition(0, hit.point + new Vector3(0, 0, -0.01f));
	                CurrentLine.SetPosition(1, hit.point + new Vector3(0, 0, -0.01f));
					//DictionaryOfCountryLastKnownOffset [country] = JDScrollBar.value;
	            }
	        }
	        else
	        {
	            CurrentLine = null;
	        }
        }

		Offset = PlaneCollider.transform.position - hit.point;
//        else if (Input.GetMouseButtonUp(0))
//        {
//            //this is usless tbh
//        }
//#else
//        if (Input.touchCount > 0)
//        {
//            if (PenID == -1)
//            {
//                NoPenUpdate();
//            }
//            else
//            {
//                WithPenUpdate();
//            }
//        }
//        else
//        {
//            PenID = -1;
//        }
//#endif
    }

	public void SigningUpdateNetwork(Vector3 position, string name, float offset)
	{
		//Debug.Log (offset + " " + JDScrollBar.value);
		//Debug.Log (DictionaryOfCountryInt [name]);
		position.x = position.x +(mPosMaxOffset * (offset-JDScrollBar.value));
		Ray ray = Camera.main.ScreenPointToRay(position);
		RaycastHit hit;
		if(DictionaryOfCountryInt[name] != null)
		{
			if (PlaneCollider.Raycast(ray, out hit, Mathf.Infinity))
			{
				if (NetworkLine[DictionaryOfCountryInt[name]] != null)// && DictionaryOfCountryLastKnownOffset[name] == offset && DictionaryOfCountryLastKnownOffset [country] == JDScrollBar.value)
				{
					NetworkLine[DictionaryOfCountryInt[name]].positionCount++;
					NetworkLine[DictionaryOfCountryInt[name]].SetPosition(NetworkLine[DictionaryOfCountryInt[name]].positionCount - 1, hit.point + new Vector3(0, 0, -0.01f));
				}
				else
				{
					NetworkLine[DictionaryOfCountryInt[name]] = Instantiate(SignaturePrefab).GetComponent<LineRenderer>();
					NetworkLine[DictionaryOfCountryInt[name]].gameObject.name = name;
					NetworkLine[DictionaryOfCountryInt[name]].gameObject.transform.SetParent (DictionaryOfEmptyCountries [name].gameObject.transform);
					NetworkLine[DictionaryOfCountryInt[name]].positionCount = 2;
					NetworkLine[DictionaryOfCountryInt[name]].SetPosition(0, hit.point + new Vector3(0, 0, -0.01f));
					NetworkLine[DictionaryOfCountryInt[name]].SetPosition(1, hit.point + new Vector3(0, 0, -0.01f));
					//DictionaryOfCountryLastKnownOffset [name] = offset;
					//DictionaryOfCountryLastKnownOffset [country] = JDScrollBar.value;
				}
			}
			else
			{
				NetworkLine[DictionaryOfCountryInt[name]] = null;
			}
		}

//		if (DictionaryOfCountryInt [name] != null) 
//		{
//			if (NetworkLine [DictionaryOfCountryInt [name]] != null)
//			{
//				NetworkLine[DictionaryOfCountryInt[name]].positionCount++;
//				NetworkLine [DictionaryOfCountryInt [name]].SetPosition (NetworkLine [DictionaryOfCountryInt [name]].positionCount - 1, PlaneCollider.transform.position + position);
//								
//			}
//			else
//			{
//				NetworkLine[DictionaryOfCountryInt[name]] = Instantiate(SignaturePrefab).GetComponent<LineRenderer>();
//									NetworkLine[DictionaryOfCountryInt[name]].gameObject.name = name;
//									NetworkLine[DictionaryOfCountryInt[name]].gameObject.transform.SetParent (DictionaryOfEmptyCountries [name].gameObject.transform);
//									NetworkLine[DictionaryOfCountryInt[name]].positionCount = 2;
//				NetworkLine[DictionaryOfCountryInt[name]].SetPosition(0, PlaneCollider.transform.position + position + new Vector3(0, 0, -0.01f));
//				NetworkLine[DictionaryOfCountryInt[name]].SetPosition(1, PlaneCollider.transform.position + position+ new Vector3(0, 0, -0.01f));
//			}
//		}

	}
	public void SignLiftupNetwork(string name)
	{
		if (NetworkLine [DictionaryOfCountryInt [name]] != null) {
			NetworkLine[DictionaryOfCountryInt[name]] = null;
		}
	}
    void NoPenUpdate()
    {

        Touch UsedTouch = new Touch();
        foreach (Touch ThisTouch in Input.touches)
        {
            if (ThisTouch.type == TouchType.Stylus && ThisTouch.phase == TouchPhase.Began)
            {

                PenID = ThisTouch.fingerId;
                UsedTouch = ThisTouch;
                //Break to choose the first pen
                break;
            }
        }

        //If there is a pen
        if (PenID != -1)
        {
            Ray ray = Camera.main.ScreenPointToRay(UsedTouch.position);
            RaycastHit hit;
            if (PlaneCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                CurrentLine = Instantiate(SignaturePrefab).GetComponent<LineRenderer>();
                CurrentLine.positionCount = 2;
				CurrentLine.gameObject.transform.SetParent (DictionaryOfEmptyCountries [country].gameObject.transform);
                CurrentLine.SetPosition(0, hit.point + new Vector3(0, 0, -0.01f));
                CurrentLine.SetPosition(1, hit.point + new Vector3(0, 0, -0.01f));
            }
            else
            {
                PenID = -1;
            }

        }
    }

    void WithPenUpdate()
    {
        bool PenStillExist = false;
        Touch UsedTouch = new Touch();
        foreach (Touch ThisTouch in Input.touches)
        {
            if (ThisTouch.type == TouchType.Stylus && ThisTouch.phase == TouchPhase.Moved)
            {
                UsedTouch = ThisTouch;
                PenStillExist = true;

                break;
            }
        }

        if (PenStillExist)
        {
            Ray ray = Camera.main.ScreenPointToRay(UsedTouch.position);
            RaycastHit hit;
            if (PlaneCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (CurrentLine != null)
                {
                    CurrentLine.positionCount++;
                    CurrentLine.SetPosition(CurrentLine.positionCount - 1, hit.point + new Vector3(0, 0, -0.01f));
                }
                else
                {
                    CurrentLine = Instantiate(SignaturePrefab).GetComponent<LineRenderer>();
                    CurrentLine.positionCount = 2;
					CurrentLine.gameObject.transform.SetParent (DictionaryOfEmptyCountries [country].gameObject.transform);
                    CurrentLine.SetPosition(0, hit.point + new Vector3(0, 0, -0.01f));
                    CurrentLine.SetPosition(1, hit.point + new Vector3(0, 0, -0.01f));
                }
            }
            else
            {
                CurrentLine = null;
            }
        }
        else
        {
            CurrentLine = null;
            PenID = -1;
        }
    }


    public void TakeSS()
    {
        StartCoroutine(SSCouroutine());
    }


    IEnumerator SSCouroutine()
    {
        foreach (GameObject ToSet in ThingsToSet)
        {
            ToSet.SetActive(false);
        }

        MC.SetActive(false);
        
        SSCamOne.SetActive(true);
        ScreenCapture.CaptureScreenshot("Picture1.png");

        yield return new WaitForSeconds(2);

        SSCamOne.SetActive(false);
        SSCamTwo.SetActive(true);

        ScreenCapture.CaptureScreenshot("Picture2.png");

        yield return new WaitForSeconds(2);//


        SSCamOne.SetActive(false);
        SSCamTwo.SetActive(false);
        MC.SetActive(true);
        foreach (GameObject ToSet in ThingsToSet)
        {
            ToSet.SetActive(true);
        }
        yield return null ;
    }

  
}
