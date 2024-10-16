﻿using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;

namespace BannerPlugin
{
    public class BannerPlugin : BasePlugin, IPluginConfig<PluginConfig>
    {
        public FakeConVar<bool> bannerEnabled = new("banner_enable", "Whether banner is enabled or not. Default: false", false);

        public override string ModuleName => "Banner Plugin";

        public override string ModuleVersion => "0.0.1";

        public override string ModuleAuthor => "JasonX- (https://github.com/p07575/)";
    
        public override string ModuleDescription => "A plugin for showing banner";

        public required PluginConfig Config { get; set; } = new();

        private string _currentBanner;
        private DateTime _lastGenerated;
        private readonly TimeSpan _duration = TimeSpan.FromSeconds(20); // Defina X segundos
        private readonly Random _random = new Random();

        public override void Load(bool hotReload)
        {
            Console.WriteLine("Banner Plugin Loaded");
            RegisterListeners();
            ScheduleBannerShow();
        }

        public void showBanner(CCSPlayerController player, string text)
        {  
            player.PrintToCenterHtml(text);
            return;
        }

        public void showHtml()
        {
            foreach (var player in Utilities.GetPlayers())
			{
				if (player != null && !player.IsBot && bannerEnabled.Value)
				{
                    //if (!player.IsValid || player.IsBot || player.IsHLTV) return;
                    player.PrintToCenterHtml($"<img src='{_currentBanner}'</img>");
                }
			}
        }

        [ConsoleCommand("css_b", "alias for !b")]
        [ConsoleCommand("css_banner", "alias for !banner")]
        [ConsoleCommand("sv_showbanner", "Shows a banner in players hud")]
        public void OnBanner(CCSPlayerController? player, CommandInfo command)
        {
            if (player != null && bannerEnabled.Value) {
                showBanner(player, "You are playing on Jason's Server");
                return;
            }

            Console.WriteLine("Banner command called.");
        }

		private void OnTick()
		{
            if (!bannerEnabled.Value) return;

            showHtml();
		}

        private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
		{
            GetBanner();
            bannerEnabled.Value = true;
			return HookResult.Continue;
		}

        private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            bannerEnabled.Value = false;
			return HookResult.Continue;
        }

        public void ScheduleBannerShow()
        {
            if (Config.ShowBannerAfterTime)
            {
                AddTimer(Config.ShowBannerAfterTimeSeconds, () =>
                {
                    Console.WriteLine("Banner enabled");
                    GetBanner();
                    bannerEnabled.Value = true;

                    AddTimer(8f, () =>
                    {
                        bannerEnabled.Value = false;

                    });

               }, TimerFlags.REPEAT);
            }
        }


        public void RegisterListeners()
		{
            RegisterListener<Listeners.OnTick>(OnTick);
            
            if (Config.ShowBannerWhenRoundEnd)
            {
                RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
                RegisterEventHandler<EventRoundStart>(OnRoundStart);
            }
         
            return ;
		}

        public void OnConfigParsed(PluginConfig config)
        {
            if (config.Version < Config.Version) Logger.LogWarning(Localizer["Banner.Console.ConfigVersionMismatch", Config.Version, config.Version]);

            Config = config;
        }

        private string GetBanner()
        {
            var bannerList = Config.Banners;
            if (bannerList.Length == 0) return string.Empty;

            _currentBanner = bannerList[_random.Next(bannerList.Length)];
            return _currentBanner;
        }
    }
}