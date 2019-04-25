using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine
{
    
    public class MonoManager
    {
        public List<MonoBehaviour> m_monoList = new List<MonoBehaviour>();

        public void AddMono(MonoBehaviour mono)
        {
            m_monoList.Add(mono);

            //Type type = typeof(MonoBehaviour);
            Type type = mono.GetType();
            mono.m_awakeCallback = type.GetMethod("Awake");
            mono.m_startCallback = type.GetMethod("Start");
            mono.m_updateCallback = type.GetMethod("Update");
            mono.m_destroyCallback = type.GetMethod("Destroy");

            if(mono.m_awakeCallback != null)
            {
                mono.m_awakeCallback.Invoke(mono, null);
            }
        }

        void Update()
        {
            foreach(var iter in m_monoList)
            {
                if(iter.enabled)
                {
                    if(!iter.m_hasStart)
                    {
                        iter.m_hasStart = true;
                        if(iter.m_startCallback != null)
                        {
                            iter.m_startCallback.Invoke(iter, null);
                        }
                    }
                    if (iter.m_updateCallback != null)
                    {
                        iter.m_updateCallback.Invoke(iter, null);
                    }
                }
            }
        }
    }
}

