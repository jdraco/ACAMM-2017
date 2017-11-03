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
	public authentication_Manager authManager;
	public string country = "Singapore";
    // Use this for initialization
	//public Vector3 mousepos = new Vector3();
	public Scrollbar JDScrollBar;
	public float mPosMaxOffset = 423;
    void Start()
	{
		jdFolderDfX = JDFolder.position.x;
		authManager = GlobalValues.authManager;
		if (authManager != null) {
			authManager.signLocal = this;
			country = authManager.userName;
		}
		if (country == "Brunei" || country == "Cambodia" || country == "Indonesia" || country == "Laos") {
		} else {
			JDScrollBar.value = 1;
			ScrollPage (JDScrollBar);
		}
		int i = 0;
        foreach (Transform Countries in CountryParent.transform)
        {
            DictionaryOfEmptyCountries.Add(Countries.name, Countries.gameObject);
			DictionaryOfCountryInt.Add(Countries.name, i);
			i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
		//mousepos = Input.mousePosition;
		//Debug.Log(DictionaryOfEmptyCountries["Singapore"].name);
		if (Input.GetMouseButtonDown(0)){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (PlaneCollider.Raycast (ray, out hit, Mathf.Infinity))
				SigningBool = true;
			else
				SigningBool = false;
		}
		if (SigningBool && Input.GetMouseButton (0)) {
			SigningUpdate (Input.mousePosition);
			if (authManager != null) {
				Vector3 networkPosition = Input.mousePosition;
				networkPosition.x = networkPosition.x + (mPosMaxOffset * JDScrollBar.value);
				authManager.SendSigningCoordinates (networkPosition);
			}
		} else if (Input.GetMouseButtonUp (0)) {
			if (authManager != null)
				authManager.SendPenLiftup ();
		}
        else
        {
            PenID = -1;
        }
    }

	public void ScrollPage(Scrollbar sbar){
		JDFolder.position = new Vector3 (jdFolderDfX - (scrollValue * sbar.value), JDFolder.position.y, JDFolder.position.z);
	}

	void SigningUpdate(Vector3 position)
    {
//#if UNITY_EDITOR

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0))
        {
            if (PlaneCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                CurrentLine = Instantiate(SignaturePrefab).GetComponent<LineRenderer>();
                CurrentLine.positionCount = 2;
				CurrentLine.gameObject.transform.SetParent (DictionaryOfEmptyCountries [country].gameObject.transform);
                CurrentLine.SetPosition(0, hit.point + new Vector3(0, 0, -0.01f));
                CurrentLine.SetPosition(1, hit.point + new Vector3(0, 0, -0.01f));
            }
        }
        else if (Input.GetMouseButton(0))
        {
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

	public void SigningUpdateNetwork(Vector3 position, string name)
	{
		Debug.Log (DictionaryOfCountryInt [name]);
		position.x = position.x -(mPosMaxOffset * JDScrollBar.value);
		Ray ray = Camera.main.ScreenPointToRay(position);
		RaycastHit hit;
		if(DictionaryOfCountryInt[name] != null)
		{
			if (PlaneCollider.Raycast(ray, out hit, Mathf.Infinity))
			{
				if (NetworkLine[DictionaryOfCountryInt[name]] != null)
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
				}
			}
			else
			{
				NetworkLine[DictionaryOfCountryInt[name]] = null;
			}
		}

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

}
