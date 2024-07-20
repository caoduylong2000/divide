using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinSelectGroup : MonoBehaviour {
    [SerializeField]
    protected SkinSelectButton[] buttons;

	public void SetSkinSelectFrame( int index ) {
        for( int i = 0; i < buttons.Length; ++i )
        {
            var button = buttons[i];
            button.ShowSelectFrame( i == index );
        }
    }
}
