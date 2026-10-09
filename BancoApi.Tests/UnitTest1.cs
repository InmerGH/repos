using BancoApi.Data;
using BancoApi.DTOs;
using BancoApi.Models;
using BancoApi.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
namespace BancoApi.Tests
{
    public class CuentaServiceTests
    {
        private BancoDbContext ObtenerDbContextInMemory()
        {
            var options = new DbContextOptionsBuilder<BancoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            return new BancoDbContext(options);
        }
        [Fact]
        public async Task Retirar_SinFondosSuficientes_DebeRechazarTransaccion()
        {
            // 1. Preparar (Arrange)
            using var context = ObtenerDbContextInMemory();
            context.Cuentas.Add(new CuentaBancaria
            {
                Id = 1,
                NumeroCuenta = "CTA-TEST",
                Saldo = 100,
                ClienteId =
            1
            });
            await context.SaveChangesAsync();
            var service = new CuentaService(context);
            // 2 y 3. Actuar y Validar (Act & Assert)
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RetirarAsync(new TransaccionRequest("CTA-TEST", 500)));
        }
        [Fact]
        public async Task AplicarInteres_DebeAumentarSaldoCorrectamente()
        {
            using var context = ObtenerDbContextInMemory();
            context.Cuentas.Add(new CuentaBancaria
            {
                Id = 1,
                NumeroCuenta = "CTA-TEST",
                Saldo = 1000,
                ClienteId =
            1
            });
            await context.SaveChangesAsync();
            var service = new CuentaService(context);
            var resultado = await service.AplicarInteresAsync(new AplicarInteresRequest("CTA-TEST", 10)); // 10%
            Assert.Equal(1100, resultado.SaldoFinal);
        }
    }
}
