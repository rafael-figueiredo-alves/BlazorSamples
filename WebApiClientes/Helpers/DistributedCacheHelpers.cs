using BlazorClientes.Shared.Entities;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
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

        /// <summary>
        /// Função para salvar no cache os dados do consumo atualizado
        /// </summary>
        /// <param name="cache">Serviço IDistributed cache</param>
        /// <param name="Key">API Key</param>
        /// <param name="dadosConsumo">Dados de Consumo</param>
        /// <param name="cancellation">Token de cancelamento</param>
        /// <returns>Nada</returns>
        public static async Task SaveDadosConsumoAsync(this IDistributedCache cache, string Key, DadosConsumo? dadosConsumo, CancellationToken cancellation = default)
        {
            dadosConsumo ??= new DadosConsumo(DateTime.UtcNow, 1);
            await cache.SetStringAsync(Key, JsonSerializer.Serialize(dadosConsumo), cancellation);
        }
    }
}
