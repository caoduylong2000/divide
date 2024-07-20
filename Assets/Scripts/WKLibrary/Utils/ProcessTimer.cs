using UnityEngine;//for debug.log
using System;
namespace WK { namespace Utils {

public class ProcessTimer {
    System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

    public void Start()
    {
        Debug.Assert( !stopWatch.IsRunning );
        stopWatch.Start();
    }

    public void Stop()
    {
        Debug.Assert( stopWatch.IsRunning );
        stopWatch.Stop();
    }

    public void Reset()
    {
        stopWatch.Reset();
    }

    public TimeSpan GetResult()
    {
        return stopWatch.Elapsed;
    }
}

public class ScopedProcessTimer : IDisposable{
    ProcessTimer timer = new ProcessTimer();

    public ScopedProcessTimer()
    {
        timer.Start();
    }

    public void Dispose()
    {
        timer.Stop();
        Debug.Log( "Process Time : " + timer.GetResult().TotalMilliseconds.ToString() + "msec" );
    }
}


}}
