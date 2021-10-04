using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingPanel : MonoBehaviour
{

    [SerializeField] private GameObject[] _flares;


    public void ShowFinalFlares()
    {
        for (int i = 0; i < _flares.Length; i++)
        {
            _flares[i].SetActive(true);
        }
    }

}
