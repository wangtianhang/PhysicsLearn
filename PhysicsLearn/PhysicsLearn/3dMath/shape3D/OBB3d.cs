using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public struct OBB3d
{
    public Vector3 m_pos;
    public Quaternion m_rotation;
    //public float m_xLength;
    //public float m_yLength;
    //public float m_zLength;
    //public Vector3 m_size = Vector3.one;
    public Vector3 m_size;

    public OBB3d(Vector3 pos, Quaternion qua, float xLength, float yLength, float zLength)
    {
        m_pos = pos;
        m_rotation = qua;
        //m_xLength = xLength;
        //m_yLength = yLength;
        //m_zLength = zLength;
        m_size.x = xLength;
        m_size.y = yLength;
        m_size.z = zLength;
    }

    public OBB3d(Vector3 pos, Quaternion qua, Vector3 size)
    {
        m_pos = pos;
        m_rotation = qua;

        //m_xLength = size.x;
        //m_yLength = size.y;
        //m_zLength = size.z;
        m_size = size;
    }

//     public Vector3 GetAABBMin()
//     {
//         return new Vector3(m_pos.x - m_xLength * 0.5f, m_pos.y - m_yLength * 0.5f, m_pos.z - m_zLength * 0.5f);
//     }
// 
//     public Vector3 GetAABBMax()
//     {
//         return new Vector3(m_pos.x + m_xLength * 0.5f, m_pos.y + m_yLength * 0.5f, m_pos.z + m_zLength * 0.5f);
//     }

    public Matrix4x4 GetObjToWorld()
    {
        return Matrix4x4.TRS(m_pos, m_rotation, Vector3.one);
    }

    public float[] GetOrientationMatrixAsArray()
    {
        Matrix4x4 trs = Matrix4x4.TRS(Vector3.zero, m_rotation, Vector3.one);
        float[] matrixArray = new float[9];

        matrixArray[0] = trs.m00;
        matrixArray[1] = trs.m01;
        matrixArray[2] = trs.m02;

        matrixArray[3] = trs.m10;
        matrixArray[4] = trs.m11;
        matrixArray[5] = trs.m12;

        matrixArray[6] = trs.m20;
        matrixArray[7] = trs.m21;
        matrixArray[8] = trs.m22;

        return matrixArray;
    }

    public Vector3 GetHalfSize()
    {
        return m_size / 2;
    }

    public float[] GetHalfSizeAsArray()
    {
        float[] ret = new float[3];
        ret[0] = m_size.x / 2;
        ret[1] = m_size.y / 2;
        ret[2] = m_size.z / 2;
        return ret;
    }
}

