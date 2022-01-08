using System;
using Wgs.Core;

namespace Wgs.FlipSide
{
    public class PlayerManager : Manager<PlayerManager>
    {
	    public static PlayerCharacter Player { get; private set; }
	    
	    public static void RegisterPlayer(PlayerCharacter player)
	    {
		    if (Player != player)
		    {
			    UnRegisterPlayer(Player);
		    }

		    Player = player;
		    
		    DontDestroyOnLoad(Player);
		    
		    OnPlayerRegisteredEvent?.Invoke(player);
	    }

	    public static void UnRegisterPlayer(PlayerCharacter player)
	    {
		    OnPlayerUnRegisteredEvent?.Invoke(player);

		    if (!player) return;
		    
		    Destroy(player.gameObject);
	    }

	    #region Events

	    public static event Action<PlayerCharacter> OnPlayerRegisteredEvent;
	    public static event Action<PlayerCharacter> OnPlayerUnRegisteredEvent;

	    #endregion Events
    }
}
