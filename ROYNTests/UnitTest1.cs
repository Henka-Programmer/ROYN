using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROYN;
using System;
using System.Linq;

namespace ROYNTests
{
    [TestClass]
    public class DynamicTypeBuilderTests
    {
        [TestMethod]
        public void PrimitiveTypesPropetiesTests()
        {
            var builder = new DynamicTypeBuilder("SimplePropertiesType");

            // test type
            //public class Test
            //{
            //    public int IntProperty { get; set; }
            //    public double DoubleProperty { get; set; }
            //    public decimal DecimalProperty { get; set; }
            //    public string StringProperty { get; set; }
            //    public DateTime DateTimeProperty { get; set; }
            //}

            builder.DefineProperty("IntProperty", typeof(int));
            builder.DefineProperty("DoubleProperty", typeof(double));
            builder.DefineProperty("DecimalProperty", typeof(decimal));
            builder.DefineProperty("StringProperty", typeof(string));
            builder.DefineProperty("DateTimeProperty", typeof(DateTime));

            var type = builder.CreateType(); 
            Assert.IsNotNull(type);
            var properties = type.GetProperties();
            Assert.AreEqual(5, properties.Length);
            Assert.IsTrue(properties.All(x => x.DeclaringType == type));
            Assert.IsTrue(properties.All(x => x.SetMethod != null));
            Assert.IsTrue(properties.All(x => x.GetMethod != null));
            var intProperty = properties.FirstOrDefault(x => x.Name == "IntProperty");
            var doubleProperty = properties.FirstOrDefault(x => x.Name == "DoubleProperty");
            var decimalProperty = properties.FirstOrDefault(x => x.Name == "DecimalProperty");
            var stringProperty = properties.FirstOrDefault(x => x.Name == "StringProperty");
            var datetimeProperty = properties.FirstOrDefault(x => x.Name == "DateTimeProperty");

            Assert.IsTrue(intProperty != null);
            Assert.AreEqual(typeof(int), intProperty.PropertyType);

            Assert.IsTrue(doubleProperty != null);
            Assert.AreEqual(typeof(double), doubleProperty.PropertyType);

            Assert.IsTrue(decimalProperty != null);
            Assert.AreEqual(typeof(decimal), decimalProperty.PropertyType);

            Assert.IsTrue(stringProperty != null);
            Assert.AreEqual(typeof(string), stringProperty.PropertyType);

            Assert.IsTrue(datetimeProperty != null);
            Assert.AreEqual(typeof(DateTime), datetimeProperty.PropertyType);

            var typeInstance = Activator.CreateInstance(type);
            intProperty.SetValue(typeInstance, 12);
            stringProperty.SetValue(typeInstance, "royn");
            datetimeProperty.SetValue(typeInstance, DateTime.Now.AddYears(10).Date);
            decimalProperty.SetValue(typeInstance, 0.3m);
            doubleProperty.SetValue(typeInstance, 0.48d);

            Assert.AreEqual(12, intProperty.GetValue(typeInstance));
            Assert.AreEqual("royn", stringProperty.GetValue(typeInstance));
            Assert.AreEqual(DateTime.Now.AddYears(10).Date, datetimeProperty.GetValue(typeInstance));
            Assert.AreEqual(0.3m, decimalProperty.GetValue(typeInstance));
            Assert.AreEqual(0.48d, doubleProperty.GetValue(typeInstance));
        }
    }
}