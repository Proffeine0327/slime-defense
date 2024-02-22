using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    protected event Action onDisplay;
    protected event Action onHide;

    public virtual void Display()
    {
        onDisplay?.Invoke();
        gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }

    public virtual void Hide()
    {
        onHide?.Invoke();
        gameObject.SetActive(false);
    }
}