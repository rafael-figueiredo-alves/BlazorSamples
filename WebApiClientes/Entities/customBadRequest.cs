using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApiClientes.Entities
{
    /// <summary>
    /// Classe customizada da BadRequest
    /// </summary>
    public class ErroValidacao : ValidationProblemDetails
    {
        /// <summary>
        /// Método construtor
        /// </summary>
        /// <param name="actionContext"></param>
        public ErroValidacao(ActionContext actionContext)
        {
            Title = "Houve um problema com sua requisição";
            Detail = "Um ou mais campos enviados violaram alguma regra de validação. Verifique, corrija e tente novamente.";
            Status = 400;
            ConstructErrorMessages(actionContext);
        }

        /// <summary>
        /// Método construtor dos erros
        /// </summary>
        /// <param name="context"></param>
        public void ConstructErrorMessages(ActionContext context)
        {
            foreach(var keyModelStatePair in context.ModelState)
            {
                var key = keyModelStatePair.Key;
                var errors = keyModelStatePair.Value.Errors;
                if (errors != null && errors.Count > 0) 
                {
                    if (errors.Count == 1)
                    {
                        var errorMessage = GetErrorMessage(errors[0]);
                        Errors.Add(key, new[] { errorMessage });
                    }
                    else
                    {
                        var errorMessages = new string[errors.Count];
                        for (var i = 0; i < errors.Count; i++)
                        {
                            errorMessages[i] = GetErrorMessage(errors[i]);
                        }

                        Errors.Add(key, errorMessages);
                    }
                }
            }
        }

        /// <summary>
        /// Método construtor das mensagens de erros
        /// </summary>
        /// <param name="error">Erro</param>
        /// <returns>Erros</returns>
        private string GetErrorMessage(ModelError error)
        {
            return string.IsNullOrEmpty(error.ErrorMessage) ? "A entrada não é válida." : error.ErrorMessage;
        }
    }
}
