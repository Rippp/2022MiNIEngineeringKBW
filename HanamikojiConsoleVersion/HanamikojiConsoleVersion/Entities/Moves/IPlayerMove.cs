using HanamikojiConsoleVersion.GameControl;

namespace HanamikojiConsoleVersion.Entities.Moves;

public interface IPlayerMove
{
    void Execute(Referee referee);
}
