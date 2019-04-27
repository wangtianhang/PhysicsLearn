using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

struct AARectangle
{
    public AARectangle(Vector2 pos, float xWidth, float zLength)
    {
        m_pos = pos;
        m_width = xWidth;
        m_length = zLength;
    }
    public Vector2 m_pos;
    public float m_width; // 对应x轴
    public float m_length; // 对应z轴
}

