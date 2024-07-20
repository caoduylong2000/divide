public static class PrefKeys
{
	public static readonly string RANKING_NAME                   = "rankingName";

	public static string BEST_SCORE_KEY                  = "bestScore";
	public static string BEST_SCORE_V1_3_KEY             = "bestScoreV1_3";
	public static string BEST_SCORE_4_KEY                = "bestScore4";
	public static string BEST_SCORE_TIME_LIMIT_KEY       = "bestScoreTimeLimit";
	public static string IS_FIRST_LAUNCH                 = "isFirst";
	public static string IS_TUTORIAL_FINISHED            = "isTutorialFinished";
	public static string IS_RANKING_IMPLEMENTED          = "isRankingImplemented";
	public static string APP_VERSION                     = "appVersionKey";
	public static string IS_PURCHASED_NO_ADS             = "isPurchasedNoAds";
	public static string NUM_USING_BACK_STEP_COUNT       = "numUsingBackStepCount";
	public static string EXPIRE_SEC_SUBSCRIPTION_NO_ADS  = "EXPIRE_SEC_SUBSCRIPTION_NO_ADS";

	//public static readonly string RANKING_OBJ_ID                 = "rankingObjId";
	public static readonly string RANKING_SENDED_SCORE           = "rankingSendedScore";
	public static readonly string RANKING_RESERVED_SENDING_SCORE = "rankingReservedSendingScore";
	public static readonly string RANKING_WEEKLY_OBJ_ID          = "rankingWeeklyObjId";

	public static readonly string RANKING_FOUR_SENDED_SCORE           = "rankingFourSendedScore";
	public static readonly string RANKING_FOUR_RESERVED_SENDING_SCORE = "rankingFourReservedSendingScore";
	public static readonly string RANKING_FOUR_WEEKLY_OBJ_ID          = "rankingFourWeeklyObjId";

	//public static readonly string RANKING_TIME_LIMIT_OBJ_ID                 = "rankingTimeLimitObjId";
	public static readonly string RANKING_TIME_LIMIT_SENDED_SCORE           = "rankingTimeLimitSendedScore";
	public static readonly string RANKING_TIME_LIMIT_RESERVED_SENDING_SCORE = "rankingTimeLimitReservedSendingScore";
	public static readonly string RANKING_WEEKLY_TIME_LIMIT_OBJ_ID          = "rankingTimeLimitWeeklyObjId";

	public static readonly string PREV_RANKING_RECEIVE_TIME = "prevRankingReceiveTime";
	public static readonly string PREV_FOUR_RANKING_RECEIVE_TIME = "prevFourRankingReceiveTime";
	public static readonly string PREV_TIME_LIMIT_RANKING_RECEIVE_TIME = "prevTimeLimitRankingReceiveTime";

	public static readonly string TOP100_NUM    = "top100Num";
	public static readonly string TOP100_NAME   = "top100Name";
	public static readonly string TOP100_OBJ_ID = "top100ObjID";
	public static readonly string TOP100_SCORE  = "top100Score";

	public static readonly string TOP100_FOUR_NUM    = "top100FourNum";
	public static readonly string TOP100_FOUR_NAME   = "top100FourName";
	public static readonly string TOP100_FOUR_OBJ_ID = "top100FourObjID";
	public static readonly string TOP100_FOUR_SCORE  = "top100FourScore";

	public static readonly string TOP100_TIME_LIMIT_NUM    = "top100TimeLimitNum";
	public static readonly string TOP100_TIME_LIMIT_NAME   = "top100TimeLimitName";
	public static readonly string TOP100_TIME_LIMIT_OBJ_ID = "top100TimeLimitObjID";
	public static readonly string TOP100_TIME_LIMIT_SCORE  = "top100TimeLimitScore";

	public static readonly string IS_RESUMABLE = "waken_IsResumable";

	public static readonly string WEEK_BEST_KEY            = "waken_weekBestScore";
	public static readonly string WEEK_BEST4_KEY           = "waken_weekBest4Score";
	public static readonly string WEEK_TIME_LIMIT_BEST_KEY = "waken_weekTimeLimitBestScore";
	public static readonly string WEEK_BEST_WEEK_KEY       = "waken_weekBestWeek";

	public static readonly string SKIN_INDEX               = "SKIN_INDEX";

	public static readonly string VIBRATE               = "VIBRATE";
	public static readonly string UNDO_COUNT            = "UNDO_COUNT";
	/* public static readonly string WEEK_TIME_LIMIT_BEST_WEEK_KEY = "waken_weekTimeLimitBestWeek"; */
}

public static class NCMBQueryNames
{
	//public static readonly string HighScore          = "HighScore";
	//public static readonly string HighScore4         = "HighScore4";
	//public static readonly string HighScoreTimeLimit = "HighScoreTimeLimit";

	public static readonly string WeeklyHighScore          = "WeeklyHighScore";
	public static readonly string WeeklyHighScore4         = "WeeklyHighScore4";
	public static readonly string WeeklyHighScoreTimeLimit = "WeeklyHighScoreTimeLimit";
}

public static class NCMBKeys
{
	public static readonly string Name = "Name";
	public static readonly string Score = "Score";
	public static readonly string Week = "Week";
}
