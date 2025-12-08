using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;

namespace Aquiis.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class PropertyManagementTests : PageTest
{
    private const string BaseUrl = "http://localhost:5197";
    private const int KeepBrowserOpenSeconds = 30; // Set to 0 to close immediately
    
    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true,
            BaseURL = BaseUrl,
            RecordVideoDir = Path.Combine(Directory.GetCurrentDirectory(), "test-videos"),
            RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
        };
    }

    [Test]
    public async Task CanAddProperty()
    {
        await Page.GotoAsync("http://localhost:5197/");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Sign In" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("Owner1@aquiis.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Today123");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).PressAsync("Enter");
        
        // Wait for login to complete
        await Page.WaitForSelectorAsync("text=Dashboard");
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Properties" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Add Property" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter property address" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter property address" }).FillAsync("3535 Delaney");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter property address" }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "e.g., Apt 2B, Unit" }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "City" }).FillAsync("Houston");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "City" }).PressAsync("Tab");
        await Page.Locator("select[name=\"propertyModel.State\"]").SelectOptionAsync(new[] { "TX" });
        await Page.Locator("select[name=\"propertyModel.State\"]").PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "#####-####" }).FillAsync("77066");
        await Page.Locator("select[name=\"propertyModel.PropertyType\"]").SelectOptionAsync(new[] { "House" });
        await Page.GetByPlaceholder("0.00").ClickAsync();
        await Page.GetByPlaceholder("0.00").FillAsync("1500");
        await Page.Locator("input[name=\"propertyModel.Bedrooms\"]").ClickAsync();
        await Page.Locator("input[name=\"propertyModel.Bedrooms\"]").FillAsync("4");
        await Page.Locator("input[name=\"propertyModel.Bedrooms\"]").PressAsync("Tab");
        await Page.Locator("input[name=\"propertyModel.Bathrooms\"]").FillAsync("4.5");
        await Page.Locator("input[name=\"propertyModel.Bathrooms\"]").PressAsync("Tab");
        await Page.Locator("input[name=\"propertyModel.SquareFeet\"]").FillAsync("1700");
        await Page.Locator("input[name=\"propertyModel.SquareFeet\"]").PressAsync("Tab");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Create Property" }).ClickAsync();
        
        // Verify property was created successfully
        await Page.WaitForSelectorAsync("h1:has-text('Properties')");
        await Expect(Page.GetByText("3535 Delaney").First).ToBeVisibleAsync();
    }

     [Test]
    public async Task CanRemoveProperty()
    {
        await Page.GotoAsync("http://localhost:5197/");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Sign In" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("owner1@aquiis.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Today123");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).PressAsync("Enter");

        // Wait for login to complete
        await Page.WaitForSelectorAsync("text=Dashboard");

        await Page.GetByRole(AriaRole.Link, new() { Name = "Properties" }).ClickAsync();

        // Wait for properties page to load
        await Page.WaitForSelectorAsync("h1:has-text('Properties')");

        // Find the property "3535 Delaney" and click its delete button
        await Page.Locator("[id^='property-']", new() { HasText = "3535 Delaney" })
            .GetByTitle("Delete").First.ClickAsync();
        
        // Confirm deletion
        //await Page.GetByRole(AriaRole.Button, new() { Name = "Delete", Exact = true }).ClickAsync();
        
        // Verify property was deleted
        //await Expect(Page.GetByText("3535 Delaney")).Not.ToBeVisibleAsync();
    }
}
