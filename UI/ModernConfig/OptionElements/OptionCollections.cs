using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace ImproveGame.UI.ModernConfig.OptionElements
{
    public abstract class OptionCollections : ModernConfigOption
    {
        protected DefaultListValueAttribute DefaultListValueAttribute { get; set; }
        protected JsonDefaultListValueAttribute JsonDefaultListValueAttribute { get; set; }
        protected object Data { get; set; }
        private bool expanded = true;
        protected float Scale { get; set; } = 1f;
        protected virtual bool CanAdd => true;

        private bool pendingChanges = false;

        protected object CreateCollectionElementInstance(Type type)
        {
            object toAdd;

            if (DefaultListValueAttribute != null)
            {
                toAdd = DefaultListValueAttribute.Value;
            }
            else
            {
                toAdd = ConfigManager.AlternateCreateInstance(type);

                if (!type.IsValueType && type != typeof(string))
                {
                    string json = JsonDefaultListValueAttribute?.Json ?? "{}";

                    JsonConvert.PopulateObject(json, toAdd, ConfigManager.serializerSettings);
                }
            }

            return toAdd;
        }

        // SetupList called in base.ctor, but children need Types.
        protected abstract void PrepareTypes();

        protected abstract void AddItem();

        protected abstract void InitializeCollection();

        protected virtual void NullCollection()
        {
            Data = null;
            SetValueDirect(Data);

        }

        protected abstract void ClearCollection();

        protected abstract void SetupList();

        protected SUIScrollView2 OptionView;

        protected override void CheckAttributes()
        {
            base.CheckAttributes();
            var expandAttribute = GetAttribute<ExpandAttribute>();
            if (expandAttribute != null)
                expanded = expandAttribute.Expand;

            DefaultListValueAttribute = GetAttribute<DefaultListValueAttribute>();
        }

        protected override void OnBind()
        {
            Data = VariableInfo.GetValue(Item);
            Height.Set(360, 0);
            /*ListPanel = new View
            {
                HAlign = 1f,
                VAlign = 0.5f,
                Top = { Pixels = 20f },
                Height = StyleDimension.Fill,
                Width = { Pixels = -30f, Percent = 1f },
                MaxWidth = { Pixels = -30f, Percent = 1f },
                MaxHeight = { Pixels = 300}
            };
            ListPanel.JoinParent(this);*/

            OptionView = new SUIScrollView2(Orientation.Vertical)
            {
                RelativeMode = RelativeMode.Vertical,
                Spacing = new Vector2(6)
            };
            OptionView.SetPadding(0f, 0f);
            OptionView.SetSize(0f, -60, 1f, 1f);
            OptionView.JoinParent(this);
            

            PrepareTypes();

            SetupList();

            pendingChanges = true;
            Recalculate();

            base.OnBind();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //var dimension = ListPanel.GetDimensions();
            //var dimension2 = GetDimensions();

            if (!pendingChanges)
                return;
            pendingChanges = false;
            /*
            if (CanAdd)
            {
                RemoveChild(initializeButton);
                RemoveChild(addButton);
                RemoveChild(deleteButton);
            }
            RemoveChild(expandButton);
            RemoveChild(upDownButton);
            RemoveChild(DataListElement);

            if (Data == null)
            {
                Append(initializeButton);
            }
            else
            {
                if (CanAdd)
                {
                    Append(addButton);
                    Append(deleteButton);
                }
                Append(expandButton);
                if (expanded)
                {
                    Append(upDownButton);
                    Append(DataListElement);
                    expandButton.HoverText = Language.GetTextValue("tModLoader.ModConfigCollapse");
                    expandButton.SetImage(ExpandedTexture);
                }
                else
                {
                    expandButton.HoverText = Language.GetTextValue("tModLoader.ModConfigExpand");
                    expandButton.SetImage(CollapsedTexture);
                }
            }
            */
        }
        public override void Recalculate()
        {
            //float h = ListPanel.GetDimensions().Height + 46;
            //h = Utils.Clamp(h, 46, 300 * Scale);

            //MaxHeight.Set(300 * Scale, 0f);
            //Height.Set(h, 0f);

            //if (Parent != null)
            //{
            //    Parent.Height.Set(h, 0f);
            //}

            base.Recalculate();
        }
        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);

            if (!OptionView.ListView.IsMouseHovering)
                return;
        }
    }
}
