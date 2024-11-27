using MasterDevs.ChromeDevTools.Protocol.Chrome.Page;
using MasterDevs.ChromeDevTools.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace MasterDevs.ChromeDevTools
{
    public  class ChromeSession : IChromeSession
    {
        private readonly ConcurrentDictionary<string, ConcurrentBag<Action<object>>> _handlers = new ConcurrentDictionary<string, ConcurrentBag<Action<object>>>();
        private ICommandFactory _commandFactory;
        private IEventFactory _eventFactory;
        private ManualResetEvent _openEvent = new ManualResetEvent(false);
        private ManualResetEvent _publishEvent = new ManualResetEvent(false);
        private ConcurrentDictionary<long, ManualResetEventSlim> _requestWaitHandles = new ConcurrentDictionary<long, ManualResetEventSlim>();
        private ICommandResponseFactory _responseFactory;
        private ConcurrentDictionary<long, ICommandResponse> _responses = new ConcurrentDictionary<long, ICommandResponse>();
        private WebSocket _webSocket;
        private object _lock = new object();

        public readonly string Endpoint;
        public bool Disposed { get; private set; }
        public event Action<string> UnknownMessageReceived;
        public event Action<byte[]> UnknownDataReceived;

        public ChromeSession(string endpoint, ICommandFactory commandFactory, ICommandResponseFactory responseFactory, IEventFactory eventFactory)
        {
            Disposed = false;
            Endpoint = endpoint;
            _commandFactory = commandFactory;
            _responseFactory = responseFactory;
            _eventFactory = eventFactory;
        }
        public static void AddEvaluateScriptOnNewPage(ChromeSession chromeSession, string script, Dictionary<ChromeSession, AddScriptToEvaluateOnNewDocumentCommandResponse> evaluatedScriptsInfo)
        {
            if (chromeSession.Disposed)
                return;

            if (evaluatedScriptsInfo.TryGetValue(chromeSession, out var scriptInfo))
            {
                _ = chromeSession.SendAsync(new RemoveScriptToEvaluateOnNewDocumentCommand() { Identifier = scriptInfo.Identifier });
                evaluatedScriptsInfo.Remove(chromeSession);
            }

            var addScriptInfo = chromeSession.SendAsync(new AddScriptToEvaluateOnNewDocumentCommand() { Source = script });
            addScriptInfo.Wait();
            if (addScriptInfo.Result.Result == null)
            {
                return;
            }
            evaluatedScriptsInfo.Add(chromeSession, addScriptInfo.Result.Result);
        }
        public void Dispose()
        {
            if (Disposed)
            {
                return;
            }

            Disposed = true;

            if (null == _webSocket) return;

            _webSocket.OnOpen -= WebSocket_Opened;
            _webSocket.OnMessage -= WebSocket_MessageReceived;
            _webSocket.OnError -= WebSocket_Error;
            _webSocket.OnClose -= WebSocket_Closed;

            _webSocket.CloseAsync();
            _webSocket = null;
            //_webSocket.D();
        }

        private void EnsureInit()
        {
            if (!Disposed && (null == _webSocket || _webSocket.ReadyState != WebSocketState.Open))
            {
                lock (_lock)
                {
                    if (null == _webSocket || _webSocket.ReadyState != WebSocketState.Open)
                    {
                        Init().Wait();
                    }
                }
            }
        }

        private Task Init()
        {
            _openEvent.Reset();

            if (_webSocket != null)
            {
                Dispose();
            }

            _webSocket = new WebSocket(Endpoint);
            _webSocket.EmitOnPing = false;
            _webSocket.OnOpen += WebSocket_Opened;
            _webSocket.OnMessage += WebSocket_MessageReceived;
            _webSocket.OnError += WebSocket_Error;
            _webSocket.OnClose += WebSocket_Closed;

            _webSocket.Connect();

            return Task.Run(() =>
            {
                _openEvent.WaitOne();
            });
        }

        public Task<ICommandResponse> SendAsync<T>(CancellationToken cancellationToken)
        {
            var command = _commandFactory.Create<T>();
            return SendCommand(command, cancellationToken);
        }

        public Task<CommandResponse<T>> SendAsync<T>(ICommand<T> parameter, CancellationToken cancellationToken)
        {
            var command = _commandFactory.Create(parameter);
            var task = SendCommand(command, cancellationToken);
            return CastTaskResult<ICommandResponse, CommandResponse<T>>(task);
        }

        private Task<TDerived> CastTaskResult<TBase, TDerived>(Task<TBase> task) where TDerived : TBase, new()
        {
            var tcs = new TaskCompletionSource<TDerived>();
            task.ContinueWith(t => {
                if (t.Result is TDerived)
                    tcs.SetResult((TDerived)t.Result);
                else
                    tcs.SetResult(new TDerived());
                //if (t.Result is TDerived)
                //    tcs.SetResult((TDerived)t.Result);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
            task.ContinueWith(t => tcs.SetException(t.Exception.InnerExceptions),
                TaskContinuationOptions.OnlyOnFaulted);
            task.ContinueWith(t => tcs.SetCanceled(),
                TaskContinuationOptions.OnlyOnCanceled);
            return tcs.Task;
        }

        public void Subscribe<T>(Action<T> handler) where T : class
        {
            var handlerType = typeof(T);
            var handlerForBag = new Action<object>(obj => handler((T)obj));
            _handlers.AddOrUpdate(handlerType.FullName,
                (m) => new ConcurrentBag<Action<object>>(new[] { handlerForBag }),
                (m, currentBag) =>
                {
                    currentBag.Add(handlerForBag);
                    return currentBag;
                });
        }

        private void HandleEvent(IEvent evnt)
        {
            if (null == evnt
                || null == evnt)
            {
                return;
            }
            var type = evnt.GetType().GetGenericArguments().FirstOrDefault();
            if (null == type)
            {
                return;
            }
            var handlerKey = type.FullName;
            ConcurrentBag<Action<object>> handlers = null;
            if (_handlers.TryGetValue(handlerKey, out handlers))
            {
                var localHandlers = handlers.ToArray();
                foreach (var handler in localHandlers)
                {
                    ExecuteHandler(handler, evnt);
                }
            }
        }

        private void ExecuteHandler(Action<object> handler, dynamic evnt)
        {
            if (evnt.GetType().GetGenericTypeDefinition() == typeof(Event<>))
            {
                handler(evnt.Params);
            }
            else
            {
                handler(evnt);
            }
        }

        private void HandleResponse(ICommandResponse response)
        {
            if (null == response) return;
            ManualResetEventSlim requestMre;
            if (_requestWaitHandles.TryGetValue(response.Id, out requestMre))
            {
                _responses.AddOrUpdate(response.Id, id => response, (key, value) => response);
                requestMre.Set();
            }
            else
            {
                // in the case of an error, we don't always get the request Id back :(
                // if there is only one pending requests, we know what to do ... otherwise
                if (1 == _requestWaitHandles.Count)
                {
                    var requestId = _requestWaitHandles.Keys.First();
                    _requestWaitHandles.TryGetValue(requestId, out requestMre);
                    _responses.AddOrUpdate(requestId, id => response, (key, value) => response);
                    requestMre.Set();
                }
            }
        }

        private Task<ICommandResponse> SendCommand(Command command, CancellationToken cancellationToken)
        {
            if (Disposed)
            {
                return null;
            }

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new MessageContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
            };
            var requestString = JsonConvert.SerializeObject(command, settings);
            var requestResetEvent = new ManualResetEventSlim(false);
            _requestWaitHandles.AddOrUpdate(command.Id, requestResetEvent, (id, r) => requestResetEvent);
            return Task.Run(() =>
            {
                EnsureInit();
                _webSocket.Send(requestString);
                requestResetEvent.Wait(cancellationToken);
                ICommandResponse response = null;
                _responses.TryRemove(command.Id, out response);
                _requestWaitHandles.TryRemove(command.Id, out requestResetEvent);
                return response;
            });
        }

        private bool TryGetCommandResponse(byte[] data, out ICommandResponse response)
        {
            response = _responseFactory.Create(data);
            return null != response;
        }

        private bool TryGetCommandResponse(string message, out ICommandResponse response)
        {
            response = _responseFactory.Create(message);
            return null != response;
        }

        private bool TryGetEvent(byte[] data, out IEvent evnt)
        {
            evnt = _eventFactory.Create(data);
            return null != evnt;
        }

        private bool TryGetEvent(string message, out IEvent evnt)
        {
            evnt = _eventFactory.Create(message);
            return null != evnt;
        }

        private void TryMessageReceivedBytes(byte[] data)
        {
            ICommandResponse response;
            if (TryGetCommandResponse(data, out response))
            {
                HandleResponse(response);
                return;
            }
            IEvent evnt;
            if (TryGetEvent(data, out evnt))
            {
                HandleEvent(evnt);
                return;
            }
            UnknownDataReceived?.Invoke(data);
        }

        private void TryMessageReceivedString(string data)
        {
            ICommandResponse response;
            if (TryGetCommandResponse(data, out response))
            {
                HandleResponse(response);
                return;
            }
            IEvent evnt;
            if (TryGetEvent(data, out evnt))
            {
                HandleEvent(evnt);
                return;
            }
            UnknownMessageReceived?.Invoke(data);

        }

        private void WebSocket_Closed(object sender, EventArgs e)
        {
            //Dispose();
        }

        private void WebSocket_Error(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            //throw e.Exception;
        }

        private void WebSocket_MessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                if (e.IsBinary)
                {
                    TryMessageReceivedBytes(e.RawData);
                    return;
                }
                TryMessageReceivedString(e.Data);
            }
            catch { }
        }

        private void WebSocket_Opened(object sender, EventArgs e)
        {
            _openEvent.Set();
        }
    }
}