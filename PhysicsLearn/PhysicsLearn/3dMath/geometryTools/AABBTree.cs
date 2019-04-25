using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// 参考 https://www.azurefromthetrenches.com/introductory-guide-to-aabb-tree-collision-detection/
// 移植 form https://github.com/JamesRandall/SimpleVoxelEngine

class AABBTreeNode
{
    public AABB3d m_bounds;
    //public List<AABB3d> m_objectList = new List<AABB3d>();
    public IAABB m_object;
    public int m_parentNodeIndex = AABBTree.NullIndex;
    public int m_leftNodeIndex = AABBTree.NullIndex;
    public int m_rightNodeIndex = AABBTree.NullIndex;
    public int m_nextNodeIndex = AABBTree.NullIndex;
    public bool IsLeaf()
    {
        return m_leftNodeIndex == AABBTree.NullIndex;
    }
}

public interface IAABB
{
    AABB3d GetAABBBounds();
}

class AABBTree
{
    public const int NullIndex = -1;

    Dictionary<IAABB, int> m_objectNodeIndexMap = new Dictionary<IAABB, int>();
    List<AABBTreeNode> m_nodes = new List<AABBTreeNode>();
    int m_rootNodeIndex;
    int m_allocatedNodeCount;
    int m_nextFreeNodeIndex;
    int m_nodeCapacity;
    int m_growthSize;

    int AllocateNode()
    {
        //return AABBTree.NullIndex;
        if (m_nextFreeNodeIndex == AABBTree.NullIndex)
        {
            DebugHelper.Assert(m_allocatedNodeCount == m_nodeCapacity);

            m_nodeCapacity += m_growthSize;
            for(int nodeIndex = m_allocatedNodeCount; nodeIndex < m_nodeCapacity; nodeIndex++)
            {
                AABBTreeNode node = new AABBTreeNode();
                m_nodes.Add(node);
                node.m_nextNodeIndex = nodeIndex + 1;
            }
            m_nodes[m_nodeCapacity - 1].m_nextNodeIndex = AABBTree.NullIndex;
            m_nextFreeNodeIndex = m_allocatedNodeCount;
        }

        {
            int nodeIndex = m_nextFreeNodeIndex;
            AABBTreeNode allocateNode = m_nodes[nodeIndex];
            //allocateNode.m_parentNodeIndex = AABBTree.NullIndex;
            m_nextFreeNodeIndex = allocateNode.m_nextNodeIndex;
            m_allocatedNodeCount += 1;
            return nodeIndex;
        }
    }

    void DeallocateNode(int nodeIndex)
    {
        AABBTreeNode deallocatedNode = m_nodes[nodeIndex];
        deallocatedNode.m_nextNodeIndex = m_nextFreeNodeIndex;
        m_nextFreeNodeIndex = nodeIndex;
        m_allocatedNodeCount--;
    }

    void InsertLeaf(int leafNodeIndex)
    {
        // make sure we're inserting a new leaf
        DebugHelper.Assert(m_nodes[leafNodeIndex].m_parentNodeIndex == NullIndex);
        DebugHelper.Assert(m_nodes[leafNodeIndex].m_leftNodeIndex == NullIndex);
        DebugHelper.Assert(m_nodes[leafNodeIndex].m_rightNodeIndex == NullIndex);

        // if the tree is empty then we make the root the leaf
        if (m_rootNodeIndex == NullIndex)
        {
            m_rootNodeIndex = leafNodeIndex;
            return;
        }

        // search for the best place to put the new leaf in the tree
        // we use surface area and depth as search heuristics
        int treeNodeIndex = m_rootNodeIndex;
        AABBTreeNode leafNode = m_nodes[leafNodeIndex];
        while (!m_nodes[treeNodeIndex].IsLeaf())
        {
            // because of the test in the while loop above we know we are never a leaf inside it
            AABBTreeNode treeNode = m_nodes[treeNodeIndex];
            int leftNodeIndex = treeNode.m_leftNodeIndex;
            int rightNodeIndex = treeNode.m_rightNodeIndex;
            AABBTreeNode leftNode = m_nodes[leftNodeIndex];
            AABBTreeNode rightNode = m_nodes[rightNodeIndex];

            AABB3d combinedAabb = treeNode.m_bounds.Merge(leafNode.m_bounds);

            float newParentNodeCost = 2.0f * combinedAabb.CalculateSurfaceArea();
            float minimumPushDownCost = 2.0f * (combinedAabb.CalculateSurfaceArea() - treeNode.m_bounds.CalculateSurfaceArea());

            // use the costs to figure out whether to create a new parent here or descend
            float costLeft;
            float costRight;
            if (leftNode.IsLeaf())
            {
                costLeft = leafNode.m_bounds.Merge(leftNode.m_bounds).CalculateSurfaceArea() + minimumPushDownCost;
            }
            else
            {
                AABB3d newLeftAabb = leafNode.m_bounds.Merge(leftNode.m_bounds);
                costLeft = (newLeftAabb.CalculateSurfaceArea() - leftNode.m_bounds.CalculateSurfaceArea()) + minimumPushDownCost;
            }
            if (rightNode.IsLeaf())
            {
                costRight = leafNode.m_bounds.Merge(rightNode.m_bounds).CalculateSurfaceArea() + minimumPushDownCost;
            }
            else
            {
                AABB3d newRightAabb = leafNode.m_bounds.Merge(rightNode.m_bounds);
                costRight = (newRightAabb.CalculateSurfaceArea() - rightNode.m_bounds.CalculateSurfaceArea()) + minimumPushDownCost;
            }

            // if the cost of creating a new parent node here is less than descending in either direction then
            // we know we need to create a new parent node, errrr, here and attach the leaf to that
            if (newParentNodeCost < costLeft && newParentNodeCost < costRight)
            {
                break;
            }

            // otherwise descend in the cheapest direction
            if (costLeft < costRight)
            {
                treeNodeIndex = leftNodeIndex;
            }
            else
            {
                treeNodeIndex = rightNodeIndex;
            }
        }

        // the leafs sibling is going to be the node we found above and we are going to create a new
        // parent node and attach the leaf and this item
        int leafSiblingIndex = treeNodeIndex;
        AABBTreeNode leafSibling = m_nodes[leafSiblingIndex];
        int oldParentIndex = leafSibling.m_parentNodeIndex;
        int newParentIndex = AllocateNode();
        AABBTreeNode newParent = m_nodes[newParentIndex];
        newParent.m_parentNodeIndex = oldParentIndex;
        newParent.m_bounds = leafNode.m_bounds.Merge(leafSibling.m_bounds); // the new parents aabb is the leaf aabb combined with it's siblings aabb
        newParent.m_leftNodeIndex = leafSiblingIndex;
        newParent.m_rightNodeIndex = leafNodeIndex;
        leafNode.m_parentNodeIndex = newParentIndex;
        leafSibling.m_parentNodeIndex = newParentIndex;

        if (oldParentIndex == NullIndex)
        {
            // the old parent was the root and so this is now the root
            m_rootNodeIndex = newParentIndex;
        }
        else
        {
            // the old parent was not the root and so we need to patch the left or right index to
            // point to the new node
            AABBTreeNode oldParent = m_nodes[oldParentIndex];
            if (oldParent.m_leftNodeIndex == leafSiblingIndex)
            {
                oldParent.m_leftNodeIndex = newParentIndex;
            }
            else
            {
                oldParent.m_rightNodeIndex = newParentIndex;
            }
        }

        // finally we need to walk back up the tree fixing heights and areas
        treeNodeIndex = leafNode.m_parentNodeIndex;
        FixUpwardsTree(treeNodeIndex);
    }

    void RemoveLeaf(int leafNodeIndex)
    {
        // if the leaf is the root then we can just clear the root pointer and return
        if (leafNodeIndex == m_rootNodeIndex)
        {
            m_rootNodeIndex = NullIndex;
            return;
        }

        AABBTreeNode leafNode = m_nodes[leafNodeIndex];
        int parentNodeIndex = leafNode.m_parentNodeIndex;
        AABBTreeNode parentNode = m_nodes[parentNodeIndex];
        int grandParentNodeIndex = parentNode.m_parentNodeIndex;
        int siblingNodeIndex = parentNode.m_leftNodeIndex == leafNodeIndex ? parentNode.m_rightNodeIndex : parentNode.m_leftNodeIndex;
        DebugHelper.Assert(siblingNodeIndex != NullIndex); // we must have a sibling
        AABBTreeNode siblingNode = m_nodes[siblingNodeIndex];

        if (grandParentNodeIndex != NullIndex)
        {
            // if we have a grand parent (i.e. the parent is not the root) then destroy the parent and connect the sibling to the grandparent in its
            // place
            AABBTreeNode grandParentNode = m_nodes[grandParentNodeIndex];
            if (grandParentNode.m_leftNodeIndex == parentNodeIndex)
            {
                grandParentNode.m_leftNodeIndex = siblingNodeIndex;
            }
            else
            {
                grandParentNode.m_rightNodeIndex = siblingNodeIndex;
            }
            siblingNode.m_parentNodeIndex = grandParentNodeIndex;
            DeallocateNode(parentNodeIndex);


            FixUpwardsTree(grandParentNodeIndex);
        }
        else
        {
            // if we have no grandparent then the parent is the root and so our sibling becomes the root and has it's parent removed
            m_rootNodeIndex = siblingNodeIndex;
            siblingNode.m_parentNodeIndex = NullIndex;
            DeallocateNode(parentNodeIndex);
        }

        leafNode.m_parentNodeIndex = NullIndex;
    }

    void UpdateLeaf(int leafNodeIndex, AABB3d newAABB)
    {
        AABBTreeNode node = m_nodes[leafNodeIndex];

        // if the node contains the new aabb then we just leave things
        // TODO: when we add velocity this check should kick in as often an update will lie within the velocity fattened initial aabb
        // to support this we might need to differentiate between velocity fattened aabb and actual aabb
        if (node.m_bounds.Contains(newAABB))
            return;

        RemoveLeaf(leafNodeIndex);
        node.m_bounds = newAABB;
        InsertLeaf(leafNodeIndex);
    }

    void FixUpwardsTree(int treeNodeIndex)
    {
        while (treeNodeIndex != NullIndex)
        {
            AABBTreeNode treeNode = m_nodes[treeNodeIndex];

            // every node should be a parent
            DebugHelper.Assert(treeNode.m_leftNodeIndex != NullIndex && treeNode.m_rightNodeIndex != NullIndex);

            // fix height and area
            AABBTreeNode leftNode = m_nodes[treeNode.m_leftNodeIndex];
            AABBTreeNode rightNode = m_nodes[treeNode.m_rightNodeIndex];
            treeNode.m_bounds = leftNode.m_bounds.Merge(rightNode.m_bounds);

            treeNodeIndex = treeNode.m_parentNodeIndex;
        }
    }

    public AABBTree(int initialSize)
    {
        m_rootNodeIndex = AABBTree.NullIndex;
        m_allocatedNodeCount = 0;
        m_nextFreeNodeIndex = 0;
        m_nodeCapacity = initialSize;
        m_growthSize = initialSize;

        //m_nodes.Capacity = initialSize;
        for(int nodeIndex = 0; nodeIndex < initialSize; ++nodeIndex)
        {
            // AABBTreeNode node = m_nodes[nodeIndex];
            //node.m_nextNodeIndex = nodeIndex + 1;
            AABBTreeNode node = new AABBTreeNode();
            m_nodes.Add(node);
            node.m_nextNodeIndex = nodeIndex + 1;
        }
        m_nodes[initialSize - 1].m_nextNodeIndex = AABBTree.NullIndex;
    }

    public void InsertObject(IAABB obj)
    {
        int nodeIndex = AllocateNode();
        AABBTreeNode node = m_nodes[nodeIndex];

        node.m_bounds = obj.GetAABBBounds();
        node.m_object = obj;

        InsertLeaf(nodeIndex);
        m_objectNodeIndexMap[obj] = nodeIndex;
    }

    public void RemoveObject(IAABB obj)
    {
        int nodeIndex = m_objectNodeIndexMap[obj];
        RemoveLeaf(nodeIndex);
        DeallocateNode(nodeIndex);
        m_objectNodeIndexMap.Remove(obj);
    }

    public void UpdateObject(IAABB obj)
    {
        int nodeIndex = m_objectNodeIndexMap[obj];
        UpdateLeaf(nodeIndex, obj.GetAABBBounds());
    }

    public List<IAABB> QueryOverlaps(IAABB obj)
    {
        List<IAABB> overlaps = new List<IAABB>();
        Stack<int> stack = new Stack<int>();
        AABB3d testAabb = obj.GetAABBBounds();

        stack.Push(m_rootNodeIndex);
        while (stack.Count != 0)
        {
            int nodeIndex = stack.Pop();

            if (nodeIndex == NullIndex)
                continue;

            AABBTreeNode node = m_nodes[nodeIndex];
            if (node.m_bounds.Overlaps(testAabb))
            {
                if (node.IsLeaf() && node.m_object != obj)
			    {
                    overlaps.Insert(0, node.m_object);
                }
			    else
			    {
                    stack.Push(node.m_leftNodeIndex);
                    stack.Push(node.m_rightNodeIndex);
                }
            }
        }

        return overlaps;
    }
}

