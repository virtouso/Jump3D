using System.Collections;
using UnityEngine;

public class JumpingPanel : PathItemBase
{
    [SerializeField] public int PanelIndex;

    [SerializeField] public bool IsSupporter;
    [SerializeField] public bool IsSpecial;
    [SerializeField] public Transform Center;
    [SerializeField] private float _disappearDelayTime;
    [SerializeField] private float _wavingTime;

    [SerializeField] private MeshRenderer _panelMesh;

    [SerializeField] private float _waveSpeed;
    [SerializeField] private float _waveScele;
    [SerializeField] private GameObject _fireObject;
    [SerializeField] private float _fireTime;
    [SerializeField] private Transform _fracturedParent;
    [SerializeField] private float _destructionStepTime;
    [SerializeField] private float _destructionKeepCliveTime;
    [SerializeField] private BoxCollider _collider;
    public void ShowWave()
    {

        _panelMesh.material.SetFloat(MaterialKeys.XSpeed, _waveSpeed);
        _panelMesh.material.SetFloat(MaterialKeys.YSpeed, _waveSpeed);
        _panelMesh.material.SetFloat(MaterialKeys.Scale, _waveScele);



        StartCoroutine(EndWaving());
    }


    public void ShowFireUpEffect()
    {
        _fireObject.SetActive(true);
    }

    public void Disappear()
    {
        StartCoroutine(DisappearDelayed());
    }



    public void Destroy()
    {
        StartCoroutine(DestroyInSteps());
    }

    private IEnumerator DestroyInSteps()
    {
        _panelMesh.gameObject.SetActive(false);


        _fracturedParent.gameObject.SetActive(true);

        Rigidbody[] fractured = _fracturedParent.GetComponentsInChildren<Rigidbody>();

        for (int i = 0; i < fractured.Length; i++)
        {
            // yield return new WaitForSeconds(_destructionStepTime);
            fractured[i].isKinematic = false;
            fractured[i].AddTorque(Vector3.one * Random.Range(-10, 10), ForceMode.Impulse);
        }
        yield return new WaitForSeconds(_destructionKeepCliveTime);
        _fracturedParent.gameObject.SetActive(false);

    }




    private IEnumerator EndWaving()
    {
        yield return new WaitForSeconds(_wavingTime);

        _panelMesh.materials[0].SetFloat(MaterialKeys.XSpeed, 0);
        _panelMesh.materials[0].SetFloat(MaterialKeys.YSpeed, 0);
        _panelMesh.material.SetFloat(MaterialKeys.Scale, 0);

    }




    private IEnumerator Disapear()
    {
        yield return new WaitForSeconds(_fireTime);
        gameObject.SetActive(false);

    }





    private IEnumerator DisappearDelayed()
    {
        yield return new WaitForSeconds(_disappearDelayTime);
        gameObject.SetActive(false);
    }



}
