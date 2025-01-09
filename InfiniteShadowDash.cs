using System;
using System.Collections.Generic;
using Modding;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using System.Collections;

namespace InfiniteShadowDash
{
    public class InfiniteShadowDash : Mod,  IMenuMod, IGlobalSettings<Settings>
    {
        public override string GetVersion() => "1.0.0.0";
        public override void Initialize()
        {
            ModHooks.DashPressedHook += OnDashPressed;
            ModHooks.CharmUpdateHook += OnCharmUpdate;
        }
        public Settings settings = new Settings();
        public void OnLoadGlobal(Settings s) => settings = s;
        public Settings OnSaveGlobal() => settings;

        private static readonly Dictionary<string, Func<PlayerData, bool>> CharmConditions = new()
        {
            { "no", _ => true },
            { "dashmaster", data => data.GetBool("equippedCharm_31") },
            { "sharp shadow", data => data.GetBool("equippedCharm_16") },
            { "DM + SS", data => data.GetBool("equippedCharm_31") && data.GetBool("equippedCharm_16") }
        };
        private readonly List<string> RCharms = new() { "no", "dashmaster", "sharp shadow", "DM + SS" };
        public bool OnDashPressed()
        {
            var hero = HeroController.instance;
            if (hero == null) return false;

            if (InputHandler.Instance.inputActions.dash.IsPressed && hero.SHADOW_DASH_COOLDOWN == 0f)
            {
                CoroutineHelper.RunCoroutine(SkipFrame(hero));
            }
            return false;
        }
        private IEnumerator SkipFrame(HeroController hero)
        {
            yield return null;
            hero.shadowRechargePrefab.SetActive(false);
        }
        public void OnCharmUpdate(PlayerData data, HeroController hero)
        {
            if (hero == null || data == null) return;

            hero.SHADOW_DASH_COOLDOWN = CharmConditions.TryGetValue(settings.ReqCharm, out var condition) && condition(data)
                ? 0f
                : 1.5f;
        }
        public void Unload()
        {
            ModHooks.DashPressedHook -= OnDashPressed; 
            ModHooks.CharmUpdateHook -= OnCharmUpdate;
        }
        public bool ToggleButtonInsideMenu => false;
        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry) =>
            new List<IMenuMod.MenuEntry>
            {
                new()
                {
                    Name = "Required charms",
                    Description = "What charms needed for infinite shadow dash",
                    Values = RCharms.ToArray(),
                    Saver = option => {
                settings.ReqCharm = RCharms[option];
                OnCharmUpdate(HeroController.instance?.playerData, HeroController.instance);
            },
                    Loader = () => RCharms.IndexOf(settings.ReqCharm) >= 0 ? RCharms.IndexOf(settings.ReqCharm) : 0
                }
            };
    }
    public static class CoroutineHelper
    {
        private static CoroutineRunner _coroutineRunner;
        public static void RunCoroutine(IEnumerator coroutine)
        {
            if (_coroutineRunner == null)
            {
                GameObject runnerObject = new GameObject("CoroutineRunner");
                _coroutineRunner = runnerObject.AddComponent<CoroutineRunner>();
                UnityEngine.Object.DontDestroyOnLoad(runnerObject); // Чтобы объект не уничтожался при смене сцен
            }
            _coroutineRunner.StartCoroutine(coroutine);
        }
    }
    public class CoroutineRunner : MonoBehaviour
    {
    }
}
