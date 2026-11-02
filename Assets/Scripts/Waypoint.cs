using System.Collections.Generic;
using UnityEngine;

public enum WaypointType { Street, Home, Shop, Bakery, Market }

// A node citizens walk between. Links are two-way: if A lists B, B should list A.
// One-way links are drawn red in the Scene view so they're easy to spot.
public class Waypoint : MonoBehaviour
{
    public WaypointType type = WaypointType.Street;
    public List<Waypoint> neighbours = new List<Waypoint>();

    public Vector3 Position => transform.position;

    public void LinkTo(Waypoint other)
    {
        if (other == null || other == this) return;
        if (!neighbours.Contains(other)) neighbours.Add(other);
        if (!other.neighbours.Contains(this)) other.neighbours.Add(this);
    }

    public void UnlinkFrom(Waypoint other)
    {
        if (other == null) return;
        neighbours.Remove(other);
        other.neighbours.Remove(this);
    }

    public static Color TypeColour(WaypointType t)
    {
        switch (t)
        {
            case WaypointType.Home: return new Color(0.4f, 0.7f, 1f);
            case WaypointType.Shop: return new Color(0.6f, 1f, 0.5f);
            case WaypointType.Bakery: return new Color(1f, 0.6f, 0.2f);
            case WaypointType.Market: return new Color(1f, 0.4f, 0.8f);
            default: return new Color(1f, 1f, 1f, 0.8f);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = TypeColour(type);
        Gizmos.DrawSphere(Position + Vector3.up * 0.1f, type == WaypointType.Street ? 0.25f : 0.35f);

        foreach (Waypoint n in neighbours)
        {
            if (n == null) continue;
            bool twoWay = n.neighbours.Contains(this);
            Gizmos.color = twoWay ? new Color(1f, 1f, 0.3f, 0.6f) : Color.red;
            Gizmos.DrawLine(Position + Vector3.up * 0.1f, n.Position + Vector3.up * 0.1f);
        }
    }
}
