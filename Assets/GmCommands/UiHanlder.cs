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
public class UiHanlder : MonoBehaviour
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
        m_RootUi = GameObject.Find("RootPanel");
        InitUiCells();
        HideUi();

        //The UI of the current experiment is loaded here. Only one is the current one.
        LoadUi(c_TestUI);
    }
    public void ShowUi()
    {
        m_RootUi.SetActive(true);
        OnUiShow(m_CurUiRes);
    }
    public void HideUi()
    {
        m_RootUi.SetActive(false);
    }
    public void LoadUi(string res)
    {
        ClearCells();
        m_UiLoaded = false;
        m_UiInited = false;

        var ta = Resources.Load<TextAsset>(res);
        string txt = ta.text;
        Dsl.DslFile file = new Dsl.DslFile();
        if (file.LoadFromString(txt, LogSystem.Log)) {
            foreach (var info in file.DslInfos) {
                string type = info.GetId();
                if (type == "label") {
                    BuildLabel(info);
                }
                else if (type == "input") {
                    BuildInput(info);
                }
                else if (type == "button") {
                    BuildButton(info);
                }
                else if (type == "dropdown") {
                    BuildDropdown(info);
                }
                else if (type == "toggle") {
                    BuildToggle(info);
                }
                else if (type == "toggle_group") {
                    BuildToggleGroup(info);
                }
                else if (type == "slider") {
                    BuildSlider(info);
                }
            }
        }

        m_CurUiRes = res;
        m_UiLoaded = true;
        OnUiInit(res);
        m_UiInited = true;
    }

    private void InitUiCells()
    {
        float ratioRow = 1.0f / c_CellRowNum;
        float ratioCol = 1.0f / c_CellColNum;
        m_Cells = new RectTransform[c_CellRowNum, c_CellColNum];
        for (int i = 0; i < c_CellRowNum; ++i) {
            for (int j = 0; j < c_CellColNum; ++j) {
                var cellObj = new GameObject("cell" + i + "" + j);
                cellObj.transform.SetParent(m_RootUi.transform);
                var cell = cellObj.AddComponent<RectTransform>();
                cell.pivot = new Vector2(0.5f, 0.5f);
                cell.anchorMin = new Vector2(ratioCol * j, ratioRow * (c_CellRowNum - i - 1));
                cell.anchorMax = new Vector2(ratioCol * (j + 1), ratioRow * (c_CellRowNum - i));
                cell.offsetMin = Vector2.zero;
                cell.offsetMax = Vector2.zero;
                cell.localScale = Vector3.one;
                m_Cells[i, j] = cell;
            }
        }
    }
    private void ClearCells()
    {
        foreach (var cell in m_Cells) {
            var list = new List<GameObject>();
            for (int ix = 0; ix < cell.childCount; ++ix) {
                var gobj = cell.GetChild(ix).gameObject;
                list.Add(gobj);
            }
            cell.DetachChildren();
            foreach (var gobj in list) {
                StoryScriptUtility.DestroyObject(gobj);
            }
        }

        m_UiControls.Clear();
    }

    private void BuildLabel(Dsl.ISyntaxComponent info)
    {
        var fd = info as Dsl.FunctionData;
        if (null != fd) {
            var id = fd.GetParamId(0);
            id = ResolveId(id);
            var rowStr = fd.GetParamId(1);
            var colStr = fd.GetParamId(2);
            var caption = fd.GetParamId(3);
            if (int.TryParse(rowStr, out var row) && int.TryParse(colStr, out var col)) {
                AddLabel(id, row, col, caption);
            }
        }
    }
    private void BuildInput(Dsl.ISyntaxComponent info)
    {
        var fd = info as Dsl.FunctionData;
        if (null != fd) {
            var id = fd.GetParamId(0);
            id = ResolveId(id);
            var rowStr = fd.GetParamId(1);
            var colStr = fd.GetParamId(2);
            var dataType = fd.GetParamId(3);
            bool hasDef = fd.GetParamNum() > 4;
            var defStr = hasDef ? fd.GetParamId(4) : string.Empty;
            if (int.TryParse(rowStr, out var row) && int.TryParse(colStr, out var col)) {
                AddInput(id, row, col, dataType, hasDef, defStr);
            }
        }
    }
    private void BuildButton(Dsl.ISyntaxComponent info)
    {
        var fd = info as Dsl.FunctionData;
        if (null != fd) {
            var id = fd.GetParamId(0);
            id = ResolveId(id);
            var rowStr = fd.GetParamId(1);
            var colStr = fd.GetParamId(2);
            var caption = fd.GetParamId(3);
            var method = fd.GetParamId(4);
            if (int.TryParse(rowStr, out var row) && int.TryParse(colStr, out var col)) {
                AddButton(id, row, col, caption, method);
            }
        }
    }
    private void BuildDropdown(Dsl.ISyntaxComponent info)
    {
        var fd = info as Dsl.FunctionData;
        if (null != fd && fd.IsHighOrder && fd.HaveStatement()) {
            var cd = fd.LowerOrderFunction;
            var id = cd.GetParamId(0);
            id = ResolveId(id);
            var rowStr = cd.GetParamId(1);
            var colStr = cd.GetParamId(2);
            var method = cd.GetParamId(3);
            bool hasDef = fd.GetParamNum() > 4;
            var selStr = hasDef ? cd.GetParamId(4) : string.Empty;
            if (int.TryParse(rowStr, out var row) && int.TryParse(colStr, out var col)) {
                var dropdown = AddDropdown(id, row, col, method);
                var list = new List<string>();
                foreach (var p in fd.Params) {
                    list.Add(p.GetId());
                }
                dropdown.AddOptions(list);
                if (hasDef && int.TryParse(selStr, out var select)) {
                    dropdown.value = select;
                }
            }
        }
    }
    private void BuildToggle(Dsl.ISyntaxComponent info)
    {
        var fd = info as Dsl.FunctionData;
        if (null != fd) {
            var id = fd.GetParamId(0);
            id = ResolveId(id);
            var rowStr = fd.GetParamId(1);
            var colStr = fd.GetParamId(2);
            var caption = fd.GetParamId(3);
            var method = fd.GetParamId(4);
            bool hasDef = fd.GetParamNum() > 5;
            var checkStr = hasDef ? fd.GetParamId(5) : string.Empty;
            if (int.TryParse(rowStr, out var row) && int.TryParse(colStr, out var col)) {
                AddToggle(id, row, col, caption, method, hasDef, checkStr == "true" || checkStr == "True");
            }
        }
    }
    private void BuildToggleGroup(Dsl.ISyntaxComponent info)
    {
        var fd = info as Dsl.FunctionData;
        if (null != fd && fd.IsHighOrder && fd.HaveStatement()) {
            var cd = fd.LowerOrderFunction;
            var id = cd.GetParamId(0);
            id = ResolveId(id);
            var rowStr = cd.GetParamId(1);
            var colStr = cd.GetParamId(2);
            if (int.TryParse(rowStr, out var row) && int.TryParse(colStr, out var col)) {
                var group = AddToggleGroup(id, row, col);
                foreach (var p in fd.Params) {
                    var pfd = p as Dsl.FunctionData;
                    if (null != pfd && pfd.GetId() == "toggle") {
                        var pid = pfd.GetParamId(0);
                        pid = ResolveId(pid);
                        var pcap = pfd.GetParamId(1);
                        var method = pfd.GetParamId(2);
                        bool hasDef = fd.GetParamNum() > 3;
                        var pchecked = hasDef ? pfd.GetParamId(3) : string.Empty;
                        AddToToggleGroup(group, pid, row, col++, pcap, method, hasDef, pchecked == "true" || pchecked == "True");
                    }
                }
            }
        }
    }
    private void BuildSlider(Dsl.ISyntaxComponent info)
    {
        var fd = info as Dsl.FunctionData;
        if (null != fd) {
            var id = fd.GetParamId(0);
            id = ResolveId(id);
            var rowStr = fd.GetParamId(1);
            var colStr = fd.GetParamId(2);
            var method = fd.GetParamId(3);
            var minValStr = fd.GetParamId(4);
            var maxValStr = fd.GetParamId(5);
            bool hasDef = fd.GetParamNum() > 6;
            var defValStr = hasDef ? fd.GetParamId(6) : string.Empty;
            if (int.TryParse(rowStr, out var row) && int.TryParse(colStr, out var col)
                && float.TryParse(minValStr, out var minVal) && float.TryParse(maxValStr, out var maxVal) && float.TryParse(defValStr, out var defVal)) {
                AddSlider(id, row, col, method, minVal, maxVal, hasDef, defVal);
            }
        }
    }

    private void AddLabel(string id, int row, int col, string caption)
    {
        var labelObj = AddToCell(LabelTemplate, id, row, col);
        var label = labelObj.GetComponent<TMPro.TextMeshProUGUI>();
        label.text = caption;

        AddUiControl(id, label);
    }
    private void AddInput(string id, int row, int col, string dataType, bool hasDef, string defStr)
    {
        var inputObj = AddToCell(InputTemplate, id, row, col);
        var input = inputObj.GetComponent<TMPro.TMP_InputField>();
        input.inputType = TMPro.TMP_InputField.InputType.Standard;
        if (dataType == "int")
            input.contentType = TMPro.TMP_InputField.ContentType.IntegerNumber;
        else if (dataType == "float")
            input.contentType = TMPro.TMP_InputField.ContentType.DecimalNumber;
        else
            input.contentType = TMPro.TMP_InputField.ContentType.Standard;
        if (hasDef) {
            input.text = defStr;
        }

        AddUiControl(id, input);
    }
    private void AddButton(string id, int row, int col, string caption, string method)
    {
        var buttonObj = AddToCell(ButtonTemplate, id, row, col);
        var button = buttonObj.GetComponent<UnityEngine.UI.Button>();
        var labelObj = buttonObj.transform.Find("Text (TMP)");
        if (null != labelObj) {
            var label = labelObj.GetComponent<TMPro.TextMeshProUGUI>();
            label.text = caption;

            AddUiControl(id + "|Text", label);
        }
        var mobj = GetEventHandler(method);
        if (null != mobj) {
            button.onClick.AddListener(mobj);
        }

        AddUiControl(id, button);
    }
    private TMPro.TMP_Dropdown AddDropdown(string id, int row, int col, string method)
    {
        var dropdownObj = AddToCell(DropDownTemplate, id, row, col);
        var dropdown = dropdownObj.GetComponent<TMPro.TMP_Dropdown>();
        var mobj = GetEventHandler<int>(method);
        if (null != mobj) {
            dropdown.onValueChanged.AddListener(mobj);
        }

        AddUiControl(id, dropdown);
        return dropdown;
    }
    private void AddToggle(string id, int row, int col, string caption, string method, bool hasDef, bool isChecked)
    {
        var toggleObj = AddToCell(ToggleTemplate, id, row, col);
        var toggle = toggleObj.GetComponent<UnityEngine.UI.Toggle>();
        var labelObj = toggleObj.transform.Find("Label");
        if (null != labelObj) {
            var label = labelObj.GetComponent<Text>();
            label.text = caption;

            AddUiControl(id + "|Label", label);
        }
        if (hasDef) {
            toggle.isOn = isChecked;
        }
        toggle.group = null;
        var mobj = GetEventHandler<bool>(method);
        if (null != mobj) {
            toggle.onValueChanged.AddListener(mobj);
        }

        AddUiControl(id, toggle);
    }
    private UnityEngine.UI.ToggleGroup AddToggleGroup(string id, int row, int col)
    {
        var toggleObj = AddToCell(ToggleGroupTemplate, id, row, col);
        var toggle = toggleObj.GetComponent<UnityEngine.UI.ToggleGroup>();

        AddUiControl(id, toggle);
        return toggle;
    }
    private void AddToToggleGroup(UnityEngine.UI.ToggleGroup group, string id, int row, int col, string caption, string method, bool hasDef, bool isChecked)
    {
        var toggleObj = AddToCell(ToggleTemplate, id, row, col);
        var toggle = toggleObj.GetComponent<UnityEngine.UI.Toggle>();
        var labelObj = toggleObj.transform.Find("Label");
        if (null != labelObj) {
            var label = labelObj.GetComponent<Text>();
            label.text = caption;

            AddUiControl(id + "|Label", label);
        }
        if (hasDef) {
            toggle.isOn = isChecked;
        }
        toggle.group = group;
        var mobj = GetEventHandler<bool>(method);
        if (null != mobj) {
            toggle.onValueChanged.AddListener(mobj);
        }

        AddUiControl(id, toggle);
    }
    private void AddSlider(string id, int row, int col, string method, float minVal, float maxVal, bool hasDef, float defVal)
    {
        var sliderObj = AddToCell(SliderTemplate, id, row, col);
        var slider = sliderObj.GetComponent<UnityEngine.UI.Slider>();
        slider.minValue = minVal;
        slider.maxValue = maxVal;
        if (hasDef) {
            slider.value = defVal;
        }
        var mobj = GetEventHandler<float>(method);
        if (null != mobj) {
            slider.onValueChanged.AddListener(mobj);
        }

        AddUiControl(id, slider);
    }

    private GameObject AddToCell(GameObject template, string id, int row, int col)
    {
        var uiObj = Object.Instantiate<GameObject>(template, m_Cells[row, col]);
        uiObj.SetActive(true);
        uiObj.name = id;
        var rt = uiObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localScale = Vector3.one;
        return uiObj;
    }
    private void AddUiControl(string id, UIBehaviour ui)
    {
        m_UiControls[id] = ui;
    }
    private string ResolveId(string id)
    {
        if (id == c_AutoIdKeyword)
            return GenAutoId();
        else
            return id;
    }
    private string GenAutoId()
    {
        ++m_CurAutoId;
        return c_AutoIdKeyword + "_" + m_CurAutoId.ToString();
    }

    //UI configuration is a .txt file in the Resources directory
    //After the UI is loaded, initialization processing is performed, mainly initial value setting.
    private void OnUiInit(string res)
    {
        if (res == c_TestUI) {
            InitTestUi();
        }
    }
    //Values on the UI can be reset or synchronized while the UI is displayed
    private void OnUiShow(string res)
    {
        if (res == c_TestUI) {
            InitTestUi();
        }
    }

    //These two methods are used to associate the event processing method in the UI configuration
    //to the corresponding C# method of UiHandler (the actual method is later in this document)
    private UnityAction GetEventHandler(string method)
    {
        if (method == "OnButton") {
            return this.OnButton;
        }
        return null;
    }
    private UnityAction<T> GetEventHandler<T>(string method)
    {
        if (method == "OnValueChanged") {
            return (UnityAction<int>)this.OnValueChanged as UnityAction<T>;
        }
        else if (method == "OnCheckedChanged") {
            return (UnityAction<bool>)this.OnCheckedChanged as UnityAction<T>;
        }
        else if (method == "OnOneChanged") {
            return (UnityAction<bool>)this.OnOneChanged as UnityAction<T>;
        }
        else if (method == "OnTwoChanged") {
            return (UnityAction<bool>)this.OnTwoChanged as UnityAction<T>;
        }
        else if (method == "OnSliderChanged") {
            return (UnityAction<float>)this.OnSliderChanged as UnityAction<T>;
        }
        return null;
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
    private GameObject m_RootUi;
    private RectTransform[,] m_Cells;
    private Dictionary<string, UIBehaviour> m_UiControls = new Dictionary<string, UIBehaviour>();
    private int m_CurAutoId = 0;
    private string m_CurUiRes = string.Empty;
    private bool m_UiLoaded = false;
    private bool m_UiInited = false;

    private const string c_AutoIdKeyword = "@auto";
    private const int c_CellRowNum = 20;
    private const int c_CellColNum = 6;
}
