using System.Collections.Generic;
using UnityEngine;
using StoryScript;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// This file handles all GM-related UI logic. Considering that one project may be used for
/// experiments with multiple functions, the UI is configurable, but the logic is all written
/// here.
/// We provide a loadui ("ui configuration resource") command to load resources, but generally
/// you can load the resources you want to use for the current experiment directly in Start.
/// If you don't want to re-output the package, you can use the command to switch ui.
/// The ui configuration uses MetaDSL syntax, but the script interpreter is not considered here,
/// because the functional codes of the experiment are hard-coded in the project, and using
/// scripts is of little significance and may complicate things.
/// </summary>
[UnityEngine.Scripting.Preserve]
public class UiHandler : MonoBehaviour
{
    public GameObject LabelTemplate;
    public GameObject InputTemplate;
    public GameObject ButtonTemplate;
    public GameObject DropDownTemplate;
    public GameObject ToggleTemplate;
    public GameObject ToggleGroupTemplate;
    public GameObject SliderTemplate;

    private void Start()
    {
        m_DebugUi.LabelTemplate = LabelTemplate;
        m_DebugUi.InputTemplate = InputTemplate;
        m_DebugUi.ButtonTemplate = ButtonTemplate;
        m_DebugUi.DropDownTemplate = DropDownTemplate;
        m_DebugUi.ToggleTemplate = ToggleTemplate;
        m_DebugUi.ToggleGroupTemplate = ToggleGroupTemplate;
        m_DebugUi.SliderTemplate = SliderTemplate;

        m_DebugUi.ApiObject = this;
        m_DebugUi.OnLoadUi = LoadUi;
        m_DebugUi.OnShowUi = ShowUi;
        m_DebugUi.OnInitUi = OnInitUi;
        m_DebugUi.Init();

        //The UI of the current experiment is loaded here. Only one is the current one.
        LoadUi(c_TestUI);
    }
    public void ShowUi()
    {
        m_DebugUi.ShowRootUi();
        OnUiShow(m_DebugUi.CurUiRes);
    }
    public void HideUi()
    {
        m_DebugUi.HideRootUi();
    }
    public void LoadUi(string res)
    {
        var ta = Resources.Load<TextAsset>(res);
        m_DebugUi.LoadUi(ta);
    }
    public void LoadAndShowIndexUi()
    {
        m_DebugUi.LoadAndShowIndexUi();
    }

    //UI configuration is a .txt file in the Resources directory
    //After the UI is loaded, initialization processing is performed, mainly initial value setting.
    private void OnInitUi(string res)
    {
        if (res == c_TestUI) {
            InitTestUi();
        }
    }
    //Values on the UI can be reset or synchronized while the UI is displayed
    private void OnUiShow(string res)
    {
        m_DebugUi.CheckTmpFont();
        if (res == c_TestUI) {
            InitTestUi();
        }
    }

    //Event handling corresponding to TestUI configuration
    private void InitTestUi()
	{
	}
    private void OnButton()
    {
        LogSystem.Warn("OnButton");
    }
    private void OnValueChanged(int val)
    {
        LogSystem.Warn("OnValueChanged {0}", val);
    }
    private void OnCheckedChanged(bool val)
    {
        LogSystem.Warn("OnCheckedChanged {0}", val);
    }
    private void OnOneChanged(bool val)
    {
        LogSystem.Warn("OnOneChanged {0}", val);
    }
    private void OnTwoChanged(bool val)
    {
        LogSystem.Warn("OnTwoChanged {0}", val);
    }
    private void OnSliderChanged(float val)
    {
        LogSystem.Warn("OnSliderChanged {0}", val);
    }
    private const string c_TestUI = "TestUI";

    private TabularDebugUI m_DebugUi = new TabularDebugUI(new List<KeyValuePair<string, string>>
        {
        	KeyValuePair.Create(c_TestUI, c_TestUI),
        });
}
