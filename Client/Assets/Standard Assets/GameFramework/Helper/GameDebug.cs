using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

public class GameDebug
{
    public static int LEVEL_LOG = 2;
    public static int LEVEL_ERROR = 1;

    public static int logLevel = 2;

    private static string logPath;
    private static Queue<string> logQueue = new Queue<string>();
    private static Queue<string> errorQueue = new Queue<string>();

    private static StreamWriter logWriter;
    private static StreamWriter errorWriter;
    private static int logCount = 0;

    private static bool winConsole = false;
    private static bool writeLog = true;
    private static bool writeError = true;

    public static string LogPath
    {
        get
        {
            if (logPath == null)
            {
                logPath = Path.Combine(GameResPath.persistentDataPath, "Log");
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
            }
            return logPath;
        }
    }

    public static void Init(bool startLog)
    {
        Application.logMessageReceived += OnMessageReceived;

        if (startLog)
        {
            logLevel = LEVEL_LOG;
        }
        else
        {
            logLevel = LEVEL_ERROR;
        }

#if UNITY_STANDALONE_WIN
        if (WinAPI::ContainCmdArg("-console"))
        {
            winConsole = true;
            WinConsole.Init();
        }
#endif
    }

    public static void SetLogLevel(int level)
    {
        logLevel = level;
    }

    public static void Release()
    {
        if (logWriter != null)
        {
            logWriter.Close();
            logWriter = null;
        }
        if (errorWriter != null)
        {
            errorWriter.Close();
            errorWriter = null;
        }

        if (winConsole)
        {
            WinConsole.Release();
        }

        Application.logMessageReceived -= OnMessageReceived;
    }

    public static void CallUpdate()
    {
        while (logQueue.Count > 0)
        {
            string msg = logQueue.Dequeue();
            Log(msg);
        }
        while (errorQueue.Count > 0)
        {
            string msg = errorQueue.Dequeue();
            LogError(msg);
        }
    }

    private static void OnMessageReceived(string msg, string stackTrace, LogType logType)
    {
        if (string.IsNullOrEmpty(msg))
            return;

        if (logType == LogType.Error || logType == LogType.Exception)
        {
            if (winConsole)
            {
                WinConsole.LogError(msg);
            }
            Log(msg);
            LogError(msg, stackTrace);
        }
    }

    public static void Log(object msg, Object context = null)
    {
        if (logLevel >= LEVEL_LOG)
        {
            UnityEngine.Debug.Log(msg, context);
            string s = msg.ToString();
            if (writeLog)
            {
                WriteLog(s);
            }
            if (winConsole)
            {
                WinConsole.Log(s);
            }
        }
    }

    public static void LogError(string msg, string stackTrace = null)
    {
        if (logLevel >= LEVEL_ERROR)
        {
            UnityEngine.Debug.LogError(msg);
            string s = msg.ToString();
            if (writeError)
            {
                WriteError(s, stackTrace);
            }
            if (winConsole)
            {
                WinConsole.LogError(s);
            }
        }
    }

    public static void LogException(System.Exception e)
    {
        if (logLevel >= LEVEL_ERROR)
        {
            UnityEngine.Debug.LogException(e);

            string s = e.ToString();
            if (writeError)
            {
                WriteError(s, e.StackTrace);
            }

            if (winConsole)
            {
                WinConsole.LogError(s);
            }
        }
    }

    public static void LogInMainThread(string msg)
    {
        if (logLevel >= LEVEL_LOG)
        {
            logQueue.Enqueue(msg);
        }
    }

    public static void LogErrorInMainThread(string msg)
    {
        if (logLevel >= LEVEL_ERROR)
        {
            errorQueue.Enqueue(msg);
        }
    }

    public static void LogStackTrace()
    {
        StackTrace stackTrace = new StackTrace();
        StackFrame[] stackFrames = stackTrace.GetFrames();
        StringBuilder tb = new StringBuilder();
        for (int i = 0; i < stackFrames.Length; i++)
        {
            var method = stackFrames[i].GetMethod();
            if (method.Name.Equals("LogStack()"))
                continue;

            string text = string.Format("{0}:{1}\n", method.ReflectedType != null ? method.ReflectedType.Name : string.Empty, method.Name);
            tb.Insert(0, text);
        }
        tb.Insert(0, "-------LogStackTrack--------\n");
        UnityEngine.Debug.Log(tb.ToString());
    }

    private static void WriteLog(string msg)
    {
        logCount += 1;
        if (logWriter == null || logCount > 10000)
        {
            logWriter = CreateLogWriter();
            logCount = 0;
        }
        try
        {
            logWriter.WriteLine(msg);
            logWriter.Flush();
        }
        catch (System.Exception)
        {
            if (logWriter != null)
            {
                logWriter.Close();
            }
        }
    }

    private static void WriteError(string msg, string stackTrace)
    {
        if (errorWriter == null)
        {
            errorWriter = CreateErrorWriter();
        }
        try
        {
            errorWriter.WriteLine(msg);
            if (!string.IsNullOrEmpty(stackTrace))
            {
                errorWriter.WriteLine(stackTrace);
            }
            errorWriter.WriteLine("");
            errorWriter.Flush();
        }
        catch (System.Exception)
        {
            if (errorWriter != null)
            {
                errorWriter.Close();
            }
        }
    }

    private static StreamWriter CreateLogWriter()
    {
        StreamWriter streamWriter = null;
        int index = 0;
        string path;
        while (streamWriter == null)
        {
            if (index == 0)
            {
                path = string.Format("{0}/{1}.log", LogPath, System.DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            }
            else
            {
                path = string.Format("{0}/{1}_{2}.log", LogPath, System.DateTime.Now.ToString("yyyyMMdd_HHmmss"), index);
            }

            try
            {
                if (File.Exists(path))
                {
                    streamWriter = File.AppendText(path);
                }
                else
                {
                    streamWriter = File.CreateText(path);
                }
            }
            catch (System.Exception)
            {
                index++;
            }
        }
        return streamWriter;
    }

    private static StreamWriter CreateErrorWriter()
    {
        StreamWriter streamWriter = null;
        string path = string.Format("{0}/{1}.err", LogPath, System.DateTime.Now.ToString("yyyyMMdd_HH"));
        try
        {
            if (File.Exists(path))
            {
                streamWriter = File.AppendText(path);
            }
            else
            {
                streamWriter = File.CreateText(path);
            }
        }
        catch (System.Exception)
        {
            streamWriter = null;
        }
        return streamWriter;
    }
}
