using ImproveGame.Content.Functions.Construction;
using ImproveGame.UIFramework.UIElements;
using SilkyUIFramework;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Extensions;
using CWand = ImproveGame.Content.Items.CreateWand;

namespace ImproveGame.UserInterfaces.CreateWand;

[RegisterUI]
public partial class CreateWandController : BaseBody
{
    public static CreateWandController Instance { get; private set; }

    public void Toggle(CWand wand)
    {
        Enabled = !Enabled;
        if (LocalDataContext is CreateWandViewModel cwvm)
        {
            cwvm.SetModel(wand);
            for (int n = 0; n < 24; n++)
                ItemSlots_Interanl[n]?.Item = wand.BuildingMaterials[n];
        }
    }

    public override IEnumerable<UIView> BlurElements => [MainContainer];
    private SUIBuildMaterialItemSlot[] ItemSlots_Interanl { get; } = new SUIBuildMaterialItemSlot[24];
    public IReadOnlyList<SUIBuildMaterialItemSlot> ItemSlots => ItemSlots_Interanl;

    protected override void OnInitialize()
    {
        Instance = this;

        LocalDataContext = new CreateWandViewModel();

        InitializeComponent();

        MainContainer.BorderColor = SUIColor.Border;
        MainContainer.BackgroundColor = SUIColor.Background * 0.75f;

        Header.ControlTarget = this;
        Title.UseDeathText();

        X.Texture2D = ModAsset.X;
        X.LeftMouseDown += delegate { Enabled = false; };

        Title.Text = GetText("UI.CreateWandController.Title");
        FromStructureFileButton.Text = GetText("UI.CreateWandController.ImportFromStructureFile");
        FromDatamapButton.Text = GetText("UI.CreateWandController.ImportFromDatamap");
        MaterialButton.Text = GetText("UI.CreateWandController.BuildingMaterial");
        BuildingDataListButton.Text = GetText("UI.CreateWandController.StructureSelection");

        Folder.Texture2D = ModAsset.Folder;

        SetHeaderButtonHoverAnim(Folder, X);

        SetNavButtonHoverAnim(MaterialButton, BuildingDataListButton);

        for (int i = 0; i < 24; i++)
        {
            var slot =
            new SUIBuildMaterialItemSlot()
            {
                Width = new Dimension(48),
                Height = new Dimension(48),
                BorderRadius = new Vector4(8),
                BorderColor = SUIColor.Border * 0.75f,
                BackgroundColor = SUIColor.Background * 0.5f,
            };
            slot.Join(ItemSlot_Container);
            int k = i;
            slot.ItemChanged += (sender, arg) =>
            {
                if (LocalDataContext is CreateWandViewModel cwvm)
                    cwvm.SetMaterial(arg.NewValue, k);
            };
            ItemSlots_Interanl[i] = slot;
        }

        MaterialButton.LeftMouseClick += SwitchToMaterialList;
        BuildingDataListButton.LeftMouseClick += SwitchToBuildingDataList;
        FromStructureFileButton.LeftMouseClick += SwtichToStructureFileList;

        FromStructureFileButton.OnUpdateStatus += delegate
        {
            FromStructureFileButton.BackgroundColor = Color.Black * FromStructureFileButton.HoverTimer.Lerp(0.25f, 0.1f);
        };
        FromDatamapButton.OnUpdateStatus += delegate
        {
            FromDatamapButton.BackgroundColor = Color.Black * FromDatamapButton.HoverTimer.Lerp(0.25f, 0.1f);
        };

        BuildingDataList.ViewTemplate = StructurePreviewCardTemplate.Instance;
        StructureFileList.ViewTemplate = ConstructStructureCardTemplate.Instance;
    }

    private void SwitchToMaterialList(UIView sender, SilkyUIFramework.UIMouseEvent evt)
    {
        ItemSlot_Container.Invalid = false;
        BuildingDataListPanel.Invalid = true;
    }

    private void SwitchToBuildingDataList(UIView sender, SilkyUIFramework.UIMouseEvent evt)
    {
        ItemSlot_Container.Invalid = true;
        BuildingDataListPanel.Invalid = false;
    }

    private void SwtichToStructureFileList(UIView sender, SilkyUIFramework.UIMouseEvent evt)
    {
        if (StructureFileList.Invalid)
        {
            BuildingDataList.Invalid = true;
            StructureFileList.Invalid = false;
            FromStructureFileButton.Text = GetText("UI.CreateWandController.BackToBuildingDataList");
        }
        else
        {
            BuildingDataList.Invalid = false;
            StructureFileList.Invalid = true;
            FromStructureFileButton.Text = GetText("UI.CreateWandController.ImportFromStructureFile");
        }
    }

    static void SetHeaderButtonHoverAnim(params SUIImage[] images)
    {
        foreach (var image in images)
        {
            image.OnUpdateStatus += (_) => image.ImageColor = Color.White * image.HoverTimer.Lerp(0.5f, 1f);
        }
    }

    static void SetNavButtonHoverAnim(params UIView[] buttons)
    {
        foreach (var button in buttons)
        {
            button.OnUpdateStatus += (_) => button.BackgroundColor = Color.Black * button.HoverTimer.Lerp(0f, 0.25f);
        }
    }

    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);
    }

    protected override void OnEnterTree()
    {
        LocalDataContext = new CreateWandViewModel();
    }
}