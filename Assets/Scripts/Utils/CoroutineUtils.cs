using System;
using System.Collections;

public static class CoroutineUtils
{
    public static IEnumerator WaitOneFrame(Action callback)
    {
        yield return null; // Waits for the next frame
        callback();
    }
}
