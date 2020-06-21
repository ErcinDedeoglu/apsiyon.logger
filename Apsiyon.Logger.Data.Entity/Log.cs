using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apsiyon.Logger.Data.Entity
{
    [Table("_Logs")]
    public class Log : Entity
    {
        [MaxLength(2048)]
        public string Message { get; set; }
        public byte[] Object { get; set; }
        [MaxLength(50)]
        public string ObjectType { get; set; }
    }
}