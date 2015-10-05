using ColossalFramework.UI;
using UnityEngine;

namespace ToggleableWhiteness
{
    public static class Util
    {
        public static bool IsInfoViewsPanelVisible()
        {
            var gameObject = GameObject.Find("InfoViewsPanel");
            if (gameObject != null)
            {
                var uiPanel = gameObject.GetComponent<UIPanel>();
                if (uiPanel != null && !uiPanel.isVisible)
                {
                    return false;
                }
            }
            return true;
        }
    }
}