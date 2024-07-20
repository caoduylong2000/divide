namespace WK { namespace Collections {

public class Tuple<T1, T2>
{
	public T1 Item1 { get; private set; }
	public T2 Item2 { get; private set; }
	public Tuple( T1 i1, T2 i2 )
	{
		Item1 = i1;
		Item2 = i2;
	}
}

public class Tuple<T1, T2, T3>
{
	public T1 Item1 { get; private set; }
	public T2 Item2 { get; private set; }
	public T3 Item3 { get; private set; }
	public Tuple( T1 i1, T2 i2, T3 i3 )
	{
		Item1 = i1;
		Item2 = i2;
		Item3 = i3;
	}
}

} //end of namespace WK
} //end of namespace Generic
