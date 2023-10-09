using NUnit.Framework;
using FlaskServer;

public class NonHTTPMethodsTest
{
    private Utilities utils;

    [SetUp]
    public void SetUp()
    {
        utils = new Utilities();
    }

    [Test]
    public void FormatDateTimetoHHmm_ValidInput_FormatsCorrectly()
    {
        string dateTimeStr = "2023-09-27 15:30:45";
        string expected = "15:30";

        string result = utils.FormatDateTimetoHHmm(dateTimeStr);

        Assert.AreEqual(expected, result);
    }

    [Test]
    public void FormatDateTimetoHHmm_InvalidInput_ReturnsEmptyString()
    {
        string dateTimeStr = "InvalidDate";
        string expected = string.Empty;

        string result = utils.FormatDateTimetoHHmm(dateTimeStr);

        Assert.AreEqual(expected, result);
    }

    [Test]
    public void FormatDateTimetoHHmmss_ValidInput_FormatsCorrectly()
    {
        string dateTimeStr = "2023-09-27 15:30:45";
        string expected = "15:30:45";

        string result = utils.FormatDateTimetoHHmmss(dateTimeStr);

        Assert.AreEqual(expected, result);
    }

    [Test]
    public void FormatDateTimetoYMDHHmmss_ValidInput_FormatsCorrectly()
    {
        string dateTimeStr = "2023-09-27 15:30:45";
        string expected = "2023-09-27 15:30:45";

        string result = utils.FormatDateTimetoYMDHHmmss(dateTimeStr);

        Assert.AreEqual(expected, result);
    }
}