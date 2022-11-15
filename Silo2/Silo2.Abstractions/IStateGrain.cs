namespace Silo2.Abstractions;

public interface IStateGrain : IGrainWithStringKey
{
    Task<string> GetValue();
}