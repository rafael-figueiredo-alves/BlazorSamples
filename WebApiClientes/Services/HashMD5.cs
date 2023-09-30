using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;

namespace WebApiClientes.Services
{
    /// <summary>
    /// Serviço Destinado a fazer o Hash e verificar o Hash
    /// </summary>
    public class HashMD5
    {
        private static string GetMd5Hash(byte[] data)
        {
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));
            return sBuilder.ToString();
        }

        private static bool VerifyMd5Hash(byte[] data, string hash)
        {
            return 0 == StringComparer.OrdinalIgnoreCase.Compare(GetMd5Hash(data), hash);
        }

        /// <summary>
        /// Gera Hash do input fornecido
        /// </summary>
        /// <param name="data">String a converter</param>
        /// <returns>Hash MD5</returns>
        public static string Hash(string data)
        {
            using (var md5 = MD5.Create())
                return GetMd5Hash(md5.ComputeHash(Encoding.UTF8.GetBytes(data)));
        }

        /// <summary>
        /// Gera Hash do input fornecido
        /// </summary>
        /// <param name="data">Filestream a converter</param>
        /// <returns>Hash MD5</returns>
        public static string Hash(FileStream data)
        {
            using (var md5 = MD5.Create())
                return GetMd5Hash(md5.ComputeHash(data));
        }

        /// <summary>
        /// Método para verificar o Hash
        /// </summary>
        /// <param name="data">Informação</param>
        /// <param name="hash">Hash da informação para comparar</param>
        /// <returns>True ou False</returns>
        public static bool Verify(string data, string hash)
        {
            using (var md5 = MD5.Create())
                return VerifyMd5Hash(md5.ComputeHash(Encoding.UTF8.GetBytes(data)), hash);
        }

        /// <summary>
        /// Método para verificar o Hash
        /// </summary>
        /// <param name="data">Informação</param>
        /// <param name="hash">Hash da informação para comparar</param>
        /// <returns>True ou False</returns>
        public static bool Verify(FileStream data, string hash)
        {
            using (var md5 = MD5.Create())
                return VerifyMd5Hash(md5.ComputeHash(data), hash);
        }

        /// <summary>
        /// Método para verificar o ETag com Hash do conteúdo a ser enviado
        /// </summary>
        /// <param name="etag">ETag provindo da requisição</param>
        /// <param name="dataHash">Hash do Conteúdo a ser enviado</param>
        /// <returns>Verdadeiro se etag e hash forem iguais</returns>
        public static bool VerifyETag(string etag, string dataHash)
        {
            return 0 == StringComparer.OrdinalIgnoreCase.Compare(etag, dataHash);
        }
    }
}
