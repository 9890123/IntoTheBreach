using System;
using UnityEngine;

public class BuiltInDialogueViewController : MonoBehaviour
{
    public const string BuiltInDialogueViewPath = "Built-inAssets/BuiltInDialogueView";
    private static BuiltInDialogueViewController _instance;

    public Action _cancelHandler;
    public Action _okHandler;

    private UIButton simple_CancelButton;
    private UILabel simple_CancelLabel;
    private UILabel simple_InfoLabel;
    private UIButton simple_OKButton;
    private UILabel simple_OKLabel;

    public static void OpenView(string msg,
        Action okHandler = null,
        Action cancelHandler = null,
        UIWidget.Pivot pivot = UIWidget.Pivot.Center,
        string okLabelStr = "确定", string cancelLabelStr = "取消")
    {
        if (_instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>(BuiltInDialogueViewPath);
            var obj = UnityEngine.GameObject.Find("GameRoot/UIRoot");
            var root = obj == null ? UICamera.eventHandler.gameObject : obj;
            GameObject module = NGUITools.AddChild(root, prefab);
            var com = module.AddMissingComponent<BuiltInDialogueViewController>();
            com.InitView();
            _instance = com;
        }
        _instance.Open(msg, okHandler, cancelHandler, pivot, okLabelStr, cancelLabelStr);
    }

    public static void CloseView()
    {
        if (_instance != null)
        {
            _instance.Dispose();
            Destroy(_instance.gameObject);
            _instance = null;
        }
    }

    public void Open(string msg,
        Action okHandler,
        Action cancelHandler,
        UIWidget.Pivot pivot,
        string okLabelStr, string cancelLblStr)
    {
        simple_InfoLabel.pivot = pivot;
        simple_InfoLabel.text = msg;

        _okHandler = okHandler;
        _cancelHandler = cancelHandler;

        simple_OKLabel.text = okLabelStr;
        simple_OKLabel.spacingX = GetLabelSpacingX(okLabelStr);

        if (_cancelHandler != null)
        {
            simple_CancelLabel.text = cancelLblStr;
            simple_CancelLabel.spacingX = GetLabelSpacingX(cancelLblStr);
            simple_CancelButton.gameObject.SetActive(true);
        }
        else
        {
            simple_OKButton.transform.localPosition = new Vector3(0, -48, 0);
            simple_CancelButton.gameObject.SetActive(false);
        }
    }

    public void initView()
    {
        Transform root = transform;
        simple_InfoLabel = root.Find("SimpleWin/simple_InfoLabel").GetComponent<UILabel>();
        simple_OKButton = root.Find("SimpleWin/simple_OKButton").GetComponent<UIButton>();
        simple_OKLabel = root.Find("SimpleWin/simple_OKButton/simple_OKLabel").GetComponent<UILabel>();
        simple_CancelButton = root.Find("SimpleWin/simple_CancelButton").GetComponent<UIButton>();
        simple_CancelLabel = root.Find("SimpleWin/simple_CancelButton/simple_CancelLabel").GetComponent<UILabel>();

        RegisterEvent();
    }

    public void RegisterEvent()
    {
        EventDelegate.Set(simple_OKButton.onClick, OnClickOkButton);
        EventDelegate.Set(simple_CancelButton.onClick, OnClickCancelButton);
    }

    public void Dispose()
    {

    }

    private void OnClickOkButton()
    {
        CloseView();

        if (_okHandler != null)
        {
            _okHandler();
        }
    }

    private void OnClickCancelButton()
    {
        CloseView();

        if (_cancelHandler != null)
        {
            _cancelHandler();
        }
    }

    private int GetLabelSpacingX(string text)
    {
        if (text.Length <= 2)
        {
            return 12;
        }
        if (text.Length <= 3)
        {
            return 6;
        }
        return 1;
    }
}
