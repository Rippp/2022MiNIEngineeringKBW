using CommonResources.Game;
using CommonResources.Game.Constants;
using CommonResources.Network;

namespace HanamikojiServer.States
{
    public class EndRoundState : AbstractServerState
    {
        private PlayerData _currentPlayerData;
        private PlayerData _otherPlayerData;
        public EndRoundState(HanamikojiGame game) : base(game) { }

        public override void EnterState()
        {
            Console.WriteLine("Entered State: EndRoundState");
            _currentPlayerData = _game.GetCurrentPlayerData();
            _otherPlayerData = _game.GetOtherPlayerData();
        }

        public override AbstractServerState? DoWork()
        {
            if (CheckIfEndOfTheGame(out var convincedToCurrentPlayer, out var convincedToOtherPlayer))
            {
                _game.SendToCurrentPlayer(PacketCommandEnum.Message, "Game has ended.");
                _game.SendToOtherPlayer(PacketCommandEnum.Message, "Game has ended.");
                _game.StopGame("Game ended");
                return null;
            }
            else
            {
                SaveConvincedGeisha(convincedToCurrentPlayer, convincedToOtherPlayer);
                return new BeginRoundState(_game);
            }
        }

        public override void ExitState() { }

        private bool CheckIfEndOfTheGame(out IReadOnlyList<GeishaType> convincedToCurrentPlayer, 
            out IReadOnlyList<GeishaType> convincedToOtherPlayer)
        {
            var geishaScore = new Dictionary<GeishaType, (int currentPlayerScore, int OtherPlayerScore)>();

            foreach (var geishaType in Enum.GetValues<GeishaType>())
            {
                var currentPlayerScore = _currentPlayerData.CountPointsForGeishaType(geishaType);
                var otherPlayerScore = _otherPlayerData.CountPointsForGeishaType(geishaType);

                currentPlayerScore += _currentPlayerData.SecretCard?.Type == geishaType ? 1 : 0;
                otherPlayerScore += _otherPlayerData.SecretCard?.Type == geishaType ? 1 : 0;

                geishaScore.Add(geishaType, (currentPlayerScore, otherPlayerScore));
            }

            convincedToCurrentPlayer = geishaScore.Where(x => x.Value.currentPlayerScore > x.Value.OtherPlayerScore).Select(x => x.Key).ToList();
            convincedToOtherPlayer = geishaScore.Where(x => x.Value.currentPlayerScore < x.Value.OtherPlayerScore).Select(x => x.Key).ToList();

            var currentPlayerPoints = convincedToCurrentPlayer.Select(x => GeishaConstants.GeishaPoints[x]).Sum();
            var otherPlayerPoints = convincedToOtherPlayer.Select(x => GeishaConstants.GeishaPoints[x]).Sum();

            const int pointsToWin = 11;
            const int convincedGeishaToWin = 4;

            return (currentPlayerPoints > pointsToWin ||
                    otherPlayerPoints > pointsToWin ||
                    convincedToCurrentPlayer.Count() >= convincedGeishaToWin ||
                    convincedToOtherPlayer.Count() >= convincedGeishaToWin);
        }

        private void SaveConvincedGeisha(IReadOnlyList<GeishaType> convincedToCurrentPlayer, IReadOnlyList<GeishaType> convincedToOtherPlayer)
        {
            _currentPlayerData.ClearData();
            _currentPlayerData.ConvincedGeishasInPreviousRound.AddRange(convincedToCurrentPlayer);

            _otherPlayerData.ClearData();
            _otherPlayerData.ConvincedGeishasInPreviousRound.AddRange(convincedToOtherPlayer);
        }
    }
}
