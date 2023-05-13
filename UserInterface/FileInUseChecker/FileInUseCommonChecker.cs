using System.IO;
using System.Threading;

namespace UserInterface.FileInUseChecker
{
    public class FileInUseCommonChecker : IFileInUseChecker
    {
        private const int TIMEOUT = 10;
        private const int WAIT_MILLISECONDS = 500;

        public bool IsFileInUse(string filePath)
        {
            int cnt = 0;
            while (cnt < TIMEOUT && CheckIsFileInUse(new FileInfo(filePath)))
            {
                Thread.Sleep(WAIT_MILLISECONDS);
                cnt++;
            }
            if (cnt >= TIMEOUT)
                return true;
            return false;
        }

        private bool CheckIsFileInUse(FileInfo file)
        {
            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist

                return true;
            }
            finally
            {
                if (stream != null)

                    stream.Dispose();
            }
            //file is not locked
            return false;
        }
    }
}
