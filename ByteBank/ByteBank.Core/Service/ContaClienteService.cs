﻿using ByteBank.Core.Model;
using System;
using System.Threading;

namespace ByteBank.Core.Service
{
    public class ContaClienteService
    {
        public string ConsolidarMovimentacao(ContaCliente conta) 
            => ConsolidarMovimentacao(conta, CancellationToken.None);

        private static decimal FatorDeMultiplicacao(DateTime dataMovimento)
            => FatorDeMultiplicacao(dataMovimento, CancellationToken.None);

        public string ConsolidarMovimentacao(ContaCliente conta, CancellationToken ct)
        {
            var soma = 0m;

            foreach (var movimento in conta.Movimentacoes)
            {
                ct.ThrowIfCancellationRequested();
                soma += movimento.Valor * FatorDeMultiplicacao(movimento.Data,ct);
            }

            ct.ThrowIfCancellationRequested();
            AtualizarInvestimentos(conta);
            return $"Cliente {conta.NomeCliente} tem saldo atualizado de R${soma.ToString("#00.00")}";
        }

        private static decimal FatorDeMultiplicacao(DateTime dataMovimento,CancellationToken ct)
        {
            const decimal CTE_FATOR = 1.0000000005m;

            var diasCorridosDesdeDataMovimento = (dataMovimento - new DateTime(1900, 1, 1)).Days;
            var resultado = 1m;

            for (int i = 0; i < diasCorridosDesdeDataMovimento * 2; i++)
            {
                ct.ThrowIfCancellationRequested();
                resultado = resultado * CTE_FATOR;
            }

            return resultado;
        }
        private static void AtualizarInvestimentos(ContaCliente cliente)
        {
            const decimal CTE_BONIFICACAO_MOV = 1m / (10m * 5m);
            cliente.Investimento *= CTE_BONIFICACAO_MOV;
        }
    }
}