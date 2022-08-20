using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onestick.src
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Vintagestory.API.Client;
    using Vintagestory.API.Common;
    using Vintagestory.API.Common.Entities;
    using Vintagestory.API.Datastructures;
    using Vintagestory.API.MathTools;
    using Vintagestory.GameContent;
    class ItemAxeFix : ItemAxe
    {
        static readonly int MAX_TOOL_TIER = 5;

        static SimpleParticleProperties dustParticles = new SimpleParticleProperties()
        {
            MinPos = new Vec3d(),
            AddPos = new Vec3d(),
            MinQuantity = 0,
            AddQuantity = 3,
            Color = ColorUtil.ToRgba(100, 200, 200, 200),
            GravityEffect = 1f,
            WithTerrainCollision = true,
            ParticleModel = EnumParticleModel.Quad,
            LifeLength = 0.5f,
            MinVelocity = new Vec3f(-1, 2, -1),
            AddVelocity = new Vec3f(2, 0, 2),
            MinSize = 0.07f,
            MaxSize = 0.1f,
            WindAffected = true
        };

        static ItemAxeFix()
        {
            dustParticles.ParticleModel = EnumParticleModel.Quad;
            dustParticles.AddPos.Set(1, 1, 1);
            dustParticles.MinQuantity = 2;
            dustParticles.AddQuantity = 12;
            dustParticles.LifeLength = 4f;
            dustParticles.MinSize = 0.2f;
            dustParticles.MaxSize = 0.5f;
            dustParticles.MinVelocity.Set(-0.4f, -0.4f, -0.4f);
            dustParticles.AddVelocity.Set(0.8f, 1.2f, 0.8f);
            dustParticles.DieOnRainHeightmap = false;
            dustParticles.WindAffectednes = 0.5f;
        }

        public override bool OnBlockBrokenWith(IWorldAccessor world, Entity byEntity, ItemSlot itemslot, BlockSelection blockSel, float dropQuantityMultiplier = 1)
        {
            if (ModConfig.instance.DebugMode) world.Logger.Notification("BlockBroken");
            IPlayer byPlayer = null;
            if (byEntity is EntityPlayer)
                byPlayer = byEntity.World.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);

            WeatherSystemBase weatherSystemBase = api.ModLoader.GetModSystem<WeatherSystemBase>();
            double windspeed = 0;
            if(weatherSystemBase!=null) windspeed = weatherSystemBase.WeatherDataSlowAccess.GetWindSpeed(byEntity.SidedPos.XYZ);

            Stack<BlockPos> foundPositions = FindTree(world, blockSel.Position, out int _, out int woodTier);

            if (foundPositions.Count == 0)
            {
                return base.OnBlockBrokenWith(world, byEntity, itemslot, blockSel, dropQuantityMultiplier);
            }

            bool damageable = DamagedBy != null && DamagedBy.Contains(EnumItemDamageSource.BlockBreaking);

            float leavesMul = 1;
            float leavesBranchyMul = 0.8f;
            int blocksbroken = 0;

            var rand = NatFloat.create(EnumDistribution.UNIFORM, 0.5f, 0.5f);

            //var one = NatFloat.create(EnumDistribution.UNIFORM, 1, 0);
            //var test1 = one.nextFloat();
            //var test2 = one.nextFloat();
            //var test3 = one.nextFloat();
            //var test4 = one.nextFloat();

            var isStone = itemslot.Itemstack.Item.Code.Path.Contains("Stone");

            float LowDropRate = ModConfig.instance.ToolTierZeroFellingBranchDropRate;
            float HighDropRate = ModConfig.instance.ToolTierZeroFellingBranchDropRate;
            if (ModConfig.instance.ToolTierFiveFellingBranchDropRate > LowDropRate) HighDropRate = ModConfig.instance.ToolTierFiveFellingBranchDropRate;
            else LowDropRate = ModConfig.instance.ToolTierFiveFellingBranchDropRate;
            float spread = HighDropRate - LowDropRate;
            float step = spread / MAX_TOOL_TIER;

            float BranchRate = 0;
            if (ModConfig.instance.UseTierMode) BranchRate = LowDropRate + itemslot.Itemstack.Item.ToolTier * step;
            else BranchRate = isStone ? ModConfig.instance.BranchDropRateStoneAxe : ModConfig.instance.BranchDropRateAxeMetal;
            if (ModConfig.instance.DebugMode) world.Logger.Notification("BlockBroken with "+(BranchRate*100)+ "% Drop Rate");

            int branches = 0;
            int brokenBranches = 0;
            /*
            int goodBreakAttempts = 0;
            int goodBreaks = 0;
            int wouldAttempt = 0;
            */
            while (foundPositions.Count > 0)
            {
                BlockPos pos = foundPositions.Pop();
                blocksbroken++;

                Block block = world.BlockAccessor.GetBlock(pos);

                bool isLog = block.BlockMaterial == EnumBlockMaterial.Wood;
                bool isBranchy = block.Code.Path.Contains("branchy");
                bool isLeaves = block.BlockMaterial == EnumBlockMaterial.Leaves;

                float breakBranch = 1;
                if(isBranchy) breakBranch = rand.nextFloat();
                //world.BlockAccessor.BreakBlock(pos, byPlayer, 1);
                float shouldBreak = 1;
                if (breakBranch > BranchRate && isBranchy) shouldBreak = 0;
                if (ModConfig.instance.DebugMode)
                {
                    if (isBranchy) branches++;
                    if (isBranchy && shouldBreak > 0) brokenBranches++;
                }
                /*
                if (isLeaves) {
                    shouldBreak = leavesMul; 
                } else if (isBranchy)
                {
                    if (breakBranch < 0.8f)
                    {
                        shouldBreak = 1;
                        goodBreakAttempts++;
                    } else shouldBreak = 0;
                }
                else
                {
                    shouldBreak = 1;
                }
                if (isBranchy) branches++;
                if (breakBranch < 0.8 && isBranchy) goodBreaks++;
                 */
                //if (shouldBreak) goodBreakAttempts++;
                //if (isBranchy && (isLeaves ? leavesMul : (isBranchy ? (breakBranch < 0.8 ? 1 : 0) : 1)) > 0) wouldAttempt++;

                //world.BlockAccessor.BreakBlock(pos, byPlayer, (breakBranch > BranchRate && isBranchy) ? 0 : isLeaves ? leavesMul : 1);
                world.BlockAccessor.BreakBlock(pos, byPlayer, shouldBreak);
                //world.BlockAccessor.BreakBlock(pos, byPlayer, isLeaves ? leavesMul : (isBranchy ? (breakBranch < 0.8 ? 1 : 0) : 1));

                if (world.Side == EnumAppSide.Client)
                {
                    dustParticles.Color = block.GetRandomColor(world.Api as ICoreClientAPI, pos, BlockFacing.UP);
                    dustParticles.Color |= 255 << 24;
                    dustParticles.MinPos.Set(pos.X, pos.Y, pos.Z);

                    if (block.BlockMaterial == EnumBlockMaterial.Leaves)
                    {
                        dustParticles.GravityEffect = (float)world.Rand.NextDouble() * 0.1f + 0.01f;
                        dustParticles.ParticleModel = EnumParticleModel.Quad;
                        dustParticles.MinVelocity.Set(-0.4f + 4 * (float)windspeed, -0.4f, -0.4f);
                        dustParticles.AddVelocity.Set(0.8f + 4 * (float)windspeed, 1.2f, 0.8f);

                    }
                    else
                    {
                        dustParticles.GravityEffect = 0.8f;
                        dustParticles.ParticleModel = EnumParticleModel.Cube;
                        dustParticles.MinVelocity.Set(-0.4f + (float)windspeed, -0.4f, -0.4f);
                        dustParticles.AddVelocity.Set(0.8f + (float)windspeed, 1.2f, 0.8f);
                    }


                    world.SpawnParticles(dustParticles);
                }


                if (damageable && isLog)
                {
                    DamageItem(world, byEntity, itemslot);
                }

                if (itemslot.Itemstack == null)
                    return true;

                if (isLeaves && leavesMul > 0.03f)
                    leavesMul *= 0.85f;
                if (isBranchy && leavesBranchyMul > 0.015f)
                    leavesBranchyMul *= 0.6f; //Ignoring this as it would cause no mater what around 7 sticks max to drop ever.
            }

            if (ModConfig.instance.DebugMode)
            {
                //world.Logger.Notification("-----{ Results }------");
                world.Logger.Notification("    branches found: " + branches);
                world.Logger.Notification("   branches broken: " + brokenBranches);
                world.Logger.Notification("actual broken rate: " + (int)((float)(brokenBranches)/(float)(branches)*100f) + "%");
                //world.Logger.Notification("--{ End of Results }--" + branches);
            }

            //world.Logger.Notification("branches:         " + branches);
            //world.Logger.Notification("goodBreakAttempts:" + goodBreakAttempts);
            //world.Logger.Notification("goodBreaks:       " + goodBreaks);
            //world.Logger.Notification("wouldAttempt:     " + wouldAttempt);

            if (blocksbroken > 35)
            {
                Vec3d pos = blockSel.Position.ToVec3d().Add(0.5, 0.5, 0.5);
                api.World.PlaySoundAt(new AssetLocation("game:sounds/effect/treefell"), pos.X, pos.Y, pos.Z, byPlayer, false, 32, GameMath.Clamp(blocksbroken / 100f, 0.25f, 1));
            }

            return true;
        }
    }
}
