using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signing : MonoBehaviour
{

    public Collider PlaneCollider;
    public GameObject SignaturePrefab;
    LineRenderer CurrentLine;
    bool SigningBool = true;
    int PenID = -1;

    public GameObject CountryParent;
    Dictionary<string, GameObject> DictionaryOfEmptyCountries = new Dictionary<string, GameObject>();

    // Use this for initialization
    void Start()
    {
        foreach (Transform Countries in CountryParent.transform)
        {
            DictionaryOfEmptyCountries.Add(Countries.name, Countries.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SigningBool)
        {
            SigningUpdate();
        }
        else
        {
            PenID = -1;
        }
    }

    void SigningUpdate()
    {
#if UNITY_EDITOR

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0))
        {
            if (PlaneCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                CurrentLine = Instantiate(SignaturePrefab).GetComponent<LineRenderer>();
                CurrentLine.positionCount = 2;
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
                    CurrentLine.SetPosition(0, hit.point + new Vector3(0, 0, -0.01f));
                    CurrentLine.SetPosition(1, hit.point + new Vector3(0, 0, -0.01f));
                }
            }
            else
            {
                CurrentLine = null;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            //this is usless tbh
        }
#else
        if (Input.touchCount > 0)
        {
            if (PenID == -1)
            {
                NoPenUpdate();
            }
            else
            {

            }
        }
        else
        {
            PenID = -1;
        }
#endif
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

    }

}
