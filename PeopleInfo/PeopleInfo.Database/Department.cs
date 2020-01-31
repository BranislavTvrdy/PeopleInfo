using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeopleInfo.Database
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviatinon { get; set; }

        public List<Person> People { get; set; }

        public Department()
        {
            People = new List<Person>();
        }

        public Department(int paId, string paName, string paAbb)
        {
            Id = paId;
            Name = paName;
            Abbreviatinon = paAbb;
            People = new List<Person>();
        }
    }
}
