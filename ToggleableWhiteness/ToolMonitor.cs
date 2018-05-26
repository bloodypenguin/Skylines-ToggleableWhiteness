using System.Reflection;
using System.Runtime.CompilerServices;
using ToggleableWhiteness.Detours;
using UnityEngine;
using ColossalFramework;

namespace ToggleableWhiteness
{
    public class ToolMonitor : MonoBehaviour
    {
        private static ToolBase _previousTool;

        public void Awake()
        {
            _previousTool = null;
        }

        public void Update()
        {
            var currentTool = ToolsModifierControl.GetCurrentTool<ToolBase>();
            if (currentTool != _previousTool)
            {
                ToolBaseDetour.ResetForceMode();
            }
            _previousTool = currentTool;
            if (currentTool is ResourceTool || currentTool is TreeTool)
            {
                ToolBaseDetour.ForceInfoMode(InfoManager.InfoMode.NaturalResources, InfoManager.SubInfoMode.Default);
            }
            else if (currentTool is DistrictTool)
            {
                ToolBaseDetour.ForceInfoMode(InfoManager.InfoMode.Districts, InfoManager.SubInfoMode.Default);
            }
            else if (currentTool is TransportTool)
            {
                TransportManager.instance.LinesVisible = ToolsModifierControl.GetCurrentTool<TransportTool>().m_prefab?.m_class?.m_service == ItemClass.Service.Disaster ? 128 : -129;
                TransportManager.instance.TunnelsVisible = true;
            }
            else if (currentTool is TerrainTool || currentTool.GetType().Name == "InGameTerrainTool")
            {
                ToolBaseDetour.ForceInfoMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
            }
            else
            {
                var nextInfoMode = InfoManager.instance.NextMode;
                TransportManager.instance.LinesVisible = (nextInfoMode == InfoManager.InfoMode.Transport) ? -129 : (nextInfoMode == InfoManager.InfoMode.EscapeRoutes ? 128 : 0);
                TransportManager.instance.TunnelsVisible =
                    (nextInfoMode == InfoManager.InfoMode.Transport || nextInfoMode == InfoManager.InfoMode.Traffic);
            }

        }

        public void Destroy()
        {
            _previousTool = null;
        }
    }
}