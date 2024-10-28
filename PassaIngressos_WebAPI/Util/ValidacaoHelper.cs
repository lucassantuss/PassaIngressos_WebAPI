namespace PassaIngressos_WebAPI.Util
{
    public class ValidacaoHelper
    {
        public bool IsValidCPF(string cpf)
        {
            // Remove caracteres não numéricos
            cpf = cpf.Replace(".", "").Replace("-", "").Trim();

            // Verifica se o CPF tem 11 dígitos
            if (cpf.Length != 11 || !long.TryParse(cpf, out _))
                return false;

            // Verifica se todos os dígitos são iguais (ex: 111.111.111-11)
            if (cpf.All(c => c == cpf[0]))
                return false;

            // Validação dos dígitos verificadores
            int[] cpfArray = cpf.Select(c => (int)char.GetNumericValue(c)).ToArray();

            // Cálculo do primeiro dígito verificador
            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += cpfArray[i] * (10 - i);
            int firstDigit = (sum * 10) % 11;
            if (firstDigit == 10) firstDigit = 0;

            // Verifica o primeiro dígito verificador
            if (firstDigit != cpfArray[9])
                return false;

            // Cálculo do segundo dígito verificador
            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += cpfArray[i] * (11 - i);
            int secondDigit = (sum * 10) % 11;
            if (secondDigit == 10) secondDigit = 0;

            // Verifica o segundo dígito verificador
            return secondDigit == cpfArray[10];
        }

        public int CalcularIdade(DateTime dataNascimento)
        {
            var hoje = DateTime.UtcNow;
            int idade = hoje.Year - dataNascimento.Year;

            // Verifica se já fez aniversário este ano
            if (hoje < dataNascimento.AddYears(idade))
                idade--;

            return idade;
        }
    }
}
