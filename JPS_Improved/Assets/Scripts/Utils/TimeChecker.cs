using System.Diagnostics;

public static class TimeChecker
{
    private static Stopwatch _stopWatch;

    public static void StartTimer()
    {
        if (_stopWatch == null)
            _stopWatch = new Stopwatch();

        _stopWatch.Reset();
        _stopWatch.Start();
    }

    public static void ResetTimer()
    {
        _stopWatch?.Reset();
    }

    public static void RestartTimer()
    {
        _stopWatch?.Restart();
    }

    public static string StopTimer()
    {
        if (_stopWatch == null)
            return "타이머를 시작해 주세요";

        _stopWatch.Stop();
        return string.Format("{0:F5}", _stopWatch.ElapsedMilliseconds * 0.001f);
    }
}
