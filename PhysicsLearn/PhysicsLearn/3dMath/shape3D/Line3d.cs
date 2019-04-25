using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public struct Line3d
{
    public Line3d(Vector3 point1, Vector3 point2)
    {
        m_point1 = point1;
        m_point2 = point2;
    }
    public Vector3 m_point1;
    public Vector3 m_point2;
}

