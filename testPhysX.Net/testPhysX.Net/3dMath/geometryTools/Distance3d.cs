using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

class Distance3d
{
    /// <summary>
    /// 3d空间中点到直线距离
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="s"></param>
    /// <returns></returns>
//     public static float DistanceOfPoint3dWithLine3d(Line3d line3d, Vector3 s)
//     {
//         float ab = FixPointMath.Sqrt(FixPointMath.Pow((line3d.m_point1.x - line3d.m_point2.x), 2.0f) + FixPointMath.Pow((line3d.m_point1.y - line3d.m_point2.y), 2.0f) + FixPointMath.Pow((line3d.m_point1.z - line3d.m_point2.z), 2.0f));
//         float as2 = FixPointMath.Sqrt(FixPointMath.Pow((line3d.m_point1.x - s.x), 2.0f) + FixPointMath.Pow((line3d.m_point1.y - s.y), 2.0f) + FixPointMath.Pow((line3d.m_point1.z - s.z), 2.0f));
//         float bs = FixPointMath.Sqrt(FixPointMath.Pow((s.x - line3d.m_point2.x), 2.0f) + FixPointMath.Pow((s.y - line3d.m_point2.y), 2.0f) + FixPointMath.Pow((s.z - line3d.m_point2.z), 2.0f));
//         float cos_A = (FixPointMath.Pow(as2, 2.0f) + FixPointMath.Pow(ab, 2.0f) - FixPointMath.Pow(bs, 2.0f)) / (2 * ab * as2);
//         float sin_A = FixPointMath.Sqrt(1 - FixPointMath.Pow(cos_A, 2.0f));
//         return as2 * sin_A;
//     }
    public static Vector3 ClosestPointOfPoint3dWithSegment3d(Vector3 point, Segment3d line)
    {
        Vector3 lVec = line.m_point2 - line.m_point1; // Line Vector
        float t = Vector3.Dot(point - line.m_point1, lVec) / Vector3.Dot(lVec, lVec);
        t = Math.Max(t, 0.0f); // Clamp to 0
        t = Math.Min(t, 1.0f); // Clamp to 1
        return line.m_point1 + lVec * t;
    }

    public static Vector3 ClosestPointOfPoint3dWithLine3d(Vector3 point, Line3d line)
    {
        Vector3 lVec = line.m_point2 - line.m_point1; // Line Vector
        float t = Vector3.Dot(point - line.m_point1, lVec) / Vector3.Dot(lVec, lVec);
        return line.m_point1 + lVec * t;
    }

    public static Vector3 ClosestPointOfPoint3dWithAABB3d(Vector3 point, AABB3d aabb)
    {
        Vector3 result = point;
        Vector3 min = aabb.GetMin();
        Vector3 max = aabb.GetMax();
        result.x = (result.x < min.x) ? min.x : result.x;
        result.y = (result.y < min.y) ? min.y : result.y;
        result.z = (result.z < min.z) ? min.z : result.z;
        result.x = (result.x > max.x) ? max.x : result.x;
        result.y = (result.y > max.y) ? max.y : result.y;
        result.z = (result.z > max.z) ? max.z : result.z;
        return result;
    }

    public static Vector3 ClosestPointOfPoint3dWithOBB3d(Vector3 point, OBB3d obb)
    {
        //Vector3 objMin = obb.GetAABBMin();
        //Vector3 objMax = obb.GetAABBMax();
        Matrix4x4 obj2World = obb.GetObjToWorld();
        Matrix4x4 worldToObj = obj2World.inverse;
        Vector3 objMin = (Vector3)(worldToObj * obb.m_pos) - obb.GetHalfSize();
        Vector3 objMax = (Vector3)(worldToObj * obb.m_pos) + obb.GetHalfSize();
        Vector3 objPoint = worldToObj * point;
        
        Vector3 objResult = objPoint;
        objResult.x = (objResult.x < objMin.x) ? objMin.x : objResult.x;
        objResult.y = (objResult.y < objMin.y) ? objMin.y : objResult.y;
        objResult.z = (objResult.z < objMin.z) ? objMin.z : objResult.z;
        objResult.x = (objResult.x > objMax.x) ? objMax.x : objResult.x;
        objResult.y = (objResult.y > objMax.y) ? objMax.y : objResult.y;
        objResult.z = (objResult.z > objMax.z) ? objMax.z : objResult.z;

        Vector3 worldResult = obj2World * objResult;
        return worldResult;
    }

    public static Vector3 ClosestPointOfPoint3dWithSphere(Vector3 point, Sphere3d sphere)
    {
        if((sphere.m_pos - point).magnitude > sphere.m_radius)
        {
            return sphere.m_pos + (point - sphere.m_pos).normalized * sphere.m_radius;
        }
        else
        {
            return point;
        }
    }

    public static Vector3 ClosestPointOfPoint3dWithPlane3d(Vector3 point, Plane3d plane)
    {
        float dot = Vector3.Dot(plane.m_planeNormal, point);
        float distance = dot - plane.GetDistanceFromOrigin();
        return point - plane.m_planeNormal * distance;
    }

    public static Vector3 ClosestPointOfPoint3dWithRay3d(Vector3 point, Ray3d ray)
    {
        float t = Vector3.Dot(point - ray.m_rayOrigin, ray.m_rayDir);
        // We assume the direction of the ray is normalized
        // If for some reason the direction is not normalized
        // the below division is needed. So long as the ray
        // direction is normalized, we don't need this divide
        // t /= Dot(ray.direction, ray.direction);
        t = Math.Max(t, 0.0f);
        return ray.m_rayOrigin + ray.m_rayDir * t;
    }

    public static Plane3d FromTriangle(Triangle3d t)
    {
        Plane3d result = new Plane3d(Vector3.Cross(t.m_point1 - t.m_point0, t.m_point2 - t.m_point0), t.m_point0);
        return result;
    }

    public static Vector3 ClosestPointOfPoint3dWithTriangle3d(Triangle3d t, Vector3 p) 
    {
	    Plane3d plane = FromTriangle(t);
        Vector3 closest = ClosestPointOfPoint3dWithPlane3d(p, plane);

	    // Closest point was inside triangle
        if (IntersectionTest3D.Point3dWithTriangle(closest, t))
        {
		    return closest;
	    }

        Vector3 c1 = ClosestPointOfPoint3dWithSegment3d(closest, new Segment3d(t.m_point0, t.m_point1)); // Line AB
        Vector3 c2 = ClosestPointOfPoint3dWithSegment3d(closest, new Segment3d(t.m_point1, t.m_point2)); // Line BC
        Vector3 c3 = ClosestPointOfPoint3dWithSegment3d(closest, new Segment3d(t.m_point2, t.m_point0)); // Line CA

	    float magSq1 = (closest - c1).sqrMagnitude;
        float magSq2 = (closest - c2).sqrMagnitude;
        float magSq3 = (closest - c3).sqrMagnitude;

	    if (magSq1 < magSq2 && magSq1 < magSq3) {
		    return c1;
	    }
	    else if (magSq2 < magSq1 && magSq2 < magSq3) {
		    return c2;
	    }

	    return c3;
    }
}


