using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace RhinoETL.Examples
{
    [TestFixture]
    public class YieldExamples
    {
        [Test]
        public void YieldTest1()
        {
            var enumerable = Helper.CreateEnumerable(5);
        }

        [Test]
        public void YieldTest2()
        {
            foreach (int i in Helper.CreateEnumerable(int.MaxValue))
            {
                if (i == 5) break;
            }
        }

        [Test]
        public void YieldTest3()
        {
            Helper.CreateEnumerable(10).Where(i => i%2 == 0).ToList();
        }


        static class Helper
        {
            internal static IEnumerable<int> CreateEnumerable(int maxVal)
            {
                for (int i = 0; i < maxVal; i++)
                {
                    Console.WriteLine(i);
                    yield return i;
                }
            }
        }
    }
}
