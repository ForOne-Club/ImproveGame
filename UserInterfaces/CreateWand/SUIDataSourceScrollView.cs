using SilkyUIFramework.Attributes;
using SilkyUIFramework.Elements;
using System.Collections;
using System.Collections.Specialized;

namespace ImproveGame.UserInterfaces.CreateWand;

[XmlElementMapping("DataSourceScrollView")]
public class SUIDataSourceScrollView : SUIScrollView
{
    public IEnumerable ItemSource
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            if (value is INotifyCollectionChanged notifier)
                notifier.CollectionChanged += Notifier_CollectionChanged;

            Container.RemoveAllChildren();
            if (ViewTemplate is not { } template) return;
            foreach (var item in value)
                Container.AddChild(template.ConstructFromSource(item));
        }
    }

    private void Notifier_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (ViewTemplate is not { } template) return;
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
            {
                int count = e.NewItems.Count;
                if (count == 1)
                {
                    Container.AddChild(template.ConstructFromSource(e.NewItems[0]));
                }
                else
                {
                    foreach (var item in e.NewItems)
                        Container.AddChild(template.ConstructFromSource(item));
                }
                break;
            }
            case NotifyCollectionChangedAction.Remove:
            {
                int count = e.OldItems.Count;
                if (count == 1)
                {
                    Container.RemoveChild(Container.Children[e.OldStartingIndex]);
                }
                else
                {
                    int maxIndex = e.OldStartingIndex + e.OldItems.Count;
                    HashSet<UIView> childToRemove = [];
                    for (int n = e.OldStartingIndex; n < maxIndex; n++)
                        childToRemove.Add(Container.Children[n]);
                    foreach (var child in childToRemove)
                        Container.RemoveChild(child);
                }
                break;
            }
            case NotifyCollectionChangedAction.Replace:
            {
                int count = e.NewItems.Count;
                if (count == 1)
                {
                    var child = Container.Children[e.NewStartingIndex];
                    Container.RemoveChild(child);
                    Container.AddChild(template.ConstructFromSource(e.NewItems[0]), e.NewStartingIndex);
                }
                else
                {
                    int maxIndex = e.NewStartingIndex + e.NewItems.Count;
                    HashSet<UIView> childToRemove = [];
                    for (int n = e.NewStartingIndex; n < maxIndex; n++)
                        childToRemove.Add(Container.Children[n]);
                    foreach (var child in childToRemove)
                        Container.RemoveChild(child);

                    int index = e.NewStartingIndex;
                    foreach (var item in e.NewItems)
                    {
                        Container.AddChild(template.ConstructFromSource(item), index);
                        index++;
                    }
                }
                break;
            }
            case NotifyCollectionChangedAction.Move:
            {
                int count = e.NewItems.Count;
                if (count == 1)
                {
                    var child = Container.Children[e.OldStartingIndex];
                    Container.RemoveChild(child);
                    Container.AddChild(child, e.NewStartingIndex);
                }
                else
                {
                    int maxIndex = e.OldStartingIndex + e.OldItems.Count;
                    HashSet<UIView> childToMove = [];
                    for (int n = e.OldStartingIndex; n < maxIndex; n++)
                        childToMove.Add(Container.Children[n]);
                    foreach (var child in childToMove)
                        Container.RemoveChild(child);
                    int index = e.NewStartingIndex;
                    foreach (var item in childToMove)
                    {
                        Container.AddChild(item, index);
                        index++;
                    }
                }
                break;
            }
            case NotifyCollectionChangedAction.Reset:
            {
                Container.RemoveAllChildren();
                break;
            }
        }
    }

    /// <summary>
    /// 元素模板
    /// </summary>
    public ISourcedUIViewTemplate ViewTemplate
    {
        get;
        set
        {
            if (field == value) return;

            field = value;
            Container.RemoveAllChildren();
            if (ItemSource is not { } source) return;
            foreach (var item in source)
                Container.AddChild(value.ConstructFromSource(item));
        }
    }
}
