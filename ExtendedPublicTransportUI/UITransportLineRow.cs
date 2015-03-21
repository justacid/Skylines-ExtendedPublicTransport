using ColossalFramework.UI;
using System;
using UnityEngine;

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

        public bool IsLineHidden { get; set; }

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
            /*
        }

        public override void Start()
        {
            base.Start();*/

            _checkBox.relativePosition = new Vector3(5, 0);
            _color.relativePosition = new Vector3(22, -1.5f);
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
            _checkBox.IsChecked = !IsLineHidden;

            _color.normalBgSprite = "ColorPickerOutline";
            _color.normalFgSprite = "ColorPickerColor";
            _color.hoveredBgSprite = "ColorPickerOutlineHovered";
            _color.size = new Vector2(15, 15);
            //_color.triggerButton = _color.AddUIComponent<UIButton>();

            /* // Need to attach the ColorPicker somehow, the Button is being setup by ColorField itself (That's what I believe at least)
            var panel = UIView.library.Get<PublicTransportWorldInfoPanel>("PublicTransportWorldInfoPanel");
            var fieldInfo = typeof (PublicTransportWorldInfoPanel).GetField("m_ColorField", BindingFlags.NonPublic | BindingFlags.Instance);
            var picker = (UIColorField)fieldInfo.GetValue(panel);
            _color.colorPicker = Instantiate<UIColorPicker>(picker.colorPicker);
            */

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

            // scale label texts
            _name.textScale = 0.8f;
            _stops.textScale = 0.8f;
            _passengers.textScale = 0.8f;
            _trips.textScale = 0.8f;
            _vehicles.textScale = 0.8f;
        }

        public void ShowLine()
        {
            TransportUtil.ShowTransportLine(LineID);
            _checkBox.IsChecked = true;
        }

        public void HideLine()
        {
            TransportUtil.HideTransportLine(LineID);
            _checkBox.IsChecked = false;
        }

        public override void Update()
        {
            base.Update();

            var residents = TransportUtil.GetResidentPassengerCount(LineID);
            var tourists = TransportUtil.GetTouristPassengerCount(LineID);

            LineName = TransportUtil.GetLineName(LineID);
            _name.text = LineName;
            _stops.text = TransportUtil.GetStopCount(LineID).ToString();
            _passengers.text = String.Format("{0}/{1}", residents, tourists);
            _trips.text = String.Format("{0}%", TransportUtil.GetTripsSaved(LineID));
            _vehicles.text = TransportUtil.GetVehicleCount(LineID).ToString();

            _color.selectedColor = TransportUtil.GetLineColor(LineID);
        }
    }
}
