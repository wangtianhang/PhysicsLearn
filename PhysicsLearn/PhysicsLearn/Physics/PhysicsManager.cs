using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine
{
    class PhysicsManager
    {
        public void Init()
        {

        }

        public bool RayCast(Ray3d ray, float distance, ref RaycastHit outhit, int mask)
        {
            return false;
        }

        public RaycastHit[] RaycastAll(Ray3d ray, float distance, int mask)
        {
            return new RaycastHit[0];
        }

        public bool SphereTest(Vector3 p, float radius, int mask)
        {
            return false;
        }

        public Collider[] OverlapSphere(Vector3 p, float radius, int mask)
        {
            return new Collider[0];
        }

        public bool CapsuleCast(Vector3 p0, Vector3 p1, float radius, Vector3 direction, float distance, ref RaycastHit outHit, int mask)
        {
            return false;
        }

        public RaycastHit[] CapsuleCastAll(Vector3 p0, Vector3 p1, float radius, Vector3 direction, float distance, int mask)
        {
            return new RaycastHit[0];
        }
    }
}
