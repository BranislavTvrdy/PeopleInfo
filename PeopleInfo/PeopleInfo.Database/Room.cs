using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeopleInfo.Database
{
    public class Room
    {
        public int Id { get; set; }
        public string Label { get; set; }

        public List<Person> People { get; set; }

        public Room()
        {
            People = new List<Person>();
        }

        public Room(int paId, string paLabel)
        {
            Id = paId;
            Label = paLabel;
            People = new List<Person>();
        }
    }
}
