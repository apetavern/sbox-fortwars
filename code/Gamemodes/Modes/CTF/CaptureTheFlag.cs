﻿namespace Fortwars;

public partial class CaptureTheFlag : Gamemode
{
	public override string GamemodeName => "Capture the Flag";

	/// <summary>
	/// Define the Game State's possible in Capture the Flag mode.
	/// </summary>
	public enum GameState
	{
		Lobby,
		Countdown,
		Build,
		Combat,
		GameOver
	}
	public override string GetGameStateLabel()
	{
		return CurrentState switch
		{
			GameState.Lobby => "Waiting for players",
			GameState.Countdown => "Game starting soon",
			GameState.Build => "Build Phase",
			GameState.Combat => "Combat Phase",
			GameState.GameOver => "Game Over!",
			_ => null,
		};
	}

	public override float GetTimeRemaining()
	{
		return TimeUntilNextState;
	}

	/// <summary>
	/// How long the countdown lasts before the game starts.
	/// </summary>
	public static float CountdownDuration => 15f;

	/// <summary>
	/// How long the build phase lasts.
	/// </summary>
	public static float BuildPhaseDuration => 90f;

	/// <summary>
	/// How long the combat phase lasts.
	/// </summary>
	public static float CombatPhaseDuration => 180f;

	/// <summary>
	/// How long the game over phase lasts before the game is restarted.
	/// </summary>
	public static float GameOverDuration => 10f;

	public override IEnumerable<Team> Teams
	{
		get
		{
			yield return Team.Red;
			yield return Team.Blue;
		}
	}

	[Net]
	public GameState CurrentState { get; set; }

	[Net]
	public TimeUntil TimeUntilNextState { get; set; }

	public override bool AllowDamage => CurrentState == GameState.Combat;

	internal override void Initialize()
	{
		_ = GameLoop();
	}

	protected async Task GameLoop()
	{
		CurrentState = GameState.Lobby;
		await WaitForPlayers();

		CurrentState = GameState.Countdown;
		await WaitAsync( CountdownDuration );

		CurrentState = GameState.Build;
		await WaitAsync( BuildPhaseDuration );

		CurrentState = GameState.Combat;
		await WaitAsync( CombatPhaseDuration );

		CurrentState = GameState.GameOver;
		await WaitAsync( GameOverDuration );
	}

	private async Task WaitForPlayers()
	{
		while ( PlayerCount < MinimumPlayers )
		{
			await Task.DelayRealtimeSeconds( 1f );
		}
	}

	private async Task WaitAsync( float time )
	{
		TimeUntilNextState = time;
		await Task.DelayRealtimeSeconds( time );
	}

	internal override void OnClientJoined( IClient client )
	{
		base.OnClientJoined( client );
	}

	/// <summary>
	/// PrepareLoadout for the CTF gamemode will load in the player's primary and secondary,
	/// plus the equipment from their chosen class.
	/// </summary>
	/// <param name="player">The player whose loadout we are preparing.</param>
	/// <param name="inventory">The inventory to add the items to.</param>
	internal override void PrepareLoadout( Player player, Inventory inventory )
	{
		inventory.AddWeapon(
			WeaponAsset.CreateInstance( WeaponAsset.FromPath( player.SelectedPrimaryWeapon ) ), true );
		inventory.AddWeapon(
			WeaponAsset.CreateInstance( WeaponAsset.FromPath( player.SelectedSecondaryWeapon ) ), false );

		if ( player.Class.Equipment == null )
			return;

		inventory.AddWeapon(
			WeaponAsset.CreateInstance( player.Class.Equipment ), false );
	}

	internal override void MoveToSpawnpoint( Entity pawn )
	{
		var spawnpoints = All.OfType<InfoPlayerTeamspawn>();
		var randomSpawn = spawnpoints.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

		if ( randomSpawn == null )
		{
			Log.Warning( "Couldn't find spawnpoint!" );
			return;
		}

		pawn.Position = randomSpawn.Position;
		pawn.Rotation = randomSpawn.Rotation;
	}
}