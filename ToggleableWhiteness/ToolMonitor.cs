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
                var linesVisible = -35969;
                switch (ToolsModifierControl.GetCurrentTool<TransportTool>().m_prefab?.m_class?.m_service)
                {
                    case ItemClass.Service.Disaster:
                        linesVisible = 128;
                        break;
                    case ItemClass.Service.Tourism:
                        linesVisible = 3072;
                        break;
                }
                TransportManager.instance.LinesVisible =  linesVisible;
                TransportManager.instance.TunnelsVisible = true;
            }
            else if (currentTool is TerrainTool || currentTool.GetType().Name == "InGameTerrainTool")
            {
                ToolBaseDetour.ForceInfoMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
            }
            else
            {
                var nextInfoMode = InfoManager.instance.NextMode;
                var linesVisible = 0;
                switch (nextInfoMode)
                {
                    case InfoManager.InfoMode.Transport: 
                        linesVisible =  -35969;
                        break;
                    case InfoManager.InfoMode.EscapeRoutes:
                        linesVisible =  128;
                        break;
                    case InfoManager.InfoMode.Fishing:
                        linesVisible = 32768;
                        break;
                    case InfoManager.InfoMode.Tours: 
                        linesVisible = 3072;
                        break;
                }
                if ((currentTool as NetTool)?.m_prefab?.m_netAI is FishingPathAI)
                {
                    linesVisible = 32768; 
                }

                TransportManager.instance.LinesVisible = linesVisible;
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