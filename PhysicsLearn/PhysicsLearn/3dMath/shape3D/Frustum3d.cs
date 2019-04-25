using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public struct Frustum3d8
{
    public Vector3 m_nearLeftTop;
    public Vector3 m_nearRightTop;
    public Vector3 m_nearRightBottom;
    public Vector3 m_nearLeftBottom;

    public Vector3 m_farLeftTop;
    public Vector3 m_farRightTop;
    public Vector3 m_farRightBottom;
    public Vector3 m_farLeftBottom;
}

public struct Frustum3d
{
    public Plane3d[] m_planeArray;

    public Frustum3d(Plane3d[] planeArray)
    {
        m_planeArray = planeArray;
    }

    public bool IsInFrustum(Vector3 point)
    {
        //         float ret1 = Plane3d.PlaneEquation(point, m_leftPlane);
        //         float ret2 = Plane3d.PlaneEquation(point, m_rightPlane);
        //         float ret3 = Plane3d.PlaneEquation(point, m_topPlane);
        //         float ret4 = Plane3d.PlaneEquation(point, m_bottomPlane);
        //         float ret5 = Plane3d.PlaneEquation(point, m_nearPlane);
        //         float ret6 = Plane3d.PlaneEquation(point, m_farPlane);
        // 
        //         return ret1 > 0 && ret2 > 0 && ret3 > 0 && ret4 > 0 && ret5 > 0 && ret6 > 0;
        foreach (var iter in m_planeArray)
        {
            if (Plane3d.PlaneEquation(point, iter) <= 0)
            {
                return false;
            }
        }

        return true;
    }

//     public bool IsInOrAgainstFrustum(HitShape3dData data)
//     {
//         if (data.m_type == HitShape3dType.Sphere)
//         {
//             //Sphere3d sphere = new Sphere3d(data.GetPos(), data.m_sphere.m_radius);
//             return IntersectionTest3D.Frustum3dWithSphere3d(new Frustum3d(m_planeArray), data.m_sphere);
//         }
//         else if (data.m_type == HitShape3dType.OBB)
//         {
//             //OBB3d obb = new OBB3d(data.GetPos(), data.m_obb.m_rotation, data.m_obb.m_size);
//             return IntersectionTest3D.Frustum3dWithOBB3d(new Frustum3d(m_planeArray), data.m_obb);
//         }
//         else
//         {
//             return false;
//         }
//     }

}

