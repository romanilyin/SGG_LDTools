using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;
using com.sungeargames.ldtools_editor;

namespace com.sungeargames.ldtools_editor
{
    // Класс для кнопки открытия Drop Objects Tool
    [EditorToolbarElement(id, typeof(SceneView))]
    internal class DropObjectsButton : EditorToolbarButton
    {
        public const string id = "LDTools/DropObjectsButton";

        public DropObjectsButton()
        {
            // Используем стандартную иконку Unity
            icon = EditorGUIUtility.IconContent("d_Animation.FilterBySelection").image as Texture2D;
            tooltip = "Drop Objects Tool";
            clicked += OnClick;
            
            // Скрываем текст, оставляем только иконку
            var textIcon = this.Q<VisualElement>("EditorToolbarButtonTextIcon");
            if (textIcon != null)
                textIcon.style.fontSize = 0;
        }

        void OnClick()
        {
            // Открываем окно Drop Objects
            DropObjectsWindow.ShowWindow();
        }
    }

    // Панель инструментов для вашего пакета
    [Overlay(typeof(SceneView), "SGG LD Tools", defaultDisplay = true)]
    [Icon("Packages/com.sungeargames.ldtools_editor/Editor/LDTTools_Icon.png")] // Путь к иконке в пакете
    public class SGGToolsPanel : ToolbarOverlay
    {
        public SGGToolsPanel() : base(DropObjectsButton.id) { }

        public override VisualElement CreatePanelContent()
        {
            VisualElement group = new OverlayToolbar();
            
            // Добавляем кнопку Drop Objects
            group.Add(new DropObjectsButton());
            
            return group;
        }
    }
}