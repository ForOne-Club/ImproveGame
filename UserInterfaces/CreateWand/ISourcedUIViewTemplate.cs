using SilkyUIFramework.Elements;

namespace ImproveGame.UserInterfaces.CreateWand;

public interface ISourcedUIViewTemplate
{
    UIView ConstructFromSource(object sourceData);
}