using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MathHelper
{
//     public static Vector3 Convert3dPosToNguiPos(Vector3 pos3d, Camera mainCamera, Camera nguiCamera)
//     {
//         if (mainCamera == null
//             || nguiCamera == null)
//         {
//             return Vector3.zero;
//         }
// 
//         Vector3 nguiPos = Vector3.zero;
// 
//         Vector3 screenPos = mainCamera.WorldToScreenPoint(pos3d);
//         screenPos.z = 0;
//         nguiPos = nguiCamera.ScreenToWorldPoint(screenPos);
// 
//         return nguiPos;
//     }

    public static bool GetSegmentIntersection2D(Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4, ref Vector3 result)
    {
        float resultX = 0;
        float resultY = 0;
        bool ret = get_line_intersection(pos1.x, pos1.z, pos2.x, pos2.z, pos3.x, pos3.z, pos4.x, pos4.z,
            ref resultX, ref resultY);
        result.x = resultX;
        result.z = resultY;
        return ret;
    }

//     public static bool GetSegmentIntersection2D(Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4, ref Vector3 result)
//     {
//         float resultX = 0;
//         float resultY = 0;
//         bool ret = get_line_intersection(pos1.x, pos1.z, pos2.x, pos2.z, pos3.x, pos3.z, pos4.x, pos4.z,
//             ref resultX, ref resultY);
//         result.x = resultX;
//         result.z = resultY;
//         return ret;
//     }

    static bool get_line_intersection(float p0_x, float p0_y, float p1_x, float p1_y,
        float p2_x, float p2_y, float p3_x, float p3_y, ref float i_x, ref float i_y)
    {
        float s1_x, s1_y, s2_x, s2_y;
        s1_x = p1_x - p0_x; s1_y = p1_y - p0_y;
        s2_x = p3_x - p2_x; s2_y = p3_y - p2_y;

        float s, t;
        s = (-s1_y * (p0_x - p2_x) + s1_x * (p0_y - p2_y)) / (-s2_x * s1_y + s1_x * s2_y);
        t = (s2_x * (p0_y - p2_y) - s2_y * (p0_x - p2_x)) / (-s2_x * s1_y + s1_x * s2_y);

        if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
        {
            // Collision detected
            i_x = p0_x + (t * s1_x);
            i_y = p0_y + (t * s1_y);
            return true;
        }

        return false; // No collision
    }

//     static bool get_line_intersection(float p0_x, float p0_y, float p1_x, float p1_y,
//     float p2_x, float p2_y, float p3_x, float p3_y, ref float i_x, ref float i_y)
//     {
//         float s1_x, s1_y, s2_x, s2_y;
//         s1_x = p1_x - p0_x; s1_y = p1_y - p0_y;
//         s2_x = p3_x - p2_x; s2_y = p3_y - p2_y;
// 
//         float s, t;
//         s = (-s1_y * (p0_x - p2_x) + s1_x * (p0_y - p2_y)) / (-s2_x * s1_y + s1_x * s2_y);
//         t = (s2_x * (p0_y - p2_y) - s2_y * (p0_x - p2_x)) / (-s2_x * s1_y + s1_x * s2_y);
// 
//         if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
//         {
//             // Collision detected
//             i_x = p0_x + (t * s1_x);
//             i_y = p0_y + (t * s1_y);
//             return true;
//         }
// 
//         return false; // No collision
//     }

    /// <summary>
    /// 获得预测的命中时间
    /// https://www.cnblogs.com/softimagewht/p/3756379.html
    /// 核心思路为根据 余弦公式 联立一元二次方程组
    /// 因为子弹当帧不移动，所以计算命中一帧后的target位置
    /// </summary>
    /// <param name="srcPos"></param>
    /// <param name="srcSpeed"></param>
    /// <param name="targetCurPos"></param>
    /// <param name="targetDir"></param>
    /// <param name="targetSpeed"></param>
    /// <returns></returns>
    public static bool GetPredicateTime(Vector3 srcPos, float srcSpeed, Vector3 curTargetPos, Vector3 targetDir, float targetSpeed, ref float time)
    {
        if (targetSpeed == 0)
        {
            time = (curTargetPos - srcPos).magnitude / srcSpeed;
            return true;
        }

        float p2 = srcSpeed;
        float p1 = targetSpeed;
        float sideC = (curTargetPos - srcPos).magnitude;

        Vector3 tmpPos = curTargetPos + targetDir * 1;
        float cosTheta = Vector3.Dot((srcPos - curTargetPos), (tmpPos - curTargetPos)) / (srcPos - curTargetPos).magnitude / (tmpPos - curTargetPos).magnitude;
        //float thetaDegree = Mathf.Acos(cosTheta) * Mathf.Rad2Deg;

        float a = 1 - (p2 / p1) * (p2 / p1);
        float b = -2 * sideC * cosTheta;
        float c = sideC * sideC;

        float sideB1 = 0;
        float sideB2 = 0;
        if (a != 0)
        {
            // 解一元二次方程组
            sideB1 = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
            sideB2 = (-b - Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
        }
        else
        {
            // 解一元一次方程组
            sideB1 = -c / b;
            sideB2 = -c / b;
        }

        float sideB = 0;
        if (sideB1 > 0)
        {
            sideB = sideB1;
        }
        else if (sideB2 > 0)
        {
            sideB = sideB2;
        }
        else
        {
            //Debug.Log("GetPredicateTime 无解");
            return false;
        }
        float tb = sideB / p1;

        float sideA = 0;
        float ta;
        if (sideB > 0)
        {
            sideA = Mathf.Sqrt(sideB * sideB + sideC * sideC - cosTheta * 2 * sideB * sideC);
            ta = sideA / p2;
        }

        //float t1 = sideB1 / p1;
        //float t2 = sideB2 / p1;

        //         Debug.Log("t1 " + t1 + " t2 " + t2);
        //         if(t1 > 0)
        //         {
        //             return t1;
        //         }
        //         else if(t2 > 0)
        //         {
        //             return t2;
        //         }
        //         else
        //         {
        //             Debug.Log("GetPredicateTime 无解");
        //             return 0;
        //         }
        time = tb;
        return true;
    }

    public class PathFuncBase
    {
        public virtual Vector3 GetPoint(float time)
        {
            return Vector3.zero;
        }
    }

//     public class FlyGo2Pos : PathFuncBase
//     {
//         public FlyGo2 m_flyGo;
// 
//         public void Init(FlyGo2 flyGo)
//         {
//             m_flyGo = flyGo;
//         }
// 
//         public override Vector3 GetPoint(float time)
//         {
//             return m_flyGo.m_transform.position + m_flyGo.m_transform.GetForward() * m_flyGo.GetForwardSpeed() * time;
//         }
//     }
// 
//     public class AnimationPos : PathFuncBase
//     {
//         GameObject m_go = null;
//         AnimationClip m_anim = null;
//         float m_baseTime = 0;
//         Vector3 m_offset = Vector3.zero;
//         FlyGo2Pos m_flyGo2Pos = null;
// 
//         public void Init(FlyGo2Pos flyGo2Pos, GameObject go, AnimationClip anim, float baseTime, Vector3 offset)
//         {
//             m_flyGo2Pos = flyGo2Pos;
//             m_go = go;
//             m_anim = anim;
//             m_baseTime = baseTime;
//             m_offset = offset;
//         }
// 
//         public override Vector3 GetPoint(float time)
//         {
//             m_anim.SampleAnimation(m_go, m_baseTime + (float)time);
//             return m_flyGo2Pos.GetPoint(time) + m_go.transform.position + m_offset;
//         }
//     }

    public class FunctionOfOneVariableD
    {
        public virtual float GetValue(float param)
        {
            return 0;
        }
    }

    public class PathDistanceFunc : FunctionOfOneVariableD
    {
        public PathFuncBase m_pathFunc1;
        //public PathFuncBase m_pahtFunc2;
        public Vector3 m_srcPos;
        public float m_speed;
        public override float GetValue(float time)
        {
            Vector3 animationPos = m_pathFunc1.GetPoint(time);
            Vector3 bulletPos = m_srcPos + (animationPos - m_srcPos).normalized * m_speed * time;
            return (animationPos - bulletPos).sqrMagnitude;
        }
    }

    public static bool ResolvePathFunc(PathFuncBase path1, Vector3 srcPos, float speed, float leftTime, float rightTime, ref float time)
    {
        //int maxLoopCount = 100;
        PathDistanceFunc func = new PathDistanceFunc();
        func.m_pathFunc1 = path1;
        //func.m_pahtFunc2 = path2;
        func.m_srcPos = srcPos;
        func.m_speed = speed;

        float step = (rightTime - leftTime) / 100;
        float curStep = leftTime;
        float minDistance = float.MaxValue;
        float minTime = 0;
        for (int i = 0; i < 100; ++i)
        {
            float sqrDistance = func.GetValue(curStep);
            if (sqrDistance < minDistance)
            {
                minDistance = sqrDistance;
                minTime = curStep;
            }
            curStep += step;
            //Debug.Log("distance " + Mathf.Sqrt(sqrDistance) + " time " + curStep);
        }

        //         float minTime = GetMinByThree(func, leftTime, rightTime);
        //         float sqrDistance = func.GetValue(minTime);
        if (minDistance < 0.5f)
        {
            time = minTime;
            return true;
        }
        else
        {
            return false;
        }

    }

    /// <summary>
    /// 三分法计算极值
    /// </summary>
    /// <param name="function"></param>
    /// <param name="x"></param>
    /// <returns></returns>
    //     public static float GetMinByThree(FunctionOfOneVariableD function, float left, float right)
    //     {
    //         do
    //         {
    //             float yLeft = function.GetValue(left);
    //             float yRight = function.GetValue(right);
    //             float mid = (left + right) / 2;
    //             float yMid = function.GetValue(mid);
    //             float midMid = (left + mid) / 2;
    //             float yMidMid = function.GetValue(midMid);
    //             if (yMidMid > yMid)
    //             {
    //                 left = midMid;
    //             }
    //             else
    //             {
    //                 right = mid;
    //             }
    // 
    //             if (Math.Abs(left - right) < 0.1d)
    //             {
    //                 return left;
    //             }
    // 
    //         } while (true);
    //     }

    // 外插值
    public static Vector3 Extrapoloation(Vector3 from, Vector3 to, float t)
    {
        if (t < 0)
        {
            t = 0;
        }
        return new Vector3(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t, from.z + (to.z - from.z) * t);
    }

    /// <summary>
    /// 是否在前方 这个算法应该不太对 但历史代码先用着 新代码别用了
    /// </summary>
    /// <param name="srcPos"></param>
    /// <param name="right"></param>
    /// <param name="judgePoint"></param>
    /// <returns></returns>
    public static bool _IsAtFront(Vector3 srcPos, Vector3 right, Vector3 judgePoint)
    {
        Vector3 dir = judgePoint - srcPos;
        Vector3 crossDir = Vector3.Cross(dir, right);
        return crossDir.y > 0;
    }

    /// <summary> 
    /// 是否在左方 这个算法应该不太对 但历史代码先用着 新代码别用了
    /// </summary>
    /// <param name="srcPos"></param>
    /// <param name="forward"></param>
    /// <param name="judgePoint"></param>
    /// <returns></returns>
    public static bool _IsAtLeft(Vector3 srcPos, Vector3 forward, Vector3 judgePoint)
    {
        Vector3 dir = judgePoint - srcPos;
        Vector3 crossDir = Vector3.Cross(dir, forward);
        return crossDir.y > 0;
    }

    /// <summary>
    /// 是否在右方 这个算法应该不太对 但历史代码先用着 新代码别用了
    /// </summary>
    /// <param name="srcPos"></param>
    /// <param name="forward"></param>
    /// <param name="judgePoint"></param>
    /// <returns></returns>
    public static bool _IsAtRight(Vector3 srcPos, Vector3 forward, Vector3 judgePoint)
    {
        return !_IsAtLeft(srcPos, forward, judgePoint);
    }

    /// <summary>
    /// 正确的正前方判定代码
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="judgePoint"></param>
    /// <returns></returns>
    public static bool IsAtFront(Transform transform, Vector3 judgePoint)
    {
        Vector3 localPos = transform.worldToLocalMatrix.MultiplyPoint(judgePoint);
        if(localPos.z > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static Vector3 GetPointToSegmentDir(Vector3 srcPoint, Segment3d segment)
    {
        Vector3 closestPoint = Distance3d.ClosestPointOfPoint3dWithSegment3d(srcPoint, segment);
        return closestPoint - srcPoint;
    }

    public static Vector3 ConvertToVector3L(Vector3 src)
    {
        return src;
    }

    public static Frustum3d8 CreateFrustum3d8(float nearPlaneDistance, float farPlaneDistance,
        float nearPlaneWidth, float nearPlaneHeight,
        float farPlaneWidth, float farPlaneHeight,
        Vector3 localCenterOffset, Matrix4x4 localToWorld, bool debug = false)
    {
        float nearHalfX = nearPlaneWidth / 2;
        float nearHalfY = nearPlaneHeight / 2;

        Vector3 nearCenter = localCenterOffset + Vector3.forward * nearPlaneDistance;

        Vector3 nearLeftTop = nearCenter + new Vector3(-nearHalfX, nearHalfY, 0);
        Vector3 nearRightTop = nearCenter + new Vector3(+nearHalfX, nearHalfY, 0);
        Vector3 nearRightBottom = nearCenter + new Vector3(+nearHalfX, -nearHalfY, 0);
        Vector3 nearLeftBottom = nearCenter + new Vector3(-nearHalfX, -nearHalfY, 0);

        Plane3d nearPlane = new Plane3d(Vector3.Cross(nearLeftTop - nearRightTop, nearRightBottom - nearRightTop), nearRightTop);
        Plane3d farPlane = new Plane3d(-Vector3.forward, Vector3.forward * farPlaneDistance);

        /*
         * 远截面的部分采用配置的方式
        Ray3d rayLeftTop = new Ray3d(Vector3.zero, nearLeftTop);
        Ray3d rayRightTop = new Ray3d(Vector3.zero, nearRightTop);
        Ray3d rayRightBottom = new Ray3d(Vector3.zero, nearRightBottom);
        Ray3d rayLeftBottom = new Ray3d(Vector3.zero, nearLeftBottom);

        IntersectionTest3D.RaycastResult raycastResult = new IntersectionTest3D.RaycastResult();
        IntersectionTest3D.Ray3dWithPlane3d(farPlane, rayLeftTop, ref raycastResult);
        Vector3 farLeftTop = raycastResult.m_point;
        IntersectionTest3D.Ray3dWithPlane3d(farPlane, rayRightTop, ref raycastResult);
        Vector3 farRightTop = raycastResult.m_point;
        IntersectionTest3D.Ray3dWithPlane3d(farPlane, rayRightBottom, ref raycastResult);
        Vector3 farRightBottom = raycastResult.m_point;
        IntersectionTest3D.Ray3dWithPlane3d(farPlane, rayLeftBottom, ref raycastResult);
        Vector3 farLeftBottom = raycastResult.m_point;
        */

        float farHalfX = farPlaneWidth / 2;
        float farHalfY = farPlaneHeight / 2;
        Vector3 farCenter = localCenterOffset + Vector3.forward * farPlaneDistance;
        Vector3 farLeftTop = farCenter + new Vector3(-farHalfX, farHalfY, 0);
        Vector3 farRightTop = farCenter + new Vector3(+farHalfX, farHalfY, 0);
        Vector3 farRightBottom = farCenter + new Vector3(+farHalfX, -farHalfY, 0);
        Vector3 farLeftBottom = farCenter + new Vector3(-farHalfX, -farHalfY, 0);

        //Matrix4x4 localToWorld = transform.localToWorldMatrix;
        nearLeftTop = localToWorld.MultiplyPoint(nearLeftTop);
        nearRightTop = localToWorld.MultiplyPoint(nearRightTop);
        nearRightBottom = localToWorld.MultiplyPoint(nearRightBottom);
        nearLeftBottom = localToWorld.MultiplyPoint(nearLeftBottom);

        farLeftTop = localToWorld.MultiplyPoint(farLeftTop);
        farRightTop = localToWorld.MultiplyPoint(farRightTop);
        farRightBottom = localToWorld.MultiplyPoint(farRightBottom);
        farLeftBottom = localToWorld.MultiplyPoint(farLeftBottom);

        Frustum3d8 ret = new Frustum3d8();
        ret.m_nearLeftTop = nearLeftTop;
        ret.m_nearRightTop = nearRightTop;
        ret.m_nearRightBottom = nearRightBottom;
        ret.m_nearLeftBottom = nearLeftBottom;
        ret.m_farLeftTop = farLeftTop;
        ret.m_farRightTop = farRightTop;
        ret.m_farRightBottom = farRightBottom;
        ret.m_farLeftBottom = farLeftBottom;

        if (debug)
        {
            //             nearLeftBottom = trs.MultiplyPoint(nearLeftBottom);
            //             nearLeftTop = trs.MultiplyPoint(nearLeftTop);
            //             nearRightTop = trs.MultiplyPoint(nearRightTop);
            //             nearRightBottom = trs.MultiplyPoint(nearRightBottom);
            // 
            //             farLeftBottom = trs.MultiplyPoint(farLeftBottom);
            //             farLeftTop = trs.MultiplyPoint(farLeftTop);
            //             farRightTop = trs.MultiplyPoint(farRightTop);
            //             farRightBottom = trs.MultiplyPoint(farRightBottom);

            Debug.DrawLine(ret.m_nearLeftBottom, ret.m_nearLeftTop, Color.red);
            Debug.DrawLine(ret.m_nearLeftTop, ret.m_nearRightTop, Color.red);
            Debug.DrawLine(ret.m_nearRightTop, ret.m_nearRightBottom, Color.red);
            Debug.DrawLine(ret.m_nearRightBottom, ret.m_nearLeftBottom, Color.red);

            Debug.DrawLine(ret.m_farLeftBottom, ret.m_farLeftTop, Color.red);
            Debug.DrawLine(ret.m_farLeftTop, ret.m_farRightTop, Color.red);
            Debug.DrawLine(ret.m_farRightTop, ret.m_farRightBottom, Color.red);
            Debug.DrawLine(ret.m_farRightBottom, ret.m_farLeftBottom, Color.red);

            Debug.DrawLine(ret.m_nearLeftBottom, ret.m_farLeftBottom, Color.red);
            Debug.DrawLine(ret.m_nearLeftTop, ret.m_farLeftTop, Color.red);
            Debug.DrawLine(ret.m_nearRightTop, ret.m_farRightTop, Color.red);
            Debug.DrawLine(ret.m_nearRightBottom, ret.m_farRightBottom, Color.red);
        }

        return ret;
    }

    public static void ConvertFrustum3d8ToFrustum3d(Frustum3d8 frustum, ref Frustum3d output)
    {
        Plane3d nearPlane = new Plane3d(Vector3.Cross(frustum.m_nearLeftTop - frustum.m_nearRightTop, frustum.m_nearRightBottom - frustum.m_nearRightTop), frustum.m_nearRightTop);
        Plane3d farPlane = new Plane3d(Vector3.Cross(frustum.m_farRightTop - frustum.m_farLeftTop, frustum.m_farLeftBottom - frustum.m_farLeftTop), frustum.m_farLeftTop);

        Plane3d leftPlane = new Plane3d(Vector3.Cross(frustum.m_nearLeftTop - frustum.m_nearLeftBottom, frustum.m_farLeftBottom - frustum.m_nearLeftBottom), frustum.m_nearLeftBottom);
        Plane3d rightPlane = new Plane3d(Vector3.Cross(frustum.m_farRightBottom - frustum.m_nearRightBottom, frustum.m_nearRightTop - frustum.m_nearRightBottom), frustum.m_nearRightBottom);

        Plane3d topPlane = new Plane3d(Vector3.Cross(frustum.m_farRightTop - frustum.m_nearRightTop, frustum.m_nearLeftTop - frustum.m_nearRightTop), frustum.m_nearRightTop);
        Plane3d bottomPlane = new Plane3d(Vector3.Cross(frustum.m_farLeftBottom - frustum.m_nearLeftBottom, frustum.m_nearRightBottom - frustum.m_nearLeftBottom), frustum.m_nearLeftBottom);

        output.m_planeArray[0] = nearPlane;
        output.m_planeArray[1] = farPlane;
        output.m_planeArray[2] = leftPlane;
        output.m_planeArray[3] = rightPlane;
        output.m_planeArray[4] = topPlane;
        output.m_planeArray[5] = bottomPlane;
    }

//     public static Frustum3d CreateFrustum3d(float nearPlaneDistance, float farPlaneDistance,
//         float nearPlaneWidth, float nearPlaneHeight,
//         float farPlaneWidth, float farPlaneHeight,
//         Vector3 localCenterOffset, Transform transform, bool debug = false)
//     {
//         //         float nearHalfX = nearPlaneWidth / 2;
//         //         float nearHalfY = nearPlaneHeight / 2;
//         // 
//         //         Vector3 nearCenter = localCenterOffset + Vector3.forward * nearPlaneDistance;
//         // 
//         //         Vector3 nearLeftTop = nearCenter + new Vector3(-nearHalfX, nearHalfY, 0);
//         //         Vector3 nearRightTop = nearCenter + new Vector3(+nearHalfX, nearHalfY, 0);
//         //         Vector3 nearRightBottom = nearCenter + new Vector3(+nearHalfX, -nearHalfY, 0);
//         //         Vector3 nearLeftBottom = nearCenter + new Vector3(-nearHalfX, -nearHalfY, 0);
//         // 
//         //         Plane3d nearPlane = new Plane3d(Vector3.Cross(nearLeftTop - nearRightTop, nearRightBottom - nearRightTop), nearRightTop);
//         //         Plane3d farPlane = new Plane3d(-Vector3.forward, Vector3.forward * farPlaneDistance);
//         // 
//         //         Ray3d rayLeftTop = new Ray3d(Vector3.zero, nearLeftTop );
//         //         Ray3d rayRightTop = new Ray3d(Vector3.zero, nearRightTop );
//         //         Ray3d rayRightBottom = new Ray3d(Vector3.zero, nearRightBottom );
//         //         Ray3d rayLeftBottom = new Ray3d(Vector3.zero, nearLeftBottom );
//         // 
//         //         IntersectionTest3D.RaycastResult raycastResult = new IntersectionTest3D.RaycastResult();
//         //         IntersectionTest3D.Ray3dWithPlane3d(farPlane, rayLeftTop, ref raycastResult);
//         //         Vector3 farLeftTop = raycastResult.m_point;
//         //         IntersectionTest3D.Ray3dWithPlane3d(farPlane, rayRightTop, ref raycastResult);
//         //         Vector3 farRightTop = raycastResult.m_point;
//         //         IntersectionTest3D.Ray3dWithPlane3d(farPlane, rayRightBottom, ref raycastResult);
//         //         Vector3 farRightBottom = raycastResult.m_point;
//         //         IntersectionTest3D.Ray3dWithPlane3d(farPlane, rayLeftBottom, ref raycastResult);
//         //         Vector3 farLeftBottom = raycastResult.m_point;
//         Frustum3d8 frustum = CreateFrustum3d8(nearPlaneDistance, farPlaneDistance, nearPlaneWidth, nearPlaneHeight,
//             farPlaneWidth, farPlaneHeight, localCenterOffset, transform.localToWorldMatrix, debug);
// 
// 
// 
//         //         Plane3d[] planes = new Plane3d[6];
//         //         planes[0] = nearPlane;
//         //         planes[1] = farPlane;
//         //         planes[2] = leftPlane;
//         //         planes[3] = rightPlane;
//         //         planes[4] = topPlane;
//         //         planes[5] = bottomPlane;
//         Frustum3d frustum3d = new Frustum3d(new Plane3d[6]);
// 
// 
//         //         Matrix4x4 trs = transform.localToWorldMatrix;
//         //         for (int i = 0; i < planes.Length; ++i)
//         //         {
//         //             Plane3d iter = planes[i];
//         //             iter.m_planeNormal = trs.MultiplyVector(iter.m_planeNormal);
//         //             iter.m_planeOnePoint = trs.MultiplyPoint(iter.m_planeOnePoint);
//         //             planes[i] = iter;
//         //         }
// 
// 
// 
//         //Debug.Break();
// 
//         return frustum3d;
//     }

//     public static OBB3d GetOBB(BoxCollider collider)
//     {
//         if (collider == null)
//         {
//             return new OBB3d();
//         }
// 
//         BoxCollider boxCollider = collider as BoxCollider;
// 
//         Matrix4x4 mat = Matrix4x4.TRS(collider.transform.position, collider.transform.rotation, Vector3.one);
//         //Vector3 euler = transform.localEulerAngles;
//         Matrix4x4 matlocal = Matrix4x4.TRS(boxCollider.center, Quaternion.identity, Vector3.one);
//         Matrix4x4 totalMat = mat * matlocal;
//         Vector3 center = totalMat.MultiplyPoint(Vector3.zero);
// 
//         return new OBB3d(center, collider.transform.rotation, collider.size);
//     }

    public static bool Approximately(Vector3 a, Vector3 b)
    {
        return Mathf.Approximately(a.x, b.x)
            && Mathf.Approximately(a.y, b.y)
            && Mathf.Approximately(a.z, b.z);
    }

    public struct WeightItem
    {
        public WeightItem(System.Object data, int weight)
        {
            m_data = data;
            m_weight = weight;
        }
        public System.Object m_data;
        public int m_weight;
    }

    public static System.Object RandomWeightList(List<WeightItem> itemList, out int outRandom)
    {
        if(itemList.Count == 0)
        {
            outRandom = -1;
            return null;
        }

        int totalWeight = 0;
        foreach (var iter in itemList)
        {
            totalWeight += iter.m_weight;
        }
        outRandom = Common.Random.Range(1, totalWeight + 1);
        int sumHelper = 0;
        for (int i = 0; i < itemList.Count; ++i)
        {
            int nextStep = GetNextStep(sumHelper, i, itemList, totalWeight);
            if (outRandom > sumHelper
                && outRandom <= nextStep)
            {
                return itemList[i].m_data;
            }
            sumHelper += itemList[i].m_weight;
        }
        return null;
    }

    public static void TestRandomWeightList()
    {
        WeightItem item1 = new WeightItem("1", 4);
        WeightItem item2 = new WeightItem("2", 1);
        WeightItem item3 = new WeightItem("3", 1);
        WeightItem item4 = new WeightItem("4", 1);

        List<WeightItem> itemList1 = new List<WeightItem>();
        itemList1.Add(item1);
        int outRandom = 0;
        Debug.Log("TestRandomWeightList1 " +  RandomWeightList(itemList1, out outRandom));
        List<WeightItem> itemList2 = new List<WeightItem>();
        itemList2.Add(item1);
        itemList2.Add(item2);
        itemList2.Add(item3);
        itemList2.Add(item4);

        for (int j = 0; j < 1; ++j)
        {
            //Debug.Log("TestRandomWeightList2 " + RandomWeightList(itemList2));
            int totalWeight = 0;
            foreach (var iter in itemList2)
            {
                totalWeight += iter.m_weight;
            }
            //int random = Common.Random.Range(1, totalWeight + 1);
            int random = 7;
            int sumHelper = 0;
            for (int i = 0; i < itemList2.Count; ++i)
            {
                if (random > sumHelper
                    && random <= GetNextStep(sumHelper, i, itemList2, totalWeight))
                {
                    Debug.Log("TestRandomWeightList2 " + itemList2[i].m_data);
                    break;
                }
                sumHelper += itemList2[i].m_weight;
            }
        }

        WeightItem item21 = new WeightItem("1", 4);
        WeightItem item22 = new WeightItem("2", 5);
        WeightItem item23 = new WeightItem("3", 1);
        WeightItem item24 = new WeightItem("4", 1);
        WeightItem item25 = new WeightItem("5", 1);
        List<WeightItem> itemList3 = new List<WeightItem>();
        itemList3.Add(item21);
        itemList3.Add(item22);
        itemList3.Add(item23);
        itemList3.Add(item24);
        itemList3.Add(item25);

        {
            int totalWeight = 0;
            foreach (var iter in itemList3)
            {
                totalWeight += iter.m_weight;
            }
            //int random = Common.Random.Range(1, totalWeight + 1);
            int random = 8;
            int sumHelper = 0;
            for (int i = 0; i < itemList3.Count; ++i)
            {
                int nextStep = GetNextStep(sumHelper, i, itemList3, totalWeight);
                if (random > sumHelper
                    && random <= nextStep)
                {
                    Debug.Log("TestRandomWeightList3 " + itemList3[i].m_data);
                    break;
                }
                sumHelper += itemList3[i].m_weight;
            }
        }

    }

    public static int GetNextStep(int curSum, int i, List<WeightItem> itemList, int totalWeight)
    {
        if(i < itemList.Count -1)
        {
            return curSum + itemList[i].m_weight;
        }
        else
        {
            return totalWeight;
        }
    }

    public static Vector3 Divide(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    public static Vector2 Divide(Vector2 a, Vector2 b)
    {
        return new Vector2(a.x / b.x, a.y / b.y);
    }

    public static Vector3 PingPong(Vector3 a, Vector3 b, float t)
    {
        float modt = t % 2;
        //t = t % 2;
        if (modt < 1)
        {
            return Vector3.Lerp(a, b, modt);
        }
        else
        {
            return Vector3.Lerp(b, a, modt - 1);
        }
    }

    public static bool Approximately(Vector4 a, Vector4 b, float t)
    {
        return Mathf.Approximately(a.x, b.x)
            && Mathf.Approximately(a.y, b.y)
            && Mathf.Approximately(a.z, b.z)
            && Mathf.Approximately(a.w, b.w);
    }
}

