namespace AnkaCMS.Core.Specification
{
    public abstract class Specification<T> : ISpecification<T>
    {
        public abstract bool IsSatisfiedBy(T candidate);

        public ISpecification<T> And(ISpecification<T> other)
        {
            return new AndSpecification<T>(this, other);
        }

        public ISpecification<T> Or(ISpecification<T> other)
        {
            return new OrSpecification<T>(this, other);
        }

        public ISpecification<T> Not()
        {
            return new NotSpecification<T>(this);
        }

        public static Specification<T> operator |(Specification<T> spec1, Specification<T> spec2)
        {
            return new OrSpecification<T>(spec1, spec2);
        }
        public static Specification<T> operator &(Specification<T> spec1, Specification<T> spec2)
        {
            return new AndSpecification<T>(spec1, spec2);
        }
        public static Specification<T> operator !(Specification<T> spec)
        {
            return new NotSpecification<T>(spec);
        }
    }
}
