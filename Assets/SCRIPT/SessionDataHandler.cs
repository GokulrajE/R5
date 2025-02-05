using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using UnityEngine;
using static AppData;

public static class SessionDataHandler
{

    public static float[] moveTimeData;
    public static string[] dateData;
    public static string DATEFORMAT = "dd/MM";

    //SESSION FILE  HEADER FORMAT AND DATETIME FORMAT
    public static string DATEFORMAT_INFILE = "dd-MM-yyyy HH:mm:ss";
    public static string DATETIME = "DateTime";
    public static string MOVETIME = "moveTime";
    public static string STARTTIME = "StartTime";
    public static string STOPTIME = "StopTime";
    public static string MOVEMENT = "Mechanism";
    static public string movement = "Mechanism";
    static public string moveTime = "MoveTime";
    static public String dateTime = "DateTime";
    static public string dateTimeFormat = "dd-MM-yyyy HH:mm:ss";
   
  
    public static void parseMoveTimecurr()
    {
        
        var _totalMoveTime = UserData.dTableSession.AsEnumerable()
            .Where(row => DateTime.ParseExact(row.Field<string>(dateTime), dateTimeFormat, CultureInfo.InvariantCulture).Date == DateTime.Now.Date)
            .Sum(row => Convert.ToInt32(row[moveTime]));
        UserData.MovetimeCurr = (int)(_totalMoveTime / 60f);
        Debug.Log(_totalMoveTime+"totalmovetime");
       
    }

    //CALCULATE MOVETIME PER DAY FOR ALL MOVEMENTS
    public static  void MovTimePerDay()
    {
        var movTimePerDay = UserData.dTableSession.AsEnumerable()
            .GroupBy(row => DateTime.ParseExact(row.Field<string>(DATETIME), DATEFORMAT_INFILE, CultureInfo.InvariantCulture).Date) // Group by date only
            .Select(group => new
            {
                Date = group.Key,
                DayOfWeek = group.Key.DayOfWeek,   // Get the day of the week
                TotalMovTime = group.Sum(row => Convert.ToInt32(row[MOVETIME]))
            })
            .ToList();
        moveTimeData = new float[movTimePerDay.Count];
        dateData = new string[movTimePerDay.Count];
       
        for (int i = 0; i < movTimePerDay.Count; i++)
        {
            moveTimeData[i] = movTimePerDay[i].TotalMovTime / 60f; // Convert seconds to minutes
           
            dateData[i] = movTimePerDay[i].Date.ToString(DATEFORMAT);       // Format date as "dd/MM"
           
        }
    }

   
      

    /*
    * Calculate the movement time for each training day.
    */
    public static DaySummary[] CalculateMoveTimePerDay(int noOfPastDays = 7)
    {
        DateTime today = DateTime.Now.Date;
        DaySummary[] daySummaries = new DaySummary[noOfPastDays];
        // Find the move times for the last seven days excluding today. If the date is missing, then the move time is set to zero.
        for (int i = 1; i <= noOfPastDays; i++)
        {
            DateTime _day = today.AddDays(-i);
            // Get the summary data for this date.
            var _moveTime = UserData.dTableSession.AsEnumerable()
                .Where(row => DateTime.ParseExact(row.Field<string>(dateTime), dateTimeFormat, CultureInfo.InvariantCulture).Date == _day)
                .Sum(row => Convert.ToInt32(row[moveTime]));
            // Create the day summary.
            daySummaries[i - 1] = new DaySummary
            {
                Day = Miscellaneous.GetAbbreviatedDayName(_day.DayOfWeek),
                Date = _day.ToString(DATEFORMAT),
                MoveTime = _moveTime / 60f
            };
            Debug.Log($"{i} | {daySummaries[i - 1].Day} | {daySummaries[i - 1].Date} | {daySummaries[i - 1].MoveTime}");
        }
        return daySummaries;
    }
    public static class Miscellaneous
    {
        public static string GetAbbreviatedDayName(DayOfWeek dayOfWeek)
        {
            return dayOfWeek.ToString().Substring(0, 3);
        }
    }
}
