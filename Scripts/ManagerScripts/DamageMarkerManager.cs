using System.Collections.Generic;
using UnityEngine;
using static DamageMarkerAddon;
using static DamageMarkerManager;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DamageMarkerManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private DebugInfo debug;

    [Header("Outline Settings")]
    [SerializeField] private float defaultOutlineWidth = 0.1f;
    [SerializeField] private int circleSegments = 32;
    [SerializeField] private int coneSegments = 16;


    public void InitializeDamageMarkerManager()
    {
        
    }
    private GameObject CreateMarkerGO(string name, Vector3 position)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform);
        go.transform.localPosition = position;
        return go;
    }

    // --- Circle ---
    public GameObject CreateCircleMarker(
        Vector3 position,
        float radius,
        Color fillColor,
        float fillAlpha = 1f,
        float outlineWidth = -1f,
        Color? outlineColor = null,
        float outlineAlpha = 1f)
    {
        if (outlineWidth < 0) outlineWidth = defaultOutlineWidth;
        Color oc = outlineColor ?? Color.black;
        oc.a = outlineAlpha;
        fillColor.a = fillAlpha;

        var go = CreateMarkerGO("CircleMarker", position);
        // Fill mesh
        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Sprites/Default"));
        mr.material.color = fillColor;
        mr.sortingLayerName = "DamageMarkers";
        mr.sortingOrder = 0;

        Mesh mesh = new Mesh();
        int seg = circleSegments;
        Vector3[] verts = new Vector3[seg + 1];
        int[] tris = new int[seg * 3];
        verts[0] = Vector3.zero;
        for (int i = 0; i < seg; i++)
        {
            float angle = 2 * Mathf.PI * i / seg;
            verts[i + 1] = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        }
        for (int i = 0; i < seg; i++)
        {
            tris[i * 3] = 0;
            tris[i * 3 + 1] = i + 1;
            tris[i * 3 + 2] = (i + 1) % seg + 1;
        }
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mf.mesh = mesh;

        // Outline
        CreateOutline(go, radius, radius, oc, outlineWidth, circleSegments);
        return go;
    }

    // --- Donut ---
    public GameObject CreateDonutMarker(
        Vector3 position,
        float innerRadius,
        float outerRadius,
        Color fillColor,
        float fillAlpha = 1f,
        float outlineWidth = -1f,
        Color? outlineColor = null,
        float outlineAlpha = 1f)
    {
        if (outlineWidth < 0) outlineWidth = defaultOutlineWidth;
        Color oc = outlineColor ?? Color.black;
        oc.a = outlineAlpha;
        fillColor.a = fillAlpha;

        var go = CreateMarkerGO("DonutMarker", position);
        // Fill mesh (ring)
        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Sprites/Default"));
        mr.material.color = fillColor;
        mr.sortingLayerName = "DamageMarkers";
        mr.sortingOrder = 0;

        Mesh mesh = new Mesh();
        int seg = circleSegments;
        List<Vector3> verts = new List<Vector3>();
        List<int> trisList = new List<int>();
        for (int i = 0; i <= seg; i++)
        {
            float angle = 2 * Mathf.PI * i / seg;
            Vector3 outer = new Vector3(Mathf.Cos(angle) * outerRadius, Mathf.Sin(angle) * outerRadius, 0);
            Vector3 inner = new Vector3(Mathf.Cos(angle) * innerRadius, Mathf.Sin(angle) * innerRadius, 0);
            verts.Add(outer);
            verts.Add(inner);
        }
        for (int i = 0; i < seg * 2; i += 2)
        {
            trisList.Add(i);
            trisList.Add(i + 1);
            trisList.Add(i + 2);
            trisList.Add(i + 2);
            trisList.Add(i + 1);
            trisList.Add(i + 3);
        }
        mesh.SetVertices(verts);
        mesh.SetTriangles(trisList, 0);
        mesh.RecalculateNormals();
        mf.mesh = mesh;

        // Outline outer & inner
        CreateOutline(go, outerRadius, outerRadius, oc, outlineWidth, circleSegments);
        CreateOutline(go, innerRadius, innerRadius, oc, outlineWidth, circleSegments);
        return go;
    }

    // --- Rectangle ---
    public GameObject CreateRectangleMarker(
        Vector3 position,
        float width,
        float height,
        Color fillColor,
        float fillAlpha = 1f,
        float outlineWidth = -1f,
        Color? outlineColor = null,
        float outlineAlpha = 1f)
    {
        if (outlineWidth < 0) outlineWidth = defaultOutlineWidth;
        Color oc = outlineColor ?? Color.black;
        oc.a = outlineAlpha;
        fillColor.a = fillAlpha;

        var go = CreateMarkerGO("RectangleMarker", position);
        // Fill
        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Sprites/Default"));
        mr.material.color = fillColor;
        mr.sortingLayerName = "DamageMarkers";
        mr.sortingOrder = 0;

        Mesh mesh = new Mesh();
        Vector3[] verts = new Vector3[4]
        {
            new Vector3(-width/2, -height/2),
            new Vector3(width/2, -height/2),
            new Vector3(width/2, height/2),
            new Vector3(-width/2, height/2)
        };
        int[] tris = new int[] { 0, 1, 2, 2, 3, 0 };
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mf.mesh = mesh;

        // Outline
        var outlineGO = new GameObject("Outline");
        outlineGO.transform.SetParent(go.transform);
        outlineGO.transform.localPosition = Vector3.zero;
        var lr = outlineGO.AddComponent<LineRenderer>();
        SetupLineRenderer(lr, oc, outlineWidth, loop: true);
        lr.positionCount = 5;
        lr.SetPosition(0, new Vector3(-width / 2, -height / 2, 0));
        lr.SetPosition(1, new Vector3(width / 2, -height / 2, 0));
        lr.SetPosition(2, new Vector3(width / 2, height / 2, 0));
        lr.SetPosition(3, new Vector3(-width / 2, height / 2, 0));
        lr.SetPosition(4, new Vector3(-width / 2, -height / 2, 0));
        return go;
    }

    // --- Cone ---
    public GameObject CreateConeMarker(
        Vector3 position,
        float angleDegrees, // <- new parameter
        float angleWidth,
        float length,
        Color fillColor,
        float fillAlpha = 1f,
        float outlineWidth = -1f,
        Color? outlineColor = null,
        float outlineAlpha = 1f)
    {
        if (outlineWidth < 0) outlineWidth = defaultOutlineWidth;
        Color oc = outlineColor ?? Color.black;
        oc.a = outlineAlpha;
        fillColor.a = fillAlpha;

        var go = CreateMarkerGO("ConeMarker", position);

        // Convert angle to direction vector
        float rad = angleDegrees * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);

        // Apply rotation
        go.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

        // Fill
        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Sprites/Default"));
        mr.material.color = fillColor;
        mr.sortingLayerName = "DamageMarkers";
        mr.sortingOrder = 0;

        Mesh mesh = new Mesh();
        int seg = coneSegments;
        Vector3[] verts = new Vector3[seg + 2];
        int[] tris = new int[seg * 3];
        verts[0] = Vector3.zero;
        for (int i = 0; i <= seg; i++)
        {
            float a = -angleWidth / 2 + angleWidth * i / seg;
            float r = Mathf.Deg2Rad * a;
            verts[i + 1] = new Vector3(Mathf.Cos(r) * length, Mathf.Sin(r) * length, 0);
        }
        for (int i = 0; i < seg; i++)
        {
            tris[i * 3] = 0;
            tris[i * 3 + 1] = i + 1;
            tris[i * 3 + 2] = i + 2;
        }
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mf.mesh = mesh;

        // Outline
        CreateOutline(go, length, angleWidth, oc, outlineWidth, coneSegments, isCone: true);
        return go;
    }


    // --- Helpers ---
    private void CreateOutline(GameObject parent, float radiusA, float radiusBOrSegments, Color color, float width, int segments, bool isCone = false)
    {
        var outlineGO = new GameObject("Outline");
        outlineGO.transform.SetParent(parent.transform);
        outlineGO.transform.localPosition = Vector3.zero;
        var lr = outlineGO.AddComponent<LineRenderer>();
        SetupLineRenderer(lr, color, width, loop: false);

        if (isCone)
        {
            lr.positionCount = segments + 3;
            lr.SetPosition(0, Vector3.zero); // Tip of the cone

            float rotationOffset = radiusBOrSegments; // This is the angleWidth in degrees

            for (int i = 0; i <= segments; i++)
            {
                float a = -rotationOffset / 2 + rotationOffset * i / segments;
                float rad = Mathf.Deg2Rad * a;
                Vector3 point = new Vector3(Mathf.Cos(rad) * radiusA, Mathf.Sin(rad) * radiusA, 0);

                // Rotate the point by parent.rotation (which is aligned to angleDegrees)
                point = parent.transform.rotation * point;

                lr.SetPosition(i + 1, point);
            }

            lr.SetPosition(segments + 2, Vector3.zero); // Close back to the center
        }

        else
        {
            lr.positionCount = segments + 1;
            for (int i = 0; i <= segments; i++)
            {
                float angle = 2 * Mathf.PI * i / segments;
                float r = radiusA;
                lr.SetPosition(i, new Vector3(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r, 0));
            }
        }
    }


    private void SetupLineRenderer(LineRenderer lr, Color color, float width, bool loop)
    {
        var mat = new Material(Shader.Find("Sprites/Default"));
        lr.material = mat;
        lr.useWorldSpace = false;
        lr.loop = loop;
        lr.startWidth = width;
        lr.endWidth = width;
        lr.startColor = color;
        lr.endColor = color;
        lr.numCornerVertices = 5;
        lr.numCapVertices = 5;
        lr.sortingLayerName = "DamageMarkers";
        lr.sortingOrder = 1;
    }


    #region // References
    private GameManager GetGameManager() => gameManager;
    #endregion

}








/*
public enum MarkerMaterialType { Base, Fire, Ice, Poison }
[Header("Materials by Type")]
[SerializeField] private Material baseFillMaterial;
[SerializeField] private Material baseOutlineMaterial;
[SerializeField] private Material fireFillMaterial;
[SerializeField] private Material fireOutlineMaterial;
[SerializeField] private Material iceFillMaterial;
[SerializeField] private Material iceOutlineMaterial;
[SerializeField] private Material poisonFillMaterial;
[SerializeField] private Material poisonOutlineMaterial;

[Header("Rendering")]
[SerializeField] private string sortingLayerName = "DamageMarkers";
[SerializeField] private int sortingOrder = 100;

private struct MatPair { public Material fill; public Material outline; }
private Dictionary<MarkerMaterialType, MatPair> matLookup;


public void InitializeDamageMarkerManager()
{
    matLookup = new Dictionary<MarkerMaterialType, MatPair>() {
        { MarkerMaterialType.Base, new MatPair { fill = baseFillMaterial, outline = baseOutlineMaterial } },
        { MarkerMaterialType.Fire, new MatPair { fill = fireFillMaterial, outline = fireOutlineMaterial } },
        { MarkerMaterialType.Ice, new MatPair { fill = iceFillMaterial, outline = iceOutlineMaterial } },
        { MarkerMaterialType.Poison, new MatPair { fill = poisonFillMaterial, outline = poisonOutlineMaterial } }
    };

    // Validate
    foreach (var kv in matLookup)
    {
        if (!kv.Value.fill || !kv.Value.outline)
            Debug.LogError($"Missing materials for {kv.Key}");
    }
}


private GameObject CreateBaseMarker(string name, Vector3 position, MarkerMaterialType type)
{
    var go = new GameObject(name);
    go.transform.SetParent(transform);
    go.transform.localPosition = position;
    var sr = go.AddComponent<SpriteRenderer>();
    sr.sortingLayerName = sortingLayerName;
    sr.sortingOrder = sortingOrder;
    // assign fill material now or later
    return go;
}

public GameObject CreateDamageMarkerCircle(Vector3 position, float radius, MarkerMaterialType type, float outlineWidth)
{
    var mats = matLookup[type];
    int segments = Mathf.Max(16, Mathf.CeilToInt(radius * 4));
    var sprite = GenerateCircleSprite(radius, segments, outlineWidth);
    var marker = CreateBaseMarker("CircleMarker", position, type);
    var sr = marker.GetComponent<SpriteRenderer>();
    sr.sprite = sprite;
    sr.material = mats.fill;
    AddOutline(marker, sprite, mats.outline, outlineWidth);
    return marker;
}

public GameObject CreateDamageMarkerDonut(Vector3 position, float innerRadius, float outerRadius, MarkerMaterialType type, float outlineWidth)
{
    var mats = matLookup[type];
    int segments = Mathf.Max(16, Mathf.CeilToInt(outerRadius * 4));
    var sprite = GenerateDonutSprite(innerRadius, outerRadius, segments, outlineWidth);
    var marker = CreateBaseMarker("DonutMarker", position, type);
    var sr = marker.GetComponent<SpriteRenderer>();
    sr.sprite = sprite;
    sr.material = mats.fill;
    AddOutline(marker, sprite, mats.outline, outlineWidth);
    return marker;
}

public GameObject CreateDamageMarkerRectangle(Vector3 position, float width, float height, MarkerMaterialType type, float outlineWidth)
{
    var mats = matLookup[type];
    var sprite = GenerateRectSprite(width, height, outlineWidth);
    var marker = CreateBaseMarker("RectMarker", position, type);
    var sr = marker.GetComponent<SpriteRenderer>();
    sr.sprite = sprite;
    sr.material = mats.fill;
    AddOutline(marker, sprite, mats.outline, outlineWidth);
    return marker;
}

public GameObject CreateDamageMarkerCone(Vector3 position, Vector3 direction, float width, float length, MarkerMaterialType type, float outlineWidth)
{
    var mats = matLookup[type];
    int segments = Mathf.Max(8, Mathf.CeilToInt(width * 2));
    var sprite = GenerateConeSprite(width, length, segments, outlineWidth);
    var marker = CreateBaseMarker("ConeMarker", position, type);
    var sr = marker.GetComponent<SpriteRenderer>();
    sr.sprite = sprite;
    sr.material = mats.fill;
    marker.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
    AddOutline(marker, sprite, mats.outline, outlineWidth);
    return marker;
}

private void AddOutline(GameObject parent, Sprite baseSprite, Material outlineMat, float outlineWidth)
{
    var outlineGO = new GameObject("Outline");
    outlineGO.transform.SetParent(parent.transform);
    outlineGO.transform.localPosition = Vector3.zero;
    var sr = outlineGO.AddComponent<SpriteRenderer>();
    sr.sprite = baseSprite;
    sr.material = outlineMat;
    sr.sortingLayerName = sortingLayerName;
    sr.sortingOrder = sortingOrder - 1;
    outlineGO.transform.localScale = Vector3.one * (1f + outlineWidth);
}

// --- Sprite Generation Helpers ---
// Generates a white mask sprite with alpha=1 for fill and outline pixels
private Sprite GenerateCircleSprite(float radius, int segments, float oWidth)
{
    int size = Mathf.CeilToInt((radius + oWidth) * 2 * 100);
    var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
    tex.filterMode = FilterMode.Bilinear;
    float center = size / 2f;
    for (int y = 0; y < size; y++)
    {
        for (int x = 0; x < size; x++)
        {
            float dx = (x - center) / 100f;
            float dy = (y - center) / 100f;
            float dist = Mathf.Sqrt(dx * dx + dy * dy);
            Color c = Color.clear;
            if (dist <= radius)
                c = Color.white;
            if (dist >= radius - oWidth && dist <= radius + oWidth)
                c = Color.white;
            tex.SetPixel(x, y, c);
        }
    }
    tex.Apply();
    return Sprite.Create(tex, new Rect(0, 0, size, size), Vector2.one * 0.5f, 100);
}

private Sprite GenerateDonutSprite(float inner, float outer, int segments, float oWidth)
{
    int size = Mathf.CeilToInt((outer + oWidth) * 2 * 100);
    var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
    tex.filterMode = FilterMode.Bilinear;
    float center = size / 2f;
    for (int y = 0; y < size; y++)
    {
        for (int x = 0; x < size; x++)
        {
            float dx = (x - center) / 100f;
            float dy = (y - center) / 100f;
            float dist = Mathf.Sqrt(dx * dx + dy * dy);
            Color c = Color.clear;
            if (dist >= inner && dist <= outer)
                c = Color.white;
            if ((dist >= inner - oWidth && dist <= inner + oWidth) || (dist >= outer - oWidth && dist <= outer + oWidth))
                c = Color.white;
            tex.SetPixel(x, y, c);
        }
    }
    tex.Apply();
    return Sprite.Create(tex, new Rect(0, 0, size, size), Vector2.one * 0.5f, 100);
}

private Sprite GenerateRectSprite(float width, float height, float oWidth)
{
    int w = Mathf.CeilToInt((width + oWidth * 2) * 100);
    int h = Mathf.CeilToInt((height + oWidth * 2) * 100);
    var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
    tex.filterMode = FilterMode.Bilinear;
    for (int y = 0; y < h; y++)
    {
        for (int x = 0; x < w; x++)
        {
            float dx = x / 100f - oWidth;
            float dy = y / 100f - oWidth;
            Color c = Color.clear;
            if (dx >= 0 && dx <= width && dy >= 0 && dy <= height)
                c = Color.white;
            if ((dx <= oWidth || dx >= width - oWidth || dy <= oWidth || dy >= height - oWidth) && c != Color.clear)
                c = Color.white;
            tex.SetPixel(x, y, c);
        }
    }
    tex.Apply();
    return Sprite.Create(tex, new Rect(0, 0, w, h), Vector2.zero, 100);
}

private Sprite GenerateConeSprite(float width, float length, int segments, float oWidth)
{
    int w = Mathf.CeilToInt((width + oWidth * 2) * 100);
    int h = Mathf.CeilToInt((length + oWidth * 2) * 100);
    var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
    tex.filterMode = FilterMode.Bilinear;
    float half = w / 2f;
    for (int y = 0; y < h; y++)
    {
        for (int x = 0; x < w; x++)
        {
            float dy = (y - oWidth * 100) / 100f;
            float span = (1 - dy / length) * (width / 2);
            float dx = (x - half) / 100f;
            Color c = Color.clear;
            if (dy >= 0 && dy <= length && Mathf.Abs(dx) <= span)
                c = Color.white;
            if (c != Color.clear && (Mathf.Abs(Mathf.Abs(dx) - span) < oWidth / 100f || Mathf.Abs(dy) < oWidth / 100f || Mathf.Abs(dy - length) < oWidth / 100f))
                c = Color.white;
            tex.SetPixel(x, y, c);
        }
    }
    tex.Apply();
    return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0), 100);
}


#region // References
private GameManager GetGameManager() => gameManager;
#endregion
}
*/