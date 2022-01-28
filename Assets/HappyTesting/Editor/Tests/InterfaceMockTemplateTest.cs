using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace HappyTesting.Editor.Tests
{
    public class InterfaceMockTemplateTest
    {
        [Test]
        public void InterfaceMockTemplateTestSimplePasses()
        {
            Generator.GenerateTestTemplate();
        }
    }
}
