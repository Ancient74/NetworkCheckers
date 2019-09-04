namespace NetworkCheckers
{
    public interface IUserNameProvider
    {
        string GetName();
        void SaveName(string name);
    }
}