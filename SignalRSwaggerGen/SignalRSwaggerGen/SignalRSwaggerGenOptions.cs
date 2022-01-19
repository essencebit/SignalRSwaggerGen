using SignalRSwaggerGen.Enums;
using SignalRSwaggerGen.Naming;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SignalRSwaggerGen
{
	/// <summary>
	/// Options used by SignalRSwaggerGen to generate documentation for SignalR hubs
	/// </summary>
	public class SignalRSwaggerGenOptions
	{
		/// <summary>
		/// The func that will get the hub name and return the path for the hub. The func will be skipped for the hubs that have a not null path specified for them in particular.
		/// If the func not specified explicitly, the default func will return a value based on the template 'Constants.DefaultHubPathTemplate'.
		/// If you decide to set a custom func, make sure the func will return a different path for each hub.
		/// </summary>
		/// <example>hubName => $"hubs/are/here/{hubName}"</example>
		/// <exception cref="ArgumentNullException">Thrown if the value is null</exception>
		public Func<string, string> HubPathFunc
		{
			get => _hubPathFunc;
			set
			{
				_hubPathFunc = value ?? throw new ArgumentNullException(nameof(HubPathFunc));
			}
		}

		/// <summary>
		/// A flag indicating what components will have Swagger documentation enabled automatically.
		/// Can be overridden for a specific component by specifying auto-discover value for that component in particular.
		/// If not specified explicitly, the default value is 'Constants.DefaultAutoDiscover'.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown if the value is 'AutoDiscover.Inherit', since there's no other higher level configuration to inherit from</exception>
		public AutoDiscover AutoDiscover
		{
			get => _autoDiscover;
			set
			{
				if (value == AutoDiscover.Inherit) throw new ArgumentException($"Auto-discover option '{value}' not allowed, since there's no other higher level configuration to inherit from");
				_autoDiscover = value;
			}
		}

		/// <summary>
		/// Same as HTTP verb. Can be overridden for a specific method by specifying the operation for that method in particular.
		/// If not specified explicitly, the default value is 'Constants.DefaultOperation'.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown if the value is 'Operation.Inherit', since there's no other higher level configuration to inherit from</exception>
		public Operation Operation
		{
			get => _operation;
			set
			{
				if (value == Operation.Inherit) throw new ArgumentException($"Operation '{value}' not allowed, since there's no other higher level configuration to inherit from");
				_operation = value;
			}
		}

		/// <summary>
		/// The name transformer that will be used to transform the name of the hubs and their methods.
		/// Can be overridden for a specific component by specifying a transformer for that component in particular.
		/// If not specified at any level, no transformation will happen. The namespace 'SignalRSwaggerGen.Naming' already contains some predefined name transformers, so check 'em out.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if the value is null</exception>
		public NameTransformer NameTransformer
		{
			get => _nameTransformer;
			set
			{
				_nameTransformer = value ?? throw new ArgumentNullException(nameof(NameTransformer));
			}
		}

		/// <summary>
		/// Use summary section from hub's XML comments as tag for Swagger doc
		/// </summary>
		public bool UseHubXmlCommentsSummaryAsTag { get; set; }

		/// <summary>
		/// Specify the assembly to be scanned for SignalR hubs. If no assemblies specified explicitly, the entry assembly will be scanned by default.
		/// This method has additive effect. You can use it multiple times to add more assemblies.
		/// </summary>
		/// <param name="assembly">Assembly to be scanned for SignalR hubs</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null</exception>
		public void ScanAssembly(Assembly assembly)
		{
			if (assembly == null) throw new ArgumentNullException(nameof(assembly));
			Assemblies.Add(assembly);
		}

		/// <summary>
		/// Specify assemblies to be scanned for SignalR hubs. If no assemblies specified explicitly, the entry assembly will be scanned by default.
		/// This method has additive effect. You can use it multiple times to add more assemblies.
		/// </summary>
		/// <param name="assemblies">Assemblies to be scanned for SignalR hubs</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblies"/> or any of its items is null</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="assemblies"/> is empty</exception>
		public void ScanAssemblies(IEnumerable<Assembly> assemblies)
		{
			if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
			if (!assemblies.Any()) throw new ArgumentException("Empty", nameof(assemblies));
			foreach (var assembly in assemblies)
			{
				ScanAssembly(assembly);
			}
		}

		/// <summary>
		/// Specify assemblies to be scanned for SignalR hubs. If no assemblies specified explicitly, the entry assembly will be scanned by default.
		/// This method has additive effect. You can use it multiple times to add more assemblies.
		/// </summary>
		/// <param name="assemblies">Assemblies to be scanned for SignalR hubs</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblies"/> or any of its items is null</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="assemblies"/> is empty</exception>
		public void ScanAssemblies(params Assembly[] assemblies)
		{
			if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
			if (assemblies.Length == 0) throw new ArgumentException("Empty", nameof(assemblies));
			foreach (var assembly in assemblies)
			{
				ScanAssembly(assembly);
			}
		}

		/// <summary>
		/// Specify the name of the Swagger document the hubs will be displayed in.
		/// Can be overridden for a specific hub by specifying document names for that hub in particular.
		/// If no document names specified explicitly, then the hubs will be displayed in all documents.
		/// This method has additive effect. You can use it multiple times to add more document names.
		/// </summary>
		/// <param name="documentName">Name of the Swagger document the hubs will be displayed in</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="documentName"/> is null</exception>
		public void DisplayInDocument(string documentName)
		{
			if (documentName == null) throw new ArgumentNullException(nameof(documentName));
			DocumentNames.Add(documentName);
		}

		/// <summary>
		/// Specify the list of names of the Swagger documents the hubs will be displayed in.
		/// Can be overridden for a specific hub by specifying document names for that hub in particular.
		/// If no document names specified explicitly, then the hubs will be displayed in all documents.
		/// This method has additive effect. You can use it multiple times to add more document names.
		/// </summary>
		/// <param name="documentNames">Names of the Swagger documents the hubs will be displayed in</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="documentNames"/> or any of its items is null</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="documentNames"/> is empty</exception>
		public void DisplayInDocuments(IEnumerable<string> documentNames)
		{
			if (documentNames == null) throw new ArgumentNullException(nameof(documentNames));
			if (!documentNames.Any()) throw new ArgumentException("Empty", nameof(documentNames));
			foreach (var documentName in documentNames)
			{
				DisplayInDocument(documentName);
			}
		}

		/// <summary>
		/// Specify the list of names of the Swagger documents the hubs will be displayed in.
		/// Can be overridden for a specific hub by specifying document names for that hub in particular.
		/// If no document names specified explicitly, then the hubs will be displayed in all documents.
		/// This method has additive effect. You can use it multiple times to add more document names.
		/// </summary>
		/// <param name="documentNames">Names of the Swagger documents the hubs will be displayed in</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="documentNames"/> or any of its items is null</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="documentNames"/> is empty</exception>
		public void DisplayInDocuments(params string[] documentNames)
		{
			if (documentNames == null) throw new ArgumentNullException(nameof(documentNames));
			if (documentNames.Length == 0) throw new ArgumentException("Empty", nameof(documentNames));
			foreach (var documentName in documentNames)
			{
				DisplayInDocument(documentName);
			}
		}

		/// <summary>
		/// Specify an XML comments file to be used for generating Swagger doc.
		/// This method has additive effect. You can use it multiple times to add more XML comments files.
		/// </summary>
		/// <param name="path">Path to the file that contains XML comments</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="path"/> does not exist</exception>
		public void UseXmlComments(string path)
		{
			if (path == null) throw new ArgumentNullException(nameof(path));
			if (!File.Exists(path)) throw new ArgumentException($"Does not exist: path=[{path}]", nameof(path));
			PathsToXmlCommentsFiles.Add(path);
		}

		/// <summary>
		/// Specify a list of XML comments files to be used for generating Swagger doc.
		/// This method has additive effect. You can use it multiple times to add more XML comments files.
		/// </summary>
		/// <param name="paths">Paths to the files that contain XML comments</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="paths"/> or any of its items is null</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="paths"/> is empty or any of its items does not exist</exception>
		public void UseXmlComments(IEnumerable<string> paths)
		{
			if (paths == null) throw new ArgumentNullException(nameof(paths));
			if (!paths.Any()) throw new ArgumentException("Empty", nameof(paths));
			foreach (var path in paths)
			{
				UseXmlComments(path);
			}
		}

		/// <summary>
		/// Specify a list of XML comments files to be used for generating Swagger doc.
		/// This method has additive effect. You can use it multiple times to add more XML comments files.
		/// </summary>
		/// <param name="paths">Paths to the files that contain XML comments</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="paths"/> or any of its items is null</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="paths"/> is empty or any of its items does not exist</exception>
		public void UseXmlComments(params string[] paths)
		{
			if (paths == null) throw new ArgumentNullException(nameof(paths));
			if (paths.Length == 0) throw new ArgumentException("Empty", nameof(paths));
			foreach (var path in paths)
			{
				UseXmlComments(path);
			}
		}

		internal HashSet<Assembly> Assemblies { get; } = new HashSet<Assembly>();
		internal HashSet<string> PathsToXmlCommentsFiles { get; } = new HashSet<string>();
		internal HashSet<string> DocumentNames { get; } = new HashSet<string>();

		private Func<string, string> _hubPathFunc = hubName => Constants.DefaultHubPathTemplate.Replace(Constants.HubNamePlaceholder, hubName);
		private AutoDiscover _autoDiscover = Constants.DefaultAutoDiscover;
		private Operation _operation = Constants.DefaultOperation;
		private NameTransformer _nameTransformer;
	}
}
