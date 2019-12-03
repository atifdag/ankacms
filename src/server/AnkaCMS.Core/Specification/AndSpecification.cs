namespace AnkaCMS.Core.Specification
{
    internal class AndSpecification<T> : CompositeSpecification<T>
    {
        public AndSpecification(ISpecification<T> left, ISpecification<T> right) : base(left, right) { }

        public override bool IsSatisfiedBy(T candidate)
        {
            return Left.IsSatisfiedBy(candidate) && Right.IsSatisfiedBy(candidate);
        }
    }
}