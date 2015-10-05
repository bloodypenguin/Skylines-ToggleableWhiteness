using System.Reflection;
using ToggleableWhiteness.Detours;
using UnityEngine;

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

        }

        public void Destroy()
        {
            _previousTool = null;
        }
    }
}