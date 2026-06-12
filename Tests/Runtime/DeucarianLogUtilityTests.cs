using NUnit.Framework;

namespace Deucarian.Logging.Tests
{
    public sealed class DeucarianLogUtilityTests
    {
        [Test]
        public void RedactSensitiveDataRedactsObviousSecrets()
        {
            string input =
                "token=abc123 access_token=def456 password: hunter2; secret=sauce api_key=key789 Authorization: Bearer xyz";

            string redacted = DeucarianLogUtility.RedactSensitiveData(input);

            Assert.IsFalse(redacted.Contains("abc123"));
            Assert.IsFalse(redacted.Contains("def456"));
            Assert.IsFalse(redacted.Contains("hunter2"));
            Assert.IsFalse(redacted.Contains("sauce"));
            Assert.IsFalse(redacted.Contains("key789"));
            Assert.IsFalse(redacted.Contains("Bearer xyz"));
            Assert.IsTrue(redacted.Contains("[REDACTED]"));
        }

        [Test]
        public void RedactSensitiveDataRemovesUrlQueryStrings()
        {
            string input = "GET https://example.com/api/items?token=abc123&id=42";

            string redacted = DeucarianLogUtility.RedactSensitiveData(input);

            Assert.AreEqual("GET https://example.com/api/items?[REDACTED_QUERY]", redacted);
        }

        [Test]
        public void RedactSensitiveDataHandlesJsonStyleValues()
        {
            string input = "{\"password\":\"hunter2\",\"token\":\"abc123\"}";

            string redacted = DeucarianLogUtility.RedactSensitiveData(input);

            Assert.AreEqual("{\"password\":\"[REDACTED]\",\"token\":\"[REDACTED]\"}", redacted);
        }
    }
}
