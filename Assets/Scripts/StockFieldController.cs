//#define FOR_PROMOTION
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;
using WK.Collections;
using WK.Unity;
using WK.Audio;
using Sirenix.OdinInspector;

public class StockFieldController : SingletonMonoBehaviour<StockFieldController> {
    struct DestPosData{
        public Vector3 pos;
        public Vector3 scale;
        public float alpha;
    }

    [SerializeField]
    private Text levelText = null;

    //[SerializeField]
    //private GameObject levelUpParticle = null;

    //[SerializeField]
    //private GameObject levelUpEffect;

    //[SerializeField]
    //private Text levelUpEffectText;

    [SerializeField]
    private LevelUpEffectController levelUpEffectCtrl;

    private float moveNextSpeed = 8.0f;

    public bool isDebugMode = false;

    [AssetsOnly]
    [SerializeField]
	protected GameObject blockPrehab;

    [ReadOnly]
    [SerializeField]
	private SlotController slot;

    public const int cNumPrimeNumbers = 6;
    private const int numPreNumbers = 14;
    private const int cMaxTutorialBirthCount = 4;
    private int[] preNumbers;
	//private int[] primeNumberList;
    //private float[] primeNumberRatioList;
    /* private int birthPrimeNumberRate = 6; */
	//private int[] numberList;
	private DestPosData[] destPosDatas;
	private GameObject[] nextBlockObjs;

    private int currState = 0;
    private int nextState = 0;
    private const int cStateIdle         = 0;
    private const int cStateChangeToNext = 1;

    private int birthCounter = 0;
    [SerializeField]
    private int currLevel = 0;
    //private int[] levelUpExp;
    //private int[] levelUpExpTimeLimit;
    //private int[] levelMaxNumber;
    //private int[] levelPrimeNumberRate;
    //private int[] levelOverThreeDigitRate;
    //private int minNumber = 6;
    private bool isReserveLevelUpEffect = false;
    private bool isInterstialClosedCallback = false;
    /* private int? birthRandomSeed = null; */
    /* public int BirthRandomSeed { get{ return birthRandomSeed.Value; } set{ birthRandomSeed = value; } } */

    public int BirthCounter { get { return birthCounter; }
        set {
            birthCounter = value;
            updateCurrLevel();
        }
    }

    //private int cMaxLevel = 17;
    //private int cOverThreeDigitLevel = 8;
    //private int cOverThreeDigitStartIndex = 60;
    private GameModeInfo gameModeInfo;

    private int previousBirthNumber;

    private MainFieldController mainFieldController = null;
    private KeepFieldController keepFieldController = null;

    private CanvasGroup canvasGroup;

    //------------------------------------------------------------------------------
    private int getLevelUpExp( int level )
    {
        //int level_up_exp = levelUpExp[ level ];
        int level_up_exp = gameModeInfo.levelUpExp[ level ];
        //if( GameSceneManager.Instance.IsTimeLimit )
        //{
        //    level_up_exp = levelUpExpTimeLimit[ level ];
        //}
        return level_up_exp;
    }

    //------------------------------------------------------------------------------
    //@memo
    //preNumbersを考慮して、- numPreNumbersする
    private void updateCurrLevel()
    {
        currLevel = 0;
        //for( int i = 0; i < ( cMaxLevel - 1 ); ++i )
        for( int i = 0; i < ( gameModeInfo.maxLevel - 1 ); ++i )
        {
            if( ( birthCounter - numPreNumbers ) > getLevelUpExp(i) )
            {
                currLevel++;
            }
        }
        levelText.text = ( currLevel + 1 ).ToString();
    }

    //------------------------------------------------------------------------------
    //@memo
    //preNumbers分、早めにレベルが上がる
    private int levelForBirth()
    {
        int level = 0;
        //for( int i = 0; i < ( cMaxLevel - 1 ); ++i )
        for( int i = 0; i < ( gameModeInfo.maxLevel - 1 ); ++i )
        {
            if( birthCounter > getLevelUpExp(i) )
            {
                level++;
            }
        }
        return level;
    }

    //------------------------------------------------------------------------------
    public bool IsStable()
    {
        return ( currState == cStateIdle ) && ( nextState == cStateIdle ) && ( slot.block != null );
    }

	//------------------------------------------------------------------------------
	public void ResetParams () {
        BlockController block = null;
        if( slot.block != null )
        {
            block = slot.block.GetComponent< BlockController >();
        }
        //実際にdestroyされるのはちょっと後なので、ここでnullに設定しておかないと、
        //ResetParams直後にFillStocks()できない。
        if( block )
        {
            block.transform.SetParent( null );
            Destroy( block.gameObject );
        }

        currState = cStateIdle;
        nextState = cStateChangeToNext;
        currLevel = 0;
        birthCounter = 0;
        isReserveLevelUpEffect = false;
        isInterstialClosedCallback = false;
        invalidateGameMode();
#if UNITY_EDITOR
        if( GameSceneManager.Instance.isDebugBirthCounter )
        {
            birthCounter = GameSceneManager.Instance.debugBirthCounter;
        }
        updateCurrLevel();
#endif
        previousBirthNumber = 0;
        SetDraggable( true );

        for( int i = 0; i < numPreNumbers; ++i )
        {
            preNumbers[ i ] = birthNumber( levelForBirth() );
        }

        levelText.text = ( currLevel + 1 ).ToString();
	}

    //------------------------------------------------------------------------------
	protected override void Awake () {
        base.Awake();

        /* levelUpExp = new int[ cMaxLevel - 1 ]; */
        /* levelUpExpTimeLimit = new int[ cMaxLevel - 1 ]; */
        /* levelMaxNumber = new int[ cMaxLevel ]; */
        /* levelPrimeNumberRate = new int[ cMaxLevel ]; */
        /* levelOverThreeDigitRate = new int[ cMaxLevel ]; */

        /* levelUpExp[ 0  ] = 12; */
        /* levelUpExp[ 1  ] = 30; */
        /* levelUpExp[ 2  ] = 60; */
        /* levelUpExp[ 3  ] = 100; */
        /* levelUpExp[ 4  ] = 160; */
        /* levelUpExp[ 5  ] = 240; */
        /* levelUpExp[ 6  ] = 320; */
        /* levelUpExp[ 7  ] = 480; */
        /* levelUpExp[ 8  ] = 640; */
        /* levelUpExp[ 9  ] = 800; */
        /* levelUpExp[ 10 ] = 960; */
        /* levelUpExp[ 11 ] = 1120; */
        /* levelUpExp[ 12 ] = 1280; */
        /* levelUpExp[ 13 ] = 1440; */
        /* levelUpExp[ 14 ] = 1600; */
        /* levelUpExp[ 15 ] = 1760; */

        /* levelUpExpTimeLimit[ 0  ] = 10; */
        /* levelUpExpTimeLimit[ 1  ] = 20; */
        /* levelUpExpTimeLimit[ 2  ] = 30; */
        /* levelUpExpTimeLimit[ 3  ] = 40; */
        /* levelUpExpTimeLimit[ 4  ] = 50; */
        /* levelUpExpTimeLimit[ 5  ] = 60; */
        /* levelUpExpTimeLimit[ 6  ] = 70; */
        /* levelUpExpTimeLimit[ 7  ] = 80; */
        /* levelUpExpTimeLimit[ 8  ] = 90; */
        /* levelUpExpTimeLimit[ 9  ] = 140; */
        /* levelUpExpTimeLimit[ 10 ] = 200; */
        /* levelUpExpTimeLimit[ 11 ] = 300; */
        /* levelUpExpTimeLimit[ 12 ] = 400; */
        /* levelUpExpTimeLimit[ 13 ] = 500; */
        /* levelUpExpTimeLimit[ 14 ] = 600; */
        /* levelUpExpTimeLimit[ 15 ] = 700; */

/* /1* #if UNITY_EDITOR *1/ */
/* /1*         levelUpExp[ 0  ] = 2; *1/ */
/* /1*         levelUpExp[ 1  ] = 4; *1/ */
/* /1*         levelUpExp[ 2  ] = 6; *1/ */
/* /1*         levelUpExp[ 3  ] = 10; *1/ */
/* /1* #endif *1/ */

        /* levelMaxNumber[ 0  ] = 19; */
        /* levelMaxNumber[ 1  ] = 34; */
        /* levelMaxNumber[ 2  ] = 59; */
        /* levelMaxNumber[ 3  ] = 59; */
        /* levelMaxNumber[ 4  ] = 59; */
        /* levelMaxNumber[ 5  ] = 59; */
        /* levelMaxNumber[ 6  ] = 59; */
        /* levelMaxNumber[ 7  ] = 59; */
        /* levelMaxNumber[ 8  ] = 96; */
        /* levelMaxNumber[ 9  ] = 104; */
        /* levelMaxNumber[ 10 ] = 112; */
        /* levelMaxNumber[ 11 ] = 120; */
        /* levelMaxNumber[ 12 ] = 128; */
        /* levelMaxNumber[ 13 ] = 150; */
        /* levelMaxNumber[ 14 ] = 170; */
        /* levelMaxNumber[ 15 ] = 200; */
        /* levelMaxNumber[ 16 ] = 240; */

        /* /1* levelMaxNumber[ 2 ] = 59; *1/ */
        /* /1* levelMaxNumber[ 3 ] = 71; *1/ */
        /* /1* levelMaxNumber[ 4 ] = 96; *1/ */
        /* /1* levelMaxNumber[ 5 ] = 122; *1/ */
        /* /1* levelMaxNumber[ 6 ] = 240; *1/ */

        /* levelPrimeNumberRate[ 0  ] = 4; */
        /* levelPrimeNumberRate[ 1  ] = 4; */
        /* levelPrimeNumberRate[ 2  ] = 5; */
        /* levelPrimeNumberRate[ 3  ] = 6; */
        /* levelPrimeNumberRate[ 4  ] = 7; */
        /* levelPrimeNumberRate[ 5  ] = 8; */
        /* levelPrimeNumberRate[ 6  ] = 9; */
        /* levelPrimeNumberRate[ 7  ] = 10; */
        /* levelPrimeNumberRate[ 8  ] = 10; */
        /* levelPrimeNumberRate[ 9  ] = 10; */
        /* levelPrimeNumberRate[ 10 ] = 10; */
        /* levelPrimeNumberRate[ 11 ] = 10; */
        /* levelPrimeNumberRate[ 12 ] = 10; */
        /* levelPrimeNumberRate[ 13 ] = 10; */
        /* levelPrimeNumberRate[ 14 ] = 10; */
        /* levelPrimeNumberRate[ 15 ] = 10; */
        /* levelPrimeNumberRate[ 16 ] = 10; */

        /* levelOverThreeDigitRate[ 0  ] = 1; */
        /* levelOverThreeDigitRate[ 1  ] = 1; */
        /* levelOverThreeDigitRate[ 2  ] = 1; */
        /* levelOverThreeDigitRate[ 3  ] = 1; */
        /* levelOverThreeDigitRate[ 4  ] = 1; */
        /* levelOverThreeDigitRate[ 5  ] = 1; */
        /* levelOverThreeDigitRate[ 6  ] = 1; */
        /* levelOverThreeDigitRate[ 7  ] = 1; */
        /* levelOverThreeDigitRate[ 8  ] = 5; */
        /* levelOverThreeDigitRate[ 9  ] = 4; */
        /* levelOverThreeDigitRate[ 10 ] = 3; */
        /* levelOverThreeDigitRate[ 11 ] = 2; */
        /* levelOverThreeDigitRate[ 12 ] = 1; */
        /* levelOverThreeDigitRate[ 13 ] = 1; */
        /* levelOverThreeDigitRate[ 14 ] = 1; */
        /* levelOverThreeDigitRate[ 15 ] = 1; */
        /* levelOverThreeDigitRate[ 16 ] = 1; */

        /* primeNumberList = new int[ cNumPrimeNumbers ]; */
        /* primeNumberList[ 0 ] = 2; */
        /* primeNumberList[ 1 ] = 3; */
        /* primeNumberList[ 2 ] = 5; */
        /* primeNumberList[ 3 ] = 7; */
        /* primeNumberList[ 4 ] = 11; */
        /* primeNumberList[ 5 ] = 13; */

		/* /1* primeNumberRatioList = new float[ 6 ]; *1/ */
		/* /1* primeNumberRatioList[ 0 ] = 0.47691939727337956f; *1/ */
		/* /1* primeNumberRatioList[ 1 ] = 0.7151399186797417f; *1/ */
		/* /1* primeNumberRatioList[ 2 ] = 0.8344893566132504f; *1/ */
		/* /1* primeNumberRatioList[ 3 ] = 0.9134178426213825f; *1/ */
		/* /1* primeNumberRatioList[ 4 ] = 0.9607749342262617f; *1/ */
		/* /1* primeNumberRatioList[ 5 ] = 1.0f; *1/ */

		/* int numberNum = 345; */
		/* numberList = new int[ numberNum ]; */
		/* /1* numberList[ 0 ] = 2; *1/ */
		/* /1* numberList[ 1 ] = 3; *1/ */
		/* /1* numberList[ 2 ] = 4; *1/ */
		/* /1* numberList[ 3 ] = 5; *1/ */
		/* /1* numberList[ 4 ] = 6; *1/ */
		/* /1* numberList[ 5 ] = 7; *1/ */
		/* /1* numberList[ 6 ] = 8; *1/ */
		/* /1* numberList[ 7 ] = 9; *1/ */
		/* /1* numberList[ 8 ] = 10; *1/ */
		/* /1* numberList[ 9 ] = 11; *1/ */
		/* /1* numberList[ 10 ] = 12; *1/ */
		/* /1* numberList[ 11 ] = 13; *1/ */

        /* //使用しない。適当に入れておく。 */
		/* numberList[ 0 ] = -1; */
		/* numberList[ 1 ] = -1; */
		/* numberList[ 2 ] = -1; */
		/* numberList[ 3 ] = -1; */
		/* numberList[ 4 ] = -1; */
		/* numberList[ 5 ] = -1; */

		/* numberList[ 6 ] = 4; */
		/* numberList[ 7 ] = 6; */
		/* numberList[ 8 ] = 8; */
		/* numberList[ 9 ] = 9; */
		/* numberList[ 10 ] = 10; */
		/* numberList[ 11 ] = 12; */
		/* numberList[ 12 ] = 14; */
		/* numberList[ 13 ] = 15; */
		/* numberList[ 14 ] = 16; */
		/* numberList[ 15 ] = 18; */
		/* numberList[ 16 ] = 20; */
		/* numberList[ 17 ] = 21; */
		/* numberList[ 18 ] = 22; */
		/* numberList[ 19 ] = 24; */
		/* numberList[ 20 ] = 25; */
		/* numberList[ 21 ] = 26; */
		/* numberList[ 22 ] = 27; */
		/* numberList[ 23 ] = 28; */
		/* numberList[ 24 ] = 30; */
		/* numberList[ 25 ] = 32; */
		/* numberList[ 26 ] = 33; */
		/* numberList[ 27 ] = 35; */
		/* numberList[ 28 ] = 36; */
		/* numberList[ 29 ] = 39; */
		/* numberList[ 30 ] = 40; */
		/* numberList[ 31 ] = 42; */
		/* numberList[ 32 ] = 44; */
		/* numberList[ 33 ] = 45; */
		/* numberList[ 34 ] = 48; */
		/* numberList[ 35 ] = 49; */
		/* numberList[ 36 ] = 50; */
		/* numberList[ 37 ] = 52; */
		/* numberList[ 38 ] = 54; */
		/* numberList[ 39 ] = 55; */
		/* numberList[ 40 ] = 56; */
		/* numberList[ 41 ] = 60; */
		/* numberList[ 42 ] = 63; */
		/* numberList[ 43 ] = 64; */
		/* numberList[ 44 ] = 65; */
		/* numberList[ 45 ] = 66; */
		/* numberList[ 46 ] = 70; */
		/* numberList[ 47 ] = 72; */
		/* numberList[ 48 ] = 75; */
		/* numberList[ 49 ] = 77; */
		/* numberList[ 50 ] = 78; */
		/* numberList[ 51 ] = 80; */
		/* numberList[ 52 ] = 81; */
		/* numberList[ 53 ] = 84; */
		/* numberList[ 54 ] = 88; */
		/* numberList[ 55 ] = 90; */
		/* numberList[ 56 ] = 91; */
		/* numberList[ 57 ] = 96; */
		/* numberList[ 58 ] = 98; */
		/* numberList[ 59 ] = 99; */
		/* numberList[ 60 ] = 100; */
		/* numberList[ 61 ] = 104; */
		/* numberList[ 62 ] = 105; */
		/* numberList[ 63 ] = 108; */
		/* numberList[ 64 ] = 110; */
		/* numberList[ 65 ] = 112; */
		/* numberList[ 66 ] = 117; */
		/* numberList[ 67 ] = 120; */
		/* numberList[ 68 ] = 121; */
		/* numberList[ 69 ] = 125; */
		/* numberList[ 70 ] = 126; */
		/* numberList[ 71 ] = 128; */
		/* numberList[ 72 ] = 130; */
		/* numberList[ 73 ] = 132; */
		/* numberList[ 74 ] = 135; */
		/* numberList[ 75 ] = 140; */
		/* numberList[ 76 ] = 143; */
		/* numberList[ 77 ] = 144; */
		/* numberList[ 78 ] = 147; */
		/* numberList[ 79 ] = 150; */
		/* numberList[ 80 ] = 154; */
		/* numberList[ 81 ] = 156; */
		/* numberList[ 82 ] = 160; */
		/* numberList[ 83 ] = 162; */
		/* numberList[ 84 ] = 165; */
		/* numberList[ 85 ] = 168; */
		/* numberList[ 86 ] = 169; */
		/* numberList[ 87 ] = 175; */
		/* numberList[ 88 ] = 176; */
		/* numberList[ 89 ] = 180; */
		/* numberList[ 90 ] = 182; */
		/* numberList[ 91 ] = 189; */
		/* numberList[ 92 ] = 192; */
		/* numberList[ 93 ] = 195; */
		/* numberList[ 94 ] = 196; */
		/* numberList[ 95 ] = 198; */
		/* numberList[ 96 ] = 200; */
		/* numberList[ 97 ] = 208; */
		/* numberList[ 98 ] = 210; */
		/* numberList[ 99 ] = 216; */
		/* numberList[ 100 ] = 220; */
		/* numberList[ 101 ] = 224; */
		/* numberList[ 102 ] = 225; */
		/* numberList[ 103 ] = 231; */
		/* numberList[ 104 ] = 234; */
		/* numberList[ 105 ] = 240; */
		/* numberList[ 106 ] = 242; */
		/* numberList[ 107 ] = 243; */
		/* numberList[ 108 ] = 245; */
		/* numberList[ 109 ] = 250; */
		/* numberList[ 110 ] = 252; */
		/* numberList[ 111 ] = 256; */
		/* numberList[ 112 ] = 260; */
		/* numberList[ 113 ] = 264; */
		/* numberList[ 114 ] = 270; */
		/* numberList[ 115 ] = 273; */
		/* numberList[ 116 ] = 275; */
		/* numberList[ 117 ] = 280; */
		/* numberList[ 118 ] = 286; */
		/* numberList[ 119 ] = 288; */
		/* numberList[ 120 ] = 294; */
		/* numberList[ 121 ] = 297; */
		/* numberList[ 122 ] = 300; */
		/* numberList[ 123 ] = 308; */
		/* numberList[ 124 ] = 312; */
		/* numberList[ 125 ] = 315; */
		/* numberList[ 126 ] = 320; */
		/* numberList[ 127 ] = 324; */
		/* numberList[ 128 ] = 325; */
		/* numberList[ 129 ] = 330; */
		/* numberList[ 130 ] = 336; */
		/* numberList[ 131 ] = 338; */
		/* numberList[ 132 ] = 343; */
		/* numberList[ 133 ] = 350; */
		/* numberList[ 134 ] = 351; */
		/* numberList[ 135 ] = 352; */
		/* numberList[ 136 ] = 360; */
		/* numberList[ 137 ] = 363; */
		/* numberList[ 138 ] = 364; */
		/* numberList[ 139 ] = 375; */
		/* numberList[ 140 ] = 378; */
		/* numberList[ 141 ] = 384; */
		/* numberList[ 142 ] = 385; */
		/* numberList[ 143 ] = 390; */
		/* numberList[ 144 ] = 392; */
		/* numberList[ 145 ] = 396; */
		/* numberList[ 146 ] = 400; */
		/* numberList[ 147 ] = 405; */
		/* numberList[ 148 ] = 416; */
		/* numberList[ 149 ] = 420; */
		/* numberList[ 150 ] = 429; */
		/* numberList[ 151 ] = 432; */
		/* numberList[ 152 ] = 440; */
		/* numberList[ 153 ] = 441; */
		/* numberList[ 154 ] = 448; */
		/* numberList[ 155 ] = 450; */
		/* numberList[ 156 ] = 455; */
		/* numberList[ 157 ] = 462; */
		/* numberList[ 158 ] = 468; */
		/* numberList[ 159 ] = 480; */
		/* numberList[ 160 ] = 484; */
		/* numberList[ 161 ] = 486; */
		/* numberList[ 162 ] = 490; */
		/* numberList[ 163 ] = 495; */
		/* numberList[ 164 ] = 500; */
		/* numberList[ 165 ] = 504; */
		/* numberList[ 166 ] = 507; */
		/* numberList[ 167 ] = 512; */
		/* numberList[ 168 ] = 520; */
		/* numberList[ 169 ] = 525; */
		/* numberList[ 170 ] = 528; */
		/* numberList[ 171 ] = 539; */
		/* numberList[ 172 ] = 540; */
		/* numberList[ 173 ] = 546; */
		/* numberList[ 174 ] = 550; */
		/* numberList[ 175 ] = 560; */
		/* numberList[ 176 ] = 567; */
		/* numberList[ 177 ] = 572; */
		/* numberList[ 178 ] = 576; */
		/* numberList[ 179 ] = 585; */
		/* numberList[ 180 ] = 588; */
		/* numberList[ 181 ] = 594; */
		/* numberList[ 182 ] = 600; */
		/* numberList[ 183 ] = 605; */
		/* numberList[ 184 ] = 616; */
		/* numberList[ 185 ] = 624; */
		/* numberList[ 186 ] = 625; */
		/* numberList[ 187 ] = 630; */
		/* numberList[ 188 ] = 637; */
		/* numberList[ 189 ] = 640; */
		/* numberList[ 190 ] = 648; */
		/* numberList[ 191 ] = 650; */
		/* numberList[ 192 ] = 660; */
		/* numberList[ 193 ] = 672; */
		/* numberList[ 194 ] = 675; */
		/* numberList[ 195 ] = 676; */
		/* numberList[ 196 ] = 686; */
		/* numberList[ 197 ] = 693; */
		/* numberList[ 198 ] = 700; */
		/* numberList[ 199 ] = 702; */
		/* numberList[ 200 ] = 704; */
		/* numberList[ 201 ] = 715; */
		/* numberList[ 202 ] = 720; */
		/* numberList[ 203 ] = 726; */
		/* numberList[ 204 ] = 728; */
		/* numberList[ 205 ] = 729; */
		/* numberList[ 206 ] = 735; */
		/* numberList[ 207 ] = 750; */
		/* numberList[ 208 ] = 756; */
		/* numberList[ 209 ] = 768; */
		/* numberList[ 210 ] = 770; */
		/* numberList[ 211 ] = 780; */
		/* numberList[ 212 ] = 784; */
		/* numberList[ 213 ] = 792; */
		/* numberList[ 214 ] = 800; */
		/* numberList[ 215 ] = 810; */
		/* numberList[ 216 ] = 819; */
		/* numberList[ 217 ] = 825; */
		/* numberList[ 218 ] = 832; */
		/* numberList[ 219 ] = 840; */
		/* numberList[ 220 ] = 845; */
		/* numberList[ 221 ] = 847; */
		/* numberList[ 222 ] = 858; */
		/* numberList[ 223 ] = 864; */
		/* numberList[ 224 ] = 875; */
		/* numberList[ 225 ] = 880; */
		/* numberList[ 226 ] = 882; */
		/* numberList[ 227 ] = 891; */
		/* numberList[ 228 ] = 896; */
		/* numberList[ 229 ] = 900; */
		/* numberList[ 230 ] = 910; */
		/* numberList[ 231 ] = 924; */
		/* numberList[ 232 ] = 936; */
		/* numberList[ 233 ] = 945; */
		/* numberList[ 234 ] = 960; */
		/* numberList[ 235 ] = 968; */
		/* numberList[ 236 ] = 972; */
		/* numberList[ 237 ] = 975; */
		/* numberList[ 238 ] = 980; */
		/* numberList[ 239 ] = 990; */
		/* numberList[ 240 ] = 1000; */
		/* numberList[ 241 ] = 1001; */
		/* numberList[ 242 ] = 1008; */
		/* numberList[ 243 ] = 1014; */
		/* numberList[ 244 ] = 1024; */
		/* numberList[ 245 ] = 1029; */
		/* numberList[ 246 ] = 1040; */
		/* numberList[ 247 ] = 1050; */
		/* numberList[ 248 ] = 1053; */
		/* numberList[ 249 ] = 1056; */
		/* numberList[ 250 ] = 1078; */
		/* numberList[ 251 ] = 1080; */
		/* numberList[ 252 ] = 1089; */
		/* numberList[ 253 ] = 1092; */
		/* numberList[ 254 ] = 1100; */
		/* numberList[ 255 ] = 1120; */
		/* numberList[ 256 ] = 1125; */
		/* numberList[ 257 ] = 1134; */
		/* numberList[ 258 ] = 1144; */
		/* numberList[ 259 ] = 1152; */
		/* numberList[ 260 ] = 1155; */
		/* numberList[ 261 ] = 1170; */
		/* numberList[ 262 ] = 1176; */
		/* numberList[ 263 ] = 1183; */
		/* numberList[ 264 ] = 1188; */
		/* numberList[ 265 ] = 1200; */
		/* numberList[ 266 ] = 1210; */
		/* numberList[ 267 ] = 1215; */
		/* numberList[ 268 ] = 1225; */
		/* numberList[ 269 ] = 1232; */
		/* numberList[ 270 ] = 1248; */
		/* numberList[ 271 ] = 1250; */
		/* numberList[ 272 ] = 1260; */
		/* numberList[ 273 ] = 1274; */
		/* numberList[ 274 ] = 1280; */
		/* numberList[ 275 ] = 1287; */
		/* numberList[ 276 ] = 1296; */
		/* numberList[ 277 ] = 1300; */
		/* numberList[ 278 ] = 1320; */
		/* numberList[ 279 ] = 1323; */
		/* numberList[ 280 ] = 1331; */
		/* numberList[ 281 ] = 1344; */
		/* numberList[ 282 ] = 1350; */
		/* numberList[ 283 ] = 1352; */
		/* numberList[ 284 ] = 1365; */
		/* numberList[ 285 ] = 1372; */
		/* numberList[ 286 ] = 1375; */
		/* numberList[ 287 ] = 1386; */
		/* numberList[ 288 ] = 1400; */
		/* numberList[ 289 ] = 1404; */
		/* numberList[ 290 ] = 1408; */
		/* numberList[ 291 ] = 1430; */
		/* numberList[ 292 ] = 1440; */
		/* numberList[ 293 ] = 1452; */
		/* numberList[ 294 ] = 1456; */
		/* numberList[ 295 ] = 1458; */
		/* numberList[ 296 ] = 1470; */
		/* numberList[ 297 ] = 1485; */
		/* numberList[ 298 ] = 1500; */
		/* numberList[ 299 ] = 1512; */
		/* numberList[ 300 ] = 1521; */
		/* numberList[ 301 ] = 1536; */
		/* numberList[ 302 ] = 1540; */
		/* numberList[ 303 ] = 1560; */
		/* numberList[ 304 ] = 1568; */
		/* numberList[ 305 ] = 1573; */
		/* numberList[ 306 ] = 1575; */
		/* numberList[ 307 ] = 1584; */
		/* numberList[ 308 ] = 1600; */
		/* numberList[ 309 ] = 1617; */
		/* numberList[ 310 ] = 1620; */
		/* numberList[ 311 ] = 1625; */
		/* numberList[ 312 ] = 1638; */
		/* numberList[ 313 ] = 1650; */
		/* numberList[ 314 ] = 1664; */
		/* numberList[ 315 ] = 1680; */
		/* numberList[ 316 ] = 1690; */
		/* numberList[ 317 ] = 1694; */
		/* numberList[ 318 ] = 1701; */
		/* numberList[ 319 ] = 1715; */
		/* numberList[ 320 ] = 1716; */
		/* numberList[ 321 ] = 1728; */
		/* numberList[ 322 ] = 1750; */
		/* numberList[ 323 ] = 1755; */
		/* numberList[ 324 ] = 1760; */
		/* numberList[ 325 ] = 1764; */
		/* numberList[ 326 ] = 1782; */
		/* numberList[ 327 ] = 1792; */
		/* numberList[ 328 ] = 1800; */
		/* numberList[ 329 ] = 1815; */
		/* numberList[ 330 ] = 1820; */
		/* numberList[ 331 ] = 1848; */
		/* numberList[ 332 ] = 1859; */
		/* numberList[ 333 ] = 1872; */
		/* numberList[ 334 ] = 1875; */
		/* numberList[ 335 ] = 1890; */
		/* numberList[ 336 ] = 1911; */
		/* numberList[ 337 ] = 1920; */
		/* numberList[ 338 ] = 1925; */
		/* numberList[ 339 ] = 1936; */
		/* numberList[ 340 ] = 1944; */
		/* numberList[ 341 ] = 1950; */
		/* numberList[ 342 ] = 1960; */
		/* numberList[ 343 ] = 1980; */
		/* numberList[ 344 ] = 2000; */

        preNumbers = new int[ numPreNumbers ];
        nextBlockObjs = new GameObject[ numPreNumbers ];
        destPosDatas = new DestPosData[ numPreNumbers + 1 ];
        destPosDatas[ 0 ].pos = new Vector3( 0.0f, 0.0f, 0.0f );
        destPosDatas[ 0 ].scale = new Vector3( 1.0f, 1.0f, 1.0f );
        destPosDatas[ 0 ].alpha = 1.0f;

        previousBirthNumber = 0;

        canvasGroup = GetComponent< CanvasGroup >();

#if UNITY_EDITOR
        if( GameSceneManager.Instance.isDebugBirthCounter )
        {
            birthCounter = GameSceneManager.Instance.debugBirthCounter;
        }
#endif

        //levelUpEffect.GetComponent<WK.CurveAnimator>().OnComplete.AddListener( () => {
        //            levelUpEffect.SetActive( false );
        //            //@memo This place is good for inserting interstitial Ads.
        //            if(!GameSceneManager.Instance.IsTimeLimit)
        //            {
        //                if(!GameSceneManager.Instance.IsPurchasedNoAds)
        //                {
        //                    WK.AdListenerMax.Instance.ShowInterstitial();
        //                    //for debug
        //                    //if(UnityEngine.Debug.isDebugBuild)
        //                    //{
        //                    //    AdListener.Instance.ShowInterstitial();
        //                    //}
        //                }
        //            }
        //        }
        //        );
        //levelUpEffect.SetActive( false );

        WK.AdListenerMax.Instance.userCallbackOnCloseInterstitial = () => {
            isInterstialClosedCallback = true;
        };
    }

	//------------------------------------------------------------------------------
	// Use this for initialization
    void Start()
    {
        levelText.text = ( currLevel + 1 ).ToString();
        for( int i = 0; i < numPreNumbers; ++i )
        {
            //preNumbers[ i ] = birthNumber(levelForBirth());
            nextBlockObjs[ i ] = transform.Find( "NextBlock" + i.ToString() ).gameObject;
            //nextBlockObjs[ i ].GetComponentInChildren<Text>().text = preNumbers[ i ].ToString();
            destPosDatas[ i + 1 ].pos = new Vector3( nextBlockObjs[ i ].transform.localPosition.x
                                                    , nextBlockObjs[ i ].transform.localPosition.y
                                                    , nextBlockObjs[ i ].transform.localPosition.z );
            destPosDatas[ i + 1 ].scale = new Vector3( nextBlockObjs[ i ].transform.localScale.x
                                                      , nextBlockObjs[ i ].transform.localScale.y
                                                      , nextBlockObjs[ i ].transform.localScale.z );
            destPosDatas[ i + 1 ].alpha = nextBlockObjs[ i ].GetComponent<CanvasGroup>().alpha;

            nextBlockObjs[ i ].transform.localPosition  = new Vector3(   nextBlockObjs[ i ].transform.localPosition.x
                                                   , nextBlockObjs[ i ].transform.localPosition.y
                                                   , nextBlockObjs[ i ].transform.localPosition.z );
            nextBlockObjs[ i ].transform.localScale = new Vector3( nextBlockObjs[ i ].transform.localScale.x
                                                   , nextBlockObjs[ i ].transform.localScale.y
                                                   , nextBlockObjs[ i ].transform.localScale.z );
        }

        slot = transform.Find( "SFSlot" ).gameObject.GetComponent< SlotController >();
        mainFieldController = GameObject.Find( "MainField" ).GetComponent< MainFieldController >();
        keepFieldController = GameObject.Find( "KeepField" ).GetComponent< KeepFieldController >();
        //gameModeInfo = Parameters.Instance.ModeInfo3;
        //fillStocksImidiately();
	}

    //------------------------------------------------------------------------------
    public void InvalidateNextBlockObjSize()
    {
        int grid_size = GameSceneManager.Instance.GameMode == EGameMode.four ? 4 : 3;
        var size = grid_size == 4 ? Parameters.BLOCK_SIZE_4 : Parameters.BLOCK_SIZE_3;
        for( int i = 0; i < numPreNumbers; ++i )
        {
            var rect = nextBlockObjs[ i ].GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2( size, size );
        }
    }

    //------------------------------------------------------------------------------
    private void invalidateGameMode()
    {
        gameModeInfo = Parameters.Instance.ModeInfo3;
        if( GameSceneManager.Instance.GameMode == EGameMode.four )
        {
            gameModeInfo = Parameters.Instance.ModeInfo4;
        }
        else if( GameSceneManager.Instance.IsTimeLimit )
        {
            gameModeInfo = Parameters.Instance.ModeInfo3Time;
        }
    }

    //------------------------------------------------------------------------------
    public void InvalidateGameMode()
    {
        invalidateGameMode();
    }

    //------------------------------------------------------------------------------
    public void FillStocksImidiately()
    {
        //levelText.text = ( currLevel + 1 ).ToString();
        //invalidateGameMode();
        //for( int i = 0; i < numPreNumbers; ++i )
        //{
        //    preNumbers[ i ] = birthNumber(levelForBirth());
        //}
        fillStocksImidiately();
    }

    //------------------------------------------------------------------------------
    public void FillStocks()
    {
        changeState( cStateChangeToNext );
    }

    //------------------------------------------------------------------------------
    private void fillStocksImidiately()
    {
        InvalidateNextBlockObjSize();
        for( int i = 0; i < numPreNumbers; ++i )
        {
            nextBlockObjs[ i ].transform.localPosition = destPosDatas[ i + 1 ].pos;
            nextBlockObjs[ i ].transform.localScale = destPosDatas[ i + 1 ].scale;
            nextBlockObjs[ i ].GetComponent<CanvasGroup>().alpha = destPosDatas[ i + 1 ].alpha;
        }

        //stock
        if( slot.block == null )
        {
            GameObject block = CreateBlock();
            SetItemOnSlot( block.transform );
        }

        var bctrl = slot.block.GetComponent< BlockController >();
        bctrl.GetComponent< BlockController >().SetNumberImidiately( preNumbers[0] );
        bctrl.UpdateSkin();

        for( int i = 0; i < numPreNumbers - 1; ++i )
        {
            preNumbers[ i ] = preNumbers[ i + 1 ];
            var next_ctrl = nextBlockObjs[ i ].GetComponent<NextObjController>();
            next_ctrl.Init( preNumbers[ i ] );
            //nextBlockObjs[ i ].GetComponentInChildren<Text>().text = preNumbers[ i ].ToString();
            //nextBlockObjs[ i ].GetComponentInChildren<Image>().color = ColorManager.Instance.GetBlockColor( preNumbers[ i ] );
        }

        {
            preNumbers[ numPreNumbers - 1 ] = birthNumber(levelForBirth());
            //nextBlockObjs[ numPreNumbers - 1 ].GetComponentInChildren<Text>().text = preNumbers[ numPreNumbers - 1 ].ToString();
            //nextBlockObjs[ numPreNumbers - 1 ].GetComponentInChildren<Image>().color = ColorManager.Instance.GetBlockColor( preNumbers[ numPreNumbers - 1 ] );
            var next_ctrl = nextBlockObjs[ numPreNumbers - 1 ].GetComponent<NextObjController>();
            next_ctrl.Init( preNumbers[ numPreNumbers - 1 ] );
        }

        changeState( cStateIdle );
    }

    //------------------------------------------------------------------------------
    //
    // @memo
    // birthPrimeNumberRate 回に一回素数を生む
    // その素数は、場に出ている数を割れるものの最大数
    //
	private int birthNumber( int level )
    {
        //int min_num = minNumber;
        int min_num = gameModeInfo.minNumber;
        //int max_num = levelMaxNumber[ level ];
        int max_num = gameModeInfo.levelMaxNumber[ level ];
        /* Debug.Log( "birthNumber random seed: " + UnityEngine.Random.seed ); */

        /* if( birthRandomSeed != null ) */
        /* { */
        /*     UnityEngine.Random.seed = birthRandomSeed.Value; */
        /* } */

        int result = 0;
        birthCounter++;
        //cOverThreeDigitLevelまでは普通に今までどおりbirth
        //if( level < cOverThreeDigitLevel )
        if( level < gameModeInfo.overThreeDigitLevel )
        {
            result = birthNumberImpl( min_num, max_num, level );
        }
        else
        {
            //rate回に一回3桁も含んでサイコロを振る
            //+1しているのはlevelPrimeNumberRateと被りにくくするため(完璧ではない)
            //if( ( birthCounter + 1 ) % levelOverThreeDigitRate[level] == 0 && ( !isDebugMode ) )
            //if( ( birthCounter + 1 ) % gameModeInfo.levelOverThreeDigitRate[level] == 0
            if( UnityEngine.Random.Range( 0.0f, 1.0f ) < gameModeInfo.levelOverThreeDigitRate[level]
                    && ( !isDebugMode ) )
            {
                result = birthNumberImpl( min_num, max_num, level );
            }
            else
            {
                result = birthNumberImpl( min_num, gameModeInfo.overThreeDigitStartIndex - 1, level );
            }
        }

        /* birthRandomSeed = UnityEngine.Random.seed; */

        return result;
    }

	private int birthNumberImpl( int min_num, int max_num, int level )
    {
        if( !GameSceneManager.Instance.isTutorialFinished &&
                birthCounter <= cMaxTutorialBirthCount )
        {
            return birthNormalNumber( min_num, max_num );
        }

        //if( birthCounter % levelPrimeNumberRate[level] == 0 && ( !isDebugMode ) )
        if( birthCounter % gameModeInfo.levelPrimeNumberRate[level] == 0 && ( !isDebugMode ) )
        {
            int prime = birthPrimeNumber();
            /* Debug.Log( "birthPrimeNumber " + prime.ToString() ); */
            return prime;
        }
        else
        {
            int birth_number = birthNormalNumber( min_num, max_num );
            int maxCounter = 20;//一応限界を設けておく
            while( birth_number == previousBirthNumber )//連続で同じ数字が出ないように
            {
                birth_number = birthNormalNumber( min_num, max_num );
                if( maxCounter-- < 0 ) break;
            }
            previousBirthNumber = birth_number;
            return birth_number;
        }
    }

    /* //------------------------------------------------------------------------------ */
    /* private int birthDividableNumber( int min_num, int max_num ) */
    /* { */
    /*     Assert.IsNotNull( mainFieldController ); */
    /*     //割れる数字が出るまで回す */
    /*     int maxCounter = 20;//一応限界を設けておく */
    /*     int number = birthNormalNumber( min_num, max_num ); */
    /*     while( mainFieldController.CheckDividableNumberConsideringPlace( number ) == false ) */
    /*     { */
    /*         number = birthNormalNumber( min_num, max_num ); */
    /*         if( maxCounter-- < 0 ) break; */
    /*     } */

    /*     /1* Debug.Log( "birth dividable number " + number.ToString() ); *1/ */
    /*     return number; */
    /* } */

    //------------------------------------------------------------------------------
    private bool checkIsThereDividableNumber()
    {
        Assert.IsNotNull( mainFieldController );

        int[] holding_numbers = new int[ numPreNumbers + 2 ];
        int num_holding_numbers = 0;
        /* GameObject block = slot.GetComponent< SlotController >().block; */
        /* //blockがあったら+1 */
        /* if( block ){ */
        /*     holding_numbers[ num_holding_numbers++ ] = block.GetComponent< BlockController >().number; */
        /* } */

        // keepにあったら+1
        if( keepFieldController ) {
            if( keepFieldController.slot.block ) {
                holding_numbers[ num_holding_numbers++ ] = keepFieldController.slot.block.GetComponent< BlockController >().number;
            }
        }

        /* Debug.Log( "check dividable -1" ); */
        for( int i = 0; i < numPreNumbers; ++i )
        {
            /* Debug.Log( "preNumbers " + preNumbers[ i ].ToString() ); */
            holding_numbers[ num_holding_numbers++ ] = preNumbers[ i ];
        }

        for( int i = 0; i < num_holding_numbers; ++i )
        {
            /* Debug.Log( "check dividable " + holding_numbers[i].ToString() ); */
            if( mainFieldController.CheckDividableNumberConsideringPlace( holding_numbers[ i ] ) ) return true;
        }

        //相互に割れるか判定
        for( int i = 0; i < num_holding_numbers; ++i )
        {
            for( int j = i + 1; j < num_holding_numbers; ++j )
            {
                /* Debug.Log( "check dividable " + holding_numbers[ i ].ToString() + ", " + holding_numbers[j].ToString() ); */
                if( holding_numbers[ i ] % holding_numbers[ j ] == 0 ) return true;
                if( holding_numbers[ j ] % holding_numbers[ i ] == 0 ) return true;
            }
        }

        return false;
    }

    //------------------------------------------------------------------------------
	private int birthPrimeNumber()
    {
        Assert.IsNotNull( mainFieldController );
        WK.Collections.Tuple< int[], int > candidates = mainFieldController.GetDividableNumbers( gameModeInfo.primeNumberList, gameModeInfo.primeNumberList.Length );

        if( candidates.Item2 == 0 )
        {
            return 2;
        }
        else
        {
            return candidates.Item1[ UnityEngine.Random.Range( 0, candidates.Item2 ) ];
            /* return candidates.Item1[ candidates.Item2 - 1 ]; */
        }
    }

    //------------------------------------------------------------------------------
	private int birthNormalNumber( int min_num, int max_num )
	{
        if( !GameSceneManager.Instance.isTutorialFinished )
        {
            switch( birthCounter )
            {
                case 1:
                    return 24;
                case 2:
                    return 4;
                case 3:
                    return 10;
                case 4:
                    return 3;
            }
        }

#if FOR_PROMOTION
        isDebugMode = UnityEngine.Debug.isDebugBuild;
        if( isDebugMode )
        {
            // for 3 x 3
            switch( birthCounter )
            {
                case 1:
                    return 2 * 5;
                case 2:
                    return 2;
                //case 3:
                //    return 5 * 2;
                //case 4:
                //    return 2 * 3;
                case 3:
                    return 7 * 3;
                case 4:
                    return 5 * 7;
                case 5:
                    return 13 * 5;
                case 6:
                    return 2 * 13;
                case 7:
                    return 7 * 2;
                case 8:
                    return 3 * 7;
                case 9:
                    return 5 * 3;
                case 10:
                    return 5 * 5;
            }

            //// for 4 x 4
            //switch( birthCounter )
            //{
            //    case 1:
            //        return 2 * 5;
            //    case 2:
            //        return 17 * 2;
            //    case 3:
            //        return 7 * 17;
            //    case 4:
            //        return 4 * 7;
            //    case 5:
            //        return 13 * 4;
            //    case 6:
            //        return 3 * 13;
            //    case 7:
            //        return 7 * 3;
            //    case 8:
            //        return 8 * 7;
            //    case 9:
            //        return 5 * 8;
            //    case 10:
            //        return 5 * 5;
            //    case 11:
            //        return 5 * 11;
            //    case 12:
            //        return 11 * 7;
            //    case 13:
            //        return 7 * 9;
            //    case 14:
            //        return 9 * 11;
            //    case 15:
            //        return 11 * 13;
            //    case 16:
            //        return 13;
            //}

            //switch( birthCounter )
            //{
            //    case 1:  return 2  * 2 * 2;
            //    case 2:  return 2  * 2;
            //    case 3:  return 2;
            //    case 4:  return 5;
            //    case 5:  return 7  * 3;
            //    case 6:  return 5  * 7;
            //    case 7:  return 13 * 5;
            //    case 8:  return 2  * 13;
            //    case 9:  return 7  * 2;
            //    case 10: return 3  * 7;
            //    case 11: return 5  * 3;
            //    case 12: return 5  * 5;
            //}
        }
#endif

#if UNITY_EDITOR
        if( isDebugMode )
        {
            switch( birthCounter )
            {
                case 1:
                    return 2 * 5;
                case 2:
                    return 2 * 2 * 3;
                case 3:
                    return 2 * 2 * 2 * 3;
                case 4:
                    return 11 * 7;
                case 5:
                    return 7 * 3;
                case 6:
                    return 7 * 5;
                case 7:
                    return 2;
                case 8:
                    return 7;
                case 9:
                    return 13;
                case 10:
                    return 13 * 5;
                case 11:
                    return 7 * 5;
                case 12:
                    return 7;
                case 13:
                    return 11;
                /* case 1: */
                /*     return 2 * 5 * 7; */
                /* case 2: */
                /*     return 2 * 2 * 3 * 5; */
                /* case 3: */
                /*     return 2 * 2 * 2 * 3; */
                /* case 4: */
                /*     return 2 * 3 * 5; */
                /* case 5: */
                /*     return 3 * 3 * 5; */
                /* case 6: */
                /*     return 7 * 3; */
                /* case 7: */
                /*     return 11 * 7; */
                /* case 8: */
                /*     return 11 * 13; */
                /* case 9: */
                /*     return 13; */
                /* case 5: */
                /*     return 8; */
                /* case 6: */
                /*     return 8; */
                /* case 7: */
                /*     return 11; */
                /* case 8: */
                /*     return 11; */
                /* case 9: */
                /*     return 10; */
                /* case 10: */
                /*     return 10; */
                /* case 11: */
                /*     return 9; */
                /* case 12: */
                /*     return 9; */
                /* case 13: */
                /*     return 8; */
                /* case 14: */
                /*     return 8; */

                /* case 1: */
                /*     return 2 * 3; */
                /* case 2: */
                /*     return 2 * 5; */
                /* case 3: */
                /*     return 5 * 5; */
                /* case 4: */
                /*     return 5; */

                /* case 0: */
                /*     return 7 * 2 * 3; */
                /* case 1: */
                /*     return 2 * 3 * 13; */
                /* case 2: */
                /*     return 13 * 5; */
                /* case 3: */
                /*     return 5 * 2 * 6; */
                /* case 4: */
                /*     return 2 * 6 * 7; */
                /* case 5: */
                /*     return 7 * 5; */
                /* case 6: */
                /*     return 5; */

                /* case 0: */
                /*     return 4; */
                /* case 1: */
                /*     return 6; */
                /* case 2: */
                /*     return 8; */
                /* case 3: */
                /*     return 48; */
                /* case 4: */
                /*     return 96; */

                /* case 0: */
                /*     return 4; */
                /* case 1: */
                /*     return 6; */
                /* case 2: */
                /*     return 33; */
                /* case 3: */
                /*     return 26; */
                /* case 4: */
                /*     return 77; */
                /* case 5: */
                /*     return 75; */
                /* case 6: */
                /*     return 39; */
                /* case 7: */
                /*     return 65; */
                /* case 8: */
                /*     return 50; */
                /* case 9: */
                /*     return 49; */
            }
        }
#endif

        /* Debug.Log( "level " + currLevel.ToString() + ", counter : " + birthCounter.ToString() ); */

        //if( currLevel < ( cMaxLevel - 1 ) )
        if( currLevel < ( gameModeInfo.maxLevel - 1 ) )
        {
            if( ( birthCounter - numPreNumbers ) > getLevelUpExp( currLevel ) )
            {
                currLevel++;

                //@memo On TimeLimitMode, the full screen effect is annoying
                if(!GameSceneManager.Instance.IsTimeLimit)
                {
                    isReserveLevelUpEffect = true;
                }
                else
                {
                    //levelText.text = ( currLevel + 1 ).ToString();
                    //GameObject obj = Instantiate( levelUpParticle );
                    //obj.transform.SetParent( levelText.gameObject.transform );
                    //obj.GetComponent<LevelUpParticleController >().Init( new Vector3( 100.0f, 0.0f, 0.0f ) );
                    //SoundManager.Instance.PlaySe( "levelup" );

                    levelUpEffectCtrl.StartSmallEffect( currLevel );
                }
            }
        }

		//return numberList[ UnityEngine.Random.Range( min_num, max_num ) ];
		return gameModeInfo.numberList[ UnityEngine.Random.Range( min_num, max_num ) ];
	}

	//------------------------------------------------------------------------------
	// Update is called once per frame
	void Update () {
        if( currState != nextState )
        {
            currState = nextState;
        }

        switch( currState )
        {
            case cStateIdle:
                //たまにブロックが途中で止まってしまうことがある対策
                {
                    var dragged = BlockController.blockBeginDragged;
                    if( dragged != null )
                    {
                        bool is_touching = Input.touchCount > 0;
#if UNITY_EDITOR
                        is_touching = Input.GetMouseButton(0);
#endif
                        if(!is_touching)
                        {
                            BlockController.draggingCheckCounter++;
                            const int DRAG_CHECK_THRESHOLD = 4;
                            //何らかの不具合でドラッグしてない臭い
                            if( DRAG_CHECK_THRESHOLD < BlockController.draggingCheckCounter )
                            {
                                dragged.GetComponent<BlockController>().BackToOrgPosition();
                            }
                        }
                    }
                }
                break;
            case cStateChangeToNext:
                {
                    if( slot.block == null )
                    {
                        bool is_finish_moving = false;
                        for( int i = 0; i < numPreNumbers; ++i )
                        {
                            Vector3 rel_pos = destPosDatas[i].pos - nextBlockObjs[ i ].transform.localPosition;
                            nextBlockObjs[ i ].transform.localPosition += rel_pos * Time.deltaTime * moveNextSpeed;

                            Vector3 rel_scale = destPosDatas[i].scale - nextBlockObjs[ i ].transform.localScale;
                            nextBlockObjs[ i ].transform.localScale += rel_scale * Time.deltaTime * moveNextSpeed;

                            float relAlpha = destPosDatas[i].alpha - nextBlockObjs[ i ].GetComponent<CanvasGroup>().alpha;
                            nextBlockObjs[ i ].GetComponent<CanvasGroup>().alpha += relAlpha * Time.deltaTime * moveNextSpeed;

                            if( Mathf.Abs( rel_pos.x ) < 5.0f )
                            {
                                is_finish_moving = true;
                            }

                            /*
                            if( Mathf.Abs( relAlpha ) < 0.04f )
                            {
                                is_finish_moving = true;
                            }
                            */
                        }

                        if( is_finish_moving )
                        {
                            for( int i = 0; i < numPreNumbers; ++i )
                            {
                                nextBlockObjs[ i ].transform.localPosition = destPosDatas[ i + 1 ].pos;
                                nextBlockObjs[ i ].transform.localScale = destPosDatas[ i + 1 ].scale;
                                nextBlockObjs[ i ].GetComponent<CanvasGroup>().alpha = destPosDatas[ i + 1 ].alpha;
                            }

                            int next_pre_number = birthNumber( levelForBirth() );
                            Assert.IsNotNull( mainFieldController );

                            /* //残り4スペースで、もし詰まりそうだったら */
                            /* //最後のnext numberを詰まらないように数字に置き換え */
                            /* if( mainFieldController.GetFreeSpaceCount() == */
                            /*         4 - ( keepFieldController.slot.block != null ? 0 : 1 ) ) */
                            /* { */
                            /*     //手持ちの数字では割り切れない */
                            /*     if( checkIsThereDividableNumber() == false ) */
                            /*     { */
                            /*         next_pre_number = birthDividableNumber( minNumber, levelMaxNumber[ currLevel ] ); */
                            /*     } */
                            /* } */

                            //stock
                            GameObject block = CreateBlock();
                            SetItemOnSlot( block.transform );
                            var bctrl = block.GetComponent< BlockController >();
                            bctrl.SetNumberImidiately( preNumbers[0] );
                            bctrl.UpdateSkin();

                            for( int i = 0; i < numPreNumbers - 1; ++i )
                            {
                                preNumbers[ i ] = preNumbers[ i + 1 ];
                                var next_ctrl = nextBlockObjs[ i ].GetComponent<NextObjController>();
                                next_ctrl.Init( preNumbers[ i ] );
                                //nextBlockObjs[ i ].GetComponentInChildren<Text>().text = preNumbers[ i ].ToString();
                                //nextBlockObjs[ i ].GetComponentInChildren<Image>().color = ColorManager.Instance.GetBlockColor( preNumbers[ i ] );
                            }

                            {
                                //nextBlockObjs[ numPreNumbers - 1 ].GetComponentInChildren<Text>().text = preNumbers[ numPreNumbers - 1 ].ToString();
                                //nextBlockObjs[ numPreNumbers - 1 ].GetComponentInChildren<Image>().color = ColorManager.Instance.GetBlockColor( preNumbers[ numPreNumbers - 1 ] );
                                preNumbers[ numPreNumbers - 1 ] = next_pre_number;
                                var next_ctrl = nextBlockObjs[ numPreNumbers - 1 ].GetComponent<NextObjController>();
                                next_ctrl.Init( preNumbers[ numPreNumbers - 1 ] );
                            }
                        }
                    }
                    else
                    {
                        changeState( cStateIdle );
                    }
                }
                break;
        }

        if( isReserveLevelUpEffect )
        {
            if( mainFieldController.checkFinished() )
            {
                isReserveLevelUpEffect = false;
            }
            else
            {
                if( mainFieldController.IsStableIdle() )
                {
                    //startLevelUpEffect();
                    levelUpEffectCtrl.StartLargeEffect( currLevel );
                    isReserveLevelUpEffect = false;
                }
            }
        }

        if( isInterstialClosedCallback )
        {
            if( BlockController.blockBeginDragged != null )
            {
                BlockController.blockBeginDragged.GetComponent<BlockController>().BackToOrgPosition();
            }
            mainFieldController.SetSlotColorNormal();
            isInterstialClosedCallback = false;
        }

        //Debug.Log( "birthCounter : " + birthCounter + ", level : " + currLevel );
	}

	//------------------------------------------------------------------------------
    private void changeState( int state )
    {
        nextState = state;
	}

	//------------------------------------------------------------------------------
    //private void startLevelUpEffect()
    //{
    //    levelUpEffect.SetActive(true);
    //    levelUpEffectText.text = "Level " + ( currLevel + 1 ).ToString();
    //    StartCoroutine( this.DelayMethod( 0.2f, () => {
    //                    SoundManager.Instance.PlaySe( "levelup" );
    //                    levelText.text = ( currLevel + 1 ).ToString();
    //                } ) );
    //}

	//------------------------------------------------------------------------------
    public void SetDraggable( bool draggable )
    {
        canvasGroup.blocksRaycasts = draggable;
	}

	//------------------------------------------------------------------------------
	public void SetItemOnSlot( Transform transform ) {
		transform.SetParent( slot.transform );
		transform.localPosition = new Vector3( 0.0f, 0.0f, 0.0f );
		/* transform.localScale = new Vector3 ( 1.0f, 1.0f, 1.0f ) * 0.95f; */
		transform.localScale = new Vector3 ( 1.0f, 1.0f, 1.0f );
	}

	//------------------------------------------------------------------------------
    public GameObject CreateBlock()
    {
        GameObject obj = Instantiate( blockPrehab, new Vector3( 0, 0, 0 ), Quaternion.identity ) as GameObject;
        var rect = obj.GetComponent<RectTransform>();
        float size = GameSceneManager.Instance.GameMode == EGameMode.four ? Parameters.BLOCK_SIZE_4 : Parameters.BLOCK_SIZE_3;
        rect.sizeDelta = new Vector2( size, size );
        return obj;
    }

	//------------------------------------------------------------------------------
    public void SetSaveData( string str)
    {
        Debug.Log( "SetSaveData : " + str );
        InvalidateNextBlockObjSize();

        if( str == "" ) return;

        char[] delimiter = { ',' };
        string[] input = str.Split( delimiter );

        BlockController block = slot.block.GetComponent< BlockController >();
        int num = Int32.Parse( input[ 0 ] );
        try {
            if( num <= 0 )
            {
                throw new Exception( "stock number is " + num );
            }
            block.SetNumberImidiately( num );

            for( int i = 0; i < numPreNumbers; ++i )
            {
                num = Int32.Parse( input[ i + 1 ] );
                //Debug.Log( "stock num : " + num );
                if( num <= 0 )
                {
                    throw new Exception( "stock number is 0!" );
                }
                preNumbers[i] = num;
                //nextBlockObjs[ i ].GetComponentInChildren<Text>().text = input[ i + 1 ];
                //nextBlockObjs[ i ].GetComponentInChildren<Image>().color = ColorManager.Instance.GetBlockColor( Int32.Parse( input[ i + 1 ] ) );
                var next_ctrl = nextBlockObjs[ i ].GetComponent<NextObjController>();
                next_ctrl.Init( preNumbers[ i ] );

            }
        }
        catch ( Exception e )
        {
            string message = WK.Translate.TranslateManager.Instance.GetText( "2500" ) + "\n" + e.Message;
            CommonDialogManager.Instance.SetDialog( message, null );
            CommonDialogManager.Instance.EnterNotationDialog();
            CommonDialogManager.Instance.SetExitCompleteCallback( () => {
                    ResumeManager.Instance.Clear();
                    Application.Quit();
                    }
                    );
        }
    }

	//------------------------------------------------------------------------------
    public string GetSaveData()
    {
        string str = "";

        Debug.Assert( slot.block != null );
        if( slot.block == null ) return str;///念のため

        BlockController block = slot.block.GetComponent< BlockController >();
        str += block.number.ToString();
        str += ",";
        for( int i = 0; i < numPreNumbers; ++i )
        {
//#if UNITY_EDITOR
//            str += "0";
//#else
//            str += nextBlockObjs[ i ].GetComponentInChildren<Text>().text;
//#endif
            str += nextBlockObjs[ i ].GetComponentInChildren<Text>().text;
            str += ",";
        }

        return str;
    }

	//------------------------------------------------------------------------------
    public string GetClearSaveDataStr()
    {
        return "";
    }

	//------------------------------------------------------------------------------
    public void UpdateSkin()
    {
        for( int i = 0; i < numPreNumbers; ++i )
        {
            var next = nextBlockObjs[ i ];
            var next_text = next.GetComponentInChildren<Text>();
            var next_ctrl = nextBlockObjs[ i ].GetComponent<NextObjController>();

            int number = 2;
            if( Int32.TryParse( next_text.text, out number ) )
            {
                next_ctrl.Init( number );
            }
            //next_image.sprite = ColorManager.Instance.NextSprite;
        }

        var block = slot.block;
        if( block != null )
        {
            block.GetComponent< BlockController >().UpdateSkin();
        }
    }

}
