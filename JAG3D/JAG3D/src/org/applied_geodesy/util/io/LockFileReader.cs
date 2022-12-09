using System;
using System.Collections.Generic;
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

namespace org.applied_geodesy.util.io
{

	using FileProgressEventType = org.applied_geodesy.util.io.FileProgressEvent.FileProgressEventType;

	public abstract class LockFileReader
	{
		private Path sourceFilePath = null;
		private string ignoreStartString = "";
		public const string UTF8_BOM = "\uFEFF";
//JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private bool interrupt_Conflict = false;
		private IList<EventListener> listenerList = new List<EventListener>();

		internal LockFileReader()
		{
		}

		internal LockFileReader(string fileName) : this((new File(fileName)).toPath())
		{
		}

		internal LockFileReader(File sf) : this(sf.toPath())
		{
		}

		internal LockFileReader(Path path)
		{
			Path = path;
		}

		internal virtual Path Path
		{
			set
			{
				this.sourceFilePath = value;
			}
			get
			{
				return this.sourceFilePath;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public abstract void parse(String line) throws java.sql.SQLException;
		public abstract void parse(string line);

		public virtual void ignoreLinesWhichStartWith(string str)
		{
			this.ignoreStartString = str;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void read() throws IOException, java.sql.SQLException
		public virtual void read()
		{
			if (this.sourceFilePath == null || !Files.exists(this.sourceFilePath) || !Files.isRegularFile(this.sourceFilePath) || !Files.isReadable(this.sourceFilePath))
			{
				return;
			}

			this.interrupt_Conflict = false;
			StreamReader reader = null;
			bool isFirstLine = true;
			try
			{
				long totalBytes = Files.size(this.sourceFilePath);
				File sourceFile = this.sourceFilePath.toFile();
				FileStream inputStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read);
				inputStream.getChannel().@lock(0, long.MaxValue, true);
				reader = new StreamReader(new StreamReader(inputStream, Charset.forName("UTF-8")), 1024 * 64);

				long readedBytes = 0L;
				string currentLine = null;
				while (!this.interrupt_Conflict && !string.ReferenceEquals((currentLine = reader.ReadLine()), null))
				{
					readedBytes += currentLine.Length;

					if (isFirstLine && currentLine.StartsWith(UTF8_BOM, StringComparison.Ordinal))
					{
						currentLine = currentLine.Substring(1);
						isFirstLine = false;
					}

					if (currentLine.Trim().Length > 0 && (this.ignoreStartString.Length == 0 || !currentLine.StartsWith(this.ignoreStartString, StringComparison.Ordinal)))
					{
						this.parse(currentLine);
					}

					this.fireFileProgressChanged(sourceFile, FileProgressEventType.READ_LINE, readedBytes, totalBytes);
				}
				this.fireFileProgressChanged(sourceFile, FileProgressEventType.READ_LINE, totalBytes, totalBytes);
			}
			finally
			{
				try
				{
					// closed all other streams
					// https://stackoverflow.com/questions/24362980/when-i-close-a-bufferedinputstream-is-the-underlying-inputstream-also-closed
					if (reader != null)
					{
						reader.Close();
					}
				}
				catch (IOException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
		}

		public virtual void interrupt()
		{
			this.interrupt_Conflict = true;
		}

		public virtual bool Interrupted
		{
			get
			{
				return this.interrupt_Conflict;
			}
		}

		public virtual void addFileProgressChangeListener(FileProgressChangeListener l)
		{
			this.listenerList.Add(l);
		}

		public virtual void removeFileProgressChangeListener(FileProgressChangeListener l)
		{
			this.listenerList.Remove(l);
		}

		private void fireFileProgressChanged(File file, FileProgressEventType eventType, long readedBytes, long totalBytes)
		{
			FileProgressEvent evt = new FileProgressEvent(file, eventType, Math.Min(readedBytes, totalBytes), Math.Max(readedBytes, totalBytes));
			object[] listeners = this.listenerList.ToArray();
			for (int i = 0; i < listeners.Length; i++)
			{
				if (listeners[i] is FileProgressChangeListener)
				{
					((FileProgressChangeListener)listeners[i]).fileProgressChanged(evt);
				}
			}
		}
	}
}