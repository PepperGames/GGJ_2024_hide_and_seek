using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityBlockDisplay : MonoBehaviour
{
    [SerializeField] private Image _image;
    private Color _initialColor;
    [SerializeField] private Color _blockedColorr;
    //private float _initialAlpha;
    //[Range(0, 255)] [SerializeField] private int _blockedAlphaq = 120;

    public BaseAbility baseAbility;

    void Start()
    {
        //_initialAlpha = _image.color.a;
        _initialColor = _image.color;
        if (baseAbility != null)
        {
            baseAbility.OnBlockUse.AddListener(OnBlockUseHandler);
            baseAbility.OnUnblockUse.AddListener(OnUnblockUseHandler);
        }
    }
    private void OnBlockUseHandler()
    {
        _image.color = _blockedColorr;
    }

    private void OnUnblockUseHandler()
    {
        _image.color = _initialColor;
    }
}
