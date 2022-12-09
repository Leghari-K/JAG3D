using System.Collections.Generic;

namespace org.applied_geodesy.adjustment
{
	public sealed class EstimationStateType
	{
		public static readonly EstimationStateType ERROR_FREE_ESTIMATION = new EstimationStateType("ERROR_FREE_ESTIMATION", InnerEnum.ERROR_FREE_ESTIMATION, 1);
		public static readonly EstimationStateType BUSY = new EstimationStateType("BUSY", InnerEnum.BUSY, 0);
		public static readonly EstimationStateType ITERATE = new EstimationStateType("ITERATE", InnerEnum.ITERATE, 0);
		public static readonly EstimationStateType CONVERGENCE = new EstimationStateType("CONVERGENCE", InnerEnum.CONVERGENCE, 0);
		public static readonly EstimationStateType PRINCIPAL_COMPONENT_ANALYSIS = new EstimationStateType("PRINCIPAL_COMPONENT_ANALYSIS", InnerEnum.PRINCIPAL_COMPONENT_ANALYSIS, 0);
		public static readonly EstimationStateType ESTIAMTE_STOCHASTIC_PARAMETERS = new EstimationStateType("ESTIAMTE_STOCHASTIC_PARAMETERS", InnerEnum.ESTIAMTE_STOCHASTIC_PARAMETERS, 0);
		public static readonly EstimationStateType INVERT_NORMAL_EQUATION_MATRIX = new EstimationStateType("INVERT_NORMAL_EQUATION_MATRIX", InnerEnum.INVERT_NORMAL_EQUATION_MATRIX, 0);
		public static readonly EstimationStateType EXPORT_COVARIANCE_MATRIX = new EstimationStateType("EXPORT_COVARIANCE_MATRIX", InnerEnum.EXPORT_COVARIANCE_MATRIX, 0);
		public static readonly EstimationStateType EXPORT_COVARIANCE_INFORMATION = new EstimationStateType("EXPORT_COVARIANCE_INFORMATION", InnerEnum.EXPORT_COVARIANCE_INFORMATION, 0);
		public static readonly EstimationStateType UNSCENTED_TRANSFORMATION_STEP = new EstimationStateType("UNSCENTED_TRANSFORMATION_STEP", InnerEnum.UNSCENTED_TRANSFORMATION_STEP, 0);
		public static readonly EstimationStateType INTERRUPT = new EstimationStateType("INTERRUPT", InnerEnum.INTERRUPT, -1);
		public static readonly EstimationStateType SINGULAR_MATRIX = new EstimationStateType("SINGULAR_MATRIX", InnerEnum.SINGULAR_MATRIX, -2);
		public static readonly EstimationStateType ROBUST_ESTIMATION_FAILED = new EstimationStateType("ROBUST_ESTIMATION_FAILED", InnerEnum.ROBUST_ESTIMATION_FAILED, -3);
		public static readonly EstimationStateType NO_CONVERGENCE = new EstimationStateType("NO_CONVERGENCE", InnerEnum.NO_CONVERGENCE, -4);
		public static readonly EstimationStateType NOT_INITIALISED = new EstimationStateType("NOT_INITIALISED", InnerEnum.NOT_INITIALISED, -5);
		public static readonly EstimationStateType OUT_OF_MEMORY = new EstimationStateType("OUT_OF_MEMORY", InnerEnum.OUT_OF_MEMORY, -6);

		private static readonly List<EstimationStateType> valueList = new List<EstimationStateType>();

		static EstimationStateType()
		{
			valueList.Add(ERROR_FREE_ESTIMATION);
			valueList.Add(BUSY);
			valueList.Add(ITERATE);
			valueList.Add(CONVERGENCE);
			valueList.Add(PRINCIPAL_COMPONENT_ANALYSIS);
			valueList.Add(ESTIAMTE_STOCHASTIC_PARAMETERS);
			valueList.Add(INVERT_NORMAL_EQUATION_MATRIX);
			valueList.Add(EXPORT_COVARIANCE_MATRIX);
			valueList.Add(EXPORT_COVARIANCE_INFORMATION);
			valueList.Add(UNSCENTED_TRANSFORMATION_STEP);
			valueList.Add(INTERRUPT);
			valueList.Add(SINGULAR_MATRIX);
			valueList.Add(ROBUST_ESTIMATION_FAILED);
			valueList.Add(NO_CONVERGENCE);
			valueList.Add(NOT_INITIALISED);
			valueList.Add(OUT_OF_MEMORY);
		}

		public enum InnerEnum
		{
			ERROR_FREE_ESTIMATION,
			BUSY,
			ITERATE,
			CONVERGENCE,
			PRINCIPAL_COMPONENT_ANALYSIS,
			ESTIAMTE_STOCHASTIC_PARAMETERS,
			INVERT_NORMAL_EQUATION_MATRIX,
			EXPORT_COVARIANCE_MATRIX,
			EXPORT_COVARIANCE_INFORMATION,
			UNSCENTED_TRANSFORMATION_STEP,
			INTERRUPT,
			SINGULAR_MATRIX,
			ROBUST_ESTIMATION_FAILED,
			NO_CONVERGENCE,
			NOT_INITIALISED,
			OUT_OF_MEMORY
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private int id;
		private EstimationStateType(string name, InnerEnum innerEnum, int id)
		{
			this.id = id;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		public int Id
		{
			get
			{
				return id;
			}
		}

		public static EstimationStateType getEnumByValue(int value)
		{
			foreach (EstimationStateType element in EstimationStateType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return null;
		}

		public static EstimationStateType[] values()
		{
			return valueList.ToArray();
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public override string ToString()
		{
			return nameValue;
		}

		public static EstimationStateType valueOf(string name)
		{
			foreach (EstimationStateType enumInstance in EstimationStateType.valueList)
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new System.ArgumentException(name);
		}
	}

}