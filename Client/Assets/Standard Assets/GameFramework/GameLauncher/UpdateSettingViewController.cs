using System;
using System.IO;
using UnityEngine;
using AssetPipeline;

public class UpdateSettingViewController : MonoBehaviour
{
    public enum UpdateMode
    {
        DEV_TEST,
        OFFICIAL_TEST,
        OFFICIAL,
        SKIP_UPDATE,
    }

    public const string BuiltInDialogueViewPath = "Built-inAssets/UpdateSettingView";
    private static UpdateSettingViewController _instance;

    public Action<UpdateMode> _okHandler;

    private UIButton button1;
    private UIButton button2;
    private UIButton button3;
    private UIButton button4;
    private UIButton button5;

    public static void OpenView(Action<UpdateMode> okHandler)
    {
        if (_instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>(BuiltInDialogueViewPath);
            GameObject module = NGUITools.AddChild(UICamera.eventHandler.gameObject, prefab);
            var com = module.AddMissingComponent<UpdateSettingViewController>();
            com.InitView();
            _instance = com;
            com._okHandler = okHandler;
        }
    }

    public void InitView()
    {
        Transform root = transform;
        button1 = root.Find("Button1").GetComponent<UIButton>();
        button2 = root.Find("Button2").GetComponent<UIButton>();
        button3 = root.Find("Button3").GetComponent<UIButton>();
        button4 = root.Find("Button4").GetComponent<UIButton>();
        button5 = root.Find("Button5").GetComponent<UIButton>();
        EventDelegate.Set(button1.onClick, OnClickButton1);
        EventDelegate.Set(button2.onClick, OnClickButton2);
        EventDelegate.Set(button3.onClick, OnClickButton3);
        EventDelegate.Set(button4.onClick, onClickButton4);
        EventDelegate.Set(button5.onClick, onClickButton5);
    }

    private void OnClickButton1()
    {
        CloseView();

        if (_okHandler != null)
        {
            _okHandler(UpdateMode.DEV_TEST);
        }
    }

    private void OnClickButton2()
    {
        CloseView();

        if (_okHandler != null)
        {
            _okHandler(UpdateMode.OFFICIAL_TEST);
        }
    }

    private void OnClickButton3()
    {
        CloseView();

        if (_okHandler != null)
        {
            _okHandler(UpdateMode.OFFICIAL);
        }
    }

    private void OnClickButton4()
    {
        string dir = GameResPath.persistentDataPath;
        AssetUpdate.Instance.CleanUpBundleResFolder();
        AssetUpdate.Instance.CleanUpDllFolder();
        PlatformAPI.RestartGame();
    }

    private void OnClickButton5()
    {
        CloseView();

        if (_okHandler != null)
        {
            _okHandler(UpdateMode.SKIP_UPDATE);
        }
    }

    private void CloseView()
    {
        if (_instance != null)
        {
            Destroy(_instance.gameObject);
            _instance = null;
        }
    }
}
