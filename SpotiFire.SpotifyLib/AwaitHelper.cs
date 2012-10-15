using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpotiFire.SpotifyLib
{
    static class AwaitHelper
    {
        internal static void OnComplete(ISpotifyAwaitable awaitable, Action continuation, bool continueOnCapturedContext, bool flowExecutionContext)
        {
            Continuation _continuation = null;
            if (continuation != null)
            {
                if (continueOnCapturedContext)
                {
                    SynchronizationContext synchronizationContext = SynchronizationContext.Current;
                    if (synchronizationContext == null)
                    {
                        /*do nothing*/
                    }
                    else
                    {
                        _continuation = new SynchronizationContextContinuation(synchronizationContext, continuation, flowExecutionContext);
                    }
                }

                if (_continuation == null && flowExecutionContext)
                {
                    _continuation = new Continuation(continuation, true);
                }

                if (_continuation == null)
                {
                    if (!awaitable.AddContinuation(new Continuation(continuation, false), false))
                    {
                        UnsafeScheduleAction(continuation);
                    }
                }
                else
                {
                    if (!awaitable.AddContinuation(_continuation, false))
                    {
                        _continuation.Run(awaitable, false);
                    }
                }
            }
            else
            {
                throw new ArgumentNullException("continuation");
            }
        }

        internal static ISpotifyAwaiter<T> GetAwaiter<T>(T value)
            where T : ISpotifyObject
        {
            return new AwaitableAwaiter<T>(value);
        }

        internal class Continuation
        {
            [SecurityCritical]
            static ContextCallback _invokeActionCallback;

            readonly protected Action _action;
            readonly protected bool _flowExecutionContext;
            readonly protected ExecutionContext _capturedContext;

            [SecurityCritical]
            protected static ContextCallback GetInvokeActionCallback()
            {
                ContextCallback sInvokeCallback = _invokeActionCallback;
                if (sInvokeCallback == null)
                {
                    ContextCallback contextCallback = new ContextCallback(InvokeAction);
                    sInvokeCallback = contextCallback;
                    _invokeActionCallback = contextCallback;
                }
                return sInvokeCallback;
            }

            static void InvokeAction(object state)
            {
                ((Action)state)();
            }

            public Continuation (Action action, bool flowExecutionContext)
            {
                _action = action;
                _flowExecutionContext = flowExecutionContext;
                if (flowExecutionContext)
                    _capturedContext = ExecutionContext.Capture();
            }

            public virtual void WaitCallback(object state)
            {
                throw new NotImplementedException();
            }


            public virtual void Run(ISpotifyAwaitable awaitable, bool canInlineContinueTask)
            {
                RunCallback(GetInvokeActionCallback(), _action);
            }

            [SecurityCritical]
            protected void RunCallback(ContextCallback callback, object state)
            {
                try
                {
                    if (_capturedContext != null)
                        ExecutionContext.Run(_capturedContext, callback, state);
                    else
                        callback(state);
                }
                catch (Exception e)
                {
                    if (e as ThreadAbortException == null || e as AppDomainUnloadedException == null)
                    {
                        ExceptionDispatchInfo info = ExceptionDispatchInfo.Capture(e);
                        ThreadPool.QueueUserWorkItem((object s) => ((ExceptionDispatchInfo)s).Throw(), info);
                    }
                }
                finally
                {
                    if (_capturedContext != null)
                        _capturedContext.Dispose();
                }
            }
        }

        sealed class SynchronizationContextContinuation : Continuation
        {
            readonly SynchronizationContext _synchronizationContext;

            static readonly SendOrPostCallback _postCallback;

            [SecurityCritical]
            static ContextCallback _postActionCallback;

            public SynchronizationContextContinuation (SynchronizationContext synchronizationContext, Action action, bool flowExecutionContext)
                : base(action, flowExecutionContext)
            {
                _synchronizationContext = synchronizationContext;
            }

            static SynchronizationContextContinuation()
            {
                _postCallback = (object state) => ((Action)state)();
            }

            public override void WaitCallback(object state)
            {
                throw new NotImplementedException();
            }

            [SecurityCritical]
            static ContextCallback GetPostActionCallback()
            {
                ContextCallback sPostActionCallback = _postActionCallback;
                if (sPostActionCallback == null)
                {
                    ContextCallback contextCallback = new ContextCallback(PostAction);
                    sPostActionCallback = contextCallback;
                    _postActionCallback = contextCallback;
                }
                return sPostActionCallback;
            }

            [SecurityCritical]
            static void PostAction(object state)
            {
                SynchronizationContextContinuation context = (SynchronizationContextContinuation)state;
                context._synchronizationContext.Post(_postCallback, context._action);
            }

            [SecurityCritical]
            public override void Run(ISpotifyAwaitable awaitable, bool canInlineContinueTask)
            {
                if (!canInlineContinueTask || _synchronizationContext != SynchronizationContext.Current)
                    RunCallback(GetPostActionCallback(), this);
                else
                    RunCallback(GetInvokeActionCallback(), _action);
            }
        }

        static void UnsafeScheduleAction(Action action)
        {
            ThreadPool.UnsafeQueueUserWorkItem(new Continuation(action, false).WaitCallback, null);
        }

        struct AwaitableAwaiter<T> : ISpotifyAwaiter<T>
            where T : ISpotifyObject
        {
            T result;
            ISpotifyAwaitable awaitable;

            public AwaitableAwaiter(T value)
            {
                result = value;
                awaitable = null;
            }

            ISpotifyAwaitable Awaitable
            {
                get
                {
                    if (awaitable == null)
                        Interlocked.CompareExchange(ref awaitable, (ISpotifyAwaitable)result, null);
                    return awaitable;
                }
            }

            public bool IsCompleted
            {
                get { return Awaitable.IsComplete; }
            }

            public T GetResult()
            {
                return result;
            }

            public void OnCompleted(Action continuation)
            {
                AwaitHelper.OnComplete(Awaitable, continuation, true, true);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                AwaitHelper.OnComplete(Awaitable, continuation, true, false);
            }
        }
    }
}
