using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

namespace Wgs.FlipSide
{
    [EditorTool("TriggerZone")]
    public class TriggerDesigner : EditorTool
    {
        [SerializeField] private Texture2D _icon;

        private GUIContent _content;
        public override GUIContent toolbarIcon => _content;
        
        private void OnEnable()
        {
            _content = new GUIContent
            {
                image = _icon,
                text = "TriggerZone",
                tooltip = ""
            };
        }

        public override void OnActivated()
        {
            Selection.objects = null;
        }

        private Vector3 start;
        private bool isDown;
        public override void OnToolGUI(EditorWindow window)
        {
            //If we're not in the scene view, exit.
            if (!(window is SceneView))
                return;

            //If we're not the active tool, exit.
            if (!ToolManager.IsActiveTool(this))
                return;

            ClearSelection();

            var position = Event.current.mousePosition;
            var isPlaceValid = HandleUtility.PlaceObject(position, out var point, out var normal);

            if (!isPlaceValid) point = HandleUtility.GUIPointToWorldRay(position).GetPoint(10);
            
            Handles.DrawWireDisc(point, Vector3.up, 0.25f);

            if (Event.current.button == 0)
            {
                switch (Event.current.type)
                {
                    //Store start position
                    case EventType.MouseDown:
                        start = point;
                        isDown = true;
                        break;
                    case EventType.MouseUp:
                        isDown = false;
                        break;
                }

                if (isDown)
                {
                    Handles.DrawLine(start, point);
                }
            }

            window.Repaint();
        }

        private void ClearSelection()
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
    }
}
