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

        public override AbstractServerState DoWork()
        {
            // Oczekiwanie na odpowiedź od obu klientow ze sa gotowi
            Console.WriteLine("Obecny stan: BeginRoundState");
            return new CurrentPlayerBeginTurnState(_game);
        }

        public override void EnterState()
        {
            _game.StartNewRound();
            _game.SendGameDataToPlayers();
        }

        public override void ExitState()
        {
            
        }
    }
}
