using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileLoader : MonoBehaviour {
	public InitDB DB;
	public GameObject pTabPrefab;
	public List<GameObject> profileList = new List<GameObject>();
	public GameObject pTabContainer;
	public pStat statContainer;
	public Texture2D loading;
	public bool viewProfile = false;
	public l_sceneManager lsm;
	public float defScreenWidth = 1280;
	public float widthRatio = 1;
	public Sprite unknownSprite;
	public GameObject[] commentBox;

    public float SpacingBetweenTabs = 10.0f;
    public Canvas ThisCanvas;

    bool SetEverything = false;
    int FrameChecker = 0;

	//show list of personnel or stats of specific personnel
	public enum state{
		list,
		stat
	}

	//stats to be loaded
	public class stats{
		public int value = 0;
		public string name = "";
		public string dob = "";
		public string country = "";
		public string rank = "";
		public string comment = "";
		public string picture = "";
	}

	//sprites of each country
	[System.Serializable]
	public class aseanPicture
	{
		public Sprite SG;
		public Sprite TH;
		public Sprite VN;
		public Sprite BN;
		public Sprite CM;
		public Sprite ID;
		public Sprite LS;
		public Sprite MY;
		public Sprite MYR;
		public Sprite PH;
	}

	public aseanPicture ap;

	// Use this for initialization
	// init list of personnel based on country selected
	void Start () {
		int i = 0;
		//widthRatio = Screen.width / defScreenWidth;
        foreach (dbTypes.Profile tProfile in DB.profileList)
		{
            //Vector3 tabPos = pTabContainer.transform.position;
            //tabPos.y += i * (-distBetweenProfileTabs	);
            //GameObject newObj = Instantiate(pTabPrefab, tabPos, Quaternion.identity);

            GameObject newObj = Instantiate(pTabPrefab);

            pTab ThisPTab = newObj.GetComponent<pTab>();

            ThisPTab.profile = tProfile;
            ThisPTab.pLoader = this;
            ThisPTab.LoadInfo();

            switch (tProfile.country) {
			case "SINGAPORE":
				newObj.GetComponent<pTab> ().cPicture.sprite = ap.SG;
				break;
			case "THAILAND":
				newObj.GetComponent<pTab> ().cPicture.sprite = ap.TH;
				break;
			case "VIETNAM":
				newObj.GetComponent<pTab> ().cPicture.sprite = ap.VN;
				break;
			case "BRUNEI":
				newObj.GetComponent<pTab> ().cPicture.sprite = ap.BN;
				break;
			case "CAMBODIA":
				newObj.GetComponent<pTab> ().cPicture.sprite = ap.CM;
				break;
			case "INDONESIA":
				newObj.GetComponent<pTab> ().cPicture.sprite = ap.ID;
				break;
			case "LAOS":
				newObj.GetComponent<pTab> ().cPicture.sprite = ap.LS;
				break;
			case "MALAYSIA":
				newObj.GetComponent<pTab> ().cPicture.sprite = ap.MY;
				break;
			case "MYANMAR":
				newObj.GetComponent<pTab> ().cPicture.sprite = ap.MYR;
				break;
			case "PHILIPPINES":
				newObj.GetComponent<pTab> ().cPicture.sprite = ap.PH;
				break;
			default:
				break;
			}

            profileList.Add (newObj);
			i++;
		}
	}
	
	void Update() {
		if (Input.GetKeyUp ("escape"))
			back ();

       

        //foreach (GameObject ThisTab in profileList)
        //{

        //    RectTransform ThisRectTransform = ThisTab.GetComponent<RectTransform>();

        //    //newObj.transform.Rotate (0, 0, -90);
        //    pTab ThisPTab = ThisTab.GetComponent<pTab>();

        //    //PreviousBaseHeight = 

        //    float TopOfTab = ThisTab.transform.position.y + ((ThisRectTransform.lossyScale.y * ThisRectTransform.sizeDelta.y) * 0.5f);
        //    float BottomOfTab = ThisPTab.Value.GetComponent<RectTransform>().position.y - ((ThisPTab.Value.GetComponent<RectTransform>().lossyScale.y * ThisPTab.Value.GetComponent<RectTransform>().sizeDelta.y) * 0.5f);

        //    //ThisRectTransform.
        //    float SizeOfNewTab = TopOfTab - BottomOfTab;

        //    ThisRectTransform.sizeDelta = new Vector2(ThisRectTransform.sizeDelta.x, (SizeOfNewTab/ ThisRectTransform.lossyScale.y)); 

        //    Debug.Log(ThisPTab.profile.name + " " + SizeOfNewTab);
        //}
    }

    void LateUpdate()
    {

        if (FrameChecker <= 2)
        {
            FrameChecker++;
        }

        if (FrameChecker == 2)
        {
            int i = 0;
            Vector3 TopOfProfileList = pTabContainer.transform.position;// + new Vector3(0, (pTabContainer.transform.lossyScale.y * pTabContainer.GetComponent<RectTransform>().sizeDelta.y) , 0);
            foreach (GameObject ThisTab in profileList)
            {

                ThisTab.transform.SetParent(pTabContainer.transform);

                RectTransform ThisRectTransform = ThisTab.GetComponent<RectTransform>();

                float PreviousBaseHeight, PreviousBasePosY;

                pTab ThisPTab = ThisTab.GetComponent<pTab>();

                float BottomOfName = ThisPTab.Name.GetComponent<RectTransform>().position.y - ((ThisPTab.Name.GetComponent<RectTransform>().lossyScale.y * ThisPTab.Name.GetComponent<RectTransform>().sizeDelta.y));
                ThisPTab.Value.GetComponent<RectTransform>().position = new Vector3(ThisPTab.Name.GetComponent<RectTransform>().position.x, BottomOfName - 0.5f, ThisPTab.Name.GetComponent<RectTransform>().position.z);

                float TopOfTab = ThisTab.transform.position.y + ((ThisRectTransform.lossyScale.y * ThisRectTransform.sizeDelta.y) * 0.5f);
                float BottomOfTab = ThisPTab.Value.GetComponent<RectTransform>().position.y - ((ThisPTab.Value.GetComponent<RectTransform>().lossyScale.y * ThisPTab.Value.GetComponent<RectTransform>().sizeDelta.y));

                //ThisRectTransform. 
                float SizeOfNewTab = TopOfTab - BottomOfTab;

                ThisRectTransform.sizeDelta = new Vector2(ThisRectTransform.sizeDelta.x, ((SizeOfNewTab + 2) / ThisRectTransform.lossyScale.y));

                Debug.Log(ThisPTab.profile.name + SizeOfNewTab);


                //Set Tab/Box Position
                float SizeOfTab = ThisRectTransform.lossyScale.y * ThisRectTransform.sizeDelta.y;
                if (i == 0)
                {

                    ThisTab.transform.position = TopOfProfileList;
                    ThisTab.transform.position -= new Vector3(0, SizeOfTab * 0.5f, 0);
                }
                else
                {
                    float SizeOfPreviousTab = profileList[i - 1].transform.lossyScale.y * profileList[i - 1].GetComponent<RectTransform>().sizeDelta.y;

                    ThisTab.transform.position = profileList[i - 1].transform.position - new Vector3(0, SizeOfPreviousTab * 0.5f, 0);
                    ThisTab.transform.position -= new Vector3(0, SizeOfTab * 0.5f, 0);
                    ThisTab.transform.position -= new Vector3(0, SpacingBetweenTabs, 0);
                }

                //ThisRectTransform.anchorMin = new Vector2(0.5f, 1f);
                //ThisRectTransform.anchorMax = new Vector2(0.5f, 1f);
                //ThisRectTransform.pivot = new Vector2(0.5f, 1.0f);

                if (profileList.Count - 1 == i)
                {
                    float TopOfProfile = pTabContainer.transform.position.y;// + ((ThisRectTransform.transform.lossyScale.y * pTabContainer.GetComponent<RectTransform>().sizeDelta.y) * 0.5f);
                    float BottomOfProfile = ThisRectTransform.position.y - (ThisRectTransform.transform.lossyScale.y * ThisRectTransform.sizeDelta.y * 0.5f) - 1.5f;
                    float SizeOfNewProfile = TopOfProfile - BottomOfProfile;

                    pTabContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(pTabContainer.GetComponent<RectTransform>().sizeDelta.x, (SizeOfNewProfile/pTabContainer.transform.lossyScale.y)); ;
                }

                i++;
            }
        }
    }

    //load a specific personnel profile
    public void loadProfile(dbTypes.Profile profile)
    {
        statContainer.value.text = profile.role;//.ToString();
        statContainer.name.text = profile.name;
        statContainer.dob.text = profile.dob;
        statContainer.country.text = profile.country;
        statContainer.rank.text = profile.rank;
        if (profile.comment != "NIL") {
            statContainer.comment.text = profile.comment;
            foreach (GameObject cbox in commentBox) {
                cbox.SetActive(true);
            }
        } else {
            foreach (GameObject cbox in commentBox) {
                cbox.SetActive(false);
            }
        }
        //statContainer.picture.text = profile.picture;
        switch (profile.country) {
            case "SINGAPORE":
                statContainer.cPicture.sprite = ap.SG;
                break;
            case "THAILAND":
                statContainer.cPicture.sprite = ap.TH;
                break;
            case "VIETNAM":
                statContainer.cPicture.sprite = ap.VN;
                break;
            case "BRUNEI":
                statContainer.cPicture.sprite = ap.BN;
                break;
            case "CAMBODIA":
                statContainer.cPicture.sprite = ap.CM;
                break;
            case "INDONESIA":
                statContainer.cPicture.sprite = ap.ID;
                break;
            case "LAOS":
                statContainer.cPicture.sprite = ap.LS;
                break;
            case "MALAYSIA":
                statContainer.cPicture.sprite = ap.MY;
                break;
            case "MYANMAR":
                statContainer.cPicture.sprite = ap.MYR;
                break;
            case "PHILIPPINES":
                statContainer.cPicture.sprite = ap.PH;
                break;
            default:
                break;
        }
        if (profile.picture != "NIL")
            StartCoroutine("loadImage", profile.picture);
        else
        { 
            statContainer.picture.sprite = unknownSprite;
            statContainer.picture.rectTransform.sizeDelta = new Vector2(350,350);
        }
		statContainer.gameObject.SetActive (true);
		pTabContainer.transform.parent.parent.gameObject.SetActive (false);
		viewProfile = true;
	}

	//corutine to load an image, note that all www loading needs to be ienumerator or else will hang for abit/not work
	public IEnumerator loadImage(string url)
	{
		// Start a download of the given URL
		WWW www = new WWW(url);
		statContainer.picture.sprite = Sprite.Create(loading, new Rect(0, 0, loading.width, loading.height), new Vector2(0, 0));
		// Wait for download to complete
		yield return www;

		// assign texture
		statContainer.picture.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));

        float RatioForX = (350.0f / www.texture.height) * (www.texture.width);

        statContainer.picture.rectTransform.sizeDelta = new Vector2(RatioForX, statContainer.picture.rectTransform.sizeDelta.y) ;
	}

	//exit viewing specific personnel profile
	public void exitProfile()
	{
		statContainer.gameObject.SetActive (false);
        pTabContainer.transform.parent.parent.gameObject.SetActive (true);
		viewProfile = false;
	}

	public void back(){
		if (viewProfile)
			exitProfile ();
		else
			#if UNITY_ANDROID
			lsm.changeScene ("profileselector_and");
			#else
			lsm.changeScene ("profileselector");	
			#endif
	}
}
