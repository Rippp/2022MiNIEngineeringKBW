using System;
using System.Threading;
using System.Threading.Tasks;
using HanamikojiClient;
using HanamikojiMonoGameClient.Managers;
using HanamikojiMonoGameClient.Managers.Moves;
using HanamikojiMonoGameClient.Managers.Moves.Helpers;
using HanamikojiMonoGameClient.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HanamikojiMonoGameClient
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
            => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureServices((_, services) => ConfigureServices(services));

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<GameWorker>();

            services.AddSingleton<PointedEntityProvider>();
            services.AddSingleton<IGame, MonoGameClient>();
            services.AddSingleton<ITcpGameClientProvider, TcpGameClientProvider>();
            services.AddSingleton<IEntitiesRepository, GameEntitiesRepository>();

            services.AddSingleton<TableManager>();
            services.AddSingleton<InputManager>();
            services.AddSingleton<IPointedCardAnimator, PointedCardAnimator>();

            services.AddSingleton<IMoveHandlerProvider, MoveHandlerProvider>();
            services.AddSingleton<SecretMoveHandler>();
            services.AddSingleton<EliminationMoveHandler>();
            services.AddSingleton<DoubleGiftMoveHandler>();
            services.AddSingleton<DoubleGiftOfferResponseMoveHandler>();
            services.AddSingleton<CardsOnHandSelector>();
            services.AddSingleton<ClickedEntityProvider>();
        }
    }
    
    public class GameWorker : IHostedService
    {
        private readonly IGame _game;
        private readonly IHostApplicationLifetime _appLifetime;

        public GameWorker(IGame game, IHostApplicationLifetime appLifetime)
        {
            _game = game;
            _appLifetime = appLifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);

            _game.Exiting += OnGameExiting;

            return Task.CompletedTask;
        }

        private void OnGameExiting(object sender, System.EventArgs e)
        {
            StopAsync(new CancellationToken());
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _appLifetime.StopApplication();

            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            _game.Run();
        }

        private void OnStopping()
        {
        }

        private void OnStopped()
        {
        }
    }
}