using ColossalFramework.UI;
using UnityEngine;

namespace EPTUI
{
    public class UIButtonContainer : UIPanel
    {
        private UIButton _selectAll;
        private UIButton _selectNone;

        public UIButton SelectAll
        {
            get { return _selectAll; }
        }
        public UIButton SelectNone
        {
            get { return _selectNone; }
        }

        public override void Awake()
        {
            base.Start();

            _selectAll = AddUIComponent<UIButton>();
            _selectNone = AddUIComponent<UIButton>();

            _selectAll.text = "Select All";
            _selectAll.textScale = 0.9f;
            _selectNone.text = "Select None";
            _selectNone.textScale = 0.9f;

            _selectAll.normalBgSprite = "ButtonMenu";
            _selectAll.hoveredBgSprite = "ButtonMenuHovered";
            _selectAll.focusedBgSprite = "ButtonMenuFocused";
            _selectAll.pressedBgSprite = "ButtonMenuPressed";

            _selectNone.normalBgSprite = "ButtonMenu";
            _selectNone.hoveredBgSprite = "ButtonMenuHovered";
            _selectNone.focusedBgSprite = "ButtonMenuFocused";
            _selectNone.pressedBgSprite = "ButtonMenuPressed";

            _selectAll.size = new Vector2(100, 30);
            _selectNone.size = new Vector2(100, 30);

            _selectAll.relativePosition = new Vector3(5, 0);
            _selectNone.relativePosition = new Vector3(110, 0);

            width = 450;
            height = 30;
        }
    }
}
