using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FingerPrint : MonoBehaviour {
    bool StartEffect = false;
	bool OppoDirection = false;
    float TimePassed = 0.0f;
    float TimeInterval = 0.05f;
    
    int RadialCounter = 0;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (StartEffect && RadialCounter < 6 && OppoDirection)
		{
			TimePassed += Time.deltaTime;

			if (TimePassed >= TimeInterval)
			{
				TimePassed -= TimeInterval;
				RadialCounter++; 
				this.GetComponent<Image>().fillAmount = 1-(1.0f / 6.0f) * (float)RadialCounter;

				if (RadialCounter == 6)
				{
					iTween.ScaleBy(gameObject, iTween.Hash("time", 0.5f, "amount", new Vector3(0.9f,0.9f,0.9f)));
				}
			}

		}
        else if (StartEffect && RadialCounter < 6)
        {
            TimePassed += Time.deltaTime;

            if (TimePassed >= TimeInterval)
            {
                TimePassed -= TimeInterval;
                RadialCounter++; 
                this.GetComponent<Image>().fillAmount = (1.0f / 6.0f) * (float)RadialCounter;

                if (RadialCounter == 6)
                {
                    iTween.ScaleBy(gameObject, iTween.Hash("time", 0.5f, "amount", new Vector3(0.9f,0.9f,0.9f)));
                }
            }
            
        }
	}

   public void StartTheEffect()
    {
		RadialCounter = 0;
        StartEffect = true;
    }

	public void OppositeDirection(bool od)
	{
		OppoDirection = od;
	}
}

