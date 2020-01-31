using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeopleInfo.Database
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TitleBefore { get; set; }
        public string TitleAfter { get; set; }
        public string JobPosition { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int DepartmentId { get; set; }
        public int? RoomId { get; set; }
        
        public string FullName { get; set; }


        public List<Department> Departments { get; set; }
        public List<Room> Rooms { get; set; }

        public Person()
        {
        }
        public Person(int paId, string paFirstName, string paLastName, string paTitleBefore, string paTitleAfter,
            string paJobPosition, string paEmail, string paPhone, int paDepartmentId, int? paRoomId)
        {
            Id = paId;
            FirstName = paFirstName;
            LastName = paLastName;
            TitleBefore = paTitleBefore;
            TitleAfter = paTitleAfter;
            JobPosition = paJobPosition;
            Email = paEmail;
            Phone = paPhone;
            DepartmentId = paDepartmentId;
            RoomId = paRoomId;
            FullName = FirstName + " " + LastName;
        }


    }
}
