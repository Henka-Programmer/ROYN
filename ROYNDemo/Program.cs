using MySql.Data.Entity;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using ROYN;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Runtime.Serialization;

namespace ROYNDemo
{
    public enum Role
    {
        User,
        Admin
    }

    [Table("Departments")]
    public class Department : ModelBase
    {
    }

    [Table("Jobs")]
    public class Job : ModelBase
    {
        public virtual ICollection<User> Users { get; set; }
    }

    public abstract class ModelBase
    {
        public bool Archived { get; set; } = false;

        public DateTime CreatedAt { get; set; }

        public int? CreatedByUId { get; set; }

        public ModelBase()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Note { get; set; }
        public int Order { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? UpdatedByUId { get; set; }
    }

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class MyDbContext : DbContext
    {
        public MyDbContext() : base(new MySqlConnection("server=localhost;port=3306;database=royn;uid=root;password=gamadev"), true)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<User> Users { get; set; }
    }

    [Table("Titles")]
    public class Title : ModelBase
    {
    }

    [Table("Users")]
    public class User : ModelBase
    {
        private int _CompanyId;

        [ForeignKey(nameof(Company))]
        public int CompanyId
        {
            get { return _CompanyId; }
            set
            {
                if (value != _CompanyId)
                {
                    _CompanyId = value;
                }
            }
        }

        public virtual Company Company { get; set; }

        private int _DepartmentId;

        public User()
        {
        }

        public string Address { get; set; }

        public DateTime Birthday { get; set; }

        public string City { get; set; }

        public virtual Department Department { get; set; }

        [ForeignKey(nameof(Department))]
        public int DepartmentId
        {
            get { return _DepartmentId; }
            set
            {
                if (value != _DepartmentId)
                {
                    _DepartmentId = value;
                }
            }
        }

        [DataMember]
        public virtual Job Job { get; set; }

        [ForeignKey(nameof(Job))]
        public int? JobId { get; set; }

        public string Password { get; set; }

        public Role Role { get; set; }

        public virtual Title Title { get; set; }

        [ForeignKey(nameof(Title))]
        public int? TitleId { get; set; }

        public string Username { get; set; }
        public string ZipCode { get; set; }
        public string FatherName { get; internal set; }
        public string MotherName { get; internal set; }
    }

    [Table("Companies")]
    public class Company : ModelBase
    {
    }

    internal class Program
    {
        public static List<User> Source = new List<User>();

        static Program()
        {
            for (int i = 0; i < 20; i++)
            {
                Source.Add(new User
                {
                    Username = $"User {i}",
                    Password = $"{i}",
                    Birthday = DateTime.Now.AddDays(i),
                    Role = i % 2 == 0 ? Role.Admin : Role.User,
                    Job = i % 2 == 0 ? new Job { Name = $"Job {i}" } : null,
                    Department = new Department { Name = $"Department {i}" },
                    Title = new Title { Name = $"Title {i}" },
                    ZipCode = "323265",
                    Note = $"Note {i}",
                    Order = i,
                    CreatedAt = DateTime.Now.AddDays(i * -1),
                    UpdatedAt = DateTime.Now,
                    City = $"City {i}",
                    Address = $"Address {i}",
                    Name = $"User {i}",
                    FatherName = $"User father {i}",
                    MotherName = $"User Mother {i}",
                    Company = new Company() { Name = $"Company {i}" },
                });
            }
        }

        private static void Main(string[] args)
        {
            var query = new RoynRequest<User>()
                .Add(x => x.Job.Name)
                .Add(x => x.Job.Id)
                .Add(x => x.Username)
                .Add(x => x.Id)
                .Add(x => x.Role)
                .Add(x => x.JobId).OrderBy(x => x.Job.Name).Skip(1).Take(2);

            var queryS = JsonConvert.SerializeObject(query);
            var queryF = JsonConvert.DeserializeObject<RoynRequest>(queryS);

            using (var context = new MyDbContext())
            {
                if (!context.Database.Exists())
                {
                    context.Database.Initialize(true);
                    context.Users.AddRange(Source);
                    context.SaveChanges();
                }

                RoynResult q = context.Execute(queryF);

                Console.WriteLine(q.Raw);
            }

            Console.ReadKey();
        }
    }
}