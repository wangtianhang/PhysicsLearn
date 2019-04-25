using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// 预计实现point2d ray2d line2d segment2d circle2d orientedRectangle2d convex2d sector2d 之间的相交测试
/// </summary>
public class IntersectionTest2D
{
    public static void Test()
    {
        Line2d line1 = new Line2d();
        line1.m_point1 = new Vector2(0, 0);
        line1.m_point2 = new Vector2(100, 100);

        Line2d line2 = new Line2d();
        line2.m_point1 = new Vector2(0, 100);
        line2.m_point2 = new Vector2(100, 0);

        Vector2 intersectionPoint = new Vector2();
        if (Line2dWithLine2d(line1, line2, ref intersectionPoint))
        {
            Debug.Log("双线相交 交点 " + intersectionPoint);
        }
        else
        {
            Debug.Log("双线没有相交");
        }

        List<Vector2> pointList = new List<Vector2>{new Vector2(-4.5f, -10f), 
            new Vector2(-4.5f, 10f), 
            new Vector2(4.5f, 10f), 
            new Vector2(4.5f, -10f)};
        Convex2d convex1 = new Convex2d(new Vector2(-5, 0), Quaternion.Euler(Vector3.zero), pointList);
        Convex2d convex2 = new Convex2d(new Vector2(+5, 0), Quaternion.Euler(Vector3.zero), pointList);
        Debug.Log("凸多边形相交测试1 " + (Convex2dWithConvex2d(convex1, convex2, false, true) != null));
        Convex2d convex3 = new Convex2d(new Vector2(+5, 0), Quaternion.Euler(new Vector3(0, -90, 0)), pointList);
        Debug.Log("凸多边形相交测试2 " + (Convex2dWithConvex2d(convex1, convex3, false, true) != null));

        Circle2d circle = new Circle2d(new Vector3(-5, 0), 5);
        Debug.Log("圆与凸多边形相交测试1 " + (Circle2dWithConvex2d(circle, convex2, false, true) != null));
        Debug.Log("圆与凸多边形相交测试2 " + (Circle2dWithConvex2d(circle, convex3, false, true) != null));
    }

    /// <summary>
    /// 2d线和2d线相交
    /// </summary>
    /// <param name="line2"></param>
    /// <returns></returns>
//     public static bool Line2dWithLine2d(Line2d line1, Line2d line2)
//     {
//         return Math.Abs(line1.m_slope - line2.m_slope) > Line2d.epsilon
//             || Math.Abs(line1.m_y_interept - line2.m_y_interept) < Line2d.epsilon;
//     }
    public static bool Line2dWithLine2d(Line2d line1, Line2d line2, ref Vector2 intersectionPoint)
    {
        return linesCross(line1.m_point1, line1.m_point2, line2.m_point1, line2.m_point2, ref intersectionPoint);
    }

    /// Perform the cross product on a scalar and a vector. In 2D this produces
    /// a vector.
    static Vector2 b2Cross(float s, Vector2 a)
    {
        return new Vector2(-s * a.y, s * a.x);
    }

    static bool linesCross(Vector2 v0, Vector2 v1, Vector2 t0, Vector2 t1, ref Vector2 intersectionPoint)
    {
        if (v1 == t0 ||
            v0 == t0 ||
            v1 == t1 ||
            v0 == t1)
            return false;

        Vector2 vnormal = v1 - v0;
        vnormal = b2Cross(1.0f, vnormal);
        float v0d = Vector2.Dot(vnormal, v0);
        float t0d = Vector2.Dot(vnormal, t0);
        float t1d = Vector2.Dot(vnormal, t1);
        if (t0d > v0d && t1d > v0d)
            return false;
        if (t0d < v0d && t1d < v0d)
            return false;

        Vector2 tnormal = t1 - t0;
        tnormal = b2Cross(1.0f, tnormal);
        t0d = Vector2.Dot(tnormal, t0);
        v0d = Vector2.Dot(tnormal, v0);
        float v1d = Vector2.Dot(tnormal, v1);
        if (v0d > t0d && v1d > t0d)
            return false;
        if (v0d < t0d && v1d < t0d)
            return false;

        intersectionPoint = v0 + ((t0d - v0d) / (v1d - v0d)) * (v1 - v0);

        return true;
    }

    public static bool Segment2dWithSegment2d(Segment2d segment1, Segment2d segment2, ref Vector3 result)
    {
        float resultX = 0;
        float resultY = 0;
        bool ret = get_line_intersection(segment1.m_point1.x, segment1.m_point1.y, segment1.m_point2.x, segment1.m_point2.y,
            segment2.m_point1.x, segment2.m_point1.y, segment2.m_point2.x, segment2.m_point2.y,
            ref resultX, ref resultY);
        result.x = resultX;
        result.z = resultY;
        return ret;
    }

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

    public static bool Point2dWithRectangle2d(Vector2 point2d, OrientedRectangle2d rectangle2d)
    {
        Vector3 forward = RotateHelper.GetForward(rectangle2d.m_rotation);
        Vector3 right = Vector3.Cross(Vector3.up, forward);
        Vector3 pos = new Vector3(rectangle2d.m_pos.x, 0, rectangle2d.m_pos.y);
        Vector3 a = pos + forward * rectangle2d.m_length * 0.5f + -right * rectangle2d.m_width * 0.5f;
        Vector3 b = pos + forward * rectangle2d.m_length * 0.5f + right * rectangle2d.m_width * 0.5f;
        Vector3 c = pos + -forward * rectangle2d.m_length * 0.5f + right * rectangle2d.m_width * 0.5f;
        Vector3 d = pos + -forward * rectangle2d.m_length * 0.5f + -right * rectangle2d.m_width * 0.5f;
        return IsInRectangle2d(a, b, c, d, new Vector3(point2d.x, 0, point2d.y));
    }

    /// <summary>
    /// 注意abcd为绕序
    /// </summary>
    /// <param name="p"></param>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    static bool IsInRectangle2d(Vector3 p, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        Vector3 v11 = p - a;
        Vector3 v12 = b - a;

        Vector3 v21 = p - b;
        Vector3 v22 = c - b;

        Vector3 v31 = p - c;
        Vector3 v32 = d - c;

        Vector3 v41 = p - d;
        Vector3 v42 = a - d;

        Vector3 cross1 = Vector3.Cross(v11, v12);
        Vector3 cross2 = Vector3.Cross(v21, v22);
        Vector3 cross3 = Vector3.Cross(v31, v32);
        Vector3 cross4 = Vector3.Cross(v41, v42);

        if (Vector3.Dot(cross1, cross2) > 0
            && Vector3.Dot(cross2, cross3) > 0
            && Vector3.Dot(cross3, cross4) > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool Point2dWithPolygon2d(Vector2 pos, Polygon2d polygon2d)
    {
        var edgePoint = (polygon2d.m_pointList[1] + polygon2d.m_pointList[0]) * 0.5f;
        Vector2 outPoint = (edgePoint - pos).normalized * 10000;
        int count = 0;
        List<Vector2> pointList = polygon2d.GetWorldPosList();
        for (int i = 0; i < pointList.Count; i++)
        {
            var a = pointList[i % pointList.Count];
            var b = pointList[(i + 1) % pointList.Count];

            var r = IsTwoSegmentIntersection(a, b, pos, outPoint);

            if (r)
            {
                count += 1;
            }
        }
        return count % 2 == 1;
    }

    /// <summary>
    /// 判断两条线段是否相交
    /// 现在有线段AB和线段CB
    //用线段AB的方向和C，D两点分别做差乘比较。如果C,D在同侧则return跳出
    //用线段CD的方向和A，B两点分别做差乘比较。如果A,B在同侧则return跳出
    //最终返回相交
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    static bool IsTwoSegmentIntersection(Vector2 a2d, Vector2 b2d, Vector2 c2d, Vector2 d2d)
    {
        Vector3 a = new Vector3(a2d.x, 0, a2d.y);
        Vector3 b = new Vector3(b2d.x, 0, b2d.y);
        Vector3 c = new Vector3(c2d.x, 0, c2d.y);
        Vector3 d = new Vector3(d2d.x, 0, d2d.y);

        var crossA = Math.Sign(Vector3.Cross(d - c, a - c).y);
        var crossB = Math.Sign(Vector3.Cross(d - c, b - c).y);

        if (Mathf.Approximately(crossA, crossB)) 
            return false;

        var crossC = Math.Sign(Vector3.Cross(b - a, c - a).y);
        var crossD = Math.Sign(Vector3.Cross(b - a, d - a).y);

        if (Mathf.Approximately(crossC, crossD)) 
            return false;

        return true;
    }

    public static bool Point2dWithSector2d(Vector2 pos, Sector2d sector2d)
    {
        Vector2 distance = pos - sector2d.m_pos;
        if (distance.magnitude > sector2d.m_radius)
        {
            return false;
        }

        Vector3 sectorForward = RotateHelper.GetForward(sector2d.m_rotation);
        float cosTarget = Vector3.Dot(sectorForward, distance) / sectorForward.magnitude / distance.magnitude;
        float cosHalfDegree = Mathf.Cos((Mathf.Deg2Rad * sector2d.m_theraDegree / 2));
        return cosTarget > cosHalfDegree;
    }

    public static bool Point2dWithLine2d(Vector2 point, Line2d line2d)
    {
        // Find the slope
        float dy = (line2d.m_point2.y - line2d.m_point1.y);
        float dx = (line2d.m_point2.x - line2d.m_point1.x);
        float M = dy / dx;
        // Find the Y-Intercept
        float B = line2d.m_point1.y - M * line2d.m_point1.x;
        // Check line equation
        return Mathf.Approximately(point.y, M * point.x + B);
    }

    public static bool Segment2dWithCircle2d(Segment2d segment, Circle2d circle)
    {
        Segment3d segment3d = new Segment3d(
            new Vector3(segment.m_point1.x, 0, segment.m_point1.y),
            new Vector3(segment.m_point2.x, 0, segment.m_point2.y));
        Vector3 closestPoint = Distance3d.ClosestPointOfPoint3dWithSegment3d(new Vector3(circle.m_pos.x, 0, circle.m_pos.y), segment3d);
        Vector2 distance = circle.m_pos - new Vector2(closestPoint.x, closestPoint.z);
        return distance.magnitude <= circle.m_radius;
    }

    /// <summary>
    /// 算法来自 game physics cookbook
    /// </summary>
    /// <param name="line"></param>
    /// <param name="circle"></param>
    /// <returns></returns>
    public static bool Line2dWithCircle2d(Line2d line, Circle2d circle)
    {
        Vector2 ab = line.m_point2 - line.m_point1;
        float t = Vector2.Dot(circle.m_pos - line.m_point1, ab) / Vector2.Dot(ab, ab);
        Vector2 closestPoint = line.m_point1 + ab * t;
        return (circle.m_pos - closestPoint).sqrMagnitude < circle.m_radius * circle.m_radius;
    }

    public static bool Line2dWithOrientedRectangle2d(Line2d line, OrientedRectangle2d rectangle2d)
    {
        Vector3 forward = RotateHelper.GetForward(rectangle2d.m_rotation);
        Vector3 right = Vector3.Cross(Vector3.up, forward);
        Vector3 pos = new Vector3(rectangle2d.m_pos.x, 0, rectangle2d.m_pos.y);
        Vector3 a = pos + forward * rectangle2d.m_length * 0.5f + -right * rectangle2d.m_width * 0.5f;
        Vector3 b = pos + forward * rectangle2d.m_length * 0.5f + right * rectangle2d.m_width * 0.5f;
        Vector3 c = pos + -forward * rectangle2d.m_length * 0.5f + right * rectangle2d.m_width * 0.5f;
        Vector3 d = pos + -forward * rectangle2d.m_length * 0.5f + -right * rectangle2d.m_width * 0.5f;
        List<Vector2> lineList = new List<Vector2>();
        lineList.Add(new Vector2(a.x, a.z));
        lineList.Add(new Vector2(b.x, b.z));
        lineList.Add(new Vector2(c.x, c.z));
        lineList.Add(new Vector2(d.x, d.z));
        for (int i = 0; i < lineList.Count; ++i )
        {
            if (IsTwoSegmentIntersection(lineList[i], lineList[(i + 1) % lineList.Count], line.m_point1, line.m_point2))
            {
                return true;
            }
        }

        return false;
    }

    public static bool Circle2dWithOrientedRectangle2d(Circle2d circle, OrientedRectangle2d rectangle)
    {
        Vector3 forward = RotateHelper.GetForward(rectangle.m_rotation);
        Vector3 right = Vector3.Cross(Vector3.up, forward);
        Vector3 pos = new Vector3(rectangle.m_pos.x, 0, rectangle.m_pos.y);
        Vector3 a = pos + forward * rectangle.m_length * 0.5f + -right * rectangle.m_width * 0.5f;
        Vector3 b = pos + forward * rectangle.m_length * 0.5f + right * rectangle.m_width * 0.5f;
        Vector3 c = pos + -forward * rectangle.m_length * 0.5f + right * rectangle.m_width * 0.5f;
        Vector3 d = pos + -forward * rectangle.m_length * 0.5f + -right * rectangle.m_width * 0.5f;
        List<Vector2> lineList = new List<Vector2>();
        lineList.Add(new Vector2(a.x, a.z));
        lineList.Add(new Vector2(b.x, b.z));
        lineList.Add(new Vector2(c.x, c.z));
        lineList.Add(new Vector2(d.x, d.z));
        for (int i = 0; i < lineList.Count; ++i)
        {
            Segment2d segment = new Segment2d();
            segment.m_point1 = lineList[i];
            segment.m_point2 = lineList[(i + 1) % lineList.Count];
            if (Segment2dWithCircle2d(segment, circle))
            {
                return true;
            }
        }
        return false;
    }

    #region Separating Axis Theorem

    public class CollisionInfo
    {
        public System.Object shapeA;
        public System.Object shapeB;
        public bool shapeAContained;
        public bool shapeBContained;
        public float distance;
        public Vector2 vector;
    }

    static Vector2 GetAxisNormal(List<Vector2> vertexArray, int pointIndex)
    {
        Vector2 pt1 = vertexArray[pointIndex];
        Vector2 pt2 = pointIndex >= vertexArray.Count - 1 ? vertexArray[0] : vertexArray[pointIndex + 1];
        Vector2 p = new Vector2(-(pt2.y - pt1.y), pt2.x - pt1.x);
        p.Normalize();
        return p;
    }

    /// <summary>
    /// 根据Separating Axis Theorem理论实现
    /// https://www.sevenson.com.au/actionscript/sat/
    /// https://github.com/sevdanski/SAT_AS3
    /// </summary>
    /// <param name="convex1"></param>
    /// <param name="convex2"></param>
    /// <returns></returns>
    public static CollisionInfo Convex2dWithConvex2d(Convex2d polygonA, Convex2d polygonB, bool flip, bool docalc)
    {
        CollisionInfo result = new CollisionInfo();
        result.shapeA = (flip) ? polygonB : polygonA;
        result.shapeB = (flip) ? polygonA : polygonB;
        result.shapeAContained = true;
        result.shapeBContained = true;

        // get the vertices
        List<Vector2> p1 = polygonA.GetRotateList();
        List<Vector2> p2 = polygonB.GetRotateList();

        // get the offset
        Vector2 vOffset = new Vector2(polygonA.m_pos.x - polygonB.m_pos.x, polygonA.m_pos.y - polygonB.m_pos.y);

        float shortestDist = float.MaxValue;

        // loop through all of the axis on the first polygon
        for (int i = 0; i < p1.Count; ++i )
        {
            // find the axis that we will project onto
            Vector2 vAxis = GetAxisNormal(p1, i);

            // project polygon A
            float min0 = Vector2.Dot(vAxis, p1[0]);
            float max0 = min0;

            for (int j = 1; j < p1.Count; ++j )
            {
                float t = Vector2.Dot(vAxis, p1[j]);
                if(t < min0)
                {
                    min0 = t;
                }
                if(t > max0)
                {
                    max0 = t;
                }
            }

            // project polygon B
            float min1 = Vector2.Dot(vAxis, p2[0]);
            float max1 = min1;
            for (int j = 1; j < p2.Count; ++j )
            {
                float t = Vector2.Dot(vAxis, p2[j]);
                if(t < min1)
                {
                    min1 = t;
                }
                if(t > max1)
                {
                    max1 = t;
                }
            }

            // shift polygonA's projected points
            float sOffset = Vector2.Dot(vAxis, vOffset);
            min0 += sOffset;
            max0 += sOffset;

            // test for intersections
            float d0 = min0 - max1;
            float d1 = min1 - max0;
            if(d0 > 0 || d1 > 0)
            {
                // gap found
                return null;
            }

            if(docalc)
            {
                // check for containment
                if(!flip)
                {
                    if(max0 > max1 || min0 < min1)
                    {
                        result.shapeAContained = false;
                    }
                    if(max1 > max0 || min1 < min0)
                    {
                        result.shapeBContained = false;
                    }
                }
                else
                {
                    if(max0 < max1 || min0 > min1)
                    {
                        result.shapeAContained = false;
                    }
                    if(max1 < max0 || min1 > min0)
                    {
                        result.shapeBContained = false;
                    }
                }

                float distmin = (max1 - min0) * -1;
                if(flip)
                {
                    distmin *= -1;
                }
                float distminAbs = (distmin < 0) ? distmin * -1 : distmin;
                if (distminAbs < shortestDist)
                {
                    // this distance is shorter so use it...
                    result.distance = distmin;
                    result.vector = vAxis;

                    shortestDist = distminAbs;
                }

            }
        }

        // if you are here then no gap was found
        return result;
    }

    /// <summary>
    /// 根据Separating Axis Theorem理论实现
    /// https://www.sevenson.com.au/actionscript/sat/
    /// https://github.com/sevdanski/SAT_AS3
    /// </summary>
    /// <param name="convex1"></param>
    /// <param name="convex2"></param>
    /// <returns></returns>
    public static CollisionInfo Circle2dWithConvex2d(Circle2d circleA, Convex2d polygonA, bool flip, bool docalc)
    {
        CollisionInfo result = new CollisionInfo();
        if(flip)
        {
            result.shapeA = polygonA;
            result.shapeB = circleA;
        }
        else
        {
            result.shapeA = circleA;
            result.shapeB = polygonA;
        }
        result.shapeAContained = true;
        result.shapeBContained = true;

        // get the offset
        Vector2 vOffset = new Vector2(polygonA.m_pos.x - circleA.m_pos.x, polygonA.m_pos.y - circleA.m_pos.y);

        // get the vertices
        List<Vector2> p1 = polygonA.GetRotateList();

        // find the closest point
        Vector2 closestPoint = new Vector2();
        float minDist = float.MaxValue;
        foreach(var iter in p1)
        {
            float currentDist = (circleA.m_pos - (polygonA.m_pos + iter)).sqrMagnitude;
            if(currentDist < minDist)
            {
                minDist = currentDist;
                closestPoint = polygonA.m_pos + iter;
            }
        }

        // make a normal of this vector
        Vector2 vAxis = closestPoint - circleA.m_pos;
        vAxis.Normalize();

        // project polygon A
        float min0 = Vector2.Dot(vAxis, p1[0]);
        float max0 = min0;

        for (int j = 1; j < p1.Count; ++j )
        {
            float t = Vector2.Dot(vAxis, p1[j]);
            if(t < min0)
            {
                min0 = t;
            }
            if(t > max0)
            {
                max0 = t;
            }
        }

        // project circle A
        float min1 = Vector2.Dot(vAxis, Vector2.zero);
        float max1 = min1 + circleA.m_radius;
        min1 -= circleA.m_radius;

        // shift polygonA's projected points
        float sOffset = Vector2.Dot(vAxis, vOffset);
        min0 += sOffset;
        max0 += sOffset;

        // test for intersections
        float d0 = min0 - max1;
        float d1 = min1 - max0;

        if (d0 > 0 || d1 > 0)
        {
            // gap found
            return null;
        }

        float shortestDist = float.MaxValue;
        if(docalc) {
			float distmin = (max1 - min0) * -1;  //Math.min(dist0, dist1);
            if (flip) distmin *= -1;
			float distminAbs = (distmin < 0) ? distmin * -1 : distmin;
							
			// check for containment
			if (!flip) {
				if (max0 > max1 || min0 < min1) result.shapeAContained = false;
				if (max1 > max0 || min1 < min0) result.shapeBContained = false;
			} else {
				if (max0 < max1 || min0 > min1) result.shapeAContained = false;				
				if (max1 < max0 || min1 > min0) result.shapeBContained = false;				
			}			
				
			// this distance is shorter so use it...
			result.distance = distmin;
			result.vector = vAxis;
			//
			shortestDist = distminAbs;
		}

        // loop through all of the axis on the first polygon
		for (int i = 0; i < p1.Count; i++) {
			// find the axis that we will project onto
			vAxis = GetAxisNormal(p1, i);
				
			// project polygon A
			min0 = Vector2.Dot(vAxis, p1[0]);
			max0 = min0;
				
			//
			for (int j = 1; j < p1.Count; j++) {
				float t = Vector2.Dot(vAxis, p1[j]);
				if (t < min0) min0 = t;
				if (t > max0) max0 = t;
			}
				
			// project circle A
			min1 = Vector2.Dot(vAxis, new Vector2(0,0) );
			max1 = min1 + circleA.m_radius;
			min1 -= circleA.m_radius;				
				
			// shift polygonA's projected points
			sOffset = Vector2.Dot(vAxis, vOffset);
			min0 += sOffset;
			max0 += sOffset;				
				
			// test for intersections
			d0 = min0 - max1;
			d1 = min1 - max0;
				
			if (d0 > 0 || d1 > 0) {
				// gap found
				return null;
			}
				
			if(docalc) {

				// check for containment
				if (!flip) {
					if (max0 > max1 || min0 < min1) result.shapeAContained = false;
					if (max1 > max0 || min1 < min0) result.shapeBContained = false;
				} else {
					if (max0 < max1 || min0 > min1) result.shapeAContained = false;				
					if (max1 < max0 || min1 > min0) result.shapeBContained = false;				
				}				
					
				float distmin = (max1 - min0) * -1;
                if (flip) distmin *= -1;
				float distminAbs = (distmin < 0) ? distmin * -1 : distmin;
				if (distminAbs < shortestDist) {
					// this distance is shorter so use it...
					result.distance = distmin;
					result.vector = vAxis;
					//
					shortestDist = distminAbs;
				}
			}
		}

        // if you are here then no gap was found
        return result;
    }

    #endregion
}



