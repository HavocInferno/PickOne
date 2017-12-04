﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Utility to visit all chirdren in the subtree recursively.
/// </summary>
static public class Hierarchy
{
    static public IEnumerable<Transform> GetTransforms(Transform root)
    {
        Stack<Transform> stack = new Stack<Transform>();
        stack.Push(root);
        while (stack.Count > 0)
        {
            Transform currentTransform = stack.Pop();
            yield return currentTransform;
            if (!currentTransform)
                continue; // this transform was destroyed
            foreach (Transform child in currentTransform)
                stack.Push(child);
        }
    }

    static public IEnumerable<ComponentType> GetComponents<ComponentType>(Transform root)
    {
        foreach (var transform in GetTransforms(root))
        {
            var component = transform.gameObject.GetComponent<ComponentType>();
            if (component != null)
                yield return component;
        }
    }

    static public IEnumerable<Material> GetMaterials(Transform root)
    {
        foreach (var meshRenderer in GetComponents<MeshRenderer>(root))
        { 
            if (meshRenderer != null && meshRenderer.material != null)
                yield return meshRenderer.material;
        }
    }
}