namespace MvLib
{
    public interface IAssemblyCallback 
    {
        void OnBeforeAssemblyReload();
        void OnAfterAssemblyReload();
        
    }
}
