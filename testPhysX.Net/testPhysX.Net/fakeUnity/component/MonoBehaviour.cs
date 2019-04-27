using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine
{
    public class MonoBehaviour : Behaviour
    {
        public MethodInfo m_awakeCallback = null;
        public MethodInfo m_startCallback = null;
        public bool m_hasStart = false;
        public MethodInfo m_updateCallback = null;
        public MethodInfo m_destroyCallback = null;
    }
}
