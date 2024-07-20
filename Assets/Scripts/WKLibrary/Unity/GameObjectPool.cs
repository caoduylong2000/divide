//------------------------------------------------------------------------
//
// (C) Copyright 2019 Nukenin LLC.
//
//------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace WK {
namespace Unity {

public class GameObjectPool : MonoBehaviour {
    [AssetsOnly]
    [SerializeField]
    protected GameObject obj;

    [SerializeField]
    protected int poolSize = 10;

    protected List<GameObject> pool = new List<GameObject>();

    void Awake() {
        CreatePool();
    }

    void CreatePool() {
        //obj.SetActive( false );
        //pool.Add( obj );
        for( int i = 0; i < poolSize; ++i )
        {
            var o = Instantiate( obj, transform );
            o.name += i;
            o.SetActive( false );
            pool.Add( o );
        }
    }

    public GameObject Birth() {
        for( int i = 0; i < poolSize; ++i )
        {
            var p = pool[i];
            if( !p.activeSelf )
            {
                //Debug.Log( "Birth : " + i );
                p.SetActive( true );
                return p;
            }
        }
        return null;
    }

    public void Kill( GameObject o ) {
#if UNITY_EDITOR
        bool is_valid = false;
        for( int i = 0; i < poolSize; ++i )
        {
            var p = pool[i];
            if( p == o )
            {
                is_valid = true;
            }
        }
        Debug.Assert( is_valid, "o is not included!" );
#endif
        o.transform.SetParent(transform);
        o.SetActive( false );
    }

    public void KillAll() {
        for( int i = 0; i < poolSize; ++i )
        {
            var p = pool[i];
            p.transform.SetParent(transform);
            p.SetActive( false );
        }
    }
}

}
}
