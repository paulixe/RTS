using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Environment : Data,IGround
{
    public virtual int SpeedPenalty { get; protected set; }
    public int GetSpeedPenalty()
    {
        return SpeedPenalty;
    }
}
