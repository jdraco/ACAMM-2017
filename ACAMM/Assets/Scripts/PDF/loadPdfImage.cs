using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loadPdfImage : MonoBehaviour {

    public string[] ImageLocation = new string[0];
    public GameObject[] Pages = new GameObject[0];
    // Use this for initialization
    void Start () {
        StartCoroutine(loadIMG(0,ImageLocation.Length-1));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetImage(string url, GameObject page, int i)
    {
        ImageLocation[i] = url;
        Pages[i] = page;
    }

    //public IEnumerator loadIMGList(string[] url)
    //{
    //    for (int i = 0; i < url.Length; i++) {
    //        StartCoroutine(loadIMG(i));
    //        yield return new WaitForSeconds(0.5f);
    //    }
        
    //}

    public IEnumerator loadIMG(int i,int maxi)
    {
        WWW www = new WWW(ImageLocation[i]);

        // Wait for download to complete
        yield return www;

        Pages[i].GetComponent<Image>().sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
        if (i < maxi)
        {
            i++;
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(loadIMG(i, maxi));
            
        }
        
    }
}
