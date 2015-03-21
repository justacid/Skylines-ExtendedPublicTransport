using ColossalFramework.UI;
using System;
using UnityEngine;

namespace EPTUI
{
    public class UITitleContainer : UIPanel
    {
        private UISprite _icon;
        private UILabel _title;
        private UIButton _close;
        private UIDragHandle _drag;

        public UIPanel Parent { get; set; }
        public string IconSprite { get; set; }

        public string TitleText
        {
            get { return _title.text; }
            set { _title.text = value; }
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

        public override void Awake()
        {
            base.Awake();

            _icon = AddUIComponent<UISprite>();
            _title = AddUIComponent<UILabel>();
            _close = AddUIComponent<UIButton>();
            _drag = AddUIComponent<UIDragHandle>();

            height = 40;
            width = 450;
            TitleText = "(None)";
            IconSprite = "";
        }

        public override void Start()
        {
            base.Start();

            if (Parent == null)
            {
                ModDebug.Error(String.Format("Parent not set in {0}", this.GetType().Name));
                return;
            }

            width = Parent.width;
            relativePosition = Vector3.zero;
            isVisible = true;
            canFocus = true;
            isInteractive = true;;

            _drag.width = width - 50;
            _drag.height = height;
            _drag.relativePosition = Vector3.zero;
            _drag.target = Parent;

            _icon.spriteName = IconSprite;
            _icon.relativePosition = new Vector3(5, 10);

            _title.relativePosition = new Vector3(50, 13);
            _title.text = TitleText;

            _close.relativePosition = new Vector3(width - 35, 2);
            _close.normalBgSprite = "buttonclose";
            _close.hoveredBgSprite = "buttonclosehover";
            _close.pressedBgSprite = "buttonclosepressed"; 
            _close.eventClick += (component, param) => Parent.Hide();
        }
    }
}
