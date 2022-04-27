using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Meat : Ressources
{
    private Coroutine RotRoutine;
    private static int TimeToRot = 4;
    private float initialAmount;
    public Meat(float amountOfMeat)
    {
        amountOfResource = amountOfMeat;
        initialAmount = amountOfMeat;
        RotRoutine = InputManager.StartCoroutineS(Rot());
    }
    private IEnumerator Rot()
    {

        while (true)
        {
            yield return new WaitForSeconds(TimeToRot);
            LoseLife(0.07f * initialAmount);
        }
    }
    protected override void EndOfResource()
    {

        InputManager.StopCoroutineS(RotRoutine);
        base.EndOfResource();

    }
}
