using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MarkerManager
{
    private static Dictionary<int, MarkerBehaviour> allMakers = new Dictionary<int, MarkerBehaviour>();
    private static Dictionary<int, ARMarkerPlane> allMakerPlanes = new Dictionary<int, ARMarkerPlane>();
    
    
    public static void RegisterMarker(MarkerBehaviour m)
    {
        allMakers.Add(m.GetMarkerID(), m);
    }
    
    public static void RegisterMarkerPlane(ARMarkerPlane m)
    {
        allMakerPlanes.Add(m.arenaID, m);
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
    
    public static ARMarkerPlane GetMarkerPlane(int arenaID)
    {
        if (allMakerPlanes.ContainsKey(arenaID))
        {
            return allMakerPlanes[arenaID];
        }
        else
        {
            return null;
        }
    }

    public static ARMarkerPlane GetPlaneBasedOnMarkerID(int id)
    {
        foreach (ARMarkerPlane plane in allMakerPlanes.Values)
        {
            if (plane.ContainsMarkerID(id))
            {
                return plane;
            }
        }
        return null;
    }
    

    public static bool IsMarkerRegistered(int markerId)
    {
        return allMakers.ContainsKey(markerId);
    }

    public static bool CompareMarkers(MarkerBehaviour m1, MarkerBehaviour m2)
    {
        return m1.GetInstanceID() == m2.GetMarkerID();
    }

}
