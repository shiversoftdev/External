using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace System.PEStructures
{
    public class PEImage
    {
        public PEReader Reader { get; }
        public PEHeaders Headers => Reader?.PEHeaders;
        public PEExportDirectory Exports { get; }
        public PEImportDirectory Imports { get; }
        public PEImage(Memory<byte> imageData)
        {
            Reader = new PEReader(imageData.ToArray().ToImmutableArray());
            if (Headers?.PEHeader is null) throw new BadImageFormatException("The provided file was not a valid PE");
            Exports = new PEExportDirectory(Headers, imageData);
            Imports = new PEImportDirectory(Headers, imageData);
        }
    }
}
