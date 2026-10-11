using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Application.Interfaces.Importers
{
    public interface IProfileUrlImporter
    {
        Task ImportAsync(CancellationToken cancellationToken = default);
    }
}
