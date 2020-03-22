using System.Text;
using ServiceLayerApi.Common;
using Xunit;

namespace ServiceLayerApi.Tests.UnitTests
{
    public class SerializerTest
    {
        [Fact]
        public void TestSerialize()
        {
            var target = "{\"TestValue\":3}";
            var toSerialize = new TestClass() {TestValue = 3};

            var actual = toSerialize.ToJson();
            
            Assert.Equal(target, actual);
        }

        [Fact]
        public void TestDeserialize()
        {
            var target = new TestClass() {TestValue = 3};
            var source = "{\"TestValue\":3}";

            var actual = source.DeserializeJson<TestClass>();
            
            Assert.Equal(actual.TestValue, target.TestValue);
            Assert.Equal(actual.GetType(), target.GetType());
        }

        [Fact]
        public void TestSerializeToBytes()
        {
            var target = "{\"TestValue\":3}";
            var targetBytes = Encoding.UTF8.GetBytes(target);
            var toSerialize = new TestClass() {TestValue = 3};

            var actual = toSerialize.ToJsonBytes();
            
            Assert.Equal(targetBytes, actual);
        }
        
        [Fact]
        public void TestDeserializeBytes()
        {
            var target = new TestClass() {TestValue = 3};
            var source = "{\"TestValue\":3}";
            var sourceBytes = Encoding.UTF8.GetBytes(source);

            var actual = sourceBytes.DeserializeJsonBytes<TestClass>();
            
            Assert.Equal(actual.TestValue, target.TestValue);
            Assert.Equal(actual.GetType(), target.GetType());
        }

        public class TestClass
        {
            public int TestValue { get; set; }
        }
    }
}