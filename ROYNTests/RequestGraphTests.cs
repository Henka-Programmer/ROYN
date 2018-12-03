using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROYN;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ROYNTests
{
    public class NestedModel
    {
        public string NestedModelName { get; set; }
        public ComplextModel ComplextModel { get; set; }
    }

    public class ComplextModel : TestModel
    {
        public NestedModel NestedModel { get; set; }
    }

    [TestClass]
    public class RequestGraphTests
    {
        [TestMethod]
        public void Select()
        {
            Royn.Configure(typeof(ComplextModel));
            Royn.Configure(typeof(NestedModel));

            var source = new List<ComplextModel>()
            {
                new ComplextModel{ Name="1",Age=1, Date= DateTime.Now, NestedModel = new NestedModel{NestedModelName = "nested", ComplextModel = new ComplextModel{ Name = "oooh", NestedModel = new NestedModel{  NestedModelName = "2222", ComplextModel = new ComplextModel{ Name = "bababa"} } } } },
                new ComplextModel{ Name="2",Age=2, Date= DateTime.Now.AddDays(2), NestedModel = new NestedModel{NestedModelName = "nested", ComplextModel = new ComplextModel{ Name = "oooh"} }},
                new ComplextModel{ Name="3",Age=3, Date= DateTime.Now.AddDays(3), NestedModel = new NestedModel{NestedModelName = "nested", ComplextModel = new ComplextModel{ Name = "oooh"} }},
                new ComplextModel{ Name="4",Age=4, Date= DateTime.Now.AddDays(4), NestedModel = new NestedModel{NestedModelName = "nested" , ComplextModel = new ComplextModel{ Name = "oooh"}}},
            };

            var request = new RoynRequest<ComplextModel>().Add(x => x.Name).Add(x => x.NestedModel.NestedModelName).Add(x=>x.NestedModel.ComplextModel.Name).Add(x=>x.NestedModel.ComplextModel.NestedModel.ComplextModel.Name);
            var graph = RequestGraph.BuildGraph(request);
            var result = RoynHelper.RoynSelect(source.AsQueryable(), graph);
           
            var list = result.GetResult<List<ComplextModel>>();
            Assert.IsTrue(list.All(x => x.Age == 0 && x.Date == DateTime.MinValue && x.Name != string.Empty && x.NestedModel.NestedModelName == "nested"));
        }
    }
}