using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// 从orgenode copy
namespace UnityEngine
{
    public class Transform : Component
    {
        bool m_needParentUpdate = true;

        Vector3 m_localPostion;
        Quaternion m_localRotation = Quaternion.identity;
        Vector3 m_localScale = Vector3.one;

        Quaternion m_DerivedOrientation = Quaternion.identity; // 包含自身和上级
        Vector3 m_DerivedScale = Vector3.one; // 包含自身和上级
        Vector3 m_DerivedPosition; // 包含自身和上级

        Transform m_parent = null;
        List<Transform> m_childList = new List<Transform>();

        Vector3 m_worldPosition;
        bool m_isWorldPositionNeedUpdate = true;

        public static void Test()
        {
            Debug.Log("==========================LogicTransform.Test begin");
            {
                GameObject t1 = new GameObject("transformTest1");
                GameObject t2 = new GameObject("transformTest2");
                t2.transform.parent = t1.transform;
                t2.transform.position = new Vector3(1, 0, 0);
                Debug.Log("LocalToWorld " + t2.transform.localToWorldMatrix);
                Debug.Log("worldToLocal " + t2.transform.worldToLocalMatrix);

                //Quaternion delta = Quaternion.FromToRotation(t2.transform.forward, new Vector3(1, 1, 1));
                t2.transform.forward = new Vector3(1, 1, 1);
                //t2.transform.rotation = t2.transform.rotation * delta;
                Debug.Log("forward rotation " + t2.transform.rotation);
            }


            {
                Transform t1 = new Transform();
                Debug.Log(t1.position);
                Debug.Log(t1.rotation);
                Debug.Log(t1.lossyScale);

                Transform t2 = new Transform();
                t2.parent = t1;

                t1.position = new Vector3(1, 0, 0);

                Debug.Log(t2.position);
                Debug.Log(t2.rotation);
                Debug.Log(t2.lossyScale);

                Debug.Log("LocalToWorldL " + t2.localToWorldMatrix);
                Debug.Log("worldToLocalL " + t2.worldToLocalMatrix);

                t2.forward = new Vector3(1, 1, 1);
                Debug.Log("forward rotationL " + t2.rotation);
            }
            Debug.Log("==========================LogicTransform.Test end");
        }

        public Vector3 position
        {
            get
            {
                if (m_isWorldPositionNeedUpdate)
                {
                    m_worldPosition = convertLocalToWorldPosition(Vector3.zero);
                    m_isWorldPositionNeedUpdate = false;
                }
                return m_worldPosition;
            }

            set
            {
                m_isWorldPositionNeedUpdate = true;
                m_localPostion = convertWorldToLocalPosition(value);
                SetChildNeedUpdate();
            }
        }

        public Vector3 localPosition
        {
            get
            {
                return m_localPostion;
            }
            set
            {
                m_isWorldPositionNeedUpdate = true;
                m_localPostion = value;
                SetChildNeedUpdate();
            }
        }

        Vector3 convertWorldToLocalPosition(Vector3 worldPos)
        {
            if (m_needParentUpdate)
            {
                _updateFromParent();
            }

            Quaternion inverse = Quaternion.Inverse(GetParentOrientation());
            Vector3 divide = MathHelper.Divide(worldPos - GetParentPosition(), GetParentScale());
            return inverse * divide;
        }

        Vector3 convertLocalToWorldPosition(Vector3 localPos)
        {
            if (m_needParentUpdate)
            {
                _updateFromParent();
            }
            //Matrix4x4 matrix = Matrix4x4.TRS(_getDerivedPosition(), _getDerivedOrientation(), _getDerivedScale());
            Matrix4x4 matrix = GetSelfTransform();
            return matrix.MultiplyPoint(localPos);
        }

        void SetChildNeedUpdate()
        {
            //if(m_parent == null)
            {
                m_needParentUpdate = true;
                //m_isWorldPositionNeedUpdate = true;
            }

            foreach (var iter in m_childList)
            {
                iter.SetNeedUpdateParent();
            }
        }

        void SetNeedUpdateParent()
        {
            m_needParentUpdate = true;
            //m_isWorldPositionNeedUpdate = true;

            SetChildNeedUpdate();
        }

        void _updateFromParent()
        {
            if (m_parent != null)
            {
                Quaternion parentOrientation = m_parent._getDerivedOrientation();
                m_DerivedOrientation = parentOrientation * m_localRotation;
                Vector3 parentScale = m_parent._getDerivedScale();
                m_DerivedScale = Vector3.Scale(parentScale, m_localScale);
                // Change position vector based on parent's orientation & scale
                m_DerivedPosition = parentOrientation * Vector3.Scale(parentScale, m_localPostion);
                // Add altered position vector to parents
                m_DerivedPosition += m_parent._getDerivedPosition();
            }
            else
            {
                m_DerivedOrientation = m_localRotation;
                m_DerivedPosition = m_localPostion;
                m_DerivedScale = m_localScale;
            }

            m_needParentUpdate = false;
            m_isWorldPositionNeedUpdate = true;
        }

        Quaternion GetParentOrientation()
        {
            if (m_parent != null)
            {
                return m_parent._getDerivedOrientation();
            }
            else
            {
                return Quaternion.identity;
            }
        }

        Vector3 GetParentScale()
        {
            if (m_parent != null)
            {
                return m_parent._getDerivedScale();
            }
            else
            {
                return Vector3.one;
            }
        }

        Vector3 GetParentPosition()
        {
            if (m_parent != null)
            {
                return m_parent._getDerivedPosition();
            }
            else
            {
                return Vector3.zero;
            }
        }

        Quaternion _getDerivedOrientation()
        {
            //         if(m_parent == null)
            //         {
            //             return m_localRotation;
            //         }
            //         else
            //         {
            if (m_needParentUpdate)
            {
                _updateFromParent();
            }
            return m_DerivedOrientation;
            //        }
        }

        Vector3 _getDerivedScale()
        {
            //         if(m_parent == null)
            //         {
            //             return m_localScale;
            //         }
            //         else
            //         {
            if (m_needParentUpdate)
            {
                _updateFromParent();
            }
            return m_DerivedScale;
            //        }
        }

        Vector3 _getDerivedPosition()
        {
            //         if(m_parent == null)
            //         {
            //             return m_localPostion;
            //         }
            //         else
            //         {
            if (m_needParentUpdate)
            {
                _updateFromParent();
            }
            return m_DerivedPosition;
            //        }
        }

        public Quaternion rotation
        {
            get
            {
                return convertLocalToWorldOrientation(m_localRotation);
            }

            set
            {
                //m_isWorldPositionNeedUpdate = true;

                m_localRotation = convertWorldToLocalOrientation(value);
                SetChildNeedUpdate();
            }
        }

        public Vector3 localEulerAngles
        {
            get
            {
                return m_localRotation.eulerAngles;
            }
            set
            {
                m_localRotation = Quaternion.Euler(value);
                SetChildNeedUpdate();
            }
        }

        public Vector3 eulerAngles
        {
            get
            {
                return rotation.eulerAngles;
            }
            set
            {
                rotation = Quaternion.Euler(value);
            }
        }

        Quaternion convertLocalToWorldOrientation(Quaternion localOrientation)
        {
            if (m_needParentUpdate)
            {
                _updateFromParent();
            }
            return GetParentOrientation() * localOrientation;
        }

        Quaternion convertWorldToLocalOrientation(Quaternion worldOrientation)
        {
            if (m_needParentUpdate)
            {
                _updateFromParent();
            }
            return Quaternion.Inverse(GetParentOrientation()) * worldOrientation;
        }

        public Transform parent
        {
            get
            {
                return m_parent;
            }

            set
            {
                Vector3 cacheWorldPos = position;
                Quaternion cacheWorldQua = rotation;
                Vector3 cacheWorldScale = lossyScale;

                if (m_parent != null)
                {
                    m_parent.m_childList.Remove(this);
                }

                m_parent = value;

                if (m_parent != null)
                {
                    m_parent.m_childList.Add(this);
                }

                SetChildNeedUpdate();

                position = cacheWorldPos;
                rotation = cacheWorldQua;
                lossyScale = cacheWorldScale;
            }
        }

        public Quaternion localRotation
        {
            get
            {
                return m_localRotation;
            }
            set
            {
                //m_isWorldPositionNeedUpdate = true;
                m_localRotation = value;
                SetChildNeedUpdate();
            }
        }

        public Vector3 localScale
        {
            get
            {
                return m_localScale;
            }
            set
            {
                //m_isWorldPositionNeedUpdate = true;
                m_localScale = value;
                SetChildNeedUpdate();
            }
        }

        public Vector3 lossyScale
        {
            get
            {
                return Vector3.Scale(m_DerivedScale, m_localScale);
            }
            set
            {
                //m_isWorldPositionNeedUpdate = true;
                m_localScale = MathHelper.Divide(lossyScale, m_DerivedScale);
                SetChildNeedUpdate();
            }
        }

        Matrix4x4 GetSelfTransform()
        {
            Matrix4x4 matrix = Matrix4x4.TRS(_getDerivedPosition(), _getDerivedOrientation(), _getDerivedScale());
            return matrix;
        }

        //     Matrix4x4 GetSelfTransform()
        //     {
        //         Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, lossyScale);
        //         return matrix;
        //     }

        public Matrix4x4 localToWorldMatrix
        {
            get
            {

                return GetSelfTransform();
            }
        }

        public Matrix4x4 worldToLocalMatrix
        {
            get
            {
                //Matrix4x4 matrix = Matrix4x4.TRS(_getDerivedPosition(), _getDerivedOrientation(), _getDerivedScale());
                return GetSelfTransform().inverse;
            }
        }

        public Vector3 forward
        {
            get
            {
                return rotation * Vector3.forward;
            }

            set
            {
                //Quaternion forwardQua = Quaternion.identity;
                //Quaternion newForwardQua = RotateHelper.DirectionToRotation(value);
                //Quaternion delta = Quaternion.FromToRotation(forward, value);
                //rotation = rotation * delta;
                //SetChildNeedUpdate();
                rotation = Quaternion.LookRotation(value);
            }
        }

        public Vector3 right
        {
            get
            {
                return rotation * Vector3.right;
            }
            set
            {
                rotation = Quaternion.FromToRotation(Vector3.right, value);
            }
        }

        public Vector3 up
        {
            get
            {
                return rotation * Vector3.up;
            }
            set
            {
                rotation = Quaternion.FromToRotation(Vector3.up, value);
            }
        }

        public Vector3 TransformPoint(Vector3 local)
        {
            return localToWorldMatrix.MultiplyPoint(local);
        }
    }
}