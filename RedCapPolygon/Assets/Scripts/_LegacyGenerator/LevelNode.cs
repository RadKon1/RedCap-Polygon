using System.Collections.Generic;
using UnityEngine;

namespace LegacyGenerator
{
    public enum RoomType
    {
        Start,
        Combat,
        UpgradeRoom,
        Transition,
        Boss
    }

    public class LevelNode
    {
        public RoomType NodeType { get; private set; }
        public List<LevelNode> NextNodes { get; private set; } = new List<LevelNode>();
        public LevelNode(RoomType assignedType)
        {
            NodeType = assignedType;
        }

        public void AddNextNode(LevelNode node)
        {
            NextNodes.Add(node);
        }
    }
}