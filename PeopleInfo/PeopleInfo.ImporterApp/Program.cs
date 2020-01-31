using PeopleInfo.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeopleInfo.ImporterApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = args[0];
            var people = new List<Person>();
            var rooms = new List<Room>();
            var departments = new List<Department>();
            //ReadPeople(ref people, path, ref rooms, ref departments);
            #region InicializationDB
            using (var db_context = new PeopleInfoContext())
            {
                db_context.Database.ExecuteSqlCommand("DELETE FROM Departments");
                db_context.Database.ExecuteSqlCommand("DELETE FROM Rooms");
                db_context.Database.ExecuteSqlCommand("DELETE FROM People");

                //db_context.Database.ExecuteSqlCommand("TRUNCATE TABLE Departments");
                //db_context.Database.ExecuteSqlCommand("TRUNCATE TABLE Rooms");
                //db_context.Database.ExecuteSqlCommand("TRUNCATE TABLE People");

                //db_context.Database.ExecuteSqlCommand("ALTER TABLE Departments AUTO_INCREMENT=0");
                //db_context.Database.ExecuteSqlCommand("ALTER TABLE Rooms AUTO_INCREMENT=0");
                //db_context.Database.ExecuteSqlCommand("ALTER TABLE People AUTO_INCREMENT=0");

                db_context.SaveChanges();

                //values[4] room
                //values[7] department
                var reader = new StreamReader(path);
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var values = line.Split(';');
                        var displayName = values[2];
                        var titBef = displayName.Remove(displayName.IndexOf(values[0]), displayName.Length - displayName.IndexOf(values[0]));
                        if (String.IsNullOrWhiteSpace(titBef))
                        {
                            titBef = null;
                        }
                        string titAft = null;
                        if (displayName.Contains(","))
                        {
                            titAft = displayName.Remove(0, displayName.IndexOf(values[1]) + values[1].Length + 1);
                        }
                        string jobPosition = String.IsNullOrWhiteSpace(values[3]) ? null : values[3];
                        string emailValue = String.IsNullOrWhiteSpace(values[5]) ? null : values[5];
                        string phoneNum = String.IsNullOrWhiteSpace(values[6]) ? null : values[6];
                        var roomLabel = values[4];
                        int? roomId = null;
                        if (roomLabel != "")
                        {
//                            bool willAddRoom = true;
//                            roomId = rooms.Count;
//                            foreach (var room in rooms)
//                            {
//                                if (room.Label == roomLabel)
//                                {
//                                    willAddRoom = false;
//                                }
//                            }
                            var foundRoom = db_context.Rooms.FirstOrDefault(r => r.Label == roomLabel);
                            if (foundRoom == null)
                            {
                                db_context.Rooms.Add(new Room(){Label = roomLabel});
                                db_context.SaveChanges();
                                
                            }
                            roomId = db_context.Rooms.FirstOrDefault(r => r.Label == roomLabel).Id;

                            //                            if (willAddRoom)
                            //                            {
                            //                                rooms.Add(new Room(rooms.Count, roomLabel));
                            //                            }
                        }
                        var department = values[7];
                        var departmentName = department.Remove(department.IndexOf('('));
                        var departmentAbb = department.Remove(0, department.IndexOf('(')).Replace("(", "").Replace(")", "");
                        var departId = 0;
                        //paDepartments.Add();

//                        bool willAddDepart = true;
//                        foreach (var depart in departments)
//                        {
//                            if (depart.Name == departmentName)
//                            {
//                                willAddDepart = false;
//                            }
//                        }
                        var foundDepart = db_context.Departments.FirstOrDefault(d => d.Name == departmentName);
                        if (foundDepart == null)
                        {
                            db_context.Departments.Add(new Department() { Name = departmentName,Abbreviatinon = departmentAbb});
                            db_context.SaveChanges();
                        }
                        departId = db_context.Departments.FirstOrDefault(d => d.Name == departmentName).Id;

                        //                        if (willAddDepart)
                        //                        {
                        //                            departments.Add(new Department(departId, departmentName, departmentAbb));
                        //                        }
                        //                        db_context.People.Add(new Person(people.Count, values[0], values[1], titBef, titAft, jobPosition,
                        //                            values[5], values[6], departId, roomId));
                        db_context.People.Add(new Person()
                        {
                            FirstName = values[0], 
                            LastName = values[1], 
                            TitleBefore = titBef, 
                            TitleAfter = titAft,
                            JobPosition = jobPosition,
                            Email = emailValue, 
                            Phone = phoneNum,
                            DepartmentId = departId,
                            RoomId = roomId,
                            FullName = values[0] + " " + values[1]
                        });
                    }
                    db_context.SaveChanges();
                }
                reader.Close();


                /*foreach (var room in rooms)
                {
                    db_context.Rooms.Add(room);
                }
                foreach (var depart in departments)
                {
                    db_context.Departments.Add(depart);
                }
                foreach (var person in people)
                {
                    db_context.People.Add(person);
                }
                db_context.SaveChanges();*/
            }
            Console.WriteLine("DB Loaded!");
            Console.ReadLine();
            #endregion
        }


        public static void ReadPeople(ref List<Person> paPeople, string paPeoplePath,
            ref List<Room> paRooms, ref List<Department> paDepartments)
        {
            //values[4] room
            //values[7] department
            var reader = new StreamReader(paPeoplePath);
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var values = line.Split(';');
                    var displayName = values[2];
                    var titBef = displayName.Remove(displayName.IndexOf(values[0]), displayName.Length - displayName.IndexOf(values[0]));
                    if (titBef == "")
                    {
                        titBef = null;
                    }
                    string titAft = null;
                    if (displayName.Contains(","))
                    {
                        titAft = displayName.Remove(0, displayName.IndexOf(values[1]) + values[1].Length + 1);
                    }
                    string jobPosition = values[3] == "" ? null : values[3];
                    var roomLabel = values[4];
                    int? roomId = null;
                    if (roomLabel != "")
                    {
                        bool willAddRoom = true;
                        roomId = paRooms.Count;
                        foreach(var room in paRooms)
                        {
                            if(room.Label == roomLabel)
                            {
                                willAddRoom = false;
                            }
                        }
                        if(willAddRoom)
                        {
                            paRooms.Add(new Room(paRooms.Count, roomLabel));
                        }
                    }
                    

                    var department = values[7];
                    var departmentName = department.Remove(department.IndexOf('('));
                    var departmentAbb = department.Remove(0, department.IndexOf('(')).Replace("(", "").Replace(")", "");
                    var departId = paDepartments.Count;
                    //paDepartments.Add();

                    bool willAddDepart = true;
                    foreach (var depart in paDepartments)
                    {
                        if (depart.Name == departmentName)
                        {
                            willAddDepart = false;
                        }
                    }
                    if (willAddDepart)
                    {
                        paDepartments.Add(new Department(departId, departmentName,departmentAbb));
                    }

                    
                    paPeople.Add(new Person(paPeople.Count,values[0], values[1],titBef,titAft,jobPosition, 
                        values[5], values[6],departId,roomId));
                }
            }
            reader.Close();
        }

    }
}
