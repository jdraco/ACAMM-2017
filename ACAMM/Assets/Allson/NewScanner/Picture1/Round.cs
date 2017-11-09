using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Round : MonoBehaviour {

    Image ThisImage;
    float PreviousFillAmount;
    float AimFloatAmount;
    float CurrentLerpTime;
    float TimeToLerp = 0.5f;
	// Use this for initialization
	void Start () {
        ThisImage = GetComponent<Image>();
        PreviousFillAmount = ThisImage.fillAmount;
        AimFloatAmount = Random.Range(0f, 1.25f);

        CurrentLerpTime = 0.0f;

    }
	
	// Update is called once per frame
	void Update () {

        this.transform.Rotate(new Vector3(0,0,1) * -Time.deltaTime*120);

        CurrentLerpTime += Time.deltaTime;
        float PercLerp = CurrentLerpTime / TimeToLerp;

        ThisImage.fillAmount = Mathf.Lerp(PreviousFillAmount, AimFloatAmount, PercLerp);

        if (CurrentLerpTime > TimeToLerp)
        {
            CurrentLerpTime = 0.0f;
            PreviousFillAmount = AimFloatAmount;
            AimFloatAmount = Random.Range(0f, 1f); 
        }
    }
}
