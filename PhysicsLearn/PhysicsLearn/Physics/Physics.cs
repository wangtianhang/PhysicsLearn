using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine
{
    class Physics
    {
        public static bool CheckSphere(Vector3 position, float radius, int mask)
        {
            return Application.GetPhysicsMgr().SphereTest(position, radius, mask);
        }

        public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo, int mask)
        {
            //hitInfo = new RaycastHit();
            //return false;
            Vector3 dir = end - start;
            return Raycast(start, dir, out hitInfo, dir.magnitude, mask);
        }

        public static Collider[] OverlapSphere(Vector3 position, float radius, int mask)
        {
            //return null;
            return Application.GetPhysicsMgr().OverlapSphere(position, radius, mask);
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int mask)
        {
            hitInfo = new RaycastHit();
            //hitInfo.collider = null;
            float dirLength = direction.magnitude;
            if(dirLength > 0)
            {
                Vector3 normalizedDirection = direction / dirLength;
                Ray3d ray = new Ray3d(origin, normalizedDirection);
                bool didHit = Application.GetPhysicsMgr().RayCast(ray, maxDistance, ref hitInfo, mask);
                if(didHit)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float maxDistance, int mask)
        {
            float dirLength = direction.magnitude;
            if (dirLength > 0)
            {
                Vector3 normalizedDirection = direction / dirLength;
                Ray3d ray = new Ray3d(origin, normalizedDirection);
                return Application.GetPhysicsMgr().RaycastAll(ray, maxDistance, mask);
            }
            else
            {
                return new RaycastHit[0];
            }
        }

        public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int mask)
        {
            //hitInfo = new RaycastHit();
            //return false;
            //hitInfo = new RaycastHit();
            return CapsuleCast(origin, origin, radius, direction, out hitInfo, maxDistance, mask);
        }

        public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float distance, int layermask)
        {
            hitInfo = new RaycastHit();
            float dirLength = direction.magnitude;
            if (dirLength > 0)
            {
                Vector3 normalizedDirection = direction / dirLength;
                return Application.GetPhysicsMgr().CapsuleCast(point1, point2, radius, normalizedDirection, distance, ref hitInfo, layermask);
            }
            else
            {
                return false;
            }
        }

        public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, float maxDistance, int mask)
        {
            return CapsuleCastAll(origin, origin, radius, direction, maxDistance, mask);
        }

        public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, int mask)
        {
            float dirLength = direction.magnitude;
            if (dirLength > 0)
            {
                Vector3 normalizedDirection = direction / dirLength;
                return Application.GetPhysicsMgr().CapsuleCastAll(point1, point2, radius, normalizedDirection, maxDistance, mask);
            }
            else
            {
                return new RaycastHit[0];
            }
        }

        /// <summary>
        /// 同步场景中的Collider到物理系统
        /// </summary>
        public static void SyncTransforms()
        {

        }
    }
}

