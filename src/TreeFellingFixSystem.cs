using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace treefellingfix.src
{
    using Vintagestory.API.Common;
    using Vintagestory.API.Server;
    using Vintagestory.API.Client;

    class TreeFellingFixSystem : ModSystem
    {
        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return true;
        }

        public override void Start(ICoreAPI api)
        {
            Console.WriteLine("WTF!");
            base.Start(api);
            try
            {
                ModConfig file;
                if ((file = api.LoadModConfig<ModConfig>("TreeFellingFixConfig.json")) == null)
                {
                    api.StoreModConfig<ModConfig>(ModConfig.instance, "TreeFellingFixConfig.json");
                }
                else ModConfig.instance = file;
            }
            catch
            {
                api.StoreModConfig<ModConfig>(ModConfig.instance, "TreeFellingFixConfig.json");
            }

            api.RegisterItemClass("tv_eforen_tff_axe", typeof(ItemAxeFix));
        }
    }
}
