using PongGame.Core;
using UnityEngine;

namespace PongGame.Gameplay
{
    public enum GoalOwner {Player, AI}
    public class GoalZone : MonoBehaviour
    {
        [Header("Goal Settings")]
        [SerializeField] private GoalOwner scorer;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent<Ball>(out Ball ball))
            {
                HandleGoal();
            }
        }
        private void HandleGoal()
        {
            if (scorer == GoalOwner.Player)
            {
                GameManager.Instance.OnPlayerScored();
            }
            else
            {
                GameManager.Instance.OnAIScored();
            }
        }
    }
}