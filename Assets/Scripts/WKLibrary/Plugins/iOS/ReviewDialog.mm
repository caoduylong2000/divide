#import <StoreKit/StoreKit.h>

extern "C" {

    void ShowReviewDialog ()
    {
    // レビューダイアログ呼び出し
        [SKStoreReviewController requestReview];
    }
    
}
