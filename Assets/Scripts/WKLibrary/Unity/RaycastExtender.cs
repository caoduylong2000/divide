using UnityEngine;
using UnityEngine.UI;

//Graphicの当たり判定だけを使って
//当たり判定を拡張する
namespace WK { namespace Unity { namespace UI {

public class RaycastExtender : UnityEngine.UI.Graphic{
#if UNITY_EDITOR
    public bool isDebugView = false;
    protected override void Start()
    {
        color = new Color( 1.0f, 0, 0, 0.2f );
    }
#endif

    protected override void OnPopulateMesh(VertexHelper v)
    {
        base.OnPopulateMesh(v);
#if UNITY_EDITOR
        if( isDebugView ) { return; }
#endif
        v.Clear();
    }
} 

}}}
