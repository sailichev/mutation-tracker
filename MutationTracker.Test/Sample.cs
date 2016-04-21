using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MutationTracker.Test
{
    [TestClass]
    public class Sample
    {
        [TestMethod]
        public void TestSeparate()
        {
            var marx = new Person
            {
                Name = "Karl Marx",
                Age = 50,
                Address = new Address
                {
                    Street = "Webber Street",
                    Building = 1,
                },
            };

            var hasChanges = marx.TrackFields();

            marx.Age = 51;
            Assert.IsTrue(hasChanges());

            marx.Age = 50;
            Assert.IsFalse(hasChanges());

            marx.Address.Building = 2;
            Assert.IsTrue(hasChanges());

            marx.Address.Building = 1;
            Assert.IsFalse(hasChanges());
        }

        [TestMethod]
        public void TestEmbedded()
        {
            var companies = new Company[]
            {
                new Company
                {
                    Name = "Your Dream Ltd.",
                    Rating = 4.5m,
                    Address = new Address
                    {
                        Street = "Brixton Road",
                        Building = 1,
                    },
                },
                new Company
                {
                    Name = "Their Dream Ltd.",
                    Rating = 4.6m,
                    Address = new Address
                    {
                        Street = "Great Dover Street",
                        Building = 1,
                    },
                },
            };

            foreach (var item in companies)
                item.Track();

            companies[0].Rating += 0.2m;
            Assert.AreEqual(1, companies.Count(_ => _.IsModified));

            companies[0].Rating -= 0.2m;
            Assert.AreEqual(0, companies.Count(_ => _.IsModified));
        }
    }


    public struct Address : IEquatable<Address>
    {
        public string Street { get; set; }
        public int Building { get; set; }

        public bool Equals(Address other)
        {
            return
                this.Street.Equals(other.Street) &&
                this.Building.Equals(other.Building);
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public Address Address;
    }

    public class Company
    {
        public string Name { get; set; }
        public decimal Rating { get; set; }

        public Address Address;

        #region Modifications Tracking

        private Func<bool> isModified = () => false; // returning default value

        public bool IsModified => this.isModified();

        public void Track() => this.isModified = this.TrackFields(); //  set to the actual tracker func

        #endregion
    }
}