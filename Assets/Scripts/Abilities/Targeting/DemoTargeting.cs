using System.Collections;
using RPG.Control;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Demo Targeting", menuName = "Abilities/Targeting/Demo", order = 0)]
    public class DemoTargeting : TargetingStrategy
    {
        [SerializeField] private Texture2D cursorTexture;
        [SerializeField] private Vector2 cursorHotspot;

        public override void StartTargeting(GameObject user)
        {
            var playerController = user.GetComponent<PlayerController>();
            playerController.StartCoroutine(Targeting(user, playerController));
        }

        private IEnumerator Targeting(GameObject user, PlayerController playerController)
        {
            playerController.enabled = false;
            
            while (true)
            {
                Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
                yield return null;
            }
        }
    }
}