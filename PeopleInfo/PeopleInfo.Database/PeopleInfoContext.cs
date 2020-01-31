using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Collections.Generic;
using System.Data.Entity;


namespace PeopleInfo.Database
{
    public class PeopleInfoContext : DbContext
    {

        public PeopleInfoContext() : base("name=BaliContext")
        {
        }

        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=sharp.kst.fri.uniza.sk;" +
                                            "Initial Catalog=problem2019_DissertationThemes_TvrdyBranislavDb;" +
                                            "Persist Security Info=True;" +
                                            "User ID=problem2019_DissertationThemes_TvrdyBranislav;" +
                                            "Password=558884");

            }
        }*/

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>().ToTable("Departments");

            modelBuilder.Entity<Person>().ToTable("People");

            modelBuilder.Entity<Room>().ToTable("Rooms");

            modelBuilder.Entity<Department>().HasMany(depar => depar.People);

            modelBuilder.Entity<Room>().HasMany(room => room.People);

            //OnModelCreatingPartial(modelBuilder);
        }


        public IQueryable<Person> GetPeople()
        {
            //Enumerable.Empty<Club>().AsQueryable();
            return this.People.AsQueryable();
        }

        public List<Room> GetRooms()
        {
            List<Room> ret = new List<Room>();
            foreach(var rooma in this.Rooms.AsQueryable())
            {
                ret.Add(rooma);
            }
            return ret;
        }

    }
}
