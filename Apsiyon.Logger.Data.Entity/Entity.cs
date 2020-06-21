using System;

namespace Apsiyon.Logger.Data.Entity
{
    public abstract class Entity
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public bool Deleted { get; set; }
    }
}