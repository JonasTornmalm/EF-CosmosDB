using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EF_CosmosDB.Data
{
    public class CosmosContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Task> Tasks { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseCosmos(
                "https://localhost:8081",
                "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                databaseName: "UsersDB"
            );
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
