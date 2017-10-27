using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextScript : MonoBehaviour
{

    bool TextActivated = false;
    string BlockOfText = "<p class=\" text\" data-text= \" struct group_info init_groups = { .usage = ATOMIC_INIT(2) }; struct group_info * groups_alloc(int gidsetsize) { struct group_info * group_info; int nblocks; int i; nblocks = (gidsetsize + NGROUPS_PER_BLOCK - 1) / NGROUPS_PER_BLOCK; /n /* Make sure we always allocate at least one indirect block pointer */ nblocks = nblocks ? : 1; group_info = kmalloc(sizeof(*group_info) + ";
    Text ChildTextBlock;
    string[] BrokenUpText;
    int TextCounter;
    public bool Ended = false;

    float TimeInterval;


    // Use this for initialization
    void Start()
    {
        gameObject.SetActive(false);
        ChildTextBlock = GetComponentInChildren<Text>();
        ChildTextBlock.text = "";

        TimeInterval = 0.01f;
        TextCounter = 0;
        BrokenUpText = BlockOfText.Split(' ');
    }

    // Update is called once per frame
    void Update()
    {
        if (TextActivated)
        {
            TimeInterval -= Time.deltaTime;
            if (TimeInterval <= 0.0f)
            {
                TimeInterval = 0.03f;
                ChildTextBlock.text += BrokenUpText[TextCounter] + ' ';
                TextCounter++;
                if(TextCounter == BrokenUpText.Length)
                {
                    FinishTexting();
                }
            }
        }
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        iTween.ScaleFrom(gameObject, iTween.Hash("scale", new Vector3(0, 0, 0), "time", 0.4f, "easetype", "easeInOutQuint", "oncomplete", "FinishExpanding", "oncompletetarget", gameObject));
    }

    void FinishExpanding()
    {
        TextActivated = true;
        Debug.Log("I've reached here");
    }

    void FinishTexting()
    {

        TextActivated = false;
        iTween.ScaleTo(gameObject, iTween.Hash("scale", new Vector3(0, 0, 0), "time", 0.2f, "easetype", "easeInOutQuint", "oncomplete", "TextBoxClosed"));
    }

    void TextBoxClosed()
    {
        Ended = true;
    }
}
