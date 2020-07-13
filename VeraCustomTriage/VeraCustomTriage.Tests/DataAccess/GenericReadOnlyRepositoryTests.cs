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
        private string[] jsonPaths = new string[]
        {
            "https://raw.githubusercontent.com/sebcoles/VeraCustomTriage/master/VeraCustomTriage/customtriage.global.json",
            "https://raw.githubusercontent.com/sebcoles/VeraCustomTriage/master/VeraCustomTriage/customtriage.team.json",
            "https://raw.githubusercontent.com/sebcoles/VeraCustomTriage/master/VeraCustomTriage/customtriage.personal.json"
        };

        [Test]
        public void GetAllReturnsAutoResponses()
        {
            var responsesRepository = new GenericReadOnlyRepository<AutoResponse>(jsonPaths);
            Assert.AreEqual(responsesRepository.GetAll().Count(), 3);
        }
    }
}
