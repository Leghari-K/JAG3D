
namespace org.applied_geodesy.adjustment.geometry
{

	public class FeatureEvent : EventObject
	{
		private const long serialVersionUID = 4470566122319625334L;

		public enum FeatureEventType
		{
			FEATURE_ADDED,
			FEATURE_REMOVED
		}

		private readonly FeatureEventType type;
		internal FeatureEvent(Feature feature, FeatureEventType type) : base()
		{
			this.type = type;
		}

		public virtual FeatureEventType EventType
		{
			get
			{
				return this.type;
			}
		}
	}
}