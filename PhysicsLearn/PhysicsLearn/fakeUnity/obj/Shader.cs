using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*using OpenTK.Graphics.OpenGL4;*/

class Shader
{
    public string m_vertexShaderSrc;
    public string m_pixelShaderSrc;
    
    public bool m_useZWrite = true;
    public bool m_useZTest = true;
    public bool m_useBlend = false;

    public bool m_useCull = true;
    public bool m_cullBack = true;

    public static Shader Find(string name)
    {
        if(name == "Diffuse")
        {

        }

        return null;
    }


}

