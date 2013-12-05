using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;

namespace RhinoETL.Examples
{
    [TestFixture]
    public class ThreadingExamples
    {
        /// <summary>
        /// Just to show how RhinoETL is naturally multi-threaded.
        /// </summary>
        [Test]
        public void show_how_threads_work_for_each_operation()
        {
            var process = new EasyEtlProcess(etlProcess =>
                {
                    etlProcess.Register(new EnumerateNumbersOperation(20));
                    etlProcess.Register(new PrintThreadOperation("2nd operation"));
                    etlProcess.Register(new SleepOperation(100));
                    etlProcess.Register(new PrintThreadOperation("4th operation"));
                });

            process.Execute();

            var allErrors = process.GetAllErrors();
            if(allErrors.Count() > 0)
                throw new Exception("fail");
        }

    }

    public class SleepOperation : AbstractOperation
    {
        private readonly int _msToSleep;

        public SleepOperation(int msToSleep)
        {
            _msToSleep = msToSleep;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            foreach (Row row in rows)
            {
                Thread.Sleep(_msToSleep);
                yield return row;
            }
        }
    }

    public class PrintThreadOperation : AbstractOperation
    {
        private readonly string _name;

        public PrintThreadOperation(string name)
        {
            _name = name;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            foreach (Row row in rows)
            {
                Console.WriteLine(string.Format("Thread: {0} Name: {1}", Thread.CurrentThread.ManagedThreadId, _name));
                yield return row;
            }
        }
    }


    public class EnumerateNumbersOperation : AbstractOperation
    {
        private readonly int _maxNum;

        public EnumerateNumbersOperation(int maxNum)
        {
            _maxNum = maxNum;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            foreach (int i in Enumerable.Range(0, _maxNum))
            {
                var row = new Row();
                row["number"] = i;
                Console.WriteLine(string.Format("Yielding number: {0}", i));
                yield return row;
            }
        }
    }
}
