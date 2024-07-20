using UnityEngine;
using System.Collections;
using WK.Unity;

public class ComboEffectManager : SingletonMonoBehaviour<ComboEffectManager> {

    [SerializeField]
    private GameObject particlePrehab;

    //private Vector3[,] comboEffectPos;
    private int[] comboEffectFontSize;
    private Color[] comboEffectFontColor;
    /* private Vector3[] comboEffectScale; */

	public void Birth ( Vector3 pos, int number ) {
        GameObject obj = Instantiate( particlePrehab );
        obj.transform.SetParent( transform );
        /* obj.transform.localScale = GetComboEffectScale( number ); */
        obj.GetComponent< ComboParticleController >().Init( pos, number );
	}

	public Vector3 GetComboEffectPos ( int i_x, int i_y, int grid_size ) {
        float x = -300.0f + ( 300.0f / ( grid_size / 3.0f ) ) * i_y;
        float y = 750.0f - ( 400.0f / ( grid_size / 3.0f ) ) * i_x;
        return new Vector3( x, y, 0.0f );

        //return comboEffectPos[x,y];
    }

	/* public Vector3 GetComboEffectScale ( int num ) { */
        /* Debug.Assert( num > 0 ); */
        /* return comboEffectScale[ num - 1 ]; */
    /* } */

	public int GetComboEffectFontSize ( int num ) {
        return comboEffectFontSize[ num - 1 ];
    }

	public Color GetComboEffectFontColor ( int num ) {
        return comboEffectFontColor[ num - 1 ];
    }

    protected override void Awake()
    {
        base.Awake();

		//comboEffectPos       = new Vector3[ MainFieldController.MaxSlotNumX, MainFieldController.MaxSlotNumY ];
		comboEffectFontSize  = new int[     MainFieldController.MaxSlotNumX * MainFieldController.MaxSlotNumY ];
		comboEffectFontColor = new Color[   MainFieldController.MaxSlotNumX * MainFieldController.MaxSlotNumY ];

        //comboEffectPos[0,0] = new Vector3( -300.0f, 750.0f );
        //comboEffectPos[0,1] = new Vector3( 0.0f,    750.0f );
        //comboEffectPos[0,2] = new Vector3( 300.0f,  750.0f );
        //comboEffectPos[1,0] = new Vector3( -300.0f, 350.0f );
        //comboEffectPos[1,1] = new Vector3( 0.0f,    350.0f );
        //comboEffectPos[1,2] = new Vector3( 300.0f,  350.0f );
        //comboEffectPos[2,0] = new Vector3( -300.0f, -50.0f );
        //comboEffectPos[2,1] = new Vector3( 0.0f,    -50.0f );
        //comboEffectPos[2,2] = new Vector3( 300.0f,  -50.0f );

        //float x = -300.0f + ( 300.0f / ( grid_size / 3.0f ) ) * x;
        //float y = 750.0f - ( 400.0f / ( grid_size / 3.0f ) ) * y;

		/* comboEffectScale = new Vector3[MainFieldController.xSlotNum * MainFieldController.ySlotNum]; */
        /* comboEffectScale[0] = new Vector3( 1.0f, 1.0f, 1.0f ); */
        /* comboEffectScale[1] = new Vector3( 1.1f, 1.1f, 1.1f ); */
        /* comboEffectScale[2] = new Vector3( 1.2f, 1.2f, 1.2f ); */
        /* comboEffectScale[3] = new Vector3( 1.5f, 1.5f, 1.5f ); */
        /* comboEffectScale[4] = new Vector3( 2.0f, 2.0f, 2.0f ); */
        /* comboEffectScale[5] = new Vector3( 3.0f, 3.0f, 3.0f ); */
        /* comboEffectScale[6] = new Vector3( 3.0f, 3.0f, 3.0f ); */
        /* comboEffectScale[7] = new Vector3( 3.0f, 3.0f, 3.0f ); */
        /* comboEffectScale[8] = new Vector3( 3.0f, 3.0f, 3.0f ); */

        comboEffectFontSize[0] = 100;
        comboEffectFontSize[1] = 120;
        comboEffectFontSize[2] = 140;
        comboEffectFontSize[3] = 160;
        comboEffectFontSize[4] = 180;
        comboEffectFontSize[5] = 200;
        comboEffectFontSize[6] = 200;
        comboEffectFontSize[7] = 200;
        comboEffectFontSize[8] = 200;
        comboEffectFontSize[9] = 200;
        comboEffectFontSize[10] = 200;
        comboEffectFontSize[11] = 200;
        comboEffectFontSize[12] = 200;
        comboEffectFontSize[13] = 200;
        comboEffectFontSize[14] = 200;
        comboEffectFontSize[15] = 200;

        comboEffectFontColor[0] = WK.Utils.Utils.GetRgbColor( 0xE6E573FF );
        comboEffectFontColor[1] = WK.Utils.Utils.GetRgbColor( 0xFF8B32FF );
        comboEffectFontColor[2] = WK.Utils.Utils.GetRgbColor( 0xFF33F9FF );
        comboEffectFontColor[3] = WK.Utils.Utils.GetRgbColor( 0xB033FFFF );
        comboEffectFontColor[4] = WK.Utils.Utils.GetRgbColor( 0x3356FFFF );
        comboEffectFontColor[5] = WK.Utils.Utils.GetRgbColor( 0x33B3FFFF );
        comboEffectFontColor[6] = WK.Utils.Utils.GetRgbColor( 0x33FFF6FF );
        comboEffectFontColor[7] = WK.Utils.Utils.GetRgbColor( 0xE2FF33FF );
        comboEffectFontColor[8] = WK.Utils.Utils.GetRgbColor( 0xFF3394FF );
        comboEffectFontColor[9] = WK.Utils.Utils.GetRgbColor( 0xFF3394FF );
        comboEffectFontColor[10] = WK.Utils.Utils.GetRgbColor( 0xFF3394FF );
        comboEffectFontColor[11] = WK.Utils.Utils.GetRgbColor( 0xFF3394FF );
        comboEffectFontColor[12] = WK.Utils.Utils.GetRgbColor( 0xFF3394FF );
        comboEffectFontColor[13] = WK.Utils.Utils.GetRgbColor( 0xFF3394FF );
        comboEffectFontColor[14] = WK.Utils.Utils.GetRgbColor( 0xFF3394FF );
        comboEffectFontColor[15] = WK.Utils.Utils.GetRgbColor( 0xFF3394FF );
    }
}
