namespace UserInterface.FileInUseChecker
{
    public interface IFileInUseChecker
    {
        bool IsFileInUse(string filePath);
    }
}
