using System.Reflection;
using ColossalFramework;

namespace ToggleableWhiteness.Detours
{
    public class ToolBaseDetour : ToolBase
    {
        private static readonly FieldInfo Field = typeof(ToolBase).GetField("m_forcedInfoMode", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo Original = typeof(ToolBase).GetMethod("ForceInfoMode",
                BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo Detour = typeof(ToolBaseDetour).GetMethod("ForceInfoMode",
                BindingFlags.Instance | BindingFlags.NonPublic);

        private static RedirectCallsState _state;
        private static bool _deployed;


        private static bool _forceMode;
        private static InfoManager.InfoMode _forcedMode;
        private static InfoManager.SubInfoMode _forcesSubMode;

        public static void Deploy()
        {
            if (_deployed)
            {
                return;
            }
            _state = RedirectionHelper.RedirectCalls(Original, Detour);
            _deployed = true;

        }

        public static void Revert()
        {
            if (!_deployed)
            {
                return;
            }
            RedirectionHelper.RevertRedirect(Original, _state);
            _deployed = false;
        }

        public static void ForceMode(InfoManager.InfoMode mode, InfoManager.SubInfoMode subMode)
        {
            _forceMode = true;
            _forcedMode = mode;
            _forcesSubMode = subMode;
        }

        public static void ResetForceMode()
        {
            _forceMode = false;
        }



        protected new void ForceInfoMode(InfoManager.InfoMode mode, InfoManager.SubInfoMode subMode)
        {
            var currentTool = ToolsModifierControl.GetCurrentTool<ToolBase>();
            switch (currentTool.GetType().Name)
            {
             
                case "BuildingTool":
                case "TreeTool":
                case "ResourceTool":
                case "DistrictTool":
                case "TransportTool":
                case "CustomTransportTool":
                    if (!Util.IsInfoViewsPanelVisible())
                    {
                        mode = InfoManager.InfoMode.None;
                        subMode = InfoManager.SubInfoMode.Default;
                    }
                    else
                    {
                        ApplyForcedMode(ref mode, ref  subMode);
                    }
                    break;
                case "NetTool":
                case "NetToolFine":
                    if (Util.IsInfoViewsPanelVisible())
                    {
                        ApplyForcedMode(ref mode, ref subMode);
                        if (mode == InfoManager.InfoMode.None)
                        {
                            mode = InfoManager.InfoMode.Traffic;
                            subMode = InfoManager.SubInfoMode.Default;
                        }
                    }
                    else
                    {
                        mode = InfoManager.InfoMode.None;
                        subMode = InfoManager.SubInfoMode.Default;
                    }
                    break;
                default:
                    break;
            }

            if (mode == InfoManager.InfoMode.None &&
                (InfoManager.InfoMode)Field.GetValue(this) == InfoManager.InfoMode.None)
            {
                return;
            }

            Field.SetValue(this, mode);
            Singleton<InfoManager>.instance.SetCurrentMode(mode, subMode);
        }

        private static void ApplyForcedMode(ref InfoManager.InfoMode mode, ref InfoManager.SubInfoMode subMode)
        {
            if (!_forceMode) return;
            mode = _forcedMode;
            subMode = _forcesSubMode;
        }
    }
}