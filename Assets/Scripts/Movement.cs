using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
using Min_Max_Slider;
using UnityEngine.UI;
using TMPro;

public class Movement : MonoBehaviour
{

    public float speed; //affects animal movement speed
    public AnimalData[] targets;
    public string filePath;
    private int total = 1;
    private GameObject Map;
    private int count = 0; 
    private int final;  //this is used to indicate the final point in the array that is travelled to
    public GameObject timeSlider;
    public GameObject progressSlider;
    public GameObject minInput;
    public GameObject maxInput;
    public GameObject curTime;
    public Material pointMat;
    public Material colorChangeMat;
    private float HeightMin;
    private float HeightMax;
    public GameObject restObject;
    public GameObject speedLimit;
    private float speedMin;
    private float speedMax;

    // Start is called before the first frame update
    void Start()
    {
        Map = GameObject.Find("Map");
        speedMin = 100;
        speedMax = 0;

        targets = new AnimalData[1];

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (targets.Length < 2) return;
        float step = speed * Time.deltaTime;
        if (count < final) // checks to see if done
        {
            if (gameObject.transform.position == targets[count].position) //changes target to next after reaching coordinates and drops marker at coordinate
            {
                GameObject tar = GameObject.Find("Point" + count.ToString());
                tar.GetComponent<ColorChange>().enabled = true;        //adds script that handles the gradual color change from green to red
                progressSlider.GetComponent<Slider>().value = count;
                count++;
                speed = (targets[count].speed - speedMin) / (speedMax - speedMin) * 0.5f + 0.05f;
                if (count >= total) return;
            }
            transform.position = Vector3.MoveTowards(transform.position, targets[count].position, step); //gradually moves from point to point
        }

    }

    //loads the data set instead of inside of start
    public void LoadData(string path)
    {
        targets = new AnimalData[1];
        foreach (Transform child in Map.transform)
        {
            Destroy(child.gameObject);
        }

        GameObject pointProto = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pointProto.GetComponent<Renderer>().material = pointMat;
        pointProto.AddComponent<ColorChange>().enabled = false;
        pointProto.GetComponent<ColorChange>().pointMatE = colorChangeMat;
        pointProto.GetComponent<Renderer>().material = pointMat;
        pointProto.GetComponent<Renderer>().material.color = new Color(Color.gray.r, Color.gray.g, Color.gray.b, 0.25f);

        string[] csvFile = File.ReadAllLines(path); //reads csv into string array, each entry is one line
        string[] line;
        string[] headers = csvFile[0].Split(','); //gets the list of headers
        int north = -1;
        int east = -1;
        int timestamp = -1;
        int height = -1;
        int speed = -1;
        AnimalData[] tempList = new AnimalData[csvFile.Length];
        for (int i = 0; i < headers.Length; i++) //finds the desired headers
        {
            if (headers[i] == "northing") north = i;
            else if (headers[i] == "easting") east = i;
            else if (headers[i] == "timestamp") timestamp = i;
            else if (headers[i] == "height-above-ellipsoid") height = i;
            else if (headers[i] == "ground-speed") speed = i;
        }
        if (north < 0 || east < 0 || timestamp < 0 || height < 0) Debug.LogError("Header not found");
        tempList[0].position = Vector3.zero;        //done for sizing reasons
        tempList[0].time = System.DateTime.Now;

        Debug.Log(height);


        float heightMin = 0;
        float heightMax = 0;
        for (int i = 1; i < csvFile.Length; i++)
        {
            line = csvFile[i].Split(',');
            if (line[height] != "")
            {
                //Debug.Log(line[height]);
                if (heightMin == 0 && heightMax == 0)
                {
                    heightMin = float.Parse(line[height]);
                    heightMax = float.Parse(line[height]);
                }
                if (float.Parse(line[height]) > heightMax) heightMax = float.Parse(line[height]);
                if (float.Parse(line[height]) < heightMin) heightMin = float.Parse(line[height]);
            }
        }
        HeightMax = heightMax;
        HeightMin = heightMin;

        for (int i = 1; i < csvFile.Length; i++) //goes through the UTM coordinates
        {
            line = csvFile[i].Split(',');
            //Debug.Log(line[timestamp]);
            if (line[north] != "0.0")
            {
                Debug.Log(line[timestamp]);
                string time = line[timestamp].Split('.')[0];
                tempList[i].position = CreatePoint(float.Parse(line[north]), float.Parse(line[east]), float.Parse(line[height]));
                tempList[i].time = System.DateTime.ParseExact(time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture); //Parses 2019-06-23 23:16:01
                tempList[i].speed = float.Parse(line[speed]);
            }
            else
            {
                tempList[i].position = Vector3.zero;
                //tempList[i].time = System.DateTime.Now;
            }
            //Debug.Log(tempList[i].time);
        }

        List<AnimalData> tempA = new List<AnimalData>();
        for (int i = 0; i < tempList.Length; i++)       //removes all the zero vectors to make handling coloring simpler
        {
            if (tempList[i].position != Vector3.zero)
            {
                tempA.Add(tempList[i]);
                if (tempList[i].speed < speedMin) speedMin = tempList[i].speed;
                if (tempList[i].speed > speedMax) speedMax = tempList[i].speed;
            }
        }

        targets = tempA.ToArray();      //converts to array for future accessing
        //Debug.Log(targets[0].time.ToString());
        total = targets.Length;
        final = total;


        GameObject duplicate;       //used for instantiating the points
        for (int i = 0; i < total; i++)         //places all the points on the map
        {
            duplicate = Instantiate(pointProto, targets[i].position, Quaternion.identity, Map.transform);    //creates point that corresponds to GPS coord
            duplicate.name = "Point" + i.ToString();        //gives a unique identifier to each point for differentiation

            duplicate.GetComponent<ColorChange>().SetSpeed(targets[i].speed);
            duplicate.GetComponent<ColorChange>().speedLimit = speedLimit;
            duplicate.GetComponent<ColorChange>().restObject = restObject;
        }
        Destroy(pointProto);

        gameObject.transform.position = targets[0].position;

        //This sets the slides to the starting values that correspond to points in the array
        MinMaxSlider s = timeSlider.GetComponent<MinMaxSlider>();
        s.SetValues(0f, total - 1, 0f, total - 1);
        Slider p = progressSlider.GetComponent<Slider>();
        p.minValue = 0;
        p.maxValue = total - 1;
        minInput.GetComponent<TMP_InputField>().text = targets[0].time.ToString();
        maxInput.GetComponent<TMP_InputField>().text = targets[total - 1].time.ToString();
        curTime.GetComponent<TextMeshProUGUI>().text = targets[0].time.ToString();
    }

    private Vector3 CreatePoint(float northing, float easting, float height)       //maps UTM coordinate to world space
    {

        float xMin, xMax, zMin, zMax, xRange, zRange, hRange;       //used for calculating position
        Vector3 extents;

        extents = Map.GetComponent<MeshRenderer>().bounds.extents;

        xMin = Map.transform.position.x - extents.x;        //Gets the range of world positions that can be had
        xMax = Map.transform.position.x + extents.x;
        zMin = Map.transform.position.z - extents.z;
        zMax = Map.transform.position.z + extents.z;
        xRange = xMax - xMin;
        zRange = zMax - zMin;
        hRange = HeightMax - HeightMin;

        float sUTMx = 624079.8465020715f;        //coordinates of the image
        float eUTMx = 629752.8465020715f;
        float sUTMz = 1015157.5668793379f;
        float eUTMz = 1009715.5668793379f;

        float UTMxD = eUTMx - sUTMx;        //used to find the percentage through the image that the point is
        float UTMzD = eUTMz - sUTMz;

        Vector3 tPoint = new Vector3(0, 0, 0);

        float xP = (easting - sUTMx) / UTMxD;
        float zP = (northing - sUTMz) / UTMzD;
        float hP = 1 / UTMxD * height;

        tPoint.x = xMin + xP * xRange;
        tPoint.y = hP;// * 0.5f;
        tPoint.z = zMin + zP * zRange;

        if (tPoint.x > 5 || tPoint.x < -5 || tPoint.z > 5 || tPoint.z < -5) return Vector3.zero;



        return tPoint;

    }

    //this updates the vis based on the time slider
    public void UpdateVisTime(float start, float end)
    {
        GameObject tar;

        //disables all the points not found in the range
        for(int i = 0; i < total; i++)
        {
            tar = GameObject.Find("Point" + i.ToString());
            tar.GetComponent<ColorChange>().enabled = false;
            //tar.GetComponent<Renderer>().material.color = Color.gray;
            var tempColor = Color.gray;
            tempColor.a = 0.01f;
            tar.GetComponent<Renderer>().material.color = tempColor;
            if (i < start || i > end) tar.transform.localScale = Vector3.zero;
            if (i >= start && i <= end)
            {
                tar.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
                tar.GetComponent<Renderer>().material = pointMat;
            }
        }

        //This is to set the starting parameters for the new start and end
        transform.position = targets[Mathf.RoundToInt(start)].position;
        count = Mathf.RoundToInt(start);
        final = Mathf.RoundToInt(end);

        minInput.GetComponent<TMP_InputField>().text = targets[count].time.ToString();
        maxInput.GetComponent<TMP_InputField>().text = targets[final].time.ToString();

    }

    //this updates the vis based on the text input
    public void UpdateVisInput(string startS, string endS)
    {
        int start = -1;
        int end = -2;
        for(int i = 0;  i < total; i++)
        {
            if (targets[i].time.ToString() == startS)
            {
                start = i;
            }
            if(targets[i].time.ToString() == endS)
            {
                end = i;
                break;
            }
            if(i > 0)
            {
                if (System.DateTime.Compare(targets[i].time, System.DateTime.ParseExact(startS, "M/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)) > 0 && System.DateTime.Compare(targets[i-1].time, System.DateTime.ParseExact(startS, "M/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)) < 0)
                {
                    start = i - 1;
                    break;
                }
                if (System.DateTime.Compare(targets[i].time, System.DateTime.ParseExact(endS, "M/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)) > 0 && System.DateTime.Compare(targets[i - 1].time, System.DateTime.ParseExact(endS, "M/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)) < 0)
                {
                    end = i - 1;
                    break;
                }
            }

        }
        //this ensures that end takes place after the start;
        if (end < start)
        {
            end = total - 1;
        }
        if (start < 0) start = 0;
        if (end < 0) end = total - 1;
        

        //progress slider stuff
        Slider pr = progressSlider.GetComponent<Slider>();
        pr.minValue = start;
        pr.maxValue = end;
        pr.value = start;

        MinMaxSlider tb = timeSlider.GetComponent<MinMaxSlider>();
        tb.SetValues(start, end);
        UpdateVisTime(start, end);
    }

    public void ProgressUpdate(float p)
    {
        curTime.GetComponent<TextMeshProUGUI>().text = targets[Mathf.RoundToInt(p)].time.ToString();
    }

    public void HeightAdjust(bool ex)
    {
        GameObject tar;
        if (ex)
        {
            for (int i = 0; i < total; i++)
            {
                tar = GameObject.Find("Point" + i.ToString());
                tar.transform.position = new Vector3(tar.transform.position.x, tar.transform.position.y * 50, tar.transform.position.z);
                targets[i].position.y = targets[i].position.y * 50;
            }
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y * 50, gameObject.transform.position.z);
        }
        else
        {
            for (int i = 0; i < total; i++)
            {
                tar = GameObject.Find("Point" + i.ToString());
                tar.transform.position = new Vector3(tar.transform.position.x, tar.transform.position.y / 50, tar.transform.position.z);
                targets[i].position.y = targets[i].position.y / 50;
            }
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y / 50, gameObject.transform.position.z);
        }
    }

    public struct AnimalData
    {
        public Vector3 position;
        public System.DateTime time;
        public float speed;
    }


}
