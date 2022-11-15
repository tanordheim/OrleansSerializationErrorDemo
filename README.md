# Orleans serialization issue

This showcases an issue with Orleans 7 serialization when running multiple solutions locally with project reference dependencies between them; this is not a good developer setup, just one I happened to use while prototyping some services and this tripped me up for a while.

Essentially:

- `Silo1/` contains a solution with a simple Orleans silo, and also has an `Silo1.Abstractions` class library that contains a `public abstract record BaseModel {}` type which contains some base properties for a state model. This is annotated with Orleans serializer attributes.
- `Silo2/` contains a solution with a simple Orleans silo, which has a `<ProjectReference />` dependency on the `Silo1.Abstractions` class library from `Silo1`. Ideally this would be a NuGet dependency, but isn't right now.

If starting the project like this:

```
dotnet run --project Silo2/Silo2/Silo2.csproj
```

Then everything works fine; hit enter when starting the service and it will show the data being returned from the grain call as expected.

However, if starting both projects like this (in different terminals) - it maybe important that Silo2 is started first:

```
dotnet run --project Silo2/Silo2/Silo2.csproj
dotnet run --project Silo1/Silo1/Silo1.csproj
```

Then I guess the `Abstractions` class library is recompiled, and the codegen for the serializations runs again and causes some incompatibilities. If hitting enter on the Silo2 terminal now, the grain call fails with the following error:

```
Unhandled exception. Orleans.Runtime.OrleansException: Error from storage provider MemoryGrainStorage.state during ReadState for grain Type=state Id=test Error=

Exc level 0: Orleans.Serialization.UnsupportedWireTypeException: A WireType value of Reference is expected by this codec. [TagDelimited, IdDelta:0, SchemaType:Encoded, RuntimeType:]
   at Orleans.Serialization.WireProtocol.Field.UnsupportedWireType(WireType expectedType) in /_/src/Orleans.Serialization/WireProtocol/Field.cs:line 245
   at Orleans.Serialization.Codecs.VoidCodec.ReadValue[TInput](Reader`1& reader, Field field) in /_/src/Orleans.Serialization/Codecs/VoidCodec.cs:line 26
   at Orleans.Runtime.Messaging.MessageSerializer.ReadBodyObject[TInput](Message message, Reader`1& reader) in /_/src/Orleans.Core/Messaging/MessageSerializer.cs:line 141
   at Orleans.Runtime.Messaging.MessageSerializer.TryRead(ReadOnlySequence`1& input, Message& message) in /_/src/Orleans.Core/Messaging/MessageSerializer.cs:line 109 at Orleans.Runtime.Messaging.Connection.ProcessIncoming() in /_/src/Orleans.Core/Networking/Connection.cs:line 304
--- End of stack trace from previous location ---
   at Orleans.Serialization.Invocation.ResponseCompletionSource`1.GetResult(Int16 token) in /_/src/Orleans.Serialization/Invocation/ResponseCompletionSource.cs:line 230
   at System.Threading.Tasks.ValueTask`1.ValueTaskSourceAsTask.<>c.<.cctor>b__4_0(Object state)
--- End of stack trace from previous location ---
   at Orleans.Storage.MemoryGrainStorage.ReadStateAsync[T](String grainType, GrainId grainId, IGrainState`1 grainState) in /_/src/Orleans.Persistence.Memory/Storage/MemoryStorage.cs:line 73
   at Orleans.Core.StateStorageBridge`1.ReadStateAsync() in /_/src/Orleans.Runtime/Storage/StateStorageBridge.cs:line 68
 ---> Orleans.Serialization.UnsupportedWireTypeException: A WireType value of Reference is expected by this codec. [TagDelimited, IdDelta:0, SchemaType:Encoded, RuntimeType:]
   at Orleans.Serialization.WireProtocol.Field.UnsupportedWireType(WireType expectedType) in /_/src/Orleans.Serialization/WireProtocol/Field.cs:line 245
   at Orleans.Serialization.Codecs.VoidCodec.ReadValue[TInput](Reader`1& reader, Field field) in /_/src/Orleans.Serialization/Codecs/VoidCodec.cs:line 26
   at Orleans.Runtime.Messaging.MessageSerializer.ReadBodyObject[TInput](Message message, Reader`1& reader) in /_/src/Orleans.Core/Messaging/MessageSerializer.cs:line 141
   at Orleans.Runtime.Messaging.MessageSerializer.TryRead(ReadOnlySequence`1& input, Message& message) in /_/src/Orleans.Core/Messaging/MessageSerializer.cs:line 109
   at Orleans.Runtime.Messaging.Connection.ProcessIncoming() in /_/src/Orleans.Core/Networking/Connection.cs:line 304
--- End of stack trace from previous location ---
   at Orleans.Serialization.Invocation.ResponseCompletionSource`1.GetResult(Int16 token) in /_/src/Orleans.Serialization/Invocation/ResponseCompletionSource.cs:line 230
   at System.Threading.Tasks.ValueTask`1.ValueTaskSourceAsTask.<>c.<.cctor>b__4_0(Object state)
--- End of stack trace from previous location ---
   at Orleans.Storage.MemoryGrainStorage.ReadStateAsync[T](String grainType, GrainId grainId, IGrainState`1 grainState) in /_/src/Orleans.Persistence.Memory/Storage/MemoryStorage.cs:line 73
   at Orleans.Core.StateStorageBridge`1.ReadStateAsync() in /_/src/Orleans.Runtime/Storage/StateStorageBridge.cs:line 68
   --- End of inner exception stack trace ---
   at Orleans.Core.StateStorageBridge`1.ReadStateAsync() in /_/src/Orleans.Runtime/Storage/StateStorageBridge.cs:line 79
   at Orleans.LifecycleSubject.OnStart(CancellationToken cancellationToken) in /_/src/Orleans.Core/Lifecycle/LifecycleSubject.cs:line 118
   at Orleans.Internal.OrleansTaskExtentions.MakeCancellable(Task task, CancellationToken cancellationToken) in /_/src/Orleans.Core/Async/TaskExtensions.cs:line 188
   at Orleans.Internal.OrleansTaskExtentions.WithCancellation(Task taskToComplete, CancellationToken cancellationToken, String message) in /_/src/Orleans.Core/Async/TaskExtensions.cs:line 145
   at Orleans.Runtime.ActivationData.<ActivateAsync>g__CallActivateAsync|132_0(Dictionary`2 requestContextData, CancellationToken cancellationToken) in /_/src/Orleans.Runtime/Catalog/ActivationData.cs:line 1180
   at Orleans.Serialization.Invocation.ResponseCompletionSource`1.GetResult(Int16 token) in /_/src/Orleans.Serialization/Invocation/ResponseCompletionSource.cs:line 230
   at System.Threading.Tasks.ValueTask`1.ValueTaskSourceAsTask.<>c.<.cctor>b__4_0(Object state)
--- End of stack trace from previous location ---
   at Program.<Main>$(String[] args) in /Users/trond/Desktop/OrleansSerializationErrorDemo/Silo2/Silo2/Program.cs:line 26
   at Program.<Main>(String[] args)
```
