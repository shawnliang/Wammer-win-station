using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Waveface.Stream.ClientFramework
{
	public class InvokeRestfulCommand : WebSocketCommandBase
	{
		#region Public Property
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public override string Name
		{
			get { return "invokeRestful"; }
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Executes the specified parameters.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
        public override Dictionary<string, Object> Execute(Dictionary<string, Object> parameters = null)
		{
			const string RESTFUL_API_KEY = "restful_api";
			var restfulAPI = parameters[RESTFUL_API_KEY];
			var restfulUrl = string.Format(@"http://127.0.0.1:9981/v2/{0}", restfulAPI);

			var restfulParameters = new NameValueCollection();
			foreach (var parameter in parameters)
			{
				restfulParameters.Add(parameter.Key, parameter.Value.ToString());
			}

            var restfulResponse = StationAPI.Post(restfulUrl, restfulParameters);
			Trace.WriteLine(string.Format("Received restful response: {0}", restfulResponse));
			var jObject = JObject.Parse(restfulResponse);

			var responseParameters = new Dictionary<string, Object>() 
			{
				{RESTFUL_API_KEY, restfulAPI}
			};


			foreach(var property in jObject.Properties())
			{
				responseParameters.Add(property.Name, property.Value);
			}

            return responseParameters;
		}
		#endregion
	}
}
