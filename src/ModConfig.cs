using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace treefellingfix.src
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

        public bool DebugMode { get; set; }
    }
}
