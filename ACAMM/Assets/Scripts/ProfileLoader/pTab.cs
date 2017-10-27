using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pTab : MonoBehaviour {
	public Text Rank,Name,Value;
	public Image cPicture;
	public dbTypes.Profile profile;
	public ProfileLoader pLoader;

	//init profile data
	void Start () {
		Rank.text = profile.rank;
		Name.text = profile.name;
		Value.text = profile.role;

	}

	public void loadProfile()
	{
		pLoader.loadProfile (profile);
	}
}
