
namespace TotallyMoney.Api.Tests;

[TestFixture]
public class PreferenceConverterTests
{
    [Test]
    [TestCaseSource(nameof(TestSerializeTestCases))]
    public void TestSerialize(OneOf<int, int[], bool> preference, string expected)
    {
        var json = JsonConvert.SerializeObject(preference, new PreferenceConverter());
        Assert.AreEqual(expected, json);
    }

    public static IEnumerable<TestCaseData> TestSerializeTestCases
    {
        get
        {
            yield return new TestCaseData(
                (OneOf<int, int[], bool>)1,
                @"{""type"":""specificDate"",""specificDate"":1}").SetName("Serialize specificDate");
            yield return new TestCaseData(
                (OneOf<int, int[], bool>)(new[] { 1, 2, 3 }),
                @"{""type"":""daysOfWeek"",""daysOfWeek"":[1,2,3]}").SetName("Serialize daysOfWeek");
            yield return new TestCaseData(
                (OneOf<int, int[], bool>)true,
                @"{""type"":""everyDay""}").SetName("Serialize everyDay");
            yield return new TestCaseData(
                (OneOf<int, int[], bool>)false,
                @"{""type"":""never""}").SetName("Serialize never");
        }
    }

    [Test]
    [TestCaseSource(nameof(TestDeserializeTestCases))]
    public void TestDeserialize(string preference, OneOf<int, int[], bool> expected)
    {
        var obj = JsonConvert.DeserializeObject<OneOf<int, int[], bool>>(preference, new PreferenceConverter());
        Assert.AreEqual(expected.Index, obj.Index);
    }

    public static IEnumerable<TestCaseData> TestDeserializeTestCases
    {
        get
        {
            yield return new TestCaseData(
                @"{""type"":""specificDate"",""specificDate"":1}",
                (OneOf<int, int[], bool>)1).SetName("Deserialize specificDate");
            yield return new TestCaseData(
                @"{""type"":""daysOfWeek"",""daysOfWeek"":[1,2,3]}",
                (OneOf<int, int[], bool>)(new[] { 1, 2, 3 })).SetName("Deserialize daysOfWeek");
            yield return new TestCaseData(
                @"{""type"":""everyDay""}",
                (OneOf<int, int[], bool>)true).SetName("Deserialize everyDay");
            yield return new TestCaseData(
                @"{""type"":""never""}",
                (OneOf<int, int[], bool>)false).SetName("Deserialize never");
        }
    }

}
