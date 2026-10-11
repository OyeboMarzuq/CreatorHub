using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Application.Interfaces.Importers
{
    public interface IPlatformImporter
    {
        string PlatformSlug { get; }

        Task ImportAsync(CancellationToken cancellationToken);
    }
}
