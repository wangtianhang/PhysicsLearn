using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine
{
    public class GameObject : Object
    {
        public GameObject()
        {

        }

        public GameObject(string name)
        {
            this.name = name;
        }

        public Transform transform;

        MonoManager m_monoMgr = new MonoManager();

        public T AddComponent<T>() where T : Component
        {
            T t = default(T);
            if(t is MonoBehaviour)
            {
                m_monoMgr.AddMono(t as MonoBehaviour);
            }
            return t;
        }

        public T GetComponent<T>() where T : Component
        {
            if (typeof(MonoBehaviour).IsAssignableFrom(typeof(T)))
            {
                foreach (var iter in m_monoMgr.m_monoList)
                {
                    if (iter is T)
                    {
                        return iter as T;
                    }
                }
            }

            return null;
        }
    }
}
