using UnityEngine;

public static class UtilityCurve
{


    public static Vector2 CalculateBezier2d(Vector2 start, Vector2 control, Vector2 end, float t)
    {
        return (((1 - t) * (1 - t)) * start) + (2 * t * (1 - t) * control) + ((t * t) * end);
    }

    public static Vector3 CalculateBezier3d(Vector3 start, Vector3 control, Vector3 end, float t)
    {
        return (((1 - t) * (1 - t)) * start) + (2 * t * (1 - t) * control) + ((t * t) * end);
    }



}
