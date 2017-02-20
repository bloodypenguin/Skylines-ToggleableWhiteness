using ColossalFramework.UI;
using UnityEngine;

namespace ToggleableWhiteness
{
    public static class Util
    {
        public static bool IsInfoViewsPanelVisible()
        {
            var gameObject = GameObject.Find("InfoViewsPanel");
            var uiPanel = gameObject?.GetComponent<UIPanel>();
            return uiPanel == null || uiPanel.isVisible;
        }
    }
}