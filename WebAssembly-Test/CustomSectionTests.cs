using Xunit;
using System;
using System.Linq;

namespace WebAssembly.Test {
    // <summary>
    /// Validates behavior of the <see cref="CustomSection"/> class.
    /// </summary>
    public class CustomSectionTests {
        /// <summary>
        /// Ensures that values supplied to the <see cref="CustomSection.PrecedingSection"/> property are validated.
        /// </summary>
        [Fact]
        public void CustomSection_PrecedingSectionValidity() {
            var custom = new CustomSection();

            foreach (var value in Enum.GetValues(typeof(Section)).Cast<Section>()) {
                //All values of Section should be accepted.
                custom.PrecedingSection = value;
            }

            Assert.Throws<ArgumentOutOfRangeException>(() => custom.PrecedingSection = (Section)255);
        }
    }

}
