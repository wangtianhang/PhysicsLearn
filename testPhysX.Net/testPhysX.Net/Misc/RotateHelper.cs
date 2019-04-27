using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

class RotateHelper
{
    /// <summary>
    /// 从rotation获取方向
    /// </summary>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static Vector3 GetForward(Quaternion rotation)
    {
        return rotation * Vector3.forward;
    }

//     public static Vector3 GetForward(Quaternion rotation)
//     {
//         return rotation * Vector3.forward;
//     }

//     public static Vector3 GetForward(Quaternion rotation)
//     {
//         return rotation * Vector3.forward;
//     }

    public static Vector3 GetUp(Quaternion rotation)
    {
        return rotation * Vector3.up;
    }

//     public static Vector3 GetUp(Quaternion rotation)
//     {
//         return rotation * Vector3.up;
//     }

//     public static Vector3 GetUp(Quaternion rotation)
//     {
//         return rotation * Vector3.up;
//     }

    public static Vector3 GetRight(Quaternion rotation)
    {
        return rotation * Vector3.right;
    }

//     public static Vector3 GetRight(Quaternion rotation)
//     {
//         return rotation * Vector3.right;
//     }

//     public static Vector3 GetRight(Quaternion rotation)
//     {
//         return rotation * Vector3.right;
//     }

    /// <summary>
    /// 从水平方向转换rotation
    /// </summary>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static Quaternion LookAt(Vector3 dir)
    {
        return Quaternion.LookRotation(dir, Vector3.up);
    }

//     public static Quaternion LookAt(Vector3 dir)
//     {
//         return Quaternion.LookRotation(dir, Vector3.up);
//     }

//     public static Quaternion LookAt(Vector3 dir)
//     {
//         return Quaternion.LookRotation(dir, Vector3.up);
//     }

    /// <summary>
    /// 各个方向转rotation
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    [Obsolete("和transform.forward赋值 误差很大，少用，原因不明")]
    public static Quaternion _DirectionToRotation(Vector3 dir)
    {
        return Quaternion.FromToRotation(Vector3.forward, dir);
    }

//     [Obsolete("和transform.forward赋值 误差很大，少用，原因不明")]
//     public static Quaternion _DirectionToRotation(Vector3 dir)
//     {
//         return Quaternion.FromToRotation(Vector3.forward, dir);
//     }
 
//     [Obsolete("和transform.forward赋值 误差很大，少用，原因不明")]
//     public static Quaternion _DirectionToRotation(Vector3 dir)
//     {
//         return Quaternion.FromToRotation(Vector3.forward, dir);
//     }

    /// <summary>
    /// 从矩阵中提取Quaternion
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    public static Quaternion GetRotationFromMatrix(Matrix4x4 m)
    {
        return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
    }

//     public static Quaternion GetRotationFromMatrix(Matrix4x4 m)
//     {
//         return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
//     }

//     public static Quaternion GetRotationFromMatrix(Matrix4x4 m)
//     {
//         return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
//     }

//     public static void DecompositeQuaternion(Quaternion rotate, ref Vector3 forward, ref Vector3 up, ref Vector3 right)
//     {
// 
//     }
}

