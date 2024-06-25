using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ASaltyPeter.TrabalhoAED1
{
    class Program
    {
        private static readonly string nomeMedico = "Dr. Renata";
        private static readonly string diretorioHistoricoAgendamentos = "..\\Historico\\Agendamentos";

        static async Task Main(string[] args)
        {
            while (true)
            {
                if (ConsoleDisponivel())
                {
                    Console.Clear();
                }

                Console.WriteLine("Olá!");
                Console.WriteLine("Qual sua requisição para hoje?");
                Console.WriteLine("Escolha dentre o menu abaixo: ");
                Console.WriteLine("1. Verificar agenda do Dia");
                Console.WriteLine("2. Agendar nova consulta");
                Console.WriteLine("3. Cadastrar paciente");
                Console.WriteLine("4. Ficha digital do paciente");
                Console.WriteLine("5. Atualizar cadastro do paciente");
                Console.WriteLine("6. Atualizar data de consulta");
                Console.WriteLine("7. Sair");
                Console.Write("___________________________________________________ \r\n ");
                Console.Write("Escolha uma opção: \r\n ");

                string opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        await VerificarAgendaAsync();
                        break;
                    case "2":
                        await AgendarConsulta();
                        break;
                    case "3":
                        await CadastrarPaciente();
                        break;
                    case "4":
                        await FichaPaciente();
                        break;
                    case "5":
                        await AtualizarCadastroPaciente();
                        break;
                    case "6":
                        await AtualizarDataConsulta();
                        break;
                    case "7":
                        return;
                    default:
                        Console.WriteLine("Opção inválida! Tente novamente.");
                        break;
                }

                if (!Console.IsInputRedirected)
                {
                    Console.WriteLine("Pressione qualquer tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }

        static bool ConsoleDisponivel()
        {
            try
            {
                int windowHeight = Console.WindowHeight;
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        static async Task VerificarAgendaAsync()
        {
            Console.WriteLine("Verificando agenda do dia...");
            Console.Write("Digite a data que deseja verificar (dd/MM/yyyy): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime dataVerificar))
            {
                Console.WriteLine("Data inválida. Operação cancelada.");
                return;
            }

            string nomeArquivo = $"{dataVerificar.ToString("yyyy-MM-dd")}.txt";
            string caminhoArquivoConsulta = Path.Combine(diretorioHistoricoAgendamentos, nomeArquivo);

            if (File.Exists(caminhoArquivoConsulta))
            {
                Console.WriteLine($"Agenda para {dataVerificar.ToString("dd/MM/yyyy")}:");
                Console.WriteLine();

                using (StreamReader reader = new StreamReader(caminhoArquivoConsulta))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }
                }
            }
            else
            {
                Console.WriteLine($"Não há consultas agendadas para {dataVerificar.ToString("dd/MM/yyyy")}.");
            }

            await Task.CompletedTask;
        }

        static async Task AgendarConsulta()
        {
            try
            {
                Console.WriteLine("Agendando nova consulta...");
                Console.Write("Digite o nome do paciente: ");
                string nomePaciente = Console.ReadLine();

                var paciente = CriadorPaciente.BuscarPacientePorNome(nomePaciente);
                if (paciente == null)
                {
                    Console.WriteLine("Paciente não encontrado. É necessário realizar o cadastro primeiro.");
                    return;
                }

                Console.Write("Digite a data da consulta (dd/MM/yyyy): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime dataConsulta))
                {
                    Console.WriteLine("Erro: Formato de data inválido.");
                    return;
                }

                Console.Write("Digite a hora da consulta (HH:mm): ");
                if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan horaConsulta))
                {
                    Console.WriteLine("Erro: Formato de hora inválido.");
                    return;
                }

                DateTime dataHoraConsulta = dataConsulta.Add(horaConsulta);

                paciente.Consultas.Add(new Consulta
                {
                    Data = dataHoraConsulta
                });

                ArquivosUtils.SalvarPacienteEmArquivo(paciente);
                ArquivosUtils.SalvarConsultaEmArquivo(nomePaciente, dataHoraConsulta);

                Console.WriteLine($"Consulta agendada para {nomePaciente} em {dataHoraConsulta}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao agendar consulta: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        static async Task CadastrarPaciente()
        {
            try
            {
                Console.WriteLine("Cadastrando novo paciente...");
                Console.Write("Digite o nome do paciente: ");
                string nomePaciente = Console.ReadLine();

                if (string.IsNullOrEmpty(nomePaciente))
                {
                    Console.WriteLine("Erro: Nome do paciente não pode estar em branco.");
                    return;
                }

                Console.Write("Digite o CPF do paciente: ");
                string cpf = Console.ReadLine();
                if (string.IsNullOrEmpty(cpf))
                {
                    Console.WriteLine("Erro: CPF do paciente não pode estar em branco.");
                    return;
                }

                Console.Write("Digite o logradouro do paciente: ");
                string rua = Console.ReadLine();
                if (string.IsNullOrEmpty(rua))
                {
                    Console.WriteLine("Erro: Logradouro do paciente não pode estar em branco.");
                    return;
                }

                Console.Write("Digite o número da casa do paciente: ");
                string numero = Console.ReadLine();
                if (string.IsNullOrEmpty(numero))
                {
                    Console.WriteLine("Erro: Número da casa do paciente não pode estar em branco.");
                    return;
                }

                Console.Write("Digite o bairro do paciente: ");
                string bairro = Console.ReadLine();
                if (string.IsNullOrEmpty(bairro))
                {
                    Console.WriteLine("Erro: Bairro do paciente não pode estar em branco.");
                    return;
                }

                Console.Write("Digite a cidade do paciente: ");
                string cidade = Console.ReadLine();
                if (string.IsNullOrEmpty(cidade))
                {
                    Console.WriteLine("Erro: Cidade do paciente não pode estar em branco.");
                    return;
                }

                Console.Write("Digite o CEP do paciente: ");
                string cep = Console.ReadLine();
                if (string.IsNullOrEmpty(cep))
                {
                    Console.WriteLine("Erro: CEP do paciente não pode estar em branco.");
                    return;
                }

                Console.Write("Digite a idade do paciente: ");
                if (!int.TryParse(Console.ReadLine(), out int idadePaciente))
                {
                    Console.WriteLine("Erro: Idade do paciente inválida.");
                    return;
                }

                Console.Write("Digite o email do paciente: ");
                string email = Console.ReadLine();
                if (string.IsNullOrEmpty(email))
                {
                    Console.WriteLine("Erro: Email do paciente não pode estar em branco.");
                    return;
                }

                var paciente = CriadorPaciente.CriarPaciente(nomePaciente, cpf, rua, numero, bairro, cidade, cep, idadePaciente, email);
                Console.WriteLine($"Paciente {nomePaciente} cadastrado com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao cadastrar paciente: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        static async Task FichaPaciente()
        {
            Console.WriteLine("Consultando prontuário...");
            Console.Write("Digite o nome ou CPF do paciente: ");
            string consulta = Console.ReadLine();

            if (string.IsNullOrEmpty(consulta))
            {
                Console.WriteLine("Erro: Nome ou CPF do paciente não pode estar em branco.");
                return;
            }

            Paciente paciente = CriadorPaciente.BuscarPaciente(consulta);
            if (paciente != null)
            {
                Console.WriteLine("Ficha digital do paciente:");
                Console.WriteLine($"Nome: {paciente.Nome}");
                Console.WriteLine($"CPF: {paciente.CPF}");
                Console.WriteLine($"Idade: {paciente.Idade}");
                Console.WriteLine($"Email: {paciente.Email}");
                Console.WriteLine($"Endereço: {paciente.Endereco.Logradouro}, {paciente.Endereco.Numero}, {paciente.Endereco.Bairro}, {paciente.Endereco.Cidade}, {paciente.Endereco.CEP}");

                Console.Write("Deseja registrar uma nova consulta? (s/n): ");
                if (Console.ReadLine().ToLower() == "s")
                {
                    await AgendarConsulta();
                }
            }
            else
            {
                Console.WriteLine($"Paciente com nome ou CPF '{consulta}' não encontrado.");
            }

            await Task.CompletedTask;
        }

       static async Task AtualizarCadastroPaciente()
        {
            Console.WriteLine("Atualizando cadastro do paciente...");
            Console.Write("Digite o nome ou CPF do paciente: ");
            string consulta = Console.ReadLine();

            if (string.IsNullOrEmpty(consulta))
            {
                Console.WriteLine("Erro: Nome ou CPF do paciente não pode estar em branco.");
                return;
            }

            Paciente paciente = CriadorPaciente.BuscarPaciente(consulta);
            if (paciente != null)
            {
                Console.WriteLine("Dados atuais do paciente:");
                Console.WriteLine($"1. Nome: {paciente.Nome}");
                Console.WriteLine($"2. CPF: {paciente.CPF}");
                Console.WriteLine($"3. Idade: {paciente.Idade}");
                Console.WriteLine($"4. Email: {paciente.Email}");
                Console.WriteLine($"5. Endereço: {paciente.Endereco.Logradouro}, {paciente.Endereco.Numero}, {paciente.Endereco.Bairro}, {paciente.Endereco.Cidade}, {paciente.Endereco.CEP}");
                Console.WriteLine("Digite o número do campo que deseja atualizar (ou pressione Enter para sair):");

                while (true)
                {
                    string campo = Console.ReadLine();
                    if (string.IsNullOrEmpty(campo)) break;

                    switch (campo)
                    {
                        case "1":
                            Console.Write("Digite o novo nome: ");
                            paciente.Nome = Console.ReadLine();
                            break;
                        case "2":
                            Console.Write("Digite o novo CPF: ");
                            paciente.CPF = Console.ReadLine();
                            break;
                        case "3":
                            Console.Write("Digite a nova idade: ");
                            if (int.TryParse(Console.ReadLine(), out int novaIdade))
                            {
                                paciente.Idade = novaIdade;
                            }
                            else
                            {
                                Console.WriteLine("Idade inválida.");
                            }
                            break;
                        case "4":
                            Console.Write("Digite o novo email: ");
                            paciente.Email = Console.ReadLine();
                            break;
                        case "5":
                            Console.Write("Digite o novo logradouro: ");
                            paciente.Endereco.Logradouro = Console.ReadLine();
                            Console.Write("Digite o novo número: ");
                            paciente.Endereco.Numero = Console.ReadLine();
                            Console.Write("Digite o novo bairro: ");
                            paciente.Endereco.Bairro = Console.ReadLine();
                            Console.Write("Digite a nova cidade: ");
                            paciente.Endereco.Cidade = Console.ReadLine();
                            Console.Write("Digite o novo CEP: ");
                            paciente.Endereco.CEP = Console.ReadLine();
                            break;
                        default:
                            Console.WriteLine("Opção inválida. Tente novamente.");
                            break;
                    }

                    Console.WriteLine("Digite o número do próximo campo que deseja atualizar (ou pressione Enter para sair):");
                }

                ArquivosUtils.SalvarPacienteEmArquivo(paciente);
                Console.WriteLine("Dados do paciente atualizados com sucesso.");
            }
            else
            {
                Console.WriteLine($"Paciente com nome ou CPF '{consulta}' não encontrado.");
            }

            await Task.CompletedTask;
        }
static async Task AtualizarDataConsulta()
{
    try
    {
        Console.WriteLine("Atualizando data de consulta...");
        Console.Write("Digite o nome do paciente: ");
        string nomePaciente = Console.ReadLine();

        var paciente = CriadorPaciente.BuscarPacientePorNome(nomePaciente);
        if (paciente == null)
        {
            Console.WriteLine("Paciente não encontrado.");
            return;
        }

        Console.WriteLine("Consultas agendadas:");
        for (int i = 0; i < paciente.Consultas.Count; i++)
        {
            Console.WriteLine($"{i + 1}. Data: {paciente.Consultas[i].Data}");
        }

        Console.Write("Selecione o número da consulta que deseja atualizar a data: ");
        if (!int.TryParse(Console.ReadLine(), out int indiceConsulta) || indiceConsulta < 1 || indiceConsulta > paciente.Consultas.Count)
        {
            Console.WriteLine("Consulta inválida.");
            return;
        }

        Console.Write("Digite a nova data da consulta (dd/MM/yyyy): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime novaDataConsulta))
        {
            Console.WriteLine("Formato de data inválido.");
            return;
        }

        Console.Write("Digite a nova hora da consulta (HH:mm): ");
        if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan novaHoraConsulta))
        {
            Console.WriteLine("Formato de hora inválido.");
            return;
        }

        DateTime novaDataHoraConsulta = novaDataConsulta.Date.Add(novaHoraConsulta);

       
        paciente.Consultas[indiceConsulta - 1].Data = novaDataHoraConsulta;
        ArquivosUtils.SalvarPacienteEmArquivo(paciente);

        
        string nomeArquivo = $"{novaDataHoraConsulta.ToString("yyyy-MM-dd")}.txt";
        string caminhoArquivoConsulta = Path.Combine(diretorioHistoricoAgendamentos, nomeArquivo);

        if (!File.Exists(caminhoArquivoConsulta))
        {
           
            using (StreamWriter writer = File.CreateText(caminhoArquivoConsulta))
            {
           
                writer.WriteLine(novaDataHoraConsulta.ToString("dd/MM/yyyy"));

                writer.WriteLine(paciente.Nome);

                writer.WriteLine(novaDataHoraConsulta.ToString("HH:mm"));
            }

            Console.WriteLine($"Arquivo {caminhoArquivoConsulta} criado com a nova consulta.");
        }
        else
        {
          
            List<string> linhas = File.ReadAllLines(caminhoArquivoConsulta).ToList();
            linhas[0] = novaDataHoraConsulta.ToString("dd/MM/yyyy");
            linhas[1] = paciente.Nome;
            linhas[2] = novaDataHoraConsulta.ToString("HH:mm");

            File.WriteAllLines(caminhoArquivoConsulta, linhas);

            Console.WriteLine($"Arquivo {caminhoArquivoConsulta} atualizado com a nova consulta.");
        }

        Console.WriteLine($"Data da consulta atualizada para {novaDataHoraConsulta}.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao atualizar data de consulta: {ex.Message}");
    }

    await Task.CompletedTask;
}
    }
}