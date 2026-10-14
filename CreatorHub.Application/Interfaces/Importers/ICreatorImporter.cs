using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Application.Interfaces.Importers
{
    public interface ICreatorImporter
    {
        Task ImportAsync(CancellationToken cancellationToken = default);

        Task ImportAsync(
        int batchSize,
        CancellationToken cancellationToken = default);
    }
}
