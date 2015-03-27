using ColossalFramework.UI;
using System;
using UnityEngine;
using System.Reflection;

namespace EPTUI
{
    public class UITransportLineRow : UIPanel
    {
        private UICustomCheckbox _checkBox;
        private UIColorField _color;

        private UILabel _name;
        private UILabel _stops;
        private UILabel _passengers;
        private UILabel _trips;
        private UILabel _vehicles;
        
        private ushort _lineID;
        private InstanceID _instanceID;
        private string _lineName;

        public ushort LineID
        {
            get { return _lineID; }
            set
            {
                _lineID = value;
                _instanceID = new InstanceID { TransportLine = _lineID };
            }
        }

        public string LineName
        {
            get
            {
                if (_lineName == null)
                    return TransportUtil.GetLineName(LineID);
                return _lineName;
            }
            private set { _lineName = value; }
        }

        public InstanceID InstanceID
        {
            get { return _instanceID; }
        }

		public bool IsOdd { get; set; }

        public float Height
        {
            get { return height; }
            set { height = value; }
        }

        public float Width
        {
            get { return width; }
            set { width = value; }
        }

        public bool IsChecked
        {
            get { return _checkBox.IsChecked; }
            set { _checkBox.IsChecked = value; }
        }

        public override void Awake()
        {
            base.Awake();

            _checkBox = AddUIComponent<UICustomCheckbox>();
            _color = AddUIComponent<UIColorField>();

            _name = AddUIComponent<UILabel>();
            _stops = AddUIComponent<UILabel>();
            _passengers = AddUIComponent<UILabel>();
            _trips = AddUIComponent<UILabel>();
            _vehicles = AddUIComponent<UILabel>();

            height = 16;
            width = 450;
        }

        public override void Start()
        {
            base.Start();

            _checkBox.relativePosition = new Vector3(5, 0);
            _color.relativePosition = new Vector3(22, 0);
            _name.relativePosition = new Vector3(43, 0);
            _stops.relativePosition = new Vector3(170, 0);
            _passengers.relativePosition = new Vector3(225, 0);
            _trips.relativePosition = new Vector3(320, 0);
            _vehicles.relativePosition = new Vector3(401, 0);

            _name.textColor = new Color32(185, 221, 254, 255);
            _stops.textColor = new Color32(185, 221, 254, 255);
            _passengers.textColor = new Color32(185, 221, 254, 255);
            _trips.textColor = new Color32(185, 221, 254, 255);
            _vehicles.textColor = new Color32(185, 221, 254, 255);

            _checkBox.size = new Vector2(12, 12);

            _color.normalBgSprite = "ColorPickerOutline";
            _color.normalFgSprite = "ColorPickerColor";
            _color.hoveredBgSprite = "ColorPickerOutlineHovered";
            _color.size = new Vector2(15, 15);

            _color.triggerButton = _color.AddUIComponent<UIButton>();
            _color.triggerButton.FitTo(_color.triggerButton.parent);
            _color.triggerButton.CenterToParent();

            // Need to attach the ColorPicker somehow, the Button is being setup by ColorField itself (That's what I believe at least)
            var panel = UIView.library.Get<PublicTransportWorldInfoPanel>("PublicTransportWorldInfoPanel");
            var fieldInfo = typeof (PublicTransportWorldInfoPanel).GetField("m_ColorField", BindingFlags.NonPublic | BindingFlags.Instance);
            var picker = (UIColorField)fieldInfo.GetValue(panel);
            _color.colorPicker = UnityEngine.Object.Instantiate<UIColorPicker>(picker.colorPicker);

            // event handler
            _checkBox.eventClick += (component, param) =>
            {
                _checkBox.IsChecked = !_checkBox.IsChecked;

                if (!_checkBox.IsChecked)
                    TransportUtil.HideTransportLine(LineID);
                else
                    TransportUtil.ShowTransportLine(LineID);
            };

            _name.eventClick += (component, param) =>
                WorldInfoPanel.Show<PublicTransportWorldInfoPanel>(TransportUtil.GetFirstLineStop(LineID), InstanceID);

            _name.eventMouseHover += (component, param) =>
            {
                TransportUtil.SelectTransportLine(LineID);
                _name.textColor = TransportUtil.GetLineColor(LineID);
            };

            _name.eventMouseLeave += (component, param) =>
            {
                TransportUtil.DeselectTransportLine(LineID);
                _name.textColor = new Color32(185, 221, 254, 255);
            };
            _color.eventSelectedColorChanged += (UIComponent component, Color value) => {
                TransportUtil.SetLineColor(LineID,value);
            };

            // scale label texts
            _name.textScale = 0.8f;
            _stops.textScale = 0.8f;
            _passengers.textScale = 0.8f;
            _trips.textScale = 0.8f;
            _vehicles.textScale = 0.8f;

            // zebra stripes background
            backgroundSprite = "GenericPanelLight";
            if (IsOdd)
                color = new Color32(150, 150, 150, 255);
            else
                color = new Color32(130, 130, 130, 255);

            // center elements in row
			UIComponent[] children = GetComponentsInChildren<UIComponent>();
			foreach (UIComponent child in children) {
				if(child == this) continue;

				child.pivot = UIPivotPoint.MiddleLeft;
				child.transformPosition = new Vector3(child.transformPosition.x, GetBounds().center.y, 0);
			}
        }

        public void ShowLine()
        {
            TransportUtil.ShowTransportLine(LineID);
        }

        public void HideLine()
        {
            TransportUtil.HideTransportLine(LineID);
        }

        public override void Update()
        {
            base.Update();

            var residents = TransportUtil.GetResidentPassengerCount(LineID);
            var tourists = TransportUtil.GetTouristPassengerCount(LineID);

            LineName = TransportUtil.GetLineName(LineID);

            _name.text = LineName.Trim();
            bool clipped = false;
            while (_name.width > 110)
            {
                _name.text = _name.text.Remove(_name.text.Length - 1);
                clipped = true;
            }
            if (clipped)
                _name.text = _name.text.Trim() + "...";

            _stops.text = TransportUtil.GetStopCount(LineID).ToString();
            _passengers.text = String.Format("{0}/{1}", residents, tourists);
            _trips.text = String.Format("{0}%", TransportUtil.GetTripsSaved(LineID));
            _vehicles.text = TransportUtil.GetVehicleCount(LineID).ToString();

            _color.selectedColor = TransportUtil.GetLineColor(LineID);
            IsChecked = !TransportUtil.IsTransportLineHidden(LineID);
        }
    }
}
