using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sTab : MonoBehaviour {

	public Text value, day, date, time, event_, location;
	public dbTypes.Schedule schedule;
	public ScheduleLoader sLoader;

	void Start () {
		day.text = "Day " + schedule.day;
		time.text = schedule.time;
		date.text = dateConvert(schedule.date);
		event_.text = schedule.event_;
		if (schedule.location != "NIL")
			location.text = schedule.location;
		else
			location.text = "";
	}

	//converts date format to day/month
	//only has november for now
	string dateConvert(string date){
		string[] dateSplit = new string[3];
		dateSplit = date.Split("/"[0]);
		string newDate = "";
		switch (int.Parse(dateSplit [1])) {
		case 11:
			newDate = dateSplit [0] + " " + "Nov";
			return newDate;
		default:
			newDate = dateSplit [0] + " " + "Nov";
			return newDate;
		}
		return date;
	}
		
}
