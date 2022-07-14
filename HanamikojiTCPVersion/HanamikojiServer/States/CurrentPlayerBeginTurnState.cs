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
        }

        public override AbstractServerState DoWork()
        {
            // wyslanie do current player "Make move"
            return null;
        }

        public override void ExitState()
        {
            
        }
    }
}
