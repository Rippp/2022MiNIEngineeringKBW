using CommonResources.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanamikojiServer.States
{
    public class CurrentPlayerBeginTurnState : AbstractServerState
    {
        public CurrentPlayerBeginTurnState(HanamikojiGame game) : base(game) { }
        public override void EnterState()
        {
            Console.WriteLine("Entered State: CurrentPlayerBeginTurnState");
            
            _game.SendToCurrentPlayer(PacketCommandEnum.MakeMove);
        }

        public override AbstractServerState DoWork()
        {
            return new AwaitCurrentPlayerMoveState(_game);
        }

        public override void ExitState()
        {
            
        }
    }
}
