using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace Waveface.Stream.WindowsClient
{
	/// <summary>
	/// 
	/// </summary>
	public class AutoImportContentProvider
	{
		#region Var
		private IEnumerable<IContentProvider> _contentProviders;
		#endregion


		#region Private Property
		/// <summary>
		/// Gets the m_ content providers.
		/// </summary>
		/// <value>The m_ content providers.</value>
		private IEnumerable<IContentProvider> m_ContentProviders
		{
			get
			{
				return _contentProviders ?? (_contentProviders = GetContentProviders());
			}
		}
		#endregion


		#region Constructor
		#endregion


		#region Private Method
		/// <summary>
		/// Gets the content providers.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IContentProvider> GetContentProviders()
		{
			var assembly = Assembly.GetExecutingAssembly();

			Debug.Assert(assembly != null);

			return from type in assembly.GetTypes()
				   where type.IsPublic && !type.IsAbstract && type.GetInterface(typeof(IContentProvider).Name, true) != null && type.GetConstructor(Type.EmptyTypes) != null
				   select assembly.CreateInstance(type.FullName) as IContentProvider;
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Gets the contents.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IContent> GetContents()
		{
			return from contentProvider in m_ContentProviders
				   from content in contentProvider.GetContents()
				   select content;
		}

		/// <summary>
		/// Gets the contents.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IContent> GetContents(ContentProviderType providerType)
		{
			return from contentProvider in m_ContentProviders
				   where contentProvider.Type == providerType
				   from content in contentProvider.GetContents()
				   select content;
		}
		#endregion
	}
}
