using ColossalFramework.UI;

namespace EPTUI
{
    public class UICustomCheckbox : UISprite
    {
        public bool IsChecked { get; set; }

        public override void Awake()
        {
            base.Awake();
            IsChecked = true;
            spriteName = "AchievementCheckedTrue";
        }

        public override void Update()
        {
            base.Update();
            spriteName = IsChecked ? "AchievementCheckedTrue" : "AchievementCheckedFalse";
        }
    }
}
