using System;
using System.Collections.Generic;

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

namespace org.applied_geodesy.util.sql
{

	using SqlFile = org.hsqldb.cmdline.SqlFile;
	using SqlToolError = org.hsqldb.cmdline.SqlToolError;

	public abstract class DataBase
	{
		private readonly string dbDriver, username, password;
		private Connection conn = null;
		private bool isOpen = false;

		public DataBase(string dbDriver, string username, string password)
		{
			this.dbDriver = dbDriver;
			this.username = username;
			this.password = password;
		}

		public abstract string URI {get;}

		public string DBDriver
		{
			get
			{
				return this.dbDriver;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void open() throws ClassNotFoundException, java.sql.SQLException
		public virtual void open()
		{
			if (this.conn == null || this.conn.isClosed())
			{
				this.conn = this.createConnection();
				this.isOpen = true;
			}
		}

		public virtual bool Open
		{
			get
			{
				try
				{
					return this.conn != null && !this.conn.isClosed() && this.isOpen;
				}
				catch (SQLException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
				return false;
			}
		}

		public virtual void close()
		{
			if (this.conn == null)
			{
				return;
			}
			try
			{
				if (this.conn.isClosed())
				{
					return;
				}
				this.conn.close();
			}
			catch (SQLException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			finally
			{
				this.conn = null;
				this.isOpen = false;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public abstract int getLastInsertId() throws java.sql.SQLException;
		public abstract int LastInsertId {get;}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public java.sql.PreparedStatement getPreparedStatement(String sql) throws java.sql.SQLException
		public virtual PreparedStatement getPreparedStatement(string sql)
		{
			if (this.Open)
			{
				return this.conn.prepareStatement(sql);
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public java.sql.Statement getStatement() throws java.sql.SQLException
		public virtual Statement Statement
		{
			get
			{
				if (this.Open)
				{
					return this.conn.createStatement();
				}
				return null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void commit() throws java.sql.SQLException
		public virtual void commit()
		{
			if (this.Open)
			{
				this.conn.commit();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setAutoCommit(boolean autoCommit) throws java.sql.SQLException
		public virtual bool AutoCommit
		{
			set
			{
				if (this.Open)
				{
					this.conn.setAutoCommit(value);
				}
			}
			get
			{
				if (this.Open)
				{
					return this.conn.getAutoCommit();
				}
				return false;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void rollback() throws java.sql.SQLException
		public virtual void rollback()
		{
			if (this.Open)
			{
				this.conn.rollback();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.sql.Connection createConnection() throws ClassNotFoundException, java.sql.SQLException
		private Connection createConnection()
		{
			Type.GetType(this.DBDriver);
			Connection con = DriverManager.getConnection(this.URI, this.username, this.password);
			return con;
		}

		internal virtual Connection Connection
		{
			get
			{
				return this.conn;
			}
		}

		// http://hsqldb.org/doc/2.0/verbatim/src/org/hsqldb/sample/SqlFileEmbedder.java
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void executeFiles(java.util.List<java.io.File> files) throws SQLException, java.io.IOException
		public virtual void executeFiles(IList<File> files)
		{
			bool autoCommit = true; // default value
			try
			{
				autoCommit = this.AutoCommit;
				this.AutoCommit = false;
				IDictionary<string, string> sqlVarMap = new Dictionary<string, string>();
				foreach (File file in files)
				{
					if (!file.isFile())
					{
						throw new IOException("Error, selected SQL file is not present, " + file.getAbsolutePath() + "!");
					}

					SqlFile sqlFile = new SqlFile(file);
					sqlFile.setConnection(this.conn);
					sqlFile.addUserVars(sqlVarMap);
					sqlFile.execute();

					this.conn = sqlFile.getConnection();
					sqlVarMap = sqlFile.getUserVars();
				}
			}
			catch (Exception e) when (e is SQLException || e is SqlToolError)
			{
				e.printStackTrace();
				this.rollback();
				throw new SQLException(e);
			}
			finally
			{
				this.AutoCommit = autoCommit;
			}
		}
	}

}