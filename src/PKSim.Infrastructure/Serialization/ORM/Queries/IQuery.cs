namespace PKSim.Infrastructure.Serialization.ORM.Queries
{
    public interface IQuery<TReturnType, TId>
    {
        TReturnType ResultFor(TId objectId);
    }
}