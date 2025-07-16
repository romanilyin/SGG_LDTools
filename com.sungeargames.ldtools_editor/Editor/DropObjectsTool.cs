using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace com.sungeargames.ldtools_editor
{
    [InitializeOnLoad]
    public static class DropObjectsTool
    {
        static DropObjectsTool()
        {
            // Подписка на горячую клавишу 
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            // Обработка горячей клавиши 'D'
/*
            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.D && !e.control && !e.shift && !e.alt)
            {
                DropObjectsWindow.ShowWindow();
                e.Use();
            }
*/
        }

        public static void DropObjects(DropObjectsWindow.DropMode mode)
        {
            if (Selection.gameObjects.Length == 0) return;

            // Собираем все коллайдеры выбранных объектов для игнорирования
            HashSet<Collider> ignoreColliders = new HashSet<Collider>();
            foreach (GameObject go in Selection.gameObjects)
            {
                ignoreColliders.UnionWith(go.GetComponentsInChildren<Collider>());
            }

            // Запоминаем позиции для Undo
            List<Transform> transforms = new List<Transform>();
            List<Vector3> originalPositions = new List<Vector3>();

            foreach (GameObject go in Selection.gameObjects)
            {
                transforms.Add(go.transform);
                originalPositions.Add(go.transform.position);
            }

            Undo.RecordObjects(transforms.ToArray(), "Drop Objects");

            // Опускаем объекты
            int notDroppedCount = 0;
            foreach (GameObject go in Selection.gameObjects)
            {
                if (!DropSingleObject(go, mode, ignoreColliders))
                {
                    notDroppedCount++;
                }
            }

            if (notDroppedCount > 0)
            {
                Debug.LogWarning($"[Drop Tool] {notDroppedCount} objects not dropped! No collision found below.");
            }
        }

        private static bool DropSingleObject(GameObject go, DropObjectsWindow.DropMode mode, HashSet<Collider> ignoreColliders)
        {
            Bounds bounds = CalculateWorldBounds(go);
            List<Vector3> rayPoints = new List<Vector3>();

            switch (mode)
            {
                case DropObjectsWindow.DropMode.Bottom:
                    rayPoints.Add(new Vector3(bounds.center.x, bounds.min.y, bounds.center.z));
                    rayPoints.Add(new Vector3(bounds.min.x, bounds.min.y, bounds.min.z));
                    rayPoints.Add(new Vector3(bounds.min.x, bounds.min.y, bounds.max.z));
                    rayPoints.Add(new Vector3(bounds.max.x, bounds.min.y, bounds.min.z));
                    rayPoints.Add(new Vector3(bounds.max.x, bounds.min.y, bounds.max.z));
                    break;
                
                case DropObjectsWindow.DropMode.Center:
                    rayPoints.Add(bounds.center);
                    rayPoints.Add(new Vector3(bounds.min.x, bounds.center.y, bounds.min.z));
                    rayPoints.Add(new Vector3(bounds.min.x, bounds.center.y, bounds.max.z));
                    rayPoints.Add(new Vector3(bounds.max.x, bounds.center.y, bounds.min.z));
                    rayPoints.Add(new Vector3(bounds.max.x, bounds.center.y, bounds.max.z));
                    break;
                
                case DropObjectsWindow.DropMode.Pivot:
                    rayPoints.Add(go.transform.position);
                    break;
            }

            float maxSurfaceY = float.NegativeInfinity;
            bool hitFound = false;

            foreach (Vector3 point in rayPoints)
            {
                if (FindHighestSurface(point, ignoreColliders, out float surfaceY))
                {
                    hitFound = true;
                    if (surfaceY > maxSurfaceY) maxSurfaceY = surfaceY;
                }
            }

            if (!hitFound) return false;

            Vector3 position = go.transform.position;
            switch (mode)
            {
                case DropObjectsWindow.DropMode.Bottom:
                    position.y += maxSurfaceY - bounds.min.y;
                    break;
                case DropObjectsWindow.DropMode.Center:
                    position.y += maxSurfaceY - bounds.center.y;
                    break;
                case DropObjectsWindow.DropMode.Pivot:
                    position.y = maxSurfaceY;
                    break;
            }

            go.transform.position = position;
            return true;
        }

        private static bool FindHighestSurface(Vector3 origin, HashSet<Collider> ignoreColliders, out float surfaceY)
        {
            surfaceY = float.NegativeInfinity;
            Ray ray = new Ray(origin + Vector3.up * 0.1f, Vector3.down);
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

            if (hits.Length == 0) return false;

            float highestY = float.NegativeInfinity;
            bool validHit = false;

            foreach (RaycastHit hit in hits)
            {
                if (ignoreColliders.Contains(hit.collider)) continue;
                if (hit.point.y > highestY)
                {
                    highestY = hit.point.y;
                    validHit = true;
                }
            }

            surfaceY = highestY;
            return validHit;
        }

        private static Bounds CalculateWorldBounds(GameObject go)
        {
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return new Bounds(go.transform.position, Vector3.zero);

            Bounds bounds = renderers[0].bounds;
            foreach (Renderer r in renderers)
            {
                bounds.Encapsulate(r.bounds);
            }
            return bounds;
        }
    }

    public class DropObjectsWindow : EditorWindow
    {
        public enum DropMode { Bottom, Center, Pivot }
        private DropMode selectedMode = DropMode.Bottom;

        [MenuItem("Window/LD Tools/Drop Objects")]
        public static void ShowWindow()
        {
            var window = GetWindow<DropObjectsWindow>("Drop Objects");
            window.minSize = new Vector2(300, 150);
            window.Show();
        }

        void OnEnable()
        {
            // Подписываемся на событие изменения выбора
            Selection.selectionChanged += Repaint;
        }

        void OnDisable()
        {
            // Отписываемся при закрытии окна
            Selection.selectionChanged -= Repaint;
        }

        void OnGUI()
        {
            EditorGUILayout.Space(10);
            GUILayout.Label("SELECTED OBJECTS", EditorStyles.boldLabel);
            
            int selectedCount = Selection.gameObjects.Length;
            int nestedCount = Selection.gameObjects.Count(go => go.transform.childCount > 0);
            
            EditorGUILayout.HelpBox(
                $"Selected: {selectedCount} game objects\n" +
                $"With children: {nestedCount} objects",
                MessageType.Info);
            
            EditorGUILayout.Space(15);
            GUILayout.Label("DROP SETTINGS", EditorStyles.boldLabel);
            
            EditorGUI.BeginChangeCheck();
            selectedMode = (DropMode)EditorGUILayout.EnumPopup("Drop Mode:", selectedMode);
            
            EditorGUILayout.Space(20);
            if (GUILayout.Button("Drop Objects", GUILayout.Height(30)))
            {
                DropObjectsTool.DropObjects(selectedMode);
            }

            EditorGUILayout.Space(10);
            if (GUILayout.Button("Drop Objects (Pivot Mode)"))
            {
                DropObjectsTool.DropObjects(DropMode.Pivot);
            }

            EditorGUILayout.HelpBox(
                //"Shortcut: Use hotkey 'D' in scene view\n" +
                "Ctrl+Z supported for all operations",
                MessageType.None);
        }
    }
}