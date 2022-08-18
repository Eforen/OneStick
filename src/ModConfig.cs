using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onestick.src
{
    class ModConfig
    {
        private static ModConfig _instance = new ModConfig();
        public static ModConfig instance { get { return _instance; } set { _instance = value; } }

        // Stone Axes
        private float _BranchDropRateStoneAxe = 0.8f;
        public float BranchDropRateStoneAxe { get { return _BranchDropRateStoneAxe; } set { _BranchDropRateStoneAxe = value >= 0 ? value : 0;  } }
        // Metal Axes
        private float _BranchDropRateAxeMetal = 0.8f;
        public float BranchDropRateAxeMetal { get { return _BranchDropRateAxeMetal; } set { _BranchDropRateAxeMetal = value >= 0 ? value : 0; } }

        /// <summary>
        /// Should the mod use the Tier logic or the basic stone/metal mode
        /// </summary>
        public bool UseTierMode { get { return _UseTierMode; } set { _UseTierMode = value; } }
        public bool _UseTierMode = true;

        /// <summary>
        /// Bottom Drop Rate when a tree is fell by an axe
        /// </summary>
        public float ToolTierZeroFellingBranchDropRate { get { return _ToolTierZeroFellingBranchDropRate; } set { _ToolTierZeroFellingBranchDropRate = value >= 0 ? value : 0; } }
        public float _ToolTierZeroFellingBranchDropRate = 0.1f;

        /// <summary>
        /// Top Drop Rate when a tree is fell by an axe
        /// </summary>
        public float ToolTierFiveFellingBranchDropRate { get { return _ToolTierFiveFellingBranchDropRate; } set { _ToolTierFiveFellingBranchDropRate = value >= 0 ? value : 0; } }
        public float _ToolTierFiveFellingBranchDropRate = 0.8f;

        public bool DebugMode { get; set; }
    }
}
