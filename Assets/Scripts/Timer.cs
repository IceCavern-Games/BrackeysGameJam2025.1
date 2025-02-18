using System;
using UnityEngine;

public class Timer
{
    public event Action Finished;

    public bool IsActive { get; private set; } = false;
    public float Time { get; private set; }
    
    public float ElapsedTime { get; private set; }

    private readonly float _initialTimeInSeconds;

    public Timer(float timeInSeconds, bool startNow = true)
    {
        _initialTimeInSeconds = timeInSeconds;
        Time = _initialTimeInSeconds;
        IsActive = startNow;
    }

    /// <summary>
    /// Reset the timer back to its initial value.
    /// </summary>
    public void Reset()
    {
        Time = _initialTimeInSeconds;
        ElapsedTime = 0;
    }

    /// <summary>
    /// Start the timer.
    /// </summary>
    public void Start()
    {
        IsActive = true;
    }

    /// <summary>
    /// Stop the timer from running.
    /// </summary>
    public void Stop()
    {
        IsActive = false;
    }

    /// <summary>
    /// Decrement the time by the given amount.
    /// </summary>
    public void Tick(float delta)
    {
        if (!IsActive)
            return;

        Time = Mathf.Max(Time - delta, 0.0f);
        ElapsedTime += delta;

        if (Time == 0.0f)
        {
            IsActive = false;
            Finished?.Invoke();
        }
    }
}

