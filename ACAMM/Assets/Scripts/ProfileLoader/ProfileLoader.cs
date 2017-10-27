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
	public float distBetweenProfileTabs = 0;
	public float defScreenWidth = 1280;
	public float widthRatio = 1;
	public Sprite unknownSprite;

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
		widthRatio = Screen.width / defScreenWidth;
		foreach(dbTypes.Profile tProfile in DB.profileList)
		{
			Vector3 tabPos = pTabContainer.transform.position;
			tabPos.y += i * (-distBetweenProfileTabs	);
			GameObject newObj = Instantiate(pTabPrefab, tabPos, Quaternion.identity);
			//newObj.transform.Rotate (0, 0, -90);
			newObj.GetComponent<pTab> ().profile = tProfile;
			newObj.GetComponent<pTab> ().pLoader = this;
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
			newObj.transform.parent = pTabContainer.transform;

			profileList.Add (newObj);
			i++;
		}
	}
	
	void Update() {
		if (Input.GetKeyUp ("escape"))
			back ();
	}

	//load a specific personnel profile
	public void loadProfile(dbTypes.Profile profile)
	{
		statContainer.value.text = profile.role;//.ToString();
		statContainer.name.text = profile.name;
		statContainer.dob.text = profile.dob;
		statContainer.country.text = profile.country;
		statContainer.rank.text = profile.rank;
		statContainer.comment.text = profile.comment;
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
			StartCoroutine ("loadImage", profile.picture);
		else
			statContainer.picture.sprite = unknownSprite;
		statContainer.gameObject.SetActive (true);
		pTabContainer.gameObject.SetActive (false);
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
	}

	//exit viewing specific personnel profile
	public void exitProfile()
	{
		statContainer.gameObject.SetActive (false);
		pTabContainer.gameObject.SetActive (true);
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
