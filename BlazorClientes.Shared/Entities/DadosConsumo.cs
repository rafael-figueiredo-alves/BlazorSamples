namespace BlazorClientes.Shared.Entities
{
    public class DadosConsumo
    {
        public DateTime UltimaResposta { get; private set; }
        public int QtdRequesicoes { get; private set; }

        public DadosConsumo(DateTime ultimaResposta, int qtdRequesicoes)
        {
            UltimaResposta = ultimaResposta;
            QtdRequesicoes = qtdRequesicoes;
        }

        public bool AtingiuConsumo(int TempoEmSegundos, int RequesicoesMax)
            => DateTime.UtcNow < UltimaResposta.AddSeconds(TempoEmSegundos) && QtdRequesicoes == RequesicoesMax;

        public void IncrementarRequesicoes(int RequesicoesMax)
        {
            UltimaResposta = DateTime.UtcNow;

            if (QtdRequesicoes == RequesicoesMax)
                QtdRequesicoes = 1;

            else
                QtdRequesicoes++;
        }
    }
}
