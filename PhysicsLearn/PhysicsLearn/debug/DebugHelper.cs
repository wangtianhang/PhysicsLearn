using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DebugHelper
{
    public static void Assert(bool InCondition)
    {
        DebugHelper.Assert(InCondition, null, null);
    }

    public static void Assert(bool InCondition, string InFormat)
    {
        DebugHelper.Assert(InCondition, InFormat, null);
    }

    public static void Assert(bool InCondition, string InFormat, params object[] InParameters)
    {
        if (!InCondition)
        {
            try
            {
                string text = null;
                if (!string.IsNullOrEmpty(InFormat))
                {
                    if (InParameters != null)
                    {
                        try
                        {
                            text = string.Format(InFormat, InParameters);
                        }
                        catch (Exception)
                        {

                        }
                    }
                    else;
                    {
                        text = InFormat;
                    }
                }
                if (text != null)
                {
                    string text2 = "Assert failed! " + text;
                    Debug.LogError(text2);
                    //DebugHelper.CrashAttchLog(text2);
                }
                else
                {
                    Debug.LogError("Assert failed!");
                }
            }
            catch (Exception)
            {
            }
        }
    }

//     public static void TestDrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
//     {
//         GameObject myLine = new GameObject();
//         myLine.transform.position = start;
//         myLine.AddComponent<LineRenderer>();
//         LineRenderer lr = myLine.GetComponent<LineRenderer>();
//         Material mat = new Material(Shader.Find("Particles/Alpha Blended"));
//         mat.SetColor("_TintColor", color);
//         lr.material = mat;
//         //lr.material.color = color;
//         lr.SetColors(Color.white, Color.white);
//         lr.SetWidth(0.01f, 0.01f);
//         lr.SetPosition(0, start);
//         lr.SetPosition(1, end);
//         GameObject.Destroy(myLine, duration);
//     }
}

