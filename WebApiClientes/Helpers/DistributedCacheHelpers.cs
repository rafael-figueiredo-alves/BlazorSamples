using BlazorClientes.Shared.Entities;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace WebApiClientes.Helpers
{
    /// <summary>
    /// Helper do Distributed Cache
    /// </summary>
    public static class DistributedCacheHelpers
    {
        /// <summary>
        /// Método para pegar os dados de consumo do usuário baseado na chave de API
        /// </summary>
        /// <param name="Cache">Servido IDistributed Cache</param>
        /// <param name="APIKey">Chave de API</param>
        /// <param name="cancellation">Token de cancelamento</param>
        /// <returns>Dados de consumo</returns>
        public static async Task<DadosConsumo?> GetDadosConsumoAsync(this IDistributedCache Cache, string APIKey, CancellationToken cancellation = default)
        {
            var Result = await Cache.GetStringAsync(APIKey, cancellation);
            if (Result is null)
                return null;

            return JsonSerializer.Deserialize<DadosConsumo>(Result);
        }

        public static async Task SaveDados
    }
}
