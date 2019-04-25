using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Scene3d
{
    List<Model3d> m_modleList = new List<Model3d>();

    OctreeNode m_octree;

    public void AddModel(Model3d model)
    {
        if(m_modleList.Contains(model))
        {
            return;
        }
        m_modleList.Add(model);
    }

    public void RemoveModel(Model3d model)
    {
        m_modleList.Remove(model);
    }

    public void UpdateModel(Model3d model)
    {

    }

    public List<Model3d> FindChildren(Model3d model)
    {
        List<Model3d> ret = new List<Model3d>();
        foreach (var iter in m_modleList)
        {
            if(iter.m_parent == model)
            {
                ret.Add(iter);
            }
        }
        return ret;
    }

    public Model3d Raycast(Ray3d ray)
    {
        if (m_octree != null) {
		    // :: lets the compiler know to look outside class scope
            return Octree3d.Raycast(m_octree, ray);
	    }

        Model3d result = null;
        float result_t = -1;

        for (int i = 0, size = m_modleList.Count; i < size; ++i)
        {
            float t = IntersectionTest3D.Ray3dWithModel3d(ray, m_modleList[i]);
            if (result == null && t >= 0)
            {
                result = m_modleList[i];
                result_t = t;
            }
            else if (result != null && t < result_t)
            {
                result = m_modleList[i];
                result_t = t;
            }
        }

        return result;
    }

    public List<Model3d> Query(Sphere3d sphere)
    {
        if (m_octree != null)
        {
            // :: lets the compiler know to look outside class scope
            return Octree3d.Query(m_octree, sphere);
        }

        List<Model3d> result = new List<Model3d>();
        for (int i = 0, size = m_modleList.Count; i < size; ++i)
        {
            OBB3d bounds = m_modleList[i].GetOBB();
            if (IntersectionTest3D.Sphere3dWithObb3d(sphere, bounds))
            {
                result.Add(m_modleList[i]);
            }
        }
        return result;
    }

    public List<Model3d> Query(AABB3d aabb)
    {
        if (m_octree != null)
        {
            // :: lets the compiler know to look outside class scope
            return Octree3d.Query(m_octree, aabb);
        }

        List<Model3d> result = new List<Model3d>();
        for (int i = 0, size = m_modleList.Count; i < size; ++i)
        {
            OBB3d bounds = m_modleList[i].GetOBB();
            if (IntersectionTest3D.AABB3dWithOBB3d(aabb, bounds))
            {
                result.Add(m_modleList[i]);
            }
        }
        return result;
    }

    bool Accelerate(Vector3 position, float size) 
    {
	    if (m_octree != null) {
		    return false;
	    }

	    Vector3 min = new Vector3(position.x - size, position.y - size, position.z - size);
        Vector3 max = new Vector3(position.x + size, position.y + size, position.z + size);

	    // Construct tree root
        m_octree = new OctreeNode();
        m_octree.m_bounds = Mesh3d.FromMinMax(min, max);
        m_octree.m_children = null;
        for (int i = 0, size2 = m_modleList.Count; i < size2; ++i)
        {
            m_octree.m_models.Add(m_modleList[i]);
	    }

	    // Create tree
        Octree3d.SplitTree(m_octree, 5);
	    return true;
    }

    public List<Model3d> CullByFrustum(Frustum3d frustum)
    {
        List<Model3d> result = new List<Model3d>();
        foreach(var iter in m_modleList)
        {
            iter.m_cullFlag = false;
        }

        if(m_octree == null)
        {
            foreach(var iter in m_modleList)
            {
                OBB3d bounds = iter.GetOBB();
                if(IntersectionTest3D.Frustum3dWithOBB3d(frustum, bounds))
                {
                    result.Add(iter);
                }
            }
        }
        else
        {
            List<OctreeNode> nodes = new List<OctreeNode>();
            nodes.Add(m_octree);

            while(nodes.Count > 0)
            {
                OctreeNode active = nodes[0];
                nodes.RemoveAt(0);

                if(active.m_children != null)
                {
                    // Has child nodes
                    for(int i = 0; i < 8; ++i)
                    {
                        AABB3d bounds = active.m_children[i].m_bounds;
                        if(IntersectionTest3D.Frustum3dWithAABB3d(frustum, bounds))
                        {
                            nodes.Add(active.m_children[i]);
                        }
                    }
                }
                else
                {
                    // Is leaf node
                    for(int i = 0; i < active.m_models.Count; ++i)
                    {
                        if(!active.m_models[i].m_cullFlag)
                        {
                            OBB3d bounds = active.m_models[i].GetOBB();
                            if(IntersectionTest3D.Frustum3dWithOBB3d(frustum, bounds))
                            {
                                active.m_models[i].m_cullFlag = true;
                                result.Add(active.m_models[i]);
                            }
                        }
                    }
                }
            }
        }

        return result;
    }
}

