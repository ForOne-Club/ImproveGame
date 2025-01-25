using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config.UI;

namespace ImproveGame.UI.ModernConfig.OptionElements
{
    public class OptionArray:OptionCollections
    {
        private Type itemType;

        protected override bool CanAdd => false;

        protected override void AddItem()
        {
            throw new NotImplementedException();
        }

        protected override void ClearCollection()
        {
            throw new NotImplementedException();
        }

        protected override void InitializeCollection()
        {
            throw new NotImplementedException();
        }

        protected override void NullCollection()
        {
            throw new NotImplementedException();
        }

        protected override void PrepareTypes()
        {
            itemType = VariableInfo.Type.GetElementType();
        }

        protected override void SetupList()
        {
            //ListPanel.Clear();
            OptionView.ListView.RemoveAllChildren();
            Array array = VariableInfo.GetValue(Item) as Array;
            int count = array.Length;
            for (int i = 0; i < count; i++)
                WrapIt(OptionView.ListView, Config, VariableInfo, Item, array, itemType, i,this);
            //TODO 排版上改成网格式而不是纵向列表
            Recalculate();

        }
    }
}
