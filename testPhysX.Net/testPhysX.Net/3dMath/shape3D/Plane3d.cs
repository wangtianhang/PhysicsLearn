using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public struct Plane3d
{
    public Plane3d(Vector3 normal, Vector3 point)
    {
        m_planeNormal = normal;
        m_planeNormal.Normalize();
        m_planeOnePoint = point;
    }
    public Vector3 m_planeNormal;
    public Vector3 m_planeOnePoint;

    public float GetDistanceFromOrigin()
    {
        return Vector3.Dot(m_planeOnePoint, m_planeNormal);
    }

    public static float PlaneEquation(Vector3 point, Plane3d plane)
    {
        return Vector3.Dot(point, plane.m_planeNormal) - plane.GetDistanceFromOrigin();
    }
}

