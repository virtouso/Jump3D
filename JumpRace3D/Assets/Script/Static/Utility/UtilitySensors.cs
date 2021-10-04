using UnityEngine;

public class UtilitySensors : MonoBehaviour
{

    public static bool CheckRayCastHit(Vector3 origin, Vector3 direction, LayerMask mask, float maxDistance, out Transform objectTransform, out Vector3 hitPosition)
    {

        bool isHit = Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, mask);
        hitPosition = hit.point;
        objectTransform = hit.transform;
        if (isHit)
        {
            Debug.DrawRay(origin, direction * maxDistance, Color.red);
        }
        else
        {
            Debug.DrawRay(origin, direction * maxDistance, Color.white);
        }

        return isHit;
    }

}
