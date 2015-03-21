using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EPTUI
{
    public class UITransportPanel : UIPanel
    {
        public TransportInfo.TransportType Type { get; set; }

        private UITitleContainer _title;
        private UIButtonContainer _buttons;
        private UICaptionContainer _captions;
        private List<GameObject> _transportLineLabels;

        public void PopulateTransportLineLabels()
        {
            foreach (var index in TransportUtil.GetUsedTransportLineIndices())
            {
                if (TransportUtil.GetTransportLineType(index) != Type)
                    continue;

                var go = new GameObject(Enum.GetName(typeof (TransportInfo.TransportType), Type) + "LineRow");
                var uic = go.AddComponent<UITransportLineRow>();
                uic.LineID = index;
                uic.IsLineHidden = TransportUtil.IsTransportLineHidden(index);
                _transportLineLabels.Add(go);
            }

            _transportLineLabels.Sort(
                (left, right) =>
                    String.Compare(left.GetComponent<UITransportLineRow>()
                        .LineName, right.GetComponent<UITransportLineRow>().LineName, StringComparison.OrdinalIgnoreCase)
            );

            foreach (var go in _transportLineLabels)
                AttachUIComponent(go);

            var neededHeight = _transportLineLabels.Count*(16+2) + 95;
            height = Mathf.Clamp(neededHeight, 300, 1000);
        }

        public void ClearTransportLineLabels()
        {
            // the obvious approach using RemoveUIComponent doesn't work -
            // probably because it only removes the Component in the PoolList
            // but doesn't remove the Unity GameObject containing the Component itself
            foreach (var go in _transportLineLabels)
                Destroy(go);
            _transportLineLabels.Clear();
        }

        public override void Start()
        {
            base.Start();

            relativePosition = new Vector3(396, 58);
            backgroundSprite = "MenuPanel2";
            isVisible = false;
            canFocus = true;
            isInteractive = true;
            width = 450;
            height = 300;

            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            autoLayoutPadding = new RectOffset(0, 0, 1, 1);
            autoLayoutStart = LayoutStart.TopLeft;

            SetupControls();
            PopulateTransportLineLabels();
        }

        private void SetupControls()
        {
            _transportLineLabels = new List<GameObject>();

            _title = AddUIComponent<UITitleContainer>();
            _title.Parent = this;

            _buttons = AddUIComponent<UIButtonContainer>();
            _captions = AddUIComponent<UICaptionContainer>();

            switch (Type)
            {
                case TransportInfo.TransportType.Bus:
                    _title.IconSprite = "SubBarPublicTransportBus";
                    _title.TitleText = "Bus Lines";
                    break;
                case TransportInfo.TransportType.Metro:
                    _title.IconSprite = "SubBarPublicTransportMetro";
                    _title.TitleText = "Metro Lines";
                    break;
                case TransportInfo.TransportType.Train:
                    _title.IconSprite = "SubBarPublicTransportTrain";
                    _title.TitleText = "Train Lines";
                    break;
            }

            eventVisibilityChanged += (component, visible) => 
            {
                if (visible)
                {
                    foreach (var index in TransportUtil.GetUsedTransportLineIndices())
                    {
                        if (TransportUtil.GetTransportLineType(index) != Type)
                            TransportUtil.HideTransportLine(index);
                    }
                }
                else
                {
                    foreach (var index in TransportUtil.GetUsedTransportLineIndices())
                        TransportUtil.ShowTransportLine(index);
                }
            };

            // update if lines added or deleted
            TransportObserver.LineCountChanged += count =>
            {
                ClearTransportLineLabels();
                PopulateTransportLineLabels();
            };

            _buttons.SelectAll.eventClick += (component, param) =>
            {
                foreach (var go in _transportLineLabels)
                {
                    var row = go.GetComponent<UITransportLineRow>();
                    row.ShowLine();
                }
            };

            _buttons.SelectNone.eventClick += (component, param) =>
            {
                foreach (var go in _transportLineLabels)
                {
                    var row = go.GetComponent<UITransportLineRow>();
                    row.HideLine();
                }
            };
        }
    }
}