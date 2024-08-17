using UnityEngine;

namespace Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.TagManagement
{
    class GameTag : MonoBehaviour
    {
        public string gameTag;

        public bool CompareGameTag(string tag)
        {
            return this.gameTag == tag;
        }
    }
}
