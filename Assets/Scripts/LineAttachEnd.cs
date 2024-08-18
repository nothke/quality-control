using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineAttachEnd : MonoBehaviour
{
    public Transform anchor;

    LineRenderer _line;
    LineRenderer line { get { if (!_line) _line = GetComponent<LineRenderer>(); return _line; } }

    private void Start()
    {
        line.useWorldSpace = true;
    }

    void Update()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, anchor.position);
    }
}
