namespace Kostassoid.Anodyne.DataAccess
{
    public interface IOperationResolver
    {
        TOp Get<TOp>() where TOp : IDataOperation;
    }
}