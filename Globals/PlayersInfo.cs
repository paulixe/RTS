using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersInfo : MonoBehaviour
{

    private static Dictionary<string, float> PlayerResources;
    static PlayersInfo()
    {
        PlayerResources = new Dictionary<string, float>();
        PlayerResources.Add("Food", 1000);
        PlayerResources.Add("Wood", 1000);
        PlayerResources.Add("Steel", 1000);
    }
    public static void Increase(string resource, float amount)
    {
        if (resource == "Fish" || resource == "Wheat" || resource == "Meat")
        {
            resource = "Food";
        }
        if (PlayerResources.ContainsKey(resource))
        {
            PlayerResources[resource] += amount;
        }
        else { throw new System.Exception("Wrong Key"); }
    }
    public static void Increase(float[] amount)
    {
        Increase("Wood", amount[0]);
        Increase("Steel",amount[1]);
        Increase("Food", amount[2]);
    }
    public static void Pay(string resource, float price)
    {
        if (resource == "Fish" || resource == "Wheat" || resource == "Meat")
        {
            resource = "Food";
        }
        if (PlayerResources.ContainsKey(resource))
        {
            if (HasEnough(resource, price))
            {
                PlayerResources[resource] -= price;
            }

        }
        else { throw new System.Exception("Wrong Key"); }
    }
    public static float GetAmount(string resource)
    {
        if (PlayerResources.ContainsKey(resource))
        {
            return PlayerResources[resource];
        }
        else
        {
            throw new System.Exception("Wrong Key");
        }
    }
    public static bool HasEnough(string resource, float price)
    {
        return GetAmount(resource) >= price;
    }
    public static bool HasEnoughResource(float[] price)
    {
        if (HasEnough("Wood", price[0]) && HasEnough("Steel", price[1]) && HasEnough("Food", price[2]))
        {
            return true;
        }
        else 
        {
            UIManager.AlertS("Vous n'avez pas assez de ressource");
            return false;
        }

    }
    public static void Pay(float[] price)
    {
        if (HasEnoughResource(price))
        {
            Pay("Wood", price[0]);
            Pay("Steel", price[1]);
            Pay("Food", price[2]);
        }
    }


}
