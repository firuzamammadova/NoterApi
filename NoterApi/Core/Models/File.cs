using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class RecordFile
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public FileTypeEnum Type { get; set; }

        public bool IsParent { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? TypeId { get; set; }
        public long? UserId { get; set; }
        public string? Context { get; set; }


        public DateTime? CreatedDate { get; set; }

        public bool? DeleteStatus { get; set; }

    }
}
