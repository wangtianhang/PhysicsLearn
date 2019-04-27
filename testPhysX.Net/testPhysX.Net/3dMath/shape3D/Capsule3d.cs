using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public struct Capsule3d
{
    public Vector3 m_point1;
    public Vector3 m_point2;
    public float m_radius;

    public Capsule3d(Vector3 point1, Vector3 point2, float radius)
    {
        m_point1 = point1;
        m_point2 = point2;
        m_radius = radius;
    }

    public Vector3 AnyPointFast()
    {
        return m_point1;
    }

    public Vector3 ExtremePoint(Vector3 direction)
    {
        float len = direction.magnitude;
        return (Vector3.Dot(direction, m_point2 -  m_point1) >= 0 ? m_point2 : m_point1)  + direction * (m_radius / len);
    }

    public Vector3 ExtremePoint(Vector3 direction, ref float projectionDistance)
    {
        direction.Normalize();
        Vector3 extremePoint = ExtremePoint(direction);
        projectionDistance = Vector3.Dot(extremePoint, direction);
        return extremePoint;
    }

    public bool ContainPoint(Vector3 point)
    {
        Segment3d segment = new Segment3d(m_point1, m_point2);
        Vector3 closestPoint = Distance3d.ClosestPointOfPoint3dWithSegment3d(point, segment);
        float distance = (closestPoint - point).magnitude;
        return distance <= m_radius;
    }
}

