using System;
using Rhino.Etl.Core;

namespace RhinoETL.Examples
{
    public class EasyEtlProcess : EtlProcess
    {
        private readonly Action<EtlProcess> _setup;

        public EasyEtlProcess(Action<EtlProcess> setup)
        {
            _setup = setup;
        }

        protected override void Initialize()
        {
            _setup(this);
        }
    }
}