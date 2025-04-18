using System;
using UnityEngine;

[Serializable]
public class RouteData
{
    public Guid id;
    public string routeType;
    public int stepOrder;
    public string title;
    public string description;
    public string iconName;
    public float x;
    public float y;
}
