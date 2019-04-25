using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public struct AABB3d
{
    public AABB3d(Vector3 pos, Vector3 size)
    {
        m_pos = pos;
        m_size = size;
    }

    public Vector3 m_pos;
    public Vector3 m_size;
    //public float m_xLength;
    //public float m_yLength;
    //public float m_zLength;

    public Vector3 GetMin()
    {
        //return new Vector3(m_pos.x - m_size.x * 0.5f, m_pos.y - m_size.y * 0.5f, m_pos.z - m_size * 0.5f);
        return m_pos - GetHalfSize();
    }

    public Vector3 GetMax()
    {
        //return new Vector3(m_pos.x + m_size * 0.5f, m_pos.y + m_size * 0.5f, m_pos.z + m_size * 0.5f);
        return m_pos + GetHalfSize();
    }

    public Vector3 GetHalfSize()
    {
        return m_size / 2;
    }

    public Vector3 AnyPointFast()
    {
        return GetMin();
    }

    public Vector3 ExtremePoint(Vector3 direction)
    {
        Vector3 maxPoint = GetMax();
        Vector3 minPoint = GetMin();
        return new Vector3(
            direction.x >= 0 ? maxPoint.x : minPoint.x,
            direction.y >= 0 ? maxPoint.y : minPoint.y,
            direction.z >= 0 ? maxPoint.z : minPoint.z
            );
    }

    public Vector3 ExtremePoint(Vector3 direction, ref float projectionDistance)
    {
        direction.Normalize();
        Vector3 extremePoint = ExtremePoint(direction);
        projectionDistance = Vector3.Dot(extremePoint, direction);
        return extremePoint;
    }

    public AABB3d Merge(AABB3d other)
    {
        Vector3 selfMin = GetMin();
        Vector3 otherMin = other.GetMin();
        Vector3 selfMax = GetMax();
        Vector3 otherMax = GetMax();

        Vector3 totalMin = new Vector3(Math.Min(selfMin.x, otherMin.x), Math.Min(selfMin.y, otherMin.y), Math.Min(selfMin.z, otherMin.z));
        Vector3 totalMax = new Vector3(Math.Max(selfMax.x, otherMax.x), Math.Max(selfMax.y, otherMax.y), Math.Max(selfMax.z, otherMax.z));

        return new AABB3d((totalMin + totalMax) / 2, totalMax - totalMin);
    }

    public bool Contains(AABB3d other)
    {
        Vector3 selfMin = GetMin();
        Vector3 otherMin = other.GetMin();
        Vector3 selfMax = GetMax();
        Vector3 otherMax = GetMax();

        return otherMin.x >= selfMin.x &&
            otherMax.x <= selfMax.x &&
            otherMin.y >= selfMin.y &&
            otherMax.y <= selfMax.y &&
            otherMin.z >= selfMin.z &&
            otherMax.z <= selfMax.z;
    }

    public float CalculateSurfaceArea()
    {
        return 2 * (m_size.x * m_size.y +  m_size.x * m_size.z + m_size.y * m_size.z); 
    }

    public bool Overlaps(AABB3d other)
    {
        Vector3 selfMin = GetMin();
        Vector3 otherMin = other.GetMin();
        Vector3 selfMax = GetMax();
        Vector3 otherMax = GetMax();

        // y is deliberately first in the list of checks below as it is seen as more likely than things
        // collide on x,z but not on y than they do on y thus we drop out sooner on a y fail
        return selfMax.x > otherMin.x &&
            selfMin.x < otherMax.x &&
            selfMax.y > otherMin.y &&
            selfMin.y < otherMax.y &&
            selfMax.z > otherMin.z &&
            selfMin.z < otherMax.z;
    }
}

