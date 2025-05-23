namespace MovieReservation.Domain.Entities.Base;

public abstract class EntityBase : IEntityBase
{
    public Guid Id { get; set; }

    protected EntityBase()
    {
    }
}