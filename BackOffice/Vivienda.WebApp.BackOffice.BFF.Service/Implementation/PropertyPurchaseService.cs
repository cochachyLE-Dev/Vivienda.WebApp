using Polly;
using Polly.CircuitBreaker;

using System.Diagnostics;
using System.Text;
using System.Text.Json;

using Vivienda.WebApp.BackOffice.BFF.Service.Contract;
using Vivienda.WebApp.BackOffice.BFF.Service.Exceptions;

namespace Vivienda.WebApp.BackOffice.BFF.Service.Implementation
{
    internal class PropertyPurchaseService:IPropertyPurchaseService
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        public PropertyPurchaseService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _circuitBreakerPolicy = Policy
                .Handle<ApiException>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (ex, breakDelay) => Debug.WriteLine($"Open circuit for {breakDelay.TotalSeconds} seconds due to: {ex.Message}"),
                    onReset: () => Debug.WriteLine("Closed circuit. Applications are resuming."),
                    onHalfOpen: () => Debug.WriteLine("Circuit in medium state. One test request is allowed.")
                );
        }
        public async Task<TResponse> PurchaseOrder<TRequest,TResponse>(string url, TRequest content)
        {            
            return await _circuitBreakerPolicy.ExecuteAsync(async () => 
            {
                var httpContent = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, httpContent);

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    throw new BadRequestException("Bad request error.");

                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    throw new ApiException($"Internal server error occurred during sending purchase order.");

                if(!response.IsSuccessStatusCode)
                    throw new ApiException($"Error ocurred with status code: {response.StatusCode}");

                var dataStr = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<TResponse>(dataStr)!;

                return data;
            });
        }
        /// <summary>
        /// Calculadora de Hipoteca, para estimar los pagos mensuales de una hipoteca.        
        /// </summary>
        /// <param name="loanAmount">Monto del préstamo.</param>
        /// <param name="interestRate">Tasa de interés anual.</param>
        /// <param name="loanTerm">Duración del préstamo en años.</param>
        /// <returns></returns>
        public (decimal MonthlyPayment, decimal TotalCost) MortgageCalculator(decimal loanAmount, decimal interestRate, int loanTerm)
        {
            // Calcular el pago mensual y el costo total
            decimal monthlyPayment = CalculateMonthlyPayment();
            decimal totalCost = CalculateTotalCost();

            return (monthlyPayment, totalCost);

            // Calcula el pago mensual
            decimal CalculateMonthlyPayment()
            {
                // Convertir la tasa de interés anual en tasa de interés mensual (dividida por 12 y por 100 para convertir de porcentaje a decimal)
                decimal monthlyInterestRate = (interestRate / 100) / 12;

                // Calcular el número total de pagos (meses)
                int totalPayments = loanTerm * 12;

                // Formula de amortización
                decimal monthlyPayment = loanAmount * (monthlyInterestRate * (decimal)Math.Pow(1 + (double)monthlyInterestRate, totalPayments))
                    / (decimal)(Math.Pow(1 + (double)monthlyInterestRate, totalPayments) - 1);

                return monthlyPayment;
            }
            // Calcula el costo total del préstamo (suma de todos los pagos mensuales)
            decimal CalculateTotalCost()
            {
                // Costo total del préstamo (pagos mensuales * número de pagos)
                return CalculateMonthlyPayment() * loanTerm * 12;
            }
        }
        /// <summary>
        /// Calculadora de financiación, calcula la cantidad máxima que el usuario puede pedir prestado según su capacidad de pago mensual y tasa de interés.
        /// </summary>
        /// <param name="monthlyIncome">Ingresos mensuales.</param>
        /// <param name="monthlyExpenses">Gastos mensuales.</param>
        /// <param name="interestRate">Tasa de interés anual.</param>
        /// <param name="loanTerm">Duración del préstamo en años.</param>
        /// <param name="debtToIncomeRatio">Porcentaje de deuda permitido, algunos bancos permiten un cierto porcentaje de los ingresos que se pueden destinar a pagar deudas (por ejemplo, 30%-40%).</param>
        /// <returns></returns>
        public (decimal MaxLoanAmount, decimal MaxMonthlyPayment) FinancingCalculator(decimal monthlyIncome, decimal monthlyExpenses, decimal interestRate, int loanTerm, decimal debtToIncomeRatio)
        {
            decimal maxLoanAmount = CalculateMaxLoanAmount();
            decimal maxMonthlyPayment = CalculateMaxMonthlyPayment();

            return (maxLoanAmount, maxMonthlyPayment);

            decimal CalculateMaxLoanAmount()
            {
                // Calcular la capadidad de pago mensual del usuario
                decimal availableIncome = monthlyIncome * debtToIncomeRatio - monthlyExpenses;
                if (availableIncome <= 0)
                {
                    throw new Exception("There is not enough income after expenses to qualify for a loan.");
                }

                // Convertir la tasa de interés anual en tasa de interés mensual
                decimal monthlyInterestRate = (interestRate / 100) / 12;

                // Calcular en número total de pagos (meses)
                int totalPayments = loanTerm * 12;

                // Calcular el monto máximo del préstamo usando la fórmula de amortización inversa
                decimal maxLoanAmount = availableIncome * (decimal)(Math.Pow(1 + (double)monthlyInterestRate, totalPayments) - 1)
                    / (monthlyInterestRate * (decimal)Math.Pow(1 + (double)monthlyInterestRate, totalPayments));

                return maxLoanAmount;
            }
            decimal CalculateMaxMonthlyPayment()
            {
                // La cantidad máxima que el usuario puede destinar al pago mensual
                return monthlyIncome * debtToIncomeRatio - monthlyExpenses;
            }
        }
    }
}
