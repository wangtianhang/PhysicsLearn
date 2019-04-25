using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Mesh3d
{
    public class BVHNode
    {
        public AABB3d m_bounds;
        public BVHNode[] m_children = null;
        public List<Triangle3d> m_triangles = new List<Triangle3d>();
    }

    public List<Triangle3d> m_triangleList = new List<Triangle3d>();
    public BVHNode m_accelerator = null;
    //public Vector3[] vertices = null;

    public Mesh3d(List<Triangle3d> triangleList)
    {
        m_triangleList = triangleList;

        //AccelerateMesh(this);
    }

    public Vector3[] GetVertices()
    {
        Vector3[] vertices = new Vector3[m_triangleList.Count * 3];
        for(int i = 0; i < m_triangleList.Count; ++i)
        {
            vertices[i * 3 + 0] = m_triangleList[i].GetPoint(0);
            vertices[i * 3 + 1] = m_triangleList[i].GetPoint(1);
            vertices[i * 3 + 2] = m_triangleList[i].GetPoint(2);
        }
        return vertices;
    }

    public static AABB3d FromMinMax(Vector3 min, Vector3 max) 
    {
	    return new AABB3d((min + max) * 0.5f, (max - min));
    }

    public static void AccelerateMesh(Mesh3d mesh)
    {
        if (mesh.m_accelerator != null)
        {
		    return;
	    }

        Vector3[] vertices = mesh.GetVertices();
	    Vector3 min = vertices[0];
	    Vector3 max = vertices[0];

        for (int i = 1; i < mesh.m_triangleList.Count * 3; ++i)
        {
		    min.x = Math.Min(vertices[i].x, min.x);
            min.y = Math.Min(vertices[i].y, min.y);
            min.z = Math.Min(vertices[i].z, min.z);

            max.x = Math.Max(vertices[i].x, max.x);
            max.y = Math.Max(vertices[i].y, max.y);
            max.z = Math.Max(vertices[i].z, max.z);
	    }

        mesh.m_accelerator = new BVHNode();
        mesh.m_accelerator.m_bounds = FromMinMax(min, max);
        mesh.m_accelerator.m_children = null;
//         this.m_accelerator.m_triangles = this.m_triangleList.Count;
//         this.m_accelerator->triangles = new int[mesh.numTriangles];
// 	    for (int i = 0; i < mesh.numTriangles; ++i) {
// 		    mesh.accelerator->triangles[i] = i;
// 	    }
        //this.m_accelerator.m_triangles = this.m_triangleList;
//         for (int i = 0; i < mesh.m_triangleList.Count; ++i)
//         {
//             mesh.m_accelerator.m_triangles.Add(mesh.m_triangleList);
//         }
        mesh.m_accelerator.m_triangles.AddRange(mesh.m_triangleList);

        SplitBVHNode(mesh.m_accelerator, mesh, 3);
    }

    static void SplitBVHNode(BVHNode node, Mesh3d model, int depth) 
    {
	    if (depth-- <= 0) { // Decrements depth
		    return;
	    }

	    if (node.m_children == null) {
		    // Only split if this node contains triangles
		    if (node.m_triangles.Count > 0) {
			    node.m_children = new BVHNode[8];

			    Vector3 c = node.m_bounds.m_pos;
			    Vector3 e = node.m_bounds.GetHalfSize() * 0.5f;

                node.m_children[0].m_bounds = new AABB3d(c + new Vector3(-e.x, +e.y, -e.z), node.m_bounds.GetHalfSize());
                node.m_children[1].m_bounds = new AABB3d(c + new Vector3(+e.x, +e.y, -e.z), node.m_bounds.GetHalfSize());
                node.m_children[2].m_bounds = new AABB3d(c + new Vector3(-e.x, +e.y, +e.z), node.m_bounds.GetHalfSize());
                node.m_children[3].m_bounds = new AABB3d(c + new Vector3(+e.x, +e.y, +e.z), node.m_bounds.GetHalfSize());
                node.m_children[4].m_bounds = new AABB3d(c + new Vector3(-e.x, -e.y, -e.z), node.m_bounds.GetHalfSize());
                node.m_children[5].m_bounds = new AABB3d(c + new Vector3(+e.x, -e.y, -e.z), node.m_bounds.GetHalfSize());
                node.m_children[6].m_bounds = new AABB3d(c + new Vector3(-e.x, -e.y, +e.z), node.m_bounds.GetHalfSize());
                node.m_children[7].m_bounds = new AABB3d(c + new Vector3(+e.x, -e.y, +e.z), node.m_bounds.GetHalfSize());

		    }
	    }

	    // If this node was just split
	    if (node.m_children != null && node.m_triangles.Count > 0) 
        {
		    for (int i = 0; i < 8; ++i) 
            { // For each child
			    // Count how many triangles each child will contain
			    //node.m_children[i].numTriangles = 0;
                for (int j = 0; j < node.m_triangles.Count; ++j) 
                {
				    Triangle3d t = node.m_triangles[j];
				    if (IntersectionTest3D.Triangle3dWithAABB3d(t, node.m_children[i].m_bounds)) 
                    {
					    node.m_children[i].m_triangles.Add(t);
				    }
			    }
// 			    if (node.children[i].numTriangles == 0) {
// 				    continue;
// 			    }
// 			    node.children[i].triangles = new int[node.children[i].numTriangles];
// 			    int index = 0; // Add the triangles in the new child arrau
// 			    for (int j = 0; j < node.numTriangles; ++j) 
//                 {
// 				    Triangle t = model.triangles[node.triangles[j]];
// 				    if (TriangleAABB(t, node.children[i].bounds)) {
// 					    node.children[i].triangles[index++] = node.triangles[j];
// 				    }
// 			    }
		    }

		    //node.numTriangles = 0;
		    //delete[] node.triangles;
		    node.m_triangles.Clear();

		    // Recurse
		    for (int i = 0; i < 8; ++i) 
            {
			    SplitBVHNode(node.m_children[i], model, depth);
		    }
	    }
    }
}

