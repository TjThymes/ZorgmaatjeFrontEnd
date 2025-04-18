using System.Collections.Generic;

public class SceneNode
{
    public string nodeId;
    public string title;
    public string description;
    public NodeType nodeType = NodeType.Info;
    public string contentId = "";

    public List<string> connectedNodeIds = new();
    public bool isAutoProgress = false;
    public SceneNode(string id, string title, string desc, NodeType nodeType, string content)
    {
        nodeId = id;
        this.title = title;
        description = desc;
        contentId = content;
        this.nodeType = nodeType;
    }
    public SceneNode(string id, string title, string desc, NodeType nodeType, string content, bool isAutoProgress)
    {
        nodeId = id;
        this.title = title;
        description = desc;
        contentId = content;
        this.nodeType = nodeType;
        this.isAutoProgress = isAutoProgress;
    }

    public void AddConnection(string targetId)
    {
        if (!connectedNodeIds.Contains(targetId))
            connectedNodeIds.Add(targetId);
    }
}
