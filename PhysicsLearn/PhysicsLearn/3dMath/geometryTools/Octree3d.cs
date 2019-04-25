using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class OctreeNode
{
    public AABB3d m_bounds;
    public OctreeNode[] m_children;
    public List<Model3d> m_models = new List<Model3d>();
}

public class Octree3d
{
    public static void SplitTree(OctreeNode node, int depth)
    {
        if (depth-- <= 0)
        { // Decrements depth
            return;
        }

        if (node.m_children == null)
        {
            node.m_children = new OctreeNode[8];

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

        if (node.m_children != null && node.m_models.Count > 0)
        {
            for (int i = 0; i < 8; ++i)
            { // For each child
                for (int j = 0, size = node.m_models.Count; j < size; ++j)
                {
                    OBB3d obb = node.m_models[j].GetOBB();
                    if (IntersectionTest3D.AABB3dWithOBB3d(node.m_children[i].m_bounds, obb))
                    {
                        node.m_children[i].m_models.Add(node.m_models[j]);
                    }
                }
            }
            node.m_models.Clear();

            // Recurse
            for (int i = 0; i < 8; ++i)
            {
                SplitTree(node.m_children[i], depth);
            }
        }
    }

    public static void Insert(OctreeNode node, Model3d model)
    {
        OBB3d bounds = model.GetOBB();
        if (IntersectionTest3D.AABB3dWithOBB3d(node.m_bounds, bounds))
        {
            if (node.m_children == null)
            {
                node.m_models.Add(model);
            }
            else
            {
                for (int i = 0; i < 8; ++i)
                {
                    Insert(node.m_children[i], model);
                }
            }
        }
    }

    public static void Remove(OctreeNode node, Model3d model) 
    {
	    if (node.m_children == null) 
        {
// 		    std::vector<Model*>::iterator it = std::find(node->models.begin(), node->models.end(), model);
// 		    if (it != node->models.end()) 
//             {
// 			    node->models.erase(it);
// 		    }
            node.m_models.Remove(model);
	    }
	    else 
        {
		    for (int i = 0; i < 8; ++i) 
            {
			    Remove(node.m_children[i], model);
		    }
	    }
    }

    public static void Update(OctreeNode node, Model3d model)
    {
        Remove(node, model);
        Insert(node, model);
    }

    public static Model3d FindClosest(List<Model3d> set, Ray3d ray) 
    {
	    if (set.Count == 0) {
		    return null;
	    }

	    Model3d closest = null;
	    float closest_t = -1;

	    for (int i = 0, size = set.Count; i < size; ++i) {
		    float this_t = IntersectionTest3D.Ray3dWithModel3d(ray, set[i]);

		    if (this_t < 0) {
			    continue;
		    }

		    if (closest_t < 0 || this_t < closest_t) {
			    closest_t = this_t;
			    closest = set[i];
		    }
	    }

	    return closest;
    }

    public static Model3d Raycast(OctreeNode node, Ray3d ray) 
    {
        IntersectionTest3D.RaycastResult raycast = new IntersectionTest3D.RaycastResult();
        IntersectionTest3D.Ray3dWithAABB3d(node.m_bounds, ray, ref raycast);
	    float t = raycast.m_t;

	    if (t >= 0) 
        {
		    if (node.m_children == null) 
            {
			    return FindClosest(node.m_models, ray);
		    }
		    else 
            {
			    /*std::vector<Model*> results;*/
                List<Model3d> results = new List<Model3d>();
			    for (int i = 0; i < 8; ++i) 
                {
				    Model3d result = Raycast(node.m_children[i], ray);
				    if (result != null) 
                    {
					    results.Add(result);
				    }
			    }
			    return FindClosest(results, ray);
		    }
	    }

	    return null;
    }

    public static List<Model3d> Query(OctreeNode node, Sphere3d sphere) 
    {
	    //std::vector<Model*> result;
        List<Model3d> result = new List<Model3d>();

	    if (IntersectionTest3D.Sphere3dWithAABB3d(sphere, node.m_bounds)) 
        {
		    if (node.m_children == null) {
			    for (int i = 0, size = node.m_models.Count; i < size; ++i) 
                {
				    OBB3d bounds = node.m_models[i].GetOBB();
                    if (IntersectionTest3D.Sphere3dWithObb3d(sphere, bounds))
                    {
					    result.Add(node.m_models[i]);
				    }
			    }
		    }
		    else 
            {
			    for (int i = 0; i < 8; ++i) 
                {
				    List<Model3d> child = Query(node.m_children[i], sphere);
				    if (child.Count > 0) 
                    {
					    //result.insert(result.end(), child.begin(), child.end());
                        result.AddRange(child);
				    }
			    }
		    }
	    }

	    return result;
    }

    public static List<Model3d> Query(OctreeNode node, AABB3d aabb)
    {
        //std::vector<Model*> result;
        List<Model3d> result = new List<Model3d>();

        if (IntersectionTest3D.AABB3dWithAABB3d(aabb, node.m_bounds))
        {
            if (node.m_children == null)
            {
                for (int i = 0, size = node.m_models.Count; i < size; ++i)
                {
                    OBB3d bounds = node.m_models[i].GetOBB();
                    if (IntersectionTest3D.AABB3dWithOBB3d(aabb, bounds))
                    {
                        result.Add(node.m_models[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 8; ++i)
                {
                    List<Model3d> child = Query(node.m_children[i], aabb);
                    if (child.Count > 0)
                    {
                        //result.insert(result.end(), child.begin(), child.end());
                        result.AddRange(child);
                    }
                }
            }
        }

        return result;
    }

    
}

