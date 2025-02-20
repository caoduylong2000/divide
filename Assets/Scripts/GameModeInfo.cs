public class GameModeInfo {
    public int[] levelUpExp;
    public int[] levelMaxNumber;
    public int[] levelPrimeNumberRate;
    public float[] levelOverThreeDigitRate;
	public int[] primeNumberList;
	public int[] numberList;
    public int minNumber = 6;
    public int maxLevel = 17;
    public int overThreeDigitLevel = 8;
    public int overThreeDigitStartIndex = 60;
};

public class GameModeInfo3 : GameModeInfo {
    public GameModeInfo3()
    {
        maxLevel = 17;

        levelUpExp = new int[ maxLevel - 1 ];
        levelMaxNumber = new int[ maxLevel ];
        levelPrimeNumberRate = new int[ maxLevel ];
        levelOverThreeDigitRate = new float[ maxLevel ];

        levelUpExp[ 0  ] = 12;
        levelUpExp[ 1  ] = 30;
        levelUpExp[ 2  ] = 60;
        levelUpExp[ 3  ] = 100;
        levelUpExp[ 4  ] = 160;
        levelUpExp[ 5  ] = 240;
        levelUpExp[ 6  ] = 320;
        levelUpExp[ 7  ] = 480;
        levelUpExp[ 8  ] = 640;
        levelUpExp[ 9  ] = 800;
        levelUpExp[ 10 ] = 960;
        levelUpExp[ 11 ] = 1120;
        levelUpExp[ 12 ] = 1280;
        levelUpExp[ 13 ] = 1440;
        levelUpExp[ 14 ] = 1600;
        levelUpExp[ 15 ] = 1760;

        levelMaxNumber[ 0  ] = 19;
        levelMaxNumber[ 1  ] = 34;
        levelMaxNumber[ 2  ] = 59;
        levelMaxNumber[ 3  ] = 59;
        levelMaxNumber[ 4  ] = 59;
        levelMaxNumber[ 5  ] = 59;
        levelMaxNumber[ 6  ] = 59;
        levelMaxNumber[ 7  ] = 59;
        levelMaxNumber[ 8  ] = 96;
        levelMaxNumber[ 9  ] = 104;
        levelMaxNumber[ 10 ] = 112;
        levelMaxNumber[ 11 ] = 120;
        levelMaxNumber[ 12 ] = 128;
        levelMaxNumber[ 13 ] = 150;
        levelMaxNumber[ 14 ] = 170;
        levelMaxNumber[ 15 ] = 200;
        levelMaxNumber[ 16 ] = 240;

        levelPrimeNumberRate[ 0  ] = 4;
        levelPrimeNumberRate[ 1  ] = 4;
        levelPrimeNumberRate[ 2  ] = 5;
        levelPrimeNumberRate[ 3  ] = 6;
        levelPrimeNumberRate[ 4  ] = 7;
        levelPrimeNumberRate[ 5  ] = 8;
        levelPrimeNumberRate[ 6  ] = 9;
        levelPrimeNumberRate[ 7  ] = 10;
        levelPrimeNumberRate[ 8  ] = 10;
        levelPrimeNumberRate[ 9  ] = 10;
        levelPrimeNumberRate[ 10 ] = 10;
        levelPrimeNumberRate[ 11 ] = 10;
        levelPrimeNumberRate[ 12 ] = 10;
        levelPrimeNumberRate[ 13 ] = 10;
        levelPrimeNumberRate[ 14 ] = 10;
        levelPrimeNumberRate[ 15 ] = 10;
        levelPrimeNumberRate[ 16 ] = 10;

        levelOverThreeDigitRate[ 0  ] = 0.0f;
        levelOverThreeDigitRate[ 1  ] = 0.0f;
        levelOverThreeDigitRate[ 2  ] = 0.0f;
        levelOverThreeDigitRate[ 3  ] = 0.0f;
        levelOverThreeDigitRate[ 4  ] = 0.0f;
        levelOverThreeDigitRate[ 5  ] = 0.0f;
        levelOverThreeDigitRate[ 6  ] = 0.0f;
        levelOverThreeDigitRate[ 7  ] = 0.0f;
        levelOverThreeDigitRate[ 8  ] = 0.2f;
        levelOverThreeDigitRate[ 9  ] = 0.25f;
        levelOverThreeDigitRate[ 10 ] = 0.33f;
        levelOverThreeDigitRate[ 11 ] = 0.50f;
        levelOverThreeDigitRate[ 12 ] = 1.0f;
        levelOverThreeDigitRate[ 13 ] = 1.0f;
        levelOverThreeDigitRate[ 14 ] = 1.0f;
        levelOverThreeDigitRate[ 15 ] = 1.0f;
        levelOverThreeDigitRate[ 16 ] = 1.0f;

        int primeNumberNum = 6;
        primeNumberList = new int[ primeNumberNum ];
        primeNumberList[ 0 ] = 2;
        primeNumberList[ 1 ] = 3;
        primeNumberList[ 2 ] = 5;
        primeNumberList[ 3 ] = 7;
        primeNumberList[ 4 ] = 11;
        primeNumberList[ 5 ] = 13;
        minNumber = primeNumberNum;

		int numberNum = 241;
		numberList = new int[ numberNum ];

		numberList[ 0 ] = -1;
		numberList[ 1 ] = -1;
		numberList[ 2 ] = -1;
		numberList[ 3 ] = -1;
		numberList[ 4 ] = -1;
		numberList[ 5 ] = -1;
		numberList[ 6 ] = 4;
		numberList[ 7 ] = 6;
		numberList[ 8 ] = 8;
		numberList[ 9 ] = 9;
		numberList[ 10 ] = 10;
		numberList[ 11 ] = 12;
		numberList[ 12 ] = 14;
		numberList[ 13 ] = 15;
		numberList[ 14 ] = 16;
		numberList[ 15 ] = 18;
		numberList[ 16 ] = 20;
		numberList[ 17 ] = 21;
		numberList[ 18 ] = 22;
		numberList[ 19 ] = 24;
		numberList[ 20 ] = 25;
		numberList[ 21 ] = 26;
		numberList[ 22 ] = 27;
		numberList[ 23 ] = 28;
		numberList[ 24 ] = 30;
		numberList[ 25 ] = 32;
		numberList[ 26 ] = 33;
		numberList[ 27 ] = 35;
		numberList[ 28 ] = 36;
		numberList[ 29 ] = 39;
		numberList[ 30 ] = 40;
		numberList[ 31 ] = 42;
		numberList[ 32 ] = 44;
		numberList[ 33 ] = 45;
		numberList[ 34 ] = 48;
		numberList[ 35 ] = 49;
		numberList[ 36 ] = 50;
		numberList[ 37 ] = 52;
		numberList[ 38 ] = 54;
		numberList[ 39 ] = 55;
		numberList[ 40 ] = 56;
		numberList[ 41 ] = 60;
		numberList[ 42 ] = 63;
		numberList[ 43 ] = 64;
		numberList[ 44 ] = 65;
		numberList[ 45 ] = 66;
		numberList[ 46 ] = 70;
		numberList[ 47 ] = 72;
		numberList[ 48 ] = 75;
		numberList[ 49 ] = 77;
		numberList[ 50 ] = 78;
		numberList[ 51 ] = 80;
		numberList[ 52 ] = 81;
		numberList[ 53 ] = 84;
		numberList[ 54 ] = 88;
		numberList[ 55 ] = 90;
		numberList[ 56 ] = 91;
		numberList[ 57 ] = 96;
		numberList[ 58 ] = 98;
		numberList[ 59 ] = 99;
		numberList[ 60 ] = 100;
		numberList[ 61 ] = 104;
		numberList[ 62 ] = 105;
		numberList[ 63 ] = 108;
		numberList[ 64 ] = 110;
		numberList[ 65 ] = 112;
		numberList[ 66 ] = 117;
		numberList[ 67 ] = 120;
		numberList[ 68 ] = 121;
		numberList[ 69 ] = 125;
		numberList[ 70 ] = 126;
		numberList[ 71 ] = 128;
		numberList[ 72 ] = 130;
		numberList[ 73 ] = 132;
		numberList[ 74 ] = 135;
		numberList[ 75 ] = 140;
		numberList[ 76 ] = 143;
		numberList[ 77 ] = 144;
		numberList[ 78 ] = 147;
		numberList[ 79 ] = 150;
		numberList[ 80 ] = 154;
		numberList[ 81 ] = 156;
		numberList[ 82 ] = 160;
		numberList[ 83 ] = 162;
		numberList[ 84 ] = 165;
		numberList[ 85 ] = 168;
		numberList[ 86 ] = 169;
		numberList[ 87 ] = 175;
		numberList[ 88 ] = 176;
		numberList[ 89 ] = 180;
		numberList[ 90 ] = 182;
		numberList[ 91 ] = 189;
		numberList[ 92 ] = 192;
		numberList[ 93 ] = 195;
		numberList[ 94 ] = 196;
		numberList[ 95 ] = 198;
		numberList[ 96 ] = 200;
		numberList[ 97 ] = 208;
		numberList[ 98 ] = 210;
		numberList[ 99 ] = 216;
		numberList[ 100 ] = 220;
		numberList[ 101 ] = 224;
		numberList[ 102 ] = 225;
		numberList[ 103 ] = 231;
		numberList[ 104 ] = 234;
		numberList[ 105 ] = 240;
		numberList[ 106 ] = 242;
		numberList[ 107 ] = 243;
		numberList[ 108 ] = 245;
		numberList[ 109 ] = 250;
		numberList[ 110 ] = 252;
		numberList[ 111 ] = 256;
		numberList[ 112 ] = 260;
		numberList[ 113 ] = 264;
		numberList[ 114 ] = 270;
		numberList[ 115 ] = 273;
		numberList[ 116 ] = 275;
		numberList[ 117 ] = 280;
		numberList[ 118 ] = 286;
		numberList[ 119 ] = 288;
		numberList[ 120 ] = 294;
		numberList[ 121 ] = 297;
		numberList[ 122 ] = 300;
		numberList[ 123 ] = 308;
		numberList[ 124 ] = 312;
		numberList[ 125 ] = 315;
		numberList[ 126 ] = 320;
		numberList[ 127 ] = 324;
		numberList[ 128 ] = 325;
		numberList[ 129 ] = 330;
		numberList[ 130 ] = 336;
		numberList[ 131 ] = 338;
		numberList[ 132 ] = 343;
		numberList[ 133 ] = 350;
		numberList[ 134 ] = 351;
		numberList[ 135 ] = 352;
		numberList[ 136 ] = 360;
		numberList[ 137 ] = 363;
		numberList[ 138 ] = 364;
		numberList[ 139 ] = 375;
		numberList[ 140 ] = 378;
		numberList[ 141 ] = 384;
		numberList[ 142 ] = 385;
		numberList[ 143 ] = 390;
		numberList[ 144 ] = 392;
		numberList[ 145 ] = 396;
		numberList[ 146 ] = 400;
		numberList[ 147 ] = 405;
		numberList[ 148 ] = 416;
		numberList[ 149 ] = 420;
		numberList[ 150 ] = 429;
		numberList[ 151 ] = 432;
		numberList[ 152 ] = 440;
		numberList[ 153 ] = 441;
		numberList[ 154 ] = 448;
		numberList[ 155 ] = 450;
		numberList[ 156 ] = 455;
		numberList[ 157 ] = 462;
		numberList[ 158 ] = 468;
		numberList[ 159 ] = 480;
		numberList[ 160 ] = 484;
		numberList[ 161 ] = 486;
		numberList[ 162 ] = 490;
		numberList[ 163 ] = 495;
		numberList[ 164 ] = 500;
		numberList[ 165 ] = 504;
		numberList[ 166 ] = 507;
		numberList[ 167 ] = 512;
		numberList[ 168 ] = 520;
		numberList[ 169 ] = 525;
		numberList[ 170 ] = 528;
		numberList[ 171 ] = 539;
		numberList[ 172 ] = 540;
		numberList[ 173 ] = 546;
		numberList[ 174 ] = 550;
		numberList[ 175 ] = 560;
		numberList[ 176 ] = 567;
		numberList[ 177 ] = 572;
		numberList[ 178 ] = 576;
		numberList[ 179 ] = 585;
		numberList[ 180 ] = 588;
		numberList[ 181 ] = 594;
		numberList[ 182 ] = 600;
		numberList[ 183 ] = 605;
		numberList[ 184 ] = 616;
		numberList[ 185 ] = 624;
		numberList[ 186 ] = 625;
		numberList[ 187 ] = 630;
		numberList[ 188 ] = 637;
		numberList[ 189 ] = 640;
		numberList[ 190 ] = 648;
		numberList[ 191 ] = 650;
		numberList[ 192 ] = 660;
		numberList[ 193 ] = 672;
		numberList[ 194 ] = 675;
		numberList[ 195 ] = 676;
		numberList[ 196 ] = 686;
		numberList[ 197 ] = 693;
		numberList[ 198 ] = 700;
		numberList[ 199 ] = 702;
		numberList[ 200 ] = 704;
		numberList[ 201 ] = 715;
		numberList[ 202 ] = 720;
		numberList[ 203 ] = 726;
		numberList[ 204 ] = 728;
		numberList[ 205 ] = 729;
		numberList[ 206 ] = 735;
		numberList[ 207 ] = 750;
		numberList[ 208 ] = 756;
		numberList[ 209 ] = 768;
		numberList[ 210 ] = 770;
		numberList[ 211 ] = 780;
		numberList[ 212 ] = 784;
		numberList[ 213 ] = 792;
		numberList[ 214 ] = 800;
		numberList[ 215 ] = 810;
		numberList[ 216 ] = 819;
		numberList[ 217 ] = 825;
		numberList[ 218 ] = 832;
		numberList[ 219 ] = 840;
		numberList[ 220 ] = 845;
		numberList[ 221 ] = 847;
		numberList[ 222 ] = 858;
		numberList[ 223 ] = 864;
		numberList[ 224 ] = 875;
		numberList[ 225 ] = 880;
		numberList[ 226 ] = 882;
		numberList[ 227 ] = 891;
		numberList[ 228 ] = 896;
		numberList[ 229 ] = 900;
		numberList[ 230 ] = 910;
		numberList[ 231 ] = 924;
		numberList[ 232 ] = 936;
		numberList[ 233 ] = 945;
		numberList[ 234 ] = 960;
		numberList[ 235 ] = 968;
		numberList[ 236 ] = 972;
		numberList[ 237 ] = 975;
		numberList[ 238 ] = 980;
		numberList[ 239 ] = 990;
		numberList[ 240 ] = 1000;//240は使われないはずだが一応
    }
};

public class GameModeInfo3Time : GameModeInfo3 {
    public GameModeInfo3Time()
    {
        levelUpExp[ 0  ] = 10;
        levelUpExp[ 1  ] = 20;
        levelUpExp[ 2  ] = 30;
        levelUpExp[ 3  ] = 40;
        levelUpExp[ 4  ] = 50;
        levelUpExp[ 5  ] = 60;
        levelUpExp[ 6  ] = 70;
        levelUpExp[ 7  ] = 80;
        levelUpExp[ 8  ] = 90;
        levelUpExp[ 9  ] = 140;
        levelUpExp[ 10 ] = 200;
        levelUpExp[ 11 ] = 300;
        levelUpExp[ 12 ] = 400;
        levelUpExp[ 13 ] = 500;
        levelUpExp[ 14 ] = 600;
        levelUpExp[ 15 ] = 700;
    }
};

public class GameModeInfo4 : GameModeInfo3 {
    public GameModeInfo4()
    {
        //maxLevel = 17;
        maxLevel = 50;
        //overThreeDigitStartIndex = 70;//19まで使う時
        overThreeDigitStartIndex = 65;

        overThreeDigitLevel = 18;

        levelUpExp = new int[ maxLevel - 1 ];
        levelMaxNumber = new int[ maxLevel ];
        levelPrimeNumberRate = new int[ maxLevel ];
        levelOverThreeDigitRate = new float[ maxLevel ];

        //1操作辺り2秒
        //1レベル辺り3分想定
        //2時間で死亡させる -> Lv 40がmax
        levelUpExp[ 0 ] = 20;
        levelUpExp[ 1 ] = 70;
        for( int i = 2; i < maxLevel - 1; ++i )
        {
            levelUpExp[ i ] = 70 + 50 * ( i - 1 );
        }

        //最大328 = 34 + 6 * 49 //19まで使う時
        //最大286 = 64 + 6 * 49 //17まで使う時
        levelMaxNumber[ 0  ] = 19;
        levelMaxNumber[ 1  ] = 34;
        levelMaxNumber[ 2  ] = 36;
        levelMaxNumber[ 3  ] = 38;
        levelMaxNumber[ 4  ] = 40;
        levelMaxNumber[ 5  ] = 42;
        levelMaxNumber[ 6  ] = 44;
        levelMaxNumber[ 7  ] = 46;
        levelMaxNumber[ 8  ] = 48;
        levelMaxNumber[ 9  ] = 50;
        levelMaxNumber[ 10 ] = 52;
        levelMaxNumber[ 11 ] = 54;
        levelMaxNumber[ 12 ] = 56;
        levelMaxNumber[ 13 ] = 58;
        levelMaxNumber[ 14 ] = 60;
        levelMaxNumber[ 15 ] = 62;
        levelMaxNumber[ 16 ] = 64;
        levelMaxNumber[ 17 ] = 64;
        for( int i = 18; i < maxLevel; ++i )
        {
            levelMaxNumber[ i ] = 69 + 7 * ( i - 18 );
        }

        levelPrimeNumberRate[ 0  ] = 4;
        levelPrimeNumberRate[ 1  ] = 4;
        levelPrimeNumberRate[ 2  ] = 4;
        levelPrimeNumberRate[ 3  ] = 5;
        levelPrimeNumberRate[ 4  ] = 5;
        levelPrimeNumberRate[ 5  ] = 5;
        levelPrimeNumberRate[ 6  ] = 6;
        levelPrimeNumberRate[ 7  ] = 6;
        levelPrimeNumberRate[ 8  ] = 6;
        levelPrimeNumberRate[ 9  ] = 7;
        levelPrimeNumberRate[ 10 ] = 7;
        levelPrimeNumberRate[ 11 ] = 7;
        levelPrimeNumberRate[ 12 ] = 8;
        levelPrimeNumberRate[ 13 ] = 8;
        levelPrimeNumberRate[ 14 ] = 8;
        levelPrimeNumberRate[ 15 ] = 9;
        levelPrimeNumberRate[ 16 ] = 9;
        levelPrimeNumberRate[ 17 ] = 9;
        for( int i = 18; i < maxLevel; ++i )
        {
            levelPrimeNumberRate[ i ] = 10;
        }

        levelOverThreeDigitRate[ 0  ] = 0;
        levelOverThreeDigitRate[ 1  ] = 0;
        levelOverThreeDigitRate[ 2  ] = 0;
        levelOverThreeDigitRate[ 3  ] = 0;
        levelOverThreeDigitRate[ 4  ] = 0;
        levelOverThreeDigitRate[ 5  ] = 0;
        levelOverThreeDigitRate[ 6  ] = 0;
        levelOverThreeDigitRate[ 7  ] = 0;
        levelOverThreeDigitRate[ 8  ] = 0;
        levelOverThreeDigitRate[ 9  ] = 0;
        levelOverThreeDigitRate[ 10 ] = 0;
        levelOverThreeDigitRate[ 11 ] = 0;
        levelOverThreeDigitRate[ 12 ] = 0;
        levelOverThreeDigitRate[ 13 ] = 0;
        levelOverThreeDigitRate[ 14 ] = 0;
        levelOverThreeDigitRate[ 15 ] = 0;
        levelOverThreeDigitRate[ 16 ] = 0;
        levelOverThreeDigitRate[ 17 ] = 0;
        for( int i = 18; i < maxLevel; ++i )
        {
            //maxLevel/2 ぐらいで3桁が常に出るように
            levelOverThreeDigitRate[ i ] = 0.1f + ( 0.9f / ( ( maxLevel - 1 ) / 2 ) ) * ( i - 18 );
            levelOverThreeDigitRate[ i ] = UnityEngine.Mathf.Min( 1.0f, levelOverThreeDigitRate[ i ] );
        }

        int primeNumberNum = 7;
        primeNumberList = new int[ primeNumberNum ];
        primeNumberList[ 0 ] = 2;
        primeNumberList[ 1 ] = 3;
        primeNumberList[ 2 ] = 5;
        primeNumberList[ 3 ] = 7;
        primeNumberList[ 4 ] = 11;
        primeNumberList[ 5 ] = 13;
        primeNumberList[ 6 ] = 17;
        //primeNumberList[ 7 ] = 19;
        minNumber = primeNumberNum;

		int numberNum = 286;
		numberList = new int[ numberNum ];
		numberList[ 0 ] = -1;
		numberList[ 1 ] = -1;
		numberList[ 2 ] = -1;
		numberList[ 3 ] = -1;
		numberList[ 4 ] = -1;
		numberList[ 5 ] = -1;
		numberList[ 6 ] = -1;
		numberList[ 7 ] = 4;
		numberList[ 8 ] = 6;
		numberList[ 9 ] = 8;
		numberList[ 10 ] = 9;
		numberList[ 11 ] = 10;
		numberList[ 12 ] = 12;
		numberList[ 13 ] = 14;
		numberList[ 14 ] = 15;
		numberList[ 15 ] = 16;
		numberList[ 16 ] = 18;
		numberList[ 17 ] = 20;
		numberList[ 18 ] = 21;
		numberList[ 19 ] = 22;
		numberList[ 20 ] = 24;
		numberList[ 21 ] = 25;
		numberList[ 22 ] = 26;
		numberList[ 23 ] = 27;
		numberList[ 24 ] = 28;
		numberList[ 25 ] = 30;
		numberList[ 26 ] = 32;
		numberList[ 27 ] = 33;
		numberList[ 28 ] = 34;
		numberList[ 29 ] = 35;
		numberList[ 30 ] = 36;
		numberList[ 31 ] = 39;
		numberList[ 32 ] = 40;
		numberList[ 33 ] = 42;
		numberList[ 34 ] = 44;
		numberList[ 35 ] = 45;
		numberList[ 36 ] = 48;
		numberList[ 37 ] = 49;
		numberList[ 38 ] = 50;
		numberList[ 39 ] = 51;
		numberList[ 40 ] = 52;
		numberList[ 41 ] = 54;
		numberList[ 42 ] = 55;
		numberList[ 43 ] = 56;
		numberList[ 44 ] = 60;
		numberList[ 45 ] = 63;
		numberList[ 46 ] = 64;
		numberList[ 47 ] = 65;
		numberList[ 48 ] = 66;
		numberList[ 49 ] = 68;
		numberList[ 50 ] = 70;
		numberList[ 51 ] = 72;
		numberList[ 52 ] = 75;
		numberList[ 53 ] = 77;
		numberList[ 54 ] = 78;
		numberList[ 55 ] = 80;
		numberList[ 56 ] = 81;
		numberList[ 57 ] = 84;
		numberList[ 58 ] = 85;
		numberList[ 59 ] = 88;
		numberList[ 60 ] = 90;
		numberList[ 61 ] = 91;
		numberList[ 62 ] = 96;
		numberList[ 63 ] = 98;
		numberList[ 64 ] = 99;
		numberList[ 65 ] = 100;
		numberList[ 66 ] = 102;
		numberList[ 67 ] = 104;
		numberList[ 68 ] = 105;
		numberList[ 69 ] = 108;
		numberList[ 70 ] = 110;
		numberList[ 71 ] = 112;
		numberList[ 72 ] = 117;
		numberList[ 73 ] = 119;
		numberList[ 74 ] = 120;
		numberList[ 75 ] = 121;
		numberList[ 76 ] = 125;
		numberList[ 77 ] = 126;
		numberList[ 78 ] = 128;
		numberList[ 79 ] = 130;
		numberList[ 80 ] = 132;
		numberList[ 81 ] = 135;
		numberList[ 82 ] = 136;
		numberList[ 83 ] = 140;
		numberList[ 84 ] = 143;
		numberList[ 85 ] = 144;
		numberList[ 86 ] = 147;
		numberList[ 87 ] = 150;
		numberList[ 88 ] = 153;
		numberList[ 89 ] = 154;
		numberList[ 90 ] = 156;
		numberList[ 91 ] = 160;
		numberList[ 92 ] = 162;
		numberList[ 93 ] = 165;
		numberList[ 94 ] = 168;
		numberList[ 95 ] = 169;
		numberList[ 96 ] = 170;
		numberList[ 97 ] = 175;
		numberList[ 98 ] = 176;
		numberList[ 99 ] = 180;
		numberList[ 100 ] = 182;
		numberList[ 101 ] = 187;
		numberList[ 102 ] = 189;
		numberList[ 103 ] = 192;
		numberList[ 104 ] = 195;
		numberList[ 105 ] = 196;
		numberList[ 106 ] = 198;
		numberList[ 107 ] = 200;
		numberList[ 108 ] = 204;
		numberList[ 109 ] = 208;
		numberList[ 110 ] = 210;
		numberList[ 111 ] = 216;
		numberList[ 112 ] = 220;
		numberList[ 113 ] = 221;
		numberList[ 114 ] = 224;
		numberList[ 115 ] = 225;
		numberList[ 116 ] = 231;
		numberList[ 117 ] = 234;
		numberList[ 118 ] = 238;
		numberList[ 119 ] = 240;
		numberList[ 120 ] = 242;
		numberList[ 121 ] = 243;
		numberList[ 122 ] = 245;
		numberList[ 123 ] = 250;
		numberList[ 124 ] = 252;
		numberList[ 125 ] = 255;
		numberList[ 126 ] = 256;
		numberList[ 127 ] = 260;
		numberList[ 128 ] = 264;
		numberList[ 129 ] = 270;
		numberList[ 130 ] = 272;
		numberList[ 131 ] = 273;
		numberList[ 132 ] = 275;
		numberList[ 133 ] = 280;
		numberList[ 134 ] = 286;
		numberList[ 135 ] = 288;
		numberList[ 136 ] = 289;
		numberList[ 137 ] = 294;
		numberList[ 138 ] = 297;
		numberList[ 139 ] = 300;
		numberList[ 140 ] = 306;
		numberList[ 141 ] = 308;
		numberList[ 142 ] = 312;
		numberList[ 143 ] = 315;
		numberList[ 144 ] = 320;
		numberList[ 145 ] = 324;
		numberList[ 146 ] = 325;
		numberList[ 147 ] = 330;
		numberList[ 148 ] = 336;
		numberList[ 149 ] = 338;
		numberList[ 150 ] = 340;
		numberList[ 151 ] = 343;
		numberList[ 152 ] = 350;
		numberList[ 153 ] = 351;
		numberList[ 154 ] = 352;
		numberList[ 155 ] = 357;
		numberList[ 156 ] = 360;
		numberList[ 157 ] = 363;
		numberList[ 158 ] = 364;
		numberList[ 159 ] = 374;
		numberList[ 160 ] = 375;
		numberList[ 161 ] = 378;
		numberList[ 162 ] = 384;
		numberList[ 163 ] = 385;
		numberList[ 164 ] = 390;
		numberList[ 165 ] = 392;
		numberList[ 166 ] = 396;
		numberList[ 167 ] = 400;
		numberList[ 168 ] = 405;
		numberList[ 169 ] = 408;
		numberList[ 170 ] = 416;
		numberList[ 171 ] = 420;
		numberList[ 172 ] = 425;
		numberList[ 173 ] = 429;
		numberList[ 174 ] = 432;
		numberList[ 175 ] = 440;
		numberList[ 176 ] = 441;
		numberList[ 177 ] = 442;
		numberList[ 178 ] = 448;
		numberList[ 179 ] = 450;
		numberList[ 180 ] = 455;
		numberList[ 181 ] = 459;
		numberList[ 182 ] = 462;
		numberList[ 183 ] = 468;
		numberList[ 184 ] = 476;
		numberList[ 185 ] = 480;
		numberList[ 186 ] = 484;
		numberList[ 187 ] = 486;
		numberList[ 188 ] = 490;
		numberList[ 189 ] = 495;
		numberList[ 190 ] = 500;
		numberList[ 191 ] = 504;
		numberList[ 192 ] = 507;
		numberList[ 193 ] = 510;
		numberList[ 194 ] = 512;
		numberList[ 195 ] = 520;
		numberList[ 196 ] = 525;
		numberList[ 197 ] = 528;
		numberList[ 198 ] = 539;
		numberList[ 199 ] = 540;
		numberList[ 200 ] = 544;
		numberList[ 201 ] = 546;
		numberList[ 202 ] = 550;
		numberList[ 203 ] = 560;
		numberList[ 204 ] = 561;
		numberList[ 205 ] = 567;
		numberList[ 206 ] = 572;
		numberList[ 207 ] = 576;
		numberList[ 208 ] = 578;
		numberList[ 209 ] = 585;
		numberList[ 210 ] = 588;
		numberList[ 211 ] = 594;
		numberList[ 212 ] = 595;
		numberList[ 213 ] = 600;
		numberList[ 214 ] = 605;
		numberList[ 215 ] = 612;
		numberList[ 216 ] = 616;
		numberList[ 217 ] = 624;
		numberList[ 218 ] = 625;
		numberList[ 219 ] = 630;
		numberList[ 220 ] = 637;
		numberList[ 221 ] = 640;
		numberList[ 222 ] = 648;
		numberList[ 223 ] = 650;
		numberList[ 224 ] = 660;
		numberList[ 225 ] = 663;
		numberList[ 226 ] = 672;
		numberList[ 227 ] = 675;
		numberList[ 228 ] = 676;
		numberList[ 229 ] = 680;
		numberList[ 230 ] = 686;
		numberList[ 231 ] = 693;
		numberList[ 232 ] = 700;
		numberList[ 233 ] = 702;
		numberList[ 234 ] = 704;
		numberList[ 235 ] = 714;
		numberList[ 236 ] = 715;
		numberList[ 237 ] = 720;
		numberList[ 238 ] = 726;
		numberList[ 239 ] = 728;
		numberList[ 240 ] = 729;
		numberList[ 241 ] = 735;
		numberList[ 242 ] = 748;
		numberList[ 243 ] = 750;
		numberList[ 244 ] = 756;
		numberList[ 245 ] = 765;
		numberList[ 246 ] = 768;
		numberList[ 247 ] = 770;
		numberList[ 248 ] = 780;
		numberList[ 249 ] = 784;
		numberList[ 250 ] = 792;
		numberList[ 251 ] = 800;
		numberList[ 252 ] = 810;
		numberList[ 253 ] = 816;
		numberList[ 254 ] = 819;
		numberList[ 255 ] = 825;
		numberList[ 256 ] = 832;
		numberList[ 257 ] = 833;
		numberList[ 258 ] = 840;
		numberList[ 259 ] = 845;
		numberList[ 260 ] = 847;
		numberList[ 261 ] = 850;
		numberList[ 262 ] = 858;
		numberList[ 263 ] = 864;
		numberList[ 264 ] = 867;
		numberList[ 265 ] = 875;
		numberList[ 266 ] = 880;
		numberList[ 267 ] = 882;
		numberList[ 268 ] = 884;
		numberList[ 269 ] = 891;
		numberList[ 270 ] = 896;
		numberList[ 271 ] = 900;
		numberList[ 272 ] = 910;
		numberList[ 273 ] = 918;
		numberList[ 274 ] = 924;
		numberList[ 275 ] = 935;
		numberList[ 276 ] = 936;
		numberList[ 277 ] = 945;
		numberList[ 278 ] = 952;
		numberList[ 279 ] = 960;
		numberList[ 280 ] = 968;
		numberList[ 281 ] = 972;
		numberList[ 282 ] = 975;
		numberList[ 283 ] = 980;
		numberList[ 284 ] = 990;
		numberList[ 285 ] = 1000;

    }
}

