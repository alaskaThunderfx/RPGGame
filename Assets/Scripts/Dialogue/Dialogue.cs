using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();

        private Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

        private void OnValidate()
        {
            nodeLookup.Clear();
            foreach (var node in GetAllNodes())
            {
                nodeLookup[node.name] = node;
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach (string childID in parentNode.GetChildren())
            {
                if (nodeLookup.ContainsKey(childID))
                {
                    yield return nodeLookup[childID];
                }
            }
        }

#if UNITY_EDITOR
        public void CreateNode(DialogueNode parentNode)
        {
            var newNode = MakeNode(parentNode);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "Added Dialogue Node");
            AddNode(newNode);
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Removed Dialogue Node");
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private static DialogueNode MakeNode(DialogueNode parentNode)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();

            if (parentNode != null)
            {
                parentNode.AddChild(newNode.name);
            }

            return newNode;
        }

        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);

            OnValidate();
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (var node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }
#endif

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count == 0)
            {
                var newNode = MakeNode(null);
                AddNode(newNode);
            }

            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (DialogueNode node in GetAllNodes())
                {
                    if (AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}