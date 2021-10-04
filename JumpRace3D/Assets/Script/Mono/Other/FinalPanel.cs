using UnityEngine;
using UnityEngine.UI;
public class FinalPanel : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    [SerializeField] private Text _firstText;
    [SerializeField] private Text _secondText;
    [SerializeField] private Text _thirdText;
    [SerializeField] private Text _fourthText;




    public void Show(string first, string second, string third, string fourth)
    {
        _firstText.text = first;
        _secondText.text = second;
        _thirdText.text = third;
        _fourthText.text = fourth;
        _animator.Play(FinalScreenKeys.ShowFinal);
    }






}
