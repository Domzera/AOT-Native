using System.Text.RegularExpressions;
using WebApplication1.DTO;

namespace WebApplication1.Service
{
    /// <summary>
    /// A classe RegisterValidation é responsável por validar os dados fornecidos durante o registro de um usuário.
    /// </summary>
    public class RegisterValidation
    {
        /// <summary>
        /// Quando um registro de usuário é solicitado, este método valida os dados fornecidos.
        /// </summary>
        /// <param name="user"> USER é o bojeto que vem do front para cadastro com os dados do susário.</param>
        /// <returns> Retorna TRUE quando verdadeiro e FALSO quando ha erro no cadastro</returns>
        public bool RegisterUserValidation(RegisterUserDTO user)
        {
            /// VALIDATIONS
            /// O VALID é uma variável que controla se todas as validações foram aprovadas.
            /// Caso todos os dados estejam corretos, o valor dela permanecerá como 1.
            /// E o RETURN no final do método retornará TRUE.
            /// Caso haja algum erro , o valor dela será alterado para 0.
            /// O que fará o RETURN no final do método retornar FALSE.
            int valid = 1;

            /// Validação do NAME não está vazio e tem entre 3 e 50 caracteres.
            if (!string.IsNullOrWhiteSpace(user.Name) && user.Name.Length >= 3 && user.Name.Length < 50)
            {
                valid = 1;
            }
            /// Se o NAME estiver vazio, o valor de VALID é alterado para 0.
            else
            {
                valid = 0;
            }

            /// Validação do EMAIL não está vazio, contém "@" e tem menos de 100 caracteres.
            if (!string.IsNullOrWhiteSpace(user.Email) && user.Email.Contains("@") && user.Email.Length < 100)
            {
                valid = 1;
            }
            /// Se o EMAIL estiver vazio, o valor de VALID é alterado para 0.
            else
            {
                valid = 0;
            }

            /// Validação do PASSWORD não está vazio, tem entre 6 e 12 caracteres, contém pelo menos uma
            /// letra maiúscula, uma letra minúscula, um número e um caractere especial.
            if (!string.IsNullOrWhiteSpace(user.Password) &&
                user.Password.Length >= 6 &&
                user.Password.Length <= 12 &&
                Regex.IsMatch(user.Password, "[A-Z]") &&     // Pelo menos uma maiúscula
                Regex.IsMatch(user.Password, "[a-z]") &&     // Pelo menos uma minúscula
                Regex.IsMatch(user.Password, "[0-9]") &&     // Pelo menos um número
                Regex.IsMatch(user.Password, "[^a-zA-Z0-9]") // Pelo menos um caractere especial
                )
            {
                valid = 1;
            }
            /// Se o PASSWORD estiver vazio, o valor de VALID é alterado para 0.
            else
            {
                valid = 0;
            }

            /// Validação do CPF não está vazio e é válido.
            /// O CPF é validado através do algoritmo oficial.
            if (!string.IsNullOrWhiteSpace(user.CPF))
            {
                /// Retira todos os espaços, pontos e traços do CPF.
                var cpf = user.CPF.Trim();
                cpf = cpf.Replace(".", "").Replace("-", "");

                /// Verifica se o CPF tem 11 dígitos.
                if (cpf.Length != 11)
                {
                    return false;
                }

                ///Separa os 9 primeiros dígitos do CPF para o cálculo dos dígitos verificadores.
                var tempCpf = cpf.Substring(0, 9);
                var soma = 0;
                var multiplicador = 10;

                /// Cálculo do primeiro dígito verificador.
                /// Pega os 9 primeiros dígitos e multiplica cada um por um peso(MULTIPLICADOR) que vai de 10 a 2.
                for (int i = 0; i <= 8; i++)
                {
                    soma += int.Parse(tempCpf[i].ToString()) * multiplicador;
                    multiplicador--;
                }
                /// Depois de somar, será realizado o cálculo do dígito verificador.
                /// Cálculo do resto da divisão por 11.
                var resto = soma % 11;
                /// Se o resto for menor que 2, o dígito é 0.
                if (resto < 2)
                {
                    resto = 0;
                }
                /// Se o resto for maior ou igual a 2, o dígito é 11 menos o resto.
                else
                {
                    resto = 11 - resto;
                }
                /// Concatena o primeiro dígito verificador ao tempCpf.
                /// Salva o primeiro valor do digito para verificação final
                var digito = resto.ToString();
                tempCpf += digito;

                resto = 0;
                soma = 0;
                multiplicador = 11;
                /// Cálculo do segundo dígito verificador.
                /// Pega os 10 primeiros dígitos (9 iniciais + 1º dígito verificador) e
                /// multiplica cada um por um peso(MULTIPLICADOR) que vai de 11 a 2.
                for (int i = 0; i <= 9; i++)
                {
                    soma += int.Parse(tempCpf[i].ToString()) * multiplicador;
                    multiplicador--;
                }
                /// Depois de somar, será realizado o cálculo do dígito verificador.
                /// Cálculo do resto da divisão por 11.
                resto = soma % 11;
                /// Se o resto for menor que 2, o dígito é 0.
                if (resto < 2)
                {
                    resto = 0;
                }
                /// Se o resto for maior ou igual a 2, o dígito é 11 menos o resto.
                else
                {
                    resto = 11 - resto;
                }

                /// Concatena o segundo dígito verificador ao digito para a verificação final.
                /// Salva o segundo valor do digito para verificação final
                digito += resto.ToString();


                /// Verifica se os dígitos calculados são iguais aos dígitos fornecidos no CPF.
                /// Caso esteja ceto, o CPF é válido e o valor de VALID permanece 1.
                /// caso contrário, o CPF é inválido e o valor de VALID é alterado para 0.
                if (cpf.EndsWith(digito))
                {
                    valid = 1;
                }
                else
                {
                    valid = 0;
                }
            }
            /// Se o CPF estiver vazio, o valor de VALID é alterado para 0.
            else
            {
                valid = 0;
            }

            //RETURN
            /// Retorna TRUE se todas as validações foram aprovadas (VALID = 1).
            /// Caso contrário, retorna FALSE (VALID = 0).
            if (valid == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Quando um usuáio é alterado, este método valida os dados fornecidos.
        /// Antes de iniciar os testes de validação, verifica-se es todos os campos foram preenchidos.
        /// Caso algum campo esteja vazio, o método não avalia os dados, pois intende-se que esses dados não serão alterados.
        /// Caso contrário, se todo o campo estiver preenchido, o método prossegue com as validações.
        /// </summary>
        /// <param name="user"> USER é o bojeto que vem do front para cadastro com os dados do susário.</param>
        /// <returns> Retorna TRUE quando verdadeiro e FALSO quando ha erro no cadastro</returns>
        public bool UpdateUserValidation(UserUpdateDTO user)
        {
            /// VALIDATIONS
            /// O VALID é uma variável que controla se todas as validações foram aprovadas.
            /// Caso todos os dados estejam corretos, o valor dela permanecerá como 1.
            /// E o RETURN no final do método retornará TRUE.
            /// Caso haja algum erro , o valor dela será alterado para 0.
            /// O que fará o RETURN no final do método retornar FALSE.
            int valid = 1;

            /// Verifica se o campo NAME foi preenchido.
            /// Caso o NAME esteja vazio, o método não realiza a validação desse campo, pois intende-se
            /// que o usuário não deseja alterar esse dado.
            if (!string.IsNullOrEmpty(user.Name))
            {
                /// Validação do NAME não está vazio e tem entre 3 e 50 caracteres.
                if (!string.IsNullOrWhiteSpace(user.Name) && user.Name.Length >= 3 && user.Name.Length < 50)
                {
                    valid = 1;
                }
            }
            /// Se o NAME estiver vazio, o valor de VALID é alterado para 0.
            else
            {
                valid = 0;
            }

            /// Verifica se o campo EMAIL foi preenchido.
            /// Caso o EMAIL esteja vazio, o método não realiza a validação desse campo, pois intende-se
            /// que o usuário não deseja alterar esse dado.
            if (!string.IsNullOrEmpty(user.Email))
            {
                /// Validação do EMAIL não está vazio, contém "@" e tem menos de 100 caracteres.
                if (!string.IsNullOrWhiteSpace(user.Email) && user.Email.Contains("@") && user.Email.Length < 100)
                {
                    valid = 1;
                }
            }
            /// Se o EMAIL estiver vazio, o valor de VALID é alterado para 0.
            else
            {
                valid = 0;
            }

            /// Verifica se o campo PASSWORD foi preenchido.
            /// Caso o PASSWORD esteja vazio, o método não realiza a validação desse campo, pois intende-se
            /// que o usuário não deseja alterar esse dado.
            if (!string.IsNullOrEmpty(user.Password))
            {
                /// Validação do PASSWORD não está vazio, tem entre 6 e 12 caracteres, contém pelo menos uma
                /// letra maiúscula, uma letra minúscula, um número e um caractere especial.
                if (!string.IsNullOrWhiteSpace(user.Password) &&
                    user.Password.Length >= 6 &&
                    user.Password.Length <= 12 &&
                    Regex.IsMatch(user.Password, "[A-Z]") &&     // Pelo menos uma maiúscula
                    Regex.IsMatch(user.Password, "[a-z]") &&     // Pelo menos uma minúscula
                    Regex.IsMatch(user.Password, "[0-9]") &&     // Pelo menos um número
                    Regex.IsMatch(user.Password, "[^a-zA-Z0-9]") // Pelo menos um caractere especial
                    )
                {
                    valid = 1;
                }
            }
            /// Se o PASSWORD estiver vazio, o valor de VALID é alterado para 0.
            else
            {
                valid = 0;
            }

            /// Verifica se o campo CPF foi preenchido.
            /// Caso o CPF esteja vazio, o método não realiza a validação desse campo, pois intende-se
            /// que o usuário não deseja alterar esse dado.
            if (!string.IsNullOrEmpty(user.CPF))
            {
                /// Validação do CPF não está vazio e é válido.
                /// O CPF é validado através do algoritmo oficial.
                if (!string.IsNullOrWhiteSpace(user.CPF))
                {
                    /// Retira todos os espaços, pontos e traços do CPF.
                    var cpf = user.CPF.Trim();
                    cpf = cpf.Replace(".", "").Replace("-", "");

                    /// Verifica se o CPF tem 11 dígitos.
                    if (cpf.Length != 11)
                    {
                        return false;
                    }

                    ///Separa os 9 primeiros dígitos do CPF para o cálculo dos dígitos verificadores.
                    var tempCpf = cpf.Substring(0, 9);
                    var soma = 0;
                    var multiplicador = 10;

                    /// Cálculo do primeiro dígito verificador.
                    /// Pega os 9 primeiros dígitos e multiplica cada um por um peso(MULTIPLICADOR) que vai de 10 a 2.
                    for (int i = 0; i <= 8; i++)
                    {
                        soma += int.Parse(tempCpf[i].ToString()) * multiplicador;
                        multiplicador--;
                    }
                    /// Depois de somar, será realizado o cálculo do dígito verificador.
                    /// Cálculo do resto da divisão por 11.
                    var resto = soma % 11;
                    /// Se o resto for menor que 2, o dígito é 0.
                    if (resto < 2)
                    {
                        resto = 0;
                    }
                    /// Se o resto for maior ou igual a 2, o dígito é 11 menos o resto.
                    else
                    {
                        resto = 11 - resto;
                    }
                    /// Concatena o primeiro dígito verificador ao tempCpf.
                    /// Salva o primeiro valor do digito para verificação final
                    var digito = resto.ToString();
                    tempCpf += digito;

                    resto = 0;
                    soma = 0;
                    multiplicador = 11;
                    /// Cálculo do segundo dígito verificador.
                    /// Pega os 10 primeiros dígitos (9 iniciais + 1º dígito verificador) e
                    /// multiplica cada um por um peso(MULTIPLICADOR) que vai de 11 a 2.
                    for (int i = 0; i <= 9; i++)
                    {
                        soma += int.Parse(tempCpf[i].ToString()) * multiplicador;
                        multiplicador--;
                    }
                    /// Depois de somar, será realizado o cálculo do dígito verificador.
                    /// Cálculo do resto da divisão por 11.
                    resto = soma % 11;
                    /// Se o resto for menor que 2, o dígito é 0.
                    if (resto < 2)
                    {
                        resto = 0;
                    }
                    /// Se o resto for maior ou igual a 2, o dígito é 11 menos o resto.
                    else
                    {
                        resto = 11 - resto;
                    }

                    /// Concatena o segundo dígito verificador ao digito para a verificação final.
                    /// Salva o segundo valor do digito para verificação final
                    digito += resto.ToString();


                    /// Verifica se os dígitos calculados são iguais aos dígitos fornecidos no CPF.
                    /// Caso esteja ceto, o CPF é válido e o valor de VALID permanece 1.
                    /// caso contrário, o CPF é inválido e o valor de VALID é alterado para 0.
                    if (cpf.EndsWith(digito))
                    {
                        valid = 1;
                    }
                    else
                    {
                        valid = 0;
                    }
                }
            }
            /// Se o CPF estiver vazio, o valor de VALID é alterado para 0.
            else
            {
                valid = 0;
            }

            //RETURN
            /// Retorna TRUE se todas as validações foram aprovadas (VALID = 1).
            /// Caso contrário, retorna FALSE (VALID = 0).
            if (valid == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
