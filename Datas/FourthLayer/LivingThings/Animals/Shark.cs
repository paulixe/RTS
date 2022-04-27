using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Shark : Animal
{
    public override List<string> ClassCantWalkOn { get { return Map.CLASSAVOIDEDONTHESEA; } } 
}
