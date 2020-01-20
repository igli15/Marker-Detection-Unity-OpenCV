using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MarkerManager
{
    private static Dictionary<int, MarkerBehaviour> allMakers = new Dictionary<int, MarkerBehaviour>();

    public static void RegisterMarker(MarkerBehaviour m)
    {
        allMakers.Add(m.GetMarkerID(), m);
    }
    public static MarkerBehaviour GetMarker(int markerId)
    {
        if (allMakers.ContainsKey(markerId))
        {
            return allMakers[markerId];
        }
        else
        {
            return null;
        }
    }
    public static bool IsMarkerRegistered(int markerId)
    {
        return allMakers.ContainsKey(markerId);
    }
    
}
