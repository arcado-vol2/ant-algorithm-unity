
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class AntAlgorithm : MonoBehaviour
{
    private float alpha, beta, Q, ro, startPheromone, minPheromone;
    private int antCount = 1;
    private int eliteAntCount = 0;
    private int iteration = 0;
    private float pathSize = 0.0f;

    private float[,] adjMatrix;
    private float[,] pheromoneMatrix;

    [SerializeField]
    private AlgoPannelController algoPanelController;
    [SerializeField]
    private GraphController graphController;

    private bool run = false;

    //struct for ant, contains current pos and visited points
    public struct Ant
    {
        public int currentPoint { set; get; }
        public bool[] visitedPoints { set; get; }
        public int visitedPointsCounter;
        public List<int> path;
        public float pathLength;
        public bool isElite;
        public Ant(int point, int adjMatrixLength, bool Elite)
        {
            this.currentPoint = point;
            this.visitedPoints = new bool[adjMatrixLength];
            this.visitedPointsCounter = 0;
            this.path = new List<int>();
            this.pathLength = 0.0f;
            this.isElite = Elite;
        }

       
    }


    //func to update labels on a screen
    private void UpdateUI()
    {

        algoPanelController.UpdateLabel(algoPanelController.pathSizeLabel, (int)pathSize, algoPanelController.startPathSizeText);
        algoPanelController.UpdateLabel(algoPanelController.iterationCountLabel, iteration, algoPanelController.startIterationCountText);

    }

    public void OnStartButtonPressed()
    {
        if (!run)
        {
            //if user enter not a float also here we get constants from user
            if (!TryGetValueFromInput(algoPanelController.alphaInput, out alpha) || !TryGetValueFromInput(algoPanelController.betaInput, out beta) ||
        !TryGetValueFromInput(algoPanelController.qInput, out Q) || !TryGetValueFromInput(algoPanelController.roInput, out ro)
        || !TryGetValueFromInput(algoPanelController.minPheromoneInput, out minPheromone) || !TryGetValueFromInput(algoPanelController.startPheromoneInput, out startPheromone) )
            {
                algoPanelController.startAlgoButtonToggle.OnPressed();
                return;
            }
            antCount = (int)algoPanelController.antCountSlider.value;
            eliteAntCount = (int)algoPanelController.eliteAntCountSlider.value;
            //start algorithm
            iteration = 0;
            

            adjMatrix = graphController.GetAdjMatrix();
            if (adjMatrix.Length == 0) return;
            pheromoneMatrix = new float[adjMatrix.GetLength(0), adjMatrix.GetLength(1)];
            for (int x = 0; x < pheromoneMatrix.GetLength(0); x++)
            {
                for (int y = 0; y < pheromoneMatrix.GetLength(1); y++)
                {
                    if (x == y) pheromoneMatrix[x, y] = 0;
                    else pheromoneMatrix[x, y] = startPheromone;
                }
            }

          



            StartCoroutine(Main());

            //update UI
            algoPanelController.antCountSlider.interactable = false;
            algoPanelController.alphaInput.interactable = false;
            algoPanelController.betaInput.interactable = false;
            algoPanelController.qInput.interactable = false;
            algoPanelController.roInput.interactable = false;
            algoPanelController.startPheromoneInput.interactable = false;
            algoPanelController.minPheromoneInput.interactable = false;
            algoPanelController.createGraphButton.interactable = false;
            algoPanelController.deleteGraphButton.interactable = false;
            algoPanelController.eliteAntCountSlider.interactable = false;
            run = true;
        }
        else
        {
            //stop run if button pressed
            StopAllCoroutines();
            
            //update UI
            algoPanelController.antCountSlider.interactable = true;
            algoPanelController.alphaInput.interactable = true;
            algoPanelController.betaInput.interactable = true;
            algoPanelController.qInput.interactable = true;
            algoPanelController.roInput.interactable = true;
            algoPanelController.createGraphButton.interactable = true;
            algoPanelController.deleteGraphButton.interactable = true;
            algoPanelController.minPheromoneInput.interactable = true;
            algoPanelController.startPheromoneInput.interactable = true; 
            algoPanelController.eliteAntCountSlider.interactable = true;
            run = false;
        }
        

    }

    //basic UI handler. Try to convert string to float and if it's not possible show user error message
    private bool TryGetValueFromInput(TMP_InputField inputField, out float number)
    {
        if (float.TryParse( inputField.text, out number)) {
            return true;
            
        }
        inputField.text = "¬ведите число!";
        number = 0.0f;
        return false;

    }

    private IEnumerator Main()
    {
        int vertexCount = adjMatrix.GetLength(0);
        float minLength = float.MaxValue;
        List<int> bestPath = new List<int>();
        int startVertex = 0;
        while (true)
        {
            float[,] pheromoneAdd = new float[pheromoneMatrix.GetLength(0), pheromoneMatrix.GetLength(1)];
            for (int antNum = 0; antNum < antCount; antNum++)
            {
                Ant ant = new Ant(startVertex % vertexCount, vertexCount, antNum >= (antCount - eliteAntCount));
                startVertex++;
                for (; ant.visitedPointsCounter < vertexCount; ant.visitedPointsCounter++)
                {
                    if (!ant.isElite)
                    {
                        float sum = 0.0f;
                        List<int> possible = new List<int>();
                        for (int i = 0; i < vertexCount; i++)
                        {
                            if (ant.visitedPoints[i] || i == ant.currentPoint) continue;
                            sum += Mathf.Pow(pheromoneMatrix[ant.currentPoint, i], alpha) * Mathf.Pow(1.0f / adjMatrix[ant.currentPoint, i], beta);
                            possible.Add(i);
                        }
                        if (possible.Count == 0)
                        {
                            ant.path.Add(ant.currentPoint);
                            break;
                        }
                        float randomPoint = Random.Range(0.0f, 1.0f);
                        float marker = 0.0f;
                        int nextPoint = possible[0];
                        foreach (int point in possible)
                        {
                            if (marker >= randomPoint)
                            {
                                nextPoint = point;
                                break;
                            }
                            marker += Mathf.Pow(pheromoneMatrix[ant.currentPoint, point], alpha) * Mathf.Pow(1.0f / adjMatrix[ant.currentPoint, point], beta) / sum;
                        }
                        ant.pathLength += adjMatrix[ant.currentPoint, nextPoint];
                        ant.path.Add(ant.currentPoint);
                        ant.visitedPoints[ant.currentPoint] = true;
                        ant.currentPoint = nextPoint;
                    }
                    else
                    {
                        ant.path = bestPath;
                        ant.pathLength = minLength;
                    }
                    
                }
                float deltaTau = Q / ant.pathLength;
                for (int i = 0; i < ant.path.Count; i++)
                {
                    pheromoneAdd[ant.path[i], ant.path[(i + 1) % ant.path.Count]] += deltaTau;
                    pheromoneAdd[ant.path[(i + 1) % ant.path.Count], ant.path[i]] += deltaTau;
                }
                if (ant.pathLength < minLength)
                {
                    minLength = ant.pathLength;
                    bestPath = ant.path;
                }
            }

            for (int x = 0; x < pheromoneMatrix.GetLength(0); x++)
            {
                for (int y = 0; y < pheromoneMatrix.GetLength (1); y++)
                {
                    pheromoneMatrix[x, y] = Mathf.Max(pheromoneMatrix[x, y] * ro + pheromoneAdd[x, y], minPheromone);
                }
            }

            //update UI
            iteration++;
            UpdateUI();
            pathSize = minLength;
            graphController.UpdateEdges(pheromoneMatrix, bestPath);


           

            yield return null;
        } 
        
    }
    

}
