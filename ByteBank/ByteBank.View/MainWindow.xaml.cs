using ByteBank.Core.Model;
using ByteBank.Core.Repository;
using ByteBank.Core.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ByteBank.View
{
    public partial class MainWindow : Window
    {
        private readonly ContaClienteRepository r_Repositorio;
        private readonly ContaClienteService r_Servico;
        private CancellationTokenSource _cts;
        private ObservableCollection<string> _resultado = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();
            LstResultados.DataContext = this;
            r_Repositorio = new ContaClienteRepository();
            r_Servico = new ContaClienteService();
            LstResultados.ItemsSource = _resultado;
        }
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            BtnCencelar.IsEnabled = false;
            _cts.Cancel();
        }

        private async void BtnProcessar_Click(object sender, RoutedEventArgs e)
        {
            BtnProcessar.IsEnabled = false;
            var contas = r_Repositorio.GetContaClientes();
            _cts = new CancellationTokenSource();
            PgsProgresso.Maximum = contas.Count();

            LimparView();

            var inicio = DateTime.Now;

            BtnCencelar.IsEnabled = true;
            
            var progress = new Progress<string>(str =>
            {
                PgsProgresso.Value++;
                _resultado.Add(str);
            });

            try
            {
                var resultado = await ConsolidarContas(contas, progress, _cts.Token);

                var fim = DateTime.Now;
                AtualizarView(resultado, fim - inicio);
            }
            catch (OperationCanceledException)
            {
                TxtTempo.Text = "Operação cancelada pelo usuário";
            }
            finally
            {
                BtnProcessar.IsEnabled = true;
                BtnCencelar.IsEnabled = false;
            }
        }

        private async Task<int> ConsolidarContas(IEnumerable<ContaCliente> contas, IProgress<string> reportProgressUser, CancellationToken ct)
        {
            var totalProcessado = 0;
            var tasks = contas.Select(conta =>
                Task.Factory.StartNew(() =>
                {
                    ct.ThrowIfCancellationRequested();
                    var resultadoConsolidacao = r_Servico.ConsolidarMovimentacao(conta,ct);
                    ct.ThrowIfCancellationRequested();
                    reportProgressUser.Report(resultadoConsolidacao);
                    totalProcessado++;
                },ct));

            await Task.WhenAll(tasks);
            return totalProcessado;
        }

        private void LimparView()
        {
            _resultado.Clear();
            TxtTempo.Text = null;
            PgsProgresso.Value = 0;
        }

        private void AtualizarView(int totalProcessado, TimeSpan elapsedTime)
        {
            var tempoDecorrido = $"{ elapsedTime.Seconds }.{ elapsedTime.Milliseconds} segundos!";
            TxtTempo.Text = $"Processamento de {totalProcessado} clientes em {tempoDecorrido}";
        }
    }
}
