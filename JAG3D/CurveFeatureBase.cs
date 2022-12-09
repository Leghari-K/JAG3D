namespace org.applied_geodesy.adjustment.geometry
{
    public abstract class CurveFeatureBase
    {
        private bool immutable;

        protected CurveFeatureBase(bool immutable)
        {
            this.immutable = immutable;
        }



        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string? ToString()
        {
            return base.ToString();
        }
    }
}