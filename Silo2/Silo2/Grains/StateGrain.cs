using System.Text.Json;
using Orleans.Runtime;
using Silo2.Abstractions;

namespace Silo2.Grains;

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