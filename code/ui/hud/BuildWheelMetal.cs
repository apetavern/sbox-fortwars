﻿// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author support@apetavern.com

using Sandbox;
using System.Threading.Tasks;

namespace Fortwars;

    // TODO: we shouldn't need a separate wheel for this; implement scrollability in RadialWheel
    public partial class BuildWheelMetal : RadialWheel
    {
        public override Item[] Items => new Item[]
        {
            new ("3x2", "A wide panel good for defences", "/ui/icons/steel/fw_3x2.png"),
            new ("1x2", "A medium panel good for entrances", "/ui/icons/steel/fw_1x2.png"),
            new ("1x4", "A tall panel good for ledges", "/ui/icons/steel/fw_1x4.png"),
            new ("1x1x1", "A thicc block good for climbing", "/ui/icons/steel/fw_1x1x1.png"),
            new ("1x2x1", "A thicc block good for cover", "/ui/icons/steel/fw_1x2x1.png"),
        };

        public BuildWheelMetal() : base()
        {
            VirtualCursor.OnClick += OnClick;

            BindClass( "active", () => ( ( Input.UsingController && Input.Down( InputButton.Slot4 ) && Input.Down( InputButton.Attack2 ) ) // Gamepad compatible bind
                                    || ( !Input.UsingController && Input.Down( InputButton.Menu ) && Input.Down( InputButton.Use ) ) )
                                    && Game.Instance.Round is BuildRound );

        }

        private void OnClick()
        {
            if ( !HasClass( "active" ) )
            {
                return;
            }

            Game.Spawn( $"steel_{GetCurrentItem()?.Name}" ); // ???
            _ = ApplyShrinkEffect();
        }

        protected override void OnChange()
        {
            _ = ApplyShrinkEffect();
            PlaySound( "ui_tick" );
        }

        private async Task ApplyShrinkEffect()
        {
            Wrapper.AddClass( "shrink" );
            await Task.DelaySeconds( 0.050f );
            Wrapper.RemoveClass( "shrink" );
        }
    }
