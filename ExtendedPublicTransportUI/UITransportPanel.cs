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
        private UIScrollablePanel _scrollablePanel;
        private UIPanel _panelForScrollPanel;

        public void PopulateTransportLineLabels()
        {
            foreach (var index in TransportUtil.GetUsedTransportLineIndices())
            {
                if (TransportUtil.GetTransportLineType(index) != Type)
                    continue;

                var go = new GameObject(Enum.GetName(typeof (TransportInfo.TransportType), Type) + "LineRow");
                var uic = go.AddComponent<UITransportLineRow>();
                uic.LineID = index;
                uic.LineNameChanged += id =>
                {
                    ClearTransportLineLabels();
                    PopulateTransportLineLabels();
                };
                _transportLineLabels.Add(go);
            }

            _transportLineLabels.Sort(new LineLabelComparer());

			bool odd = false;
            foreach (var go in _transportLineLabels) {
                _scrollablePanel.AttachUIComponent(go);
				go.GetComponent<UITransportLineRow>().IsOdd = odd;
				odd = !odd;
			}

            switch (Type)
            {
                case TransportInfo.TransportType.Bus:
                    _title.TitleText = String.Format("Bus Lines ({0})", _transportLineLabels.Count);
                    break;
                case TransportInfo.TransportType.Metro:
                    _title.TitleText = String.Format("Metro Lines ({0})", _transportLineLabels.Count);
                    break;
                case TransportInfo.TransportType.Train:
                    _title.TitleText = String.Format("Train Lines ({0})", _transportLineLabels.Count);
                    break;
            }
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
            height = 347;

            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            autoLayoutPadding = new RectOffset(0, 0, 1, 1);
            autoLayoutStart = LayoutStart.TopLeft;

            SetupControls();
            SetupScrollPanel ();
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

        private void SetupScrollPanel ()
        {
            //this probably needs to exist, otherwise the autoLayout of this UITransportPanel places the scrollbar weird
            _panelForScrollPanel = AddUIComponent<UIPanel> ();
            // needed so that the colorpicker finds the right parent
            _panelForScrollPanel.gameObject.AddComponent<UICustomControl>();

            _panelForScrollPanel.width = width - 6;
            //_captions reporting 450 height? fixed value of 20
            _panelForScrollPanel.height = height - _title.height - _buttons.height - 20 - autoLayoutPadding.bottom * 4 - autoLayoutPadding.top * 4;


            // taken from http://www.reddit.com/r/CitiesSkylinesModding/comments/2zrz0k/extended_public_transport_ui_provides_addtional/cpnet5q
            _scrollablePanel = _panelForScrollPanel.AddUIComponent<UIScrollablePanel> ();
            _scrollablePanel.width = _scrollablePanel.parent.width - 5f;
            _scrollablePanel.height = _scrollablePanel.parent.height;

            _scrollablePanel.autoLayout = true;
            _scrollablePanel.autoLayoutDirection = LayoutDirection.Vertical;
            _scrollablePanel.autoLayoutStart = LayoutStart.TopLeft;
            _scrollablePanel.autoLayoutPadding = new RectOffset (0, 0, 1, 1);
            _scrollablePanel.clipChildren = true;

            _scrollablePanel.pivot = UIPivotPoint.TopLeft;
            _scrollablePanel.AlignTo (_scrollablePanel.parent, UIAlignAnchor.TopLeft);

            UIScrollbar scrollbar = _panelForScrollPanel.AddUIComponent<UIScrollbar> ();
            scrollbar.width = scrollbar.parent.width - _scrollablePanel.width;
            scrollbar.height = scrollbar.parent.height;
            scrollbar.orientation = UIOrientation.Vertical;
            scrollbar.pivot = UIPivotPoint.BottomLeft;
            scrollbar.AlignTo (scrollbar.parent, UIAlignAnchor.TopRight);
            scrollbar.minValue = 0;
            scrollbar.value = 0;
            scrollbar.incrementAmount = 50;

            UISlicedSprite tracSprite = scrollbar.AddUIComponent<UISlicedSprite> ();
            tracSprite.relativePosition = Vector2.zero;
            tracSprite.autoSize = true;
            tracSprite.size = tracSprite.parent.size;
            tracSprite.fillDirection = UIFillDirection.Vertical;
            tracSprite.spriteName = "ScrollbarTrack";

            scrollbar.trackObject = tracSprite;

            UISlicedSprite thumbSprite = tracSprite.AddUIComponent<UISlicedSprite> ();
            thumbSprite.relativePosition = Vector2.zero;
            thumbSprite.fillDirection = UIFillDirection.Vertical;
            thumbSprite.autoSize = true;
            thumbSprite.width = thumbSprite.parent.width;
            thumbSprite.spriteName = "ScrollbarThumb";

            scrollbar.thumbObject = thumbSprite;

            _scrollablePanel.verticalScrollbar = scrollbar;
            _scrollablePanel.eventMouseWheel += (component, param) =>
            {
                var sign = Math.Sign(param.wheelDelta);
                _scrollablePanel.scrollPosition += new Vector2(0, sign*(-1) * 20);
            };
        }
    }
}