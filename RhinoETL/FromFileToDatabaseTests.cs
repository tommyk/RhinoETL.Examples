using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PetaPoco;
using Rhino.Etl.Core;
using Rhino.Etl.Core.ConventionOperations;
using Rhino.Etl.Core.Files;
using Rhino.Etl.Core.Operations;

namespace RhinoETL.Examples
{
    /// <summary>
    /// Just pulls data out of a csv, and sticks it into a table in the local database.
    /// 
    /// Using some little ORM call petapoco to make easier to validate sql ran...
    /// http://www.toptensoftware.com/petapoco/
    /// </summary>
    [TestFixture]
    public class FromFileToDatabaseTests
    {
        private Database _db;

        [SetUp]
        public void Setup()
        {
            _db = new PetaPoco.Database("localRhinoEtlExample");
            //clean up the table data...
            _db.Execute("truncate table users;");
        }

        [Test]
        public void pull_from_csv_and_import_to_table()
        {
            EtlProcess process = new UserCsvToDatabaseProcess();
            process.Execute();
            if (process.GetAllErrors().Any())
            {
                foreach (Exception exception in process.GetAllErrors())
                    Console.WriteLine(exception.Message);
                throw new Exception("Errors occured!");
            }

            var users = _db.Query<User>("select * from users").ToList();

            Assert.That(users.Count, Is.EqualTo(3));
            //is tommy kelly there
            Assert.That(users.Count(user => user.FirstName == "Tommy" && 
                user.LastName == "Kelly" && 
                user.Age == 29 &&
                user.Sex == "Male"), Is.EqualTo(1));

            //is Joe Smith there
            Assert.That(users.Count(user => user.FirstName == "Joe" &&
                user.LastName == "Smith" &&
                user.Age == 21 &&
                user.Sex == "Male"), Is.EqualTo(1));

            //is Sally Sass there
            Assert.That(users.Count(user => user.FirstName == "Sally" &&
                user.LastName == "Sass" &&
                user.Age == 22 &&
                user.Sex == "Female"), Is.EqualTo(1));
        }

        [Test]
        public void bad_data_make_sure_transaction_rolls_back()
        {
            var process = new UserCsvToDatabaseProcess("bad_users.csv");
            process.Execute();
            if (process.GetAllErrors().Any())
            {
                foreach (Exception exception in process.GetAllErrors())
                    Console.WriteLine(exception.Message);
            }



            int rowCount = _db.ExecuteScalar<int>("select count(*) from users");
            Assert.That(rowCount, Is.EqualTo(0));
        }
    }

    public class UserCsvToDatabaseProcess : EtlProcess
    {
        private readonly string _userFile;
        public UserCsvToDatabaseProcess() : this("users.csv")
        {
            
        }

        public UserCsvToDatabaseProcess(string userFile)
        {
            _userFile = userFile;
        }

        protected override void Initialize()
        {
            Register(new PullUsersFromFileOperation(_userFile));
            Register(new WriteUsersToDatabase("localRhinoEtlCommand"));
        }
    }

    public class WriteUsersToDatabase : ConventionOutputCommandOperation
    {
        public WriteUsersToDatabase(string connectionStringName) : base(connectionStringName)
        {
            Command = "insert into users (FirstName, LastName, Age, Sex) values (@FirstName, @LastName, @Age, @Sex)";
        }
    }

    public class PullUsersFromFileOperation : AbstractOperation
    {
        private readonly string _fileName;

        public PullUsersFromFileOperation(string fileName)
        {
            _fileName = fileName;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            using (FileEngine file = FluentFile.For<User>().From(_fileName))
            {
                foreach (User user in file)
                    yield return Row.FromObject(user);
            }
        }
    }
}
