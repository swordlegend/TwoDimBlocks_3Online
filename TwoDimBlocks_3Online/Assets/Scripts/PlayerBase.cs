using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwoDimBlocks
{
    public abstract class PlayerBase
    {
        protected PlayerSyncData latest, previous;
        public bool updated = true;

        public PlayerBase(PlayerSyncData sync)
        {
            latest = sync;
            previous = sync;
        }

        public void ReceiveLatestData(PlayerSyncData sync)
        {
            updated = true;

            previous = latest;
            latest = sync;
        }

        public void ResetUpdateFlag()
        {
            updated = false;
        }
    }
}
