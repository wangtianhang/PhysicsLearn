using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public struct Circle2d
{
    public Circle2d(Vector2 pos, float radius)
    {
        m_pos = pos;
        m_radius = radius;
    }
    public Vector2 m_pos;
    public float m_radius;
}

