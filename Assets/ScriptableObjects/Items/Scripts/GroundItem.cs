using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GroundItem : MonoBehaviour, ISerializationCallbackReceiver
{
    public ItemObject item;

    public void OnAfterDeserialize()
    {
    }

    public void OnBeforeSerialize()
    {
        //GetComponentInChildren<SpriteRenderer>().sprite = item.uiDisplay;
        //EditorUtility.SetDirty(GetComponentInChildren<SpriteRenderer>());
        GetComponentInChildren<MeshFilter>().mesh = item.groundItem.GetComponent<MeshFilter>().sharedMesh;
        EditorUtility.SetDirty(GetComponentInChildren<MeshFilter>());
        GetComponentInChildren<MeshRenderer>().material = item.groundItem.GetComponent<MeshRenderer>().sharedMaterial;
        EditorUtility.SetDirty(GetComponentInChildren<MeshRenderer>());
    }
}
