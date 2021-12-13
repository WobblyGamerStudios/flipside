using System.Collections;
using UnityEngine;
using Wgs.Core;

namespace Wgs.FlipSide
{
    public class PlayerManager : Manager<PlayerManager>
    {
	    [SerializeField] private PlayerCharacter _playerPrefab;

	    public PlayerCharacter Player { get; private set; }
	    
	    protected override IEnumerator InitializeManager()
	    {
		    Player = Instantiate(_playerPrefab);
		    
		    return base.InitializeManager();
	    }
    }
}
