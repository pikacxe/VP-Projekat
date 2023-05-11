using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInterface.FileInUseChecker
{
    public interface IFileInUseChecker
    {
        bool IsFileInUse(string filePath);
    }
}
