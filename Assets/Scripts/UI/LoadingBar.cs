using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LoadingBar : MonoBehaviour
    {
        [SerializeField] Image _fillImage;

        public void SetLoadingAmount(float amount)
        {
            _fillImage.fillAmount = amount;
        }
    }
}
