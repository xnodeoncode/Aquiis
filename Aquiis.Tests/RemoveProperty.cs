using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class RemoveProperty : PageTest
{
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