using System.Collections.Generic;
using UnityEngine;
using StoryScript;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 这个文件里处理所有GM相关的UI逻辑，考虑到一个工程可能用于多个功能的实验，UI采用可配置的方式，但逻辑都是写在这里的
/// 我们提供了一个loadui("ui配置资源")命令来加载资源，但一般还是直接在Start里加载当前实验想用的资源即可，不想重新出包时可以使用命令切换ui
/// ui配置使用MetaDSL语法，但这里不考虑使用脚本解释器，因为实验的功能代码都是在工程里写死的，使用脚本意义不大，并且可能把事情搞复杂了
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

        //这里加载当前实验的UI，只能有一个是当前的
        LoadUi("TestUI");
    }
    public void ShowUi()
    {
        m_RootUi.SetActive(true);
    }
    public void HideUi()
    {
        m_RootUi.SetActive(false);
    }
    public void LoadUi(string res)
    {
        ClearCells();

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
                StoryUtility.DestroyObject(gobj);
            }
        }

        m_UiControls.Clear();
    }

    private void BuildLabel(Dsl.ISyntaxComponent info)
    {
        var fd = info as Dsl.FunctionData;
        if (null != fd) {
            var id = fd.GetParamId(0);
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
            var rowStr = fd.GetParamId(1);
            var colStr = fd.GetParamId(2);
            var dataType = fd.GetParamId(3);
            var defStr = fd.GetParamId(4);
            if (int.TryParse(rowStr, out var row) && int.TryParse(colStr, out var col)) {
                AddInput(id, row, col, dataType, defStr);
            }
        }
    }
    private void BuildButton(Dsl.ISyntaxComponent info)
    {
        var fd = info as Dsl.FunctionData;
        if (null != fd) {
            var id = fd.GetParamId(0);
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
            var rowStr = cd.GetParamId(1);
            var colStr = cd.GetParamId(2);
            var method = cd.GetParamId(3);
            var selStr = cd.GetParamId(4);
            if (int.TryParse(rowStr, out var row) && int.TryParse(colStr, out var col) && int.TryParse(selStr, out var select)) {
                var dropdown = AddDropdown(id, row, col, method);
                var list = new List<string>();
                foreach (var p in fd.Params) {
                    list.Add(p.GetId());
                }
                dropdown.AddOptions(list);
                dropdown.value = select;
            }
        }
    }
    private void BuildToggle(Dsl.ISyntaxComponent info)
    {
        var fd = info as Dsl.FunctionData;
        if (null != fd) {
            var id = fd.GetParamId(0);
            var rowStr = fd.GetParamId(1);
            var colStr = fd.GetParamId(2);
            var caption = fd.GetParamId(3);
            var method = fd.GetParamId(4);
            var checkStr = fd.GetParamId(5);
            if (int.TryParse(rowStr, out var row) && int.TryParse(colStr, out var col)) {
                AddToggle(id, row, col, caption, method, checkStr == "true" || checkStr == "True");
            }
        }
    }
    private void BuildToggleGroup(Dsl.ISyntaxComponent info)
    {
        var fd = info as Dsl.FunctionData;
        if (null != fd && fd.IsHighOrder && fd.HaveStatement()) {
            var cd = fd.LowerOrderFunction;
            var id = cd.GetParamId(0);
            var rowStr = cd.GetParamId(1);
            var colStr = cd.GetParamId(2);
            if (int.TryParse(rowStr, out var row) && int.TryParse(colStr, out var col)) {
                var group = AddToggleGroup(id, row, col);
                foreach (var p in fd.Params) {
                    var pfd = p as Dsl.FunctionData;
                    if (null != pfd && pfd.GetId() == "toggle") {
                        var pid = pfd.GetParamId(0);
                        var pcap = pfd.GetParamId(1);
                        var method = pfd.GetParamId(2);
                        var pchecked = pfd.GetParamId(3);
                        AddToToggleGroup(group, pid, row, col++, pcap, method, pchecked == "true" || pchecked == "True");
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
            var rowStr = fd.GetParamId(1);
            var colStr = fd.GetParamId(2);
            var method = fd.GetParamId(3);
            var minValStr = fd.GetParamId(4);
            var maxValStr = fd.GetParamId(5);
            var defValStr = fd.GetParamId(6);
            if (int.TryParse(rowStr, out var row) && int.TryParse(colStr, out var col)
                && float.TryParse(minValStr, out var minVal) && float.TryParse(maxValStr, out var maxVal) && float.TryParse(defValStr, out var defVal)) {
                AddSlider(id, row, col, method, minVal, maxVal, defVal);
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
    private void AddInput(string id, int row, int col, string dataType, string defStr)
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
        input.text = defStr;

        AddUiControl(id, input);
    }
    private void AddButton(string id, int row, int col, string caption, string method)
    {
        var buttonObj = AddToCell(ButtonTemplate, id, row, col);
        var button = buttonObj.GetComponent<UnityEngine.UI.Button>();
        var labelObj = buttonObj.transform.Find("Text");
        if (null != labelObj) {
            var label = labelObj.GetComponent<Text>();
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
    private void AddToggle(string id, int row, int col, string caption, string method, bool isChecked)
    {
        var toggleObj = AddToCell(ToggleTemplate, id, row, col);
        var toggle = toggleObj.GetComponent<UnityEngine.UI.Toggle>();
        var labelObj = toggleObj.transform.Find("Label");
        if (null != labelObj) {
            var label = labelObj.GetComponent<Text>();
            label.text = caption;

            AddUiControl(id + "|Label", label);
        }
        toggle.isOn = isChecked;
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
    private void AddToToggleGroup(UnityEngine.UI.ToggleGroup group, string id, int row, int col, string caption, string method, bool isChecked)
    {
        var toggleObj = AddToCell(ToggleTemplate, id, row, col);
        var toggle = toggleObj.GetComponent<UnityEngine.UI.Toggle>();
        var labelObj = toggleObj.transform.Find("Label");
        if (null != labelObj) {
            var label = labelObj.GetComponent<Text>();
            label.text = caption;

            AddUiControl(id + "|Label", label);
        }
        toggle.isOn = isChecked;
        toggle.group = group;
        var mobj = GetEventHandler<bool>(method);
        if (null != mobj) {
            toggle.onValueChanged.AddListener(mobj);
        }

        AddUiControl(id, toggle);
    }
    private void AddSlider(string id, int row, int col, string method, float minVal, float maxVal, float defVal)
    {
        var sliderObj = AddToCell(SliderTemplate, id, row, col);
        var slider = sliderObj.GetComponent<UnityEngine.UI.Slider>();
        slider.minValue = minVal;
        slider.maxValue = maxVal;
        slider.value = defVal;
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

    //UI配置是Resources目录下的.txt文件
    //这2个方法用于关联UI配置里的事件处理方法到UiHandler的对应的C#方法（实际方法在本文件稍后）
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

    //与TestUI配置对应的事件处理
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

    private GameObject m_RootUi;
    private RectTransform[,] m_Cells;
    private Dictionary<string, UIBehaviour> m_UiControls = new Dictionary<string, UIBehaviour>();

    private const int c_CellRowNum = 20;
    private const int c_CellColNum = 6;
}
