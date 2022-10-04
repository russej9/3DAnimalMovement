using Min_Max_Slider;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;

public class ListenerHandler : MonoBehaviour
{

    public GameObject timeBoundSlider;
    public GameObject minInput;
    public GameObject maxInput;
    public GameObject progressSlider;
    public GameObject curTimeText;
    public GameObject animal;
    public GameObject heightToggleObject;
    public GameObject loadButtonObject;

    private MinMaxSlider tb;
    private Slider pr;
    private TMP_InputField minText;
    private TMP_InputField maxText;
    private bool pause;
    private Toggle heightToggle;
    private Button loadButton;

    private MinMaxSlider.MinMaxValues values;

    // Start is called before the first frame update
    void Start()
    {
        
        //this section collects all of the parts of the gameobjects needed
        tb = timeBoundSlider.GetComponent<MinMaxSlider>();
        pr = progressSlider.GetComponent<Slider>();
        minText = minInput.GetComponent<TMP_InputField>();
        maxText = maxInput.GetComponent<TMP_InputField>();
        heightToggle = heightToggleObject.GetComponent<Toggle>();
        loadButton = loadButtonObject.GetComponent<Button>();

        //this adds all the listeners
        //tb.onValueChanged.AddListener(delegate { UpdateMapTimeBounds(); });
        minText.onEndEdit.AddListener(delegate { UpdateMapInput(); });
        maxText.onEndEdit.AddListener(delegate { UpdateMapInput(); });
        pr.onValueChanged.AddListener(delegate { UpdateProgress(pr.value); });
        heightToggle.onValueChanged.AddListener(delegate { HeightAdjust(); });
        loadButton.onClick.AddListener(delegate { LoadData(); });

        pause = false;

        FileBrowser.SetFilters(true, new FileBrowser.Filter("Text Files", ".txt", ".csv"));
        FileBrowser.SetDefaultFilter(".csv");

    }

    private void Update()
    {
        if (!Input.GetMouseButton(0) && (values.minValue != tb.Values.minValue || values.maxValue != tb.Values.maxValue)) //this is a listener for the double ended slider, this is needed because too many points will make it impossible to use with typical listener
        {
            values = tb.Values;
            UpdateMapTimeBounds();
        }

        if (Input.GetKeyDown(KeyCode.Space)) //handles pausing game on space key press
        {
            if (pause)
            {
                pause = false;
                Time.timeScale = 1f;
            }
            else
            {
                pause = true;
                Time.timeScale = 0f;
            }
        }

    }

    void UpdateMapTimeBounds()
    {
        pr.minValue = tb.Values.minValue;
        pr.maxValue = tb.Values.maxValue;
        pr.value = tb.Values.minValue;
        animal.GetComponent<Movement>().UpdateVisTime(pr.minValue, pr.maxValue);
    }

    void UpdateMapInput()
    {
        animal.GetComponent<Movement>().UpdateVisInput(minText.text, maxText.text);
    }

    void UpdateProgress(float p)
    {
        animal.GetComponent<Movement>().ProgressUpdate(p);
    }

    void HeightAdjust()
    {
        animal.GetComponent<Movement>().HeightAdjust(heightToggle.isOn);
    }

    void LoadData()
    {
        StartCoroutine(ShowLoadData());
    }

    IEnumerator ShowLoadData()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Load Data Set", "Select");

        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            animal.GetComponent<Movement>().LoadData(FileBrowser.Result[0]);
        }

    }

}
