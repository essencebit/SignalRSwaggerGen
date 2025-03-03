using SignalRSwaggerGen.Attributes;
using SignalRSwaggerGen.Enums;
using System;
using System.Threading.Tasks;

namespace TestWebApi.Hubs
{
	[SignalRHub]
	public interface IInterface : IBase1, IBase2
	{
		public Task<bool> Realy(string affirmation);
	}

	public interface IBase1 : IBase2
	{
		public void Works1();
		public int Works2(int x, string[] s);
		public void Works3();
	}

	public interface IBase2 : IBase3
	{
		public void Works2(int x, string[] s);
	}

	public interface IBase3 : IAsyncDisposable, IDisposable
	{
		public void Works2();
		public void Works3();
		public bool Bool { get; }
	}

	public interface IDno : IDisposable, IAsyncDisposable
	{
		public bool Bool { get; }
		public int Dno();
	}

	public class SuperBase
	{
		public void Dno()
		{
		}
	}

	public class Base : SuperBase
	{
		public bool Bool { get; }
		public void Inherit()
		{
		}
	}

	[SignalRHub()]
	public class Class : Base, IDno
	{
		public bool Bool => throw new System.NotImplementedException();

		public Task<bool> Realy(string affirmation)
		{
			throw new System.NotImplementedException();
		}

		public new int Dno()
		{
			return 0;
		}

		[SignalRHidden]
		public void Dispose()
		{
			throw new NotImplementedException();
		}

		[SignalRHidden]
		public ValueTask DisposeAsync()
		{
			throw new NotImplementedException();
		}
	}
}
