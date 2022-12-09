using System;

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

namespace org.applied_geodesy.adjustment.cmd
{

	using EstimationStateType = org.applied_geodesy.adjustment.EstimationStateType;
	using NetworkAdjustment = org.applied_geodesy.adjustment.network.NetworkAdjustment;
	using SQLAdjustmentManager = org.applied_geodesy.adjustment.network.sql.SQLAdjustmentManager;
	using HSQLDB = org.applied_geodesy.util.sql.HSQLDB;

	public class OpenAdjustmentCMD
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			adjustmentStateListener = new AdjustmentStateListener(this);
		}

		private bool displayState;
		private HSQLDB dataBase;
		private AdjustmentStateListener adjustmentStateListener;

		private class AdjustmentStateListener : PropertyChangeListener
		{
			private readonly OpenAdjustmentCMD outerInstance;

			public AdjustmentStateListener(OpenAdjustmentCMD outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void propertyChange(PropertyChangeEvent evt)
			{
				string name = evt.getPropertyName();

				EstimationStateType state = EstimationStateType.valueOf(name);
				if (state == null)
				{
					return;
				}

				object oldValue = evt.getOldValue();
				object newValue = evt.getNewValue();

				if (outerInstance.displayState)
				{
					Console.WriteLine("Current state: " + name + " (" + newValue + "/" + oldValue + ")");
				}
			}
		}

		public OpenAdjustmentCMD(string dataBaseName) : this(dataBaseName, true)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		public OpenAdjustmentCMD(string dataBaseName, bool displayState)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.dataBase = new HSQLDB(dataBaseName);
			this.displayState = displayState;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public int process() throws Exception
		public virtual int process()
		{
			EstimationStateType returnType = EstimationStateType.NOT_INITIALISED;

			bool isOpen = false;
			try
			{
				isOpen = this.dataBase.Open;
				if (!isOpen)
				{
					this.dataBase.open();
				}

				SQLAdjustmentManager adjustmentManager = new SQLAdjustmentManager(this.dataBase);
				NetworkAdjustment adjustment = adjustmentManager.NetworkAdjustment;

				adjustment.addPropertyChangeListener(this.adjustmentStateListener);
				returnType = adjustment.estimateModel();
				this.destroyNetworkAdjustment(adjustment);

				adjustmentManager.saveResults();
				adjustmentManager.clear();
			}
			finally
			{
				if (this.dataBase != null && !isOpen)
				{
					this.dataBase.close();
				}
			}
			return returnType.getId();
		}

		private void destroyNetworkAdjustment(NetworkAdjustment adjustment)
		{
			if (adjustment != null)
			{
				adjustment.removePropertyChangeListener(this.adjustmentStateListener);
				adjustment.clearMatrices();
				adjustment = null;
			}
		}

		public static void Main(string[] args)
		{
			try
			{
				System.setProperty("com.github.fommil.netlib.BLAS", "com.github.fommil.netlib.F2jBLAS");
				System.setProperty("com.github.fommil.netlib.LAPACK", "com.github.fommil.netlib.F2jLAPACK");
				System.setProperty("com.github.fommil.netlib.ARPACK", "com.github.fommil.netlib.F2jARPACK");

				System.setProperty("hsqldb.reconfig_logging", "false");

				LogManager.getLogManager().reset();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}

			int status = -1;

			if (args.Length == 0)
			{
				throw new System.ArgumentException("Error, no database specified!");
			}

			string dataBaseName = args[0];
			bool displayState = args.Length > 1 && args[1].Equals("TRUE", StringComparison.OrdinalIgnoreCase) ? true : false;
			try
			{
				if (!Files.isRegularFile(Paths.get(dataBaseName + ".script")) || !Files.isRegularFile(Paths.get(dataBaseName + ".properties")) || !Files.isRegularFile(Paths.get(dataBaseName + ".data")))
				{
					throw new IOException("Error, related database files (e.g. script, properties or data) not found! " + dataBaseName);
				}

				OpenAdjustmentCMD openAdjustment = new OpenAdjustmentCMD(dataBaseName, displayState);
				status = openAdjustment.process();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			Environment.Exit(status);
		}
	}

}