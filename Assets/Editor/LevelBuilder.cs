using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LevelBuilder
{
    static GameObject floorPrefab;
    static GameObject wallPrefab;
    static GameObject coverPrefab;
    static GameObject rampPrefab;
    static GameObject markerPrefab;
    static GameObject goalPrefab;
    static GameObject spawnPrefab;
    static GameObject landmarkPrefab;
    static GameObject playerPrefab;
    static GameObject coinPrefab;
    static GameObject bouncePadPrefab;
    static GameObject spikesPrefab;
    static GameObject guardPrefab;
    static GameObject crusherPrefab;
    static GameObject powerUpPrefab;
    static GameObject checkpointPrefab;
    static GameObject killZonePrefab;

    [MenuItem("CS464/Build All Levels")]
    public static void BuildAll()
    {
        Debug.Log("Starting Level Builder for CS464 Assignment 03...");

        MakeFolder("Assets", "Prefabs");
        MakeFolder("Assets", "Materials");
        MakeFolder("Assets", "Scenes");

        // Base structural prefabs
        floorPrefab = MakeBasicPrefab("Floor", PrimitiveType.Cube, new Color(0.55f, 0.55f, 0.55f));
        wallPrefab = MakeBasicPrefab("Wall", PrimitiveType.Cube, new Color(0.30f, 0.30f, 0.33f));
        coverPrefab = MakeBasicPrefab("Cover", PrimitiveType.Cube, new Color(0.65f, 0.50f, 0.35f));
        rampPrefab = MakeBasicPrefab("Ramp", PrimitiveType.Cube, new Color(0.45f, 0.55f, 0.65f));
        markerPrefab = MakeBasicPrefab("Marker", PrimitiveType.Cube, new Color(1.00f, 0.85f, 0.10f));
        landmarkPrefab = MakeBasicPrefab("Landmark", PrimitiveType.Cylinder, new Color(1.00f, 0.60f, 0.10f));
        spawnPrefab = MakeBasicPrefab("SpawnPad", PrimitiveType.Cylinder, new Color(0.20f, 0.50f, 1.00f));

        // Assignment 03 interactive and gameplay prefabs
        playerPrefab = MakePlayerPrefab();
        coinPrefab = MakeCoinPrefab();
        bouncePadPrefab = MakeBouncePadPrefab();
        spikesPrefab = MakeSpikesPrefab();
        guardPrefab = MakeGuardPrefab();
        crusherPrefab = MakeCrusherPrefab();
        powerUpPrefab = MakePowerUpPrefab();
        goalPrefab = MakeGoalPrefab();
        checkpointPrefab = MakeCheckpointPrefab();
        killZonePrefab = MakeKillZonePrefab();

        // Build all 5 levels
        BuildLevel1();
        BuildLevel2();
        BuildLevel3();
        BuildLevel4();
        BuildLevel5();

        // Set all five levels in EditorBuildSettings.scenes
        SetupBuildScenes();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("=== All five levels built and registered in Build Settings! ===");
    }

    static void MakeFolder(string parent, string name)
    {
        if (!AssetDatabase.IsValidFolder(parent + "/" + name))
        {
            AssetDatabase.CreateFolder(parent, name);
        }
    }

    static Material MakeMaterial(string name, Color color, float metallic = 0f, float smoothness = 0.5f)
    {
        string path = "Assets/Materials/" + name + ".mat";
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);

        if (existing != null)
        {
            existing.color = color;
            if (existing.HasProperty("_BaseColor")) existing.SetColor("_BaseColor", color);
            return existing;
        }

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        Material material = new Material(shader);
        material.color = color;
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
        if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smoothness);

        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    static GameObject MakeBasicPrefab(string name, PrimitiveType type, Color color)
    {
        GameObject temp = GameObject.CreatePrimitive(type);
        temp.name = name;
        temp.GetComponent<Renderer>().sharedMaterial = MakeMaterial(name, color);

        string path = "Assets/Prefabs/" + name + ".prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(temp, path);
        Object.DestroyImmediate(temp);
        return prefab;
    }

    static GameObject MakePlayerPrefab()
    {
        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        temp.name = "Player";
        temp.tag = "Player"; // Tagged Player (built-in tag requirement)
        temp.GetComponent<Renderer>().sharedMaterial = MakeMaterial("Player", new Color(0.90f, 0.15f, 0.15f));

        Rigidbody rb = temp.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0.05f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        temp.AddComponent<PlayerController>();
        temp.AddComponent<Health>();

        string path = "Assets/Prefabs/Player.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(temp, path);
        Object.DestroyImmediate(temp);
        return prefab;
    }

    static GameObject MakeCoinPrefab()
    {
        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        temp.name = "Coin";
        temp.transform.localScale = new Vector3(0.6f, 0.12f, 0.6f);
        temp.GetComponent<Renderer>().sharedMaterial = MakeMaterial("Coin", new Color(1.0f, 0.85f, 0.0f), 0.8f, 0.8f);

        Collider col = temp.GetComponent<Collider>();
        col.isTrigger = true;

        temp.AddComponent<Coin>();

        string path = "Assets/Prefabs/Coin.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(temp, path);
        Object.DestroyImmediate(temp);
        return prefab;
    }

    static GameObject MakeBouncePadPrefab()
    {
        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        temp.name = "BouncePad";
        temp.transform.localScale = new Vector3(2.0f, 0.25f, 2.0f);
        temp.GetComponent<Renderer>().sharedMaterial = MakeMaterial("BouncePad", new Color(0.1f, 1.0f, 0.35f), 0.2f, 0.7f);

        BoxCollider col = temp.GetComponent<BoxCollider>();
        col.isTrigger = true;

        temp.AddComponent<BouncePad>();

        string path = "Assets/Prefabs/BouncePad.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(temp, path);
        Object.DestroyImmediate(temp);
        return prefab;
    }

    static GameObject MakeSpikesPrefab()
    {
        GameObject root = new GameObject("Spikes");
        GameObject baseObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        baseObj.name = "SpikesVisual";
        baseObj.transform.SetParent(root.transform, false);
        baseObj.transform.localScale = new Vector3(2.0f, 0.35f, 2.0f);
        baseObj.GetComponent<Renderer>().sharedMaterial = MakeMaterial("Spikes", new Color(0.85f, 0.1f, 0.15f), 0.5f, 0.3f);
        Object.DestroyImmediate(baseObj.GetComponent<Collider>());

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.size = new Vector3(2.0f, 0.5f, 2.0f);
        col.center = new Vector3(0f, 0.25f, 0f);
        col.isTrigger = true; // No Rigidbody on spikes: player's Rigidbody is enough

        Hazard hazard = root.AddComponent<Hazard>();
        hazard.SetDamage(1);

        string path = "Assets/Prefabs/Spikes.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        return prefab;
    }

    static GameObject MakeGuardPrefab()
    {
        GameObject root = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        root.name = "Guard";
        root.transform.localScale = new Vector3(1.0f, 1.8f, 1.0f);
        root.GetComponent<Renderer>().sharedMaterial = MakeMaterial("Guard", new Color(0.95f, 0.35f, 0.1f));

        // Add a visor cube so front direction is clear
        GameObject visor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visor.name = "Visor";
        visor.transform.SetParent(root.transform, false);
        visor.transform.localPosition = new Vector3(0f, 0.4f, 0.4f);
        visor.transform.localScale = new Vector3(0.5f, 0.2f, 0.3f);
        visor.GetComponent<Renderer>().sharedMaterial = MakeMaterial("Visor", Color.cyan, 0.9f, 0.9f);
        Object.DestroyImmediate(visor.GetComponent<Collider>());

        // Guard requirements: Kinematic Rigidbody, trigger collider, Hazard.cs
        Rigidbody rb = root.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        CapsuleCollider col = root.GetComponent<CapsuleCollider>();
        col.isTrigger = true;

        Hazard hazard = root.AddComponent<Hazard>();
        hazard.SetDamage(1);

        root.AddComponent<EnemyPatrol>();

        string path = "Assets/Prefabs/Guard.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        return prefab;
    }

    static GameObject MakeCrusherPrefab()
    {
        GameObject root = new GameObject("Crusher");

        // Top Frame
        GameObject topFrame = GameObject.CreatePrimitive(PrimitiveType.Cube);
        topFrame.name = "TopFrame";
        topFrame.transform.SetParent(root.transform, false);
        topFrame.transform.localPosition = new Vector3(0f, 4.5f, 0f);
        topFrame.transform.localScale = new Vector3(3f, 0.4f, 3f);
        topFrame.GetComponent<Renderer>().sharedMaterial = MakeMaterial("CrusherFrame", new Color(0.25f, 0.25f, 0.3f));
        Object.DestroyImmediate(topFrame.GetComponent<Collider>());

        // Warning light/indicator on the frame
        GameObject lightObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        lightObj.name = "WarningIndicator";
        lightObj.transform.SetParent(root.transform, false);
        lightObj.transform.localPosition = new Vector3(0f, 4.8f, 0f);
        lightObj.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        Renderer indicatorRend = lightObj.GetComponent<Renderer>();
        indicatorRend.sharedMaterial = MakeMaterial("CrusherWarning", new Color(0.6f, 0.6f, 0.6f));
        Object.DestroyImmediate(lightObj.GetComponent<Collider>());

        // Crusher Slab that moves down
        GameObject slab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        slab.name = "CrusherSlab";
        slab.transform.SetParent(root.transform, false);
        slab.transform.localPosition = new Vector3(0f, 4.0f, 0f);
        slab.transform.localScale = new Vector3(2.6f, 0.8f, 2.6f);
        slab.GetComponent<Renderer>().sharedMaterial = MakeMaterial("CrusherSlab", new Color(0.4f, 0.4f, 0.45f));

        BoxCollider slabCol = slab.GetComponent<BoxCollider>();
        slabCol.isTrigger = true;
        slabCol.enabled = false; // Disabled while resting

        Hazard hazard = slab.AddComponent<Hazard>();
        hazard.SetDamage(2);

        // Crusher controller component
        Crusher crusher = root.AddComponent<Crusher>();
        SerializedObject so = new SerializedObject(crusher);
        so.FindProperty("crusherSlab").objectReferenceValue = slab;
        so.FindProperty("strikeCollider").objectReferenceValue = slabCol;
        so.FindProperty("warningIndicator").objectReferenceValue = indicatorRend;
        so.FindProperty("upPosition").vector3Value = new Vector3(0f, 4.0f, 0f);
        so.FindProperty("downPosition").vector3Value = new Vector3(0f, 0.4f, 0f);
        so.ApplyModifiedPropertiesWithoutUndo();

        string path = "Assets/Prefabs/Crusher.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        return prefab;
    }

    static GameObject MakePowerUpPrefab()
    {
        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        temp.name = "PowerUp";
        temp.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        temp.GetComponent<Renderer>().sharedMaterial = MakeMaterial("PowerUp", new Color(0.1f, 0.85f, 1.0f), 0.7f, 0.9f);

        BoxCollider col = temp.GetComponent<BoxCollider>();
        col.isTrigger = true;

        temp.AddComponent<PowerUp>();

        string path = "Assets/Prefabs/PowerUp.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(temp, path);
        Object.DestroyImmediate(temp);
        return prefab;
    }

    static GameObject MakeGoalPrefab()
    {
        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        temp.name = "Goal";
        temp.transform.localScale = new Vector3(3.0f, 0.15f, 3.0f);
        temp.GetComponent<Renderer>().sharedMaterial = MakeMaterial("Goal", new Color(0.10f, 0.90f, 0.30f));

        Collider col = temp.GetComponent<Collider>();
        col.isTrigger = true;

        temp.AddComponent<Goal>();

        string path = "Assets/Prefabs/Goal.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(temp, path);
        Object.DestroyImmediate(temp);
        return prefab;
    }

    static GameObject MakeCheckpointPrefab()
    {
        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        temp.name = "Checkpoint";
        temp.transform.localScale = new Vector3(2.5f, 0.1f, 2.5f);
        Renderer rend = temp.GetComponent<Renderer>();
        rend.sharedMaterial = MakeMaterial("Checkpoint", new Color(0.1f, 0.7f, 0.8f));

        Collider col = temp.GetComponent<Collider>();
        col.isTrigger = true;

        Checkpoint cp = temp.AddComponent<Checkpoint>();
        SerializedObject so = new SerializedObject(cp);
        so.FindProperty("checkpointRenderer").objectReferenceValue = rend;
        so.ApplyModifiedPropertiesWithoutUndo();

        string path = "Assets/Prefabs/Checkpoint.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(temp, path);
        Object.DestroyImmediate(temp);
        return prefab;
    }

    static GameObject MakeKillZonePrefab()
    {
        GameObject root = new GameObject("KillZone");
        BoxCollider col = root.AddComponent<BoxCollider>();
        col.size = new Vector3(200f, 2f, 200f);
        col.isTrigger = true;
        root.AddComponent<KillZone>();

        string path = "Assets/Prefabs/KillZone.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        return prefab;
    }

    static GameObject Place(GameObject prefab, string name, Vector3 position, Vector3 scale)
    {
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        obj.name = name;
        obj.transform.position = position;
        obj.transform.localScale = scale;
        return obj;
    }

    static void Floor(float x0, float x1, float z0, float z1, float top)
    {
        float width = x1 - x0;
        float depth = z1 - z0;
        float centerX = (x0 + x1) / 2f;
        float centerZ = (z0 + z1) / 2f;
        Place(floorPrefab, "Floor", new Vector3(centerX, top - 0.25f, centerZ), new Vector3(width, 0.5f, depth));
    }

    static void Wall(float x0, float x1, float z0, float z1, float baseY)
    {
        float width = x1 - x0;
        float depth = z1 - z0;
        float centerX = (x0 + x1) / 2f;
        float centerZ = (z0 + z1) / 2f;
        Place(wallPrefab, "Wall", new Vector3(centerX, baseY + 1.5f, centerZ), new Vector3(width, 3f, depth));
    }

    static void Cover(float x, float z, float width, float depth)
    {
        Place(coverPrefab, "LowCover", new Vector3(x, 0.55f, z), new Vector3(width, 1.1f, depth));
    }

    static void Ramp(float x, float width, float z0, float z1, float y0, float y1)
    {
        float run = z1 - z0;
        float rise = y1 - y0;
        float length = Mathf.Sqrt(run * run + rise * rise);
        float angle = Mathf.Atan2(rise, run);
        float centerY = (y0 + y1) / 2f - 0.15f * Mathf.Cos(angle);
        float centerZ = (z0 + z1) / 2f;

        GameObject ramp = Place(rampPrefab, "Ramp", new Vector3(x, centerY, centerZ), new Vector3(width, 0.3f, length));
        ramp.transform.rotation = Quaternion.Euler(-angle * Mathf.Rad2Deg, 0f, 0f);
    }

    static void Goal(float x, float z, float top)
    {
        Place(goalPrefab, "Goal", new Vector3(x, top + 0.15f, z), new Vector3(3f, 0.15f, 3f));
    }

    static void Landmark(float x, float z, float top)
    {
        Place(landmarkPrefab, "Landmark", new Vector3(x, top + 5f, z), new Vector3(1.5f, 5f, 1.5f));
    }

    static GameObject SpawnAndPlayer(float x, float z, float top)
    {
        GameObject pad = Place(spawnPrefab, "SpawnPad", new Vector3(x, top + 0.05f, z), new Vector3(3f, 0.05f, 3f));
        Place(playerPrefab, "Player", new Vector3(x, top + 1f, z), new Vector3(1f, 1f, 1f));
        return pad;
    }

    static void SpawnRoom(float doorHalf)
    {
        Floor(-5f, 5f, 0f, 10f, 0f);
        Wall(-5.5f, -5f, -0.5f, 10.5f, 0f);
        Wall(5f, 5.5f, -0.5f, 10.5f, 0f);
        Wall(-5f, 5f, -0.5f, 0f, 0f);
        Wall(-5f, -doorHalf, 10f, 10.5f, 0f);
        Wall(doorHalf, 5f, 10f, 10.5f, 0f);
    }

    static void AddCoin(float x, float y, float z)
    {
        Place(coinPrefab, "Coin", new Vector3(x, y + 0.6f, z), new Vector3(0.6f, 0.12f, 0.6f));
    }

    static void AddBouncePad(float x, float y, float z)
    {
        Place(bouncePadPrefab, "BouncePad", new Vector3(x, y + 0.12f, z), new Vector3(2.0f, 0.25f, 2.0f));
    }

    static void AddSpikes(float x, float y, float z)
    {
        Place(spikesPrefab, "Spikes", new Vector3(x, y, z), Vector3.one);
    }

    static void AddCrusher(float x, float y, float z)
    {
        Place(crusherPrefab, "Crusher", new Vector3(x, y, z), Vector3.one);
    }

    static void AddPowerUp(float x, float y, float z)
    {
        Place(powerUpPrefab, "SpeedPowerUp", new Vector3(x, y + 0.6f, z), new Vector3(0.7f, 0.7f, 0.7f));
    }

    static void AddCheckpoint(float x, float y, float z)
    {
        Place(checkpointPrefab, "Checkpoint", new Vector3(x, y + 0.05f, z), new Vector3(2.5f, 0.1f, 2.5f));
    }

    static void AddKillZone(float y = -5f)
    {
        Place(killZonePrefab, "KillZone", new Vector3(0f, y, 25f), Vector3.one);
    }

    static GameObject AddGuardWithWaypoints(Vector3[] waypointPositions, bool pingPong = false)
    {
        GameObject guard = Place(guardPrefab, "Guard", waypointPositions[0], Vector3.one);
        EnemyPatrol patrol = guard.GetComponent<EnemyPatrol>();

        GameObject wpContainer = new GameObject("GuardWaypoints");
        Transform[] wpTransforms = new Transform[waypointPositions.Length];

        for (int i = 0; i < waypointPositions.Length; i++)
        {
            GameObject wp = new GameObject($"WP_{i}");
            wp.transform.position = waypointPositions[i];
            wp.transform.SetParent(wpContainer.transform);
            wpTransforms[i] = wp.transform;
        }

        patrol.SetWaypoints(wpTransforms, pingPong);
        return guard;
    }

    static void SetupHUDAndGameManager(int targetScore, Transform spawnTransform)
    {
        // 1. Ensure EventSystem exists for UI button interaction
        if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // 2. Create Canvas
        GameObject canvasGO = new GameObject("HUD_Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        // 3. Create HUD Texts
        TMP_Text scoreText = CreateText(canvasGO.transform, "ScoreText", "Score: 0 / " + targetScore, 36, TextAlignmentOptions.TopLeft,
            new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(40f, -40f), new Vector2(400f, 60f), Color.yellow);

        TMP_Text healthText = CreateText(canvasGO.transform, "HealthText", "Hearts: 3 / 3", 36, TextAlignmentOptions.Top,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -40f), new Vector2(400f, 60f), Color.red);

        TMP_Text livesText = CreateText(canvasGO.transform, "LivesText", "Lives: 3", 36, TextAlignmentOptions.TopRight,
            new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-40f, -40f), new Vector2(400f, 60f), Color.cyan);

        // 4. Create Win Panel
        GameObject winPanel = CreatePanel(canvasGO.transform, "WinPanel", new Color(0f, 0f, 0f, 0.85f));
        CreateText(winPanel.transform, "WinTitle", "LEVEL COMPLETE!", 64, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.65f), new Vector2(0.5f, 0.65f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(800f, 100f), Color.green);
        Button winRestartBtn = CreateButton(winPanel.transform, "RestartButton", "RESTART LEVEL", new Vector2(0.5f, 0.4f), new Vector2(280f, 70f), new Color(0.2f, 0.7f, 0.2f));

        // 5. Create Lose Panel
        GameObject losePanel = CreatePanel(canvasGO.transform, "LosePanel", new Color(0f, 0f, 0f, 0.90f));
        CreateText(losePanel.transform, "LoseTitle", "GAME OVER", 64, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.65f), new Vector2(0.5f, 0.65f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(800f, 100f), Color.red);
        Button loseRestartBtn = CreateButton(losePanel.transform, "RestartButton", "TRY AGAIN", new Vector2(0.5f, 0.4f), new Vector2(280f, 70f), new Color(0.8f, 0.2f, 0.2f));

        winPanel.SetActive(false);
        losePanel.SetActive(false);

        // 6. Create GameManager Object
        GameObject gmGO = new GameObject("GameManager");
        GameManager gm = gmGO.AddComponent<GameManager>();

        SerializedObject so = new SerializedObject(gm);
        so.FindProperty("targetScore").intValue = targetScore;
        so.FindProperty("startingLives").intValue = 3;
        so.FindProperty("respawnPoint").objectReferenceValue = spawnTransform;
        so.FindProperty("scoreText").objectReferenceValue = scoreText;
        so.FindProperty("healthText").objectReferenceValue = healthText;
        so.FindProperty("livesText").objectReferenceValue = livesText;
        so.FindProperty("winPanel").objectReferenceValue = winPanel;
        so.FindProperty("losePanel").objectReferenceValue = losePanel;
        so.FindProperty("winRestartButton").objectReferenceValue = winRestartBtn;
        so.FindProperty("loseRestartButton").objectReferenceValue = loseRestartBtn;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static TMP_Text CreateText(Transform parent, string name, string text, float fontSize, TextAlignmentOptions align,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 pos, Vector2 size, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = align;
        tmp.color = color;
        tmp.raycastTarget = false;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return tmp;
    }

    static GameObject CreatePanel(Transform parent, string name, Color bgColor)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        Image img = panel.AddComponent<Image>();
        img.color = bgColor;

        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = Vector2.zero;
        return panel;
    }

    static Button CreateButton(Transform parent, string name, string label, Vector2 anchor, Vector2 size, Color btnColor)
    {
        GameObject btnGO = new GameObject(name);
        btnGO.transform.SetParent(parent, false);

        Image img = btnGO.AddComponent<Image>();
        img.color = btnColor;

        Button btn = btnGO.AddComponent<Button>();

        RectTransform rt = btnGO.GetComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = size;

        CreateText(btnGO.transform, "BtnText", label, 28, TextAlignmentOptions.Center,
            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, Color.white);

        return btn;
    }

    static void NewLevel()
    {
        EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
    }

    static void SetupCamera(float centerX, float centerZ, float size)
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = size;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.12f, 0.12f, 0.14f);
            cam.transform.position = new Vector3(centerX, 60f, centerZ);
            cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }

    static void SaveLevel(string name)
    {
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), "Assets/Scenes/" + name + ".unity");
    }

    static void SetupBuildScenes()
    {
        List<EditorBuildSettingsScene> sceneList = new List<EditorBuildSettingsScene>();
        string[] names = new string[] { "Level01", "Level02", "Level03", "Level04", "Level05" };
        foreach (string n in names)
        {
            string scenePath = $"Assets/Scenes/{n}.unity";
            sceneList.Add(new EditorBuildSettingsScene(scenePath, true));
        }
        EditorBuildSettings.scenes = sceneList.ToArray();
        Debug.Log("Build scenes configured with all 5 levels.");
    }

    // ==========================================
    // LEVEL 01: The Fundamentals
    // New: Coins, Goal, Bounce Pad
    // Combined with: Safe environment (nothing hurts yet)
    // ==========================================
    static void BuildLevel1()
    {
        NewLevel();

        SpawnRoom(1.5f);
        GameObject spawnPad = SpawnAndPlayer(0f, 3f, 0f);

        // Corridor
        Floor(-1.5f, 1.5f, 10f, 22f, 0f);
        Wall(-2f, -1.5f, 10f, 22f, 0f);
        Wall(1.5f, 2f, 10f, 22f, 0f);

        // Yard
        Floor(-8f, 8f, 25f, 48f, 0f);
        Wall(-8.5f, -8f, 25f, 48f, 0f);
        Wall(8f, 8.5f, 25f, 48f, 0f);
        Wall(-8.5f, 8.5f, 48f, 48.5f, 0f);

        Cover(-4f, 32f, 3f, 1f);
        Cover(4f, 36f, 3f, 1f);

        // Elevated balcony reached via the Bounce Pad!
        Floor(3f, 7f, 28f, 34f, 3f);
        Wall(7f, 7.5f, 28f, 34.5f, 3f);

        // Bounce Pad placed in front of balcony (launches player up with ForceMode.VelocityChange)
        AddBouncePad(5f, 0f, 26f);

        // 6 Coins placed across level (targetScore: 5)
        AddCoin(0f, 0f, 12f);
        AddCoin(0f, 0f, 18f);
        AddCoin(-4f, 0f, 28f);
        AddCoin(5f, 3f, 30f); // Bonus coin on elevated balcony!
        AddCoin(5f, 3f, 32f); // Bonus coin on elevated balcony!
        AddCoin(0f, 0f, 38f);

        Goal(0f, 44f, 0f);
        Landmark(0f, 47f, 0f);

        AddKillZone(-5f);
        SetupHUDAndGameManager(5, spawnPad.transform);

        SetupCamera(0f, 24f, 27f);
        SaveLevel("Level01");
    }

    // ==========================================
    // LEVEL 02: Mind the Spikes
    // New: Spikes Hazard, Hearts & Knockback
    // Combined with: Coin on a risky detour & pit gap
    // ==========================================
    static void BuildLevel2()
    {
        NewLevel();

        SpawnRoom(1.5f);
        GameObject spawnPad = SpawnAndPlayer(0f, 3f, 0f);

        Wall(-10f, -5f, 10f, 10.5f, 0f);
        Wall(5f, 10f, 10f, 10.5f, 0f);

        Floor(-10f, 10f, 10f, 30f, 0f);
        Wall(-10.5f, -10f, 10f, 30f, 0f);
        Wall(10f, 10.5f, 10f, 16f, 0f);
        Wall(10f, 10.5f, 24f, 30f, 0f);

        Wall(-1f, 1f, 14f, 26f, 0f);

        Cover(-6f, 18f, 3f, 1f);
        Cover(-4f, 23f, 3f, 1f);

        // Side room: Risky detour guarded by Spikes!
        Floor(10f, 17f, 16f, 24f, 0f);
        Wall(17f, 17.5f, 15.5f, 24.5f, 0f);
        Wall(10f, 17.5f, 15.5f, 16f, 0f);
        Wall(10f, 17.5f, 24f, 24.5f, 0f);

        // Spikes in side room detour
        AddSpikes(13.5f, 0f, 18f);
        AddSpikes(13.5f, 0f, 22f);

        // Coins in side room reward player for daring the spikes
        AddCoin(15f, 0f, 20f);
        AddCoin(13.5f, 0f, 20f);

        // Safe path markers and coins
        AddCoin(-5f, 0f, 14f);
        AddCoin(-5f, 0f, 22f);
        AddCoin(-5f, 0f, 28f);
        AddCoin(0f, 0f, 36f);

        // Gap / pitfall before goal yard
        Wall(-10f, -8f, 30f, 30.5f, 0f);
        Wall(8f, 10f, 30f, 30.5f, 0f);

        Floor(-8f, 8f, 33f, 46f, 0f); // 3m gap between 30f and 33f
        Wall(-8.5f, -8f, 33f, 46f, 0f);
        Wall(8f, 8.5f, 33f, 46f, 0f);
        Wall(-8.5f, 8.5f, 46f, 46.5f, 0f);

        Goal(0f, 42f, 0f);

        AddKillZone(-5f);
        SetupHUDAndGameManager(5, spawnPad.transform);

        SetupCamera(3f, 23f, 26f);
        SaveLevel("Level02");
    }

    // ==========================================
    // LEVEL 03: The Sentry Patrol
    // New: Enemy Patrol (Patrol, Chase, Return)
    // Combined with: Spikes beside its route & bridge
    // ==========================================
    static void BuildLevel3()
    {
        NewLevel();

        SpawnRoom(2f);
        GameObject spawnPad = SpawnAndPlayer(0f, 3f, 0f);

        Ramp(0f, 4f, 10f, 16f, 0f, 3f);

        Floor(-5f, 5f, 16f, 26f, 3f);
        // Bridge over the abyss
        Floor(-1.5f, 1.5f, 26f, 36f, 3f);
        // Patrol arena
        Floor(-8f, 8f, 36f, 50f, 3f);
        Wall(-8.5f, 8.5f, 50f, 50.5f, 3f);
        Wall(-8.5f, -8f, 36f, 50f, 3f);
        Wall(8f, 8.5f, 36f, 50f, 3f);

        // Spikes on either side of the guard's patrol route
        AddSpikes(-4f, 3f, 40f);
        AddSpikes(4f, 3f, 40f);
        AddSpikes(-4f, 3f, 46f);
        AddSpikes(4f, 3f, 46f);

        // Enemy patrol with 4 waypoints forming a loop
        Vector3[] waypoints = new Vector3[]
        {
            new Vector3(-2f, 3.9f, 40f),
            new Vector3(2f, 3.9f, 40f),
            new Vector3(2f, 3.9f, 46f),
            new Vector3(-2f, 3.9f, 46f)
        };
        AddGuardWithWaypoints(waypoints, false);

        // Coins across level (targetScore: 5)
        AddCoin(0f, 3f, 18f);
        AddCoin(0f, 3f, 22f);
        AddCoin(0f, 3f, 31f); // On the narrow bridge!
        AddCoin(-6f, 3f, 43f);
        AddCoin(6f, 3f, 43f);
        AddCoin(0f, 3f, 48f);

        Goal(0f, 48.5f, 3f);

        AddKillZone(-5f);
        SetupHUDAndGameManager(5, spawnPad.transform);

        SetupCamera(0f, 25f, 28f);
        SaveLevel("Level03");
    }

    // ==========================================
    // LEVEL 04: Under Pressure
    // New: The Crusher (>=0.5s warning, 2 damage strike)
    // Combined with: A patrolling guard & pinch corridor
    // ==========================================
    static void BuildLevel4()
    {
        NewLevel();

        SpawnRoom(1.0f);
        GameObject spawnPad = SpawnAndPlayer(0f, 3f, 0f);

        // Pinch corridor
        Floor(-1f, 1f, 10f, 24f, 0f);
        Wall(-1.5f, -1f, 10f, 24f, 0f);
        Wall(1f, 1.5f, 10f, 24f, 0f);

        // Crusher positioned in corridor choke point (warns for 0.8s before strike!)
        AddCrusher(0f, 0f, 17f);

        // Large Arena
        Floor(-15f, 15f, 24f, 52f, 0f);
        Wall(-15.5f, -15f, 24f, 52.5f, 0f);
        Wall(15f, 15.5f, 24f, 52.5f, 0f);
        Wall(-15f, 15f, 52f, 52.5f, 0f);
        Wall(-15f, -1f, 24f, 24.5f, 0f);
        Wall(1f, 15f, 24f, 24.5f, 0f);

        Cover(-6f, 30f, 4f, 1f);
        Cover(6f, 30f, 4f, 1f);
        Cover(0f, 35f, 6f, 1f);

        // Second crusher inside the arena
        AddCrusher(5f, 0f, 40f);

        // Guard patrolling around central cover
        Vector3[] waypoints = new Vector3[]
        {
            new Vector3(-6f, 0.9f, 36f),
            new Vector3(6f, 0.9f, 36f),
            new Vector3(0f, 0.9f, 44f)
        };
        AddGuardWithWaypoints(waypoints, true);

        // Spikes hazards in arena
        AddSpikes(-8f, 0f, 42f);

        // Coins (targetScore: 6)
        AddCoin(0f, 0f, 13f);
        AddCoin(0f, 0f, 21f); // Just past first crusher
        AddCoin(-8f, 0f, 32f);
        AddCoin(8f, 0f, 32f);
        AddCoin(5f, 0f, 40f); // Under second crusher (timing test!)
        AddCoin(-10f, 0f, 46f);
        AddCoin(0f, 0f, 48f);

        Goal(0f, 49f, 0f);
        Landmark(0f, 51.5f, 0f);

        AddKillZone(-5f);
        SetupHUDAndGameManager(6, spawnPad.transform);

        SetupCamera(0f, 26f, 28f);
        SaveLevel("Level04");
    }

    // ==========================================
    // LEVEL 05: Grand Gauntlet
    // New: Speed Power-Up (2 pickups within 5s - Refresh rule)
    // Combined with: Everything so far, ping-pong guard, crusher & bounce pad!
    // ==========================================
    static void BuildLevel5()
    {
        NewLevel();

        SpawnRoom(1.5f);
        GameObject spawnPad = SpawnAndPlayer(0f, 3f, 0f);

        // Section 1: Corridor with Speed Pickups
        Floor(-2f, 2f, 10f, 22f, 0f);
        Wall(-2.5f, -2f, 10f, 22f, 0f);
        Wall(2f, 2.5f, 10f, 22f, 0f);

        // Two speed pickups placed within 4s sprint to demonstrate REFRESH rule!
        AddPowerUp(0f, 0f, 12f);
        AddPowerUp(0f, 0f, 19f);

        // Section 2: Spike Trench crossed by Bounce Pad
        Floor(-4f, 4f, 22f, 26f, 0f);
        AddSpikes(-2f, 0f, 28f);
        AddSpikes(0f, 0f, 28f);
        AddSpikes(2f, 0f, 28f);
        AddBouncePad(0f, 0f, 24.5f); // Launches player over spikes!

        // Section 3: Mid-platform with Checkpoint
        Floor(-5f, 5f, 31f, 38f, 0f);
        AddCheckpoint(0f, 0f, 33f);

        // Crusher hazard right after checkpoint
        AddCrusher(0f, 0f, 36f);

        // Section 4: Raised narrow bridge with Ping-Pong Guard
        Ramp(0f, 3f, 38f, 42f, 0f, 2f);
        Floor(-1.5f, 1.5f, 42f, 54f, 2f);

        Vector3[] guardWaypoints = new Vector3[]
        {
            new Vector3(0f, 2.9f, 44f),
            new Vector3(0f, 2.9f, 52f)
        };
        AddGuardWithWaypoints(guardWaypoints, true); // Ping-pong guard!

        // Section 5: Final Goal Arena
        Floor(-8f, 8f, 54f, 66f, 2f);
        Wall(-8.5f, -8f, 54f, 66.5f, 2f);
        Wall(8f, 8.5f, 54f, 66.5f, 2f);
        Wall(-8.5f, 8.5f, 66f, 66.5f, 2f);

        // 8 Coins spread throughout (targetScore: 6)
        AddCoin(0f, 0f, 15f);
        AddCoin(0f, 0f, 24.5f);
        AddCoin(0f, 0f, 31.5f);
        AddCoin(-3f, 0f, 35f);
        AddCoin(3f, 0f, 35f);
        AddCoin(0f, 2f, 47f); // On bridge with guard
        AddCoin(-5f, 2f, 60f);
        AddCoin(5f, 2f, 60f);

        Goal(0f, 63f, 2f);

        AddKillZone(-6f);
        SetupHUDAndGameManager(6, spawnPad.transform);

        SetupCamera(0f, 33f, 35f);
        SaveLevel("Level05");
    }
}
