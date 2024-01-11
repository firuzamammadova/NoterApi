using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class FileType
    {
        public Guid Id { get; set; }
        public FileTypeEnum Type { get; set; }
        public bool DeleteStatus { get; set; }

    }

    public enum FileTypeEnum
    {
        Folder,
        Note,
        List,
        Empty,
        Test
    }
}
