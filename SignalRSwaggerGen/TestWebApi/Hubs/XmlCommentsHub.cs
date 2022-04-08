using SignalRSwaggerGen.Attributes;
using System;
using System.Collections.Generic;

namespace TestWebApi.Hubs
{
	public class Outer
	{
		public class Middle<TO1, TO2>
		{
			public class Inner
			{
				public class GenericClass<TO1, TG2, TG3> { }

				/// <summary>
				/// xml commented hub
				/// </summary>
				/// <typeparam name="TO1"></typeparam>
				/// <typeparam name="TC1"></typeparam>
				[SignalRHub(documentNames: new[] { "hubs" })]
				public class XmlCommentedHub<TO1, TC1>
				{
					public struct Struct
					{
						public struct GenericStruct<TS1>
						{
							public interface IInterface<TI1, TI2>
							{
								public delegate int Delegate<in TD1, out TD2>();
							}
						}
					}

					/// <summary>
					/// xml commented method1 summary
					/// </summary>
					/// <typeparam name="TA1"></typeparam>
					/// <typeparam name="TA2"></typeparam>
					/// <param name="arg1">xml commented arg1 description</param>
					/// <param name="arg2">xml commented arg2 description</param>
					/// <param name="arg3">xml commented arg3 description</param>
					/// <param name="arg4">xml commented arg4 description</param>
					/// <returns>something</returns>
					public unsafe void Method1<TA1, TA2>(
						ref Middle<int, TA1>.Inner.GenericClass<GenericClass<bool, TC1, TA2[]>, TO1, TO2[,]>[,,,][][,,] arg1,
						Struct.GenericStruct<Struct.GenericStruct<TA1>.IInterface<List<Struct.GenericStruct<TA1>>[,], GenericClass<Outer, Middle<TO2, TO1>, Inner>>.Delegate<object[,,,,][][,,][][][][,,,,][,,,], List<object>[]>>* arg2,
						List<List<Dictionary<XmlCommentedHub<Struct.GenericStruct<Struct>, TC1>, ISet<TO2>[,,][][]>>> arg3,
						List<List<Dictionary<XmlCommentedHub<Struct.GenericStruct<Struct.GenericStruct<Struct>.IInterface<TA2, object>.Delegate<bool, Struct.GenericStruct<TO1[][,,,,,]>.IInterface<string, int?>.Delegate<sbyte, DateTime>[,,,,,,,][][]>>, TC1>, ISet<TO2>[,,][][]>>> arg4)
					{
						return;
					}

					/// <summary>
					/// xml commented method2 summary
					/// </summary>
					/// <typeparam name="TArg1"></typeparam>
					/// <typeparam name="TArg2"></typeparam>
					public void Method2<TArg1, TArg2>()
					{
						return;
					}

					/// <summary>
					/// xml commented method3 summary
					/// </summary>
					public void Method3()
					{
						return;
					}
				}
			}
		}
	}
}
