using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;
using System.IO;

public class MasterLogger : MonoBehaviour
{
    public static MasterLogger ins = null;

    public ScrollRect sr;
    public TMPro.TMP_Text text;

    public StringBuilder sb;

    private static int ind;

    private void Awake()
    {

        ins = this;
        ind = 0;
        sb = new StringBuilder("-------로그-------\n");
    }

    private void Start()
    {
        //Error("임시에러");
        //StartCoroutine(Wowwow());
    }

    IEnumerator Wowwow()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            Log("임시");
        }
    }

    public static void Good(string log)
    {
        Debug.Log($"<color=green>{log}</color>");
        //ins.Add($"<color=green>{log}</color>");
    }

    public static void Error(string log)
    {
        Debug.LogError(log);
        //ins.Add($"<color=red>{log}</color>");
    }

    public static void Log(string log)
    {
        Debug.Log(log);
        //ins.Add(log);
    }

    private void Add(string log)
    {
        string d = DateTime.Now.ToString("u")[0..^1];
        sb.AppendLine($"[{d}] {++ind} : {log}");
        text.SetText(sb.ToString());

        if (sr.verticalNormalizedPosition < 0.001f)
        {
            Canvas.ForceUpdateCanvases();

            sr.verticalNormalizedPosition = 0f;
        }
    }



    public void CopyLog()
    {
        GUIUtility.systemCopyBuffer = sb.ToString();              
    }





    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            Add($"<color=red>{logString}</color>");
        } else
        {
            Add(logString);
        }
    }
}
