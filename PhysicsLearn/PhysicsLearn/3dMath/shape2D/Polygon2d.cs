using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Polygon2d
{
    public Vector2 m_pos = Vector2.zero;
    // 假设为xz平面的2d物体
    public Quaternion m_rotation = Quaternion.identity;

    /// <summary>
    /// 需要最后一个点和第一个点不重合
    /// </summary>
    public List<Vector2> m_pointList = new List<Vector2>();

    public List<Vector2> GetWorldPosList()
    {
        List<Vector2> ret = new List<Vector2>();
        Vector3 pos3d = new Vector3(m_pos.x, 0, m_pos.y);
        Matrix4x4 trs = Matrix4x4.TRS(pos3d, m_rotation, Vector3.one);
        foreach(var iter in m_pointList)
        {
            Vector3 objPoint3d = new Vector3(iter.x, 0, iter.y);
            Vector3 worldPoint3d = trs * objPoint3d;
            ret.Add(new Vector2(worldPoint3d.x, worldPoint3d.z));
        }
        return ret;
    }

    public List<Vector2> GetRotateList()
    {
        List<Vector2> ret = new List<Vector2>();
        Matrix4x4 trs = Matrix4x4.TRS(Vector3.zero, m_rotation, Vector3.one);
        foreach (var iter in m_pointList)
        {
            Vector3 objPoint3d = new Vector3(iter.x, 0, iter.y);
            Vector3 worldPoint3d = trs * objPoint3d;
            ret.Add(new Vector2(worldPoint3d.x, worldPoint3d.z));
        }
        return ret;
    }
}

