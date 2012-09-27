using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Text;
using System.Xml.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Runtime.Serialization;

using MonoCross.Navigation;

namespace dotDialog.Sample.PersonalInfoManger
{
    public class App : MXApplication
    {
#if MONOTOUCH
        public static string AppPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#elif ANDROID
		static string AppPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);			
#elif SILVERLIGHT
		// with IsolatedStorage there is no ROOT path, only application files can be accessed
        public static string AppPath = "";
#else
        public static string AppPath = Assembly.GetAssembly(typeof(App)).CodeBase.Substring(0, Assembly.GetAssembly(typeof(App)).CodeBase.LastIndexOf("/")).Replace("file:///", "");
#endif

		public App() { }

        public override void OnAppLoad()
        {
			DataCacheRoot = Path.Combine(AppPath, "cache");
			if (!Directory.Exists(DataCacheRoot)) { Directory.CreateDirectory(DataCacheRoot); }

            Title = "Info Mgr";

            // add controllers to navigation map
			NavigationMap.Add(ContactListController.Uri, new ContactListController());
			ContactController.RegisterUris(this);
									
			NavigationMap.Add(CalendarListController.Uri, new CalendarListController());
			CalendarEventController.RegisterUris(this);

			NavigationMap.Add(TaskListController.Uri, new TaskListController());
			TaskController.RegisterUris(this);

            // set navigate on load endpoint
            NavigateOnLoad = ContactListController.Uri;
        }

		#region Over-The-Wire method calls + Caching To Disk Methods & properties
		public static byte[] LoadBytesFromDataSource(string uri, bool makeDataCall)
		{
			byte[] returnBytes = null;

			int startingIndex = uri.LastIndexOf("/") + 1;
			string filename = uri.Substring(startingIndex, uri.Length - startingIndex);
			string filePath = Path.Combine(App.DataCacheRoot, "xml", filename);

			try 
			{
				if (File.Exists(filePath)) 
				{  	
					byte[] xmlBytes = null;

					// open byte stream to decrypted stream
					using (FileStream stream = File.OpenRead(filePath))
					{
						xmlBytes = new byte[stream.Length];
						int i = stream.Read(xmlBytes, 0, xmlBytes.Length);
						if (i < xmlBytes.Length)
						{
							Debug.WriteLine("Reading additional bytes from: " + filePath);
							stream.Read(xmlBytes, i, xmlBytes.Length - i);
						}
						stream.Close();
					}
					Debug.WriteLine("Decrypted bytes are: " + Encoding.UTF8.GetString(xmlBytes));
					returnBytes = xmlBytes;
				}
				else if (makeDataCall)
				{
					#region Call webservice
					var startTime = DateTime.UtcNow;
					
					// get data by calling a RESTful service call endpoint
					string response = GetResponseString(uri);
					if (!string.IsNullOrEmpty(response))
					{
						returnBytes = Encoding.UTF8.GetBytes(response);
						Debug.WriteLine("Loaded this xml from online: " + response);

						//TODO: Check if successful response first.
						App.CacheToDisk(response, uri);

						// log time it takes to get resource
						var ctd = DateTime.UtcNow - startTime;
						Debug.WriteLine(string.Format("Over-the-wire plus Cache-to-disk {0} cost {1:0.0}ms", filename, ctd.TotalMilliseconds));
					}
					else { Debug.WriteLine("Failed to get resource from " + uri); }
					#endregion
				}
			}
			catch (Exception e) 
			{
				Debug.WriteLine("Following exception occured while loading resource string:\r\n" + e);
			}
			return returnBytes;
		}

		private static string GetResponseString(string url)
		{
			string retVal = null;

			DateTime startTime = DateTime.UtcNow;
			HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(new Uri(url, UriKind.Absolute));

			// get response from web server
			try 
			{
				var response = wr.GetResponse();
				var sr = new StreamReader(response.GetResponseStream());
				retVal = sr.ReadToEnd();
				Debug.WriteLine("Response from {0} is:\r\n{1}", url, retVal);
			}
			catch (WebException e) { Console.WriteLine(string.Format("Failure calling {0}\r\n{1}", url, e)); }

			// log cost of downloading resource
			var otw = DateTime.UtcNow - startTime;
			Debug.WriteLine(string.Format("Over-the-wire cost {1:0.0}ms", otw.TotalMilliseconds));

			return retVal;
		}

		public static string CacheToDisk(DataContractSerializer serializer, object model, string originUri)
		{
			string xml = null;
			
			// Serialize the object to xml string.
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(memoryStream))
				{
					serializer.WriteObject(writer, model);
					writer.Close();
				}
				
				// Get bytes and close the memory stream
				xml = Encoding.UTF8.GetString(memoryStream.ToArray());
				memoryStream.Close();
			}
			
			Debug.WriteLine("Writing this XML: " + xml);
			
			// Store to disk
			CacheToDisk(xml, originUri);

			return xml;
		}

		private static void CacheToDisk(string xml, string originUri)
		{
			int startingIndex = originUri.LastIndexOf ("/") + 1;
			string filename = originUri.Substring (startingIndex, originUri.Length - startingIndex);
			string path = Path.Combine (App.DataCacheRoot, "xml", filename);
			App.CacheToDiskWithFilePath(xml, path);
		}

		public static void CacheToDiskWithFilePath(string xml, string filePath)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(xml);
#if DEBUG
			int fileStringStart = filePath.LastIndexOf(Path.DirectorySeparatorChar) + 1;
			string pathonly = filePath.Substring(0, fileStringStart);
			pathonly = Path.Combine(pathonly, "unencrypted");
			if (!Directory.Exists(pathonly)) { Directory.CreateDirectory(pathonly); }
			string uepath = Path.Combine(pathonly, filePath.Substring(fileStringStart));
			File.WriteAllBytes(uepath, bytes);

#endif
			try { File.WriteAllBytes(filePath, bytes); }
			catch (Exception e) 
			{
				Debug.WriteLine ("Error writing file to cache:\r\n" + e);
				if (File.Exists(filePath)) {
					Debug.WriteLine ("Could not cache to disk, file existed: " + filePath);
				}
				throw e;
			}
		}
		public static string DataCacheRoot = null;
		#endregion
	}

}
