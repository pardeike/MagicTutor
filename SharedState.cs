using System;
using System.Reflection;
using System.Reflection.Emit;
using FieldAttributes = System.Reflection.FieldAttributes;
using TypeAttributes = System.Reflection.TypeAttributes;

namespace Brrainz
{
	internal delegate ref object HintDelegate();

	internal class SharedState
	{
		internal static HintDelegate GetDelegate()
		{
			Type type = Type.GetType("MagicTutorState", false);
			Console.WriteLine($"type={type}");
			if (type == null)
			{
				var myCurrentDomain = AppDomain.CurrentDomain;
				var myAssemblyName = new AssemblyName() { Name = "MagicTutorState" };
				var myAssemblyBuilder = myCurrentDomain.DefineDynamicAssembly(myAssemblyName, AssemblyBuilderAccess.Run);
				var myModuleBuilder = myAssemblyBuilder.DefineDynamicModule("MagicTutorModule");
				var myTypeBuilder = myModuleBuilder.DefineType("MagicTutorState", TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.Class, typeof(object));
				var currentHintField = myTypeBuilder.DefineField("currentHint", typeof(object), FieldAttributes.Public | FieldAttributes.Static);
				var myMethodBuilder = myTypeBuilder.DefineMethod("GetRef", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(object).MakeByRefType(), new Type[0]);
				var myGenerator = myMethodBuilder.GetILGenerator();
				myGenerator.Emit(OpCodes.Ldsflda, currentHintField);
				myGenerator.Emit(OpCodes.Ret);
				type = myTypeBuilder.CreateType();
			}
			return type.GetMethod("GetRef").CreateDelegate(typeof(HintDelegate)) as HintDelegate;
		}
	}
}
