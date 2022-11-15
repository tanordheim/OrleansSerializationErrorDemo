using System.Text.Json;
using Orleans.Runtime;

namespace Silo2.Grains;

public interface IStateGrain : IGrainWithStringKey
{
    Task<string> GetValue();
}

public class StateGrain : Grain, IStateGrain
{
    private readonly IPersistentState<Model> _state;

    public StateGrain([PersistentState("state")] IPersistentState<Model> state)
    {
        _state = state;
    }
    
    public Task<string> GetValue()
    {
        return Task.FromResult(JsonSerializer.Serialize(_state.State));
    }
}