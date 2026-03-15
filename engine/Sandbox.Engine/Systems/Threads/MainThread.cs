using BlowoutTeamSoft.Engine;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;

namespace Sandbox;

/// <summary>
/// Utility functions that revolve around the main thread
/// </summary>
public static class MainThread
{
	static Channel<IDisposable> Disposables = Channel.CreateUnbounded<IDisposable>();
	static Channel<Action> Actions = Channel.CreateUnbounded<Action>();

	static TaskFactory s_Factory;

	private static CancellationTokenSource s_TokenSource = new CancellationTokenSource();

	/// <summary>
	/// Wait to execute on the main thread
	/// </summary>
	public static SyncTask Wait()
	{
		return new SyncTask( SyncContext.MainThread, allowSynchronous: true );
	}


	internal static void QueueDispose( IDisposable disposable )
	{
		if ( disposable is null )
			return;

		if ( ThreadSafe.IsMainThread )
		{
			disposable.Dispose();
			return;
		}

		Disposables.Writer.TryWrite( disposable );
	}

	/// <summary>
	/// Run a function on the main thread and wait for the result.
	/// </summary>
	internal static T Run<T>( int millisecondsTimeout, Func<T> func )
	{
		if ( ThreadSafe.IsMainThread )
			return func();

		T r = default;
		using var reset = new ManualResetEvent( false );

		Queue( () =>
		{
			try
			{
				r = func();
			}
			finally
			{
				reset.Set();
			}
		} );

		if ( !reset.WaitOne( millisecondsTimeout ) )
		{
			return default;
		}
		return r;
	}

	internal static void RunQueues()
	{
		ThreadSafe.AssertIsMainThread();

		while ( Disposables.Reader.TryRead( out var disposable ) )
		{
			disposable.Dispose();
		}

		RunMainThreadQueues();
	}

	/// <summary>
	/// When running in another thread you can queue a method to run in the main thread.
	/// If you are on the main thread we will execute the method immediately and return.
	/// </summary>
	public static void Queue( Action method )
	{
		if ( ThreadSafe.IsMainThread )
		{
			method();
			return;
		}

		Actions.Writer.TryWrite( method );
	}

	public static void Queue( Func<CancellationToken, ValueTask> methodAsync )
	{
		s_Factory ??= new TaskFactory( s_TokenSource.Token, TaskCreationOptions.None, TaskContinuationOptions.ExecuteSynchronously, new BlowoutSource2TaskScheduler() );
		s_Factory.StartNew( async () => await methodAsync( s_TokenSource.Token ), s_TokenSource.Token );
		return;
	}

	public static void ShutdownAsyncOperations()
	{
		s_TokenSource.Cancel();
		s_TokenSource.Dispose();
	}

	/// <summary>
	/// Run queued actions on the main thread
	/// </summary>
	internal static void RunMainThreadQueues()
	{
		ThreadSafe.AssertIsMainThread();

		while ( Actions.Reader.TryRead( out var action ) )
		{
			try
			{
				action();
			}
			catch ( System.Exception e )
			{
				Log.Warning( e, e.Message );
			}
		}
	}
}

//TODO: move to another file.
public sealed class BlowoutSource2TaskScheduler : TaskScheduler
{
	private readonly ConcurrentQueue<Task> _taskQueue = new ConcurrentQueue<Task>();
	private int _scheduled = 0;

	protected override IEnumerable<Task> GetScheduledTasks() =>
		// is array need??
		_taskQueue.ToArray();

	private void _ExecuteTasks()
	{
		_scheduled = 0;

		while ( _taskQueue.TryDequeue( out var task ) )
		{
			TryExecuteTask( task );
		}

		if ( !_taskQueue.IsEmpty && Interlocked.CompareExchange( ref _scheduled, 1, 0 ) == 0 )
		{
			MainThread.Queue( _ExecuteTasks );
		}
	}

	protected override void QueueTask( Task task )
	{
		_taskQueue.Enqueue( task );

		if ( Interlocked.CompareExchange( ref _scheduled, 1, 0 ) == 0 )
		{
			MainThread.Queue( _ExecuteTasks );
		}
	}

	protected override bool TryExecuteTaskInline( Task task, bool taskWasPreviouslyQueued )
	{
		if ( BlowoutEngine.ValidThreadSafe() )
		{
			return TryExecuteTask( task );
		}

		return false;
	}
}
