using CommonResources.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanamikojiServer.States
{
    public class BeginRoundState : AbstractServerState
    {
        public BeginRoundState(HanamikojiGame game) : base(game) { }

        public override void EnterState()
        {
            Console.WriteLine("Entered State: BeginRoundState");
            _game.StartNewRound();
            _game.SendGameDataToPlayers();
        }

        public override AbstractServerState? DoWork()
        {
            return new CurrentPlayerBeginTurnState(_game);
        }

        public override void ExitState()
        {
            
        }
    }
}
