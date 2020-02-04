using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF_CosmosDB.Data
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public virtual ICollection<Task> ToDoList { get; set; }
    }
}