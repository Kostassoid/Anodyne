namespace Kostassoid.Anodyne.DataAccess
{
    public interface IDataSessionFactory
    {
        IDataSession OpenSession();
    }
}