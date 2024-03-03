using UnityEngine;

public class RangeDisplayer : MonoBehaviour
{
    private void Start()
    {
        transform.localPosition = Vector3.zero;
    }

    public void Active(bool active)
    {
        gameObject.SetActive(active);
    }

    public void SetRange(float range)
    {
        var pScale = transform.parent.lossyScale;
        transform.localScale = Vector3.one * range * 1 / pScale.x;
    }
}