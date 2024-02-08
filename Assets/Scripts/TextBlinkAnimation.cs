using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBlinkAnimation : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text;

    Color _textColor;

    float _alpha;
    bool _fadeIn;

    private void Start()
    {
        _textColor = _text.color;
        _alpha = _text.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        if (_fadeIn)
        {
            _alpha -= Time.deltaTime;
            if (_alpha < 0 )
            {
                _alpha = 0;
                _fadeIn = false;
            }
        }
        else
        {
            _alpha += Time.deltaTime;
            if (_alpha > 1f)
            {
                _alpha = 1f;
                _fadeIn = true;
            }
        }

        _textColor.a = _alpha;
        _text.color = _textColor;
    }
}
