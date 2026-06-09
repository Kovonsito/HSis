using System.Threading.Tasks;

namespace HSis.Logic.Services
{
    public interface IMaterialService
    {
        Task ActualizarCostoMaterialAsync(int idMaterial, decimal nuevoCosto);
    }
}
