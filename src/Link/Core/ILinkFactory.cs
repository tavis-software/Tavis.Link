namespace Tavis
{
    public interface ILinkFactory
    {
        ILink CreateLink(string relation);
    }
}