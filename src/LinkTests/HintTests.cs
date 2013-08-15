using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Tavis.IANA;
using Xunit;

namespace LinkTests
{
    public class HintTests
    {

        [Fact]
        public void GivenAnAllowHintWithAMethodAddingItAgainDoesNotFail()
        {
            // Arrange
            var hint = new AllowHint();
            hint.AddMethod(HttpMethod.Get);
            // Act
            hint.AddMethod(HttpMethod.Get);

            //Asset
            // No error occured
            
        }
    }
}
