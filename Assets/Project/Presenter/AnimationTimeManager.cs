using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class AnimationTimeManager
{
    float time;

    internal AnimationTimeManager()
    {
        Observable.Interval(TimeSpan.FromSeconds(0.1)).Where(_ => time > 0).Subscribe(_ => {
            time = Mathf.Max(time - 0.1f, 0);
        });
    }

    internal void WaitFor(float time)
    {
        this.time = time;
    }

    internal bool IsWating => time == 0;
}
