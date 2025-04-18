using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using static NetworkManager;
using UnityEditor.Experimental.GraphView;
using TMPro;

public class UIGraphManager : MonoBehaviour
{
    [Header("References")]
    public RectTransform graphRoot;
    public RectTransform Content;
    public GameObject nodePrefab;
    public GameObject linePrefab;
    public SceneGraphManager sceneGraphManager;
    public FlowDirection flowDirection;

    private Dictionary<string, GameObject> spawnedNodes = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> connectingLines = new Dictionary<string, GameObject>();
    private Dictionary<string, FlowDirection> nodeDirections = new Dictionary<string, FlowDirection>();
    private List<string> disabledNodeIds = new List<string>();

    private string lastNodeId = null;
    private int depth = 0;
    private float distance = 300f;

    private void Start()
    {
        StartCoroutine(SpawnAfterDelay());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            PlayerPrefs.DeleteKey("Graph_LastNode");
            PlayerPrefs.DeleteKey("Graph_VisitedNodes");
            PlayerPrefs.DeleteKey("SelectedContentID");
            PlayerPrefs.DeleteKey("SelectedNodeID");
            PlayerPrefs.Save();
            foreach (var key in PlayerPrefs.GetString("Graph_VisitedNodes", "").Split(','))
            {
                PlayerPrefs.DeleteKey($"NodePos_{key}");
                PlayerPrefs.DeleteKey($"nodePos_{key}");
            }
            Debug.Log("Graph save data cleared.");
        }
    }

    private IEnumerator SpawnAfterDelay()
    {
        yield return new WaitForSeconds(0.1f); // small delay to wait for SceneGraphManager

        bool done = false;
        bool dbHadNodes = false;

        //LoadGraphState();
        LoadGraphStateDatabase(any =>
        {
            Debug.Log(any);
            dbHadNodes = any;
            done = true;
        });
        
        yield return new WaitUntil(() => done == true);
        Debug.Log($"gone through");
        Debug.Log(dbHadNodes);
        if (!dbHadNodes)
        {
            SpawnFirstNode("Start");
        }

        if (!string.IsNullOrEmpty(lastNodeId) && spawnedNodes.ContainsKey(lastNodeId))
        {
            SceneNode lastNode = sceneGraphManager.sceneNodes[lastNodeId];

            if (lastNode.isAutoProgress)
            {
                Debug.Log("Auto-clicking last node after load: " + lastNodeId);
                OnNodeClicked(lastNodeId);
            }
        }
    }

    public void SpawnFirstNode(string nodeId)
    {
        if (!sceneGraphManager.sceneNodes.ContainsKey(nodeId)) return;

        var node = sceneGraphManager.sceneNodes[nodeId];
        Vector2 canvasSize = graphRoot.rect.size;
        Vector2 bottom = -canvasSize / 2f + new Vector2(100, 100); // Fixed starting position
        bottom.x = 0f; // Centered
        Vector2 position = bottom;
        GameObject nodeGO = Instantiate(nodePrefab, Content);
        RectTransform rt = nodeGO.GetComponent<RectTransform>();
        rt.anchoredPosition = position;

        UINode uiNode = nodeGO.GetComponent<UINode>();
        uiNode.Initialize(nodeId, node.title, node.description, this);

        spawnedNodes[nodeId] = nodeGO;
        nodeDirections[nodeId] = FlowDirection.Up;
    }

    public void SpawnSceneNodeFrom(string fromNodeId, string targetNodeId, FlowDirection direction)
    {
        if (!sceneGraphManager.sceneNodes.ContainsKey(targetNodeId)) return;
        //if (!spawnedNodes.ContainsKey(fromNodeId)) return;
        var node = sceneGraphManager.sceneNodes[targetNodeId];

        // Position of the clicked (parent) node
        Vector2 basePosition = spawnedNodes[fromNodeId].GetComponent<RectTransform>().anchoredPosition;

        // angle and distance
        float angleDegrees = 22.5f;
        float angleRadians = Mathf.Deg2Rad * angleDegrees;

        // Offset components (same for both sides initially)
        float offsetX = Mathf.Cos(angleRadians) * distance;
        float offsetY = Mathf.Sin(angleRadians) * distance;

        // calculate offset based on direction
        if (direction == FlowDirection.Left) offsetX = -Mathf.Cos(angleRadians) * distance;
        else if (direction == FlowDirection.Right) offsetX = Mathf.Cos(angleRadians) * distance;
        else if (direction == FlowDirection.Up) { offsetY = distance; offsetX = 0f; }
        // Calculate final position
        Vector2 newNodePosition = basePosition + new Vector2(offsetX, offsetY);

        Vector2 canvasSize = graphRoot.rect.size;
        Vector2 thirdSize = canvasSize / 3f;
        Vector2 canvasMin = -thirdSize;
        Vector2 canvasMax = thirdSize;

        Vector2 shift = Vector2.zero;

        /*if (newNodePosition.x < canvasMin.x) shift.x = canvasMin.x - newNodePosition.x;
        else if (newNodePosition.x > canvasMax.x) shift.x = canvasMax.x - newNodePosition.x;

        if (newNodePosition.y < canvasMin.y) shift.y = canvasMin.y - newNodePosition.y;
        else if (newNodePosition.y > canvasMax.y) shift.y = canvasMax.y - newNodePosition.y;
        // shift entire graph if the new node is outside the bounds
        if (shift != Vector2.zero)
        {
            foreach (var spawnNode in spawnedNodes.Values)
            {
                RectTransform rect = spawnNode.GetComponent<RectTransform>();
                rect.anchoredPosition += shift;
            }

            foreach (var connectLine in connectingLines.Values)
            {
                RectTransform rect = connectLine.GetComponent<RectTransform>();
                rect.anchoredPosition += shift;
            }

            List<string> keys = new List<string>(spawnedNodes.Keys);
            //foreach (var key in keys)
            //{
             //   RectTransform rect = spawnedNodes[key].GetComponent<RectTransform>();
             //   spawnedNodes[key].GetComponent<RectTransform>().anchoredPosition += rect.anchoredPosition;
            //}

            basePosition += shift;
            newNodePosition += shift;
        }*/
        Debug.Log(Content);
        // Create node
        GameObject nodeGO = Instantiate(nodePrefab, Content);
        RectTransform newNodeRT = nodeGO.GetComponent<RectTransform>();
        newNodeRT.anchoredPosition = newNodePosition;
        nodeGO.transform.SetAsLastSibling();

        UINode uiNode = nodeGO.GetComponent<UINode>();
        uiNode.Initialize(targetNodeId, node.title, node.description, this);

        spawnedNodes[targetNodeId] = nodeGO;

        // Draw line
        GameObject lineGO = Instantiate(linePrefab, Content);
        RectTransform lineRT = lineGO.GetComponent<RectTransform>();
        lineGO.transform.SetAsFirstSibling();
        DrawUILine(lineRT, basePosition, newNodePosition);
        connectingLines[targetNodeId] = lineGO;

        nodeDirections[targetNodeId] = direction;
    }

    private void ShowNodeFlowForm(string fromNodeId, string targetNodeId, FlowDirection direction)
    {
        if (!sceneGraphManager.sceneNodes.ContainsKey(fromNodeId)) return;
        if (!sceneGraphManager.sceneNodes.ContainsKey(targetNodeId)) return;

        if (spawnedNodes.ContainsKey(targetNodeId)) return;

        SpawnSceneNodeFrom(fromNodeId, targetNodeId, direction);
    }

    public void SpawnNode(string nodeId, string title, string description, bool isLeftBranch)
    {
        Vector2 position;

        if (lastNodeId == null || !spawnedNodes.ContainsKey(lastNodeId))
        {
            // First node → center of the screen (Y = a bit lower to leave space for branching upward)
            float canvasHeight = graphRoot.rect.height;
            position = new Vector2(0, -canvasHeight / 2 + 100f);
        }
        else
        {
            // Branch from the last clicked node
            Vector2 lastPos = spawnedNodes[lastNodeId].GetComponent<RectTransform>().anchoredPosition;

            float rad = Mathf.Deg2Rad * (isLeftBranch ? 157.5f : 22.5f); // Left or right up angle
            Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * distance;

            position = lastPos + offset;
        }

        // Create the node
        GameObject nodeGO = Instantiate(nodePrefab, Content);
        RectTransform rt = nodeGO.GetComponent<RectTransform>();
        rt.anchoredPosition = position;

        UINode uiNode = nodeGO.GetComponent<UINode>();
        uiNode.Initialize(nodeId, title, description, this);

        spawnedNodes[nodeId] = nodeGO;

        // Draw a line from the previous node (if there is one)
        if (lastNodeId != null && spawnedNodes.ContainsKey(lastNodeId))
        {
            GameObject lineGO = Instantiate(linePrefab, Content);
            RectTransform lineRT = lineGO.GetComponent<RectTransform>();

            Vector2 start = spawnedNodes[lastNodeId].GetComponent<RectTransform>().anchoredPosition;
            Vector2 end = rt.anchoredPosition;

            DrawUILine(lineRT, start, end);

            connectingLines[nodeId] = lineGO;
        }

        // Set this node as the new "root" for future branches
        lastNodeId = nodeId;
    }


    public void PruneNode(string nodeId)
    {
        if (connectingLines.ContainsKey(nodeId))
        {
            Destroy(connectingLines[nodeId]);
            connectingLines.Remove(nodeId);
        }

        if (spawnedNodes.ContainsKey(nodeId))
        {
            Destroy(spawnedNodes[nodeId]);
            spawnedNodes.Remove(nodeId);
        }
    }

    private void DrawUILine(RectTransform lineRT, Vector2 start, Vector2 end)
    {
        Vector2 dir = end - start;
        float length = dir.magnitude;

        lineRT.sizeDelta = new Vector2(length, 9f); // line thickness
        lineRT.pivot = new Vector2(0, 0.5f);
        lineRT.anchoredPosition = start;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        lineRT.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void OnNodeClicked(string nodeId)
    {
        if (!sceneGraphManager.sceneNodes.ContainsKey(nodeId)) return;
        SceneNode current = sceneGraphManager.sceneNodes[nodeId];
        var connections = current.connectedNodeIds;

        // SPAWN CONNECTED NODES
        if (connections.Count == 1)
        {
            string nextId = connections[0];
            SceneNode nextNode = sceneGraphManager.sceneNodes[nextId];

            FlowDirection dir = nodeDirections.ContainsKey(nodeId) && nextNode.isAutoProgress
                ? nodeDirections[nodeId]
                : FlowDirection.Up;

            ShowNodeFlowForm(nodeId, nextId, dir);
        }
        else if (connections.Count > 1)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                string nextId = connections[i];
                FlowDirection dir = (i == 0) ? FlowDirection.Left : FlowDirection.Right;
                ShowNodeFlowForm(nodeId, nextId, dir);
            }
        }

        // CLEANUP OTHER OPTIONS (if previous node had choices)
        foreach (var kvp in sceneGraphManager.sceneNodes)
        {
            SceneNode parent = kvp.Value;
            if (parent.connectedNodeIds.Contains(nodeId) && parent.connectedNodeIds.Count > 1)
            {
                foreach (string sibling in parent.connectedNodeIds)
                {
                    if (sibling == nodeId) continue;

                    // Disable button & remove line
                    if (spawnedNodes.ContainsKey(sibling))
                    {
                        var uiNode = spawnedNodes[sibling].GetComponent<UINode>();
                        if (uiNode != null && uiNode.clickButton != null)
                        {
                            uiNode.clickButton.interactable = false;
                            if (!disabledNodeIds.Contains(sibling))
                            {
                                disabledNodeIds.Add(sibling);
                            }
                        }
                    }

                    if (connectingLines.ContainsKey(sibling))
                    {
                        Destroy(connectingLines[sibling]);
                        connectingLines.Remove(sibling);
                    }
                }

                break; // only one parent will match
            }
        }

        // Scene loading
        if (!string.IsNullOrEmpty(current.contentId))
        {
            //SaveGraphState();
            NetworkManager.Instance.SaveSpawnedNodes(spawnedNodes, sceneGraphManager);
            PlayerPrefs.SetString("SelectedContentID", current.contentId);
            PlayerPrefs.SetString("SelectedNodeID", current.nodeId);

            if (current.nodeType == NodeType.MiniGame)
            {
                Debug.Log("MiniGame node clicked");
                SceneManager.LoadScene("Minigame");
            }
            else
            {
                Debug.Log("Info node clicked");
                SceneManager.LoadScene("Information");
            }
        }
    }


    public void SaveGraphState()
    {
        PlayerPrefs.SetString("Graph_LastNode", lastNodeId);

        List<string> visited = new List<string>(spawnedNodes.Keys);
        string visitedStr = string.Join(",", visited);

        PlayerPrefs.SetString("Graph_VisitedNodes", visitedStr);

        foreach (var node in spawnedNodes)
        {
            string key = $"NodePos_{node.Key}";
            Vector2 pos = node.Value.GetComponent<RectTransform>().anchoredPosition;
            string posStr = $"{pos.x},{pos.y}";
            PlayerPrefs.SetString(key, posStr);
        }

        List<string> connections = new List<string>();
        foreach (var line in connectingLines)
        {
            string toNode = line.Key;

            // Try to find the node that connects TO this node
            foreach (var fromNode in spawnedNodes)
            {
                if (fromNode.Key == toNode) continue;

                Vector2 fromPos = fromNode.Value.GetComponent<RectTransform>().anchoredPosition;
                Vector2 toPos = spawnedNodes[toNode].GetComponent<RectTransform>().anchoredPosition;

                float dist = Vector2.Distance(fromPos, toPos);
                if (dist < distance + 10f && dist > distance - 10f) // rough distance match
                {
                    connections.Add($"{fromNode.Key}>{toNode}");
                    break;
                }
            }
        }

        string connectionStr = string.Join(",", connections);
        PlayerPrefs.SetString("Graph_Lines", connectionStr);
        string disabledStr = string.Join(",", disabledNodeIds);
        PlayerPrefs.SetString("Graph_DisabledNodes", disabledStr);

        PlayerPrefs.Save();
    }

    public void LoadGraphStateDatabase(Action<bool> onFinished)
    {
        bool spawnedAny = false;
        bool completed = false;
        bool callbackFired = false;

        void Finish()
        {
            if (callbackFired) return;
            callbackFired = true;
            onFinished?.Invoke(spawnedAny);
        }

        NetworkManager.Instance.MakeRequest<RouteStep>("routes/A", RequestType.GET, null, response =>
        {
            if (response != null)
            {
                spawnedAny = true;
                SpawnNode(response.id, response.title, response.description, response.x, response.y);
                lastNodeId = response.id;
                return;
            }
            completed = true;
            Finish();
        });
        

        StartCoroutine(FireIfNothingArrived(Finish, () => completed));
    }

    private IEnumerator FireIfNothingArrived(Action finished, Func<bool> isDone)
    {
        const float TIMEOUT = 5f;
        float timer = 0f;

        while (!isDone() && timer < TIMEOUT)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        Debug.Log("FireIfNothingArrived");
        
        finished();
    }

    private void SpawnNode(string id, string title, string description, float x, float y)
    {
        GameObject nodeGO = Instantiate(nodePrefab, Content);
        RectTransform rt = nodeGO.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(x, y);

        UINode uiNode = nodeGO.GetComponent<UINode>();
        uiNode.Initialize(id.ToString(), title, description, this);

        spawnedNodes[title.ToString()] = nodeGO;
        Debug.Log($"Node spawned: {title} at ({x}, {y})");
    }

    public void LoadGraphState()
    {
        if (!PlayerPrefs.HasKey("Graph_LastNode") || !PlayerPrefs.HasKey("Graph_VisitedNodes")) { Debug.Log("No saved graph state found"); return; }

        lastNodeId = PlayerPrefs.GetString("Graph_LastNode");
        string visitedStr = PlayerPrefs.GetString("Graph_VisitedNodes");
        string[] visitedIds = visitedStr.Split(',');
        
        disabledNodeIds = new List<string>();
        if (PlayerPrefs.HasKey("Graph_DisabledNodes"))
        {
            string[] disabled = PlayerPrefs.GetString("Graph_DisabledNodes").Split(',');
            disabledNodeIds.AddRange(disabled);
        }

        foreach (string nodeId in visitedIds)
        {
            if (sceneGraphManager.sceneNodes.TryGetValue(nodeId, out var node))
            {
                Vector2 position = Vector2.zero;

                // Load saved position
                string key = $"NodePos_{nodeId}";
                if (PlayerPrefs.HasKey(key))
                {
                    string[] coords = PlayerPrefs.GetString(key).Split(',');
                    if (coords.Length == 2 &&
                        float.TryParse(coords[0], out float x) &&
                        float.TryParse(coords[1], out float y))
                    {
                        position = new Vector2(x, y);
                    }
                }
                else
                {
                    Debug.LogWarning($"No saved position for node {nodeId}, using default.");
                    position = new Vector2(0, -800 + Array.IndexOf(visitedIds, nodeId) * 200);
                }

                // Instantiate and position
                GameObject nodeGO = Instantiate(nodePrefab, Content);
                RectTransform rt = nodeGO.GetComponent<RectTransform>();
                nodeGO.transform.SetAsLastSibling();
                rt.anchoredPosition = position;

                UINode uiNode = nodeGO.GetComponent<UINode>();
                uiNode.Initialize(nodeId, node.title, node.description, this);

                spawnedNodes[nodeId] = nodeGO;
                nodeDirections[nodeId] = FlowDirection.Up;

                if (disabledNodeIds.Contains(nodeId))
                {
                    uiNode.clickButton.interactable = false;
                }
            }
            else
            {
                Debug.LogWarning($"Node ID '{nodeId}' not found in sceneGraphManager.");
            }
        }

        // Load and draw lines
        if (PlayerPrefs.HasKey("Graph_Lines"))
        {
            string connectionStr = PlayerPrefs.GetString("Graph_Lines");
            string[] pairs = connectionStr.Split(',');

            foreach (var pair in pairs)
            {
                if (string.IsNullOrEmpty(pair)) continue;

                string[] parts = pair.Split('>');
                if (parts.Length != 2) continue;

                string from = parts[0];
                string to = parts[1];

                if (spawnedNodes.ContainsKey(from) && spawnedNodes.ContainsKey(to))
                {
                    Vector2 start = spawnedNodes[from].GetComponent<RectTransform>().anchoredPosition;
                    Vector2 end = spawnedNodes[to].GetComponent<RectTransform>().anchoredPosition;

                    GameObject lineGO = Instantiate(linePrefab, Content);
                    RectTransform lineRT = lineGO.GetComponent<RectTransform>();
                    lineGO.transform.SetAsFirstSibling();
                    DrawUILine(lineRT, start, end);

                    connectingLines[to] = lineGO;
                }
            }
        }
    }
}
