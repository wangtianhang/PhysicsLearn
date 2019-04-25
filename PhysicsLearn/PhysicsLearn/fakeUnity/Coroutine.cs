using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

class TestWWW
{
    public static void Test()
    {
        TestWWW test = new TestWWW();
        CoroutineMgr coroutineMgr = new CoroutineMgr();
        coroutineMgr.StartCoroutine(test.TestWWWCoroutine());
        while (true)
        {
            coroutineMgr.Update();
            Thread.Sleep(33);
        }
    }

    public IEnumerator TestWWWCoroutine()
    {
        WWW www = new WWW("http://www.baidu.com");
        yield return www;

        UnityEngine.Debug.Log("TestWWWCoroutine end");
    }
}

class CoroutineMgr
{
    public class CoroutineData
    {
        public IEnumerator m_coroutine = null;
        public System.Object m_current = null;
    }

    List<CoroutineData> m_coroutineList = new List<CoroutineData>();
    public void StartCoroutine(IEnumerator coroutine)
    {
        CoroutineData coroutineData = new CoroutineData();
        coroutineData.m_coroutine = coroutine;
        m_coroutineList.Add(coroutineData);
    }

    public void Update()
    {
        for (int i = m_coroutineList.Count - 1; i >= 0; --i )
        {
            CoroutineData iter = m_coroutineList[i];
            if(iter.m_current == null)
            {
                if (iter.m_coroutine.MoveNext())
                {
                    iter.m_current = iter.m_coroutine.Current;
                }
                else
                {
                    m_coroutineList.RemoveAt(i);
                }
            }
            else
            {
                if (iter.m_current is AsyncOperation)
                {
                    AsyncOperation current = iter.m_current as AsyncOperation;
                    if (current.isDone())
                    {
                        if (iter.m_coroutine.MoveNext())
                        {
                            iter.m_current = iter.m_coroutine.Current;
                        }
                        else
                        {
                            m_coroutineList.RemoveAt(i);
                        }
                    }
                    else
                    {
                        // 等待异步完成
                    }
                }
            }
        }
    }
}

public interface AsyncOperation
{
    bool isDone();

    float progress();
}

class WWW : AsyncOperation
{ 
    public class WebCommunication : WebClient
    {
        public int m_editorTimeOut = 3;

        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);

            request.Timeout = 1000 * 10;
            request.ReadWriteTimeout = 1000 * 10;

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            return request;
        }
    }

    WebCommunication m_webClient = null;
    bool m_isDone = false;
    string m_text = null;

    public WWW(string url)
    {
        m_webClient = new WebCommunication();

        m_webClient.DownloadDataCompleted += UploadDataCompletedEventHandler;
        m_webClient.Credentials = CredentialCache.DefaultCredentials;

        Uri uri = new Uri(url);
        m_webClient.DownloadDataAsync(uri, url);
    }

    void UploadDataCompletedEventHandler(object sender, DownloadDataCompletedEventArgs e)
    {
        string url = sender as string;

        if (e.Error == null
            || e.Error.Message == null
            || e.Error.Message == "")
        {
            m_text = System.Text.Encoding.UTF8.GetString(e.Result);
            UnityEngine.Debug.Log("UploadDataCompletedEventHandler " + m_text);
        }
        else
        {
            string errorMsg = "";
            if (e.Error != null && e.Error.Message != null)
            {
                errorMsg = e.Error.Message;
            }
            UnityEngine.Debug.LogError("UploadDataCompletedEventHandler error " + url + " " + errorMsg);
        }

        m_isDone = true;
    }

    public bool isDone()
    {
        return m_isDone;
    }

    public float progress()
    {
        return 0;
    }

    public string text 
    {
        get 
        { 
            return m_text; 
        }
    }
}

