using System;
using System.Collections;
using System.Collections.Generic;
using Modding;
using UnityEngine;

namespace InfiniteShadowDash
{
    public class InfiniteShadowDash : Mod
    {
        new public string GetName() => "Infinite Shadow Dash";
        public override string GetVersion() => "1.0.0.0";
        public override void Initialize()
        {
            ModHooks.HeroUpdateHook += OnHeroUpdate;
        }
        public void OnHeroUpdate()
        {
            HeroController hero = HeroController.instance;
            if (hero != null)
            {
                if (InputHandler.Instance.inputActions.dash.IsPressed)
                {
                    hero.SHADOW_DASH_COOLDOWN = 0;
                    hero.shadowRechargePrefab.SetActive(value: false);
                }
            }
        }
    }
}