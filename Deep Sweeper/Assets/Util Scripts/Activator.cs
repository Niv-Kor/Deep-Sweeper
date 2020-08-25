public interface IComplexActivator
{
    /// <summary>
    /// Activate or deactivate the mine's expensive components.
    /// </summary>
    /// <param name="flag">True to enable or false to disable</param>
    void Activate(bool flag);
}