using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROYN;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ROYNTests
{
    [TestClass]
    public class RoynHelperTests
    {
        [TestMethod]
        public void Select()
        {
            var source = new List<TestModel>()
            {
                new TestModel{ Name="1",Age=1, Date= DateTime.Now},
                new TestModel{ Name="2",Age=2, Date= DateTime.Now.AddDays(2)},
                new TestModel{ Name="3",Age=3, Date= DateTime.Now.AddDays(3)},
                new TestModel{ Name="4",Age=4, Date= DateTime.Now.AddDays(4)},
            };

            var query = RoynHelper.Select<TestModel>(source.AsQueryable(), new string[] { nameof(TestModel.Name) });
            var result = query.ToList();
            Assert.IsTrue(result.All(x => x.Age == 0 && x.Date == DateTime.MinValue && x.Name != string.Empty));
        }

       


    }
}