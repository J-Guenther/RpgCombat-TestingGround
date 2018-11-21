using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using RPG.Characters; // So we can detect by type

namespace RPG.CameraUI
{
    public class CameraRaycaster : MonoBehaviour 
    {
        [SerializeField] private Texture2D walkCursor = null;
        [SerializeField] private Texture2D enemyCursor = null;
        [SerializeField] private Vector2 cursorHotspot = new Vector2(0, 0);

        const int POTENTIALLY_WALKABLE_LAYER = 8;
        float maxRaycastDepth = 100f; // Hard coded value

        Rect currentScreenRect; 

        // Setup delegates for broadcasting layer changes to other classes
        public delegate void OnMouseOverTerrain(Vector3 destination);
        public event OnMouseOverTerrain onMouseOverPotentiallyWalkable;

        public delegate void OnMouseOverEnemy(EnemyAI enemy);
        public event OnMouseOverEnemy onMouseOverEnemy;

        void Update()
        {
            currentScreenRect = new Rect(0, 0, Screen.width, Screen.height);
            // Check if pointer is over an interactable UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Implement UI interaction
            } else
            {
                PerformRaycasts();
            }
        }

        void PerformRaycasts()
        {
            if (currentScreenRect.Contains(Input.mousePosition))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // Specify layer priorities below, order matters
                if (RaycastForEnemy(ray)) { return; }
                if (RaycastForWalkable(ray)) { return; }
            }
        }

        private bool RaycastForEnemy(Ray ray)
        {
            RaycastHit hitInfo;
            Physics.Raycast(ray, out hitInfo, maxRaycastDepth);
            var gameObjectHit = hitInfo.collider.gameObject;
            var enemyHit = gameObjectHit.GetComponent<EnemyAI>();
            if (enemyHit)
            {
                Cursor.SetCursor(enemyCursor, cursorHotspot, CursorMode.Auto);
                onMouseOverEnemy(enemyHit);
                return true;
            }
            return false;
        }

        private bool RaycastForWalkable(Ray ray)
        {
            RaycastHit hitInfo;
            LayerMask potentiallyWalkableLayer = 1 << POTENTIALLY_WALKABLE_LAYER;
            bool potentiallyWalkableHit = Physics.Raycast(ray, out hitInfo, maxRaycastDepth, potentiallyWalkableLayer);
            if (potentiallyWalkableHit)
            {
                Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
                onMouseOverPotentiallyWalkable(hitInfo.point);
                return true;
            }
            return false;
        }
    }
}
