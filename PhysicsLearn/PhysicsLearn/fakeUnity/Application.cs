using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine
{
    class Application
    {
        SceneMgr m_sceneMgr = null;

        public void Init()
        {
            m_sceneMgr = new SceneMgr();
            m_sceneMgr.Init(this);
        }
    }

    /*
    class Application : IDemo
    {
        MainWindow m_mainWindow = null;
        string m_titlePrefix = null;

        FakeUnityDemo m_demo = null;

        //GoManager m_goMgr = null;
        SceneMgr m_sceneMgr = null;
        RenderMgr m_renderMgr = null;

        public void Init(MainWindow mainWindow)
        {
            m_mainWindow = mainWindow;
            m_titlePrefix = "dreamstatecoding" + ": OpenGL Version: " + GL.GetString(StringName.Version);

            //m_goMgr = new GoManager();
            //m_goMgr.Init(this);

            m_sceneMgr = new SceneMgr();
            m_sceneMgr.Init(this);

            m_renderMgr = new RenderMgr();
            m_renderMgr.Init(this);

            m_demo = new FakeUnityDemo();
            m_demo.LoadScene();
        }

        public RenderMgr GetRenderMgr()
        {
            return m_renderMgr;
        }

//         public GoManager _GetGoManager()
//         {
//             return m_goMgr;
//         }

        public void OnUpdateFrame(FrameEventArgs e)
        {
            HandleKeyboard();

            m_sceneMgr.Update();
        }

        public void OnRenderFrame(FrameEventArgs e)
        {
            m_mainWindow.Title = m_titlePrefix + " Vsync " + m_mainWindow.VSync + " FPS " + (1 / e.Time).ToString("f0");

            m_sceneMgr.Render();

            GL.Finish();
            
            //m_mainWindow.SwapBuffers();
        }

        void HandleKeyboard()
        {
            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Key.Escape))
            {
                m_mainWindow.Exit();
            }

            MouseState mouseState = Mouse.GetState();
            if (mouseState.IsButtonDown(MouseButton.Left))
            {
                //Console.WriteLine("mouse left btn down");
            }
        }
    }
    */
}

