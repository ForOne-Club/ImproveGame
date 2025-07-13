namespace ImproveGame.UIFramework.SUIElements
{
    public class SUIImageSwitch(Texture2D textureEnabled, Texture2D textureDisabled, Action<bool> setter, Func<bool> getter) : SUIImage(textureEnabled)
    {
        Texture2D TextureEnabled { get; set; } = textureEnabled;
        Texture2D TextureDisabled { get; set; } = textureDisabled;
        public bool Value
        {
            get => getter.Invoke();
            set => setter.Invoke(value);
        }
        public override void OnActivate()
        {
            Texture = Value ? TextureEnabled : TextureDisabled;
            base.OnActivate();
        }
        public override void LeftMouseDown(UIMouseEvent evt)
        {
            Value = !Value;
            Texture = Value ? TextureEnabled : TextureDisabled;
            base.LeftMouseDown(evt);
        }
    }
}
