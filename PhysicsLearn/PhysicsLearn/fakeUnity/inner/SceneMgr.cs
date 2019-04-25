using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine
{
    class SceneMgr
    {
        Application m_application = null;


        List<GameObject> m_goList = new List<GameObject>();

        public void Init(Application application)
        {
            m_application = application;
        }

        public void Update()
        {

        }

//         public void Render()
//         {
//             foreach(var iter in m_goList)
//             {
//                 MeshFilter meshFilter = iter.GetComponent<MeshFilter>();
//                 MeshRenderer meshRender = iter.GetComponent<MeshRenderer>();
//                 if(meshFilter != null && meshRender != null)
//                 {
//                     m_application.GetRenderMgr().RenderMesh(meshFilter, meshRender);
//                 }
//             }
//         }
    }
}

