using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPerishable
{
    public Vector2Int Pos { get; set; }
    public float LoseLife(float lifeLost);
}
