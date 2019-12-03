namespace AnkaCMS.Core.Specification
{
    internal class OrSpecification<T> : CompositeSpecification<T>
    {
        public OrSpecification(ISpecification<T> left, ISpecification<T> right) : base(left, right) { }

        public override bool IsSatisfiedBy(T candidate)
        {
            return Left.IsSatisfiedBy(candidate) || Right.IsSatisfiedBy(candidate);
        }
    }
}