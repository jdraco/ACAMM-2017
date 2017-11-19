using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pTab : MonoBehaviour {
	public Text Name,Value;
	public Image cPicture;
	public dbTypes.Profile profile;
	public ProfileLoader pLoader;

	//init profile data
	void Start () {
		//Name.text = profile.rank +" "+ profile.name;
		//Value.text = profile.role;

	}

	public void loadProfile()
	{
		pLoader.loadProfile (profile);
	}

    public void LoadInfo()
    {
       if(profile.rank != "")
            Name.text = profile.rank +" "+ profile.name;
       else
            Name.text = profile.name;

        Value.text = profile.role;
    }
}
