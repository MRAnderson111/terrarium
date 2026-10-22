#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[InitializeOnLoad]
static class PlacementValidationTemp
{
    [Serializable] class Row { public string species, hit; public bool selected, blocked; public int stock; public Vector2 screen; }
    [Serializable] class Report
    {
        public bool playing;
        public string selection, inputModule, error;
        public int canvases, eventSystems, objectCountBefore, objectCountAfter;
        public bool pointerOverUI, groundHit;
        public Vector3 mouse;
        public Row[] buttons;
        public string[] spawned;
        public Vector3[] groundSamples;
    }

    static PlacementValidationTemp() => EditorApplication.update += Update;

    static void Update()
    {
        const string request = "Temp/CodexPlacement/request.txt";
        if (!File.Exists(request) || EditorApplication.isCompiling || EditorApplication.isUpdating)
            return;
        string command = File.ReadAllText(request).Trim();
        File.Delete(request);
        if (command == "reload" && !EditorApplication.isPlaying)
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/YoYoTest/TerrariumLevel 1.unity");
            return;
        }
        var report = new Report { playing = EditorApplication.isPlaying };
        try
        {
            var manager = UnityEngine.Object.FindObjectOfType<CreateManager>();
            var system = UnityEngine.Object.FindObjectOfType<EventSystem>();
            Canvas.ForceUpdateCanvases();
            report.selection = manager.selectPrefab != null ? manager.selectPrefab.name : "none";
            report.pointerOverUI = manager.pointerOverUI;
            report.groundHit = manager.isHit;
            report.mouse = Input.mousePosition;
            report.inputModule = system.currentInputModule != null ? system.currentInputModule.GetType().Name : "none";
            report.canvases = UnityEngine.Object.FindObjectsOfType<Canvas>().Length;
            report.eventSystems = UnityEngine.Object.FindObjectsOfType<EventSystem>().Length;
            report.objectCountBefore = UnityEngine.Object.FindObjectsOfType<GameObject>().Length;
            var rows = new List<Row>();
            foreach (var button in UnityEngine.Object.FindObjectsOfType<SelectButton>())
            {
                var rect = (RectTransform)button.transform;
                Vector2 screen = RectTransformUtility.WorldToScreenPoint(null, rect.TransformPoint(rect.rect.center));
                var pointer = new PointerEventData(system) { position = screen, button = PointerEventData.InputButton.Left };
                var hits = new List<RaycastResult>();
                system.RaycastAll(pointer, hits);
                var top = hits.Count > 0 ? hits[0].gameObject : null;
                var target = top != null ? ExecuteEvents.GetEventHandler<IPointerClickHandler>(top) : null;
                if (command == "test" && report.playing && target != null)
                    ExecuteEvents.Execute(target, pointer, ExecuteEvents.pointerClickHandler);
                rows.Add(new Row { species = button.selectPrefab.name, hit = top != null ? top.name : "none",
                    blocked = target != button.gameObject, selected = manager.selectButton == button,
                    screen = screen, stock = button.quantity });
            }
            report.buttons = rows.ToArray();
            if (command == "test") manager.ClearSelection();
            report.objectCountAfter = UnityEngine.Object.FindObjectsOfType<GameObject>().Length;
            report.spawned = UnityEngine.Object.FindObjectsOfType<GameObject>().Where(g => g.name.EndsWith("(Clone)")).Select(g => g.name).ToArray();
            var samples = new List<Vector3>();
            var camera = manager.placementCamera;
            for (int x = 4; x < 9; x++)
                for (int y = 2; y < 8; y++)
                {
                    Vector2 screen = new Vector2(Screen.width * x / 10f, Screen.height * y / 10f);
                    if (manager.TryGetGroundHit(camera.ScreenPointToRay(screen), out RaycastHit hit))
                        samples.Add(new Vector3(screen.x, screen.y, hit.distance));
                }
            report.groundSamples = samples.ToArray();
        }
        catch (Exception e) { report.error = e.ToString(); }
        File.WriteAllText("Temp/CodexPlacement/report.json", JsonUtility.ToJson(report, true));
    }
}
#endif
