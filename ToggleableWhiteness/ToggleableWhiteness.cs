using ICities;
using ToggleableWhiteness.Detours;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ToggleableWhiteness
{

    public class ToggleableWhiteness : LoadingExtensionBase, IUserMod
    {
        private static GameObject _monitor;

        public string Name => "ToggleableWhiteness";

        public string Description => "When plopping buildings, makes InfoView overlay displayed only if InfoView panel is visible";

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            if (mode != LoadMode.NewGame && mode != LoadMode.LoadGame && mode != LoadMode.NewGameFromScenario)
            {
                return;
            }
            ToolBaseDetour.Deploy();
            GameInfoViewsPanelDetour.Deploy();
            _monitor = new GameObject("ToggleableWhiteness");
            _monitor.AddComponent<ToolMonitor>();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            ToolBaseDetour.Revert();
            GameInfoViewsPanelDetour.Revert();
            if (_monitor != null)
            {
                Object.Destroy(_monitor);
            }
        }
    }


}
