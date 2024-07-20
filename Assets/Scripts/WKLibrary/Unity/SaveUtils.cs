using UnityEngine;
using System;

public static class SaveUtils
{
    static public void SetDate( string key, DateTime date )
    {
        string date_str = "";

        date_str += date.Year.ToString()    + "\n";
        date_str += date.Month.ToString()   + "\n";
        date_str += date.Day.ToString()     + "\n";
        date_str += date.Hour.ToString()    + "\n";
        date_str += date.Minute.ToString()  + "\n";
        date_str += date.Second.ToString();

        PlayerPrefs.SetString(key, date_str);
    }

    static public DateTime GetDate( string key, DateTime date )
    {
        string date_str = PlayerPrefs.GetString(key, "");
        if( date_str == "" )
        {
            return date;
        }
        else
        {
            char[] delimiter = { '\n' };
            string[] str_ary = date_str.Split( delimiter );
            return new DateTime(
                    Int32.Parse( str_ary[0] ) //year
                    , Int32.Parse( str_ary[1] ) //month
                    , Int32.Parse( str_ary[2] ) //day
                    , Int32.Parse( str_ary[3] ) //hour
                    , Int32.Parse( str_ary[4] ) //min
                    , Int32.Parse( str_ary[5] ) //sec
                    );
        }
    }
}
