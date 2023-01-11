# Issue: POST requests on ARM64 hang until timeout 

## Description

Sending a POST request with a large payload (15 MB) from a Windows on ARM(64) machine to an http or https endpoint 
hangs until timeout no matter whether System.Net.Http.HttpClient or System.Net.WebRequest is used to issue the request.
On x64 machines it does not hang.
Another interesting fact is, that cUrl (for ARM64) also takes a very long time (> 4 minutes) whereas on x64 it only takes <10 seconds.
Maybe this is an OS level issue.

Note that the long delay/hang on ARM64 may also affect other HTTP methods.

## Reproduction Steps
The solution contains two projects:
- WebApp1
- ConsoleApp1

1. Adjust the IP address in the launch configuration of the web project WebApp1.csproj
2. Start WebApp1 on an x64 machine => Displays a targetUrl
3. Copy ConsoleApp1 to ARM machine
   1. Set the targetUrl in ConsoleApp1.Program.Main to the url from WebApp1
   2. Run ConsoleApp1

## Expected behavior
The POST request issued by ConsoleApp1 from Windows on ARM machines should take the same time as on x64.

## Actual behavior
The POST request issued by ConsoleApp1 on ARM machines hangs and times out. With a long timeout (several minutes), 
the request took 10 times longer (156s) on Windows on ARM compared to x64. On x64 it only takes 15s.

## Configuration
- .Net version: 7.0.102
- Client OS: Windows 11, Windows 10 Pro
- Server OS: Windows 11, Windows 10 Pro and Windows Server 2019
- Architecture:
  - WebApp1: x64
  - ConsoleApp1: ARM
- Specific to configuration: Yes

## Other Information:
Client Exception (Timeout):
System.Threading.Tasks.TaskCanceledException: The request was canceled due to the configured HttpClient.Timeout of 100 seconds elapsing.
---> System.TimeoutException: The operation was canceled.
---> System.Threading.Tasks.TaskCanceledException: The operation was canceled.
---> System.Net.Http.HttpRequestException: Error while copying content to a stream.
---> System.IO.IOException: Unable to write data to the transport connection: The I/O operation has been aborted because of either a thread exit or an application request..
---> System.Net.Sockets.SocketException (995): The I/O operation has been aborted because of either a thread exit or an application request.
--- End of inner exception stack trace ---

Server Exception:
fail: Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware[1]
An unhandled exception has occurred while executing the request.
Microsoft.AspNetCore.Connections.ConnectionResetException: An existing connection was forcibly closed by the remote host.
---> System.Net.Sockets.SocketException (10054): An existing connection was forcibly closed by the remote host.
--- End of inner exception stack trace ---
at System.IO.Pipelines.Pipe.GetReadResult(ReadResult& result)
at System.IO.Pipelines.Pipe.GetReadAsyncResult()
at Microsoft.AspNetCore.Server.Kestrel.Core.Internal.DuplexPipeStream.ReadAsyncInternal(Memory`1 destination, CancellationToken cancellationToken)
at System.Runtime.CompilerServices.PoolingAsyncValueTaskMethodBuilder`1.StateMachineBox`1.System.Threading.Tasks.Sources.IValueTaskSource<TResult>.GetResult(Int16 token)
at System.Net.Security.SslStream.EnsureFullTlsFrameAsync[TIOAdapter](CancellationToken cancellationToken)
at System.Runtime.CompilerServices.PoolingAsyncValueTaskMethodBuilder`1.StateMachineBox`1.System.Threading.Tasks.Sources.IValueTaskSource<TResult>.GetResult(Int16 token)
at System.Net.Security.SslStream.ReadAsyncInternal[TIOAdapter](Memory`1 buffer, CancellationToken cancellationToken)
at System.Runtime.CompilerServices.PoolingAsyncValueTaskMethodBuilder`1.StateMachineBox`1.System.Threading.Tasks.Sources.IValueTaskSource<TResult>.GetResult(Int16 token)
at System.IO.Pipelines.StreamPipeReader.<ReadAsync>g__Core|38_0(StreamPipeReader reader, CancellationTokenSource tokenSource, CancellationToken cancellationToken)
at System.Runtime.CompilerServices.PoolingAsyncValueTaskMethodBuilder`1.StateMachineBox`1.System.Threading.Tasks.Sources.IValueTaskSource<TResult>.GetResult(Int16 token)
at Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.Http1ContentLengthMessageBody.ReadAsyncInternal(CancellationToken cancellationToken)
at System.Runtime.CompilerServices.PoolingAsyncValueTaskMethodBuilder`1.StateMachineBox`1.System.Threading.Tasks.Sources.IValueTaskSource<TResult>.GetResult(Int16 token)
at Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpRequestStream.ReadAsyncInternal(Memory`1 destination, CancellationToken cancellationToken)
at System.Runtime.CompilerServices.PoolingAsyncValueTaskMethodBuilder`1.StateMachineBox`1.System.Threading.Tasks.Sources.IValueTaskSource<TResult>.GetResult(Int16 token)
at System.Text.Json.Serialization.ReadBufferState.ReadFromStreamAsync(Stream utf8Json, CancellationToken cancellationToken, Boolean fillBuffer)
at System.Text.Json.JsonSerializer.ReadFromStreamAsync[TValue](Stream utf8Json, JsonTypeInfo jsonTypeInfo, CancellationToken cancellationToken)
at Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonInputFormatter.ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
at Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonInputFormatter.ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
at Microsoft.AspNetCore.Mvc.ModelBinding.Binders.BodyModelBinder.BindModelAsync(ModelBindingContext bindingContext)
at Microsoft.AspNetCore.Mvc.ModelBinding.ParameterBinder.BindModelAsync(ActionContext actionContext, IModelBinder modelBinder, IValueProvider valueProvider, ParameterDescriptor parameter, ModelMetadata metadata,
Object value, Object container)
at Microsoft.AspNetCore.Mvc.Controllers.ControllerBinderDelegateProvider.<>c__DisplayClass0_0.<<CreateBinderDelegate>g__Bind|0>d.MoveNext()
--- End of stack trace from previous location ---
at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.<InvokeInnerFilterAsync>g__Awaited|13_0(ControllerActionInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isComplete
d)
at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeFilterPipelineAsync>g__Awaited|20_0(ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Logged|17_1(ResourceInvoker invoker)
at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Logged|17_1(ResourceInvoker invoker)
at Microsoft.AspNetCore.Routing.EndpointMiddleware.<Invoke>g__AwaitRequestTask|6_0(Endpoint end         at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Logged|17_1(Resource
Invoker invoker)
at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Logged|17_1(Resource
Invoker invoker)
at Microsoft.AspNetCore.Routing.EndpointMiddleware.<Invoke>g__AwaitRequestTask|6_0(Endpoint end
point, Task requestTask, ILogger logger)
at Microsoft.AspNetCore.Authorization.AuthorizationMiddleware.Invoke(HttpContext context)
at Microsoft.AspNetCore.Authentication.AuthenticationMiddleware.Invoke(HttpContext context)
at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddlewareImpl.Invoke(HttpContext context)

