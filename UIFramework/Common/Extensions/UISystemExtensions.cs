namespace ImproveGame.UIFramework.Common.Extensions
{
    public static class UISystemExtensions
    {
        public static void Insert(this List<GameInterfaceLayer> layers, int index, string name, UIState state,
            Func<bool> func)
        {
            layers.Insert(index + 1, new LegacyGameInterfaceLayer($"ImproveGame: {name}", () =>
            {
                if (func())
                {
                    state.Draw(Main.spriteBatch);
                }

                return true;
            }, InterfaceScaleType.UI));
        }

        public static void Insert(this List<GameInterfaceLayer> layers, int index, string name,
            GameInterfaceDrawMethod func)
        {
            layers.Insert(index + 1, new LegacyGameInterfaceLayer($"ImproveGame: {name}", func, InterfaceScaleType.UI));
        }

        public static void FindVanilla(this List<GameInterfaceLayer> layers, string name, Action<int> action)
        {
            int index = layers.FindIndex(layer => layer.Name == $"Vanilla: {name}");
            if (index > -1)
            {
                action(index);
            }
        }
    }
}