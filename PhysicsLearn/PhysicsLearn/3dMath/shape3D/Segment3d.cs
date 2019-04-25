using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public struct Segment3d
{
    public Segment3d(Vector3 point1, Vector3 point2)
    {
        m_point1 = point1;
        m_point2 = point2;
    }

    public Vector3 m_point1;
    public Vector3 m_point2;

    public float GetLength()
    {
        return (m_point2 - m_point1).magnitude;
    }

    public float GetLengthSqr()
    {
        return (m_point2 - m_point1).sqrMagnitude;
    }
}

