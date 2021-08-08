using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class AnalyticGraph
{
    public Transform graphHolder;

    public Canvas graphCanvas;

    public Text valueName;
    public Text valueMaxText;
    public Text valueMinText;
    public Text timeStartText;
    public Text timeCurrentText;
    public Text gameObjectText;

    [HideInInspector] public LineRenderer line;
}

public class GraphManager : MonoBehaviour
{
    // Start is called before the first frame update

    public static GraphManager instance = null;
    
    [SerializeField] private bool enableAnalytics;
    [SerializeField] private GameObject analyticLinePrefab;
    [SerializeField][Space][Space] public AnalyticGraph valueGraph = new AnalyticGraph();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        if (enableAnalytics == false)
        {
            valueGraph.graphCanvas.enabled = false;
        }
    }

    void SpawnLine(AnalyticGraph graph)
    {
        GameObject analyticsLineClone = GameObject.Instantiate(analyticLinePrefab, graph.graphHolder.position, graph.graphHolder.rotation, graph.graphHolder);
        graph.line = analyticsLineClone.GetComponent<LineRenderer>();
    }
    
    public void ChangeValueGraphName(string name)
    {
        valueGraph.valueName.text = name;
    }

    public void ChangeGameObjectText(string name)
    {
        valueGraph.gameObjectText.text = name;
    }
    
    public void DrawCurve(AnalyticPoint analyticPoint)
    {
        if (enableAnalytics == false)
            return;
        if (valueGraph.line == null)
            SpawnLine(valueGraph);

        valueGraph.line.positionCount = analyticPoint.analyticsValues.Count;

        for(int i = 0; i < analyticPoint.analyticsValues.Count; i++)
        {
            Vector3 newPosition = Vector3.zero;
            if ((analyticPoint.maxValue - analyticPoint.minValue) != 0)
                newPosition.y =(float)((analyticPoint.analyticsValues.Values.ElementAt(i) - analyticPoint.minValue) / (analyticPoint.maxValue - analyticPoint.minValue));

            if (analyticPoint.analyticsValues.Keys.ElementAt(analyticPoint.analyticsValues.Count-1) != 0)
                newPosition.x = (float)(analyticPoint.analyticsValues.Keys.ElementAt(i) / analyticPoint.analyticsValues.Keys.ElementAt(analyticPoint.analyticsValues.Count - 1));

            valueGraph.line.SetPosition(i, newPosition);
        }

        valueGraph.valueMaxText.text = analyticPoint.maxValue.ToString("F1");
        valueGraph.valueMinText.text = analyticPoint.minValue.ToString("F1");

        valueGraph.timeStartText.text = "0";
        valueGraph.timeCurrentText.text = analyticPoint.analyticsValues.Keys.ElementAt(analyticPoint.analyticsValues.Count - 1).ToString("F1");
    }
    
    public void Clear()
    {
        if (valueGraph.line == null)
            return;
        valueGraph.line.positionCount = 0;
        valueGraph.gameObjectText.text = "";
        valueGraph.valueName.text = "";
    }
}
