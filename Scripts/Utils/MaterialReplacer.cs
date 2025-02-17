using UnityEngine;
using UnityEditor;

public class MaterialReplacer : MonoBehaviour
{
    public Material newMaterial;

    [ContextMenu("Replace Materials")]
    private void ReplaceMaterials()
    {
        var renderers = transform.GetComponentsInChildren<MeshRenderer>();

        Undo.RecordObjects(renderers, "Replace Materials");

        foreach (var rend in renderers)
        {
            rend.sharedMaterial = newMaterial;
        }
    }
}