﻿// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

/// <summary>
/// When a player is within radius of the camera we add this to their entity.
/// We remove it again when they go out of range.
/// </summary>
internal class NameTagComponent : EntityComponent<FortwarsPlayer>
{
	NameTag NameTag;

	protected override void OnActivate()
	{
		NameTag = new NameTag( Entity.Client?.Name ?? Entity.Name, Entity.Client?.SteamId );
	}

	protected override void OnDeactivate()
	{
		NameTag?.Delete();
		NameTag = null;
	}

	/// <summary>
	/// Called for every tag, while it's active
	/// </summary>
	[Event.Client.Frame]
	public void FrameUpdate()
	{
		var tx = Entity.GetAttachment( "hat" ) ?? Entity.Transform;
		tx.Position += Vector3.Up * 10.0f;
		tx.Rotation = Rotation.LookAt( -Camera.Rotation.Forward );

		NameTag.SetClass( "visible", Entity?.TeamID == ( Game.LocalPawn as FortwarsPlayer )?.TeamID );

		NameTag.Transform = tx;

		NameTag.healthPanel.Style.Width = Length.Percent( Entity.Health );
		NameTag.WorldScale = Camera.Position.Distance( NameTag.Position ) * 0.005f;
		NameTag.WorldScale = NameTag.WorldScale.Clamp( 1f, 5f );
	}

	/// <summary>
	/// Called once per frame to manage component creation/deletion
	/// </summary>
	[Event.Client.Frame]
	public static void SystemUpdate()
	{
		foreach ( var player in Sandbox.Entity.All.OfType<FortwarsPlayer>() )
		{
			void Remove()
			{
				var c = player.Components.Get<NameTagComponent>();
				c?.Remove();
			}

			if ( player.IsLocalPawn && player.IsFirstPersonMode )
			{
				Remove();
				continue;
			}

			if ( player.Position.Distance( Camera.Position ) > 500 )
			{
				Remove();
				continue;
			}

			if ( player.LifeState != LifeState.Alive )
			{
				Remove();
				continue;
			}

			// Add a component if it doesn't have one
			player.Components.GetOrCreate<NameTagComponent>();
		}
	}
}
