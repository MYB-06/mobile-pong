using UnityEngine;

namespace PongGame.Core
{
    public class GameBoundaries : MonoBehaviour
    {
        public static GameBoundaries Instance{get; private set;}

        private Camera _mainCamera;

        public float MinX {get; private set;}
        public float MaxX{get; private set;}
        public float MinY{get; private set;}
        public float MaxY{get; private set;}

        public float Width => MaxX - MinX;
        public float Height => MaxY - MinY;
        void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _mainCamera = Camera.main;

            if (_mainCamera == null) return;

            CalculateBoundaries();
        }
        private void CalculateBoundaries()
        {
            if (_mainCamera == null) return;

            float cameraHeight = _mainCamera.orthographicSize * 2f;
            float cameraWidth = cameraHeight * _mainCamera.aspect;
            
            MinX = -cameraWidth / 2f;
            MaxX = cameraWidth / 2f;
            MinY = -cameraHeight / 2f;
            MaxY = cameraHeight / 2f;
        }
    }
}
