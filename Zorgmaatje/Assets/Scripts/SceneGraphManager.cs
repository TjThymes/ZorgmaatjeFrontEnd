using System.Collections.Generic;
using UnityEngine;

public class SceneGraphManager : MonoBehaviour
{
    public Dictionary<string, SceneNode> nodeMap = new Dictionary<string, SceneNode>();
    public List<string> history = new List<string>();
    public int historyLimit = 5;

    public string currentNodeId;
    private SceneNode currentNode;
    private List<string> currentChoices = new List<string>();

    public Dictionary<string, SceneNode> sceneNodes = new();

    void Start()
    {
        CreateGraph();
        ShowCurrentNode("A");
    }

    void CreateGraph()
    {
        var a0 = new SceneNode("A0", "Start", "diabetes type 1", NodeType.Info, "Start");
        var a1 = new SceneNode("A1", "Kinderarts", "verdere uitleg", NodeType.Info, "Kinderarts", true);
        var a2 = new SceneNode("A2", "Eerste beoordeling", "verdere uitleg", NodeType.Info, "Beoordeling", true);
        var a3 = new SceneNode("A3", "Bloedonderzoek", "verdere uitleg", NodeType.Info, "Bloedonderzoek", true);
        var b00 = new SceneNode("B00", "Zorg thuis", "verdere uitleg", NodeType.Info, "Thuis");
        var b10 = new SceneNode("B10", "Ziekenhuis", "verdere uitleg", NodeType.Info, "Ziekenhuis");
        var c0 = new SceneNode("C0", "Herstel/Controle", "verdere uitleg", NodeType.Info, "Herstel");
        var d0 = new SceneNode("D0", "Dagelijksleven", "verdere uitleg", NodeType.Info, "Dagelijksleven");
        var e0 = new SceneNode("E0", "Sport", "meer over Sport", NodeType.Info, "Sport");
        var e1 = new SceneNode("E1", "Duursport", "uitleg over duursporten", NodeType.MiniGame, "Fietsen", true);
        var f0 = new SceneNode("F0", "Eten", "meer over eten", NodeType.Info, "Eten");
        var f1 = new SceneNode("F1", "Gezond eten", "uitleg over gezond eten", NodeType.MiniGame, "GezondEten", true);

        a0.AddConnection("Kinderarts");
        a1.AddConnection("Eerste beoordeling");
        a2.AddConnection("Bloedonderzoek");
        a3.AddConnection("Zorg thuis");
        a3.AddConnection("Ziekenhuis");
        b00.AddConnection("Herstel/Controle");
        b10.AddConnection("Herstel/Controle");
        c0.AddConnection("Dagelijksleven");
        d0.AddConnection("Sport");
        d0.AddConnection("Duursport");
        e0.AddConnection("Eten");
        f0.AddConnection("Gezond eten");

        sceneNodes["Start"] = a0;
        sceneNodes["Kinderarts"] = a1;
        sceneNodes["Eerste beoordeling"] = a2;
        sceneNodes["Bloedonderzoek"] = a3;
        sceneNodes["Zorg thuis"] = b00;
        sceneNodes["Ziekenhuis"] = b10;
        sceneNodes["Herstel/Controle"] = c0;
        sceneNodes["Dagelijksleven"] = d0;
        sceneNodes["Sport"] = e0;
        sceneNodes["Duursport"] = e1;
        sceneNodes["Eten"] = f0;
        sceneNodes["Gezond eten"] = f1;


    }

    public void ShowCurrentNode(string nodeId)
    {
        if (!nodeMap.ContainsKey(nodeId)) return;

        currentNode = nodeMap[nodeId];
        currentNodeId = nodeId;
        history.Add(nodeId);
        if (history.Count > historyLimit) history.RemoveAt(0);

        Debug.Log($"Current: {currentNode.title}\n{currentNode.description}");

        currentChoices = GetTwoChoices(currentNode);
        Debug.Log($"Choice 1: {currentChoices[0]} | Choice 2: {currentChoices[1]}");
    }

    public void Choose(string chosenNodeId)
    {
        if (nodeMap.ContainsKey(chosenNodeId))
        {
            ShowCurrentNode(chosenNodeId);
        }
        else
        {
            Debug.LogWarning($"Invalid choice: {chosenNodeId}");
        }
    }

    List<string> GetTwoChoices(SceneNode node)
    {
        List<string> options = new List<string>();

        foreach (string id in node.connectedNodeIds)
        {
            int weight = history.Contains(id) ? 1 : 3; // Favor newer nodes
            for (int i = 0; i < weight; i++)
            {
                options.Add(id);
            }
        }

        // Shuffle and pick two unique choices
        Shuffle(options);
        HashSet<string> unique = new HashSet<string>();

        foreach (var id in options)
        {
            unique.Add(id);
            if (unique.Count == 2) break;
        }

        // Fallback in case there are not enough unique choices
        while (unique.Count < 2 && node.connectedNodeIds.Count > 0)
        {
            unique.Add(node.connectedNodeIds[Random.Range(0, node.connectedNodeIds.Count)]);
        }

        return new List<string>(unique);
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
