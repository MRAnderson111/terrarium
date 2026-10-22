using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Draw only the prefab's meshes: no cloned behaviours, Awake, colliders, AI or growth.
public sealed class PlacementPreview
{
    struct Part
    {
        public Mesh mesh;
        public Matrix4x4 localMatrix;
        public Material[] materials;
    }

    readonly List<Part> parts = new();
    Vector3 scale;

    // Height of the model's lowest point relative to the prefab root; negative when the mesh hangs below the pivot.
    public float BottomOffset { get; private set; }

    public void Clear()
    {
        parts.Clear();
        BottomOffset = 0f;
    }

    public void SetPrefab(GameObject prefab)
    {
        Clear();
        scale = prefab.transform.localScale;
        foreach (Renderer renderer in prefab.GetComponentsInChildren<Renderer>())
        {
            if (!renderer.enabled)
                continue;
            Mesh mesh = renderer is SkinnedMeshRenderer skinned ? skinned.sharedMesh
                : renderer.TryGetComponent(out MeshFilter filter) ? filter.sharedMesh : null;
            if (mesh == null)
                continue;
            parts.Add(new Part
            {
                mesh = mesh,
                localMatrix = prefab.transform.worldToLocalMatrix * renderer.transform.localToWorldMatrix,
                materials = renderer.sharedMaterials
            });
        }
        BottomOffset = MeasureBottom();
    }

    float MeasureBottom()
    {
        Matrix4x4 root = Matrix4x4.Scale(scale);
        float lowest = float.PositiveInfinity;
        foreach (Part part in parts)
        {
            Bounds bounds = part.mesh.bounds;
            Matrix4x4 matrix = root * part.localMatrix;
            for (int i = 0; i < 8; i++)
            {
                Vector3 corner = bounds.center + Vector3.Scale(bounds.extents,
                    new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
                lowest = Mathf.Min(lowest, matrix.MultiplyPoint3x4(corner).y);
            }
        }
        return float.IsInfinity(lowest) ? 0f : lowest;
    }

    public void Draw(Vector3 position, Quaternion rotation, Camera camera)
    {
        Matrix4x4 root = Matrix4x4.TRS(position, rotation, scale);
        foreach (Part part in parts)
            for (int i = 0; i < part.mesh.subMeshCount && i < part.materials.Length; i++)
                if (part.materials[i] != null)
                    Graphics.DrawMesh(part.mesh, root * part.localMatrix, part.materials[i], 2,
                        camera, i, null, ShadowCastingMode.Off, false);
    }
}
