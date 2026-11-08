using JewelryInventory.Api.Validation.Components;

namespace JewelryInventory.Tests.Validation.Components;

public class ComponentValidationHandlerTests
{
    private class SpyHandler : ComponentValidationHandler
    {
        public bool Called { get; set; }
        public Exception? ToThrow { get; set; }

        protected override Task ValidateAsync(ComponentValidationContext context)
        {
            Called = true;
            if (ToThrow is not null) throw ToThrow;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task HandleAsync_HandlerSucceeds_CallsNextHandler()
    {
        var first = new SpyHandler();
        var second = new SpyHandler();
        first.SetNext(second);

        await first.HandleAsync(new ComponentValidationContext());
        
        Assert.True(second.Called);
    }

    [Fact]
    public async Task HandleAsync_HandlerThrows_DoesNotCallNextHandler()
    {
        var first = new SpyHandler { ToThrow = new ArgumentException("x") };
        var second = new SpyHandler();
        first.SetNext(second);

        await Assert.ThrowsAsync<ArgumentException>(() => first.HandleAsync(new ComponentValidationContext()));
        
        Assert.False(second.Called);
    }
}