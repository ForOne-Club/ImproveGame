namespace ImproveGame.UI.ExtremeStorage.ToolButtons.FilterButtons;

public class ArrowFilter : FilterButton
{
    protected override int IconIndex => 0;
    protected override string LocalizationKey => "Arrow";

    public override bool Filter(Item item) => item.ammo == AmmoID.Arrow;
}

public class BulletFilter : FilterButton
{
    protected override int IconIndex => 1;
    protected override string LocalizationKey => "Bullet";

    public override bool Filter(Item item) => item.ammo == AmmoID.Bullet;
}

public class RocketFilter : FilterButton
{
    protected override int IconIndex => 2;
    protected override string LocalizationKey => "Rocket";

    public override bool Filter(Item item) => item.ammo == AmmoID.Rocket;
}

public class DartFilter : FilterButton
{
    protected override int IconIndex => 3;
    protected override string LocalizationKey => "Dart";

    public override bool Filter(Item item) => item.ammo == AmmoID.Dart;
}

public class OtherAmmoFilter : FilterButton
{
    protected override int IconIndex => 4;
    protected override string LocalizationKey => "Other";

    public override bool Filter(Item item) => item.ammo != AmmoID.Arrow && item.ammo != AmmoID.Bullet &&
                                              item.ammo != AmmoID.Rocket && item.ammo != AmmoID.Dart;
}