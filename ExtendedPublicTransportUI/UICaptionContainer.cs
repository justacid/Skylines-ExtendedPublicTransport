using ColossalFramework.UI;
using UnityEngine;

namespace EPTUI
{
    public class UICaptionContainer : UIPanel
    {
        public SortTransportLinesDelegate sortDelegate { get; set; }

        private UILabel _name;
        private UILabel _stops;
        private UILabel _passengers;
        private UILabel _trips;
        private UILabel _vehicles;

        public override void Start()
        {
            base.Start();

            _name = AddUIComponent<UILabel>();
            _stops = AddUIComponent<UILabel>();
            _passengers = AddUIComponent<UILabel>();
            _trips = AddUIComponent<UILabel>();
            _vehicles = AddUIComponent<UILabel>();

            _name.relativePosition = new Vector3(43, 0);
            _stops.relativePosition = new Vector3(160, 0);
            _passengers.relativePosition = new Vector3(207, 0);
            _trips.relativePosition = new Vector3(294, 0);
            _vehicles.relativePosition = new Vector3(380, 0);

            _name.textScale = 0.85f;
            _stops.textScale = 0.85f;
            _passengers.textScale = 0.85f;
            _trips.textScale = 0.85f;
            _vehicles.textScale = 0.85f;

            _name.text = "Name";
            _stops.text = "Stops";
            _passengers.text = "Passengers";
            _trips.text = "Trips Saved";
            _vehicles.text = "Vehicles";

            // Sort by each column
            //  ultimately based on value from UITransportLineRow (via LineComparer)
            _name.eventClick += (component, param) =>
            {
                sortDelegate("LineName");
            };
            _stops.eventClick += (component, eventParam) =>
            {
                sortDelegate("Stops");
            };
            _passengers.eventClick += (component, param) =>
            {
                sortDelegate("TotalPassengers");
            };
            _trips.eventClick += (component, param) =>
            {
                sortDelegate("Trips");
            };
            _vehicles.eventClick += (component, param) =>
            {
                sortDelegate("Vehicles");
            };

            width = 450;
            height = 20;
        }
    }
}
