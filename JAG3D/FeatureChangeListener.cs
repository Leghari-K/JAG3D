namespace org.applied_geodesy.adjustment.geometry
{

	public interface FeatureChangeListener : EventListener
	{
		void featureChanged(FeatureEvent evt);
	}

	public interface EventListener
	{
	}
}