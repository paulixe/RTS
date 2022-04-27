using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public interface IUpgradable :IHasLevel
{
    public List<Tuple<string, float, float>> GetUpgradable();
    public float[] GetCost();// cost[0]:wood, cost[1] : steel , cost[2] : food
    public void Upgrade();
  
}
