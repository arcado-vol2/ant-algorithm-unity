using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static AntAlgorithm;

public class GraphController : MonoBehaviour
{

    public int vertexesCount { get; private set; } = 0;
    [SerializeField]
    private GameObject vertexPrefab;
    [SerializeField]
    private GameObject edgePrefab;
    private RectTransform vertexes;
    private RectTransform edges;

    private float maxEdgeThickness = 1.0f;
    private float edgeThicknessMult = 1.0f;
    private float bestPathThickness = 1.0f;

    private bool edit = false;
    private bool showOnlyBest = true;
    void Start()
    {
        this.enabled = edit;
        vertexes = transform.Find("Vertexes").GetComponent<RectTransform>();
        edges = transform.Find("Edges").GetComponent<RectTransform>();
    }

    public float[,] GetAdjMatrix()
    {
        int size = vertexes.childCount;
        float[,] adjMatrix = new float[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (adjMatrix[i, j] != 0) continue;
                adjMatrix[i, j] = Vector2.Distance(vertexes.GetChild(i).GetComponent<Transform>().position, vertexes.GetChild(j).GetComponent<Transform>().position) ;
                adjMatrix[j, i] = adjMatrix[i, j];
            }
        }
        return adjMatrix;
    }

    private void AddEdge(Vector2 point1, Vector2 point2, int point1Index, int point2Index)
    {
        
        GameObject edge = Instantiate(edgePrefab, edges);
        UILineRenderer line = edge.GetComponent<UILineRenderer>();
        line.points = new Vector2[]{ point1, point2};
        line.index_i = point1Index;
        line.index_j = point2Index;

    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 mPos = Input.mousePosition;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(vertexes, mPos, null, out localPoint);

            GameObject newVertex = Instantiate(vertexPrefab, vertexes);
            newVertex.GetComponent<VertexControll>().index = ++vertexesCount;
            newVertex.GetComponent<RectTransform>().anchoredPosition = localPoint;

        }
        
    }

    public void OnCreateGraphButtonPressed()
    {
        if (edit)
        {
            int childCount = vertexes.childCount;

            for (int i = 0; i < childCount; i++)
            {
                Vector2 point1 = vertexes.GetChild(i).GetComponent<RectTransform>().anchoredPosition;

                for (int j = i + 1; j < childCount; j++)
                {
                    Vector2 point2 = vertexes.GetChild(j).GetComponent<RectTransform>().anchoredPosition;
                    AddEdge(point1, point2, i, j);
                }
            }
        }
        else
        {
            foreach (Transform child in edges)
            {
                Destroy(child.gameObject);
            }
        }

        edit = !edit;
        this.enabled = edit;
    }

    public void OnDeleteGraphButtonPressed()
    {
        foreach (Transform child in edges)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in vertexes)
        {
            Destroy(child.gameObject);
        }
        vertexesCount = 0; 
    }


    public void OnShowOnlyBestToggle()
    {
        showOnlyBest = !showOnlyBest;
    }

    public void UpdateEdges(float[,] thicknessMatr, List<int> path)
    {
        float maxThickness = float.MinValue;
        bool[,] pathMatr = new bool[thicknessMatr.GetLength(0), thicknessMatr.GetLength(1)];
        for (int i = 0; i < path.Count; i++)
        {
            int currentIndex = path[i];
            int nextIndex = path[(i + 1) % path.Count];
            pathMatr[currentIndex, nextIndex] = true;
            pathMatr[nextIndex, currentIndex] = true;
            maxThickness = Mathf.Max(maxThickness, thicknessMatr[currentIndex, nextIndex]);
        }
        foreach (Transform child in edges)
        {
            UILineRenderer renderer = child.GetComponent<UILineRenderer>();
            if (pathMatr[renderer.index_i, renderer.index_j])
            {
                renderer.color = new Color(0.0f, 1.0f, 0.0f, 0.5f);
                renderer.thickness = bestPathThickness;
            }
            else
            {
                if (showOnlyBest)
                {
                    renderer.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                    
                }
                else
                {
                    renderer.color = new Color(1.0f, 1.0f, 1.0f, 0.58f);
                    renderer.thickness = Mathf.Min(maxEdgeThickness, thicknessMatr[renderer.index_i, renderer.index_j] * edgeThicknessMult); ;
                }
            }
            renderer.SetVerticesDirty();
        }
    }


    public void OnMaxThicknessSliderValueChanged(Slider slider)
    {
        maxEdgeThickness = slider.value;
    }
    public void OnThicknessMultVlaueChanged(Slider slider)
    {
        edgeThicknessMult = slider.value;
    }

    public void OnBestPathThicknessValueChanged(Slider slider)
    {
        bestPathThickness = slider.value;
    }

}
