using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace MyResource.Client
{
	internal class Script_context
	{
		public void PushClear(ulong address)
		{
			ScriptContext.Reset();
			ScriptContext.Push(address);
		}

		public string Result()
		{
			return ScriptContext.GetResult<string>();
		}
	}

	internal class ContextMemory
	{
		static Script_context script_Context = new Script_context();

		public static string MemResult(ulong address)
		{
			script_Context.PushClear(address);
			return script_Context.Result();
		}
	}

	public class ClientMain : BaseScript
	{
		public ClientMain()
		{
			
			string memStr = ReadAddr(0x28b82440000);
			
			Debug.WriteLine(string.Concat(new string[]
				{
					"read len ",
					memStr.Length.ToString(),
					" string '",
					memStr,
					"'"
				}));
		}

		public string ReadAddr(ulong address)
		{
			return Function.Call<string>(Hash.GET_CONVAR, address, address);
		}
	}
}