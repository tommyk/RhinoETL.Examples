using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;

namespace RhinoETL.Examples
{
    [TestFixture]
    public class OperationTests
    {
        [Test]
        public void only_even_numbers_are_output()
        {
            IEnumerable<Row> oneThroughTen = Enumerable.Range(1, 10).Select(i =>
                {
                    var row = new Row();
                    row["number"] = i;
                    return row;
                });

            var operation = new EvenNumbersOperation();
            IEnumerable<Row> result = operation.Execute(oneThroughTen);

            foreach (Row row in result)
            {
                Assert.That((int)row["number"] % 2, Is.EqualTo(0));
            }
            
        }
    }

    class EvenNumbersOperation : AbstractOperation
    {
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            return rows.Where(row => (int) row["number"]%2 == 0);
        }
    }
}
