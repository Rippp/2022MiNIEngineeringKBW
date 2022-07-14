using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanamikojiClient.States
{
    public abstract class AbstractClientState
    {
        protected readonly TcpGameClient _client;

        public AbstractClientState(TcpGameClient client)
        {
            _client = client;
        }

        public abstract void EnterState();
        public abstract AbstractClientState? DoWork();
        public abstract void ExitState();
    }
}
