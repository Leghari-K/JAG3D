namespace org.applied_geodesy.adjustment
{
	public class DefaultValue
	{
		private const int MAXIMAL_ITERATIONS = 5000;
		private const double PROBABILITY_VALUE = 0.001;
		private const double POWER_OF_TEST = 0.8;
		private const double ROBUST_ESTIMATION_LIMIT = 3.5;

		private DefaultValue()
		{
		}

		public static int MaximalNumberOfIterations
		{
			get
			{
				return MAXIMAL_ITERATIONS;
			}
		}
		public static double ProbabilityValue
		{
			get
			{
				return PROBABILITY_VALUE;
			}
		}
		public static double PowerOfTest
		{
			get
			{
				return POWER_OF_TEST;
			}
		}
		public static double RobustEstimationLimit
		{
			get
			{
				return ROBUST_ESTIMATION_LIMIT;
			}
		}
	}

}