using UnityEngine;

namespace PongGame.Core
{
    public class WallManager : MonoBehaviour
    {
        [Header("Wall References")]
        [SerializeField] private GameObject leftWall;
        [SerializeField] private GameObject rightWall;

        [Header("Wall Settings")]
        [SerializeField] private float wallThickness = 0.5f;
        private void Start()
        {
            if (GameBoundaries.Instance == null)
            {
                Debug.LogError("[WallManager] GameBoundaries instance not found!");
                return;
            }

            SetupWalls();
        }

        private void SetupWalls()
        {
            float minX = GameBoundaries.Instance.MinX;
            float maxX = GameBoundaries.Instance.MaxX;
            float height = GameBoundaries.Instance.Height;

            if(leftWall != null)
            {
                leftWall.transform.position = new Vector3(minX - wallThickness / 2f, 0f, 0f);
                SetupBoxCollider(leftWall,wallThickness,height);
            }
            if(rightWall != null)
            {
                rightWall.transform.position = new Vector3(maxX + wallThickness / 2f, 0f, 0f);
                SetupBoxCollider(rightWall, wallThickness, height);
            }
        }
        private void SetupBoxCollider(GameObject wall, float width, float height)
        {
            BoxCollider2D collider = wall.GetComponent<BoxCollider2D>();
            
            if (collider == null)
            {
                collider = wall.AddComponent<BoxCollider2D>();
            }

            collider.size = new Vector2(width, height);
        }
    }
}
