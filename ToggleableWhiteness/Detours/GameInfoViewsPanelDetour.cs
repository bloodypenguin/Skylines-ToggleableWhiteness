using System.Reflection;
using ColossalFramework;

namespace ToggleableWhiteness.Detours
{
    public class GameInfoViewsPanelDetour : GameInfoViewsPanel
    {
        private static readonly PositionData<InfoManager.InfoMode>[] kResources = Utils.GetOrderedEnumData<InfoManager.InfoMode>("Game");
        private static RedirectCallsState _state;
        private static bool _deployed;
        private static readonly MethodInfo MethodInfo = typeof(GameInfoViewsPanel).GetMethod("OnButtonClicked", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo DetourInfo = typeof(GameInfoViewsPanelDetour).GetMethod("OnButtonClicked", BindingFlags.Instance | BindingFlags.NonPublic);

        public static void Deploy()
        {
            if (_deployed)
            {
                return;
            }
            _state = RedirectionHelper.RedirectCalls(MethodInfo, DetourInfo);
            _deployed = true;
        }

        public static void Revert()
        {
            if (!_deployed)
            {
                return;
            }
            RedirectionHelper.RevertRedirect(MethodInfo, _state);
            _deployed = false;
        }

        protected override void OnButtonClicked(int index)
        {
            if (index >= 0 && index < kResources.Length)
            {
                var currentTool = ToolsModifierControl.GetCurrentTool<ToolBase>();
                if (currentTool != null && (currentTool is NetTool || currentTool is BuildingTool || currentTool is DistrictTool
                    || currentTool is TreeTool || currentTool is ResourceTool))
                {
                    ToolBaseDetour.ForceMode(kResources[index].enumValue, InfoManager.SubInfoMode.Default);
                }
                else
                { 
                    ToolBaseDetour.ResetForceMode();
                    this.CloseToolbar();
                    WorldInfoPanel.HideAllWorldInfoPanels();
                    if (!Singleton<InfoManager>.exists)
                        return;
                    Singleton<InfoManager>.instance.SetCurrentMode(kResources[index].enumValue, InfoManager.SubInfoMode.Default);
                    Singleton<GuideManager>.instance.InfoViewUsed();
                }

            }
            else
            {
                ToolBaseDetour.ResetForceMode();
                this.CloseToolbar();
                if (!Singleton<InfoManager>.exists)
                    return;
                Singleton<InfoManager>.instance.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
                Singleton<GuideManager>.instance.InfoViewUsed();
            }

        }
    }
}