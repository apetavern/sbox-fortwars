﻿// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

public class Dead : Panel
{
	Label timer;
	Label killer;

	public Dead()
	{
		StyleSheet.Load( "/ui/hud/Dead.scss" );
		Add.Label( "You are dead.", "title" );
		killer = Add.Label( "Killed by", "subtitle" );
		timer = Add.Label( "Respawn in 0:00", "timer" );

		BindClass( "visible", () => Game.LocalPawn.LifeState != LifeState.Alive );
	}

	public override void Tick()
	{
		base.Tick();

		if ( !IsVisible )
			return;

		if ( Game.LocalPawn is not FortwarsPlayer player )
			return;

		killer.Text = $"Killed by {player.Killer ?? "suicide"}";
		timer.Text = $"Respawn in {player.RespawnTimer.Relative.CeilToInt()}";
	}
}
