using System;
using WK;

public class TagProcessor : ITagProcessor {
    public string Hour         { get; set; }
    public string Num          { get; set; }

    public string Process( string text )
    {
        if( text.Contains("#{hour}") )
        {
            text = text.Replace( "#{hour}", Hour );
        }
        if( text.Contains("#{num}") )
        {
            text = text.Replace( "#{num}", Num );
        }
        return text;
    }
}
