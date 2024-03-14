using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class AlignStraight : MonoBehaviour
{
    public enum Direction { x, y, z }

    [SerializeField] private float distance;
    [SerializeField] private Direction direction;

    private void Update()
    {
        var amount = transform.childCount;
        var count = 0;
        foreach (Transform t in transform)
        {
            switch (direction)
            {
                case Direction.x:
                    t.localPosition = new Vector3(distance * count - (distance * (amount - 1) / 2), 0, 0);
                    break;
                case Direction.y:
                    t.localPosition = new Vector3(0, distance * count - (distance * (amount - 1) / 2), 0);
                    break;
                case Direction.z:
                    t.localPosition = new Vector3(0, 0, distance * count - (distance * (amount - 1) / 2));
                    break;
            }
            count++;
        }
    }
}
