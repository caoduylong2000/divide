#import <Foundation/Foundation.h>
#import <AudioToolBox/AudioToolBox.h>

extern "C" void PlaySystemSound (int n)
{
    AudioServicesPlaySystemSound(n);
}
