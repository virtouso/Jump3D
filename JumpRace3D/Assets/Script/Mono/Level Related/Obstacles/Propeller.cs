using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propeller : PathItemBase
{
    [SerializeField] private float _rotationSpeed;



    private void Update()
    {
        transform.RotateAround(transform.position, transform.up, _rotationSpeed * Time.deltaTime);
    }

}
