using System;
using System.IO;

/// <summary>
///*********************************************************************
/// Copyright by Michael Loesler, https://software.applied-geodesy.org   *
///                                                                      *
/// This program is free software; you can redistribute it and/or modify *
/// it under the terms of the GNU General Public License as published by *
/// the Free Software Foundation; either version 3 of the License, or    *
/// at your option any later version.                                    *
///                                                                      *
/// This program is distributed in the hope that it will be useful,      *
/// but WITHOUT ANY WARRANTY; without even the implied warranty of       *
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
/// GNU General Public License for more details.                         *
///                                                                      *
/// You should have received a copy of the GNU General Public License    *
/// along with this program; if not, see <http://www.gnu.org/licenses/>  *
/// or write to the                                                      *
/// Free Software Foundation, Inc.,                                      *
/// 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.            *
///                                                                      *
/// **********************************************************************
/// </summary>

namespace org.applied_geodesy.util.io.properties
{

	public class HTTPPropertiesLoader
	{

		static HTTPPropertiesLoader()
		{
			BufferedInputStream bis = null;
			const string path = "properties/proxy.default";

			try
			{
				if (typeof(HTTPPropertiesLoader).getClassLoader().getResourceAsStream(path) != null)
				{
					Properties properties = new Properties();
					bis = new BufferedInputStream(typeof(HTTPPropertiesLoader).getClassLoader().getResourceAsStream(path));
					properties.load(bis);

					string host = properties.getProperty("HOST", null);
					if (!string.ReferenceEquals(host, null) && host.Trim().Length > 0)
					{
						string protocol = properties.getProperty("PROTOCOL", "HTTP").ToLower();
						string port = properties.getProperty("PORT", "80");
						string username = properties.getProperty("USERNAME", "");
						string password = properties.getProperty("PASSWORD", "");

						Properties systemSettings = System.getProperties();
						systemSettings.put(protocol + ".proxyHost", host.Trim());
						systemSettings.put(protocol + ".proxyPort", port.Trim());
						systemSettings.put(protocol + ".proxyUser", username.Trim());
						systemSettings.put(protocol + ".proxyPassword", password.Trim());
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			finally
			{
				try
				{
					if (bis != null)
					{
						bis.close();
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
		}

		/// <summary>
		/// Stellt eine Verbindung zum Server her und prueft auf UPDATES. 
		/// Hierbei wird eine PHP-Datei gelesen bspw.:
		/// 
		/// <?php
		/// 	define("VERSION", 2.0);
		/// 	define("BUILD",   20091120);
		/// 	define("URI",     "http://sourceforge.net/projects/javagraticule3d/files/latest");
		/// 
		/// 	header('X-Powered-By: JAG3D');
		/// 	header('Cache-Control: no-cache, no-store, max-age=0, must-revalidate');
		/// 	header('Pragma: no-cache');
		/// 
		/// 	if (isset($_POST['checkupdate']) && $_POST['checkupdate'] == "jag3d") {
		/// 		header('Content-Type: text/plain; charset=utf-8');
		/// 		echo "VERSION: ".VERSION."\r\n";
		/// 		echo "BUILD: ".BUILD."\r\n";
		/// 		echo "URI: ".URI."\r\n";
		/// 	} 
		/// 	else {
		/// 		header('Content-Type: text/html; charset=utf-8');
		/// 		header('HTTP/1.1 404 Not Found');
		/// 		print('<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML 2.0//EN">
		/// 			<html><head>
		/// 			<title>404 Found</title>
		/// 			</head><body>
		/// 			<h1>Not Found</h1>
		/// 			<para>The requested URL was not found on this server.</para>
		/// 			<hr>
		/// 			<address>'.$_SERVER['SERVER_SOFTWARE'].' Server at '.$_SERVER['SERVER_NAME'].' Port '.$_SERVER['SERVER_PORT'].'</address>
		/// 			</body></html>');
		/// 		exit;
		/// 	}
		/// ?> </summary>
		/// <exception cref="IOException"> 
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static java.util.Properties getProperties(String address, URLParameter... params) throws java.io.IOException
		public static Properties getProperties(string address, params URLParameter[] @params)
		{
			Properties properties = new Properties();
			StreamReader bufferedReader = null;
			StreamWriter outputStreamWriter = null;
			try
			{
				// https://stackoverflow.com/questions/1626549/authenticated-http-proxy-with-java
				Authenticator.setDefault(new AuthenticatorAnonymousInnerClass());

				string data = "";
				foreach (URLParameter param in @params)
				{
					data += URLEncoder.encode(param.getKey().Trim(), "UTF-8") + "=" + URLEncoder.encode(param.getValue().Trim(), "UTF-8");
				}

				URL url = new URL(address);
				URLConnection conn = url.openConnection();
				conn.setDoOutput(true);
				conn.setDoInput(true);
				outputStreamWriter = new StreamWriter(conn.getOutputStream());
				outputStreamWriter.Write(data);
				outputStreamWriter.Flush();

				bufferedReader = new StreamReader(conn.getInputStream());
				properties.load(bufferedReader);

			}
			finally
			{
				if (outputStreamWriter != null)
				{
					try
					{
					outputStreamWriter.Close();
					}
				catch (Exception)
				{
				}
				}

				if (bufferedReader != null)
				{
					try
					{
					bufferedReader.Close();
					}
				catch (Exception)
				{
				}
				}
			}
			return properties;
		}

		private class AuthenticatorAnonymousInnerClass : Authenticator
		{
			protected internal override PasswordAuthentication PasswordAuthentication
			{
				get
				{
					if (getRequestorType() == RequestorType.PROXY)
					{
						string protocol = getRequestingProtocol().ToLower();
						string host = System.getProperty(protocol + ".proxyHost", "");
						string port = System.getProperty(protocol + ".proxyPort", "80");
						string username = System.getProperty(protocol + ".proxyUser", "");
						string password = System.getProperty(protocol + ".proxyPassword", "");
    
						int portNumber = 80;
						try
						{
							portNumber = int.Parse(port);
						}
						catch (Exception e)
						{
							Console.WriteLine(e.ToString());
							Console.Write(e.StackTrace);
						}
    
						if (getRequestingHost().equalsIgnoreCase(host) && portNumber == getRequestingPort())
						{
							return new PasswordAuthentication(username, password.ToCharArray());
						}
					}
					return null;
				}
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void main(String[] args) throws java.io.IOException
		public static void Main(string[] args)
		{
			string address = "https://software.applied-geodesy.org/update.php";

			URLParameter param = new URLParameter("checkupdate", "jag3d");

			Properties s = HTTPPropertiesLoader.getProperties(address, param);
			Console.WriteLine(s);
		}
	}

}