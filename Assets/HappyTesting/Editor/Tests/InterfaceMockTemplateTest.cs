using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace HappyTesting.Editor.Tests {
    public class InterfaceMockTemplateTest {
        [Test]
        public void GetObservablePairTest() {
            var code = Generator.GetObservablePair("Unit", "Hoge");
            Assert.That(code.Contains(@"public IObservable<Unit> Hoge => HogeSubject;"), Is.True, code);
            Assert.That(code.Contains(@"public readonly Subject<Unit> HogeSubject = new();"), Is.True, code);
        }
        [Test]
        public void GetIReadonlyReactivePropertyObservablePairTest() {
            var code = Generator.GetIReadonlyReactivePropertyPair("int", "Fuga");
            Assert.That(code.Contains(@"public IReadonlyReactiveProperty<int> Fuga => FugaSubject.ToReadonlyReactiveProperty();"), Is.True, code);
            Assert.That(code.Contains(@"public readonly Subject<int> FugaSubject = new();"), Is.True, code);
        }
        [Test]
        public void GetSetterResultListTest() {
            var code = Generator.GetSetterResultList("SetHoge",
                new[] { ("int", "val1"), ("float", "val2") });
            Assert.That(code.Contains(@"public int SetHogeVal1Result { private set; get; }"), Is.True, code);
            Assert.That(code.Contains(@"public float SetHogeVal2Result { private set; get; }"), Is.True, code);
        }
        [Test]
        public void GetSetterArgsTest() {
            var code = Generator.GetSetterArgs("SetHoge",
                new[] { ("int", "val1"), ("float", "val2") });
            Assert.That(code.Contains(@"public void SetHoge(int val1, float val2)"), Is.True, code);
        }
        [Test]
        public void GetSetterBodyTest() {
            var code = Generator.GetSetterBody("SetHoge",
                new[] { ("int", "val1"), ("float", "val2") });
            Assert.That(code.Contains(@"  SetHogeVal1Result = val1;"), Is.True, code);
            Assert.That(code.Contains(@"  SetHogeVal2Result = val2;"), Is.True, code);
        }
        [Test]
        public void GetSetterPairTest() {
            var code = Generator.GetSetterPair("SetHoge",
                new[] { ("int", "val1"), ("float", "val2") });
            Debug.Log(code);
        }
    }
}
