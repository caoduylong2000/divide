using System;
using UnityEngine;

namespace WK {
namespace Utils {

public static class Utils {
    //------------------------------------------------------------------------------
    public static DateTime StringToDateTime( string date_time_str )
    {
        char[] delimiter = { ',' };
        string[] days = date_time_str.Split( delimiter );

        int year  = Int32.Parse( days[0] );
        int month = Int32.Parse( days[1] );
        int day   = Int32.Parse( days[2] );
        int hour  = Int32.Parse( days[3] );
        int min   = Int32.Parse( days[4] );
        int sec   = Int32.Parse( days[5] );

        return new DateTime( year, month, day, hour, min, sec );
    }

    //------------------------------------------------------------------------------
    public static string DateTimeToString( DateTime date_time )
    {
        return date_time.Year   + ","
             + date_time.Month  + ","
             + date_time.Day    + ","
             + date_time.Hour   + ","
             + date_time.Minute + ","
             + date_time.Second;
    }

    //------------------------------------------------------------------------------
    public static void Swap<T>(ref T lhs, ref T rhs)
    {
        T temp;
        temp = lhs;
        lhs = rhs;
        rhs = temp;
    }

    //------------------------------------------------------------------------------
    public static void Shuffle<T>(ref T[] array, System.Random rnd )
    {
        int length = array.Length;
		for( int i = 0; i < length; ++i )
        {
            Utils.Swap( ref array[ i ], ref array[ rnd.Next( 0, length ) ] );
        }
    }

    //------------------------------------------------------------------------------
    public static void Shuffle<T>(ref T[] array)
    {
        System.Random rnd = new System.Random();
        Shuffle<T>( ref array, rnd );
    }

    //------------------------------------------------------------------------------
    public static void FillWithDefault<Type>(Type[] array)
    {
        for(int i=0; i<array.Length; ++i)
            array[i] = default(Type);
    }

    //------------------------------------------------------------------------------
    public static int CountChar(string str, char c) {
        return str.Length - str.Replace(c.ToString(), "").Length;
    }

    //------------------------------------------------------------------------------
    private static DateTime cUNIXEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
    public static long GetUnixTime( DateTime target )
    {
        target = target.ToUniversalTime();

        TimeSpan elapsed = target - cUNIXEpoch;

        // 経過秒数に変換
        return (long)elapsed.TotalSeconds;
    }

    //------------------------------------------------------------------------------
    //------------------------------------------------------------------------------
    //------------------------------------------------------------------------------
    // using Unity 
    public static int GetRandomSign()
    {
        return UnityEngine.Random.Range( 0, 2 ) * 2 - 1;
    }

    //------------------------------------------------------------------------------
    public static Transform FindChildRecursively( Transform parent, string name) 
    {
        var child = parent.Find( name );
        if( child != null )
        {
            return child;
        }
        else
        {
            foreach(Transform children in parent)
            {
                child = FindChildRecursively(children, name);
                if (child != null)
                    return child;
            }
        }
        return null;
    }

    //------------------------------------------------------------------------------
    // change a hexadecimal number to Color
    public static Color GetRgbColor( uint color ) 
    {
        float r,g,b,a;
        var inv = 1.0f / 255.0f;
        if( color > 0xffffff )
        {
            r = ( ( color >> 24 ) & 0xff ) * inv;
            g = ( ( color >> 16 ) & 0xff ) * inv;
            b = ( ( color >> 8  ) & 0xff ) * inv;
            a = ( ( color       ) & 0xff ) * inv;
        }
        else
        {
            r = ( ( color >> 16 ) & 0xff ) * inv;
            g = ( ( color >> 8  ) & 0xff ) * inv;
            b = ( ( color       ) & 0xff ) * inv;
            a = 1.0f;
        }
        return new Color( r, g, b, a );
    }
}

}
}
