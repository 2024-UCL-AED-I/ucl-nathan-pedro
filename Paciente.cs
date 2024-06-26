using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

public class Paciente
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string CPF { get; set; }
    public Endereco Endereco { get; set; }
    public int Idade { get; set; }
    public string Email { get; set; }
    public List<Consulta> Consultas { get; set; }

    public Paciente()
    {
        Endereco = new Endereco();
        Consultas = new List<Consulta>();
    }
}

public class Endereco
{
    public string Logradouro { get; set; }
    public string Numero { get; set; }
    public string Bairro { get; set; }
    public string Cidade { get; set; }
    public string CEP { get; set; }
}

public class Consulta
{
    public DateTime Data { get; set; }
    public string AcompanhamentoMedico { get; set; }
    public string RecomendacoesMedicas { get; set; }
}

public static class ArquivosUtils
{
    private readonly string diretorioAplicacao = Directory.GetCurrentDirectory();
    public readonly string diretorioDadosPacientes = Path.Combine(diretorioAplicacao, "DadosPacientes");
    private readonly string diretorioHistoricoAgendamentos = Path.Combine(diretorioAplicacao, "Historico", "Agendamentos");
    private readonly string caminhoUltimoId = Path.Combine(diretorioDadosPacientes, "ultimoId.txt");

    public void SalvarPacienteEmArquivo(Paciente paciente)
    {
        try
        {
            if (!Directory.Exists(diretorioDadosPacientes))
            {
                Directory.CreateDirectory(diretorioDadosPacientes);
            }

            string caminhoArquivoPaciente = Path.Combine(diretorioDadosPacientes, $"{paciente.Nome}.json");
            string json = JsonConvert.SerializeObject(paciente, Formatting.Indented);
            File.WriteAllText(caminhoArquivoPaciente, json);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro ao salvar os dados do paciente: {e.Message}");
        }
    }

    public Paciente CarregarPacienteDeArquivo(string nomePaciente)
    {
        try
        {
            string caminhoArquivoPaciente = Path.Combine(diretorioDadosPacientes, $"{nomePaciente}.json");
            if (File.Exists(caminhoArquivoPaciente))
            {
                string json = File.ReadAllText(caminhoArquivoPaciente);
                return JsonConvert.DeserializeObject<Paciente>(json);
            }
            else
            {
                Console.WriteLine($"O arquivo do paciente com nome {nomePaciente} não foi encontrado.");
                return null;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro ao carregar os dados do paciente: {e.Message}");
            return null;
        }
    }

    public int LerUltimoId()
    {
        if (File.Exists(caminhoUltimoId))
        {
            string ultimoIdTexto = File.ReadAllText(caminhoUltimoId);
            if (int.TryParse(ultimoIdTexto, out int ultimoId))
            {
                return ultimoId;
            }
        }
        return 0;
    }

    public static void SalvarUltimoId(int ultimoId)
    {
        File.WriteAllText(caminhoUltimoId, ultimoId.ToString());
    }

 public static void SalvarConsultaEmArquivo(string nomePaciente, DateTime dataConsulta)
    {
        try
        {
            if (!Directory.Exists(diretorioHistoricoAgendamentos))
            {
                Directory.CreateDirectory(diretorioHistoricoAgendamentos);
            }

            string nomeArquivo = $"{dataConsulta.ToString("yyyy-MM-dd")}.txt";
            string caminhoArquivoConsulta = Path.Combine(diretorioHistoricoAgendamentos, nomeArquivo);

            bool primeiraConsultaDoDia = !File.Exists(caminhoArquivoConsulta);

            using (StreamWriter writer = new StreamWriter(caminhoArquivoConsulta, true))
            {
                if (primeiraConsultaDoDia)
                {
                    writer.WriteLine(dataConsulta.ToString("dd/MM/yyyy"));
                }

                writer.WriteLine(nomePaciente);
                writer.WriteLine(dataConsulta.ToString("HH:mm"));
                writer.WriteLine(); 
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro ao salvar a consulta em arquivo: {e.Message}");
        }
    }
}
public static class CriadorPaciente
{
    private int ultimoIdPaciente = ArquivosUtils.LerUltimoId();

    public Paciente CriarPaciente(string nome, string cpf, string rua, string numero, string bairro, string cidade, string cep, int idade, string email)
    {
        Paciente pacienteExistente = BuscarPaciente(nome);
        if (pacienteExistente != null)
        {
            Console.WriteLine("Paciente já cadastrado.");
            return pacienteExistente;
        }

        pacienteExistente = BuscarPacientePorCPF(cpf);
        if (pacienteExistente != null)
        {
            Console.WriteLine("Paciente já cadastrado.");
            return pacienteExistente;
        }

        ultimoIdPaciente++;
        var paciente = new Paciente
        {
            Id = ultimoIdPaciente,
            Nome = nome,
            CPF = cpf,
            Endereco = CriadorEndereco.CriarEndereco(rua, numero, bairro, cidade, cep),
            Idade = idade,
            Email = email
        };

        ArquivosUtils.SalvarPacienteEmArquivo(paciente);
        ArquivosUtils.SalvarUltimoId(ultimoIdPaciente);

        return paciente;
    }

    public Paciente BuscarPaciente(string criterioBusca)
    {
        try
        {
            string[] arquivosPacientes = Directory.GetFiles(ArquivosUtils.diretorioDadosPacientes, "*.json", SearchOption.TopDirectoryOnly);

            foreach (string caminhoArquivo in arquivosPacientes)
            {
                string json = File.ReadAllText(caminhoArquivo);
                Paciente paciente = JsonConvert.DeserializeObject<Paciente>(json);
                if (paciente.Nome.Equals(criterioBusca, StringComparison.OrdinalIgnoreCase) || paciente.CPF == criterioBusca)
                {
                    return paciente;
                }
            }

            Console.WriteLine($"Paciente com critério de busca '{criterioBusca}' não encontrado.");
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro ao buscar paciente: {e.Message}");
            return null;
        }
    }

    public Paciente BuscarPacientePorCPF(string cpf)
    {
        return BuscarPaciente(cpf);
    }

    public Paciente BuscarPacientePorNome(string nome)
    {
        return BuscarPaciente(nome);
    }
}

public static class CriadorEndereco
{
    public Endereco CriarEndereco(string logradouro, string numero, string bairro, string cidade, string cep)
    {
        return new Endereco
        {
            Logradouro = logradouro,
            Numero = numero,
            Bairro = bairro,
            Cidade = cidade,
            CEP = cep
        };
    }
}
