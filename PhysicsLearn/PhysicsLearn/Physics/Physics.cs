using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine
{
    class Physics
    {
        public static bool CheckSphere(Vector3 position, float radius)
        {
            return false;
        }

        public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo)
        {
            hitInfo = new RaycastHit();
            return false;
        }

        public static Collider[] OverlapSphere(Vector3 position, float radius)
        {
            return null;
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance)
        {
            hitInfo = new RaycastHit();
            return false;
        }

        public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float maxDistance)
        {
            return null;
        }

        public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance)
        {
            hitInfo = new RaycastHit();
            return false;
        }

        public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, float maxDistance)
        {
            return null;
        }
    }
}

