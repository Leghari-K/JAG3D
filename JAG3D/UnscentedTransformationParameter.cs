namespace org.applied_geodesy.adjustment
{
	public class UnscentedTransformationParameter
	{
		private const double ALPHA = 1.0;
		private const double BETA = 2.0;
		private const double WEIGHT_ZERO = 0.0;

		public static double Alpha
		{
			get
			{
				return ALPHA;
			}
		}

		public static double Beta
		{
			get
			{
				return BETA;
			}
		}

		public static double WeightZero
		{
			get
			{
				return WEIGHT_ZERO;
			}
		}
	}

}