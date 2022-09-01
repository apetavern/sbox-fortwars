﻿// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fortwars;

public abstract class Class
{
	public virtual string Name { get; set; }
	public virtual string Description { get; set; }
	public virtual string ShortDescription { get; set; }
	public virtual string IconPath { get; set; }
	public virtual List<string> CombatLoadout { get; set; }
	public virtual List<string> BuildLoadout { get; set; }
	public virtual List<string> Cosmetics { get; set; }
	public virtual string PreviewWeapon { get; set; }
	public virtual HoldTypes PreviewHoldType { get; set; }
	public virtual HoldHandedness PreviewHoldHandedness { get; set; } = HoldHandedness.TwoHands;
	public virtual float PreviewHandpose { get; set; } = 0f;

	public virtual void AssignBuildLoadout( Inventory inventory )
	{
		AssignLoadout( BuildLoadout, inventory );
	}

	public virtual void AssignCombatLoadout( Inventory inventory )
	{
		AssignLoadout( CombatLoadout, inventory );
	}

	private async void AssignLoadout( List<string> items, Inventory inventory )
	{
		for ( int i = 0; i < items.Count; i++ )
		{
			string itemPath = items[i];
			inventory.Add( ItemUtils.GetItem( itemPath ), i == 0 );
			await Task.Delay( 100 ); //Gotta wait between each weapon added so OnChildAdded gets fired in the correct order...
		}
	}

	public virtual void Cleanup( Inventory inventory )
	{
		inventory.DeleteContents();
	}
}
