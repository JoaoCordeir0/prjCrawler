using HtmlAgilityPack;
using System;

namespace prjCrawler
{
    internal class Program
    {
        private const string URL = "http://www.mcce.org.br/newsletter";

        static void Main(string[] args)
        {
            Console.WriteLine("#### Iniciando a execução ####");

            ExecCrawler();
            
            Console.WriteLine("#### Fim da execução ####");
        }

        private static void ExecCrawler()
        {
            using (var cli = new HttpClient())
            {
                Console.WriteLine("*** Iniciando cliente HTTP ***");

                string html = cli.GetStringAsync(URL).Result;

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var trs = htmlDoc.DocumentNode.Descendants("tr").ToList();

                // Modelo do Html do site
                //
                //<tr>
                //    <th> Nome </th>
                //    <th> Email </th>
                //    <th> Data </th>
                //    <th> Ação </th>
                //</tr>

                // Indice 0 para obter o nome 
                // Indice 1 para obter o email
                // Indice 2 para obter a data
                // Indice 3 para obter o botão excluir               

                var flux01 = new Thread(() => CountEmails(trs));
                flux01.Name = "Thread-CountEmail";
                flux01.Priority = ThreadPriority.Highest;
                flux01.Start();

                var flux02 = new Thread(() => CountNames(trs));
                flux02.Name = "Thread-CountNames";
                flux02.Priority = ThreadPriority.Lowest;
                flux02.Start();

                Console.WriteLine("*** Fim do cliente HTTP ***");
            }
        }

        private static void CountEmails(List<HtmlNode> trs)
        {
            Console.WriteLine($"Nome da thread: [{Thread.CurrentThread.Name}] | Prioridade: [{Thread.CurrentThread.Priority}]");

            int count = 0;

            foreach (var tr in trs)
            {
                String email = tr.ChildNodes[1].InnerText;

                if (email.Contains("@"))
                {
                    count++;
                }
            }

            Console.WriteLine($"Quantidade de e-mails encontrados: [{count}]");
        }

        private static void CountNames(List<HtmlNode> trs)
        {
            Console.WriteLine($"Nome da thread: [{Thread.CurrentThread.Name}] | Prioridade: [{Thread.CurrentThread.Priority}]");

            int count = 0;

            foreach (var tr in trs)
            {
                String name = tr.ChildNodes[0].InnerText.Trim();

                if (!name.Equals(""))
                {
                    count++;
                }
            }

            Console.WriteLine($"Quantidade de nomes encontrados: [{count}]");
        }
    }
}