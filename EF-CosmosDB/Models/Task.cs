using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF_CosmosDB.Data
{
    public class Task
    {
        [Key]
        public string Id { get; set; }
        public string Note { get; set; }
        public DateTime DateOfEntry { get; set; }
        public virtual User User { get; set; }
    }
}