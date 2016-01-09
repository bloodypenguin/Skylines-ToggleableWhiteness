using System.Reflection;
using ToggleableWhiteness.Detours;
using UnityEngine;
using ColossalFramework;

namespace ToggleableWhiteness
{
    public class ToolMonitor : MonoBehaviour
    {
        private static ToolBase _previousTool;
        private static readonly MethodInfo ForceModeMethod = typeof(ToolBase).GetMethod("ForceInfoMode",
                BindingFlags.Instance | BindingFlags.NonPublic);

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
                ForceModeMethod.Invoke(currentTool,
                    new object[] {InfoManager.InfoMode.NaturalResources, InfoManager.SubInfoMode.Default});
            }
            else if (currentTool is DistrictTool)
            {
                ForceModeMethod.Invoke(currentTool,
                    new object[] { InfoManager.InfoMode.Districts, InfoManager.SubInfoMode.Default });
            }
            else if (currentTool is TransportTool)
            {
                Singleton<TransportManager>.instance.LinesVisible = true;
                Singleton<TransportManager>.instance.TunnelsVisible = true;
            }
            else
            {
                var nextInfoMode = Singleton<InfoManager>.instance.NextMode;
                Singleton<TransportManager>.instance.LinesVisible = (nextInfoMode == InfoManager.InfoMode.Transport);
                Singleton<TransportManager>.instance.TunnelsVisible =
                    (nextInfoMode == InfoManager.InfoMode.Transport || nextInfoMode == InfoManager.InfoMode.Traffic);
            }    
        }

        public void Destroy()
        {
            _previousTool = null;
        }
    }
}