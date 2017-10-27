using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum LineState
{
    STATE_STARTDRAWING,
    STATE_IDLE,
    STATE_MINUS,
    STATE_MINUSSTART,
    STATE_START
}

public class LineScript : MonoBehaviour {

    LineRenderer ThisLine;
    LineState CurrentState;

    float Min, Max;
    float PeperpendicularPosition;
    float ZAxis;

    float TimePassed = 0;
    float TotalTime = 0;
    float Delay = 0;


    bool IfX;
    public bool IfScanning = false;
    public bool FinishDrawing = false;
    public bool FinishUndrawing = false;
    // Use this for initialization
    void Awake() {
        ThisLine = GetComponent<LineRenderer>();
        CurrentState = LineState.STATE_IDLE;

    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentState)
        {
            case LineState.STATE_START:
                TimePassed += Time.deltaTime;
                if (TimePassed >= Delay)
                {
                    CurrentState = LineState.STATE_STARTDRAWING;
                    TimePassed -= Delay;
                    ThisLine.enabled = true;
                }
                break;
            case LineState.STATE_MINUSSTART:
                TimePassed += Time.deltaTime;
                if (TimePassed >= Delay)
                {
                    CurrentState = LineState.STATE_MINUS;
                    TimePassed -= Delay;
                }
                break;
            case LineState.STATE_STARTDRAWING:
                DrawingUpdate();
                break;

            case LineState.STATE_MINUS:
                MinusUpdate();
                break;
        }
    }

    public void SetUp(float TempMin, float TempMax, float TempPP, float TempZAxis, bool TempIfX, float OverallTime, float TempDelay)
    {
        Min = TempMin;
        Max = TempMax;
        PeperpendicularPosition = TempPP;
        ZAxis = TempZAxis;

        TotalTime = OverallTime;
        Delay = TempDelay;

        IfX = TempIfX;

        CurrentState = LineState.STATE_START;
        //switch (TempIfX)
        //{
        //    case true:

        //        ThisLine.SetPosition(0, new Vector3(TempMin, TempPP, TempZAxis));
        //        ThisLine.SetPosition(1, new Vector3(TempMax, TempPP, TempZAxis));

        //        break;

        //    case false:

        //        ThisLine.SetPosition(0, new Vector3(TempPP, TempMin, TempZAxis));
        //        ThisLine.SetPosition(1, new Vector3(TempPP, TempMax, TempZAxis));

        //        break;
        //}
    }

    void DrawingUpdate()
    {
        TimePassed += Time.deltaTime;
        float CurrentProgress = Mathf.Lerp(Min, Max, TimePassed / TotalTime);

        switch (IfX)
        {
            case true:

                ThisLine.SetPosition(0, new Vector3(Min, PeperpendicularPosition, ZAxis));
                ThisLine.SetPosition(1, new Vector3(CurrentProgress, PeperpendicularPosition, ZAxis));

                break;

            case false:

                ThisLine.SetPosition(0, new Vector3(PeperpendicularPosition, Min, ZAxis));
                ThisLine.SetPosition(1, new Vector3(PeperpendicularPosition, CurrentProgress, ZAxis));

                break;
        }

        if (TimePassed >= TotalTime)
        {
            CurrentState = LineState.STATE_IDLE;
            FinishDrawing = true;

            TimePassed = 0;
        }
    }

    void MinusUpdate()
    {
        TimePassed += Time.deltaTime;
        float CurrentProgress = Mathf.Lerp(Min, Max, TimePassed / TotalTime);

        switch (IfX)
        {
            case true:

                ThisLine.SetPosition(0, new Vector3(CurrentProgress, PeperpendicularPosition, ZAxis));
                ThisLine.SetPosition(1, new Vector3(Max, PeperpendicularPosition, ZAxis));

                break;

            case false:

                ThisLine.SetPosition(0, new Vector3(PeperpendicularPosition, CurrentProgress, ZAxis));
                ThisLine.SetPosition(1, new Vector3(PeperpendicularPosition, Max, ZAxis));

                break;
        }

        if (TimePassed >= TotalTime)
        {
            CurrentState = LineState.STATE_IDLE;
            TimePassed = 0;

            FinishUndrawing = true;
        }
    }

    public void StartDrawingAgain()
    {
        CurrentState = LineState.STATE_START;
        TimePassed = 0;

        FinishUndrawing = false;
    }

    public void MinusMode()
    {

        CurrentState = LineState.STATE_MINUSSTART;
        FinishDrawing = false;
    }
}



