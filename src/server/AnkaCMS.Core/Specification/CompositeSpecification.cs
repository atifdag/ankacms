namespace AnkaCMS.Core.Specification
{
    internal abstract class CompositeSpecification<T> : Specification<T>
    {
        protected readonly ISpecification<T> Left;

        protected readonly ISpecification<T> Right;

        protected CompositeSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            Left = left;
            Right = right;
        }
    }
}