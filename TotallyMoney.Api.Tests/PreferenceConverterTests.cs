
namespace TotallyMoney.Api.Tests;

[TestFixture]
public class PreferenceConverterTests
{
    [Test]
    [TestCaseSource(nameof(TestSerializeTestCases))]
    public void TestSerialize(OneOf<int, DayOfWeek[], bool> preference, string expected)
    {
        var json = JsonConvert.SerializeObject(preference, new PreferenceConverter());
        Assert.AreEqual(expected, json);
    }

    public static IEnumerable<TestCaseData> TestSerializeTestCases
    {
        get
        {
            yield return new TestCaseData(
                (OneOf<int, DayOfWeek[], bool>)1,
                @"{""type"":""specificDate"",""specificDate"":1}").SetName("Serialize specificDate");
            yield return new TestCaseData(
                (OneOf<int, DayOfWeek[], bool>)(new[] { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday }),
                @"{""type"":""daysOfWeek"",""daysOfWeek"":[""Sunday"",""Monday"",""Tuesday""]}").SetName("Serialize daysOfWeek");
            yield return new TestCaseData(
                (OneOf<int, DayOfWeek[], bool>)true,
                @"{""type"":""everyDay""}").SetName("Serialize everyDay");
            yield return new TestCaseData(
                (OneOf<int, DayOfWeek[], bool>)false,
                @"{""type"":""never""}").SetName("Serialize never");
        }
    }

    [Test]
    [TestCaseSource(nameof(TestDeserializeTestCases))]
    public void TestDeserialize(string preference, OneOf<int, DayOfWeek[], bool> expected)
    {
        var obj = JsonConvert.DeserializeObject<OneOf<int, DayOfWeek[], bool>>(preference, new PreferenceConverter());
        Assert.AreEqual(expected.Index, obj.Index);
    }

    public static IEnumerable<TestCaseData> TestDeserializeTestCases
    {
        get
        {
            yield return new TestCaseData(
                @"{""type"":""specificDate"",""specificDate"":1}",
                (OneOf<int, DayOfWeek[], bool>)1).SetName("Deserialize specificDate");
            yield return new TestCaseData(
                @"{""type"":""daysOfWeek"",""daysOfWeek"":[""Sunday"",""Monday"",""Tuesday""]}",
                (OneOf<int, DayOfWeek[], bool>)(new[] { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday })).SetName("Deserialize daysOfWeek");
            yield return new TestCaseData(
                @"{""type"":""everyDay""}",
                (OneOf<int, DayOfWeek[], bool>)true).SetName("Deserialize everyDay");
            yield return new TestCaseData(
                @"{""type"":""never""}",
                (OneOf<int, DayOfWeek[], bool>)false).SetName("Deserialize never");
        }
    }

}
