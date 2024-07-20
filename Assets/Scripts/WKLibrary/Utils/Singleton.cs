using System.Diagnostics;

namespace WK { namespace Utils {

//
//
//
public class Singleton<T> where T : class, new()
{
    protected Singleton()
    {
        Debug.Assert( null == _instance );
    }

    // static initializerで作ることで、
    // Instanceの生成がスレッドセーフになるというメリットがある。
    // また、static initializerとは関係ないが、
    // c#にはフレンドクラスが無いので、Tのコンストラクタは
    // publicでないといけない。そのため、シングルトン以外の場所から
    // コンストラクタを呼ばれてしまうのを防ぐことはできないが、
    // ランタイムでAssertには引っかかるし、
    // そもそも、今までの経験上そのような間違いに合ったことはない。
	private static readonly T _instance = new T();

	public static T Instance {
		get {
			return _instance;
		}
	}
}

}
}
