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
            base.OnUpdate(realTimeDelta, simulationTimeDelta);

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
            base.OnReleased();
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

        public override void OnLevelLoaded(LoadMode mode)
        {
            // don't load mod in asset and map editor
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
                return;

            // attach extended panels
            var view = UIView.GetAView();

            var goBus = new GameObject("ExtendedBusPanel");
            _extendedBusPanel = goBus.AddComponent<UITransportPanel>();
            _extendedBusPanel.transform.parent = view.cachedTransform;
            _extendedBusPanel.Type = TransportInfo.TransportType.Bus;

            var goMetro = new GameObject("ExtendedMetroPanel");
            _extendedMetroPanel = goMetro.AddComponent<UITransportPanel>();
            _extendedMetroPanel.transform.parent = view.cachedTransform;
            _extendedMetroPanel.Type = TransportInfo.TransportType.Metro;

            var goTrain = new GameObject("ExtendedTrainPanel");
            _extendedTrainPanel = goTrain.AddComponent<UITransportPanel>();
            _extendedTrainPanel.transform.parent = view.cachedTransform;
            _extendedTrainPanel.Type = TransportInfo.TransportType.Train;

            HookIntoNativeUI();
        }

        public override void OnLevelUnloading()
        {
            TransportObserver.ClearSubscribers();
            GameObject.Destroy(_extendedBusPanel.gameObject);
            GameObject.Destroy(_extendedMetroPanel.gameObject);
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
            _busPanel.eventClick += (component, param) =>
            {
                _extendedMetroPanel.isVisible = false;
                _extendedTrainPanel.isVisible = false;
                _extendedBusPanel.isVisible = !_extendedBusPanel.isVisible;
                _extendedBusPanel.relativePosition = new Vector3(396, 58);
            };
            _busPanel.eventMouseHover += (component, param) => _busBg.color = new Color32(84, 182, 231, 255);
            _busPanel.eventMouseLeave += (component, param) => _busBg.color = new Color32(44, 142, 191, 255);

            // extended metro hook
            _metroPanel.eventClick += (component, param) =>
            {
                _extendedBusPanel.isVisible = false;
                _extendedTrainPanel.isVisible = false;
                _extendedMetroPanel.isVisible = !_extendedMetroPanel.isVisible;
                _extendedMetroPanel.relativePosition = new Vector3(396, 58);
            };
            _metroPanel.eventMouseHover += (component, param) => _metroBg.color = new Color32(40, 224, 40, 255);
            _metroPanel.eventMouseLeave += (component, param) => _metroBg.color = new Color32(0, 184, 0, 255);

            // extended train hook
            _trainPanel.eventClick += (component, param) =>
            {
                _extendedBusPanel.isVisible = false;
                _extendedMetroPanel.isVisible = false;
                _extendedTrainPanel.isVisible = !_extendedTrainPanel.isVisible;
                _extendedTrainPanel.relativePosition = new Vector3(396, 58);
            };
            _trainPanel.eventMouseHover += (component, param) => _trainBg.color = new Color32(255, 126, 40, 255);
            _trainPanel.eventMouseLeave += (component, param) => _trainBg.color = new Color32(219, 86, 0, 255);

            // hide all extended panels, when the transport info view gets closed
            _busPanel.parent.eventVisibilityChanged += (component, visibility) =>
            {
                if (!visibility)
                {
                    _extendedBusPanel.isVisible = false;
                    _extendedMetroPanel.isVisible = false;
                    _extendedTrainPanel.isVisible = false;
                }
            };
        }
    }
}
