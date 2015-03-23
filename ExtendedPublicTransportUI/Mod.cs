using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace EPTUI
{
    public class Mod : IUserMod
    {
        public string Name { get { return "Extended Public Transport UI"; } }
        public string Description { get { return "Provides additional list views and toggles for public transport"; } }
    }

    public class TransportObserver : ThreadingExtensionBase
    {
        private int _lineCount;

        public delegate void LineCountChangedHandler(int count);
        public static event LineCountChangedHandler LineCountChanged;

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            var handlers = LineCountChanged;
            if (handlers == null)
                return;

            var newCount = TransportUtil.GetUsedTransportLineIndices().Count;
            if (newCount != _lineCount)
            {
                handlers(newCount);
                _lineCount = newCount;
            }
        }

        public override void OnReleased()
        {
            LineCountChanged = null;
        }

        public static void ClearSubscribers()
        {
            LineCountChanged = null;
        }
    }

    public class ModLoader : LoadingExtensionBase
    {
        private UITransportPanel _extendedBusPanel;
        private UITransportPanel _extendedMetroPanel;
        private UITransportPanel _extendedTrainPanel;

        private UIComponent _busPanel;
        private UIComponent _metroPanel;
        private UIComponent _trainPanel;

        private UIComponent _busBg;
        private UIComponent _metroBg;
        private UIComponent _trainBg;

        private LoadMode _mode;

        public override void OnLevelLoaded(LoadMode mode)
        {
            _mode = mode;

            // don't load mod in asset and map editor
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
                return;

            // attach extended panels
            var view = UIView.GetAView();

            var goBus = new GameObject("ExtendedBusPanel");
            _extendedBusPanel = goBus.AddComponent<UITransportPanel>();
            _extendedBusPanel.transform.parent = view.transform;
            _extendedBusPanel.Type = TransportInfo.TransportType.Bus;

            var goMetro = new GameObject("ExtendedMetroPanel");
            _extendedMetroPanel = goMetro.AddComponent<UITransportPanel>();
            _extendedMetroPanel.transform.parent = view.transform;
            _extendedMetroPanel.Type = TransportInfo.TransportType.Metro;

            var goTrain = new GameObject("ExtendedTrainPanel");
            _extendedTrainPanel = goTrain.AddComponent<UITransportPanel>();
            _extendedTrainPanel.transform.parent = view.transform;
            _extendedTrainPanel.Type = TransportInfo.TransportType.Train;

            HookIntoNativeUI();
        }

        public override void OnLevelUnloading()
        {
            if (_mode != LoadMode.LoadGame && _mode != LoadMode.NewGame)
                return;

            TransportObserver.ClearSubscribers();

            // Making absolutely sure to unsubscribe ALL callbacks that where set on
            // loading the mod. It seems the game sometimes(?) caches UI Elements even
            // when going back to the main menu. This obviously leads to issues when
            // the now non-existent GameObjects are being referenced.
            if (_busPanel != null)
            {
                if (_busPanel.parent != null)
                    _busPanel.parent.eventVisibilityChanged -= InfoPanelOnEventVisibilityChanged;

                _busPanel.eventClick -= BusPanelOnEventClick;
                _busPanel.eventMouseHover -= BusPanelOnEventMouseHover;
                _busPanel.eventMouseLeave -= BusPanelOnEventMouseLeave;
            }

            if (_metroPanel != null)
            {
                _metroPanel.eventClick -= MetroPanelOnEventClick;
                _metroPanel.eventMouseHover -= MetroPanelOnEventMouseHover;
                _metroPanel.eventMouseLeave -= MetroPanelOnEventMouseLeave;
            }

            if (_trainPanel != null)
            {
                _trainPanel.eventClick -= TrainPanelOnEventClick;
                _trainPanel.eventMouseHover -= TrainPanelOnEventMouseHover;
                _trainPanel.eventMouseLeave -= TrainPanelOnEventMouseLeave;
            }

            if (_extendedBusPanel != null)
                GameObject.Destroy(_extendedBusPanel.gameObject);
            if (_extendedMetroPanel != null)
                GameObject.Destroy(_extendedMetroPanel.gameObject);
            if (_extendedTrainPanel != null)
                GameObject.Destroy(_extendedTrainPanel.gameObject);
        }

        private void HookIntoNativeUI()
        {
            _busPanel = UIUtil.FindUIComponent("BusPanel");
            _metroPanel = UIUtil.FindUIComponent("MetroPanel");
            _trainPanel = UIUtil.FindUIComponent("TrainPanel");

            _busBg = UIUtil.FindUIComponent("BusBackground");
            _metroBg = UIUtil.FindUIComponent("MetroBackground");
            _trainBg = UIUtil.FindUIComponent("TrainBackground");

            if (_busPanel == null || _busBg == null)
            {
                ModDebug.Error("Failed to locate BusPanel - could not hook into native UI.");
                return;
            }

            if (_metroPanel == null || _metroBg == null)
            {
                ModDebug.Error("Failed to locate MetroPanel - could not hook into native UI.");
                return;
            }

            if (_trainPanel == null || _trainBg == null)
            {
                ModDebug.Error("Failed to locate TrainPanel - could not hook into native UI.");
                return;
            }

            // extended bus hook
            _busPanel.eventClick += BusPanelOnEventClick;
            _busPanel.eventMouseHover += BusPanelOnEventMouseHover;
            _busPanel.eventMouseLeave += BusPanelOnEventMouseLeave;

            // extended metro hook
            _metroPanel.eventClick += MetroPanelOnEventClick;
            _metroPanel.eventMouseHover += MetroPanelOnEventMouseHover;
            _metroPanel.eventMouseLeave += MetroPanelOnEventMouseLeave;

            // extended train hook
            _trainPanel.eventClick += TrainPanelOnEventClick;
            _trainPanel.eventMouseHover += TrainPanelOnEventMouseHover;
            _trainPanel.eventMouseLeave += TrainPanelOnEventMouseLeave;

            // hide all extended panels, when the transport info view gets closed
            _busPanel.parent.eventVisibilityChanged += InfoPanelOnEventVisibilityChanged;
        }

        private void BusPanelOnEventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            _extendedMetroPanel.isVisible = false;
            _extendedTrainPanel.isVisible = false;
            _extendedBusPanel.isVisible = !_extendedBusPanel.isVisible;
            _extendedBusPanel.relativePosition = new Vector3(396, 58);
        }

        private void BusPanelOnEventMouseHover(UIComponent component, UIMouseEventParameter eventParam)
        {
            _busBg.color = new Color32(84, 182, 231, 255);
        }

        private void BusPanelOnEventMouseLeave(UIComponent component, UIMouseEventParameter eventParam)
        {
            _busBg.color = new Color32(44, 142, 191, 255);
        }

        private void MetroPanelOnEventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            _extendedBusPanel.isVisible = false;
            _extendedTrainPanel.isVisible = false;
            _extendedMetroPanel.isVisible = !_extendedMetroPanel.isVisible;
            _extendedMetroPanel.relativePosition = new Vector3(396, 58);
        }
        private void MetroPanelOnEventMouseHover(UIComponent component, UIMouseEventParameter eventParam)
        {
            _metroBg.color = new Color32(40, 224, 40, 255);
        }

        private void MetroPanelOnEventMouseLeave(UIComponent component, UIMouseEventParameter eventParam)
        {
            _metroBg.color = new Color32(0, 184, 0, 255);
        }

        private void TrainPanelOnEventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            _extendedBusPanel.isVisible = false;
            _extendedMetroPanel.isVisible = false;
            _extendedTrainPanel.isVisible = !_extendedTrainPanel.isVisible;
            _extendedTrainPanel.relativePosition = new Vector3(396, 58);
        }

        private void TrainPanelOnEventMouseHover(UIComponent component, UIMouseEventParameter eventParam)
        {
            _trainBg.color = new Color32(255, 126, 40, 255);
        }

        private void TrainPanelOnEventMouseLeave(UIComponent component, UIMouseEventParameter eventParam)
        {
            _trainBg.color = new Color32(219, 86, 0, 255);
        }

        private void InfoPanelOnEventVisibilityChanged(UIComponent component, bool visibility)
        {
            if (!visibility)
            {
                _extendedBusPanel.isVisible = false;
                _extendedMetroPanel.isVisible = false;
                _extendedTrainPanel.isVisible = false;
            }
        }

    }
}
