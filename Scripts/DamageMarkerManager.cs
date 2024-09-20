using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMarkerManager : MonoBehaviour
{
    [SerializeField] private float setPositionZ = -0.1f;
    [SerializeField] private bool isDebugging;
    [SerializeField] private GameObject damageMarkerObjectPrefab; // Prefab with DamageMarkerAddon attached
    [SerializeField] private Material damageMarkerCircleEdgeMaterial; // Material for circle edges
    [SerializeField] private Material damageMarkerCircleBaseMaterial; // Material for circle base
    [SerializeField] private Material damageMarkerRectangleEdgeMaterial; // Material for rectangle
    [SerializeField] private Material damageMarkerRectangleBaseMaterial; // Material for rectangle
    [SerializeField] private Material damageMarkerConeEdgeMaterial; // Material for cone edges
    [SerializeField] private Material damageMarkerConeBaseMaterial; // Material for cone base

    // Define custom orange color
    private Color customOrange = new Color(1f, 0.5f, 0f);

    // Method to create a circle damage marker
    public void CreateDamageMarkerCircle(Vector3 position, float radius, float lifetime)
    {
        GameObject damageMarker = Instantiate(damageMarkerObjectPrefab, new Vector3(position.x, position.y, setPositionZ), Quaternion.identity);
        damageMarker.transform.localRotation = Quaternion.Euler(0, 180, 0);
        DamageMarkerAddon addon = damageMarker.GetComponent<DamageMarkerAddon>();
        if (addon != null)
        {
            CreateSpherePrimitive(damageMarker.transform, radius);
            addon.SetLifetime(lifetime);
        }
    }

    // Method to create a rectangle damage marker
    public void CreateDamageMarkerRectangle(Vector3 position, float width, float height, float lifetime)
    {
        GameObject damageMarker = Instantiate(damageMarkerObjectPrefab, new Vector3(position.x, position.y, setPositionZ), Quaternion.identity);
        DamageMarkerAddon addon = damageMarker.GetComponent<DamageMarkerAddon>();
        if (addon != null)
        {
            CreateBoxPrimitive(damageMarker.transform, width, height);
            addon.SetLifetime(lifetime);
        }
    }

    // Method to create a cone damage marker
    public void CreateDamageMarkerCone(Vector3 position, float direction, float length, float width, float lifetime)
    {
        GameObject damageMarker = Instantiate(damageMarkerObjectPrefab, new Vector3(position.x, position.y, setPositionZ), Quaternion.identity);
        damageMarker.transform.localRotation = Quaternion.Euler(0, 180, 0);
        DamageMarkerAddon addon = damageMarker.GetComponent<DamageMarkerAddon>();
        if (addon != null)
        {
            CreateConePrimitive(damageMarker.transform, direction, length, width);
            addon.SetLifetime(lifetime);
        }
    }
    private void CreateCirclePrimitive(Transform transform, float radius)
    {
        MeshFilter meshFilter = transform.gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = transform.gameObject.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = transform.gameObject.AddComponent<MeshCollider>();

        Mesh mesh = new Mesh();
        int segmentCount = 36;
        Vector3[] vertices = new Vector3[segmentCount + 1];
        int[] triangles = new int[segmentCount * 3];
        Vector2[] uv = new Vector2[vertices.Length];
        Color[] colors = new Color[vertices.Length];

        vertices[0] = Vector3.zero;
        colors[0] = Color.yellow;

        for (int i = 0; i < segmentCount; i++)
        {
            float angle = Mathf.Deg2Rad * i * (360f / segmentCount);
            vertices[i + 1] = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            colors[i + 1] = Color.Lerp(Color.yellow, customOrange, 1f);
            uv[i + 1] = new Vector2(vertices[i + 1].x / (2 * radius) + 0.5f, vertices[i + 1].y / (2 * radius) + 0.5f);
        }

        for (int i = 0; i < segmentCount; i++)
        {
            int next = (i + 1) % segmentCount;
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = next + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.uv = uv;

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh; // Assign the generated mesh to the collider

        transform.localScale = new Vector3(1, 1, 1); // Set Z scale to 1

        // Set materials to the MeshRenderer
        meshRenderer.materials = new Material[] { damageMarkerCircleBaseMaterial, damageMarkerCircleEdgeMaterial };
    }
    private void CreateSpherePrimitive(Transform transform, float radius)
    {
        MeshFilter meshFilter = transform.gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = transform.gameObject.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = transform.gameObject.AddComponent<MeshCollider>();

        // Use Unity's built-in Sphere primitive mesh
        Mesh sphereMesh = CreateSphereMesh(radius);
        meshFilter.mesh = sphereMesh;
        meshCollider.sharedMesh = sphereMesh; // Assign the generated mesh to the collider

        // Set the scale, with a Z scale of 0.2f
        transform.localScale = new Vector3(1, 1, 0.2f);

        // Set materials to the MeshRenderer
        meshRenderer.materials = new Material[] { damageMarkerCircleBaseMaterial, damageMarkerCircleEdgeMaterial };
    }

    private Mesh CreateSphereMesh(float radius)
    {
        GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Mesh mesh = tempSphere.GetComponent<MeshFilter>().sharedMesh;
        Destroy(tempSphere); // Clean up temporary game object
        return mesh;
    }

    private void CreateRectanglePrimitive(Transform transform, float width, float height)
    {
        MeshFilter meshFilter = transform.gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = transform.gameObject.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = transform.gameObject.AddComponent<MeshCollider>();

        Mesh mesh = new Mesh();
        Vector3[] vertices = {
        new Vector3(-width / 2, -height / 2, 0),
        new Vector3(width / 2, -height / 2, 0),
        new Vector3(-width / 2, height / 2, 0),
        new Vector3(width / 2, height / 2, 0)
    };
        int[] triangles = { 0, 2, 1, 1, 2, 3 };
        Color[] colors = new Color[vertices.Length];
        Color centerColor = Color.yellow;
        Color edgeColor = Color.Lerp(Color.yellow, customOrange, 1f);

        colors[0] = colors[1] = colors[2] = colors[3] = edgeColor;
        colors[0] = centerColor;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh; // Assign the generated mesh to the collider

        transform.localScale = new Vector3(1, 1, 1); // Set Z scale to 1

        // Set material to the MeshRenderer
        meshRenderer.materials = new Material[] { damageMarkerCircleBaseMaterial, damageMarkerCircleEdgeMaterial };
    }
    private void CreateBoxPrimitive(Transform transform, float width, float height)
    {
        MeshFilter meshFilter = transform.gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = transform.gameObject.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = transform.gameObject.AddComponent<MeshCollider>();

        // Use Unity's built-in Cube primitive mesh
        Mesh boxMesh = CreateBoxMesh(width, height);
        meshFilter.mesh = boxMesh;
        meshCollider.sharedMesh = boxMesh; // Assign the generated mesh to the collider

        // Set the scale to match the width and height with a Z scale of 0.1f
        transform.localScale = new Vector3(width, height, 0.1f);

        // Set materials to the MeshRenderer
        meshRenderer.materials = new Material[] { damageMarkerRectangleBaseMaterial, damageMarkerRectangleEdgeMaterial };
    }

    private Mesh CreateBoxMesh(float width, float height)
    {
        // Create a temporary box object
        GameObject tempBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Mesh mesh = tempBox.GetComponent<MeshFilter>().sharedMesh;
        Destroy(tempBox); // Clean up temporary game object
        return mesh;
    }

    private void CreateConePrimitive(Transform transform, float direction, float length, float width)
    {
        MeshFilter meshFilter = transform.gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = transform.gameObject.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = transform.gameObject.AddComponent<MeshCollider>();

        // Create a 3D cone mesh
        Mesh coneMesh = CreateConeMesh(length, width);
        meshFilter.mesh = coneMesh;
        meshCollider.sharedMesh = coneMesh; // Assign the generated mesh to the collider

        // Set the rotation to point in the desired direction
        transform.rotation = Quaternion.Euler(0, direction, 0);

        // Set materials to the MeshRenderer
        meshRenderer.materials = new Material[] { damageMarkerConeBaseMaterial, damageMarkerConeEdgeMaterial };
    }

    private Mesh CreateConeMesh(float height, float baseRadius)
    {
        Mesh mesh = new Mesh();

        int segments = 36;
        int vertCount = segments + 2;
        int triCount = segments * 2;
        int triIndexCount = triCount * 3;

        Vector3[] vertices = new Vector3[vertCount];
        int[] triangles = new int[triIndexCount];
        Vector3[] normals = new Vector3[vertCount];
        Vector2[] uv = new Vector2[vertCount];

        // Tip of the cone
        vertices[0] = Vector3.up * height;
        normals[0] = Vector3.up;
        uv[0] = new Vector2(0.5f, 1);

        // Center of the base
        vertices[1] = Vector3.zero;
        normals[1] = Vector3.down;
        uv[1] = new Vector2(0.5f, 0);

        // Vertices around the base
        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / (segments - 1) * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * baseRadius;
            float z = Mathf.Sin(angle) * baseRadius;
            vertices[i + 2] = new Vector3(x, 0, z);
            normals[i + 2] = Vector3.Normalize(new Vector3(x, height, z));
            uv[i + 2] = new Vector2((float)i / (segments - 1), 0);
        }

        // Triangles around the cone's surface
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 2;
            triangles[i * 3 + 2] = i + 3 <= segments + 1 ? i + 3 : 2;
        }

        // Triangles of the base
        for (int i = 0; i < segments; i++)
        {
            triangles[segments * 3 + i * 3] = 1;
            triangles[segments * 3 + i * 3 + 1] = i + 3 <= segments + 1 ? i + 3 : 2;
            triangles[segments * 3 + i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;

        return mesh;
    }


}
