using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using VeraCustomTriage.DataAccess.Json;
using VeraCustomTriage.Shared.Models;

namespace VeraCustomTriage.Tests.DataAccess
{
    [TestFixture]
    public class GenericReadOnlyRepositoryTests
    {
        [Test]
        public void GetAllReturnsAutoResponses()
        {
            var settings = new EndpointConfiguration()
            {
                Global = "https://raw.githubusercontent.com/sebcoles/VeraCustomTriage/master/VeraCustomTriage/customtriage.global.json",
                Team = "https://raw.githubusercontent.com/sebcoles/VeraCustomTriage/master/VeraCustomTriage/customtriage.team.json",
                Personal = "https://raw.githubusercontent.com/sebcoles/VeraCustomTriage/master/VeraCustomTriage/customtriage.personal.json"
            };
        
            IOptions<EndpointConfiguration> config = Options.Create(settings);
            var responsesRepository = new GenericReadOnlyRepository<AutoResponse>(config);
            Assert.AreEqual(responsesRepository.GetAll().Count(), 3);
        }
    }
}
