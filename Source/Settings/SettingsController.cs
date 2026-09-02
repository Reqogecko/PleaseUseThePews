using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PleaseUseThePews
{
    public class PleaseUseThePews_Settings_Controller : Mod
    {
        private readonly Settings settings;

        public PleaseUseThePews_Settings_Controller(ModContentPack content) : base(content)
        {
            settings = GetSettings<Settings>();
        }

        public override string SettingsCategory()
        {
            return "PleaseUseThePews - Settings";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoWindowContents(inRect);
        }

    }
}