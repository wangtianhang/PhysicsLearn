using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// 预计实现point3d ray3d line3d plane3d sphere3d aabb3d obb3d triangle3d mesh3d 
/// Capsule3d(胶囊 delay) cylinder(圆柱 delay) cone3d(圆锥 delay) frustum3d(视锥 平截头体 delay) polyhedron3d(多面体 delay)
/// </summary>
public class IntersectionTest3D
{
    public static void Test()
    {
        Plane3d plane = new Plane3d(new Vector3(1, 1, 1), new Vector3(10, 10, 10));
        Vector3 closestPoint = Distance3d.ClosestPointOfPoint3dWithPlane3d(new Vector3(20, 20, 20), plane);
        Debug.Log("最近点 " + closestPoint);
        closestPoint = Distance3d.ClosestPointOfPoint3dWithPlane3d(new Vector3(-10, -10, -10), plane);
        Debug.Log("最近点 " + closestPoint);

        OBB3d obb1 = new OBB3d(new Vector3(-5, 0, 0), Quaternion.Euler(Vector3.zero), 9, 20, 10);
        OBB3d obb2 = new OBB3d(new Vector3(5, 0, 0), Quaternion.Euler(Vector3.zero), 9, 20, 10);
        Debug.Log("obb相交测试1 " + OBB3dWithOBB3d(obb1, obb2));
        OBB3d obb3 = new OBB3d(new Vector3(5, 0, 0), Quaternion.Euler(new Vector3(0, 0, 90)), 9, 20, 10);
        Debug.Log("obb相交测试2 " + OBB3dWithOBB3d(obb1, obb3));
    }

    /// <summary>
    /// 射线与平面相交
    /// </summary>
    /// <param name="rayOrigin"></param>
    /// <param name="rayDir"></param>
    /// <param name="planeNormal"></param>
    /// <param name="planeOnePoint"></param>
    /// <returns></returns>
//     public static bool Ray3dWithPlane3d(Ray3d ray, Plane3d plane, out Vector3 result)
//     {
//         float t = (Vector3.Dot(plane.m_planeNormal, plane.m_planeOnePoint) - Vector3.Dot(plane.m_planeNormal, ray.m_rayOrigin))
//             / (Vector3.Dot(plane.m_planeNormal, ray.m_rayDir));
// 
//         if (t < 0)
//         {
//             result = Vector3.zero;
//             return false;
//         }
//         else
//         {
//             result = ray.m_rayOrigin + ray.m_rayDir * t;
//             return true;
//         }
// 
//     }

    /// <summary>
    /// 来自于微软pick.cpp demo
    /// </summary>
    /// <param name="ray3d"></param>
    /// <param name="triangle3d"></param>
    /// <param name="t"></param>
    /// <param name="u"></param>
    /// <param name="v"></param>
    /// <returns></returns>
//     public static bool Ray3dWithTriangle3d(Ray3d ray3d,
//                     Triangle3d triangle3d,
//                     ref float t, ref float u, ref float v)
//     {
//         // Find vectors for two edges sharing vert0
//         Vector3 edge1 = triangle3d.m_point1 - triangle3d.m_point0;
//         Vector3 edge2 = triangle3d.m_point2 - triangle3d.m_point0;
// 
//         // Begin calculating determinant - also used to calculate U parameter
//         Vector3 pvec;
//         pvec = Vector3.Cross(ray3d.m_rayDir, edge2);
// 
//         // If determinant is near zero, ray lies in plane of triangle
//         float det = Vector3.Dot(edge1, pvec);
// 
//         Vector3 tvec;
//         if (det > 0)
//         {
//             tvec = ray3d.m_rayOrigin - triangle3d.m_point0;
//         }
//         else
//         {
//             tvec = triangle3d.m_point0 - ray3d.m_rayOrigin;
//             det = -det;
//         }
// 
//         if (det < 0.0001f)
//             return false;
// 
//         // Calculate U parameter and test bounds
//         u = Vector3.Dot(tvec, pvec);
//         if (u < 0.0f || u > det)
//             return false;
// 
//         // Prepare to test V parameter
//         Vector3 qvec;
//         qvec = Vector3.Cross(tvec, edge1);
// 
//         // Calculate V parameter and test bounds
//         v = Vector3.Dot(ray3d.m_rayDir, qvec);
//         if (v < 0.0f || u + v > det)
//             return false;
// 
//         // Calculate t, scale parameters, ray intersects triangle
//         t = Vector3.Dot(edge2, qvec);
//         float fInvDet = 1.0f / det;
//         t *= fInvDet;
//         u *= fInvDet;
//         v *= fInvDet;
// 
//         return true;
//     }

    public static bool Point3dWithOBB3d(Vector3 point, OBB3d obb)
    {
        //Vector3 objMin = obb.GetAABBMin();
        //Vector3 objMax = obb.GetAABBMax();
        Matrix4x4 obj2World = obb.GetObjToWorld();
        Matrix4x4 worldToObj = obj2World.inverse;
        Vector3 objMin = (Vector3)(worldToObj * obb.m_pos) - obb.GetHalfSize();
        Vector3 objMax = (Vector3)(worldToObj * obb.m_pos) + obb.GetHalfSize();
        Vector3 objPoint = worldToObj * point;
        if (objPoint.x < objMin.x || objPoint.y < objMin.y || objPoint.z < objMin.z)
        {
            return false;
        }
        if (objPoint.x > objMax.x || objPoint.y > objMax.y || objPoint.z > objMax.z)
        {
            return false;
        }
        return true;
    }

    public static bool Point3dWithPlane3d(Vector3 point, Plane3d plane)
    {
        // 根据3d平面定义
        float dot = Vector3.Dot(plane.m_planeOnePoint - point, plane.m_planeNormal);
        return Mathf.Approximately(dot, 0);
    }

    public static bool Point3dWithLine3d(Vector3 point, Line3d line)
    {
        Vector3 closestPoint = Distance3d.ClosestPointOfPoint3dWithLine3d(point, line);
        float distanceSq = (closestPoint - point).sqrMagnitude;
        return Mathf.Approximately(distanceSq, 0);
    }

    public static bool Point3dWithSegment3d(Vector3 point, Segment3d segment)
    {
        Vector3 closestPoint = Distance3d.ClosestPointOfPoint3dWithSegment3d(point, segment);
        float distanceSq = (closestPoint - point).sqrMagnitude;
        return Mathf.Approximately(distanceSq, 0);
    }

    public static bool Point3dWithRay3d(Vector3 point, Ray3d ray)
    {
        if(point == ray.m_rayOrigin)
        {
            return true;
        }
        Vector3 norm = point - ray.m_rayOrigin;
        norm.Normalize();
        float diff = Vector3.Dot(norm, ray.m_rayDir);
        return Mathf.Approximately(diff, 1);
    }

    public static bool Sphere3dWithAABB3d(Sphere3d sphere, AABB3d aabb)
    {
        Vector3 closestPoint = Distance3d.ClosestPointOfPoint3dWithAABB3d(sphere.m_pos, aabb);
        return (sphere.m_pos - closestPoint).magnitude <= sphere.m_radius;
    }

    public static bool Sphere3dWithObb3d(Sphere3d sphere, OBB3d obb)
    {
        Vector3 closestPoint = Distance3d.ClosestPointOfPoint3dWithOBB3d(sphere.m_pos, obb);
        return (sphere.m_pos - closestPoint).magnitude <= sphere.m_radius;
    }

    public static bool Sphere3dWithPlane3d(Sphere3d sphere, Plane3d plane)
    {
        Vector3 closestPoint = Distance3d.ClosestPointOfPoint3dWithPlane3d(sphere.m_pos, plane);
        return (sphere.m_pos - closestPoint).magnitude <= sphere.m_radius;
    }

    public static bool AABB3dWithAABB3d(AABB3d aabb1, AABB3d aabb2)
    {
        Vector3 aMin = aabb1.GetMin();
        Vector3 aMax = aabb1.GetMax();

        Vector3 bMin = aabb2.GetMin();
        Vector3 bMax = aabb2.GetMax();

        return (aMin.x <= bMax.x && aMax.x >= bMin.x) &&
            (aMin.y <= bMax.y && aMax.y >= bMin.y) &&
            (aMin.z <= bMax.z && aMax.z >= bMin.z);
    }



    #region Separating Axis Theorem
    /// <summary>
    /// 来自game physics cookbook
    /// https://github.com/gszauer/GamePhysicsCookbook
    /// </summary>
    struct Interval
    {
        public float min;
        public float max;
    }

    static Interval GetInterval(OBB3d obb, Vector3 axis)
    {
        Vector3[] vertex = new Vector3[8];

	    Vector3 C = obb.m_pos;	// OBB Center
	    Vector3 E = obb.GetHalfSize();		// OBB Extents
	    float[] o = obb.GetOrientationMatrixAsArray();
	    Vector3[] A = {			// OBB Axis
		    new Vector3(o[0], o[1], o[2]),
		    new Vector3(o[3], o[4], o[5]),
		    new Vector3(o[6], o[7], o[8]),
	    };

	    vertex[0] = C + A[0] * E[0] + A[1] * E[1] + A[2] * E[2];
	    vertex[1] = C - A[0] * E[0] + A[1] * E[1] + A[2] * E[2];
	    vertex[2] = C + A[0] * E[0] - A[1] * E[1] + A[2] * E[2];
	    vertex[3] = C + A[0] * E[0] + A[1] * E[1] - A[2] * E[2];
	    vertex[4] = C - A[0] * E[0] - A[1] * E[1] - A[2] * E[2];
	    vertex[5] = C + A[0] * E[0] - A[1] * E[1] - A[2] * E[2];
	    vertex[6] = C - A[0] * E[0] + A[1] * E[1] - A[2] * E[2];
	    vertex[7] = C - A[0] * E[0] - A[1] * E[1] + A[2] * E[2];

	    Interval result = new Interval();
	    result.min = result.max = Vector3.Dot(axis, vertex[0]);

	    for (int i = 1; i < 8; ++i) {
            float projection = Vector3.Dot(axis, vertex[i]);
		    result.min = (projection < result.min) ? projection : result.min;
		    result.max = (projection > result.max) ? projection : result.max;
	    }

	    return result;
    }

    static Interval GetInterval(Triangle3d triangle, Vector3 axis) 
    {
	    Interval result = new Interval();

        result.min = Vector3.Dot(axis, triangle.m_point0);
	    result.max = result.min;
	    for (int i = 1; i < 3; ++i) {
            float value = Vector3.Dot(axis, triangle.GetPoint(i));
		    result.min = Math.Min(result.min, value);
		    result.max = Math.Max(result.max, value);
	    }

	    return result;
    }

    static Interval GetInterval(AABB3d aabb, Vector3 axis) 
    {
	    Vector3 i = aabb.GetMin();
	    Vector3 a = aabb.GetMax();

	    Vector3[] vertex = {
		    new Vector3(i.x, a.y, a.z),
		    new Vector3(i.x, a.y, i.z),
		    new Vector3(i.x, i.y, a.z),
		    new Vector3(i.x, i.y, i.z),
		    new Vector3(a.x, a.y, a.z),
		    new Vector3(a.x, a.y, i.z),
		    new Vector3(a.x, i.y, a.z),
		    new Vector3(a.x, i.y, i.z)
	    };

	    Interval result = new Interval();
        result.min = result.max = Vector3.Dot(axis, vertex[0]);

	    for (int j = 1; j < 8; ++j) {
            float projection = Vector3.Dot(axis, vertex[j]);
		    result.min = (projection < result.min) ? projection : result.min;
		    result.max = (projection > result.max) ? projection : result.max;
	    }

	    return result;
    }

    static bool OverlapOnAxis(OBB3d obb1, OBB3d obb2, Vector3 axis)
    {
        Interval a = GetInterval(obb1, axis);
        Interval b = GetInterval(obb2, axis);
        return ((b.min <= a.max) && (a.min <= b.max));
    }

    static bool OverlapOnAxis(OBB3d obb, Triangle3d triangle, Vector3 axis) 
    {
        Interval a = GetInterval(obb, axis);
	    Interval b = GetInterval(triangle, axis);
	    return ((b.min <= a.max) && (a.min <= b.max));
    }

    static bool OverlapOnAxis(Triangle3d t1, Triangle3d t2, Vector3 axis) 
    {
	    Interval a = GetInterval(t1, axis);
	    Interval b = GetInterval(t2, axis);
	    return ((b.min <= a.max) && (a.min <= b.max));
    }

    static bool OverlapOnAxis(AABB3d aabb, Triangle3d triangle, Vector3 axis) 
    {
	    Interval a = GetInterval(aabb, axis);
	    Interval b = GetInterval(triangle, axis);
	    return ((b.min <= a.max) && (a.min <= b.max));
    }

    static bool OverlapOnAxis(AABB3d aabb, OBB3d obb, Vector3 axis) 
    {
	    Interval a = GetInterval(aabb, axis);
	    Interval b = GetInterval(obb, axis);
	    return ((b.min <= a.max) && (a.min <= b.max));
    }

    public static bool OBB3dWithOBB3d(OBB3d obb1, OBB3d obb2) {
	    float[] o1 = obb1.GetOrientationMatrixAsArray();
	    float[] o2 = obb2.GetOrientationMatrixAsArray();

	    Vector3[] test2 = {
		    new Vector3(o1[0], o1[1], o1[2]),
		    new Vector3(o1[3], o1[4], o1[5]),
		    new Vector3(o1[6], o1[7], o1[8]),
		    new Vector3(o2[0], o2[1], o2[2]),
		    new Vector3(o2[3], o2[4], o2[5]),
		    new Vector3(o2[6], o2[7], o2[8])
	    };

        Vector3[] test = new Vector3[15];
        Array.Copy(test2, test, test2.Length);

	    for (int i = 0; i < 3; ++i) { // Fill out rest of axis
            test[6 + i * 3 + 0] = Vector3.Cross(test[i], test[0]);
            test[6 + i * 3 + 1] = Vector3.Cross(test[i], test[1]);
            test[6 + i * 3 + 2] = Vector3.Cross(test[i], test[2]);
	    }

	    for (int i = 0; i < 15; ++i) {
		    if (!OverlapOnAxis(obb1, obb2, test[i])) {
			    return false; // Seperating axis found
		    }
	    }

	    return true; // Seperating axis not found
    }

    public static bool Triangle3dWithOBB3d(Triangle3d t, OBB3d o)
    {
        // Compute the edge vectors of the triangle  (ABC)
	    Vector3 f0 = t.m_point1 - t.m_point0;
	    Vector3 f1 = t.m_point2 - t.m_point1;
	    Vector3 f2 = t.m_point0 - t.m_point2;

	    // Compute the face normals of the AABB
	    float[] orientation = o.GetOrientationMatrixAsArray();
	    Vector3 u0 = new Vector3(orientation[0], orientation[1], orientation[2]);
	    Vector3 u1 = new Vector3(orientation[3], orientation[4], orientation[5]);
	    Vector3 u2 = new Vector3(orientation[6], orientation[7], orientation[8]);

	    Vector3[] test = {
		    // 3 Normals of AABB
		    u0, // AABB Axis 1
		    u1, // AABB Axis 2
		    u2, // AABB Axis 3
		    // 1 Normal of the Triangle
		    Vector3.Cross(f0, f1),
		    // 9 Axis, cross products of all edges
		    Vector3.Cross(u0, f0),
		    Vector3.Cross(u0, f1),
		    Vector3.Cross(u0, f2),
		    Vector3.Cross(u1, f0),
		    Vector3.Cross(u1, f1),
		    Vector3.Cross(u1, f2),
		    Vector3.Cross(u2, f0),
		    Vector3.Cross(u2, f1),
		    Vector3.Cross(u2, f2)
	    };

	    for (int i = 0; i < 13; ++i) {
		    if (!OverlapOnAxis(o, t, test[i])) {
			    return false; // Seperating axis found
		    }
	    }

	    return true; // Seperating axis not found
    }

    public static bool Triangle3dWithAABB3d(Triangle3d t, AABB3d a)
    {
        // Compute the edge vectors of the triangle  (ABC)
	    Vector3 f0 = t.m_point1 - t.m_point0; 
	    Vector3 f1 = t.m_point2 - t.m_point1; 
	    Vector3 f2 = t.m_point0 - t.m_point2; 

	    // Compute the face normals of the AABB
	    Vector3 u0 = new Vector3(1.0f, 0.0f, 0.0f);
	    Vector3 u1 = new Vector3(0.0f, 1.0f, 0.0f);
	    Vector3 u2 = new Vector3(0.0f, 0.0f, 1.0f);

	    Vector3[] test = {
		    // 3 Normals of AABB
		    u0, // AABB Axis 1
		    u1, // AABB Axis 2
		    u2, // AABB Axis 3
		    // 1 Normal of the Triangle
		    Vector3.Cross(f0, f1),
		    // 9 Axis, cross products of all edges
		    Vector3.Cross(u0, f0),
		    Vector3.Cross(u0, f1),
		    Vector3.Cross(u0, f2),
		    Vector3.Cross(u1, f0),
		    Vector3.Cross(u1, f1),
		    Vector3.Cross(u1, f2),
		    Vector3.Cross(u2, f0),
		    Vector3.Cross(u2, f1),
		    Vector3.Cross(u2, f2)
	    };

	    for (int i = 0; i < 13; ++i) {
		    if (!OverlapOnAxis(a, t, test[i])) {
			    return false; // Seperating axis found
		    }
	    }

	    return true; // Seperating axis not found
    }

    public static bool AABB3dWithOBB3d(AABB3d aabb, OBB3d obb)
    {
        float[] o = obb.GetOrientationMatrixAsArray();

	    Vector3[] test2 = {
		    new Vector3(1, 0, 0), // AABB axis 1
		    new Vector3(0, 1, 0), // AABB axis 2
		    new Vector3(0, 0, 1), // AABB axis 3
		    new Vector3(o[0], o[1], o[2]),
		    new Vector3(o[3], o[4], o[5]),
		    new Vector3(o[6], o[7], o[8])
	    };

        Vector3[] test = new Vector3[15];
        Array.Copy(test2, test, test2.Length);

	    for (int i = 0; i < 3; ++i) { // Fill out rest of axis
            test[6 + i * 3 + 0] = Vector3.Cross(test[i], test[0]);
            test[6 + i * 3 + 1] = Vector3.Cross(test[i], test[1]);
            test[6 + i * 3 + 2] = Vector3.Cross(test[i], test[2]);
	    }

	    for (int i = 0; i < 15; ++i) {
		    if (!OverlapOnAxis(aabb, obb, test[i])) {
			    return false; // Seperating axis found
		    }
	    }

	    return true; // Seperating axis not found
    }
    #endregion

    public static bool Obb3dWithPlane3d(OBB3d obb, Plane3d plane)
    {
        // Local variables for readability only
	    float[] o = obb.GetOrientationMatrixAsArray();
	    Vector3[] rot = { // rotation / orientation
		    new Vector3(o[0], o[1], o[2]),
		    new Vector3(o[3], o[4], o[5]),
		    new Vector3(o[6], o[7], o[8]),
	    };
        Vector3 normal = plane.m_planeNormal;

	    // Project the half extents of the AABB onto the plane normal
        float pLen = obb.GetHalfSize().x * Math.Abs(Vector3.Dot(normal, rot[0])) +
                    obb.GetHalfSize().y * Math.Abs(Vector3.Dot(normal, rot[1])) +
                    obb.GetHalfSize().z * Math.Abs(Vector3.Dot(normal, rot[2]));
	    // Find the distance from the center of the OBB to the plane
	    float dist = Vector3.Dot(plane.m_planeNormal, obb.m_pos) - plane.GetDistanceFromOrigin();
	    // Intersection occurs if the distance falls within the projected side
	    return Math.Abs(dist) <= pLen;
    }

    public static bool Plane3dWithPlane3d(Plane3d plane1, Plane3d plane2)
    {
        Vector3 d = Vector3.Cross(plane1.m_planeNormal, plane2.m_planeNormal);
        return Mathf.Approximately(Vector3.Dot(d, d), 0);
    }

    public struct RaycastResult
    {
        public float m_t;
        public bool m_hit;
        public Vector3 m_point;
        public Vector3 m_normal;

        public void Reset()
        {
            m_t = -1;
            m_hit = false;
            m_point = Vector3.zero;
            m_normal = Vector3.forward;
        }
    }

    public static bool Ray3dWithSphere3d(Ray3d ray, Sphere3d sphere, ref RaycastResult result)
    {
        result.Reset();

        Vector3 e = sphere.m_pos - ray.m_rayOrigin;
        float rSq = sphere.m_radius * sphere.m_radius;

        float eSq = e.sqrMagnitude;
        float a = Vector3.Dot(e, ray.m_rayDir); // ray.direction is assumed to be normalized
        float bSq = /*sqrtf(*/eSq - (a * a)/*)*/;
        float f = Mathf.Sqrt(Math.Abs((rSq) - /*(b * b)*/bSq));

        // Assume normal intersection!
        float t = a - f;

        // No collision has happened
        if (rSq - (eSq - a * a) < 0.0f)
        {
            return false;
        }
        // Ray starts inside the sphere
        else if (eSq < rSq)
        {
            // Just reverse direction
            t = a + f;
        }

        //if (result != null)
        {
            result.m_t = t;
            result.m_hit = true;
            result.m_point = ray.m_rayOrigin + ray.m_rayDir * t;
            result.m_normal = (result.m_point - sphere.m_pos).normalized;
        }
        return true;
    }

    public static bool Ray3dWithAABB3d(AABB3d aabb, Ray3d ray, ref RaycastResult outResult) 
    {
	    //ResetRaycastResult(outResult);
        outResult.Reset();

	    Vector3 min = aabb.GetMin();
	    Vector3 max = aabb.GetMax();

	    // Any component of direction could be 0!
	    // Address this by using a small number, close to
	    // 0 in case any of directions components are 0
	    float t1 = (min.x - ray.m_rayOrigin.x) / (Mathf.Approximately(ray.m_rayDir.x, 0.0f) ? float.Epsilon : ray.m_rayDir.x);
	    float t2 = (max.x - ray.m_rayOrigin.x) / (Mathf.Approximately(ray.m_rayDir.x, 0.0f) ? float.Epsilon : ray.m_rayDir.x);
	    float t3 = (min.y - ray.m_rayOrigin.y) / (Mathf.Approximately(ray.m_rayDir.y, 0.0f) ? float.Epsilon : ray.m_rayDir.y);
	    float t4 = (max.y - ray.m_rayOrigin.y) / (Mathf.Approximately(ray.m_rayDir.y, 0.0f) ? float.Epsilon : ray.m_rayDir.y);
	    float t5 = (min.z - ray.m_rayOrigin.z) / (Mathf.Approximately(ray.m_rayDir.z, 0.0f) ? float.Epsilon : ray.m_rayDir.z);
	    float t6 = (max.z - ray.m_rayOrigin.z) / (Mathf.Approximately(ray.m_rayDir.z, 0.0f) ? float.Epsilon : ray.m_rayDir.z);

	    float tmin = Math.Max(Math.Max(Math.Min(t1, t2), Math.Min(t3, t4)), Math.Min(t5, t6));
	    float tmax = Math.Min(Math.Min(Math.Max(t1, t2), Math.Max(t3, t4)), Math.Max(t5, t6));

	    // if tmax < 0, ray is intersecting AABB
	    // but entire AABB is behing it's origin
	    if (tmax < 0) {
		    return false;
	    }

	    // if tmin > tmax, ray doesn't intersect AABB
	    if (tmin > tmax) {
		    return false;
	    }

	    float t_result = tmin;

	    // If tmin is < 0, tmax is closer
	    if (tmin < 0.0f) {
		    t_result = tmax;
	    }

	    //if (outResult != null)
        {
		    outResult.m_t = t_result;
		    outResult.m_hit = true;
		    outResult.m_point = ray.m_rayOrigin + ray.m_rayDir * t_result;

		    Vector3[] normals = {
			    new Vector3(-1, 0, 0),
			    new Vector3(1, 0, 0),
			    new Vector3(0, -1, 0),
			    new Vector3(0, 1, 0),
			    new Vector3(0, 0, -1),
			    new Vector3(0, 0, 1)
		    };
		    float[] t = { t1, t2, t3, t4, t5, t6 };

		    for (int i = 0; i < 6; ++i) {
			    if (Mathf.Approximately(t_result, t[i])) {
				    outResult.m_normal = normals[i];
			    }
		    }
	    }

	    return true;
    }

    static float[] g_t = new float[6];
    static Vector3[] g_normals = new Vector3[6];
    public static bool Ray3dWithOBB3d(OBB3d obb, Ray3d ray, ref RaycastResult outResult) 
    {
	    //ResetRaycastResult(outResult);
        outResult.Reset();

        //float[] o = obb.GetOrientationMatrixAsArray();
        Matrix4x4 objToWorld = obb.GetObjToWorld();
        //float[] size = obb.GetHalfSizeAsArray();
        Vector3 size = obb.GetHalfSize();

	    Vector3 p = obb.m_pos - ray.m_rayOrigin;

	    Vector3 X = new Vector3(objToWorld.m00, objToWorld.m01, objToWorld.m02);
	    Vector3 Y = new Vector3(objToWorld.m10, objToWorld.m11, objToWorld.m12);
	    Vector3 Z = new Vector3(objToWorld.m20, objToWorld.m21, objToWorld.m22);

	    Vector3 f = new Vector3(
		    Vector3.Dot(X, ray.m_rayDir),
		    Vector3.Dot(Y, ray.m_rayDir),
		    Vector3.Dot(Z, ray.m_rayDir)
	    );

	    Vector3 e = new Vector3(
		    Vector3.Dot(X, p),
		    Vector3.Dot(Y, p),
		    Vector3.Dot(Z, p)
	    );

    #if true
	    //float[] t = new float[]{ 0, 0, 0, 0, 0, 0 };
        for(int i = 0; i < g_t.Length; ++i)
        {
            g_t[i] = 0;
        }
	    for (int i = 0; i < 3; ++i) {
		    if (Mathf.Approximately(f[i], 0)) {
			    if (-e[i] - size[i] > 0 || -e[i] + size[i] < 0) {
				    return false;
			    }
			    f[i] = float.Epsilon; // Avoid div by 0!
		    }

            g_t[i * 2 + 0] = (e[i] + size[i]) / f[i]; // tmin[x, y, z]
            g_t[i * 2 + 1] = (e[i] - size[i]) / f[i]; // tmax[x, y, z]
	    }

	    float tmin = Math.Max(Math.Max(Math.Min(g_t[0], g_t[1]), Math.Min(g_t[2], g_t[3])), Math.Min(g_t[4], g_t[5]));
	    float tmax = Math.Min(Math.Min(Math.Max(g_t[0], g_t[1]), Math.Max(g_t[2], g_t[3])), Math.Max(g_t[4], g_t[5]));
    #else 
	    // The above loop simplifies the below if statements
	    // this is done to make sure the sample fits into the book
	    if (CMP(f.x, 0)) {
		    if (-e.x - obb.size.x > 0 || -e.x + obb.size.x < 0) {
			    return -1;
		    }
		    f.x = 0.00001f; // Avoid div by 0!
	    }
	    else if (CMP(f.y, 0)) {
		    if (-e.y - obb.size.y > 0 || -e.y + obb.size.y < 0) {
			    return -1;
		    }
		    f.y = 0.00001f; // Avoid div by 0!
	    }
	    else if (CMP(f.z, 0)) {
		    if (-e.z - obb.size.z > 0 || -e.z + obb.size.z < 0) {
			    return -1;
		    }
		    f.z = 0.00001f; // Avoid div by 0!
	    }

	    float t1 = (e.x + obb.size.x) / f.x;
	    float t2 = (e.x - obb.size.x) / f.x;
	    float t3 = (e.y + obb.size.y) / f.y;
	    float t4 = (e.y - obb.size.y) / f.y;
	    float t5 = (e.z + obb.size.z) / f.z;
	    float t6 = (e.z - obb.size.z) / f.z;

	    float tmin = fmaxf(fmaxf(fminf(t1, t2), fminf(t3, t4)), fminf(t5, t6));
	    float tmax = fminf(fminf(fmaxf(t1, t2), fmaxf(t3, t4)), fmaxf(t5, t6));
    #endif

	    // if tmax < 0, ray is intersecting AABB
	    // but entire AABB is behing it's origin
	    if (tmax < 0) {
		    return false;
	    }

	    // if tmin > tmax, ray doesn't intersect AABB
	    if (tmin > tmax) {
		    return false;
	    }

	    // If tmin is < 0, tmax is closer
	    float t_result = tmin;

	    if (tmin < 0.0f) {
		    t_result = tmax;
	    }

	    //if (outResult != null)
        {
		    outResult.m_hit = true;
		    outResult.m_t = t_result;
		    outResult.m_point = ray.m_rayOrigin + ray.m_rayDir * t_result;

            // 		    Vector3[] normals = {
            // 			    X,			// +x
            // 			    X * -1.0f,	// -x
            // 			    Y,			// +y
            // 			    Y * -1.0f,	// -y
            // 			    Z,			// +z
            // 			    Z * -1.0f	// -z
            // 		    };
            g_normals[0] = X;
            g_normals[1] = X * -1.0f;
            g_normals[2] = Y;
            g_normals[3] = Y * -1.0f;
            g_normals[4] = Z;
            g_normals[5] = Z * -1.0f;


            for (int i = 0; i < 6; ++i) {
			    if (Mathf.Approximately(t_result, g_t[i])) {
				    outResult.m_normal = g_normals[i].normalized;
			    }
		    }
	    }
	    return true;
    }

    public static bool Ray3dWithPlane3d(Plane3d plane, Ray3d ray, ref RaycastResult outResult) 
    {
	    //ResetRaycastResult(outResult);
        outResult.Reset();

	    float nd = Vector3.Dot(ray.m_rayDir, plane.m_planeNormal);
        float pn = Vector3.Dot(ray.m_rayOrigin, plane.m_planeNormal);

	    // nd must be negative, and not 0
	    // if nd is positive, the ray and plane normals
	    // point in the same direction. No intersection.
	    if (nd >= 0.0f) {
		    return false;
	    }

	    float t = (plane.GetDistanceFromOrigin() - pn) / nd;

	    // t must be positive
	    if (t >= 0.0f)
        {
		    //if (outResult != null)
            {
			    outResult.m_t = t;
			    outResult.m_hit = true;
			    outResult.m_point = ray.m_rayOrigin + ray.m_rayDir * t;
			    outResult.m_normal = plane.m_planeNormal;
		    }
		    return true;
	    }

	    return false;
    }

    public static bool Line3dWithSphere3d(Line3d line, Sphere3d sphere)
    {
        Vector3 closest = Distance3d.ClosestPointOfPoint3dWithLine3d(sphere.m_pos, line);
        float distSq = (sphere.m_pos - closest).sqrMagnitude;
        return distSq <= (sphere.m_radius * sphere.m_radius);
    }

    public static bool Segment3dWithSphere3d(Segment3d segment, Sphere3d sphere)
    {
        if(segment.m_point1 == segment.m_point2)
        {
            return false;
        }
        Vector3 closest = Distance3d.ClosestPointOfPoint3dWithSegment3d(sphere.m_pos, segment);
        float distSq = (sphere.m_pos - closest).sqrMagnitude;
        return distSq <= (sphere.m_radius * sphere.m_radius);
    }

    public static bool Line3dWithAABB3d(Line3d line, AABB3d aabb)
    {
        Ray3d ray1 = new Ray3d(line.m_point1, line.m_point2 - line.m_point1);
        Ray3d ray2 = new Ray3d(line.m_point2, line.m_point1 - line.m_point2);
        RaycastResult result = new RaycastResult();
        return Ray3dWithAABB3d(aabb, ray1, ref result)
            || Ray3dWithAABB3d(aabb, ray2, ref result);
    }

    public static bool Segment3dWithAABB3d(Segment3d segment, AABB3d aabb)
    {
        Ray3d ray = new Ray3d(segment.m_point1, segment.m_point2 - segment.m_point1);
        RaycastResult result = new RaycastResult();
        //float t = Raycast(aabb, ray);
        if (Ray3dWithAABB3d(aabb, ray, ref result))
        {
            return result.m_t >= 0 && result.m_t * result.m_t <= segment.GetLengthSqr();
        }
        else
        {
            return false;
        }
    }

    public static bool Line3dWithOBB3d(Line3d line, OBB3d obb)
    {
        Ray3d ray1 = new Ray3d(line.m_point1, line.m_point2 - line.m_point1);
        Ray3d ray2 = new Ray3d(line.m_point2, line.m_point1 - line.m_point2);
        RaycastResult result = new RaycastResult();
        return Ray3dWithOBB3d(obb, ray1, ref result)
            || Ray3dWithOBB3d(obb, ray2, ref result);
    }

    public static bool Segment3dWithOBB3d(Segment3d segment, OBB3d obb)
    {
        //ProfilerWrap.BeginSample("Segment3dWithOBB3d");
        Ray3d ray = new Ray3d(segment.m_point1, segment.m_point2 - segment.m_point1);
        RaycastResult result = new RaycastResult();
        bool ret = false;
        if(Ray3dWithOBB3d(obb, ray, ref result))
        {
            ret = result.m_t >= 0 && result.m_t * result.m_t <= segment.GetLengthSqr();
        }
        //ProfilerWrap.EndSample("Segment3dWithOBB3d");
        return ret;
    }

    public static bool Line3dWithPlane3d(Line3d line, Plane3d plane)
    {
        return Mathf.Approximately(Vector3.Dot(line.m_point2 - line.m_point1, plane.m_planeNormal), 0);
    }

    public static bool Segment3dWithPlane3d(Segment3d segment, Plane3d plane)
    {
        Vector3 ab = segment.m_point2 - segment.m_point1;
        float nA = Vector3.Dot(plane.m_planeNormal, segment.m_point1);
        float nAB = Vector3.Dot(plane.m_planeNormal, ab);
        // If the line and plane are parallel, nAB will be 0
        // This will cause a divide by 0 exception below
        // If you plan on testing parallel lines and planes
        // it is sage to early out when nAB is 0.
        float t = (plane.GetDistanceFromOrigin() - nA) / nAB;
        return t >= 0.0f && t <= 1.0f;
    }

    public static bool Point3dWithTriangle(Vector3 p, Triangle3d t) 
    {
	    // Move the triangle so that the point is  
	    // now at the origin of the triangle
        Vector3 a = t.m_point0 - p;
        Vector3 b = t.m_point1 - p;
        Vector3 c = t.m_point2 - p;

	    // The point should be moved too, so they are both
	    // relative, but because we don't use p in the
	    // equation anymore, we don't need it!
	    // p -= p; // This would just equal the zero vector!

        Vector3 normPBC = Vector3.Cross(b, c); // Normal of PBC (u)
        Vector3 normPCA = Vector3.Cross(c, a); // Normal of PCA (v)
        Vector3 normPAB = Vector3.Cross(a, b); // Normal of PAB (w)

	    // Test to see if the normals are facing 
	    // the same direction, return false if not
        if (Vector3.Dot(normPBC, normPCA) < 0.0f)
        {
		    return false;
	    }
        else if (Vector3.Dot(normPBC, normPAB) < 0.0f)
        {
		    return false;
	    }

	    // All normals facing the same way, return true
	    return true;
    }

    public static bool Triangle3dWithSphere3d(Triangle3d triangle, Sphere3d sphere)
    {
        Vector3 closest = Distance3d.ClosestPointOfPoint3dWithTriangle3d(triangle, sphere.m_pos);
        return (closest - sphere.m_pos).magnitude <= sphere.m_radius;
    }

    public static bool Triangle3dWithPlane3d(Triangle3d t, Plane3d p)
    {
        float side1 = Plane3d.PlaneEquation(t.m_point0, p);
        float side2 = Plane3d.PlaneEquation(t.m_point1, p);
        float side3 = Plane3d.PlaneEquation(t.m_point2, p);

        // On Plane
        if (Mathf.Approximately(side1, 0) && Mathf.Approximately(side2, 0) && Mathf.Approximately(side3, 0))
        {
            return true;
        }

        // Triangle in front of plane
        if (side1 > 0 && side2 > 0 && side3 > 0)
        {
            return false;
        }

        // Triangle behind plane
        if (side1 < 0 && side2 < 0 && side3 < 0)
        {
            return false;
        }

        return true; // Intersection
    }

    static Vector3 SatCrossEdge(Vector3 a, Vector3 b, Vector3 c, Vector3 d) 
    {
        Vector3 ab = b - a;
        Vector3 cd = d - c;

        Vector3 result = Vector3.Cross(ab, cd);
	    if (!Mathf.Approximately((result).sqrMagnitude, 0)) { // Is ab and cd parallel?
		    return result; // Not parallel!
	    }
	    else { // ab and cd are parallel
		    // Get an axis perpendicular to AB
            Vector3 axis = Vector3.Cross(ab, c - a);
            result = Vector3.Cross(ab, axis);
            if (!Mathf.Approximately((result).sqrMagnitude, 0))
            { // Still parallel?
			    return result; // Not parallel
		    }
	    }
	    // New axis being tested is parallel too.
	    // This means that a, b, c and d are on a line
	    // Nothing we can do!
	    return Vector3.zero;
    }

    public static bool Triangle3dWithTriangle3d(Triangle3d t1, Triangle3d t2) 
    {
	    Vector3[] axisToTest = {
		    // Triangle 1, Normal
		    SatCrossEdge(t1.m_point0, t1.m_point1, t1.m_point1, t1.m_point2),
		    // Triangle 2, Normal
		    SatCrossEdge(t2.m_point0, t2.m_point1, t2.m_point1, t2.m_point2),

		    // Cross Product of edges
		    SatCrossEdge(t2.m_point0, t2.m_point1, t1.m_point0, t1.m_point1),
		    SatCrossEdge(t2.m_point0, t2.m_point1, t1.m_point1, t1.m_point2),
		    SatCrossEdge(t2.m_point0, t2.m_point1, t1.m_point2, t1.m_point0),

		    SatCrossEdge(t2.m_point1, t2.m_point2, t1.m_point0, t1.m_point1),
		    SatCrossEdge(t2.m_point1, t2.m_point2, t1.m_point1, t1.m_point2),
		    SatCrossEdge(t2.m_point1, t2.m_point2, t1.m_point2, t1.m_point0),

		    SatCrossEdge(t2.m_point2, t2.m_point0, t1.m_point0, t1.m_point1),
		    SatCrossEdge(t2.m_point2, t2.m_point0, t1.m_point1, t1.m_point2),
		    SatCrossEdge(t2.m_point2, t2.m_point0, t1.m_point2, t1.m_point0),
	    };

	    for (int i = 0; i < 11; ++i) {
		    if (!OverlapOnAxis(t1, t2, axisToTest[i])) {
			    if (!Mathf.Approximately((axisToTest[i]).sqrMagnitude, 0)) {
				    return false; // Seperating axis found
			    }
		    }
	    }

	    return true; // Seperating axis not found
    }

    /// <summary>
    /// 返回质心坐标系中的坐标
    /// </summary>
    /// <param name="?"></param>
    /// <returns></returns>
    static Vector3 Barycentric(Vector3 p, Triangle3d t) 
    {
        Vector3 ap = p - t.m_point0;
        Vector3 bp = p - t.m_point1;
        Vector3 cp = p - t.m_point2;

        Vector3 ab = t.m_point1 - t.m_point0;
        Vector3 ac = t.m_point2 - t.m_point0;
        Vector3 bc = t.m_point2 - t.m_point1;
        Vector3 cb = t.m_point1 - t.m_point2;
        Vector3 ca = t.m_point0 - t.m_point2;

        Vector3 v = ab - Vector3.Project(ab, cb);
        float a = 1.0f - (Vector3.Dot(v, ap) / Vector3.Dot(v, ab));

        v = bc - Vector3.Project(bc, ac);
        float b = 1.0f - (Vector3.Dot(v, bp) / Vector3.Dot(v, bc));

        v = ca - Vector3.Project(ca, ab);
        float c = 1.0f - (Vector3.Dot(v, cp) / Vector3.Dot(v, ca));

        return new Vector3(a, b, c);
    }

    public static bool Ray3dWithTriangle3d(Triangle3d triangle, Ray3d ray, ref RaycastResult outResult) 
    {
	    //ResetRaycastResult(outResult);
        outResult.Reset();

	    Plane3d plane = Distance3d.FromTriangle(triangle);

	    RaycastResult planeResult = new RaycastResult();
	    if (!Ray3dWithPlane3d(plane, ray, ref planeResult)) {
		    return false;
	    }
	    float t = planeResult.m_t;

	    Vector3 result = ray.m_rayOrigin + ray.m_rayDir * t;

        Vector3 barycentric = Barycentric(result, triangle);
	    if (barycentric.x >= 0.0f && barycentric.x <= 1.0f &&
		    barycentric.y >= 0.0f && barycentric.y <= 1.0f &&
		    barycentric.z >= 0.0f && barycentric.z <= 1.0f) 
        {

		    //if (outResult != null) 
            {
			    outResult.m_t = t;
			    outResult.m_hit = true;
			    outResult.m_point = ray.m_rayOrigin + ray.m_rayDir * t;
			    outResult.m_normal = plane.m_planeNormal;
		    }

		    return true;
	    }

	    return false;
    }

    public static bool Line3dWithTriangle3d(Triangle3d triangle, Line3d line)
    {
        Ray3d ray1 = new Ray3d(line.m_point1, line.m_point2 - line.m_point1);
        Ray3d ray2 = new Ray3d(line.m_point2, line.m_point1 - line.m_point1);
        RaycastResult result = new RaycastResult();
        return Ray3dWithTriangle3d(triangle, ray1, ref result)
            || Ray3dWithTriangle3d(triangle, ray2, ref result);
    }

    public static bool Segment3dWithTriangle3d(Triangle3d triangle, Segment3d segment) 
    {
        Ray3d ray = new Ray3d(segment.m_point1, segment.m_point2 - segment.m_point1);
	    RaycastResult result =  new RaycastResult();
	    if (!Ray3dWithTriangle3d(triangle, ray, ref result)) {
		    return false;
	    }
	    float t = result.m_t;

	    return t >= 0 && t * t <= segment.GetLengthSqr();
    }

    public static float Ray3dWithMesh3d(Mesh3d mesh, Ray3d ray)
    {
        if (mesh.m_accelerator == null)
        {
            for (int i = 0; i < mesh.m_triangleList.Count; ++i)
            {
                RaycastResult raycast = new RaycastResult();
                Ray3dWithTriangle3d(mesh.m_triangleList[i], ray, ref raycast);
                float result = raycast.m_t;
                if (result >= 0)
                {
                    return result;
                }
            }
        }
        else
        {
            //std::list<BVHNode*> toProcess;
            //toProcess.push_front(mesh.accelerator);
            List<Mesh3d.BVHNode> toProcess = new List<Mesh3d.BVHNode>();
            toProcess.Add(mesh.m_accelerator);

            // Recursivley walk the BVH tree
            while (toProcess.Count != 0)
            {
                //BVHNode* iterator = *(toProcess.begin());
                //toProcess.erase(toProcess.begin());
                Mesh3d.BVHNode iterator = toProcess[0];
                toProcess.RemoveAt(0);

                if (iterator.m_triangles.Count >= 0)
                {
                    // Iterate trough all triangles of the node
                    for (int i = 0; i < iterator.m_triangles.Count; ++i)
                    {
                        // Triangle indices in BVHNode index the mesh
                        RaycastResult raycast = new RaycastResult();
                        Ray3dWithTriangle3d(iterator.m_triangles[i], ray, ref raycast);
                        float r = raycast.m_t;
                        if (r >= 0)
                        {
                            return r;
                        }
                    }
                }

                if (iterator.m_children != null)
                {
                    for (int i = 8 - 1; i >= 0; --i)
                    {
                        // Only push children whos bounds intersect the test geometry
                        RaycastResult raycast = new RaycastResult();
                        Ray3dWithAABB3d(iterator.m_children[i].m_bounds, ray, ref raycast);
                        if (raycast.m_t >= 0)
                        {
                            //toProcess.push_front(&iterator->children[i]);
                            toProcess.Insert(0, iterator.m_children[i]);
                        }
                    }
                }
            }
        }
        return -1;
    }

    public static bool Segment3dWithMesh3d(Mesh3d mesh, Segment3d segment)
    {
        if (mesh.m_accelerator == null)
        {
            for (int i = 0; i < mesh.m_triangleList.Count; ++i)
            {
                if (Segment3dWithTriangle3d(mesh.m_triangleList[i], segment))
                {
                    return true;
                }
            }
        }
        else
        {
            //std::list<BVHNode*> toProcess;
            //toProcess.push_front(mesh.accelerator);
            List<Mesh3d.BVHNode> toProcess = new List<Mesh3d.BVHNode>();
            toProcess.Add(mesh.m_accelerator);

            // Recursivley walk the BVH tree
            while (toProcess.Count != 0)
            {
                //BVHNode* iterator = *(toProcess.begin());
                //toProcess.erase(toProcess.begin());
                Mesh3d.BVHNode iterator = toProcess[0];
                toProcess.RemoveAt(0);

                if (iterator.m_triangles.Count >= 0)
                {
                    // Iterate trough all triangles of the node
                    for (int i = 0; i < iterator.m_triangles.Count; ++i)
                    {
                        // Triangle indices in BVHNode index the mesh
                        if (Segment3dWithTriangle3d(iterator.m_triangles[i], segment))
                        {
                            return true;
                        }
                    }
                }

                if (iterator.m_children != null)
                {
                    for (int i = 8 - 1; i >= 0; --i)
                    {
                        // Only push children whos bounds intersect the test geometry
                        if (Segment3dWithAABB3d(segment, iterator.m_children[i].m_bounds))
                        {
                            toProcess.Insert(0, iterator.m_children[i]);
                        }
                    }
                }
            }
        }
        return false;
    }

    public static bool Mesh3dWithAABB3d(Mesh3d mesh, AABB3d aabb) 
    {
	    if (mesh.m_accelerator == null) {
		    for (int i = 0; i < mesh.m_triangleList.Count; ++i) {
                if (Triangle3dWithAABB3d(mesh.m_triangleList[i], aabb))
                {
				    return true;
			    }
		    }
	    }
	    else {
		    //std::list<BVHNode*> toProcess;
		    //toProcess.push_front(mesh.accelerator);
            List<Mesh3d.BVHNode> toProcess = new List<Mesh3d.BVHNode>();
            toProcess.Add(mesh.m_accelerator);

		    // Recursivley walk the BVH tree
            while (toProcess.Count != 0)
            {
			    //BVHNode* iterator = *(toProcess.begin());
			    //toProcess.erase(toProcess.begin());
                Mesh3d.BVHNode iterator = toProcess[0];
                toProcess.RemoveAt(0);

			    if (iterator.m_triangles.Count >= 0) {
				    // Iterate trough all triangles of the node
				    for (int i = 0; i < iterator.m_triangles.Count; ++i) {
					    // Triangle indices in BVHNode index the mesh
                        if (Triangle3dWithAABB3d(iterator.m_triangles[i], aabb))
                        {
						    return true;
					    }
				    }
			    }

			    if (iterator.m_children != null) {
				    for (int i = 8 - 1; i >= 0; --i) {
					    // Only push children whos bounds intersect the test geometry
                        if (AABB3dWithAABB3d(iterator.m_children[i].m_bounds, aabb))
                        {
						    toProcess.Insert(0, iterator.m_children[i]);
					    }
				    }
			    }
		    }
	    }
	    return false;
    }

    public static bool Mesh3dWithSphere3d(Mesh3d mesh, Sphere3d sphere)
    {
        if (mesh.m_accelerator == null)
        {
            for (int i = 0; i < mesh.m_triangleList.Count; ++i)
            {
                if (Triangle3dWithSphere3d(mesh.m_triangleList[i], sphere))
                {
                    return true;
                }
            }
        }
        else
        {
            //std::list<BVHNode*> toProcess;
            //toProcess.push_front(mesh.accelerator);
            List<Mesh3d.BVHNode> toProcess = new List<Mesh3d.BVHNode>();
            toProcess.Add(mesh.m_accelerator);

            // Recursivley walk the BVH tree
            while (toProcess.Count != 0)
            {
                //BVHNode* iterator = *(toProcess.begin());
                //toProcess.erase(toProcess.begin());
                Mesh3d.BVHNode iterator = toProcess[0];
                toProcess.RemoveAt(0);

                if (iterator.m_triangles.Count >= 0)
                {
                    // Iterate trough all triangles of the node
                    for (int i = 0; i < iterator.m_triangles.Count; ++i)
                    {
                        // Triangle indices in BVHNode index the mesh
                        if (Triangle3dWithSphere3d(iterator.m_triangles[i], sphere))
                        {
                            return true;
                        }
                    }
                }

                if (iterator.m_children != null)
                {
                    for (int i = 8 - 1; i >= 0; --i)
                    {
                        // Only push children whos bounds intersect the test geometry
                        if (Sphere3dWithAABB3d(sphere, iterator.m_children[i].m_bounds))
                        {
                            toProcess.Insert(0, iterator.m_children[i]);
                        }
                    }
                }
            }
        }
        return false;
    }

    public static bool Mesh3dWithOBB3d(Mesh3d mesh, OBB3d obb)
    {
        if (mesh.m_accelerator == null)
        {
            for (int i = 0; i < mesh.m_triangleList.Count; ++i)
            {
                if (Triangle3dWithOBB3d(mesh.m_triangleList[i], obb))
                {
                    return true;
                }
            }
        }
        else
        {
            //std::list<BVHNode*> toProcess;
            //toProcess.push_front(mesh.accelerator);
            List<Mesh3d.BVHNode> toProcess = new List<Mesh3d.BVHNode>();
            toProcess.Add(mesh.m_accelerator);

            // Recursivley walk the BVH tree
            while (toProcess.Count != 0)
            {
                //BVHNode* iterator = *(toProcess.begin());
                //toProcess.erase(toProcess.begin());
                Mesh3d.BVHNode iterator = toProcess[0];
                toProcess.RemoveAt(0);

                if (iterator.m_triangles.Count >= 0)
                {
                    // Iterate trough all triangles of the node
                    for (int i = 0; i < iterator.m_triangles.Count; ++i)
                    {
                        // Triangle indices in BVHNode index the mesh
                        if (Triangle3dWithOBB3d(iterator.m_triangles[i], obb))
                        {
                            return true;
                        }
                    }
                }

                if (iterator.m_children != null)
                {
                    for (int i = 8 - 1; i >= 0; --i)
                    {
                        // Only push children whos bounds intersect the test geometry
                        if (AABB3dWithOBB3d(iterator.m_children[i].m_bounds, obb))
                        {
                            toProcess.Insert(0, iterator.m_children[i]);
                        }
                    }
                }
            }
        }
        return false;
    }

    public static bool AABB3dWithPlane3d(AABB3d aabb, Plane3d plane)
    {
        // Project the half extents of the AABB onto the plane normal
        float pLen = aabb.GetHalfSize().x * Math.Abs(plane.m_planeNormal.x) +
                    aabb.GetHalfSize().y * Math.Abs(plane.m_planeNormal.y) +
                    aabb.GetHalfSize().z * Math.Abs(plane.m_planeNormal.z);
        // Find the distance from the center of the AABB to the plane
        float dist = Vector3.Dot(plane.m_planeNormal, aabb.m_pos) - plane.GetDistanceFromOrigin();
        // Intersection occurs if the distance falls within the projected side
        return Math.Abs(dist) <= pLen;
    }

    public static bool Mesh3dWithPlane3d(Mesh3d mesh, Plane3d plane)
    {
        if (mesh.m_accelerator == null)
        {
            for (int i = 0; i < mesh.m_triangleList.Count; ++i)
            {
                if (Triangle3dWithPlane3d(mesh.m_triangleList[i], plane))
                {
                    return true;
                }
            }
        }
        else
        {
            //std::list<BVHNode*> toProcess;
            //toProcess.push_front(mesh.accelerator);
            List<Mesh3d.BVHNode> toProcess = new List<Mesh3d.BVHNode>();
            toProcess.Add(mesh.m_accelerator);

            // Recursivley walk the BVH tree
            while (toProcess.Count != 0)
            {
                //BVHNode* iterator = *(toProcess.begin());
                //toProcess.erase(toProcess.begin());
                Mesh3d.BVHNode iterator = toProcess[0];
                toProcess.RemoveAt(0);

                if (iterator.m_triangles.Count >= 0)
                {
                    // Iterate trough all triangles of the node
                    for (int i = 0; i < iterator.m_triangles.Count; ++i)
                    {
                        // Triangle indices in BVHNode index the mesh
                        if (Triangle3dWithPlane3d(iterator.m_triangles[i], plane))
                        {
                            return true;
                        }
                    }
                }

                if (iterator.m_children != null)
                {
                    for (int i = 8 - 1; i >= 0; --i)
                    {
                        // Only push children whos bounds intersect the test geometry
                        if (AABB3dWithPlane3d(iterator.m_children[i].m_bounds, plane))
                        {
                            toProcess.Insert(0, iterator.m_children[i]);
                        }
                    }
                }
            }
        }
        return false;
    }

    public static bool Mesh3dWithTriangle3d(Mesh3d mesh, Triangle3d triangle)
    {
        if (mesh.m_accelerator == null)
        {
            for (int i = 0; i < mesh.m_triangleList.Count; ++i)
            {
                if (Triangle3dWithTriangle3d(mesh.m_triangleList[i], triangle))
                {
                    return true;
                }
            }
        }
        else
        {
            //std::list<BVHNode*> toProcess;
            //toProcess.push_front(mesh.accelerator);
            List<Mesh3d.BVHNode> toProcess = new List<Mesh3d.BVHNode>();
            toProcess.Add(mesh.m_accelerator);

            // Recursivley walk the BVH tree
            while (toProcess.Count != 0)
            {
                //BVHNode* iterator = *(toProcess.begin());
                //toProcess.erase(toProcess.begin());
                Mesh3d.BVHNode iterator = toProcess[0];
                toProcess.RemoveAt(0);

                if (iterator.m_triangles.Count >= 0)
                {
                    // Iterate trough all triangles of the node
                    for (int i = 0; i < iterator.m_triangles.Count; ++i)
                    {
                        // Triangle indices in BVHNode index the mesh
                        if (Triangle3dWithTriangle3d(iterator.m_triangles[i], triangle))
                        {
                            return true;
                        }
                    }
                }

                if (iterator.m_children != null)
                {
                    for (int i = 8 - 1; i >= 0; --i)
                    {
                        // Only push children whos bounds intersect the test geometry
                        if (Triangle3dWithAABB3d(triangle, iterator.m_children[i].m_bounds))
                        {
                            toProcess.Insert(0, iterator.m_children[i]);
                        }
                    }
                }
            }
        }
        return false;
    }

    public static float Ray3dWithModel3d(Ray3d ray, Model3d model)
    {
        Matrix4x4 obj2world = model.GetObj2WorldMatrix();
        Matrix4x4 world2Obj = obj2world.inverse;
        Ray3d local = new Ray3d(world2Obj * ray.m_rayOrigin, world2Obj * ray.m_rayDir /*貌似只有scale不变形的法线 才能这么变换*/);
        //local.origin = MultiplyPoint(ray.origin, inv);
        //local.direction = MultiplyVector(ray.direction, inv);
        //local.NormalizeDirection();
        if (model.GetMesh() != null)
        {
            return Ray3dWithMesh3d(model.GetMesh(), local);
        }
        return -1;
    }

    public static bool Segment3dWithModel3d(Segment3d segment, Model3d model)
    {
        Matrix4x4 obj2world = model.GetObj2WorldMatrix();
        Matrix4x4 world2Obj = obj2world.inverse;
        Segment3d local = new Segment3d(world2Obj * segment.m_point1, world2Obj * segment.m_point2);
        if(model.GetMesh() != null)
        {
            return Segment3dWithMesh3d(model.GetMesh(), segment);
        }
        return false;
    }

    public static bool Model3dWithSphere3d(Model3d model, Sphere3d sphere)
    {
        Matrix4x4 obj2world = model.GetObj2WorldMatrix();
        Matrix4x4 world2Obj = obj2world.inverse;
        Sphere3d local = new Sphere3d(world2Obj * sphere.m_pos, sphere.m_radius);
        //local.position = MultiplyPoint(sphere.position, inv);
        if (model.GetMesh() != null)
        {
            return Mesh3dWithSphere3d(model.GetMesh(), local);
        }
        return false;
    }

    public static bool Model3dWithAABB3d(Model3d model, AABB3d aabb)
    {
        Matrix4x4 obj2world = model.GetObj2WorldMatrix();
        Matrix4x4 world2Obj = obj2world.inverse;
        Quaternion roation = RotateHelper.GetRotationFromMatrix(world2Obj);
        OBB3d local = new OBB3d(world2Obj * aabb.m_pos, roation, aabb.m_size.x, aabb.m_size.y, aabb.m_size.z);
        if(model.GetMesh() != null)
        {
            return Mesh3dWithOBB3d(model.GetMesh(), local);
        }
        return false;
    }

    public static bool Model3dWithOBB3d(Model3d model, OBB3d obb)
    {
        Matrix4x4 obj2world = model.GetObj2WorldMatrix();
        Matrix4x4 world2Obj = obj2world.inverse;
        Quaternion roation =  obb.m_rotation * RotateHelper.GetRotationFromMatrix(world2Obj);
        OBB3d local = new OBB3d(world2Obj * obb.m_pos, roation, obb.m_size.x, obb.m_size.y, obb.m_size.z);
        if (model.GetMesh() != null)
        {
            return Mesh3dWithOBB3d(model.GetMesh(), local);
        }
        return false;
    }

    public static bool Model3dWithPlane3d(Model3d model, Plane3d plane) 
    {
        Matrix4x4 obj2world = model.GetObj2WorldMatrix();
        Matrix4x4 world2Obj = obj2world.inverse;
        Plane3d local = new Plane3d(world2Obj * plane.m_planeNormal/*貌似只有scale不变形的法线 才能这么变换*/, world2Obj * plane.m_planeOnePoint);
	    //local.normal = MultiplyVector(plane.normal, inv);
	    //local.distance = plane.distance;
	    if (model.GetMesh() != null) {
            return Mesh3dWithPlane3d(model.GetMesh(), local);
	    }
	    return false;
    }

    public static bool Model3dWithTriangle3d(Model3d model, Triangle3d triangle) 
    {
        Matrix4x4 obj2world = model.GetObj2WorldMatrix();
        Matrix4x4 world2Obj = obj2world.inverse;
        Triangle3d local = new Triangle3d(world2Obj * triangle.m_point0, world2Obj * triangle.m_point1, world2Obj * triangle.m_point2);
        if (model.GetMesh() != null)
        {
            return Mesh3dWithTriangle3d(model.GetMesh(), local);
        }
        return false;
    }

    public static bool Frustum3dWithSphere3d(Frustum3d frustum, Sphere3d sphere)
    {
        foreach (var iter in frustum.m_planeArray)
        {
            Vector3 normal = iter.m_planeNormal;
            float dist = iter.GetDistanceFromOrigin();
            float side = Vector3.Dot(sphere.m_pos, normal) - dist;
            if (side < -sphere.m_radius)
            {
                return false;
            }
        }
        return true;
    }

    static float Classify(AABB3d aabb, Plane3d plane)
    {
	    // maximum extent in direction of plane normal 
	    float r = Math.Abs(aabb.GetHalfSize().x * plane.m_planeNormal.x)
            + Math.Abs(aabb.GetHalfSize().y * plane.m_planeNormal.y)
            + Math.Abs(aabb.GetHalfSize().z * plane.m_planeNormal.z);

        // signed distance between box center and plane
        //float d = plane.Test(mCenter);
        float d = Vector3.Dot(plane.m_planeNormal, aabb.m_pos) - plane.GetDistanceFromOrigin();

	    // return signed distance
	    if (Math.Abs(d) < r)
        {
		    return 0.0f;
	    }
	    else if (d< 0.0f)
        {
		    return d + r;
	    }
	    return d - r;
    }

    public static bool Frustum3dWithAABB3d(Frustum3d frustum, AABB3d aabb)
    {
        for (int i = 0; i < 6; ++i)
        {
            float side = Classify(aabb, frustum.m_planeArray[i]);
            if (side < 0)
            {
                return false;
            }
        }
        return true;
    }

    public static float Classify(OBB3d obb, Plane3d plane) 
    {
        Matrix4x4 obj2world = obb.GetObjToWorld();
        Matrix4x4 world2Obj = obj2world.inverse;

        //Vector3 normal = MultiplyVector(plane.normal, obb.orientation);
        Vector3 normal = world2Obj * plane.m_planeNormal;

        // maximum extent in direction of plane normal 
        float r = Math.Abs(obb.GetHalfSize().x * normal.x)
            + Math.Abs(obb.GetHalfSize().y * normal.y)
            + Math.Abs(obb.GetHalfSize().z * normal.z);

        // signed distance between box center and plane
        //float d = plane.Test(mCenter);
        float tmp1 = Vector3.Dot(plane.m_planeNormal, obb.m_pos);
        float tmp2 = -plane.GetDistanceFromOrigin();
        float d = tmp1 + tmp2;

	    // return signed distance
	    if (Math.Abs(d) < r)
        {
		    return 0.0f;
	    }
	    else if (d< 0.0f)
        {
		    return d + r;
	    }
	    return d - r;
    }

    public static bool Frustum3dWithOBB3d(Frustum3d frustum, OBB3d obb)
    {
        for (int i = 0; i < 6; ++i)
        {
            float side = Classify(obb, frustum.m_planeArray[i]);
            if (side < 0)
            {
                return false;
            }
        }
        return true;
    }

    public static bool Sphere3dWithSphere3d(Sphere3d sphere1, Sphere3d sphere2)
    {
        Vector3 distance = sphere1.m_pos - sphere2.m_pos;
        float radiusSum = sphere1.m_radius + sphere2.m_radius;
        if (distance.sqrMagnitude < radiusSum * radiusSum )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool Point3dWithSphere3d(Vector3 point, Sphere3d sphere)
    {
        Vector3 distance = sphere.m_pos - point;
        return distance.sqrMagnitude <= sphere.m_radius * sphere.m_radius;
    }

    public static bool Capsule3dWithSphere3d(Capsule3d capsule, Sphere3d sphere)
    {
        //dir.Normalize();
        Segment3d segment = new Segment3d(capsule.m_point1, capsule.m_point2);
        Vector3 closePointWithSegment = Distance3d.ClosestPointOfPoint3dWithSegment3d(sphere.m_pos, segment);

        Vector3 closestPointDistance = sphere.m_pos - closePointWithSegment;
        if(closestPointDistance.magnitude <= capsule.m_radius + sphere.m_radius)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool Capsule3dWithAABB3d(Capsule3d capsule, AABB3d aabb)
    {
        return GJKIntersection(capsule, aabb);
    }

    public static bool Capsule3dWithOBB3d(Capsule3d capsule, OBB3d obb)
    {
        //ProfilerWrap.BeginSample("Capsule3dWithOBB3d");
        Matrix4x4 obj2World = obb.GetObjToWorld();
        Matrix4x4 worldToObj = obj2World.inverse;
        Capsule3d localCapsule = new Capsule3d(worldToObj.MultiplyPoint(capsule.m_point1), worldToObj.MultiplyPoint(capsule.m_point2), capsule.m_radius);
        AABB3d localAABB = new AABB3d(worldToObj.MultiplyPoint(obb.m_pos), obb.m_size);
        bool ret = GJKIntersection(localCapsule, localAABB);
        //ProfilerWrap.EndSample("Capsule3dWithOBB3d");
        return ret;
    }
    //     public static bool SphereCastWithOBB3d(Sphere3d srcSphere, Vector3 dir, float distance, OBB3d obb)
    //     {
    // 
    //     }

    #region GJK
    // copy from MathGeoLib author Jukka Jylanki
    // Implementation of the Gilbert-Johnson-Keerthi(GJK) convex polyhedron intersection test.
    static bool GJKIntersection(Capsule3d shapeA, AABB3d shapeB)
    {
        Vector3[] support = new Vector3[4];
        float maxS = 0, minS = 0;
        // Start with an arbitrary point in the Minkowski set shape.
        support[0] = (shapeA.AnyPointFast() - shapeB.AnyPointFast());
        Vector3 dir = -support[0]; // First search direction is straight toward the origin from the found point.
        if (dir.sqrMagnitude < float.Epsilon) // Robustness check: Test if the first arbitrary point we guessed produced the zero vector we are looking for!
            return true;
        int n = 1; // Stores the current number of points in the search simplex.
        int nIterations = 50; // Robustness check: Limit the maximum number of iterations to perform to avoid infinite loop if types A or B are buggy!
        while (nIterations-- > 0)
        {
            // Compute the extreme point to the direction d in the Minkowski set shape.
            Vector3 newSupport = shapeA.ExtremePoint(dir, ref maxS) - shapeB.ExtremePoint(-dir, ref minS);
            // If the most extreme point in that search direction did not walk past the origin, then the origin cannot be contained in the Minkowski
            // convex shape, and the two convex objects a and b do not share a common point - no intersection!
            if (minS + maxS < 0)
            {
                int test = 0;
                //shapeA.ExtremePoint(dir, ref maxS);
                return false;
            }
            // Add the newly evaluated point to the search simplex.
            support[n++] = newSupport;
            // Examine the current simplex, prune a redundant part of it, and produce the next search direction.
            dir = UpdateSimplex(support, ref n);
            if (n == 0) // Was the origin contained in the current simplex? If so, then the convex shapes a and b do share a common point - intersection!
                return true;
        }
        Debug.LogError("GJK intersection test did not converge to a result!");
        return false; // Report no intersection.
    }

    static bool GJKIntersectionL(Capsule3d shapeA, AABB3d shapeB)
    {
        Vector3[] support = new Vector3[4];
        float maxS = 0, minS = 0;
        // Start with an arbitrary point in the Minkowski set shape.
        support[0] = shapeA.AnyPointFast() - shapeB.AnyPointFast();
        Vector3 dir = -support[0]; // First search direction is straight toward the origin from the found point.
        if (dir.sqrMagnitude < float.Epsilon) // Robustness check: Test if the first arbitrary point we guessed produced the zero vector we are looking for!
            return true;
        int n = 1; // Stores the current number of points in the search simplex.
        int nIterations = 50; // Robustness check: Limit the maximum number of iterations to perform to avoid infinite loop if types A or B are buggy!
        while (nIterations-- > 0)
        {
            // Compute the extreme point to the direction d in the Minkowski set shape.
            Vector3 newSupport = shapeA.ExtremePoint(dir, ref maxS) - shapeB.ExtremePoint(-dir, ref minS);
            // If the most extreme point in that search direction did not walk past the origin, then the origin cannot be contained in the Minkowski
            // convex shape, and the two convex objects a and b do not share a common point - no intersection!
            if (minS + maxS < 0)
            {
                int test = 0;
                //shapeA.ExtremePoint(dir, ref maxS);
                return false;
            }
            // Add the newly evaluated point to the search simplex.
            support[n++] = newSupport;
            // Examine the current simplex, prune a redundant part of it, and produce the next search direction.
            dir = UpdateSimplexL(support, ref n);
            if (n == 0) // Was the origin contained in the current simplex? If so, then the convex shapes a and b do share a common point - intersection!
                return true;
        }
        Debug.LogError("GJK intersection test did not converge to a result!");
        return false; // Report no intersection.
    }

    static Vector3 UpdateSimplex(Vector3[] s, ref int n)
    {
        if (n == 2)
        {
            // Four voronoi regions that the origin could be in:
            // 0) closest to vertex s[0].
            // 1) closest to vertex s[1].
            // 2) closest to line segment s[0]->s[1]. XX
            // 3) contained in the line segment s[0]->s[1], and our search is over and the algorithm is now finished. XX

            // By construction of the simplex, the cases 0) and 1) can never occur. Then only the cases marked with XX need to be checked.

            Vector3 d01 = s[1] - s[0];
            Vector3 newSearchDir = Vector3.Cross(d01, Vector3.Cross(d01, s[1]));
            if (newSearchDir.sqrMagnitude > float.Epsilon)
                return newSearchDir; // Case 2)
            else
            {
                // Case 3)
                n = 0;
                return Vector3.zero;
            }
        }
        else if (n == 3)
        {
            // Nine voronoi regions:
            // 0) closest to vertex s[0].
            // 1) closest to vertex s[1].
            // 2) closest to vertex s[2].
            // 3) closest to edge s[0]->s[1].
            // 4) closest to edge s[1]->s[2].  XX
            // 5) closest to edge s[0]->s[2].  XX
            // 6) closest to the triangle s[0]->s[1]->s[2], in the positive side.  XX
            // 7) closest to the triangle s[0]->s[1]->s[2], in the negative side.  XX
            // 8) contained in the triangle s[0]->s[1]->s[2], and our search is over and the algorithm is now finished.  XX

            // By construction of the simplex, the origin must always be in a voronoi region that includes the point s[2], since that
            // was the last added point. But it cannot be the case 2), since previous search took us deepest towards the direction s[1]->s[2],
            // and case 2) implies we should have been able to go even deeper in that direction, or that the origin is not included in the convex shape,
            // a case which has been checked for already before. Therefore the cases 0)-3) can never occur. Only the cases marked with XX need to be checked.

            Vector3 d12 = s[2] - s[1];
            Vector3 d02 = s[2] - s[0];
            Vector3 triNormal = Vector3.Cross(d02, d12);

            Vector3 e12 = Vector3.Cross(d12, triNormal);
            float t12 = Vector3.Dot(s[1], e12);
            if (t12 < 0)
            {
                // Case 4: Edge 1->2 is closest.

                Vector3 newDir = Vector3.Cross(d12, Vector3.Cross(d12, s[1]));
                s[0] = s[1];
                s[1] = s[2];
                n = 2;
                return newDir;
            }
            Vector3 e02 = Vector3.Cross(triNormal, d02);
            float t02 = Vector3.Dot(s[0], e02);
            if (t02 < 0)
            {
                // Case 5: Edge 0->2 is closest.

                Vector3 newDir = Vector3.Cross(d02, Vector3.Cross(d02, s[0]));
                s[1] = s[2];
                n = 2;
                return newDir;
            }
            // Cases 6)-8):

            float scaledSignedDistToTriangle = Vector3.Dot(triNormal, s[2]);
            float distSq = scaledSignedDistToTriangle * scaledSignedDistToTriangle;
            float scaledEpsilonSq = 1e-6f * triNormal.sqrMagnitude;

            if (distSq > scaledEpsilonSq)
            {
                // The origin is sufficiently far away from the triangle.
                if (scaledSignedDistToTriangle <= 0)
                    return triNormal; // Case 6)
                else
                {
                    // Case 7) Swap s[0] and s[1] so that the normal of Triangle(s[0],s[1],s[2]).PlaneCCW() will always point towards the new search direction.
                    //std::swap(s[0], s[1]);
                    GUtil.Swap(ref s[0], ref s[1]);
                    //Array.swap
                    return -triNormal;
                }
            }
            else
            {
                // Case 8) The origin lies directly inside the triangle. For robustness, terminate the search here immediately with success.
                n = 0;
                return Vector3.zero;
            }
        }
        else // n == 4
        {
            // A tetrahedron defines fifteen voronoi regions:
            //  0) closest to vertex s[0].
            //  1) closest to vertex s[1].
            //  2) closest to vertex s[2].
            //  3) closest to vertex s[3].
            //  4) closest to edge s[0]->s[1].
            //  5) closest to edge s[0]->s[2].
            //  6) closest to edge s[0]->s[3].  XX
            //  7) closest to edge s[1]->s[2].
            //  8) closest to edge s[1]->s[3].  XX
            //  9) closest to edge s[2]->s[3].  XX
            // 10) closest to the triangle s[0]->s[1]->s[2], in the outfacing side.
            // 11) closest to the triangle s[0]->s[1]->s[3], in the outfacing side. XX
            // 12) closest to the triangle s[0]->s[2]->s[3], in the outfacing side. XX
            // 13) closest to the triangle s[1]->s[2]->s[3], in the outfacing side. XX
            // 14) contained inside the tetrahedron simplex, and our search is over and the algorithm is now finished. XX

            // By construction of the simplex, the origin must always be in a voronoi region that includes the point s[3], since that
            // was the last added point. But it cannot be the case 3), since previous search took us deepest towards the direction s[2]->s[3],
            // and case 3) implies we should have been able to go even deeper in that direction, or that the origin is not included in the convex shape,
            // a case which has been checked for already before. Therefore the cases 0)-5), 7) and 10) can never occur and
            // we only need to check cases 6), 8), 9), 11), 12), 13) and 14), marked with XX.

            Vector3 d01 = s[1] - s[0];
            Vector3 d02 = s[2] - s[0];
            Vector3 d03 = s[3] - s[0];
            Vector3 tri013Normal = Vector3.Cross(d01, d03); // Normal of triangle 0->1->3 pointing outwards from the simplex.
            Vector3 tri023Normal = Vector3.Cross(d03, d02); // Normal of triangle 0->2->3 pointing outwards from the simplex.
            //assert(Dot(tri013Normal, d02) <= 0.f);
            //assert(Dot(tri023Normal, d01) <= 0.f);

            Vector3 e03_1 = Vector3.Cross(tri013Normal, d03); // The normal of edge 0->3 on triangle 013.
            Vector3 e03_2 = Vector3.Cross(d03, tri023Normal); // The normal of edge 0->3 on triangle 023.
            float inE03_1 = Vector3.Dot(e03_1, s[3]);
            float inE03_2 = Vector3.Dot(e03_2, s[3]);
            if (inE03_1 <= 0 && inE03_2 <= 0)
            {
                // Case 6) Edge 0->3 is closest. Simplex degenerates to a line segment.

                Vector3 newDir = Vector3.Cross(d03, Vector3.Cross(d03, s[3]));
                s[1] = s[3];
                n = 2;
                return newDir;
            }

            Vector3 d12 = s[2] - s[1];
            Vector3 d13 = s[3] - s[1];
            Vector3 tri123Normal = Vector3.Cross(d12, d13);
            //assert(Dot(tri123Normal, -d02) <= 0.f);
            Vector3 e13_0 = Vector3.Cross(d13, tri013Normal);
            Vector3 e13_2 = Vector3.Cross(tri123Normal, d13);
            float inE13_0 = Vector3.Dot(e13_0, s[3]);
            float inE13_2 = Vector3.Dot(e13_2, s[3]);
            if (inE13_0 <= 0 && inE13_2 <= 0)
            {
                // Case 8) Edge 1->3 is closest. Simplex degenerates to a line segment.

                Vector3 newDir = Vector3.Cross(d13, Vector3.Cross(d13, s[3]));
                s[0] = s[1];
                s[1] = s[3];
                n = 2;
                return newDir;
            }

            Vector3 d23 = s[3] - s[2];
            Vector3 e23_0 = Vector3.Cross(tri023Normal, d23);
            Vector3 e23_1 = Vector3.Cross(d23, tri123Normal);
            float inE23_0 = Vector3.Dot(e23_0, s[3]);
            float inE23_1 = Vector3.Dot(e23_1, s[3]);
            if (inE23_0 <= 0 && inE23_1 <= 0)
            {
                // Case 9) Edge 2->3 is closest. Simplex degenerates to a line segment.

                Vector3 newDir = Vector3.Cross(d23, Vector3.Cross(d23, s[3]));
                s[0] = s[2];
                s[1] = s[3];
                n = 2;
                return newDir;
            }

            float inTri013 = Vector3.Dot(s[3], tri013Normal);
            if (inTri013 < 0 && inE13_0 >= 0 && inE03_1 >= 0)
            {
                // Case 11) Triangle 0->1->3 is closest.

                s[2] = s[3];
                n = 3;
                return tri013Normal;
            }
            float inTri023 = Vector3.Dot(s[3], tri023Normal);
            if (inTri023 < 0 && inE23_0 >= 0 && inE03_2 >= 0)
            {
                // Case 12) Triangle 0->2->3 is closest.

                s[1] = s[0];
                s[0] = s[2];
                s[2] = s[3];
                n = 3;
                return tri023Normal;
            }
            float inTri123 = Vector3.Dot(s[3], tri123Normal);
            if (inTri123 < 0 && inE13_2 >= 0 && inE23_1 >= 0)
            {
                // Case 13) Triangle 1->2->3 is closest.

                s[0] = s[1];
                s[1] = s[2];
                s[2] = s[3];
                n = 3;
                return tri123Normal;
            }

            // Case 14) Not in the voronoi region of any triangle or edge. The origin is contained in the simplex, the search is finished.
            n = 0;
            return Vector3.zero;
        }
    }

    /// This function examines the simplex defined by the array of points in s, and calculates which voronoi region
    /// of that simplex the origin is closest to. Based on that information, the function constructs a new simplex
    /// that will be used to continue the search, and returns a new search direction for the GJK algorithm.
    /** @param s [in, out] An array of points in the simplex. When this function returns, this point array is updated to contain the new search simplex.
        @param n [in, out] The number of points in the array s. When this function returns, this reference is updated to specify how many
                           points the new search simplex contains.
        @return The new search direction vector. */
    static Vector3 UpdateSimplexL(Vector3[] s, ref int n)
    {
        if (n == 2)
        {
            // Four voronoi regions that the origin could be in:
            // 0) closest to vertex s[0].
            // 1) closest to vertex s[1].
            // 2) closest to line segment s[0]->s[1]. XX
            // 3) contained in the line segment s[0]->s[1], and our search is over and the algorithm is now finished. XX

            // By construction of the simplex, the cases 0) and 1) can never occur. Then only the cases marked with XX need to be checked.

            Vector3 d01 = s[1] - s[0];
            Vector3 newSearchDir = Vector3.Cross(d01, Vector3.Cross(d01, s[1]));
            // .sqrMagnitude 居然有溢出。。
            if (newSearchDir.sqrMagnitude > float.Epsilon)
                return newSearchDir; // Case 2)
            else
            {
                // Case 3)
                n = 0;
                return Vector3.zero;
            }
        }
        else if (n == 3)
        {
            // Nine voronoi regions:
            // 0) closest to vertex s[0].
            // 1) closest to vertex s[1].
            // 2) closest to vertex s[2].
            // 3) closest to edge s[0]->s[1].
            // 4) closest to edge s[1]->s[2].  XX
            // 5) closest to edge s[0]->s[2].  XX
            // 6) closest to the triangle s[0]->s[1]->s[2], in the positive side.  XX
            // 7) closest to the triangle s[0]->s[1]->s[2], in the negative side.  XX
            // 8) contained in the triangle s[0]->s[1]->s[2], and our search is over and the algorithm is now finished.  XX

            // By construction of the simplex, the origin must always be in a voronoi region that includes the point s[2], since that
            // was the last added point. But it cannot be the case 2), since previous search took us deepest towards the direction s[1]->s[2],
            // and case 2) implies we should have been able to go even deeper in that direction, or that the origin is not included in the convex shape,
            // a case which has been checked for already before. Therefore the cases 0)-3) can never occur. Only the cases marked with XX need to be checked.

            Vector3 d12 = s[2] - s[1];
            Vector3 d02 = s[2] - s[0];
            Vector3 triNormal = Vector3.Cross(d02, d12);

            Vector3 e12 = Vector3.Cross(d12, triNormal);
            float t12 = Vector3.Dot(s[1], e12);
            if (t12 < 0)
            {
                // Case 4: Edge 1->2 is closest.

                Vector3 newDir = Vector3.Cross(d12, Vector3.Cross(d12, s[1]));
                s[0] = s[1];
                s[1] = s[2];
                n = 2;
                return newDir;
            }
            Vector3 e02 = Vector3.Cross(triNormal, d02);
            float t02 = Vector3.Dot(s[0], e02);
            if (t02 < 0)
            {
                // Case 5: Edge 0->2 is closest.

                Vector3 newDir = Vector3.Cross(d02, Vector3.Cross(d02, s[0]));
                s[1] = s[2];
                n = 2;
                return newDir;
            }
            // Cases 6)-8):

            float scaledSignedDistToTriangle = Vector3.Dot(triNormal, s[2]);
            float distSq = scaledSignedDistToTriangle * scaledSignedDistToTriangle;
            float scaledEpsilonSq = 1e-6f * triNormal.sqrMagnitude;

            if (distSq > scaledEpsilonSq)
            {
                // The origin is sufficiently far away from the triangle.
                if (scaledSignedDistToTriangle <= 0)
                    return triNormal; // Case 6)
                else
                {
                    // Case 7) Swap s[0] and s[1] so that the normal of Triangle(s[0],s[1],s[2]).PlaneCCW() will always point towards the new search direction.
                    //std::swap(s[0], s[1]);
                    GUtil.Swap(ref s[0], ref s[1]);
                    //Array.swap
                    return -triNormal;
                }
            }
            else
            {
                // Case 8) The origin lies directly inside the triangle. For robustness, terminate the search here immediately with success.
                n = 0;
                return Vector3.zero;
            }
        }
        else // n == 4
        {
            // A tetrahedron defines fifteen voronoi regions:
            //  0) closest to vertex s[0].
            //  1) closest to vertex s[1].
            //  2) closest to vertex s[2].
            //  3) closest to vertex s[3].
            //  4) closest to edge s[0]->s[1].
            //  5) closest to edge s[0]->s[2].
            //  6) closest to edge s[0]->s[3].  XX
            //  7) closest to edge s[1]->s[2].
            //  8) closest to edge s[1]->s[3].  XX
            //  9) closest to edge s[2]->s[3].  XX
            // 10) closest to the triangle s[0]->s[1]->s[2], in the outfacing side.
            // 11) closest to the triangle s[0]->s[1]->s[3], in the outfacing side. XX
            // 12) closest to the triangle s[0]->s[2]->s[3], in the outfacing side. XX
            // 13) closest to the triangle s[1]->s[2]->s[3], in the outfacing side. XX
            // 14) contained inside the tetrahedron simplex, and our search is over and the algorithm is now finished. XX

            // By construction of the simplex, the origin must always be in a voronoi region that includes the point s[3], since that
            // was the last added point. But it cannot be the case 3), since previous search took us deepest towards the direction s[2]->s[3],
            // and case 3) implies we should have been able to go even deeper in that direction, or that the origin is not included in the convex shape,
            // a case which has been checked for already before. Therefore the cases 0)-5), 7) and 10) can never occur and
            // we only need to check cases 6), 8), 9), 11), 12), 13) and 14), marked with XX.

            Vector3 d01 = s[1] - s[0];
            Vector3 d02 = s[2] - s[0];
            Vector3 d03 = s[3] - s[0];
            Vector3 tri013Normal = Vector3.Cross(d01, d03); // Normal of triangle 0->1->3 pointing outwards from the simplex.
            Vector3 tri023Normal = Vector3.Cross(d03, d02); // Normal of triangle 0->2->3 pointing outwards from the simplex.
            //assert(Dot(tri013Normal, d02) <= 0.f);
            //assert(Dot(tri023Normal, d01) <= 0.f);

            Vector3 e03_1 = Vector3.Cross(tri013Normal, d03); // The normal of edge 0->3 on triangle 013.
            Vector3 e03_2 = Vector3.Cross(d03, tri023Normal); // The normal of edge 0->3 on triangle 023.
            float inE03_1 = Vector3.Dot(e03_1, s[3]);
            float inE03_2 = Vector3.Dot(e03_2, s[3]);
            if (inE03_1 <= 0 && inE03_2 <= 0)
            {
                // Case 6) Edge 0->3 is closest. Simplex degenerates to a line segment.

                Vector3 newDir = Vector3.Cross(d03, Vector3.Cross(d03, s[3]));
                s[1] = s[3];
                n = 2;
                return newDir;
            }

            Vector3 d12 = s[2] - s[1];
            Vector3 d13 = s[3] - s[1];
            Vector3 tri123Normal = Vector3.Cross(d12, d13);
            //assert(Dot(tri123Normal, -d02) <= 0.f);
            Vector3 e13_0 = Vector3.Cross(d13, tri013Normal);
            Vector3 e13_2 = Vector3.Cross(tri123Normal, d13);
            float inE13_0 = Vector3.Dot(e13_0, s[3]);
            float inE13_2 = Vector3.Dot(e13_2, s[3]);
            if (inE13_0 <= 0 && inE13_2 <= 0)
            {
                // Case 8) Edge 1->3 is closest. Simplex degenerates to a line segment.

                Vector3 newDir = Vector3.Cross(d13, Vector3.Cross(d13, s[3]));
                s[0] = s[1];
                s[1] = s[3];
                n = 2;
                return newDir;
            }

            Vector3 d23 = s[3] - s[2];
            Vector3 e23_0 = Vector3.Cross(tri023Normal, d23);
            Vector3 e23_1 = Vector3.Cross(d23, tri123Normal);
            float inE23_0 = Vector3.Dot(e23_0, s[3]);
            float inE23_1 = Vector3.Dot(e23_1, s[3]);
            if (inE23_0 <= 0 && inE23_1 <= 0)
            {
                // Case 9) Edge 2->3 is closest. Simplex degenerates to a line segment.

                Vector3 newDir = Vector3.Cross(d23, Vector3.Cross(d23, s[3]));
                s[0] = s[2];
                s[1] = s[3];
                n = 2;
                return newDir;
            }

            float inTri013 = Vector3.Dot(s[3], tri013Normal);
            if (inTri013 < 0 && inE13_0 >= 0 && inE03_1 >= 0)
            {
                // Case 11) Triangle 0->1->3 is closest.

                s[2] = s[3];
                n = 3;
                return tri013Normal;
            }
            float inTri023 = Vector3.Dot(s[3], tri023Normal);
            if (inTri023 < 0 && inE23_0 >= 0 && inE03_2 >= 0)
            {
                // Case 12) Triangle 0->2->3 is closest.

                s[1] = s[0];
                s[0] = s[2];
                s[2] = s[3];
                n = 3;
                return tri023Normal;
            }
            float inTri123 = Vector3.Dot(s[3], tri123Normal);
            if (inTri123 < 0 && inE13_2 >= 0 && inE23_1 >= 0)
            {
                // Case 13) Triangle 1->2->3 is closest.

                s[0] = s[1];
                s[1] = s[2];
                s[2] = s[3];
                n = 3;
                return tri123Normal;
            }

            // Case 14) Not in the voronoi region of any triangle or edge. The origin is contained in the simplex, the search is finished.
            n = 0;
            return Vector3.zero;
        }
    }

    #endregion
}

