using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        private const string STRADDLE = "Straddle";
        
        public void BeginStraddle()
        {
            Debug.LogFormat(LOG_FORMAT, nameof(BeginStraddle), "");
        }

        public void EndStraddle()
        {
            Debug.LogFormat(LOG_FORMAT, nameof(EndStraddle), ""); 
        }
    }
}
