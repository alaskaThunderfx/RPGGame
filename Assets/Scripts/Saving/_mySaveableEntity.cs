using System;
using System.Collections.Generic;
using RPG.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Saving
{
    [ExecuteAlways]
    public class _mySaveableEntity : MonoBehaviour
    {
        [SerializeField]
        string uniqueIdentifier = "";
        static Dictionary<string, _mySaveableEntity> globalLookup =
            new Dictionary<string, _mySaveableEntity>();

        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        public object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }
            return state;

            // return new _mySerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> stateDict = (Dictionary<string, object>)state;
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                string typeString = saveable.GetType().ToString();
                if (stateDict.ContainsKey(typeString))
                {
                    saveable.RestoreState(stateDict[typeString]);
                }
            }

            // _mySerializableVector3 position = (_mySerializableVector3)state;
            // GetComponent<NavMeshAgent>().enabled = false;
            // transform.position = position.ToVector();
            // GetComponent<NavMeshAgent>().enabled = true;
            // GetComponent<ActionScheduler>().CancelCurrentAction();
        }

#if UNITY_EDITOR
        void Update()
        {
            if (Application.IsPlaying(gameObject))
                return;
            if (string.IsNullOrEmpty(gameObject.scene.path))
                return;

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");

            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                property.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            globalLookup[property.stringValue] = this;
        }

        bool IsUnique(string candidate)
        {
            if (!globalLookup.ContainsKey(candidate))
                return true;

            if (globalLookup[candidate] == this)
                return true;

            if (globalLookup[candidate] == null)
            {
                globalLookup.Remove(candidate);
                return true;
            }

            if (globalLookup[candidate].GetUniqueIdentifier() != candidate)
            {
                globalLookup.Remove(candidate);
                return true;
            }

            return false;
        }
#endif
    }
}
