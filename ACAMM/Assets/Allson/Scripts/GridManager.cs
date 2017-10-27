using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {
    public GameObject X, Y;
    public LineScript LinePrefab;

    public float MinX, MaxX;
    public float MinY, MaxY;
    public float ZAxis;

    public int XNumLines, YNumLines;

    public float XDelay, YDelay;

    public float XInterval, YInterval;

    public float XLineRate, YLineRate;

    bool Scanning = false;
    bool ScanningEnded = false;
    public bool FinishEnding = false;

    List<LineScript> XListOfLines = new List<LineScript>(), YListOfLines = new List<LineScript>();
	// Use this for initialization
	void Start () {
        for(int i = 0;i< XNumLines; i++)
        {
            LineScript NewLine = Instantiate(LinePrefab, new Vector3(0, 0, 0), Quaternion.identity) as LineScript;
            NewLine.transform.parent = X.transform;

            NewLine.SetUp(MinX, MaxX, (((MaxY-MinY)/(XNumLines+1))*(i+1)) + MinY, ZAxis, true, XLineRate, XDelay + (XInterval*i));
            XListOfLines.Add(NewLine);

        }

        for (int j = 0; j < YNumLines; j++)
        {
            LineScript NewLine = Instantiate(LinePrefab, new Vector3(0, 0, 0), Quaternion.identity) as LineScript;
            NewLine.transform.parent = Y.transform;

            NewLine.SetUp(MinY, MaxY, (((MaxX - MinX) / (YNumLines + 1)) * (j + 1)) + MinX, ZAxis, false, YLineRate, YDelay + (YInterval * j));
            YListOfLines.Add(NewLine);
        }
    }

    public void ScanningStart()
    {
        Scanning = true;
    }

    public void EndOfScanning()
    {
        ScanningEnded = !ScanningEnded;
        foreach (LineScript Line in YListOfLines)
        {
            Line.MinusMode();
        }

        foreach (LineScript Line in XListOfLines)
        {
            Line.MinusMode();
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!ScanningEnded)
        {
            if (YListOfLines[YListOfLines.Count - 1].FinishDrawing && !Scanning)
            {
                foreach (LineScript Line in YListOfLines)
                {
                    Line.MinusMode();
                }

                foreach (LineScript Line in XListOfLines)
                {
                    Line.MinusMode();
                }
            }
            else if (YListOfLines[YListOfLines.Count - 1].FinishUndrawing)
            {
                foreach (LineScript Line in YListOfLines)
                {
                    Line.StartDrawingAgain();
                }

                foreach (LineScript Line in XListOfLines)
                {
                    Line.StartDrawingAgain();
                }
            }
        }
        else
        {
            if (YListOfLines[YListOfLines.Count - 1].FinishUndrawing)
            {
                FinishEnding = true;
            }
        }
    }
}
